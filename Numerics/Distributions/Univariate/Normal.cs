// Since I use functions from the Accord Math Library, here is the required license header:
// Haden Smith (November 2017)
// 
// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
// 
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Numerics.Data.Statistics;
using Numerics.Mathematics.Optimization;
using Numerics.Mathematics.SpecialFunctions;
using Numerics.Sampling;

namespace Numerics.Distributions
{

    /// <summary>
    /// The Normal (Gaussian) probability distribution.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    /// References:
    /// <list type="bullet">
    /// <item><description>
    /// Wikipedia contributors, "Normal distribution,". Wikipedia, The Free
    /// Encyclopedia. Available at: https://en.wikipedia.org/wiki/Normal_distribution
    /// </description></item>
    /// <item><description>
    /// Accord Math Library, http://accord-framework.net
    /// </description></item>
    /// </list>
    /// </para>
    /// </remarks>
    [Serializable]
    public sealed class Normal : UnivariateDistributionBase, IEstimation, IColesTawn, IMaximumLikelihoodEstimation, ILinearMomentEstimation, IStandardError, IBootstrappable
    {
      
        /// <summary>
        /// Constructs a Normal (Gaussian) distribution with a mean of 0 and standard deviation of 1.
        /// </summary>
        public Normal()
        {
            SetParameters(0d, 1d);
        }

        /// <summary>
        /// Constructs a Normal (Gaussian) distribution with a given mean and a standard deviation of 1.
        /// </summary>
        /// <param name="mean">Mean of the distribution.</param>
        public Normal(double mean)
        {
            SetParameters(mean, 1d);
        }

        /// <summary>
        /// Constructs a Normal (Gaussian) distribution with given mean and standard deviation.
        /// </summary>
        /// <param name="mean">Mean of the distribution.</param>
        /// <param name="standardDeviation">Standard deviation of the distribution.</param>
        public Normal(double mean, double standardDeviation)
        {
            SetParameters(mean, standardDeviation);
        }

        private double _mu;
        private double _sigma;
        private bool _parametersValid = true;

        /// <summary>
        /// Gets and sets the location parameter µ (Mu).
        /// </summary>
        public double Mu
        {
            get { return _mu; }
            set
            {
                _parametersValid = ValidateParameters(value, Sigma, false) is null;
                _mu = value;
            }
        }

        /// <summary>
        /// Gets and sets the scale parameter σ (sigma).
        /// </summary>
        public double Sigma
        {
            get { return _sigma; }
            set
            {
                if (value < 1E-16 && Math.Sign(value) != -1) value = 1E-16;
                _parametersValid = ValidateParameters(Mu, value, false) is null;
                _sigma = value;
            }
        }

        /// <summary>
        /// Returns the number of distribution parameters.
        /// </summary>
        public override int NumberOfParameters
        {
            get { return 2; }
        }

        /// <summary>
        /// Returns the continuous distribution type.
        /// </summary>
        public override UnivariateDistributionType Type
        {
            get { return UnivariateDistributionType.Normal; }
        }

        /// <summary>
        /// Returns the name of the distribution type as a string.
        /// </summary>
        public override string DisplayName
        {
            get { return "Normal"; }
        }

        /// <summary>
        /// Returns the short display name of the distribution as a string.
        /// </summary>
        public override string ShortDisplayName
        {
            get { return "N"; }
        }

        /// <summary>
        /// Get distribution parameters in 2-column array of string.
        /// </summary>
        public override string[,] ParametersToString
        {
            get
            {
                var parmString = new string[2, 2];
                parmString[0, 0] = "Mean (µ)";
                parmString[1, 0] = "Std Dev (σ)";
                parmString[0, 1] = Mu.ToString();
                parmString[1, 1] = Sigma.ToString();
                return parmString;
            }
        }

        /// <summary>
        /// Gets the short form parameter names.
        /// </summary>
        public override string[] ParameterNamesShortForm
        {
            get { return new[] { "µ", "σ" }; }
        }

        /// <summary>
        /// Gets the full parameter names.
        /// </summary>
        public override string[] GetParameterPropertyNames
        {
            get { return new[] { nameof(Mu), nameof(Sigma) }; }
        }

        /// <summary>
        /// Get an array of parameters.
        /// </summary>
        public override double[] GetParameters
        {
            get { return new[] { Mu, Sigma }; }
        }

