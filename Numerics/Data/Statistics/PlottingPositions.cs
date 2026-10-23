using System;
using System.Collections.Generic;

namespace Numerics.Data.Statistics
{
    /// <summary>
    /// Contains plotting position formulas.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     <b> Authors: </b>
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    /// <b> Description: </b>
    /// The choice of quantiles from a theoretical distribution can depend upon context and purpose.
    /// Several different formulas have been used or proposed as affine symmetrical plotting positions.
    /// Such formulas have the form (k − a) / (n + 1 − 2a) for some value of a in the range from 0 to 1,
    /// which gives a range between k / (n + 1) and (k − 1) / (n - 1).
    /// </para>
    /// <b> References: </b>
    /// <see href="https://en.wikipedia.org/wiki/Q%E2%80%93Q_plot"/>
    /// </remarks>
    public class PlottingPositions
    {
        /// <summary>
        /// The general plotting position formula. This formula assumes the data is uncensored and complete.
        /// </summary>
        /// <param name="N">The sample size.</param>
        /// <param name="alpha">The alpha coefficient. Range (0,1).</param>
        /// <returns>An array of plotting positions of size N.</returns>
        public static double[] Function(int N, double alpha)
        {
            if (N <= 2)
            {
                throw new ArgumentOutOfRangeException(nameof(N), "The sample size N must be greater than 2.");
            }

            if (alpha < 0d || alpha > 1d)
            {
                throw new ArgumentOutOfRangeException(nameof(alpha), "The alpha coefficient must be between 0 and 1.");
            }

            var PP = new double[N];
            for (int i = 1; i <= N; i++)
                PP[i - 1] = (i - alpha) / (N + 1 - 2d * alpha);
            return PP;
        }

        /// <summary>
        /// The general plotting position formula for weighted observations, for example importance-sampling
        /// weights.
        /// </summary>
        /// <param name="weights">The non-negative, finite weight of each observation, in the ascending order of the
        /// observations' values.</param>
        /// <param name="alpha">The alpha coefficient. Range [0,1].</param>
        /// <returns>An array of non-decreasing plotting positions, one per weight.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="weights"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="weights"/> is empty or every weight is zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when a weight is negative or not finite, or
        /// <paramref name="alpha"/> is outside [0,1].</exception>
        /// <remarks>
        /// <para>
        /// With c_i the cumulative weight through observation i, W the total weight and w̄ = W / n the mean
        /// weight, the position of observation i is F_i = (c_i − α w_i) / (W + w̄ − 2α w̄), which is
        /// (c_i − α w_i) / (W + (1 − 2α) w̄). Unit weights give c_i = i, W = n and w̄ = 1, so every position is
        /// bitwise that of <see cref="Function(int, double)"/>, (i − α) / (n + 1 − 2α). A zero-weight observation
        /// takes the position of its predecessor.
        /// </para>
        /// <para>
        /// The cumulative and total weights are sequential sums in the order given, the total being the last
        /// cumulative weight.
        /// </para>
        /// </remarks>
        public static double[] Function(IList<double> weights, double alpha)
        {
            if (weights == null) throw new ArgumentNullException(nameof(weights));
            if (weights.Count == 0) throw new ArgumentException("At least one weight is required.", nameof(weights));
            if (double.IsNaN(alpha) || alpha < 0d || alpha > 1d)
                throw new ArgumentOutOfRangeException(nameof(alpha), "The alpha coefficient must be between 0 and 1.");

            int n = weights.Count;
            var cumulative = new double[n];
            double run = 0d;
            for (int i = 0; i < n; i++)
            {
                double weight = weights[i];
                if (double.IsNaN(weight) || double.IsInfinity(weight) || weight < 0d)
                    throw new ArgumentOutOfRangeException(nameof(weights), "Each weight must be finite and non-negative.");
                run += weight;
                cumulative[i] = run;
            }
            double total = run;
            if (total <= 0d) throw new ArgumentException("The weights must not all be zero.", nameof(weights));

            double mean = total / n;
            double denominator = total + mean - 2d * alpha * mean;
            var PP = new double[n];
            for (int i = 0; i < n; i++)
                PP[i] = (cumulative[i] - alpha * weights[i]) / denominator;
            return PP;
        }

