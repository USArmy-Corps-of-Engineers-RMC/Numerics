using System;
using System.Collections.Generic;
using System.Linq;
using Numerics.Mathematics.Optimization;
using Numerics.Mathematics.SpecialFunctions;

namespace Numerics.Distributions
{

    /// <summary>
    /// The Weibull probability distribution.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    /// <see href = "https://en.wikipedia.org/wiki/Weibull_distribution" />
    /// </para>
    /// </remarks>
    [Serializable]
    public class Weibull : UnivariateDistributionBase, IColesTawn, IEstimation, IMaximumLikelihoodEstimation, IStandardError, IBootstrappable
    {
      
        /// <summary>
        /// Constructs a Weibull distribution with scale = 10 and shape = 2.
        /// </summary>
        public Weibull()
        {
            SetParameters(10d, 2d);
        }

        /// <summary>
        /// Constructs a Weibull distribution with the given parameters λ and k.
        /// </summary>
        /// <param name="scale">The scale parameter λ (lambda). Range: λ > 0.</param>
        /// <param name="shape">The shape parameter κ (kappa). Range: k > 0.</param>
        public Weibull(double scale, double shape)
        {
            SetParameters(scale, shape);
        }

        private bool _parametersValid = true;
        private double _lambda;
        private double _kappa;

        /// <summary>
        /// Gets and sets the scale parameter λ (lambda).
        /// </summary>
        public double Lambda
        {
            get { return _lambda; }
            set
            {
                _parametersValid = ValidateParameters(value, Kappa, false) is null;
                _lambda = value;
            }
        }