        /// <summary>
        /// Determines whether the parameters are valid or not.
        /// </summary>
        public override bool ParametersValid
        {
            get { return _parametersValid; }
        }

        /// <summary>
        /// Gets the mean of the distribution.
        /// </summary>
        public override double Mean
        {
            get
            {
                return Mu;
            }
        }

        /// <summary>
        /// Gets the median of the distribution.
        /// </summary>
        public override double Median
        {
            get { return Mu; }
        }

        /// <summary>
        /// Gets the mode of the distribution.
        /// </summary>
        public override double Mode
        {
            get { return Mu; }
        }

        /// <summary>
        /// Gets the standard deviation of the distribution.
        /// </summary>
        public override double StandardDeviation
        {
            get { return Sigma; }
        }

        /// <summary>
        /// Gets the skew of the distribution.
        /// </summary>
        public override double Skew
        {
            get { return 0.0d; }
        }

        /// <summary>
        /// Gets the kurtosis of the distribution.
        /// </summary>
        public override double Kurtosis
        {
            get { return 3d; }
        }
   
        /// <summary>
        /// Gets the minimum of the distribution.
        /// </summary>
        public override double Minimum
        {
            get { return double.NegativeInfinity; }
        }

        /// <summary>
        /// Gets the maximum of the distribution.
        /// </summary>
        public override double Maximum
        {
            get { return double.PositiveInfinity; }
        }

        /// <summary>
        /// Gets the minimum values allowable for each parameter.
        /// </summary>
        public override double[] MinimumOfParameters
        {
            get { return new[] { double.NegativeInfinity, 0.0d }; }
        }

        /// <summary>
        /// Gets the maximum values allowable for each parameter.
        /// </summary>
        public override double[] MaximumOfParameters
        {
            get { return new[] { double.PositiveInfinity, double.PositiveInfinity }; }
        }

        /// <summary>
        /// Estimates the parameters of the underlying distribution given a sample of observations.
        /// </summary>
        /// <param name="sample">The array of sample data.</param>
        /// <param name="estimationMethod">The parameter estimation method.</param>
        public void Estimate(IList<double> sample, ParameterEstimationMethod estimationMethod)
        {
            if (estimationMethod == ParameterEstimationMethod.MethodOfMoments)
            {
                SetParameters(Statistics.ProductMoments(sample));
            }
            else if (estimationMethod == ParameterEstimationMethod.MethodOfLinearMoments)
            {
                SetParameters(ParametersFromLinearMoments(Statistics.LinearMoments(sample)));
            }
            else if (estimationMethod == ParameterEstimationMethod.MaximumLikelihood)
            {
                // The only difference between this method and MOM is that the population standard deviation is used
                // rather than the sample; Note: the maximum likelihood estimate of the standard deviation
                // is a biased estimator.
                SetParameters(MLE(sample));
            }
        }

        /// <summary>
        /// Bootstrap the distribution based on a sample size and parameter estimation method.
        /// </summary>
        /// <param name="estimationMethod">The parameter estimation method.</param>
        /// <param name="sampleSize">Size of the random sample to generate.</param>
        /// <param name="seed">Optional. Seed for random number generator. Default = 12345.</param>
        /// <returns>
        /// Returns a bootstrapped distribution.
        /// </returns>
        public IUnivariateDistribution Bootstrap(ParameterEstimationMethod estimationMethod, int sampleSize, int seed = 12345)
        {
            var newDistribution = new Normal(Mu, Sigma);
            var sample = newDistribution.GenerateRandomValues(seed, sampleSize);
            newDistribution.Estimate(sample, estimationMethod);
            if (newDistribution.ParametersValid == false)
                throw new Exception("Bootstrapped distribution parameters are invalid.");
            return newDistribution;
        }

        /// <summary>
        /// Set the distribution parameters.
        /// </summary>
        /// <param name="location">The location parameter µ (Mu).</param>
        /// <param name="scale">The scale parameter σ (sigma).</param>
        public void SetParameters(double location, double scale)
        {
            Mu = location;
            Sigma = scale;
        }

        /// <summary>
        /// Set the distribution parameters.
        /// </summary>
        /// <param name="parameters">Array of parameters.</param>
        public override void SetParameters(IList<double> parameters)
        {
            SetParameters(parameters[0], parameters[1]);
        }

