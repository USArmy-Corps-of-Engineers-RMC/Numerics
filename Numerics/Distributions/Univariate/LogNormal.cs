using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Numerics.Data;
using Numerics.Data.Statistics;
using Numerics.Mathematics.Optimization;
using Numerics.Mathematics.SpecialFunctions;
using Numerics.Sampling;

namespace Numerics.Distributions
{

    /// <summary>
    /// The Log-Normal probability distribution.
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
    /// </description></item>
    /// </list>
    /// </para>
    /// </remarks>
    [Serializable]
    public sealed class LogNormal : UnivariateDistributionBase, IColesTawn, IEstimation, IMaximumLikelihoodEstimation, ILinearMomentEstimation, IStandardError, IBootstrappable
    {
  
        /// <summary>
        /// Constructs a Log-Normal distribution with a mean (of log) of 3 and std dev (of log) of 0.5
        /// </summary>
        public LogNormal()
        {
            SetParameters(3d, 0.5d);
        }

        /// <summary>
        /// Constructs a Log-Normal distribution with given mean (of log) and standard deviation (of log).
        /// </summary>
        /// <param name="meanOfLog">The mean of the log transformed data.</param>
        /// <param name="standardDeviationOfLog">The standard deviation of the log transformed data.</param>
        public LogNormal(double meanOfLog, double standardDeviationOfLog)
        {
            SetParameters(meanOfLog, standardDeviationOfLog);
        }

        // Private variables
        private double _mu;
        private double _sigma;
        private double _base = 10d;
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
        /// Gets and sets the base of the logarithm
        /// </summary>
        public double Base
        {
            get { return _base; }
            set
            {
                if (value < 1d)
                {
                    _base = 1d;
                }
                else
                {
                    _base = value;
                }
            }
        }

        /// <summary>
        /// Gets the log correction factor.
        /// </summary>
        private double K
        {
            get { return 1d / Math.Log(Base); }
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
            get { return UnivariateDistributionType.LogNormal; }
        }

        /// <summary>
        /// Returns the name of the distribution type as a string.
        /// </summary>
        public override string DisplayName
        {
            get { return "Log-Normal (base 10)"; }
        }

        /// <summary>
        /// Returns the short display name of the distribution as a string.
        /// </summary>
        public override string ShortDisplayName
        {
            get { return "LogN"; }
        }

