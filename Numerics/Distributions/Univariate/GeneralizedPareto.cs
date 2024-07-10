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

using System;
using System.Collections.Generic;
using System.Linq;
using Numerics.Data.Statistics;
using Numerics.Mathematics.Optimization;
using Numerics.Mathematics.RootFinding;

namespace Numerics.Distributions
{

    /// <summary>
    /// The generalized Pareto distribution.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    /// <see href = "https://en.wikipedia.org/wiki/Generalized_Pareto_distribution" />
    /// </para>
    /// </remarks>
    [Serializable]
    public sealed class GeneralizedPareto : UnivariateDistributionBase, IColesTawn, IEstimation, IMaximumLikelihoodEstimation, ILinearMomentEstimation, IStandardError, IBootstrappable
    {

        /// <summary>
        /// Constructs an Generalized Pareto distribution with a location of 100, scale of 10, and shape of 0.
        /// </summary>
        public GeneralizedPareto()
        {
            SetParameters(new[] { 100d, 10d, 0d });
        }

        /// <summary>
        /// Constructs a Generalized Pareto (GPA) distribution with the given parameters ξ, α, and κ.
        /// </summary>
        /// <param name="location">The location parameter ξ (Xi).</param>
        /// <param name="scale">The scale parameter α (alpha).</param>
        /// <param name="shape">The shape parameter κ (kappa).</param>
        public GeneralizedPareto(double location, double scale, double shape)
        {
            SetParameters(location, scale, shape);
        }

        private bool _parametersValid = true;
        private double _xi; // location
        private double _alpha; // scale
        private double _kappa; // shape
        private double _lambda = 1d;

        /// <summary>
        /// Gets and sets the location parameter ξ (Xi).
        /// </summary>
        public double Xi
        {
            get { return _xi; }
            set
            {
                _parametersValid = ValidateParameters(new[] { value, Alpha, Kappa }, false) is null;
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
                _parametersValid = ValidateParameters(Xi, value, Kappa, false) is null;
                _alpha = value;
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
                _parametersValid = ValidateParameters(new[] { Xi, Alpha, value }, false) is null;
                _kappa = value;
            }
        }

        /// <summary>
        /// Gets and sets the average number of peak per year.
        /// </summary>
        public double Lambda
        {
            get { return _lambda; }
            set { _lambda = value; }
        }

        /// <summary>
        /// Returns the number of distribution parameters.
        /// </summary>
        public override int NumberOfParameters
        {
            get { return 3; }
        }

        /// <summary>
        /// Returns the continuous distribution type.
        /// </summary>
        public override UnivariateDistributionType Type
        {
            get { return UnivariateDistributionType.GeneralizedPareto; }
        }

        /// <summary>
        /// Returns the name of the distribution type as a string.
        /// </summary>
        public override string DisplayName
        {
            get { return "Generalized Pareto"; }
        }

        /// <summary>
        /// Returns the short display name of the distribution as a string.
        /// </summary>
        public override string ShortDisplayName
        {
            get { return "GPA"; }
        }

        /// <summary>
        /// Get distribution parameters in 2-column array of string.
        /// </summary>
        public override string[,] ParametersToString
        {
            get
            {
                var parmString = new string[3, 2];
                parmString[0, 0] = "Location (ξ)";
                parmString[1, 0] = "Scale (α)";
                parmString[2, 0] = "Shape (κ)";
                parmString[0, 1] = Xi.ToString();
                parmString[1, 1] = Alpha.ToString();
                parmString[2, 1] = Kappa.ToString();
                return parmString;
            }
        }

        /// <summary>
        /// Gets the short form parameter names.
        /// </summary>
        public override string[] ParameterNamesShortForm
        {
            get { return new[] { "ξ", "α", "κ" }; }
        }

        /// <summary>
        /// Gets the full parameter names.
        /// </summary>
        public override string[] GetParameterPropertyNames
        {
            get { return new[] { nameof(Xi), nameof(Alpha), nameof(Kappa) }; }
        }

        /// <summary>
        /// Get an array of parameters.
        /// </summary>
        public override double[] GetParameters
        {
            get { return new[] { Xi, Alpha, Kappa }; }
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
                if (Math.Abs(Kappa) <= NearZero)
                {
                    return Xi + Alpha;
                }
                else if (Math.Abs(Kappa) < 1d)
                {
                    return Xi + Alpha / (1d + Kappa);
                }
                else
                {
                    return double.NaN;
                }
            }
        }