        /// <summary>
        /// Returns an array of distribution parameters given the linear moments of the sample.
        /// </summary>
        /// <param name="moments">The array of sample linear moments.</param>
        public double[] ParametersFromLinearMoments(IList<double> moments)
        {
            double mu = moments[0];
            double sigma = moments[1] * Math.Sqrt(Math.PI);
            return new[] { mu, sigma };
        }

        /// <summary>
        /// Returns an array of linear moments given the distribution parameters.
        /// </summary>
        /// <param name="parameters">The list of distribution parameters.</param>
        public double[] LinearMomentsFromParameters(IList<double> parameters)
        {
            double L1 = parameters[0];
            double L2 = parameters[1] * Math.Pow(Math.PI, -0.5);
            double T3 = 0d;
            double T4 = 30d * Math.Pow(Math.PI, -1d) * Math.Atan(Tools.Sqrt2) - 9d;
            return new[] { L1, L2, T3, T4 };
        }

        /// <summary>
        /// Validate the parameters.
        /// </summary>
        /// <param name="location">The location parameter µ (Mu).</param>
        /// <param name="scale">The scale parameter σ (sigma).</param>
        /// <param name="throwException">Determines whether to throw an exception or not.</param>
        public ArgumentOutOfRangeException ValidateParameters(double location, double scale, bool throwException)
        {
            if (double.IsNaN(location) || double.IsInfinity(scale))
            {
                if (throwException)
                    throw new ArgumentOutOfRangeException(nameof(Mu), "The mean must be a number.");
                return new ArgumentOutOfRangeException(nameof(Mu), "The mean must be a number.");
            }
            if (double.IsNaN(scale) || double.IsInfinity(scale) || scale <= 0.0d)
            {
                if (throwException)
                    throw new ArgumentOutOfRangeException(nameof(Sigma), "Standard deviation must be positive.");
                return new ArgumentOutOfRangeException(nameof(Sigma), "Standard deviation must be positive.");
            }
            return null;
        }

        /// <summary>
        /// Validate the parameters.
        /// </summary>
        /// <param name="parameters">A list of parameters.</param>
        /// <param name="throwException">Determines whether to throw an exception or not.</param>
        public override ArgumentOutOfRangeException ValidateParameters(IList<double> parameters, bool throwException)
        {
            return ValidateParameters(parameters[0], parameters[1], throwException);
        }
       
        /// <summary>
        /// Get the initial, lower, and upper values for the distribution parameters for constrained optimization.
        /// </summary>
        /// <param name="sample">The array of sample data.</param>
        /// <returns>Returns a Tuple of initial, lower, and upper values.</returns>
        public Tuple<double[], double[], double[]> GetParameterConstraints(IList<double> sample)
        {
            var initialVals = new double[NumberOfParameters];
            var lowerVals = new double[NumberOfParameters];
            var upperVals = new double[NumberOfParameters];
            // Estimate initial values using the method of moments (a.k.a product moments).
            var moments = Statistics.ProductMoments(sample);
            initialVals[0] = moments[0];
            initialVals[1] = moments[1];
            // Get bounds of mean
            if (initialVals[0] == 0d) initialVals[0] = Tools.DoubleMachineEpsilon;
            lowerVals[0] = -Math.Pow(10d, Math.Ceiling(Math.Log10(Math.Abs(initialVals[0])) + 1d));
            upperVals[0] = Math.Pow(10d, Math.Ceiling(Math.Log10(Math.Abs(initialVals[0])) + 1d));
            // Get bounds of standard deviation
            lowerVals[1] = Tools.DoubleMachineEpsilon;
            upperVals[1] = Math.Pow(10d, Math.Ceiling(Math.Log10(initialVals[1]) + 1d));
            return new Tuple<double[], double[], double[]>(initialVals, lowerVals, upperVals);
        }

        /// <summary>
        /// Estimate the distribution parameters using the method of maximum likelihood estimation.
        /// </summary>
        /// <param name="sample">The array of sample data.</param>
        public double[] MLE(IList<double> sample)
        {
            // Set constraints
            var tuple = GetParameterConstraints(sample);
            var Initials = tuple.Item1;
            var Lowers = tuple.Item2;
            var Uppers = tuple.Item3;

            // Solve using Nelder-Mead (Downhill Simplex)
            double logLH(double[] x)
            {
                var N = new Normal();
                N.SetParameters(x);
                return N.LogLikelihood(sample);
            }
            var solver = new NelderMead(logLH, NumberOfParameters, Initials, Lowers, Uppers);
            solver.ReportFailure = true;
            solver.Maximize();
            return solver.BestParameterSet.Values;
        }