        /// <summary>
        /// Get distribution parameters in 2-column array of string.
        /// </summary>
        public override string[,] ParametersToString
        {
            get
            {
                var parmString = new string[2, 2];
                parmString[0, 0] = "Mean (of log) (µ)";
                parmString[1, 0] = "Std Dev (of log) (σ)";
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
            get { return Math.Exp((Mu + Sigma * Sigma / 2.0d) / K); }
        }

        /// <summary>
        /// Gets the median of the distribution.
        /// </summary>
        public override double Median
        {
            get { return Math.Exp(Mu / K); }
        }

        /// <summary>
        /// Gets the mode of the distribution.
        /// </summary>
        public override double Mode
        {
            get { return Math.Exp((Mu - Sigma * Sigma) / K); }
        }

        /// <summary>
        /// Gets the standard deviation of the distribution.
        /// </summary>
        public override double StandardDeviation
        {
            get { return Math.Sqrt((Math.Exp(Sigma * Sigma / K) - 1.0d) * Math.Exp((2d * Mu + Sigma * Sigma) / K)); }
        }

        /// <summary>
        /// Gets the skew of the distribution.
        /// </summary>
        public override double Skew
        {
            get { return (Math.Exp(Sigma * Sigma / K) + 2.0d) * Math.Sqrt(Math.Exp(Sigma * Sigma / K) - 1d); }
        }

        /// <summary>
        /// Gets the kurtosis of the distribution.
        /// </summary>
        public override double Kurtosis
        {
            get
            {
                double siqma2 = Math.Pow(Sigma, 2d);
                return 3d + (Math.Exp(4d * siqma2 / K) + 2d * Math.Exp(3d * siqma2 / K) + 3d * Math.Exp(2d * siqma2 / K) - 6d);
            }
        }

        /// <summary>
        /// Gets the minimum of the distribution.
        /// </summary>
        public override double Minimum
        {
            get { return 0.0d; }
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
            get { return new[] { 0.0d, 0.0d }; }
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
                SetParameters(IndirectMethodOfMoments(sample));
            }
            else if (estimationMethod == ParameterEstimationMethod.MethodOfLinearMoments)
            {
                SetParameters(ParametersFromLinearMoments(IndirectMethodOfLinearMoments(sample)));
            }
            else if (estimationMethod == ParameterEstimationMethod.MaximumLikelihood)
            {
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
            var newDistribution = new LogNormal(Mu, Sigma);
            var sample = newDistribution.GenerateRandomValues(seed, sampleSize);
            newDistribution.Estimate(sample, estimationMethod);
            if (newDistribution.ParametersValid == false)
                throw new Exception("Bootstrapped distribution parameters are invalid.");
            return newDistribution;
        }

        /// <summary>
        /// Set the distribution parameters based on the moments of the log transformed data.
        /// </summary>
        /// <param name="meanOfLog">The mean of the log transformed data.</param>
        /// <param name="standardDeviationOfLog">The standard deviation of the log transformed data.</param>
        public void SetParameters(double meanOfLog, double standardDeviationOfLog)
        {
            // Set parameters
            Mu = meanOfLog;
            Sigma = standardDeviationOfLog;
        }

        /// <summary>
        /// Set the distribution parameters using the "direct method."
        /// </summary>
        /// <param name="parameters">Array of parameters.</param>
        public override void SetParameters(IList<double> parameters)
        {
            SetParameters(parameters[0], parameters[1]);
        }

        /// <summary>
        /// Validate the parameters.
        /// </summary>
        /// <param name="mu">The mean (of log).</param>
        /// <param name="sigma">The standard deviation (of log).</param>
        /// <param name="throwException">Determines whether to throw an exception or not.</param>
        public ArgumentOutOfRangeException ValidateParameters(double mu, double sigma, bool throwException)
        {
            if (double.IsNaN(mu) || double.IsInfinity(mu))
            {
                if (throwException)
                    throw new ArgumentOutOfRangeException(nameof(Mu), "Mu must be a number.");
                return new ArgumentOutOfRangeException(nameof(Mu), "Mu must be a number.");
            }
            if (double.IsNaN(sigma) || double.IsInfinity(sigma) || sigma <= 0.0d)
            {
                if (throwException)
                    throw new ArgumentOutOfRangeException(nameof(Sigma), "Sigma must be positive.");
                return new ArgumentOutOfRangeException(nameof(Sigma), "Sigma must be positive.");
            }
            return null;
        }

        /// <summary>
        /// Validate the parameters.
        /// </summary>
        /// <param name="parameters">List of parameters.</param>
        /// <param name="throwException">Determines whether to throw an exception or not.</param>
        public override ArgumentOutOfRangeException ValidateParameters(IList<double> parameters, bool throwException)
        {
            return ValidateParameters(parameters[0], parameters[1], throwException);
        }

        
        /// <summary>
        /// The indirect method of moments derives the moments from the log transformed data.
        /// This method was proposed by the U.S. Water Resources Council (WRC, 1967).
        /// </summary>
        /// <param name="sample">The array of sample data.</param>
        public double[] IndirectMethodOfMoments(IList<double> sample)
        {
            // Transform the sample
            var transformedSample = new List<double>();
            for (int i = 0; i < sample.Count; i++)
            {
                if (sample[i] > 0d)
                {
                    transformedSample.Add(Math.Log(sample[i], Base));
                }
                else
                {
                    transformedSample.Add(Math.Log(0.1d, Base));
                }
            }
            return Statistics.ProductMoments(transformedSample);
        }

        /// <summary>
        /// The indirect method of moments derives the moments from the log transformed data.
        /// This method was proposed by the U.S. Water Resources Council (WRC, 1967).
        /// </summary>
        /// <param name="sample">The array of sample data.</param>
        public double[] IndirectMethodOfLinearMoments(IList<double> sample)
        {
            // Transform the sample
            var transformedSample = new List<double>();
            for (int i = 0; i < sample.Count; i++)
            {
                if (sample[i] > 0d)
                {
                    transformedSample.Add(Math.Log(sample[i], Base));
                }
                else
                {
                    transformedSample.Add(Math.Log(0.1d, Base));
                }
            }
            return Statistics.LinearMoments(transformedSample);
        }

        /// <summary>
        /// Sets the parameters using the direct method of moments. Moments are derived from the real-space data.
        /// </summary>
        /// <param name="mean">The real-space mean of the data.</param>
        /// <param name="standardDeviation">The real-space standard deviation of the data.</param>
        public double[] DirectMethodOfMoments(double mean, double standardDeviation)
        {
            double variance = Math.Pow(standardDeviation, 2d);
            double mu = Math.Log(Math.Pow(mean, 2d) / Math.Sqrt(variance + Math.Pow(mean, 2d)), Base);
            double sigma = Math.Sqrt(Math.Log(1.0d + variance / Math.Pow(mean, 2d), Base));
            return new[] { mu, sigma };
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
            var mom = IndirectMethodOfMoments(sample);
            initialVals = new double[] { mom[0], mom[1] };
            // Get bounds of mean
            double real = Math.Exp(initialVals[0] / K);
            lowerVals[0] = -Math.Ceiling(Math.Log(Math.Pow(10d, Math.Ceiling(Math.Log10(real) + 2d)), Base));
            upperVals[0] = Math.Ceiling(Math.Log(Math.Pow(10d, Math.Ceiling(Math.Log10(real) + 2d)), Base));
            // Get bounds of standard deviation
            lowerVals[1] = Tools.DoubleMachineEpsilon;
            upperVals[1] = upperVals[0];
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
                var LogN = new LogNormal() { Base = Base };
                LogN.SetParameters(x);
                return LogN.LogLikelihood(sample);
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
            if (_parametersValid == false)
                ValidateParameters(Mu, Sigma, true);
            if (x < Minimum) return 0.0d;
            //if (x == 0) x = 1E-16;
            double d = (Math.Log(x, Base) - Mu) / Sigma;
            return Math.Exp(-0.5d * d * d) / (Tools.Sqrt2PI * Sigma) * (K / x);
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
            if (_parametersValid == false)
                ValidateParameters(Mu, Sigma, true);
            if (x <= Minimum)
                return 0d;
            return 0.5d * (1.0d + Erf.Function((Math.Log(x, Base) - Mu) / (Sigma * Math.Sqrt(2.0d))));
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
            return Math.Exp((Mu - Sigma * Math.Sqrt(2.0d) * Erf.InverseErfc(2.0d * probability)) / K);
        }

        /// <summary>
        /// Get confidence intervals using Monte Carlo simulation.
        /// </summary>
        /// <param name="sampleSize">The data sample size N used for computing the standard error.</param>
        /// <param name="realizations">The number of Monte Carlo realizations.</param>
        /// <param name="quantiles">List of exceedance probabilities for output frequency curves.</param>
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
                MonteCarloDistributions[idx] = new LogNormal(NewMu, NewSigma);
            });