        /// <summary>
        /// Gets the median of the distribution.
        /// </summary>
        public override double Median
        {
            get
            {
                if (Math.Abs(Kappa) <= NearZero)
                {
                    return Xi - Math.Log(0.5d) * Alpha;
                }
                else
                {
                    return Xi + Alpha * (Math.Pow(2.0d, -Kappa) - 1d) / Kappa;
                }
            }
        }

        /// <summary>
        /// Gets the mode of the distribution.
        /// </summary>
        public override double Mode
        {
            get
            {
                if (Math.Abs(Kappa) <= NearZero)
                {
                    return Xi;
                }
                else
                {
                    return Xi + Alpha * (Math.Pow(1d + Kappa, -Kappa) - 1d) / Kappa;
                }
            }
        }

        /// <summary>
        /// Gets the standard deviation of the distribution.
        /// </summary>
        public override double StandardDeviation
        {
            get
            {
                if (Math.Abs(Kappa) <= NearZero)
                {
                    return Alpha;
                }
                else if (Math.Abs(Kappa) < 0.5d)
                {
                    return Math.Sqrt(Math.Pow(Alpha, 2d) / ((1d + 2d * Kappa) * Math.Pow(1d + Kappa, 2d)));
                }
                else
                {
                    return double.NaN;
                }
            }
        }

        /// <summary>
        /// Gets the skew of the distribution.
        /// </summary>
        public override double Skew
        {
            get
            {
                if (Math.Abs(Kappa) <= NearZero)
                {
                    return 2.0d;
                }
                else if (Math.Abs(Kappa) < 1d / 3d)
                {
                    double num = 2d * (1d - Kappa) * Math.Sqrt(1d + 2d * Kappa);
                    double den = 1d + 3d * Kappa;
                    return num / den;
                }
                else
                {
                    return double.NaN;
                }
            }
        }

        /// <summary>
        /// Gets the kurtosis of the distribution.
        /// </summary>
        public override double Kurtosis
        {
            get
            {
                if (Math.Abs(Kappa) <= NearZero)
                {
                    return 9.0d;
                }
                else if (Math.Abs(Kappa) < 0.25d)
                {
                    double num = 3d * (1d + 2d * Kappa) * (3d - Kappa + 2d * Math.Pow(Kappa, 2d));
                    double den = (1d + 3d * Kappa) * (1d + 4d * Kappa);
                    return num / den;
                }
                else
                {
                    return double.NaN;
                }
            }
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
            get
            {
                if (Kappa <= NearZero)
                {
                    return double.PositiveInfinity;
                }
                else
                {
                    return Xi + Alpha / Kappa;
                }
            }
        }

        /// <summary>
        /// Gets the minimum values allowable for each parameter.
        /// </summary>
        public override double[] MinimumOfParameters
        {
            get { return new[] { double.NegativeInfinity, 0.0d, double.NegativeInfinity }; }
        }

