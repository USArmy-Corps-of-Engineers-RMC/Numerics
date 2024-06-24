﻿using Numerics.Data;
using Numerics.Data.Statistics;
using Numerics.Distributions;
using Numerics.Mathematics.Optimization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Numerics.Sampling.MCMC
{
    /// <summary>
    /// This class performs Bayesian inference using the self-normalizing importance sampling (SNIS) method.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    [Serializable]
    public class SNIS : MCMCSampler
    {
        /// <summary>
        /// Constructs an self-normalizing importance sampler.
        /// </summary>
        /// <param name="priorDistributions">The list of prior distributions for the model parameters.</param>
        /// <param name="logLikelihoodFunction">The Log-Likelihood function to evaluate.</param>
        /// <param name="multivariateNormal">Optional. The multivariate Normal distribution is used for importance sampling. If null, naive Monte Carlo is performed.</param>
        public SNIS(List<IUnivariateDistribution> priorDistributions, LogLikelihood logLikelihoodFunction, MultivariateNormal multivariateNormal = null)
        {
            PriorDistributions = priorDistributions;
            LogLikelihoodFunction = logLikelihoodFunction;
            mvn = multivariateNormal;
            useImportanceSampling = multivariateNormal != null ? true : false;

            // Create default settings
            NumberOfChains = 1;
            WarmupIterations = 0;
            ThinningInterval = 1;
            InitialPopulationLength = 1;
            Iterations = 50000;
            OutputLength = 10000;
        }

        private bool useImportanceSampling = false;
        private MultivariateNormal mvn = null;

        /// <summary>
        /// Initialize any custom MCMC sampler settings.
        /// </summary>
        protected override void InitializeCustomSettings()
        {
            if (mvn == null && useImportanceSampling == false && InitializeWithMAP && _MAPsuccessful)
            {
                mvn = (MultivariateNormal)_mvn.Clone();
                useImportanceSampling = true;
            }
        }

        /// <summary>
        /// Returns a proposed MCMC parameter set and its fitness. This method is not needed for Importance sampling.
        /// </summary>
        /// <param name="index">The Markov Chain zero-based index.</param>
        /// <param name="state">The current chain state to compare against.</param>
        protected override ParameterSet ChainIteration(int index, ParameterSet state)
        {
            return new ParameterSet();
        }

        /// <summary>
        /// Validate the sampler settings.
        /// </summary>
        protected override void ValidateSettings()
        {
            if (NumberOfChains != 1) throw new ArgumentException(nameof(InitialPopulationLength), "There can only be 1 chain with this method.");
            if (OutputLength < 100) throw new ArgumentException(nameof(OutputLength), "The output length must be at least 100.");
            if (Iterations < OutputLength) throw new ArgumentException(nameof(Iterations), "The number of iterations cannot be less than the output length.");
            if (WarmupIterations != 0) throw new ArgumentException(nameof(WarmupIterations), "There are no warmup iterations with this method.");
            if (ThinningInterval != 1) throw new ArgumentException(nameof(ThinningInterval), "The thinning interval must be 1 for this method.");
            if (InitialPopulationLength != 1) throw new ArgumentException(nameof(InitialPopulationLength), "The initial population must be 1 for this method.");
            if (mvn != null && mvn.ParametersValid == false)
                throw new ArgumentException(nameof(MultivariateNormal), "The multivariate Normal importance distribution is invalid.");
        }

        /// <summary>
        /// Perform importance sampling.
        /// </summary>
        public override void Sample()
        {
            InitializeChains();
            InitializeCustomSettings();
            ValidateSettings();
            ParallelizeChains = true;
            ResumeSimulation = false;
            CancellationTokenSource = new CancellationTokenSource();

            // Create inputs 
            _masterPRNG = new MersenneTwister(PRNGSeed);
            MarkovChains = new List<ParameterSet>[NumberOfChains];
            var sets = new ParameterSet[Iterations];
            MarkovChains[0] = sets.ToList();
            Output = new List<ParameterSet>[NumberOfChains];
            Output[0] = new List<ParameterSet>();

            // Create sample & accept counts
            AcceptCount = new int[NumberOfChains];
            SampleCount = new int[NumberOfChains];

            // Create mean log-likelihood list
            MeanLogLikelihood = new List<double>();

            // Keeps track of best parameter set
            MAP = new ParameterSet(new double[] { }, double.MinValue, double.MinValue);

            // Create parameter random values
            var rnds = _masterPRNG.NextDoubles(Iterations, NumberOfParameters);

            // Perform sampling
            Parallel.For(0, Iterations, (idx) =>
            {
                // Sample parameters
                var parameters = new double[NumberOfParameters];
                double logLH = 0;
                double weight = 0;
                if (useImportanceSampling == true)
                {
                    parameters = mvn.InverseCDF(rnds.GetRow(idx));
                    logLH = LogLikelihoodFunction(parameters);
                    weight = logLH - mvn.LogPDF(parameters);
                }
                else
                {
                    for (int i = 0; i < NumberOfParameters; i++)
                        parameters[i] = PriorDistributions[i].InverseCDF(rnds[idx, i]);
                    logLH = LogLikelihoodFunction(parameters);
                    weight = logLH;
                }
                // Record sample
                MarkovChains[0][idx] = new ParameterSet(parameters, logLH, weight);
            });
       
            // Get the maximum a posteriori
            for (int i = 0; i < Iterations; i++)
                if (MarkovChains[0][i].Weight > MAP.Weight)
                    MAP = MarkovChains[0][i].Clone();

            // Get the normalization factor
            double max = MAP.Weight;
            double sum = 0;
            for (int i = 0; i < Iterations; i++)
            {
                if (MarkovChains[0][i].Weight != double.MinValue)
                {
                    sum += Math.Exp(MarkovChains[0][i].Weight - max);
                }            
            }               
            double normalization = max + Math.Log(sum);

            // Compute the posterior weights
            Parallel.For(0, Iterations, (idx) => 
            {
                double w = MarkovChains[0][idx].Weight != double.MinValue ? Math.Exp(MarkovChains[0][idx].Weight - normalization) : 0d;
                MarkovChains[0][idx] = new ParameterSet(MarkovChains[0][idx].Values, MarkovChains[0][idx].Fitness, w);
            });

            // Sort list in ascending order of posterior weights
            MarkovChains[0].Sort((x, y) => x.Fitness.CompareTo(y.Fitness));
            var cdf = new double[Iterations];
            cdf[0] = MarkovChains[0][0].Weight;

            // Create CDF
            for (int i = 1; i < Iterations; i++)
                cdf[i] = cdf[i-1] + MarkovChains[0][i].Weight;

            // Record output
            var rndOut = _masterPRNG.NextDoubles(OutputLength);
            Array.Sort(rndOut);
            int idx = 0;
            for (int i = 0; i < OutputLength; i++)
            {
                idx = Search.Hunt(rndOut[i], cdf, idx);
                Output[0].Add(MarkovChains[0][idx].Clone());
            }
        }

    }
}