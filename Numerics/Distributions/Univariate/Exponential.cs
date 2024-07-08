using System;
using System.Collections.Generic;
using System.Linq;
using Numerics.Data.Statistics;
using Numerics.Mathematics.Optimization;

namespace Numerics.Distributions
{

    /// <summary>
    /// The exponential distribution.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    /// <see href = "https://en.wikipedia.org/wiki/Exponential_distribution" />
    /// </para>
    /// </remarks>
    [Serializable]
    public sealed class Exponential : UnivariateDistributionBase, IColesTawn, IEstimation, IMaximumLikelihoodEstimation, IMomentEstimation, ILinearMomentEstimation, IStandardError, IBootstrappable
    {
  
        /// <summary>
        /// Constructs an Exponential distribution with a location of 100 and scale of 10.
        /// </summary>
        public Exponential()
        {
            SetParameters(100d, 10d);
        }

        /// <summary>
        /// Constructs an Exponential distribution with a given ξ and α.
        /// </summary>
        /// <param name="location">The location parameter ξ (Xi).</param>
        /// <param name="scale">The scale parameter α (alpha).</param>
        public Exponential(double location, double scale)
        {
            SetParameters(location, scale);
        }
  
        private bool _parametersValid = true;
        private double _xi; // location
        private double _alpha; // scale

        /// <summary>
        /// Gets and sets the location parameter ξ (Xi).
        /// </summary>
        public double Xi
        {
            get { return _xi; }
            set
            {
                _parametersValid = ValidateParameters(new[] { value, Alpha }, false) is null;
                _xi = value;
            }
        }

