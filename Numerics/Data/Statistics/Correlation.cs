using System;
using System.Collections.Generic;
using System.Linq;

namespace Numerics.Data.Statistics
{

    /// <summary>
    /// Contains methods for determining the correlation of two data sets.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    /// This class contains a shared functions for Pearson and Spearman Correlation Coefficients.
    /// </para>
    /// <para>
    /// References:
    /// </para>
    /// <para>
    /// "Numerical Recipes, Routines and Examples in Basic", J.C. Sprott, Cambridge University Press, 1991.
    /// <see href="https://en.wikipedia.org/wiki/Pearson_correlation_coefficient"/>
    /// <see href="https://en.wikipedia.org/wiki/Spearman%27s_rank_correlation_coefficient"/>
    /// </para>
    /// </remarks>
    public sealed class Correlation
    {

        /// <summary>
        /// Computes the Pearson correlation coefficient.
        /// </summary>
        /// <param name="sample1">Sample data 1.</param>
        /// <param name="sample2">Sample data 2.</param>
        /// <returns>The Pearson correlation coefficient.</returns>
        public static double Pearson(IList<double> sample1, IList<double> sample2)
        {
            // Check if the arrays are the same length
            if (sample2.Count != sample1.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(sample1), "The samples arrays must be the same length.");
            }
            // 
            int i, n = sample1.Count;
            // Find means.
            double ax = 0d;
            double ay = 0d;
            for (i = 0; i <= n - 1; i++)
            {
                ax += sample1[i];
                ay += sample2[i];
            }
            ax /= n;
            ay /= n;
            // Compute the correlation coefficient
            double sxx = 0d, syy = 0d, sxy = 0d;
            double xt, yt;
            for (i = 0; i <= n - 1; i++)
            {
                xt = sample1[i] - ax;
                yt = sample2[i] - ay;
                sxx += xt * xt;
                syy += yt * yt;
                sxy += xt * yt;
            }
            return sxy / Math.Sqrt(sxx * syy);
        }

        /// <summary>
        /// Computes the Spearman ranked correlation coefficient.
        /// </summary>
        /// <param name="sample1">Sample data 1.</param>
        /// <param name="sample2">Sample data 2.</param>
        /// <returns>The Spearman ranked correlation coefficient.</returns>
        public static double Spearman(IList<double> sample1, IList<double> sample2)
        {
            // Check if the arrays are the same length
            if (sample2.Count != sample1.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(sample1), "The sample arrays must be the same length.");
            }

            var rank1 = Statistics.RanksInplace(sample1.ToArray());
            var rank2 = Statistics.RanksInplace(sample2.ToArray());
            return Pearson(rank1, rank2);
        }

        /// <summary>
        /// Computes Kendall's Tau ranked correlation coefficient.
        /// </summary>
        /// <param name="sample1">Sample data 1.</param>
        /// <param name="sample2">Sample data 2.</param>
        /// <returns>The Kendall's Tau ranked correlation coefficient.</returns>
        public static double KendallsTau(IList<double> sample1, IList<double> sample2)
        {
            // Check if the arrays are the same length
            if (sample2.Count != sample1.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(sample1), "The sample arrays must be the same length.");
            }

            int n = sample1.Count;
            int j, k, i = 0, n1 = 0, n2 = 0;
            double aa, a1, a2;
            // Loop over first member of pair and second member.
            for (j = 0; j <= n - 2; j++)
            {
                for (k = j + 1; k <= n - 1; k++)
                {
                    a1 = sample1[j] - sample1[k];
                    a2 = sample2[j] - sample2[k];
                    aa = a1 * a2;
                    if (aa != 0.0d)
                    {
                        // Neither has a tie
                        n1 += 1;
                        n2 += 1;
                        i = aa > 0.0 ? ++i : --i;
                    } else {
                        // One or both arrays have ties.
                        if (a1 != 0.0d) n1 += 1;
                        if (a2 != 0.0d) n2 += 1;
                    }
                }
            }
            return i / (Math.Sqrt(n1) * Math.Sqrt(n2));
        }
      
    }
}