        /// <summary>
        /// The Probability Density Function (PDF) of the distribution evaluated at a point X.
        /// </summary>
        /// <param name="x">A single point in the distribution range.</param>
        public override double PDF(double x)
        {
            // Validate parameters
            if (_parametersValid == false)
                ValidateParameters(Mu, Sigma, true);
            double z = (x - Mu) / Sigma;
            return Math.Exp(-0.5d * z * z) / (Tools.Sqrt2PI * Sigma);
        }

        /// <summary>
        /// The Cumulative Distribution Function (CDF) for the distribution evaluated at a point X.
        /// </summary>
        /// <param name="x">A single point in the distribution range.</param>
        /// <returns>The non-exceedance probability given a point X.</returns>
        /// <remarks>
        /// The CDF describes the cumulative probability that a given value or any value smaller than it will occur.
        /// </remarks>
        public override double CDF(double x)
        {
            // Validate parameters
            if (_parametersValid == false)
                ValidateParameters(Mu, Sigma, true);
            return 0.5d * (1.0d + Erf.Function((x - Mu) / (Sigma * Tools.Sqrt2)));
        }

        /// <summary>
        /// Constants used in the rational approximation.
        /// </summary>
        private static readonly double[] inverse_P0 = new double[] { -59.963350101410789d, 98.001075418599967d, -56.676285746907027d, 13.931260938727968d, -1.2391658386738125d };
        private static readonly double[] inverse_Q0 = new double[] { 1.9544885833814176d, 4.6762791289888153d, 86.360242139089053d, -225.46268785411937d, 200.26021238006067d, -82.037225616833339d, 15.90562251262117d, -1.1833162112133d };
        private static readonly double[] inverse_P1 = new double[] { 4.0554489230596245d, 31.525109459989388d, 57.162819224642128d, 44.080507389320083d, 14.684956192885803d, 2.1866330685079025d, -0.14025607917135449d, -0.035042462682784818d, -0.00085745678515468545d };
        private static readonly double[] inverse_Q1 = new double[] { 15.779988325646675d, 45.390763512887922d, 41.3172038254672d, 15.04253856929075d, 2.5046494620830941d, -0.14218292285478779d, -0.038080640769157827d, -0.00093325948089545744d };
        private static readonly double[] inverse_P2 = new double[] { 3.2377489177694603d, 6.9152288906898418d, 3.9388102529247444d, 1.3330346081580755d, 0.20148538954917908d, 0.012371663481782003d, 0.00030158155350823543d, 0.0000026580697468673755d, 0.0000000062397453918498331d };
        private static readonly double[] inverse_Q2 = new double[] { 6.02427039364742d, 3.6798356385616087d, 1.3770209948908132d, 0.21623699359449664d, 0.013420400608854318d, 0.00032801446468212774d, 0.0000028924786474538068d, 0.0000000067901940800998127d };

        /// <summary>
        /// A rational approximation for the Normal (Gaussian) inverse cumulative distribution function.
        /// </summary>
        /// <param name="probability">Probability between 0 and 1.</param>
        /// <remarks>
        /// <see href = "http://accord-framework.net"/>
        /// </remarks>
        /// <returns>
        /// Returns the value, <c>x</c>, for which the area under the Normal (Gaussian)
        /// probability density function (integrated from minus infinity to <c>x</c>) Is
        /// equal to the argument <c>y</c> (assumes mean Is zero, variance Is one).
        /// </returns>
        private static double RationalApproximation(double probability)
        {
            //double s2pi = Math.Sqrt(2.0d * Math.PI);
            int code = 1;
            double y = probability;
            double x;
            // 
            if (y > 0.8646647167633873d)
            {
                y = 1.0d - y;
                code = 0;
            }
            // 
            if (y > 0.1353352832366127d)
            {
                y -= 0.5d;
                double y2 = y * y;
                x = y + y * (y2 * Evaluate.PolynomialRev(inverse_P0, y2) / Evaluate.PolynomialRev_1(inverse_Q0, y2));
                x *= Tools.Sqrt2PI;
                return x;
            }
            // 
            x = Math.Sqrt(-2.0d * Math.Log(y));
            double x0 = x - Math.Log(x) / x;
            double z = 1.0d / x;
            double x1;
            // 
            if (x < 8.0d)
            {
                x1 = z * Evaluate.PolynomialRev(inverse_P1, z) / Evaluate.PolynomialRev_1(inverse_Q1, z);
            }
            else
            {
                x1 = z * Evaluate.PolynomialRev(inverse_P2, z) / Evaluate.PolynomialRev_1(inverse_Q2, z);
            }
            // 
            x = x0 - x1;
            // 
            if (code != 0)
            {
                x = -x;
            }
            return x;
        }