        /// <summary>
        /// Gets and sets the scale parameter α (alpha).
        /// </summary>
        public double Alpha
        {
            get { return _alpha; }
            set
            {
                _parametersValid = ValidateParameters(new[] { Xi, value }, false) is null;
                _alpha = value;
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
        /// Returns the univariate distribution type.
        /// </summary>
        public override UnivariateDistributionType Type
        {
            get { return UnivariateDistributionType.Exponential; }
        }

        /// <summary>
        /// Returns the name of the distribution type as a string.
        /// </summary>
        public override string DisplayName
        {
            get { return "Exponential"; }
        }

        /// <summary>
        /// Returns the short display name of the distribution as a string.
        /// </summary>
        public override string ShortDisplayName
        {
            get { return "EXP"; }
        }

        /// <summary>
        /// Get distribution parameters in 2-column array of string.
        /// </summary>
        public override string[,] ParametersToString
        {
            get
            {
                var parmString = new string[2, 2];
                parmString[0, 0] = "Location (ξ)";
                parmString[1, 0] = "Scale (α)";
                parmString[0, 1] = Xi.ToString();
                parmString[1, 1] = Alpha.ToString();
                return parmString;
            }
        }

        /// <summary>
        /// Gets the short form parameter names.
        /// </summary>
        public override string[] ParameterNamesShortForm
        {
            get { return new[] { "ξ", "α" }; }
        }

        /// <summary>
        /// Gets the full parameter names.
        /// </summary>
        public override string[] GetParameterPropertyNames
        {
            get { return new[] { nameof(Xi), nameof(Alpha) }; }
        }

        /// <summary>
        /// Get an array of parameters.
        /// </summary>
        public override double[] GetParameters
        {
            get { return new[] { Xi, Alpha }; }
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
            get { return Xi + Alpha; }
        }

        /// <summary>
        /// Gets the median of the distribution.
        /// </summary>
        public override double Median
        {
            get { return Xi - Math.Log(0.5d) * Alpha; }
        }

        /// <summary>
        /// Gets the mode of the distribution.
        /// </summary>
        public override double Mode
        {
            get { return Xi; }
        }

        /// <summary>
        /// Gets the standard deviation of the distribution.
        /// </summary>
        public override double StandardDeviation
        {
            get { return Alpha; }
        }

        /// <summary>
        /// Gets the skew of the distribution.
        /// </summary>
        public override double Skew
        {
            get { return 2.0d; }
        }

        /// <summary>
        /// Gets the kurtosis of the distribution.
        /// </summary>
        public override double Kurtosis
        {
            get { return 9.0d; }
        }

        /// <summary>
        /// Gets the minimum of the distribution.
        /// </summary>
        public override double Minimum
        {
            get { return Xi; }
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
                SetParameters(ParametersFromMoments(Statistics.ProductMoments(sample)));
            }
            else if (estimationMethod == ParameterEstimationMethod.MethodOfLinearMoments)
            {
                SetParameters(ParametersFromLinearMoments(Statistics.LinearMoments(sample)));
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
            var newDistribution = new Exponential(Xi, Alpha);
            var sample = newDistribution.GenerateRandomValues(seed, sampleSize);
            newDistribution.Estimate(sample, estimationMethod);
            if (newDistribution.ParametersValid == false)
                throw new Exception("Bootstrapped distribution parameters are invalid.");
            return newDistribution;
        }

        /// <summary>
        /// Set the distribution parameters.
        /// </summary>
        /// <param name="location">The location parameter ξ (Xi).</param>
        /// <param name="scale">The scale parameter α (alpha).</param>
        public void SetParameters(double location, double scale)
        {
            // Validate parameters
            _parametersValid = ValidateParameters(new[] { location, scale }, false) is null;
            _xi = location;
            _alpha = scale;
        }

        /// <summary>
        /// Set the distribution parameters.
        /// </summary>
        /// <param name="parameters">A list of parameters.</param>
        public override void SetParameters(IList<double> parameters)
        {
            SetParameters(parameters[0], parameters[1]);
        }

        /// <summary>
        /// Validate the parameters.
        /// </summary>
        /// <param name="parameters">A list of parameters.</param>
        /// <param name="throwException">Determines whether to throw an exception or not.</param>
        public override ArgumentOutOfRangeException ValidateParameters(IList<double> parameters, bool throwException)
        {
            if (double.IsNaN(parameters[0]) || double.IsInfinity(parameters[0]))
            {
                if (throwException)
                    throw new ArgumentOutOfRangeException(nameof(Xi), "The location parameter ξ (Xi) must be a number.");
                return new ArgumentOutOfRangeException(nameof(Xi), "The location parameter ξ (Xi) must be a number.");
            }
            if (double.IsNaN(parameters[1]) || double.IsInfinity(parameters[1]) || parameters[1] <= 0.0d)
            {
                if (throwException)
                    throw new ArgumentOutOfRangeException(nameof(Alpha), "The scale parameter α (alpha) must be positive.");
                return new ArgumentOutOfRangeException(nameof(Alpha), "The scale parameter α (alpha) must be positive.");
            }
            return null;
        }

        /// <summary>
        /// Returns an array of distribution parameters given the central moments of the sample.
        /// </summary>
        /// <param name="moments">The array of sample linear moments.</param>
        public double[] ParametersFromMoments(IList<double> moments)
        {
            var parms = new double[NumberOfParameters];
            parms[0] = moments[0] - moments[1];
            parms[1] = moments[1];
            return parms;
        }

        /// <summary>
        /// Returns an array of central moments given the distribution parameters.
        /// </summary>
        /// <param name="parameters">The list of distribution parameters.</param>
        public double[] MomentsFromParameters(IList<double> parameters)
        {
            var dist = new Exponential();
            dist.SetParameters(parameters);
            var m1 = dist.Mean;
            var m2 = dist.StandardDeviation;
            var m3 = dist.Skew;
            var m4 = dist.Kurtosis;
            return new[] { m1, m2, m3, m4 };
        }

        /// <summary>
        /// Returns an array of distribution parameters given the linear moments of the sample.
        /// </summary>
        /// <param name="moments">The array of sample linear moments.</param>
        public double[] ParametersFromLinearMoments(IList<double> moments)
        {
            double L1 = moments[0];
            double L2 = moments[1];
            double alpha = 2.0d * L2;
            double xi = L1 - alpha;
            return new[] { xi, alpha };
        }

        /// <summary>
        /// Returns an array of linear moments given the distribution parameters.
        /// </summary>
        /// <param name="parameters">The list of distribution parameters.</param>
        public double[] LinearMomentsFromParameters(IList<double> parameters)
        {
            double xi = parameters[0];
            double alpha = parameters[1];
            double L1 = xi + alpha;
            double L2 = 0.5d * alpha;
            double T3 = 1d / 3d;
            double T4 = 1d / 6d;
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

            // Get initial values
            var moments = Statistics.ProductMoments(sample);
            double minData = Statistics.Minimum(sample);
            initialVals[0] = (sample.Count * minData - moments[0]) / (sample.Count - 1);
            initialVals[1] = sample.Count * (moments[0] - minData) / (sample.Count - 1);

            // Get bounds of location
            if (initialVals[0] == 0d) initialVals[0] = Tools.DoubleMachineEpsilon;
            lowerVals[0] = initialVals[0] - Math.Pow(10d, Math.Ceiling(Math.Log10(Math.Abs(initialVals[0]))));
            upperVals[0] = minData;

            // Get bounds of scale
            lowerVals[1] = Tools.DoubleMachineEpsilon;
            upperVals[1] = Math.Pow(10d, Math.Ceiling(Math.Log10(initialVals[1]) + 1d));

            // Correct initial values if necessary
            if (initialVals[0] <= lowerVals[0] || initialVals[0] >= upperVals[0])
            {
                initialVals[0] = Statistics.Mean(new[] { lowerVals[0], upperVals[0] });
            }
            if (initialVals[1] <= lowerVals[1] || initialVals[1] >= upperVals[1])
            {
                initialVals[1] = Statistics.Mean(new[] { lowerVals[1], upperVals[1] });
            }
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
                var EXP = new Exponential();
                EXP.SetParameters(x);
                return EXP.LogLikelihood(sample);
            }
            var solver = new NelderMead(logLH, NumberOfParameters, Initials, Lowers, Uppers);
            solver.ReportFailure = true;
            solver.Maximize();
            return solver.BestParameterSet.Values;
        }

        /// <summary>
        /// Gets the Probability Density Function (PDF) of the distribution evaluated at a point X.
        /// </summary>
        /// <param name="X">A single point in the distribution range.</param>
        /// <returns>
        /// The probability of X occurring in the distribution.
        /// </returns>
        /// <remarks>
        /// The Probability Density Function (PDF) describes the probability that X will occur.
        /// </remarks>
        public override double PDF(double X)
        {
            // Validate parameters
            if (_parametersValid == false)
                ValidateParameters(new[] { Xi, Alpha }, true);
            if (X < Minimum || X > Maximum) return 0.0d;
            return 1d / Alpha * Math.Exp(-((X - Xi) / Alpha));
        }

        /// <summary>
        /// Gets the Cumulative Distribution Function (CDF) for the distribution evaluated at a point X.
        /// </summary>
        /// <param name="X">A single point in the distribution range.</param>
        /// <returns>
        /// The non-exceedance probability given a point X.
        /// </returns>
        /// <remarks>
        /// The Cumulative Distribution Function (CDF) describes the cumulative probability that a given value or any value smaller than it will occur.
        /// </remarks>
        public override double CDF(double X)
        {
            // Validate parameters
            if (_parametersValid == false)
                ValidateParameters(new[] { Xi, Alpha }, true);
            if (X <= Minimum) return 0d;
            if (X >= Maximum) return 1d;
            return 1d - Math.Exp(-((X - Xi) / Alpha));
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
                ValidateParameters(new[] { Xi, Alpha }, true);
            return Xi - Alpha * Math.Log(1d - probability);
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
                ValidateParameters(new[] { Xi, _alpha }, true);
            double a = Alpha;
            var varList = new List<double>();
            if (estimationMethod == ParameterEstimationMethod.MethodOfMoments)
            {
                varList.Add(a * a / sampleSize); // location
                varList.Add(2d * a * a / sampleSize); // scale
            }
            else if (estimationMethod == ParameterEstimationMethod.MaximumLikelihood)
            {
                varList.Add(a * a / (sampleSize * (sampleSize - 1))); // location
                varList.Add(a * a / (sampleSize - 1)); // scale
            }
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
                ValidateParameters(new[] { Xi, _alpha }, true);
            double a = Alpha;
            var covarList = new List<double>();
            if (estimationMethod == ParameterEstimationMethod.MethodOfMoments)
            {
                covarList.Add(-(a * a) / sampleSize); // location & scale
            }
            else if (estimationMethod == ParameterEstimationMethod.MaximumLikelihood)
            {
                covarList.Add(-(a * a) / (sampleSize * (sampleSize - 1))); // location & scale
            }
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
                ValidateParameters(new[] { Xi, _alpha }, true);
            var partialList = new List<double>
            {
                1.0d, // location
                -Math.Log(1d - probability) // scale
            };
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
            return new Exponential(Xi, Alpha);
        }
   
    }
}