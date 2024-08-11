/**
* NOTICE:
* The U.S. Army Corps of Engineers, Risk Management Center (USACE-RMC) makes no guarantees about
* the results, or appropriateness of outputs, obtained from Numerics.
*
* LIST OF CONDITIONS:
* Redistribution and use in source and binary forms, with or without modification, are permitted
* provided that the following conditions are met:
* ● Redistributions of source code must retain the above notice, this list of conditions, and the
* following disclaimer.
* ● Redistributions in binary form must reproduce the above notice, this list of conditions, and
* the following disclaimer in the documentation and/or other materials provided with the distribution.
* ● The names of the U.S. Government, the U.S. Army Corps of Engineers, the Institute for Water
* Resources, or the Risk Management Center may not be used to endorse or promote products derived
* from this software without specific prior written permission. Nor may the names of its contributors
* be used to endorse or promote products derived from this software without specific prior
* written permission.
*
* DISCLAIMER:
* THIS SOFTWARE IS PROVIDED BY THE U.S. ARMY CORPS OF ENGINEERS RISK MANAGEMENT CENTER
* (USACE-RMC) "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
* THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL USACE-RMC BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
* SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
* PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
* INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
* LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
* THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
* **/

using Numerics.Mathematics;
using Numerics.Mathematics.Optimization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace Numerics.Sampling.MCMC
{
    /// <summary>
    /// A class for post-processing and saving Bayesian MCMC results.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     <b> Authors: </b>
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
            MarkovChains = new List<ParameterSet>[sampler.NumberOfChains];        
            for (int i = 0; i < sampler.NumberOfChains; i++)
            {
                MarkovChains[i] = sampler.MarkovChains[i].ToList();
            }
            AcceptanceRates = sampler.AcceptanceRates.ToArray();
            MeanLogLikelihood = sampler.MeanLogLikelihood.ToList();
            Output = sampler.Output.ToList();
            MAP = sampler.MAP.Clone();
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
                int N = Output.Count;
                var x = new List<double>();
                for (int j = 0; j < N; j++)
                {
                    x.Add(Output[j].Values[i]);
                }
                // Get ACF
                var acf = Fourier.Autocorrelation(x, (int)Math.Ceiling((double)N / 2));
                var clippedACF = new double[51, 2];
                for (int k = 0; k < acf.GetLength(0); k++)
                {
                    if (k > 50) break;
                    clippedACF[k, 1] = acf[k, 1];
                }
                // Get ESS
                double rho = 0;
                for (int k = 1; k < acf.GetLength(0); k++)
                {
                    if (acf[k, 1] < 0.05) break;
                    rho += acf[k, 1];
                }
                double ess = Math.Min(N / (1d + 2d * rho), N);

                ParameterResults[i] = new ParameterResults(x, alpha);
                ParameterResults[i].SummaryStatistics.Rhat = GR[i];
                ParameterResults[i].SummaryStatistics.ESS = ess;
                ParameterResults[i].Autocorrelation = clippedACF;
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