        /// <summary>
        /// Gets the Inverse Cumulative Distribution Function (ICFD) of the distribution evaluated at a probability.
        /// </summary>
        /// <param name="probability">Probability between 0 and 1.</param>
        /// <returns>
        /// Returns for a given probability in the probability distribution of a random variable,
        /// the value at which the probability of the random variable is less than or equal to the
        /// given probability.
        /// </returns>
        /// <remarks>
        /// This function is also know as the Quantile Function.
        /// </remarks>
        public override double InverseCDF(double probability)
        {
            // Validate probability
            if (probability < 0.0d || probability > 1.0d)
                throw new ArgumentOutOfRangeException("probability", "Probability must be between 0 and 1.");
            if (probability == 0.0d)
                return Minimum;
            if (probability == 1.0d)
                return Maximum;
            // Validate parameters
            if (_parametersValid == false)
                ValidateParameters(Mu, Sigma, true);
            return Mu + Sigma * RationalApproximation(probability);
        }


        /// <summary>
        /// Returns the PDF for a standard Normal distribution, where mean of 0 and standard deviation of 1.
        /// </summary>
        /// <param name="Z">The Z variate of a standard Normal.</param>
        public static double StandardPDF(double Z)
        {
            return Math.Exp(-0.5d * Z * Z) / Tools.Sqrt2PI;
        }

        public static double StandardLogPDF(double Z)
        {
            return -0.5d * Z * Z - Tools.LogSqrt2PI;
        }

        public static double[] StandardPDF(IList<double> zValues)
        {
            var result = new double[zValues.Count];
            for (int i = 0; i < zValues.Count; i++)
                result[i] = StandardPDF(zValues[i]);
            return result;
        }

        /// <summary>
        /// Returns the CDF for a standard Normal distribution, where mean of 0 and standard deviation of 1.
        /// </summary>
        /// <param name="Z">The Z variate of a standard Normal.</param>
        public static double StandardCDF(double Z)
        {
            return MultivariateNormal.MVNPHI(Z);
            //return 0.5d * (1.0d + Erf.Function(Z / Math.Sqrt(2.0d)));
        }

        public static double[] StandardCDF(IList<double> zValues)
        {
            var result = new double[zValues.Count];
            for (int i = 0; i < zValues.Count; i++)
                result[i] = StandardCDF(zValues[i]);
            return result;
        }

        /// <summary>
        /// Returns the Z variate for a standard Normal distribution, where mean of 0 and standard deviation of 1.
        /// </summary>
        /// <param name="probability">Probability between 0 and 1.</param>
        public static double StandardZ(double probability)
        {
            // Validate probability
            if (probability < 0.0d || probability > 1.0d)
                throw new ArgumentOutOfRangeException("probability", "Probability must be between 0 and 1.");
            double limit = 1E-16;
            if (probability <= limit)
               return -8.2220822161304348;
            if (probability >= 1d - limit)
                return 8.2095361516013874;
            return RationalApproximation(probability);
        }

        public double[] StandardZ(IList<double> probabilities)
        {
            var result = new double[probabilities.Count];
            for (int i = 0; i < probabilities.Count; i++)
                result[i] = StandardZ(probabilities[i]);
            return result;
        }

        /// <summary>
        /// Get confidence intervals based on the Normal approximation.
        /// </summary>
        /// <param name="sampleSize">The data sample size N used for computing the standard error.</param>
        /// <param name="quantiles">List of nonexceedance probabilities for output frequency curves.</param>
        /// <param name="percentiles">List of confidence percentiles for confidence interval output.</param>
        /// <remarks>
        /// References: Stedinger, J. Confidence Intervals for Design Events. Journal of Hydraulic Engineering. 1983.
        /// </remarks>
        public double[,] NormalConfidenceIntervals(int sampleSize, IList<double> quantiles, IList<double> percentiles)
        {
            // validate parameters
            if (_parametersValid == false)
                ValidateParameters(Mu, _sigma, true);
            // Dimension output array
            int q = quantiles.Count;
            int p = percentiles.Count;
            var Output = new double[q, p];
            for (int i = 0; i < q; i++)
            {
                double Xp = InverseCDF(quantiles[i]);
                double Zp = StandardZ(quantiles[i]);
                // Record percentiles for user-defined probabilities
                for (int j = 0; j < p; j++)
                {
                    double Z = StandardZ(percentiles[j]);
                    double VARp = Sigma * Sigma / sampleSize * (1.0d + 0.5d * Zp * Zp);
                    Output[i, j] = Xp + Z * Math.Sqrt(VARp);
                }
            }
            // Return confidence percentile output
            return Output;
        }

