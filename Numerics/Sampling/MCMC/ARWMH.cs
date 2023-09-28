using Numerics.Distributions;
using Numerics.Mathematics.LinearAlgebra;
using Numerics.Mathematics.Optimization;
using Numerics.Data.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;

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
            sigmaOpt = 2.38 * 2.38 / NumberOfParameters;

            // The initial scale & identity covariance matrix
            sigmaInit = 0.1 * 0.1 / NumberOfParameters;
            covarId = Matrix.Identity(NumberOfParameters) * sigmaInit;

        }

        private readonly double sigmaInit;
        private readonly double sigmaOpt;
        private readonly Matrix covarId;
        private readonly double beta = 0.05;
        private RunningCovarianceMatrix[] covarAdpt;
        private MultivariateNormal[] mvn;

        /// <summary>
        /// The covariance matrix Σ (sigma) for the proposal distribution.
        /// </summary>
        public Matrix[] ProposalSigma
        {
            get
            {
                var sigmas = new Matrix[NumberOfChains];
                for (int i = 0; i < NumberOfChains; i++)
                    sigmas[i] = covarAdpt[i].Covariance * (1d / (covarAdpt[i].N - 1));
                return sigmas;
            }
        }

        /// <summary>
        /// Initialize any custom MCMC sampler settings.
        /// </summary>
        protected override void InitializeCustomSettings()
        {
            // Set up multivariate Normal distributions and 
            //adaptive covariance matrix for each chain
            mvn = new MultivariateNormal[NumberOfChains];
            covarAdpt = new RunningCovarianceMatrix[NumberOfChains];
            for (int i = 0; i < NumberOfChains; i++)
            {
                mvn[i] = new MultivariateNormal(NumberOfParameters);
                covarAdpt[i] = new RunningCovarianceMatrix(NumberOfParameters);
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

            // Get proposal vector
            if (_chainPRNGs[index].NextDouble() <= beta || SampleCount[index] <= 2 * NumberOfParameters)
            {
                mvn[index].SetParameters(state.Values.ToArray(), covarId.ToArray());          
            }
            else
            {
                // 10% of the time, sample from a different chain's covariance matrix
                int c1 = _chainPRNGs[index].NextDouble() <= 0.1 ? _chainPRNGs[index].Next(0, NumberOfChains) : index;
                mvn[index].SetParameters(state.Values.ToArray(), (covarAdpt[c1].Covariance * (1d / (covarAdpt[c1].N - 1)) * sigmaOpt).ToArray());                          
            }         
            var xp = mvn[index].InverseCDF(_chainPRNGs[index].NextDoubles(NumberOfParameters));

            // Check if the parameter is feasible (within the constraints)
            for (int i = 0; i < NumberOfParameters; i++)
            {
                if (xp[i] < PriorDistributions[i].Minimum || xp[i] > PriorDistributions[i].Maximum)
                {
                    // The proposed parameter vector was infeasible, so leave xi unchanged.
                    // Adapt Covariance Matrix
                    covarAdpt[index].Push(state.Values);
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
                covarAdpt[index].Push(xp);
                return new ParameterSet(xp, logLHp);
            }
            else
            {
                // Adapt Covariance Matrix
                covarAdpt[index].Push(state.Values);
                return state;
            }
        }

    }
}
