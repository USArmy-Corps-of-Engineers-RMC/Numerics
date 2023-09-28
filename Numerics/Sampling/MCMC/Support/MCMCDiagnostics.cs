using Numerics.Data.Statistics;
using Numerics.Distributions;
using Numerics.Mathematics.Optimization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Numerics.Sampling.MCMC
{
    /// <summary>
    /// A class for assessing Bayesian MCMC convergence diagnostics.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public class MCMCDiagnostics
    {

        /// <summary>
        /// Compute the effective sample size.
        /// </summary>
        /// <param name="series">The series of posterior samples to evaluate.</param>
        public static double EffectiveSampleSize(IList<double> series)
        {
            //https://www.rdocumentation.org/packages/LaplacesDemon/versions/16.1.4/topics/ESS
            int N = series.Count;
            var acf = Autocorrelation.Function(series, (int)Math.Ceiling((double)N / 2));
            double rho = 0;
            for (int i = 1; i < acf.GetLength(0); i++)
            {
                if (acf[i, 1] < 0.05) break;
                rho += acf[i, 1];
            }
            return Math.Min(N / (1d + 2d * rho), N);
        }

        /// <summary>
        /// The Gelman-Rubin diagnostic.
        /// </summary>
        /// <remarks>
        /// <para>
        ///     The Gelman-Rubin diagnostic tests for lack of convergence by comparing the variance between multiple chains
        ///     to the variance within each chain. If convergence has been achieved, the between-chain and within-chain
        ///     variances should be identical. To be most effective in detecting evidence for non convergence, each chain should
        ///     have been initialized to starting values that are dispersed relative to the target distribution.
        /// </para>
        /// </remarks>
        /// <param name="markovChains">The list of Markov Chains to be evaluated. The chains must be of equal length.</param>
        /// <param name="warmupIterations">The number of warm up MCMC iterations to discard at the beginning of the chains.</param>
        public static double[] GelmanRubin(IList<List<ParameterSet>> markovChains, int warmupIterations = 0)
        {

            int i, j, k;
            // Get number of chains
            int M = markovChains.Count;
            // Get the minimum number of iterations
            int N = int.MaxValue;
            for (i = 0; i < M; i++)
                if (markovChains[i].Count < N) N = markovChains[i].Count;
            N -= warmupIterations;
            // Get number of parameters
            int P = markovChains[0][0].Values.Length;

            // Validation checks
            if (M < 2)
            {
                var result = new double[P];
                result.Fill(double.NaN);
                return result;
                //throw new ArgumentOutOfRangeException(nameof(markovChains), "There must be at least two chains in the chain list.");
            }

                
            if (N < 2) throw new ArgumentOutOfRangeException(nameof(markovChains), "There must be at least two iterations to evaluate.");
            if (P < 1) throw new ArgumentOutOfRangeException(nameof(markovChains), "There must be at least one parameter to evaluate.");
            if (warmupIterations < 0) throw new ArgumentOutOfRangeException(nameof(warmupIterations), "The warm up iterations must be non-negative.");
            if (warmupIterations == 0) warmupIterations = 1;

            // Gelman-Rubin array
            var GR = new double[P];

            // Compute GR or each parameter
            for (i = 0; i < P; i++)
            {
                // Compute between- and within-chain mean
                var mean_chain = new double[M];
                double mean_all = 0;         
                for (j = 0; j < M; j++)
                {
                    for (k = warmupIterations - 1; k < N; k++)
                        mean_chain[j] += markovChains[j][k].Values[i];
                    // Get within-chain mean
                    mean_chain[j] /= N;
                    mean_all += mean_chain[j];
                }
                // Get between-chain mean
                mean_all /= M;

                // Compute between- and within-chain variance
                var var_chain = new double[M];
                double B = 0, W = 0;
                for (j = 0; j < M; j++)
                {
                    for (k = warmupIterations - 1; k < N; k++)
                        var_chain[j] += Tools.Sqr(markovChains[j][k].Values[i] - mean_chain[j]);
                    // within-chain variance
                    var_chain[j] *= 1d / (N - 1);
                    W += var_chain[j];
                    // between-chain variance
                    B += Tools.Sqr(mean_chain[j] - mean_all);
                }
                // Set between- and within-chain variance
                W /= M;
                B *= (double)N / (M - 1);

                // Compute the pooled variance
                double V = ((N - 1) * W + B) / N;

                // Compute Rhat
                GR[i] = Math.Sqrt(V / W);
            }

            return GR;
        }

        /// <summary>
        /// Computes the minimum sample size rounded to the nearest 100 based on the Raftery-Lewis method.
        /// </summary>
        /// <param name="quantile">The posterior quantile of interest; e.g., 0.975.</param>
        /// <param name="tolerance">The acceptable tolerance for this quantile; e.g., ±0.005.</param>
        /// <param name="probability">Probability of being within the range of tolerance; e.g., 0.95.</param>
        /// <returns>The minimum sample size as an integer, rounded to the nearest thousands place.</returns>
        public static int MinimumSampleSize(double quantile, double tolerance, double probability)
        {
            double q = quantile;
            double r = tolerance;
            double s = probability;
            double N = (q * (1d - q) * Math.Pow(Normal.StandardZ(0.5d * (s + 1d)), 2d)) / Math.Pow(r, 2d);
            int Nmin = (int)Math.Round(decimal.Parse(N.ToString()) / 100M, 0) * 100;
            return Nmin;
        }

    }

   
}
