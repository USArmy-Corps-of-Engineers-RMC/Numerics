using Numerics.Data.Statistics;
using Numerics.Distributions;
using Numerics.Mathematics;
using System;
using System.Collections.Generic;

namespace Numerics.Sampling.MCMC
{

    /// <summary>
    /// A class for saving Bayesian MCMC results for each parameter.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    [Serializable]
    public class ParameterResults
    {

        /// <summary>
        /// Constructs new parameter results.
        /// </summary>
        /// <param name="values">List of posterior parameter values, aggregated together from each chain.</param>
        /// <param name="alpha">The confidence level; Default = 0.1, which will result in the 90% confidence intervals.</param>
        public ParameterResults(IList<double> values, double alpha = 0.1)
        {
            // Create Kernel Density Estimate
            var kde = new KernelDensity(values);
            KernelDensity = kde.CreatePDFGraph();

            // Set summary statistics
            SummaryStatistics = new ParameterStatistics();
            SummaryStatistics.N = values.Count;
            SummaryStatistics.Mean = kde.Mean;
            SummaryStatistics.StandardDeviation = kde.StandardDeviation;
            SummaryStatistics.Median = kde.Median;
            SummaryStatistics.LowerCI = kde.InverseCDF(alpha / 2d);
            SummaryStatistics.UpperCI = kde.InverseCDF(1d - alpha / 2d);

            // Create Histogram
            Histogram = new Histogram(values);

        }

        /// <summary>
        /// Parameter summary statistics.
        /// </summary>
        public ParameterStatistics SummaryStatistics { get; private set; }

        /// <summary>
        /// The kernel density results.
        /// </summary>
        public double[,] KernelDensity { get; private set; }

        /// <summary>
        /// The histogram results.
        /// </summary>
        public Histogram Histogram { get; private set; }

        /// <summary>
        /// The autocorrelation function for each parameter. This is averaged across each chain.
        /// </summary>
        public double[,] Autocorrelation { get; set; }

    }
}