        /// <summary>
        /// Get exact confidence intervals based on the Noncentral-T distribution.
        /// </summary>
        /// <param name="sampleSize">The data sample size N used for computing the standard error.</param>
        /// <param name="quantiles">List of nonexceedance probabilities for output frequency curves.</param>
        /// <param name="percentiles">List of confidence percentiles for confidence interval output.</param>
        /// <remarks>
        /// References: Stedinger, J. Confidence Intervals for Design Events. Journal of Hydraulic Engineering. 1983.
        /// </remarks>
        public double[,] NoncentralTConfidenceIntervals(int sampleSize, IList<double> quantiles, IList<double> percentiles)
        {
            // validate parameters
            if (_parametersValid == false)
                ValidateParameters(Mu, _sigma, true);
            // Dimension output array
            int q = quantiles.Count;
            int p = percentiles.Count;
            var Output = new double[q, p];
            int df = sampleSize - 1;
            double SE = Sigma / Math.Sqrt(sampleSize);
            for (int i = 0; i < q; i++)
            {
                double Z = StandardZ(quantiles[i]);
                var NCT = new NoncentralT(df, Z * Math.Sqrt(sampleSize));
                // Record percentiles for user-defined probabilities
                for (int j = 0; j < p; j++)
                    Output[i, j] = Mu + NCT.InverseCDF(percentiles[j]) * SE;
            }
            // Return confidence percentile output
            return Output;
        }

        /// <summary>
        /// Get confidence intervals using Monte Carlo simulation.
        /// </summary>
        /// <param name="sampleSize">The data sample size N used for computing the standard error.</param>
        /// <param name="realizations">The number of Monte Carlo realizations.</param>
        /// <param name="quantiles">List of nonexceedance probabilities for output frequency curves.</param>
        /// <param name="percentiles">List of confidence percentiles for confidence interval output.</param>
        /// <remarks>
        /// This is the same sampling approach as used in HEC-FDA.
        /// </remarks>
        public double[,] MonteCarloConfidenceIntervals(int sampleSize, int realizations, IList<double> quantiles, IList<double> percentiles)
        {
            // validate parameters
            if (_parametersValid == false)
                ValidateParameters(Mu, _sigma, true);
            // Dimension output array
            int q = quantiles.Count;
            int p = percentiles.Count;
            var Output = new double[q, p];

            // Variables
            double OriginalMean = Mu;
            double OriginalStdDev = Sigma;

            // Create random numbers for mean and standard deviation
            var r = new MersenneTwister(12345);
            var rndMean = r.NextDoubles(realizations);
            r = new MersenneTwister(45678);
            var rndStdDev = r.NextDoubles(realizations);

            // Create list of Monte Carlo distributions
            var MonteCarloDistributions = new UnivariateDistributionBase[realizations];
            Parallel.For(0, realizations, idx =>
            {

                // Generate new mean
                var Normal = new Normal(OriginalMean, OriginalStdDev / Math.Sqrt(sampleSize));
                double NewMu = Normal.InverseCDF(rndMean[idx]);
                // Generate new standard deviation
                var Chi = new ChiSquared(sampleSize - 1);
                double NewSigma = Math.Sqrt((sampleSize - 1) * Math.Pow(OriginalStdDev, 2d) / Chi.InverseCDF(rndStdDev[idx]));
                // Create a new distribution with the new parameters
                MonteCarloDistributions[idx] = new Normal(NewMu, NewSigma);
            });
            // Create confidence intervals
            for (int i = 0; i < q; i++)
            {
                // Create array of X values across user-defined probabilities
                var XValues = new double[realizations];
                // Record X values
                Parallel.For(0, realizations, idx => XValues[idx] = MonteCarloDistributions[idx].InverseCDF(quantiles[i]));
                // Record percentiles for user-defined probabilities
                for (int j = 0; j < p; j++)
                    Output[i, j] = Statistics.Percentile(XValues, percentiles[j]);
            }
            // Return confidence percentile output
            return Output;
        }

