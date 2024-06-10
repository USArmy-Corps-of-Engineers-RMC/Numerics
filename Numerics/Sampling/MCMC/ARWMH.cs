using Numerics.Distributions;
using Numerics.Mathematics.LinearAlgebra;
using Numerics.Mathematics.Optimization;
using Numerics.Data.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Numerics.Sampling.MCMC
{
    /// <summary>
    /// This class performs Bayesian MCMC using the adaptive random walk Metropolis-Hastings (RWMH) method.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    ///    <see href="https://en.wikipedia.org/wiki/Metropolis%E2%80%93Hastings_algorithm"/>
    /// </para>
    /// </remarks>
    [Serializable]
    public class ARWMH : MCMCSampler
    {

        /// <summary>
        /// Constructs a new RWMH sampler.
        /// </summary>
        /// <param name="priorDistributions">The list of prior distributions for the model parameters.</param>
        /// <param name="logLikelihoodFunction">The Log-Likelihood function to evaluate.</param>      
        public ARWMH(List<IUnivariateDistribution> priorDistributions, LogLikelihood logLikelihoodFunction)
        {
            PriorDistributions = priorDistributions;
            LogLikelihoodFunction = logLikelihoodFunction;
            InitialPopulationLength = 100 * NumberOfParameters;

            // The optimal scale & adaptive covariance matrix
            Scale = 2.38 * 2.38 / NumberOfParameters;
            // The initial scale & identity covariance matrix
            sigmaIdentity = Matrix.Identity(NumberOfParameters) * (0.1 * 0.1 / NumberOfParameters);
            Beta = 0.05;
            CrossoverProbability = 0.1;
        }


        private Matrix sigmaIdentity;
        private RunningCovarianceMatrix[] sigma;
        private MultivariateNormal[] mvn;

        /// <summary>
        /// The scaling parameter used to scale the adaptive covariance matrix.
        /// </summary>
        public double Scale { get; set; }

        /// <summary>
        /// Determines how often to sample from the small identity covariance matrix; e.g., 0.05 will result in sampling 5% of the time.
        /// </summary>
        public double Beta { get; set; }

        /// <summary>
        /// Determines ho ofter to sample from a different chain; e.g., 0.10 will result in sampling from a different chain 10% of the time.
        /// </summary>
        public double CrossoverProbability { get; set; } 

        /// <summary>
        /// The covariance matrix Σ (sigma) for the proposal distribution.
        /// </summary>
        public Matrix[] ProposalSigma
        {
            get
            {
                var sigmas = new Matrix[NumberOfChains];
                for (int i = 0; i < NumberOfChains; i++)
                    sigmas[i] = sigma[i].Covariance * (1d / (sigma[i].N - 1));
                return sigmas;
            }
        }


        /// <summary>
        /// Initialize any custom MCMC sampler settings.
        /// </summary>
        protected override void InitializeCustomSettings()
        {
            // Set up multivariate Normal distributions and 
            // adaptive covariance matrix for each chain
            mvn = new MultivariateNormal[NumberOfChains];
            sigma = new RunningCovarianceMatrix[NumberOfChains];
            for (int i = 0; i < NumberOfChains; i++)
            {
                mvn[i] = new MultivariateNormal(NumberOfParameters);
                sigma[i] = new RunningCovarianceMatrix(NumberOfParameters);

                if (InitializeWithMAP && _MAPsuccessful)
                {
                    // Hot start the covariance matrix
                    for (int j = 0; j < NumberOfParameters; j++)
                    {
                        for (int k = 0; k < NumberOfParameters; k++)
                        {
                            sigma[i].Covariance[j, k] = _mvn.Covariance[j, k];
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns a proposed MCMC iteration. 
        /// </summary>
        /// <param name="index">The Markov Chain zero-based index.</param>
        /// <param name="state">The current chain state to compare against.</param>
        protected override ParameterSet ChainIteration(int index, ParameterSet state)
        {

            // Update the sample count
            SampleCount[index] += 1;

            if (_chainPRNGs[index].NextDouble() <= Beta || SampleCount[index] <= 100 * NumberOfParameters)
            {
                // Use the identity matrix the first 100*D samples
                mvn[index].SetParameters(state.Values.ToArray(), sigmaIdentity.ToArray());
            }
            else
            {
                // Use the adaptive covariance matrix
                mvn[index].SetParameters(state.Values.ToArray(), (sigma[index].Covariance * (1d / (sigma[index].N - 1)) * Scale).ToArray());
            }

            int c1 = index;
            if (SampleCount[index] > 100 * NumberOfParameters && _chainPRNGs[index].NextDouble() <= CrossoverProbability)
            {
                // Sample from a different chain's covariance matrix
                do c1 = _chainPRNGs[index].Next(0, NumberOfChains); while (c1 == index);
            }

            // Get proposal vector
            var xp = mvn[c1].InverseCDF(_chainPRNGs[index].NextDoubles(NumberOfParameters));

            // Check if the parameter is feasible (within the constraints)
            for (int i = 0; i < NumberOfParameters; i++)
            {
                if (xp[i] < PriorDistributions[i].Minimum || xp[i] > PriorDistributions[i].Maximum)
                {
                    // The proposed parameter vector was infeasible, so leave xi unchanged.
                    // Adapt Covariance Matrix after warmup
                    if (SampleCount[index] > ThinningInterval * WarmupIterations)
                        sigma[index].Push(state.Values);
                    return state;
                }
            }

            // Evaluate fitness
            var logLHp = LogLikelihoodFunction(xp);
            var logLHi = state.Fitness;

            // Calculate the Metropolis ratio
            var logRatio = logLHp - logLHi;

            // Accept the proposal with probability min(1,r)
            // otherwise leave xi unchanged
            var logU = Math.Log(_chainPRNGs[index].NextDouble());
            if (logU <= logRatio)
            {
                // The proposal is accepted
                AcceptCount[index] += 1;
                // Adapt Covariance Matrix
                sigma[index].Push(xp);
                return new ParameterSet(xp, logLHp);
            }
            else
            {
                // Adapt Covariance Matrix after warmup
                if (SampleCount[index] > ThinningInterval * WarmupIterations)
                    sigma[index].Push(state.Values);
                return state;
            }
        }

    }
}