        /// <summary>
        /// The general plotting position formula. This formula assumes the data is uncensored and complete.
        /// </summary>
        /// <param name="N">The sample size.</param>
        /// <param name="plottingPostionType">The plotting position formula type.</param>
        /// <returns>An array of plotting positions of size N.</returns>
        public static double[]? Function(int N, PlottingPositions.PlottingPostionType plottingPostionType)
        {
            if (plottingPostionType == PlottingPostionType.Blom)
            {
                return Blom(N);
            }
            else if (plottingPostionType == PlottingPostionType.Cunnane)
            {
                return Cunnane(N);
            }
            else if (plottingPostionType == PlottingPostionType.Gringorten)
            {
                return Gringorten(N);
            }
            else if (plottingPostionType == PlottingPostionType.Hazen)
            {
                return Hazen(N);
            }
            else if (plottingPostionType == PlottingPostionType.KaplanMeier)
            {
                return KaplanMeier(N);
            }
            else if (plottingPostionType == PlottingPostionType.Median)
            {
                return Median(N);
            }
            else if (plottingPostionType == PlottingPostionType.Weibull)
            {
                return Weibull(N);
            }
            return null;
        }

        /// <summary>
        /// The Weibull plotting position formula (alpha = 0.0). Recommended for uniform distribution.
        /// </summary>
        /// <param name="N">The sample size.</param>
        /// <returns>An array of plotting positions of size N.</returns>
        public static double[] Weibull(int N)
        {
            return Function(N, 0d);
        }

        /// <summary>
        /// The Median plotting position formula (alpha = 0.3175). Provides median exceedance probabilities for all distributions.
        /// </summary>
        /// <param name="N">The sample size.</param>
        /// <returns>An array of plotting positions of size N.</returns>
        public static double[] Median(int N)
        {
            return Function(N, 0.3175d);
        }

        /// <summary>
        /// The Blom (1958) plotting position formula (alpha = 0.375). Recommended for Normal, Gamma, 2-parameter Log Normal,
        /// 3-parameter Log Normal, and Log Pearson Type III distributions.
        /// </summary>
        /// <param name="N">The sample size.</param>
        /// <returns>An array of plotting positions of size N.</returns>
        public static double[] Blom(int N)
        {
            return Function(N, 0.375d);
        }

        /// <summary>
        /// The Cunnane (1978) plotting position formula (alpha = 0.40). Recommended for GEV and Log-Gumbel distributions.
        /// </summary>
        /// <param name="N">The sample size.</param>
        /// <returns>An array of plotting positions of size N.</returns>
        public static double[] Cunnane(int N)
        {
            return Function(N, 0.4d);
        }

        /// <summary>
        /// The Gringorten (1963) plotting position formula (alpha = 0.44). Recommended for Exponential, Gumbel and Weibull distributions.
        /// </summary>
        /// <param name="N">The sample size.</param>
        /// <returns>An array of plotting positions of size N.</returns>
        public static double[] Gringorten(int N)
        {
            return Function(N, 0.44d);
        }

        /// <summary>
        /// The Hazen plotting position formula (alpha = 0.50). Recommended when the parameters of the parent distribution
        /// are unknown.
        /// </summary>
        /// <param name="N">The sample size.</param>
        /// <returns>An array of plotting positions of size N.</returns>
        public static double[] Hazen(int N)
        {
            return Function(N, 0.5d);
        }

        /// <summary>
        /// The Kaplan-Meier plotting position formula for uncensored data (k/n).
        /// </summary>
        /// <param name="N">The sample size.</param>
        /// <returns>An array of plotting positions of size N.</returns>
        public static double[] KaplanMeier(int N)
        {
            if (N <= 2)
            {
                throw new ArgumentOutOfRangeException(nameof(N), "The sample size N must be greater than 2.");
            }
            var PP = new double[N];
            for (int i = 1; i <= N; i++)
                PP[i - 1] = i / (double)N;
            return PP;
        }

        /// <summary>
        /// Enumeration of plotting position types.
        /// </summary>
        public enum PlottingPostionType
        {
            /// <summary>
            /// The Weibull plotting position formula (alpha = 0.0).
            /// </summary>
            Weibull,

            /// <summary>
            /// The Median plotting position formula (alpha = 0.3175).
            /// </summary>
            Median,

            /// <summary>
            /// The Blom (1958) plotting position formula (alpha = 0.375).
            /// </summary>
            Blom,

            /// <summary>
            /// The Cunnane (1978) plotting position formula (alpha = 0.40).
            /// </summary>
            Cunnane,

            /// <summary>
            /// The Gringorten (1963) plotting position formula (alpha = 0.44).
            /// </summary>
            Gringorten,

            /// <summary>
            /// The Hazen plotting position formula (alpha = 0.50).
            /// </summary>
            Hazen,

            /// <summary>
            /// The Kaplan-Meier plotting position formula for uncensored data (k/n).
            /// </summary>
            KaplanMeier
        }
    }
}