using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Numerics.Distributions;

namespace Numerics.Sampling
{
    /// <summary>
    /// A class for stratifying probabilities for sampling, or values for numerical integration. 
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public class Stratify
    {

        /// <summary>
        /// Returns a list of stratified x value bins.
        /// </summary>
        /// <param name="options">The stratification options.</param>
        /// <param name="isLogarithmic">Optional. Determines if the x values should be stratified with a linear or logarithmic (base 10) scale. Default = False.</param>
        public static List<StratificationBin> XValues(StratificationOptions options, bool isLogarithmic = false)
        {
            var bins = new List<StratificationBin>();
            if (options.IsValid == false)
                return bins;
            if (options.IsProbability == true)
                return bins;
            double delta, offset, min, max, xl, xu;
            // 
            if (isLogarithmic == true)
            {
                // first determine the offset
                if (options.LowerBound <= 0d)
                {
                    offset = 1.0d - options.LowerBound;
                }
                else
                {
                    offset = 0.0d;
                }
                // get delta
                min = Math.Log10(options.LowerBound + offset);
                max = Math.Log10(options.UpperBound + offset);
                delta = (max - min) / options.NumberOfBins;
                // stratify first bin
                xl = Math.Pow(10d, min) - offset;
                xu = Math.Pow(10d, Math.Log10(xl + offset) + delta) - offset;
                bins.Add(new StratificationBin(xl, xu));
                // stratify remaining bins
                for (int i = 1; i < options.NumberOfBins; i++)
                {
                    xl = xu;
                    xu = Math.Pow(10d, Math.Log10(xl + offset) + delta) - offset;
                    bins.Add(new StratificationBin(xl, xu));
                }
            }
            else
            {
                // get delta
                delta = (options.UpperBound - options.LowerBound) / options.NumberOfBins;
                // stratify first bin
                xl = options.LowerBound;
                xu = xl + delta;
                bins.Add(new StratificationBin(xl, xu));
                // stratify remaining bins
                for (int i = 1; i < options.NumberOfBins; i++)
                {
                    xl = xu;
                    xu = xl + delta;
                    bins.Add(new StratificationBin(xl, xu));
                }
            }
            // 
            return bins;
        }

        /// <summary>
        /// Returns a list of stratified x value bins.
        /// </summary>
        /// <param name="options">a list of stratification options.</param>
        /// <param name="isLogarithmic">Optional. Determines if the x values should be stratified with a linear or logarithmic (base 10) scale. Default = False.</param>
        public static List<StratificationBin> XValues(List<StratificationOptions> options, bool isLogarithmic = false)
        {
            var sortedOptions = options.ToArray();
            Array.Sort(sortedOptions, (x, y) => x.UpperBound.CompareTo(y.UpperBound));
            var bins = new List<StratificationBin>();
            for (int i = 0; i < sortedOptions.Length; i++)
                bins.AddRange(XValues(sortedOptions[i], isLogarithmic));
            return bins;
        }

        /// <summary>
        /// Returns a list of stratified exceedance probability bins given a list of stratified x value bins and a function to transform x values to exceedance probabilities.
        /// The transform function must convert x values in ascending order to exceedance probabilities in descending order.
        /// </summary>
        /// <param name="xValues">List of stratified x value bins.</param>
        /// <param name="transformFunction">Function to transform x values to exceedance probabilities.
        /// The transform function must convert x values in ascending order to exceedance probabilities in descending order.</param>
        /// <param name="isExhaustive">Determines if the probability bin weights should be collectively exhaustive (i.e., sum to 1).</param>
        public static List<StratificationBin> XToExceedanceProbability(List<StratificationBin> xValues, Func<double, double> transformFunction, bool isExhaustive = true)
        {
            var bins = new List<StratificationBin>();
            if (xValues.Count == 0)
                return bins;
            double xl, xu;
            xu = transformFunction(xValues.First().LowerBound);
            xl = transformFunction(xValues.First().UpperBound);
            bins.Add(new StratificationBin(xl, xu));
            // transform the bins
            for (int i = 1; i < xValues.Count; i++)
            {
                xu = xl;
                xl = transformFunction(xValues[i].UpperBound);
                bins.Add(new StratificationBin(xl, xu));
            }

            if (isExhaustive == true)
            {
                // adjust end bins to ensure exhaustivity
                bins.First().Weight = 1.0d - bins.First().LowerBound;
                bins.Last().Weight = bins.Last().UpperBound;
            }

            return bins;
        }

        /// <summary>
        /// Returns a list of stratified non-exceedance probability bins given a list of stratified x value bins and a function to transform x values to non-exceedance probabilities.
        /// The transform function must convert x values in ascending order to non-exceedance probabilities in ascending order.
        /// </summary>
        /// <param name="xValues">List of stratified x value bins.</param>
        /// <param name="transformFunction">Function to transform x values to non-exceedance probabilities.
        /// The transform function must convert x values in ascending order to non-exceedance probabilities in ascending order.</param>
        /// <param name="isExhaustive">Determines if the probability bin weights should be collectively exhaustive (i.e., sum to 1).</param>
        public static List<StratificationBin> XToProbability(List<StratificationBin> xValues, Func<double, double> transformFunction, bool isExhaustive = true)
        {
            var bins = new List<StratificationBin>();
            double xl, xu, w;
            // transform first bin
            xl = transformFunction(xValues.First().LowerBound);
            xu = transformFunction(xValues.First().UpperBound);
            // determine if the first bin weight should ensure exhaustivity
            if (isExhaustive == true)
            {
                // The weight for the first interval is evaluated 
                // as the non-exceedance probability of its upper bound.
                w = xu;
                bins.Add(new StratificationBin(xl, xu, w));
            }
            else
            {
                bins.Add(new StratificationBin(xl, xu));
            }
            // transform the remaining inner bins
            for (int i = 1; i < xValues.Count - 1; i++)
            {
                xl = xu;
                xu = transformFunction(xValues[i].UpperBound);
                bins.Add(new StratificationBin(xl, xu));
            }
            // transform the last bin
            xl = xu;
            xu = transformFunction(xValues.Last().UpperBound);
            // determine if the last bin weight should ensure exhaustivity
            if (isExhaustive == true)
            {
                // The weight for the last interval is evaluated 
                // as the exceedance probability of its lower bound.
                w = 1.0d - xl;
                bins.Add(new StratificationBin(xl, xu, w));
            }
            else
            {
                bins.Add(new StratificationBin(xl, xu));
            }
            // 
            return bins;
        }

        /// <summary>
        /// Returns a list of stratified probability bins.
        /// </summary>
        /// <param name="options">The stratification options.</param>
        /// <param name="distributionType">Optional. The importance distribution type to stratify with. Default = Uniform.</param>
        /// <param name="isExhaustive">Determines if the probability bin weights should be collectively exhaustive (i.e., sum to 1).</param>
        public static List<StratificationBin> Probabilities(StratificationOptions options, ImportanceDistribution distributionType = ImportanceDistribution.Uniform, bool isExhaustive = true)
        {
            if (options.IsValid == false)
                return null;
            if (options.IsProbability == false)
                return null;
            var bins = new List<StratificationBin>();
            double delta, offset, min, max, pl, pu, xl, xu, w;
            var distribution = default(UnivariateDistributionBase);

            // get distribution
            switch (distributionType)
            {

                case ImportanceDistribution.Gumbel:
                    {
                        distribution = new Gumbel(0d, 1d);
                        break;
                    }

                case ImportanceDistribution.Normal:
                    {
                        distribution = new Normal(0d, 1d);
                        break;
                    }

                case ImportanceDistribution.Uniform:
                    {
                        distribution = new Uniform(0d, 1d);
                        break;
                    }

                case ImportanceDistribution.Log10Uniform:
                    {
                        distribution = new Uniform(0d, 1d);
                        break;
                    }
            }

            if (distributionType == ImportanceDistribution.Log10Uniform)
            {
                // first determine the offset
                offset = 0.0d;
                if (options.LowerBound == 0d)
                    offset = 1.0d;
                // get delta
                min = Math.Log10(distribution.InverseCDF(options.LowerBound) + offset);
                max = Math.Log10(distribution.InverseCDF(options.UpperBound) + offset);
                delta = (max - min) / options.NumberOfBins;
                // stratify first bin
                xl = Math.Pow(10d, min) - offset;
                xu = Math.Pow(10d, Math.Log10(xl + offset) + delta) - offset;
                pl = distribution.CDF(xl);
                pu = distribution.CDF(xu);
                // determine if the first bin weight should ensure exhaustivity
                if (isExhaustive == true)
                {
                    // The weight for the first interval is evaluated 
                    // as the non-exceedance probability of its upper bound.
                    w = pu;
                    bins.Add(new StratificationBin(pl, pu, w));
                }
                else
                {
                    bins.Add(new StratificationBin(pl, pu));
                }
                // stratify inner bins
                for (int i = 1; i < options.NumberOfBins - 1; i++)
                {
                    xl = xu;
                    xu = Math.Pow(10d, Math.Log10(xl + offset) + delta) - offset;
                    pl = distribution.CDF(xl);
                    pu = distribution.CDF(xu);
                    bins.Add(new StratificationBin(pl, pu));
                    // The weight of the inner intervals is simply the 
                    // width of the probability interval under consideration.
                }
                // stratify last bin
                xl = xu;
                xu = Math.Pow(10d, Math.Log10(xl + offset) + delta) - offset;
                pl = distribution.CDF(xl);
                pu = distribution.CDF(xu);
                if (isExhaustive == true)
                {
                    // The weight for the last interval is evaluated 
                    // as the exceedance probability of its lower bound.
                    w = 1.0d - pl;
                    bins.Add(new StratificationBin(pl, pu, w));
                }
                else
                {
                    bins.Add(new StratificationBin(pl, pu));
                }
            }
            else
            {
                // get delta
                min = distribution.InverseCDF(options.LowerBound);
                max = distribution.InverseCDF(options.UpperBound);
                delta = (max - min) / options.NumberOfBins;
                // stratify first bin
                xl = min;
                xu = xl + delta;
                pl = distribution.CDF(xl);
                pu = distribution.CDF(xu);
                // determine if the first bin weight should ensure exhaustivity
                if (isExhaustive == true)
                {
                    // The weight for the first interval is evaluated 
                    // as the non-exceedance probability of its upper bound.
                    w = pu;
                    bins.Add(new StratificationBin(pl, pu, w));
                }
                else
                {
                    bins.Add(new StratificationBin(pl, pu));
                }
                // stratify inner bins
                for (int i = 1; i < options.NumberOfBins - 1; i++)
                {
                    xl = xu;
                    xu = xl + delta;
                    pl = distribution.CDF(xl);
                    pu = distribution.CDF(xu);
                    bins.Add(new StratificationBin(pl, pu));
                    // The weight of the inner intervals is simply the 
                    // width of the probability interval under consideration.
                }
                // stratify last bin
                xl = xu;
                xu = xl + delta;
                pl = distribution.CDF(xl);
                pu = distribution.CDF(xu);
                if (isExhaustive == true)
                {
                    // The weight for the last interval is evaluated 
                    // as the exceedance probability of its lower bound.
                    w = 1.0d - pl;
                    bins.Add(new StratificationBin(pl, pu, w));
                }
                else
                {
                    bins.Add(new StratificationBin(pl, pu));
                }
            }
            // 
            return bins;
        }

        /// <summary>
        /// Returns a multivariate list of stratified probability bins.
        /// </summary>
        /// <param name="options">The stratification options.</param>
        /// <param name="distributionType">Optional. The importance distribution type to stratify with. Default = Uniform.</param>
        /// <param name="isExhaustive">Determines if the probability bin weights should be collectively exhaustive (i.e., sum to 1).</param>
        /// <param name="dimension">The number of dimensions to stratify.</param>
        /// <param name="seed"> Seed for random number generator. </param>
        /// <param name="correlation">The correlation matrix. If null, independence is assummed.</param>
        public static List<List<StratificationBin>> MultivariateProbabilities(StratificationOptions options, ImportanceDistribution distributionType = ImportanceDistribution.Uniform, bool isExhaustive = true, int dimension = 1, int seed = -1, double[,] correlation = null)
        {
            var bins = new List<List<StratificationBin>>();
            var output = new List<List<StratificationBin>>();

            // Create random number generators
            var rnd = seed > 0 ? new MersenneTwister(seed) : new MersenneTwister();
            var mu = new double[dimension];
            for (int i = 0; i < dimension; i++)
            {
                mu[i] = 0;
                bins.Add(Probabilities(options, distributionType, isExhaustive));
                output.Add(new List<StratificationBin>());
            }

            var _mvn = new MultivariateNormal(dimension);
            if (correlation == null)
            {
                correlation = new double[dimension, dimension];
                for (int i = 0; i < dimension; i++)
                    for (int j = 0; j < dimension; j++)
                        correlation[i, j] = i == j ? 1d : 0d;
            }
            _mvn.SetParameters(mu, correlation);
            var rvals = _mvn.LatinHypercubeRandomValues(options.NumberOfBins, seed);

            // Sample random bin without replacement
            for (int row = 0; row < options.NumberOfBins; row++)
            {
                for (int col = 0; col < dimension; col++)
                {
                    int r = (int)Math.Floor(Normal.StandardCDF(rvals[row, col]) * bins[col].Count);
                    output[col].Add((StratificationBin)bins[col][r].Clone());
                    bins[col].RemoveAt(r);
                }
            }

            return output;
        }

        /// <summary>
        /// Returns a list of stratified probability bins.
        /// </summary>
        /// <param name="options">The list of stratification options.</param>
        /// <param name="distributionType">Optional. The importance distribution type to stratify with. Default = Uniform.</param>
        /// <param name="isExhaustive">Determines if the probability bin weights should be collectively exhaustive (i.e., sum to 1).</param>
        public static List<StratificationBin> Probabilities(List<StratificationOptions> options, ImportanceDistribution distributionType = ImportanceDistribution.Uniform, bool isExhaustive = true)
        {
            var sortedOptions = options.ToArray();
            Array.Sort(sortedOptions, (x, y) => x.UpperBound.CompareTo(y.UpperBound));
            var bins = new List<StratificationBin>();
            for (int i = 0; i < sortedOptions.Length; i++)
                bins.AddRange(Probabilities(sortedOptions[i], distributionType, false));
            if (isExhaustive == true)
            {
                // adjust end bins to ensure exhaustivity
                bins.First().Weight = bins.First().UpperBound;
                bins.Last().Weight = 1.0d - bins.Last().LowerBound;
            }

            return bins;
        }

        /// <summary>
        /// Returns a list of stratified exceedance probability bins.
        /// </summary>
        /// <param name="options">The stratification options.</param>
        /// <param name="distributionType">Optional. The importance distribution type to stratify with. Default = Uniform.</param>
        /// <param name="isExhaustive">Determines if the probability bin weights should be collectively exhaustive (i.e., sum to 1).</param>
        public static List<StratificationBin> ExceedanceProbabilities(StratificationOptions options, ImportanceDistribution distributionType = ImportanceDistribution.Uniform, bool isExhaustive = true)
        {
            if (options.IsValid == false)
                return null;
            if (options.IsProbability == false)
                return null;
            var bins = new List<StratificationBin>();
            double delta, offset, min, max, pl, pu, xl, xu, w;
            var distribution = default(UnivariateDistributionBase);

            // get distribution
            switch (distributionType)
            {
                case ImportanceDistribution.Gumbel:
                    {
                        distribution = new Gumbel(0d, 1d);
                        break;
                    }

                case ImportanceDistribution.Normal:
                    {
                        distribution = new Normal(0d, 1d);
                        break;
                    }

                case ImportanceDistribution.Uniform:
                    {
                        distribution = new Uniform(0d, 1d);
                        break;
                    }

                case ImportanceDistribution.Log10Uniform:
                    {
                        distribution = new Uniform(0d, 1d);
                        break;
                    }
            }

            if (distributionType == ImportanceDistribution.Log10Uniform)
            {
                // first determine the offset
                offset = 0.0d;
                if (options.LowerBound == 0d)
                    offset = 1.0d;
                // get delta
                min = Math.Log10(distribution.InverseCDF(options.LowerBound) + offset);
                max = Math.Log10(distribution.InverseCDF(options.UpperBound) + offset);
                delta = (max - min) / options.NumberOfBins;
                // stratify first bin
                xu = Math.Pow(10d, max) - offset;
                xl = Math.Pow(10d, Math.Log10(xu + offset) - delta) - offset;
                pl = distribution.CDF(xl);
                pu = distribution.CDF(xu);
                // determine if the first bin weight should ensure exhaustivity
                if (isExhaustive == true)
                {
                    // The weight for the first interval is evaluated 
                    // as the non-exceedance probability of its lower bound.
                    w = 1.0d - pl;
                    bins.Add(new StratificationBin(pl, pu, w));
                }
                else
                {
                    bins.Add(new StratificationBin(pl, pu));
                }
                // stratify inner bins
                for (int i = 1; i < options.NumberOfBins - 1; i++)
                {
                    xu = xl;
                    xl = Math.Pow(10d, Math.Log10(xu + offset) - delta) - offset;
                    pl = distribution.CDF(xl);
                    pu = distribution.CDF(xu);
                    bins.Add(new StratificationBin(pl, pu));
                    // The weight of the inner intervals is simply the 
                    // width of the probability interval under consideration.
                }
                // stratify last bin
                xu = xl;
                xl = Math.Pow(10d, Math.Log10(xu + offset) - delta) - offset;
                pl = distribution.CDF(xl);
                pu = distribution.CDF(xu);
                if (isExhaustive == true)
                {
                    // The weight for the last interval is evaluated 
                    // as the exceedance probability of its upper bound.
                    w = pu;
                    bins.Add(new StratificationBin(pl, pu, w));
                }
                else
                {
                    bins.Add(new StratificationBin(pl, pu));
                }
            }
            else
            {
                // get delta
                min = distribution.InverseCDF(options.LowerBound);
                max = distribution.InverseCDF(options.UpperBound);
                delta = (max - min) / options.NumberOfBins;
                // stratify first bin
                xu = max;
                xl = xu - delta;
                pl = distribution.CDF(xl);
                pu = distribution.CDF(xu);
                // determine if the first bin weight should ensure exhaustivity
                if (isExhaustive == true)
                {
                    // The weight for the first interval is evaluated 
                    // as the non-exceedance probability of its lower bound.
                    w = 1.0d - pl;
                    bins.Add(new StratificationBin(pl, pu, w));
                }
                else
                {
                    bins.Add(new StratificationBin(pl, pu));
                }
                // stratify inner bins
                for (int i = 1; i < options.NumberOfBins - 1; i++)
                {
                    xu = xl;
                    xl = xu - delta;
                    pl = distribution.CDF(xl);
                    pu = distribution.CDF(xu);
                    bins.Add(new StratificationBin(pl, pu));
                    // The weight of the inner intervals is simply the 
                    // width of the probability interval under consideration.
                }
                // stratify last bin
                xu = xl;
                xl = xu - delta;
                pl = distribution.CDF(xl);
                pu = distribution.CDF(xu);
                if (isExhaustive == true)
                {
                    // The weight for the last interval is evaluated 
                    // as the exceedance probability of its upper bound.
                    w = pu;
                    bins.Add(new StratificationBin(pl, pu, w));
                }
                else
                {
                    bins.Add(new StratificationBin(pl, pu));
                }
            }
            // 
            return bins;
        }

        /// <summary>
        /// Returns a list of stratified exceedance probability bins.
        /// </summary>
        /// <param name="options">The list of stratification options.</param>
        /// <param name="distributionType">Optional. The importance distribution type to stratify with. Default = Uniform.</param>
        /// <param name="isExhaustive">Determines if the probability bin weights should be collectively exhaustive (i.e., sum to 1).</param>
        public static List<StratificationBin> ExceedanceProbabilities(List<StratificationOptions> options, ImportanceDistribution distributionType = ImportanceDistribution.Uniform, bool isExhaustive = true)
        {
            var sortedOptions = options.ToArray();
            Array.Sort(sortedOptions, (x, y) => -x.UpperBound.CompareTo(y.UpperBound));
            var bins = new List<StratificationBin>();
            for (int i = 0; i < sortedOptions.Length; i++)
                bins.AddRange(ExceedanceProbabilities(sortedOptions[i], distributionType, false));
            if (isExhaustive == true)
            {
                // adjust end bins to ensure exhaustivity
                bins.First().Weight = 1.0d - bins.First().LowerBound;
                bins.Last().Weight = bins.Last().UpperBound;
            }

            return bins;
        }

        /// <summary>
        /// Returns a list of stratified x value bins given a list of stratified non-exceedance probability bins and a function to transform non-exceedance probabilities to x values.
        /// </summary>
        /// <param name="probabilities">List of stratified probabilities.</param>
        /// <param name="transformFunction">Function to transform non-exceedance probabilities to x values.</param>
        public static List<StratificationBin> ProbabilityToX(List<StratificationBin> probabilities, Func<double, double> transformFunction)
        {
            var bins = new List<StratificationBin>();
            double xl, xu;
            xl = transformFunction(probabilities.First().LowerBound);
            xu = transformFunction(probabilities.First().UpperBound);
            bins.Add(new StratificationBin(xl, xu));
            // transform the bins
            for (int i = 1; i < probabilities.Count; i++)
            {
                xl = xu;
                xu = transformFunction(probabilities[i].UpperBound);
                bins.Add(new StratificationBin(xl, xu));
            }

            return bins;
        }

        /// <summary>
        /// Returns a list of stratified x value bins given a list of stratified exceedance probability bins and a function to transform exceedance probabilities to x values.
        /// </summary>
        /// <param name="probabilities">List of stratified exceedance probabilities.</param>
        /// <param name="transformFunction">Function to transform exceedance probabilities to x values.</param>
        public static List<StratificationBin> ExceedanceProbabilityToX(List<StratificationBin> probabilities, Func<double, double> transformFunction)
        {
            var bins = new List<StratificationBin>();
            double xl, xu;
            xl = transformFunction(probabilities.First().UpperBound);
            xu = transformFunction(probabilities.First().LowerBound);
            bins.Add(new StratificationBin(xl, xu));
            // transform the bins
            for (int i = 1; i < probabilities.Count; i++)
            {
                xl = xu;
                xu = transformFunction(probabilities[i].LowerBound);
                bins.Add(new StratificationBin(xl, xu));
            }

            return bins;
        }

        /// <summary>
        /// Enumeration of importance distributions for stratified probabilities.
        /// </summary>
        public enum ImportanceDistribution
        {
            /// <summary>
            /// The Gumbel (Extreme Value Type I) distribution.
            /// </summary>
            Gumbel,
            /// <summary>
            /// The normal (Gaussian) distribution.
            /// </summary>
            Normal,
            /// <summary>
            /// The uniform distribution.
            /// </summary>
            Uniform,
            /// <summary>
            /// The log (base 10) uniform distribution.
            /// </summary>
            Log10Uniform
        }
    }
}