        /// <summary>
        /// Gets the expected probability given an exceedance probability.
        /// </summary>
        /// <param name="sampleSize">The data sample size N used for computing the standard error.</param>
        /// <param name="probability">Exceedance probability.</param>
        public double ExpectedProbability(int sampleSize, double probability)
        {
            int N = sampleSize;
            var T = new StudentT(N - 1);
            return T.CDF(StandardZ(probability) * Math.Sqrt(N / (double)(N + 1)));
        }
  
        /// <summary>
        /// Returns a list containing the variance of each parameter given the sample size.
        /// </summary>
        /// <param name="sampleSize">The sample size.</param>
        /// <param name="estimationMethod">The distribution parameter estimation method.</param>
        public IList<double> ParameterVariance(int sampleSize, ParameterEstimationMethod estimationMethod)
        {
            // Validate parameters
            if (_parametersValid == false)
                ValidateParameters(Mu, _sigma, true);
            double u2 = Sigma;
            var varList = new List<double>();
            varList.Add(Math.Pow(u2, 2d) / sampleSize); // location
            varList.Add(2d * Math.Pow(u2, 4d) / sampleSize); // scale
            return varList;
        }

        /// <summary>
        /// Returns a list containing the covariances of the parameters given the sample size.
        /// </summary>
        /// <param name="sampleSize">The sample size.</param>
        /// <param name="estimationMethod">The distribution parameter estimation method.</param>
        public IList<double> ParameterCovariance(int sampleSize, ParameterEstimationMethod estimationMethod)
        {
            // Validate parameters
            if (_parametersValid == false)
                ValidateParameters(Mu, _sigma, true);
            double u2 = Sigma;
            var covarList = new List<double>();
            covarList.Add(0.0d); // location & scale
            return covarList;
        }

        /// <summary>
        /// Returns a list of partial derivatives of X given probability with respect to each parameter.
        /// </summary>
        /// <param name="probability">Probability between 0 and 1.</param>
        public IList<double> PartialDerivatives(double probability)
        {
            // Validate parameters
            if (_parametersValid == false)
                ValidateParameters(Mu, _sigma, true);
            double u2 = Sigma;
            double z = StandardZ(probability);
            var partialList = new List<double>();
            partialList.Add(1.0d); // location
            partialList.Add(z / (2d * u2)); // scale
            return partialList;
        }

        /// <summary>
        /// The quantile variance given probability and sample size.
        /// </summary>
        /// <param name="probability">Probability between 0 and 1.</param>
        /// <param name="sampleSize">The sample size.</param>
        /// <param name="estimationMethod">The distribution parameter estimation method.</param>
        public double QuantileVariance(double probability, int sampleSize, ParameterEstimationMethod estimationMethod)
        {
            double varA = ParameterVariance(sampleSize, estimationMethod)[0];
            double varB = ParameterVariance(sampleSize, estimationMethod)[1];
            double covAB = ParameterCovariance(sampleSize, estimationMethod)[0];
            double pXA = PartialDerivatives(probability)[0];
            double pXB = PartialDerivatives(probability)[1];
            return Math.Pow(pXA, 2d) * varA + Math.Pow(pXB, 2d) * varB + 2d * pXA * pXB * covAB;
        }
    
        /// <summary>
        /// Returns the determinant of the Jacobian.
        /// </summary>
        /// <param name="probabilities">List of probabilities, must be the same length as the number of distribution parameters.</param>
        public double Jacobian(IList<double> probabilities)
        {
            if (probabilities.Count != NumberOfParameters)
            {
                throw new ArgumentOutOfRangeException(nameof(Jacobian), "The number of probabilities must be the same length as the number of distribution parameters.");
            }
            // |a b|
            // |c d|
            // |A| = ad − bc
            var dXt1 = PartialDerivatives(probabilities[0]).ToArray();
            var dXt2 = PartialDerivatives(probabilities[1]).ToArray();
            double a = dXt1[0];
            double b = dXt1[1];
            double c = dXt2[0];
            double d = dXt2[1];
            return a * d - b * c;
        }

        /// <summary>
        /// Creates a copy of the distribution.
        /// </summary>
        public override UnivariateDistributionBase Clone()
        {
            return new Normal(Mu, Sigma);
        }
  
    }
}