        /// <summary>
        /// Gets the maximum values allowable for each parameter.
        /// </summary>
        public override double[] MaximumOfParameters
        {
            get { return new[] { double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity }; }
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
                SetParameters(DirectMethodOfMoments(Statistics.ProductMoments(sample)));
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
            var newDistribution = new GeneralizedPareto(Xi, Alpha, Kappa);
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
        /// <param name="shape">The shape parameter κ (kappa).</param>
        public void SetParameters(double location, double scale, double shape)
        {
            _parametersValid = ValidateParameters(location, scale, shape, false) is null;
            Xi = location;
            _alpha = scale;
            Kappa = shape;
        }

        /// <summary>
        /// Set the distribution parameters.
        /// </summary>
        /// <param name="parameters">A list of moments of the log transformed data.</param>
        public override void SetParameters(IList<double> parameters)
        {
            SetParameters(parameters[0], parameters[1], parameters[2]);
        }

        /// <summary>
        /// Validate the parameters.
        /// </summary>
        /// <param name="location">The location parameter ξ (Xi).</param>
        /// <param name="scale">The scale parameter α (alpha).</param>
        /// <param name="shape">The shape parameter κ (kappa).</param>
        /// <param name="throwException">Determines whether to throw an exception or not.</param>
        public ArgumentOutOfRangeException ValidateParameters(double location, double scale, double shape, bool throwException)
        {
            if (double.IsNaN(location) || double.IsInfinity(location))
            {
                if (throwException)
                    throw new ArgumentOutOfRangeException(nameof(Xi), "The location parameter ξ (Xi) must be a number.");
                return new ArgumentOutOfRangeException(nameof(Xi), "The location parameter ξ (Xi) must be a number.");
            }
            if (double.IsNaN(scale) || double.IsInfinity(scale) || scale <= 0.0d)
            {
                if (throwException) throw new ArgumentOutOfRangeException(nameof(Alpha), "The scale parameter α (alpha) must be positive.");
                return new ArgumentOutOfRangeException(nameof(Alpha), "The scale parameter α (alpha) must be positive.");
            }
            if (double.IsNaN(shape) || double.IsInfinity(shape))
            {
                if (throwException)
                    throw new ArgumentOutOfRangeException(nameof(Kappa), "The shape parameter κ (kappa) must be a number.");
                return new ArgumentOutOfRangeException(nameof(Kappa), "The shape parameter κ (kappa) must be a number.");
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
            return ValidateParameters(parameters[0], parameters[1], parameters[2], throwException);
        }

        
        /// <summary>
        /// Gets the parameters using the direct method of moments. Moments are derived from the real-space data.
        /// </summary>
        /// <param name="moments">The array of sample moments.</param>
        public double[] DirectMethodOfMoments(IList<double> moments)
        {

            // Solve for kappa
            double k = SolveforKappa(moments[2]);
            double a;
            double x;
            if (Math.Abs(k) <= NearZero)
            {
                x = moments[0] - moments[1];
                a = moments[1];
            }
            else
            {
                a = Math.Sqrt(moments[1] * moments[1] * Math.Pow(1d + k, 2d) * (1d + 2d * k));
                x = moments[0] - a / (1d + k);
            }

            // return parameters
            return new[] { x, a, k };
        }

        /// <summary>
        /// Returns an array of distribution parameters given the central moments of the sample.
        /// </summary>
        /// <param name="moments">The array of sample linear moments.</param>
        public double[] ParametersFromMoments(IList<double> moments)
        {
            var parms = new double[NumberOfParameters];
            parms[0] = 1d / (moments[0] / Math.Pow(moments[1], 2d));
            parms[1] = Math.Pow(moments[0], 2d) / Math.Pow(moments[1], 2d);
            return parms;
        }

        /// <summary>
        /// Returns an array of central moments given the distribution parameters.
        /// </summary>
        /// <param name="parameters">The list of distribution parameters.</param>
        public double[] MomentsFromParameters(IList<double> parameters)
        {
            var dist = new GeneralizedPareto();
            dist.SetParameters(parameters);
            var m1 = dist.Mean;
            var m2 = dist.StandardDeviation;
            var m3 = dist.Skew;
            var m4 = dist.Kurtosis;
            return new[] { m1, m2, m3, m4 };
        }

        /// <summary>
        /// Estimate parameters using the modified method of moments.
        /// </summary>
        /// <param name="sample">The array of sample data.</param>
        public double[] ModifiedMethodOfMoments(IList<double> sample)
        {
            int N = sample.Count;
            var moments = Statistics.ProductMoments(sample);
            double min = Statistics.Minimum(sample);
            double m1 = moments[0];
            double m2 = Math.Pow(moments[1], 2d);
            double b = (N - 1) * m2 / (m1 - min) - m1;
            double c = m1 * m1 - m2 + 2d * m2 * (m1 - N * min) / (m1 - min);
            double x = -b + Math.Sqrt(b * b - c);
            double k = 0.5d * (Math.Pow(m1 - x, 2d) / m2 - 1d);
            double a = 0.5d * ((m1 - x) * (Math.Pow(m1 - x, 2d) / m2 + 1d));
            // return parameters
            return new[] { x, a, k };
        }


        /// <summary>
        /// Solve for the shape parameter κ (kappa) given the skewness coefficient.
        /// </summary>
        /// <param name="skew">The skewness coefficient</param>
        /// <returns>
        /// Kappa
        /// </returns>
        public double SolveforKappa(double skew)
        {
            if (Math.Abs(skew) < 10d)
            {
                // Kappa must be solved for. The Brent method is used here. 
                return Brent.Solve((x) =>
                {
                    double k = 2d * (1d - x) * Math.Sqrt(1d + 2d * x) / (1d + 3d * x);
                    return k - skew;
                }, -(1d / 3d), 1d / 3d);
            }
            else
            {
                return double.NaN;
            }
        }

        /// <summary>
        /// Returns an array of distribution parameters given the linear moments of the sample.
        /// </summary>
        /// <param name="moments">The array of sample linear moments.</param>
        public double[] ParametersFromLinearMoments(IList<double> moments)
        {
            double L1 = moments[0];
            double L2 = moments[1];
            double T3 = moments[2];
            double T4 = moments[3];
            double kappa = (1.0d - 3.0d * T3) / (1.0d + T3);
            double alpha = (1.0d + kappa) * (2.0d + kappa) * L2;
            double xi = L1 - (2.0d + kappa) * L2;
            return new[] { xi, alpha, kappa };
        }

        /// <summary>
        /// Returns an array of linear moments given the distribution parameters.
        /// </summary>
        /// <param name="parameters">The list of distribution parameters.</param>
        public double[] LinearMomentsFromParameters(IList<double> parameters)
        {
            double xi = parameters[0];
            double alpha = parameters[1];
            double kappa = parameters[2];
            if (kappa <= -1.0d)
                throw new ArgumentOutOfRangeException(nameof(Kappa), "L-moments can only be defined for kappa > -1.");
            double L1 = xi + alpha / (1.0d + kappa);
            double L2 = alpha / ((1.0d + kappa) * (2.0d + kappa));
            double T3 = (1.0d - kappa) / (3.0d + kappa);
            double T4 = (1.0d - kappa) * (2.0d - kappa) / ((3.0d + kappa) * (4.0d + kappa));
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
            // 
            // Get initial values
            initialVals = ParametersFromLinearMoments(Statistics.LinearMoments(sample));
            double minData = Statistics.Minimum(sample);
            // Get bounds of location
            if (initialVals[0] == 0d) initialVals[0] = Tools.DoubleMachineEpsilon;
            lowerVals[0] = initialVals[0] - Math.Pow(10d, Math.Ceiling(Math.Log10(Math.Abs(initialVals[0]))));
            upperVals[0] = minData + Tools.DoubleMachineEpsilon;

            // Get bounds of scale
            lowerVals[1] = Tools.DoubleMachineEpsilon;
            upperVals[1] = Math.Pow(10d, Math.Ceiling(Math.Log10(Math.Abs(initialVals[1])) + 1d));
            // Get bounds of shape
            lowerVals[2] = -10d;
            upperVals[2] = 10d;
            // Correct initial values if necessary
            if (initialVals[0] <= lowerVals[0] || initialVals[0] >= upperVals[0])
            {
                initialVals[0] = Statistics.Mean(new[] { lowerVals[0], upperVals[0] });
            }
            if (initialVals[1] <= lowerVals[1] || initialVals[1] >= upperVals[1])
            {
                initialVals[1] = Statistics.Mean(new[] { lowerVals[1], upperVals[1] });
            }
            if (initialVals[2] <= lowerVals[2] || initialVals[2] >= upperVals[2])
            {
                initialVals[2] = 0d;
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

            var xi = Statistics.Minimum(sample);

            // Solve using Nelder-Mead (Downhill Simplex)
            double logLH(double[] x)
            {
                var GPA = new GeneralizedPareto();
                GPA.SetParameters(new[] { xi, x[0], x[1] });
                //GPA.SetParameters(x);
                return GPA.LogLikelihood(sample);
            }
            var solver = new NelderMead(logLH, 2, Initials.Subset(1), Lowers.Subset(1), Uppers.Subset(1));
            solver.ReportFailure = true;
            solver.Maximize();
            return new double[] { xi, solver.BestParameterSet.Values[0], solver.BestParameterSet.Values[1] };
            //var solver = new NelderMead(logLH, NumberOfParameters, Initials, Lowers, Uppers);
            //solver.ReportFailure = true;
            //solver.Maximize();
            //return solver.BestParameterSet.Values;

        }

        /// <summary>
        /// Gets the Probability Density Function (PDF) of the distribution evaluated at a point X.
        /// </summary>
        /// <param name="x">A single point in the distribution range.</param>
        public override double PDF(double x)
        {
            // Validate parameters
            if (_parametersValid == false)
                ValidateParameters(Xi, Alpha, Kappa, true);
            if (x < Minimum || x > Maximum) return 0.0d;
            double y = (x - Xi) / Alpha;
            if (Math.Abs(Kappa) > NearZero)
                y = -Math.Log(1d - Kappa * y) / Kappa;
            return Math.Exp(-(1d - Kappa) * y) / Alpha;
        }

        /// <summary>
        /// Gets the Cumulative Distribution Function (CDF) for the distribution evaluated at a point X.
        /// </summary>
        /// <param name="x">A single point in the distribution range.</param>
        /// <returns>
        /// The non-exceedance probability given a point X.
        /// </returns>
        /// <remarks>
        /// The Cumulative Distribution Function (CDF) describes the cumulative probability that a given value or any value smaller than it will occur.
        /// </remarks>
        public override double CDF(double x)
        {
            // Validate parameters
            if (_parametersValid == false)
                ValidateParameters(Xi, Alpha, Kappa, true);
            if (x <= Minimum)
                return 0d;
            if (x >= Maximum)
                return 1d;
            double y = (x - Xi) / Alpha;
            if (Math.Abs(Kappa) > NearZero)
                y = -Math.Log(1d - Kappa * y) / Kappa;
            return 1d - Math.Exp(-y);
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
                ValidateParameters(Xi, Alpha, Kappa, true);
            if (Math.Abs(Kappa) <= NearZero)
            {
                return Xi - Alpha * Math.Log(1d - probability);
            }
            else
            {
                return Xi + Alpha / Kappa * (1d - Math.Pow(1d - probability, Kappa));
            }
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
            double k = Kappa;
            int N = sampleSize;
            var varList = new List<double>();
            varList.Add(N * a * a / ((N + 2d * k) * Math.Pow(N + k, 2d))); // location
            if (estimationMethod == ParameterEstimationMethod.MethodOfMoments)
            {
                double num = Math.Pow(1d + k, 2d) * (1d + 6d * k + 12d * Math.Pow(k, 2d));
                double den = (1d + 2d * k) * (1d + 3d * k) * (1d + 4d * k);
                varList.Add(2d * a * a / N * num / den); // scale
                // 
                num = Math.Pow(1d + k, 2d) * Math.Pow(1d + 2d * k, 2d) * (1d + k + 6d * Math.Pow(k, 2d));
                varList.Add(1d / N * num / den); // location
            }
            else if (estimationMethod == ParameterEstimationMethod.MaximumLikelihood)
            {
                varList.Add((1d - k) * (2d * a * a) / N); // scale
                varList.Add(1d / N * Math.Pow(1d - k, 2d)); // location
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
            double k = Kappa;
            int N = sampleSize;
            var covarList = new List<double>();
            if (estimationMethod == ParameterEstimationMethod.MethodOfMoments)
            {
                double num = Math.Pow(1d + k, 2d) * (1d + 2d * k) * (1d + 4d * k + 12d * Math.Pow(k, 2d));
                double den = (1d + 2d * k) * (1d + 3d * k) * (1d + 4d * k);
                covarList.Add(a / N * num / den); // scale & shape
            }
            else if (estimationMethod == ParameterEstimationMethod.MaximumLikelihood)
            {
                covarList.Add(a / N * (1d - k)); // scale & shape
            }

            return covarList;
        }

        /// <summary>
        /// Returns a list of partial derivatives of X given probability with respect to each parameter.
        /// </summary>
        /// <param name="probability">Probability between 0 and 1.</param>
        public IList<double> PartialDerivatives(double probability)
        {
            if (_parametersValid == false)
                ValidateParameters(Xi, _alpha, Kappa, true);
            double p = probability;
            double a = Alpha;
            double k = Kappa;
            var partialList = new List<double>();
            partialList.Add(1.0d); // location
            partialList.Add(1d / k * (1d - Math.Pow(1d - p, Kappa))); // scale
            partialList.Add(-a / (k * k) * (1d - Math.Pow(1d - p, k)) - a / k * Math.Log(1d - p) * Math.Pow(1d - p, k)); // shape
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
            // The variance in the location parameter is of order N-2 and thus
            // can be neglected relative to the variances of scale and shape (order N-1)
            // to obtain approximate confidence intervals. 
            double varA = ParameterVariance(sampleSize, estimationMethod)[1];
            double varB = ParameterVariance(sampleSize, estimationMethod)[2];
            double covAB = ParameterCovariance(sampleSize, estimationMethod)[0];
            double pXA = PartialDerivatives(probability)[1];
            double pXB = PartialDerivatives(probability)[2];
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
            // |a b c|
            // |d e f|
            // |g h i|
            // |A| = a(ei − fh) − b(di − fg) + c(dh − eg)
            var dXt1 = PartialDerivatives(probabilities[0]).ToArray();
            var dXt2 = PartialDerivatives(probabilities[1]).ToArray();
            var dXt3 = PartialDerivatives(probabilities[2]).ToArray();
            double a = dXt1[0];
            double b = dXt1[1];
            double c = dXt1[2];
            double d = dXt2[0];
            double e = dXt2[1];
            double f = dXt2[2];
            double g = dXt3[0];
            double h = dXt3[1];
            double i = dXt3[2];
            return a * (e * i - f * h) - b * (d * i - f * g) + c * (d * h - e * g);
        }

        /// <summary>
        /// Creates a copy of the distribution.
        /// </summary>
        public override UnivariateDistributionBase Clone()
        {
            return new GeneralizedPareto(Xi, Alpha, Kappa) { Lambda = Lambda };
        }

    }
}