            // Create confidence intervals
            for (int i = 0; i < q; i++)
            {
                // Create array of X values across user-defined probabilities
                var XValues = new double[realizations];
                // Record X values
                Parallel.For(0, realizations, idx => XValues[idx] = MonteCarloDistributions[idx].InverseCDF(1d - quantiles[i]));
                // Record percentiles for user-defined probabilities
                for (int j = 0; j < p;  j++)
                    Output[i, j] = Statistics.Percentile(XValues, percentiles[j]);
            }

            // Return confidence percentile output
            return Output;
        }

        /// <summary>
        /// Compute the expected probability curve given a list of distributions.
        /// </summary>
        /// <param name="distributions">List of probability distributions.</param>
        /// <param name="XValues">List of X values for computing the expected probability E[AEP|X].</param>
        /// <param name="quantiles">List of exceedance probabilities for output frequency curve.</param>
        private double[] ExpectedProbability(IEnumerable<UnivariateDistributionBase> distributions, IList<double> XValues, IList<double> quantiles)
        {
            // Variables
            var F_Expected = new double[XValues.Count];
            var ExpectedCurve = new double[quantiles.Count];
            // Compute the expected probability E[AEP] given X
            // Use Total Probability Theorem to compute the expected probability curve.
            // Here all of the realizations have equal weight so the calculation is a simple average. 
            Parallel.For(0, XValues.Count, idx => { for (int j = 0; j < distributions.Count(); j++) F_Expected[idx] += distributions.ElementAtOrDefault(j).CDF(XValues[idx]) / distributions.Count(); });
            // Now interpolate X given E[AEP]. This is so we can plot the expected curve in a tradition manner with AEP on the X axis, and X on the Y axis.
            Linear linInt = new Linear(F_Expected, XValues);
            Parallel.For(0, quantiles.Count, (int idx) => ExpectedCurve[idx] = linInt.Interpolate(1d-quantiles[idx]));
            // Return the expected curve
            return ExpectedCurve;
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
            double z = Normal.StandardZ(probability);
            var partialList = new List<double>();
            partialList.Add(1.0d); // location
            partialList.Add(z / (2d * Sigma)); // scale
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
            double varT = Math.Pow(pXA, 2d) * varA + Math.Pow(pXB, 2d) * varB + 2d * pXA * pXB * covAB;
            return varT * Math.Pow(InverseCDF(probability) / K, 2d);
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
            double p0 = InverseCDF(probabilities[0]) / K;
            double p1 = InverseCDF(probabilities[1]) / K;
            double a = dXt1[0] * p0;
            double b = dXt1[1] * p0;
            double c = dXt2[0] * p1;
            double d = dXt2[1] * p1;
            return a * d - b * c;
        }

        /// <summary>
        /// Creates a copy of the distribution.
        /// </summary>
        public override UnivariateDistributionBase Clone()
        {
            return new LogNormal(Mu, Sigma);
        }

    }
}