        /// <summary>
        /// Gets and sets the shape parameter κ (kappa).
        /// </summary>
        public double Kappa
        {
            get { return _kappa; }
            set
            {
                _parametersValid = ValidateParameters(Lambda, value, false) is null;
                _kappa = value;
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
            get { return UnivariateDistributionType.Weibull; }
        }

        /// <summary>
        /// Returns the name of the distribution type as a string.
        /// </summary>
        public override string DisplayName
        {
            get { return "Weibull"; }
        }

        /// <summary>
        /// Returns the short display name of the distribution as a string.
        /// </summary>
        public override string ShortDisplayName
        {
            get { return "W"; }
        }

        /// <summary>
        /// Get distribution parameters in 2-column array of string.
        /// </summary>
        public override string[,] ParametersToString
        {
            get
            {
                var parmString = new string[2, 2];
                parmString[0, 0] = "Scale (λ)";
                parmString[1, 0] = "Shape (κ)";
                parmString[0, 1] = Lambda.ToString();
                parmString[1, 1] = Kappa.ToString();
                return parmString;
            }
        }

        /// <summary>
        /// Gets the short form parameter names.
        /// </summary>
        public override string[] ParameterNamesShortForm
        {
            get { return new[] { "λ", "κ" }; }
        }

        /// <summary>
        /// Gets the full parameter names.
        /// </summary>
        public override string[] GetParameterPropertyNames
        {
            get { return new[] { nameof(Lambda), nameof(Kappa) }; }
        }

        /// <summary>
        /// Get an array of parameters.
        /// </summary>
        public override double[] GetParameters
        {
            get { return new[] { Lambda, Kappa }; }
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
            get { return Lambda * Gamma.Function(1.0d + 1.0d / Kappa); }
        }

        /// <summary>
        /// Gets the median of the distribution.
        /// </summary>
        public override double Median
        {
            get { return Lambda * Math.Pow(Math.Log(2.0d), 1.0d / Kappa); }
        }

        /// <summary>
        /// Gets the mode of the distribution.
        /// </summary>
        public override double Mode
        {
            get
            {
                if (Kappa <= 1.0d)
                {
                    return 0.0d;
                }
                else
                {
                    return Lambda * Math.Pow((Kappa - 1.0d) / Kappa, 1.0d / Kappa);
                }
            }
        }

        /// <summary>
        /// Gets the standard deviation of the distribution.
        /// </summary>
        public override double StandardDeviation
        {
            get { return Math.Sqrt(Lambda * Lambda * Gamma.Function(1.0d + 2.0d / Kappa) - Mean * Mean); }
        }

        /// <summary>
        /// Gets the skew of the distribution.
        /// </summary>
        public override double Skew
        {
            get
            {
                double mu = Mean;
                double sigma = StandardDeviation;
                return (Gamma.Function(1.0d + 3.0d / Kappa) * Math.Pow(Lambda, 3.0d) - 3.0d * mu * sigma * sigma - Math.Pow(mu, 3.0d)) / Math.Pow(sigma, 3.0d);
            }
        }

        /// <summary>
        /// Gets the kurtosis of the distribution.
        /// </summary>
        public override double Kurtosis
        {
            get
            {
                double g1 = Gamma.Function(1d + 1d / Kappa);
                double g2 = Gamma.Function(1d + 2d / Kappa);
                double g3 = Gamma.Function(1d + 3d / Kappa);
                double g4 = Gamma.Function(1d + 4d / Kappa);
                double num = -6 * Math.Pow(g1, 4d) + 12d * g2 * Math.Pow(g1, 2d) - 3d * Math.Pow(g2, 2d) - 4d * g1 * g3 + g4;
                double den = Math.Pow(g2 - Math.Pow(g1, 2d), 2d);
                return 3d + num / den;
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
                throw new NotImplementedException();
            }
            else if (estimationMethod == ParameterEstimationMethod.MethodOfLinearMoments)
            {
                throw new NotImplementedException();
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
            var newDistribution = new Weibull(Lambda, Kappa);
            var sample = newDistribution.GenerateRandomValues(seed, sampleSize);
            newDistribution.Estimate(sample, estimationMethod);
            if (newDistribution.ParametersValid == false)
                throw new Exception("Bootstrapped distribution parameters are invalid.");
            return newDistribution;
        }

        /// <summary>
        /// Set the distribution parameters.
        /// </summary>
        /// <param name="scale">The scale parameter λ (lambda). Range: λ > 0.</param>
        /// <param name="shape">The shape parameter κ (kappa). Range: k > 0.</param>
        public void SetParameters(double scale, double shape)
        {
            Lambda = scale;
            Kappa = shape;
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
        /// <param name="scale">The scale parameter λ (lambda). Range: λ > 0.</param>
        /// <param name="shape">The shape parameter κ (kappa). Range: k > 0.</param>
        /// <param name="throwException">Determines whether to throw an exception or not.</param>
        public ArgumentOutOfRangeException ValidateParameters(double scale, double shape, bool throwException)
        {
            if (double.IsNaN(scale) || double.IsInfinity(scale) || scale <= 0.0d)
            {
                if (throwException)
                    throw new ArgumentOutOfRangeException(nameof(Lambda), "The scale parameter λ (lambda) must be positive.");
                return new ArgumentOutOfRangeException(nameof(Lambda), "The scale parameter λ (lambda) must be positive.");
            }
            if (double.IsNaN(shape) || double.IsInfinity(shape) || shape <= 0.0d)
            {
                if (throwException)
                    throw new ArgumentOutOfRangeException(nameof(Kappa), "The shape parameter κ (kappa) must be positive.");
                return new ArgumentOutOfRangeException(nameof(Kappa), "The shape parameter κ (kappa) must be positive.");
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
            // Get initial values
            initialVals = SolveMLE(sample);
            // Get bounds of scale
            lowerVals[0] = Tools.DoubleMachineEpsilon;
            upperVals[0] = Math.Pow(10d, Math.Ceiling(Math.Log10(initialVals[0]) + 1d));
            // Get bounds of shape
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
                var W = new Weibull();
                W.SetParameters(x);
                return W.LogLikelihood(sample);
            }
            var solver = new NelderMead(logLH, NumberOfParameters, Initials, Lowers, Uppers);
            solver.ReportFailure = true;
            solver.Maximize();
            return solver.BestParameterSet.Values;
        }

        /// <summary>
        /// The Maximum Likelihood Estimation method for the Weibull distribution.
        /// </summary>
        /// <param name="samples">The array of sample data.</param>
        /// <remarks>
        /// Implemented according to: Parameter estimation of the Weibull probability distribution, 1994, Hongzhu Qiao, Chris P. Tsokos
        /// <para>
        /// References:
        /// This code was copied and modified from the Math.NET Library.
        /// <list type="bullet">
        /// <item><description>
        /// Math.NET Numerics Library, http://numerics.mathdotnet.com
        /// </description></item>
        /// </list>
        /// </para>
        /// </remarks>
        public double[] SolveMLE(IList<double> samples)
        {
            var data = samples as double[] ?? samples.ToArray();
            double n = data.Length;
            if (n <= 1d)
            {
                throw new Exception("Observations not sufficient. There must be more than 1 data point.");
            }

            double s1 = 0d;
            double s2 = 0d;
            double s3 = 0d;
            double previousC = int.MinValue;
            double QofC = 0d;
            double c = 10d; // shape
            double b = 0d; // scale

            // solve for the shape parameter
            while (Math.Abs(c - previousC) >= 0.0001d)
            {
                s1 = 0d;
                s2 = 0d;
                s3 = 0d;
                foreach (double x in data)
                {
                    if (x > 0d)
                    {
                        s1 += Math.Log(x);
                        s2 += Math.Pow(x, c);
                        s3 += Math.Pow(x, c) * Math.Log(x);
                    }
                }

                QofC = n * s2 / (n * s3 - s1 * s2);
                previousC = c;
                c = (c + QofC) / 2d;
            }

            // solve for scale
            foreach (double x in data)
            {
                if (x > 0d)
                {
                    b += Math.Pow(x, c);
                }
            }

            b = Math.Pow(b / n, 1d / c);

            // return parameters
            return new[] { b, c };
        }
     
        /// <summary>
        /// The Probability Density Function (PDF) of the distribution evaluated at a point X.
        /// </summary>
        /// <param name="x">A single point in the distribution range.</param>
        public override double PDF(double x)
        {
            // Validate parameters
            if (_parametersValid == false)
                ValidateParameters(Lambda, Kappa, true);
            if (x < Minimum) return 0.0d;
            if (x == 0.0d && Kappa == 1.0d)
            {
                return Kappa / Lambda;
            }
            else
            {
                return Kappa / Lambda * Math.Pow(x / Lambda, Kappa - 1.0d) * Math.Exp(-Math.Pow(x / Lambda, Kappa));
            }
        }

        /// <summary>
        /// The Cumulative Distribution Function (CDF) for the distribution evaluated at a point X.
        /// </summary>
        /// <param name="x">A single point in the distribution range.</param>
        public override double CDF(double x)
        {
            // Validate parameters
            if (_parametersValid == false)
                ValidateParameters(Lambda, Kappa, true);
            if (x < Minimum)
                return 0d;
            return 1d - Math.Exp(-Math.Pow(x / Lambda, Kappa));
        }

        /// <summary>
        /// Gets the Inverse Cumulative Distribution Function (ICFD) of the distribution evaluated at a probability.
        /// </summary>
        /// <param name="probability">Probability between 0 and 1.</param>
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
                ValidateParameters(Lambda, Kappa, true);
            // Compute the inverse CDF
            return Lambda * Math.Pow(Math.Log(1d / (1d - probability)), 1d / Kappa);
        }

        /// <summary>
        /// Returns a list containing the variance of each parameter given the sample size.
        /// </summary>
        /// <param name="sampleSize">The sample size.</param>
        /// <param name="estimationMethod">The distribution parameter estimation method.</param>
        public IList<double> ParameterVariance(int sampleSize, ParameterEstimationMethod estimationMethod)
        {
            // lambda is scale a
            // kappa is shape b
            if (estimationMethod == ParameterEstimationMethod.MethodOfMoments)
            {
                throw new NotImplementedException();
            }
            // Validate parameters
            if (_parametersValid == false)
                ValidateParameters(_lambda, _kappa, true);
            double a = Lambda;
            double b = Kappa;
            var varList = new List<double>();
            varList.Add(1.108665d * a * a / (sampleSize * b * b)); // scale
            varList.Add(0.607927d * b * b / sampleSize); // shape
            return varList;
        }

        /// <summary>
        /// Returns a list containing the covariances of the parameters given the sample size.
        /// </summary>
        /// <param name="sampleSize">The sample size.</param>
        /// <param name="estimationMethod">The distribution parameter estimation method.</param>
        public IList<double> ParameterCovariance(int sampleSize, ParameterEstimationMethod estimationMethod)
        {
            if (estimationMethod == ParameterEstimationMethod.MethodOfMoments)
            {
                throw new NotImplementedException();
            }
            // Validate parameters
            if (_parametersValid == false)
                ValidateParameters(_lambda, _kappa, true);
            double a = Lambda;
            double b = Kappa;
            var covarList = new List<double>();
            covarList.Add(0.257022d * a / sampleSize); // scale & shape
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
                ValidateParameters(_lambda, _kappa, true);
            double a = Lambda;
            double b = Kappa;
            var partialList = new List<double>();
            partialList.Add(Math.Log(Math.Pow(1d / (1d - probability), 1d / Kappa))); // scale
            partialList.Add(a * Math.Log(1d - probability) / (b * b)); // shape
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
            return new Weibull(Lambda, Kappa);
        }

    }
}