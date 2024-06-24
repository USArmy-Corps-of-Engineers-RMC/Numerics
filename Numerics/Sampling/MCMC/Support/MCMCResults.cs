using Numerics.Mathematics;
using Numerics.Mathematics.Optimization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace Numerics.Sampling.MCMC
{
    /// <summary>
    /// A class for post-processing and saving Bayesian MCMC results.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    [Serializable]
    public class MCMCResults
    {
        /// <summary>
        /// Constructs an empty MCMC results.
        /// </summary>
        public MCMCResults() { }

        /// <summary>
        /// Constructs and post-processes MCMC results. 
        /// </summary>
        /// <param name="sampler">The MCMC sampler to post-process.</param>
        /// <param name="alpha">The confidence level; Default = 0.1, which will result in the 90% confidence intervals.</param> 
        public MCMCResults(MCMCSampler sampler, double alpha = 0.1)
        {
            MarkovChains = sampler.MarkovChains;
            Output = new List<ParameterSet>();
            for (int i = 0; i < sampler.NumberOfChains; i++)
                Output.AddRange(sampler.Output[i]);

            AcceptanceRates = sampler.AcceptanceRates;
            MeanLogLikelihood = sampler.MeanLogLikelihood;
            MAP = sampler.MAP;
            ProcessParameterResults(sampler, alpha);
        }

        /// <summary>
        /// The list of sampled Markov Chains.
        /// </summary>
        public List<ParameterSet>[] MarkovChains { get; private set; }

        /// <summary>
        /// Output posterior parameter sets. 
        /// </summary>
        public List<ParameterSet> Output { get; private set; }

        /// <summary>
        /// The average log-likelihood across each chain for each iteration.
        /// </summary>
        public List<double> MeanLogLikelihood { get; private set; }

        /// <summary>
        /// The acceptance rate for each chain.
        /// </summary>
        public double[] AcceptanceRates { get; private set; }

        /// <summary>
        /// Parameter results using the output posterior parameter sets.
        /// </summary>
        public ParameterResults[] ParameterResults { get; private set; }

        /// <summary>
        /// The output parameter set that produced the maximum likelihood. 
        /// This is referred to as the maximum a posteriori (MAP). 
        /// </summary>
        public ParameterSet MAP { get; private set; }

        private List<ParameterSet>[] ThinMarkovChains(MCMCSampler sampler)
        {
            // Create Markov Chains
            var markovChains = new List<ParameterSet>[sampler.NumberOfChains];
            for (int i = 0; i < sampler.NumberOfChains; i++)
                markovChains[i] = new List<ParameterSet>();

            Parallel.For(0, sampler.NumberOfChains, (i) =>
            {
                int t = 0;
                for (int j = 0; j < sampler.Iterations; j++)
                {
                    t += sampler.ThinningInterval - 1;
                    markovChains[i].Add(sampler.MarkovChains[i][t].Clone());
                }

            });

            return markovChains;
        }

        private List<ParameterSet> ThinOutput(MCMCSampler sampler)
        {

            // Create Markov Chains
            var outputChains = new List<ParameterSet>[sampler.NumberOfChains];
            for (int i = 0; i < sampler.NumberOfChains; i++)
                outputChains[i] = new List<ParameterSet>();

            int outputIterations = (int)Math.Ceiling(sampler.OutputLength / (double)sampler.NumberOfChains);

            Parallel.For(0, sampler.NumberOfChains, (i) =>
            {
                int t = 0;
                for (int j = 0; j < outputIterations; j++)
                {
                    t += sampler.ThinningInterval - 1;
                    outputChains[i].Add(sampler.Output[i][t].Clone());
                }

            });

            var output = new List<ParameterSet>();
            for (int i = 0; i < sampler.NumberOfChains; i++)
                output.AddRange(outputChains[i]);

            return output;
        }

        /// <summary>
        /// Process the parameter results.
        /// </summary>
        /// <param name="sampler">The MCMC sampler to post-process.</param>
        /// <param name="alpha">The confidence level; Default = 0.1, which will result in the 90% confidence intervals.</param> 
        private void ProcessParameterResults(MCMCSampler sampler, double alpha = 0.1)
        {
            // Compute the Gelman-Rubin diagnostic using the post-warm up period
            var GR = MCMCDiagnostics.GelmanRubin(sampler.MarkovChains, sampler.WarmupIterations);

            // Compute parameter summary statistics
            ParameterResults = new ParameterResults[sampler.NumberOfParameters];
            for (int i = 0; i < sampler.NumberOfParameters; i++)
            {

                // Compute the Autocorrelation Function (ACF) and Effective Sample Size (ESS)
                // Average the ACF and sum the ESS
                var x = new List<double>();
                var avgACF = new double[51, 2];
                double ess = 0;

                for (int j = 0; j < sampler.NumberOfChains; j++)
                {
                    // Get list of parameters
                    var N = sampler.Output[j].Count;
                    var y = new List<double>();
                    for (int k = 0; k < N; k++)
                         y.Add(sampler.Output[j][k].Values[i]);
                    x.AddRange(y);

                    // Get ACF
                    var acf = Fourier.Autocorrelation(y, (int)Math.Ceiling((double)N / 2));
                    for (int k = 0; k < acf.GetLength(0); k++)
                    {
                        if (k > 50) break;
                        avgACF[k, 1] += acf[k, 1] / sampler.NumberOfChains;
                    }
                    // Get ESS
                    double rho = 0;
                    for (int k = 1; k < acf.GetLength(0); k++)
                    {
                        if (acf[k, 1] < 0.05) break;
                        rho += acf[k, 1];                                
                    }
                    ess += Math.Min(N / (1d + 2d * rho), N);              
                }

                ParameterResults[i] = new ParameterResults(x, alpha);
                ParameterResults[i].SummaryStatistics.Rhat = GR[i];
                ParameterResults[i].SummaryStatistics.ESS = ess;
                ParameterResults[i].Autocorrelation = avgACF;
            }
        }

        #region Serialization

        /// <summary>
        /// Converts the MCMC Results to a byte array.
        /// </summary>
        /// <param name="mcmcResults">The MCMC Results.</param>
        public static byte[] ToByteArray(MCMCResults mcmcResults)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, mcmcResults);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Creates MCMC Results from a byte array.
        /// </summary>
        /// <param name="bytes">Byte array.</param>
        public static MCMCResults FromByteArray(byte[] bytes)
        {
            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                ms.Write(bytes, 0, bytes.Length);
                ms.Seek(0L, SeekOrigin.Begin);
                var obj = bf.Deserialize(ms);
                return (MCMCResults)obj;
            }
        }

        #endregion

    }
}
