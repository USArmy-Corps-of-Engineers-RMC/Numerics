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
using Numerics.Mathematics;
using Numerics.Mathematics.LinearAlgebra;
using Numerics.Mathematics.Optimization;
using Numerics.Mathematics.RootFinding;
using Numerics.Mathematics.SpecialFunctions;

namespace Numerics.Distributions
{

    /// <summary>
    /// The log-Pearson Type III distribution.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    /// <see href = "http://mathworld.wolfram.com/PearsonTypeIIIDistribution.html" />
    /// </para>
    /// </remarks>
    [Serializable]
    public sealed class LogPearsonTypeIII : UnivariateDistributionBase, IColesTawn, IEstimation, IMaximumLikelihoodEstimation, IMomentEstimation, ILinearMomentEstimation, IStandardError, IBootstrappable
    {
      
        /// <summary>
        /// Constructs a log-Pearson Type III distribution with a mean (of log) of 3, std dev (of log) of 0.5, and skew (of log) of 0.
        /// </summary>
        public LogPearsonTypeIII()
        {
            SetParameters(3d, 0.5d, 0d);
        }

        /// <summary>
        /// Constructs a log-Pearson Type III distribution with the given moments (of log) µ, σ, and γ.
        /// </summary>
        /// <param name="meanOfLog">The mean of the log transformed data.</param>
        /// <param name="standardDeviationOfLog">The standard deviation of the log transformed data.</param>
        /// <param name="skewOfLog">The skew of the log transformed data.</param>
        public LogPearsonTypeIII(double meanOfLog, double standardDeviationOfLog, double skewOfLog)
        {
            SetParameters(meanOfLog, standardDeviationOfLog, skewOfLog);
        }

        private double _mu;
        private double _sigma;
        private double _gamma;
        private double _base = 10d;
        private bool _parametersValid = true;

        /// <summary>
        /// Gets and sets the Mean (of log) of the distribution.
        /// </summary>
        public double Mu
        {
            get { return _mu; }
            set
            {
                _parametersValid = ValidateParameters(value, Sigma, Gamma, false) is null;
                SetParameters(value, Sigma, Gamma);
            }
        }

        /// <summary>
        /// Gets and sets the Standard Deviation (of log) of the distribution.
        /// </summary>
        public double Sigma
        {
            get { return _sigma; }
            set
            {
                if (value < 1E-16 && Math.Sign(value) != -1) value = 1E-16;
                _parametersValid = ValidateParameters(Mu, value, Gamma, false) is null;
                SetParameters(Mu, value, Gamma);
            }
        }

        /// <summary>
        /// Gets and sets the Skew (of log) of the distribution.
        /// </summary>
        public double Gamma
        {
            get { return _gamma; }
            set
            {
                _parametersValid = ValidateParameters(Mu, Sigma, value, false) is null;
                SetParameters(Mu, Sigma, value);
            }
        }

        /// <summary>
        /// Gets the location parameter ξ (Xi).
        /// </summary>
        public double Xi
        {
            get { return Mu - 2.0d * Sigma / Gamma; }
        }

        /// <summary>
        /// Gets and sets the scale parameter β (beta).
        /// </summary>
        public double Beta
        {
            get { return 0.5d * Sigma * Gamma; }
        }

        /// <summary>
        /// Gets and sets the shape parameter α (alpha).
        /// </summary>
        public double Alpha
        {
            get { return 4d / (Gamma * Gamma); }
        }

        /// <summary>
        /// Gets and sets the base of the logarithm
        /// </summary>
        public double Base
        {
            get { return _base;}
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
            get { return 3; }
        }

        /// <summary>
        /// Returns the continuous distribution type.
        /// </summary>
        public override UnivariateDistributionType Type
        {
            get { return UnivariateDistributionType.LogPearsonTypeIII; }
        }

        /// <summary>
        /// Returns the name of the distribution type as a string.
        /// </summary>
        public override string DisplayName
        {
            get { return "Log-Pearson Type III"; }
        }

        /// <summary>
        /// Returns the short display name of the distribution as a string.
        /// </summary>
        public override string ShortDisplayName
        {
            get { return "LPIII"; }
        }

        /// <summary>
        /// Get distribution parameters in 2-column array of string.
        /// </summary>
        public override string[,] ParametersToString
        {
            get
            {
                var parmString = new string[3, 2];
                parmString[0, 0] = "Mean (of log) (µ)";
                parmString[1, 0] = "Std Dev (of log) (σ)";
                parmString[2, 0] = "Skew (of log) (γ)";
                parmString[0, 1] = Mu.ToString();
                parmString[1, 1] = Sigma.ToString();
                parmString[2, 1] = Gamma.ToString();
                return parmString;
            }
        }

        /// <summary>
        /// Gets the short form parameter names.
        /// </summary>
        public override string[] ParameterNamesShortForm
        {
            get { return new[] { "µ", "σ", "γ" }; }
        }

        /// <summary>
        /// Gets the full parameter names.
        /// </summary>
        public override string[] GetParameterPropertyNames
        {
            get { return new[] { nameof(Mu), nameof(Sigma), nameof(Gamma) }; }
        }

        /// <summary>
        /// Get an array of parameters.
        /// </summary>
        public override double[] GetParameters
        {
            get { return new[] { Mu, Sigma, Gamma }; }
        }

        /// <summary>
        /// Determines whether the parameters are valid or not.
        /// </summary>
        public override bool ParametersValid
        {
            get { return _parametersValid; }
        }

        public override double[] CentralMoments(double tolerance = 1E-8)
        {
            return new double[] { Mu, Sigma, Gamma };
        }

        /// <summary>
        /// Gets the mean of the distribution.
        /// </summary>
        public override double Mean
        {
            get
            {
                if (Math.Abs(Gamma) <= NearZero)
                {
                    // Use LogNormal
                    return Math.Exp((Mu + Sigma * Sigma / 2.0d) / K);
                }
                else
                {
                    return Math.Exp(Xi / K) / Math.Pow(1d - Beta / K, Alpha);
                }
            }
        }

        /// <summary>
        /// Gets the median of the distribution.
        /// </summary>
        public override double Median
        {
            get { return InverseCDF(0.5d); }
        }

        /// <summary>
        /// Gets the mode of the distribution.
        /// </summary>
        public override double Mode
        {
            get
            {
                if (Math.Abs(Gamma) <= NearZero)
                {
                    // Use LogNormal
                    return Math.Exp((Mu - Sigma * Sigma) / K);
                }
                else
                {
                    return Math.Exp((Xi + (Alpha - 1d) * Beta) / K);
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
                if (Math.Abs(Gamma) <= NearZero)
                {
                    // Use LogNormal
                    return Math.Sqrt((Math.Exp(Sigma * Sigma / K) - 1.0d) * Math.Exp((2d * Mu + Sigma * Sigma) / K));
                }
                else
                {
                    return Math.Sqrt(Math.Pow(Math.Pow(1d - Beta / K, 2d) / (1d - 2d * Beta / K), Alpha) - 1d) * Mean;
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
                if (Math.Abs(Gamma) <= NearZero)
                {
                    // Use LogNormal
                    return (Math.Exp(Sigma * Sigma / K) + 2.0d) * Math.Sqrt(Math.Exp(Sigma * Sigma / K) - 1d);
                }
                else
                {
                    return (Math.Pow(1d - 3d * Beta / K, -Alpha) - 3d * Math.Pow(1d - 2d * Beta / K, -Alpha) * Math.Pow(1d - Beta / K, -Alpha) + 2d * Math.Pow(1d - Beta / K, -3 * Alpha)) / Math.Pow(Math.Pow(1d - 2d * Beta / K, -Alpha) - Math.Pow(1d - Beta / K, -2 * Alpha), 3d / 2d);
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
                if (Math.Abs(Gamma) <= NearZero)
                {
                    // Use LogNormal
                    double siqma2 = Math.Pow(Sigma, 2d);
                    return 3d + (Math.Exp(4d * siqma2 / K) + 2d * Math.Exp(3d * siqma2 / K) + 3d * Math.Exp(2d * siqma2 / K) - 6d);
                }
                else
                {
                    return (Math.Pow(1d - 4d * Beta / K, -Alpha) - 4d * Math.Pow(1d - 3d * Beta / K, -Alpha) * Math.Pow(1d - Beta / K, -Alpha) + 6d * Math.Pow(1d - 2d * Beta / K, -Alpha) * Math.Pow(1d - Beta / K, -2 * Alpha) - 3d * Math.Pow(1d - Beta / K, -4 * Alpha)) / Math.Pow(Math.Pow(1d - 2d * Beta / K, -Alpha) - Math.Pow(1d - Beta / K, -2 * Alpha), 2d);
                }
            }
        }


        /// <summary>
        /// Gets the minimum of the distribution.
        /// </summary>
        public override double Minimum
        {
            get
            {
                if (Math.Abs(Gamma) <= NearZero)
                {
                    // Use LogNormal
                    return 0.0d;
                }
                else if (Beta > 0.0d)
                {
                    return Math.Exp(Xi / K);
                }
                else
                {
                    return 0.0d;
                }
            }
        }

        /// <summary>
        /// Gets the maximum of the distribution.
        /// </summary>
        public override double Maximum
        {
            get
            {
                if (Math.Abs(Gamma) <= NearZero)
                {
                    // Use LogNormal
                    return double.PositiveInfinity;
                }
                else if (Beta > 0.0d)
                {
                    return double.PositiveInfinity;
                }
                else
                {
                    return Math.Exp(Xi / K);
                }
            }
        }

        /// <summary>
        /// Gets the minimum values allowable for each parameter.
        /// </summary>
        public override double[] MinimumOfParameters
        {
            get { return new[] { 0.0d, 0.0d, double.NegativeInfinity }; }
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
            var newDistribution = new LogPearsonTypeIII(Mu, Sigma, Gamma);
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
        /// <param name="skewOfLog">The skew of the log transformed data.</param>
        public void SetParameters(double meanOfLog, double standardDeviationOfLog, double skewOfLog)
        {
            _parametersValid = ValidateParameters(meanOfLog, standardDeviationOfLog, skewOfLog, false) is null;
            _mu = meanOfLog;
            _sigma = standardDeviationOfLog;
            _gamma = skewOfLog;
        }

        /// <summary>
        /// Set the distribution parameters based on the moments of the log transformed data.
        /// </summary>
        /// <param name="parameters">A list of moments of the log transformed data.</param>
        public override void SetParameters(IList<double> parameters)
        {
            SetParameters(parameters[0], parameters[1], parameters[2]);
        }

        /// <summary>
        /// Validate the parameters.
        /// </summary>
        /// <param name="mu">The mean of the log transformed data.</param>
        /// <param name="sigma">The standard deviation of the log transformed data.</param>
        /// <param name="gamma">The skew of the log transformed data.</param>
        /// <param name="throwException">Determines whether to throw an exception or not.</param>
        public ArgumentOutOfRangeException ValidateParameters(double mu, double sigma, double gamma, bool throwException)
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
            if (double.IsNaN(gamma) || double.IsInfinity(gamma))
            {
                if (throwException)
                    throw new ArgumentOutOfRangeException(nameof(Gamma), "Gamma must be a number.");
                return new ArgumentOutOfRangeException(nameof(Gamma), "Gamma must be a number.");
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
                    transformedSample.Add(Math.Log(0.01d, Base));
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
                    transformedSample.Add(Math.Log(0.01d, Base));
                }
            }
            return Statistics.LinearMoments(transformedSample);
        }

        /// <summary>
        /// Returns an array of distribution parameters given the central moments of the sample.
        /// </summary>
        /// <param name="moments">The array of sample linear moments.</param>
        public double[] ParametersFromMoments(IList<double> moments)
        {
            return moments.ToArray().Subset(0, 2);
        }

        /// <summary>
        /// Returns an array of central moments given the distribution parameters.
        /// </summary>
        /// <param name="parameters">The list of distribution parameters.</param>
        public double[] MomentsFromParameters(IList<double> parameters)
        {
            var dist = new LogPearsonTypeIII();
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
            double T3 = moments[2];
            double T4 = moments[3];
            var alpha = default(double);
            double z;
            // The following approximation has relative accuracy better than 5x10-5 for all values of alpha.
            if (Math.Abs(T3) > 0.0d && Math.Abs(T3) < 1d / 3d)
            {
                z = 3.0d * Math.PI * Math.Pow(T3, 2d);
                alpha = (1.0d + 0.2906d * z) / (z + 0.1882d * Math.Pow(z, 2d) + 0.0442d * Math.Pow(z, 3d));
            }
            else if (Math.Abs(T3) >= 1d / 3d && Math.Abs(T3) < 1.0d)
            {
                z = 1.0d - Math.Abs(T3);
                alpha = (0.36067d * z - 0.59567d * Math.Pow(z, 2d) + 0.25361d * Math.Pow(z, 3d)) / (1.0d - 2.78861d * z + 2.56096d * Math.Pow(z, 2d) - 0.77045d * Math.Pow(z, 3d));
            }

            double mu = L1;
            double gamma = 2.0d * Math.Pow(alpha, -0.5d) * Math.Sign(T3);
            double sigma = L2 * Math.Pow(Math.PI, 0.5d) * Math.Pow(alpha, 0.5d) * Mathematics.SpecialFunctions.Gamma.Function(alpha) / Mathematics.SpecialFunctions.Gamma.Function(alpha + 0.5d);
            if (alpha < 100d)
            {
                sigma = L2 * Math.Pow(Math.PI, 0.5d) * Math.Pow(alpha, 0.5d) * Mathematics.SpecialFunctions.Gamma.Function(alpha) / Mathematics.SpecialFunctions.Gamma.Function(alpha + 0.5d);
            }
            else
            {
                sigma = Math.Sqrt(Math.PI) * L2 / (1d - 1d / (8.0d * alpha) + 1d / (128.0d * Math.Pow(alpha, 2d)));
            }

            return new[] { mu, sigma, gamma };
        }

        /// <summary>
        /// Returns an array of linear moments given the distribution parameters.
        /// </summary>
        /// <param name="parameters">The list of distribution parameters.</param>
        public double[] LinearMomentsFromParameters(IList<double> parameters)
        {
            double mu = parameters[0];
            double sigma = parameters[1];
            double gamma = parameters[2];
            double xi = mu - 2.0d * sigma / gamma;
            double alpha = 4.0d / Math.Pow(gamma, 2d);
            double beta = 0.5d * sigma * gamma;
            double L1 = xi + alpha * beta;
            double L2 = Math.Abs(Math.Pow(Math.PI, -0.5d) * beta * Mathematics.SpecialFunctions.Gamma.Function(alpha + 0.5d) / Mathematics.SpecialFunctions.Gamma.Function(alpha));
            // The following approximations are accurate to 10-6. 
            double A0 = 0.32573501d;
            double A1 = 0.1686915d;
            double A2 = 0.078327243d;
            double A3 = -0.0029120539d;
            double B1 = 0.46697102d;
            double B2 = 0.24255406d;
            double C0 = 0.12260172d;
            double C1 = 0.05373013d;
            double C2 = 0.043384378d;
            double C3 = 0.011101277d;
            double D1 = 0.18324466d;
            double D2 = 0.20166036d;
            double E1 = 2.3807576d;
            double E2 = 1.5931792d;
            double E3 = 0.11618371d;
            double F1 = 5.1533299d;
            double F2 = 7.142526d;
            double F3 = 1.9745056d;
            double G1 = 2.1235833d;
            double G2 = 4.1670213d;
            double G3 = 3.1925299d;
            double H1 = 9.0551443d;
            double H2 = 26.649995d;
            double H3 = 26.193668d;
            double T3;
            double T4;
            if (alpha >= 1d)
            {
                T3 = Math.Pow(alpha, -0.5d) * (A0 + A1 * Math.Pow(alpha, -1) + A2 * Math.Pow(alpha, -2) + A3 * Math.Pow(alpha, -3)) / (1d + B1 * Math.Pow(alpha, -1) + B2 * Math.Pow(alpha, -2));
                T4 = (C0 + C1 * Math.Pow(alpha, -1) + C2 * Math.Pow(alpha, -2) + C3 * Math.Pow(alpha, -3)) / (1d + D1 * Math.Pow(alpha, -1) + D2 * Math.Pow(alpha, -2));
            }
            else
            {
                T3 = (1d + E1 * alpha + E2 * Math.Pow(alpha, 2d) + E3 * Math.Pow(alpha, 3d)) / (1d + F1 * alpha + F2 * Math.Pow(alpha, 2d) + F3 * Math.Pow(alpha, 3d));
                T4 = (1d + G1 * alpha + G2 * Math.Pow(alpha, 2d) + G3 * Math.Pow(alpha, 3d)) / (1d + H1 * alpha + H2 * Math.Pow(alpha, 2d) + H3 * Math.Pow(alpha, 3d));
            }

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
            // Estimate initial values using the method of linear moments.
            var mom = IndirectMethodOfMoments(sample);
            initialVals = new double[] { mom[0], mom[1], mom[2] };
            // Get bounds of mean
            double real = Math.Exp(initialVals[0] / K);
            lowerVals[0] = Tools.DoubleMachineEpsilon;
            upperVals[0] = Math.Ceiling(Math.Log(Math.Pow(10d, Math.Ceiling(Math.Log10(real) + 1d)), Base));
            upperVals[0] = double.IsNaN(upperVals[0]) ? 5 : upperVals[0];
            // Get bounds of standard deviation
            real = Math.Exp(initialVals[1] / K);
            lowerVals[1] = Tools.DoubleMachineEpsilon;
            upperVals[1] = Math.Ceiling(Math.Log(Math.Pow(10d, Math.Ceiling(Math.Log10(real) + 1d)), Base));
            upperVals[1] = double.IsNaN(upperVals[1]) ? 4 : upperVals[1];

            // Get bounds of skew
            lowerVals[2] = -2d;
            upperVals[2] = 2d;
            // Correct initial value of skew if necessary
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

            // Solve using Nelder-Mead (Downhill Simplex)
            double logLH(double[] x)
            {
                var LP3 = new LogPearsonTypeIII() { Base = Base };
                LP3.SetParameters(x);
                return LP3.LogLikelihood(sample);
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
                ValidateParameters(Mu, Sigma, Gamma, true);
            // 
            if (x < Minimum || x > Maximum) return 0.0d;

            if (Math.Abs(Gamma) <= NearZero)
            {
                double d = (Math.Log(x, Base) - Mu) / Sigma;
                return Math.Exp(-0.5d * d * d) / (Tools.Sqrt2PI * Sigma) * (K / x);
            }
            else if (Beta > 0d)
            {
                double shiftedX = Math.Log(x, Base) - Xi;
                return Math.Exp(-shiftedX / Math.Abs(Beta) + (Alpha - 1.0d) * Math.Log(shiftedX) - Alpha * Math.Log(Math.Abs(Beta)) - Mathematics.SpecialFunctions.Gamma.LogGamma(Alpha)) * (K / x);
            }
            else
            {
                double shiftedX = Xi - Math.Log(x, Base);
                return Math.Exp(-shiftedX / Math.Abs(Beta) + (Alpha - 1.0d) * Math.Log(shiftedX) - Alpha * Math.Log(Math.Abs(Beta)) - Mathematics.SpecialFunctions.Gamma.LogGamma(Alpha)) * (K / x);
            }
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
                ValidateParameters(Mu, Sigma, Gamma, true);
            if (x <= Minimum) return 0d;
            if (x >= Maximum) return 1d;
            if (Math.Abs(Gamma) <= NearZero)
            {
                return 0.5d * (1.0d + Erf.Function((Math.Log(x, Base) - Mu) / (Sigma * Math.Sqrt(2.0d))));
            }
            else if (Beta > 0d)
            {
                double shiftedX = Math.Log(x, Base) - Xi;
                return Mathematics.SpecialFunctions.Gamma.LowerIncomplete(Alpha, shiftedX / Math.Abs(Beta));
            }
            else
            {
                double shiftedX = Xi - Math.Log(x, Base);
                return 1.0d - Mathematics.SpecialFunctions.Gamma.LowerIncomplete(Alpha, shiftedX / Math.Abs(Beta));
            }
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
            if (probability == 0.0d) return Minimum;
            if (probability == 1.0d) return Maximum;
            // Validate parameters
            if (_parametersValid == false)
                ValidateParameters(Mu, Sigma, Gamma, true);
            // 
            if (Math.Abs(Gamma) <= NearZero)
            {
                return Math.Exp((Mu - Sigma * Math.Sqrt(2.0d) * Erf.InverseErfc(2.0d * probability)) / K);
            }
            else if (Beta > 0d)
            {
                return Math.Exp((Xi + Mathematics.SpecialFunctions.Gamma.InverseLowerIncomplete(Alpha, probability) * Math.Abs(Beta)) / K);
            }
            else
            {
                return Math.Exp((Xi - Mathematics.SpecialFunctions.Gamma.InverseLowerIncomplete(Alpha, 1d - probability) * Math.Abs(Beta)) / K);
            }
        }

        /// <summary>
        /// Returns the inverse CDF using the modified Wilson-Hilferty transformation.
        /// </summary>
        /// <param name="probability">Probability between 0 and 1.</param>
        /// <remarks>
        /// Cornish-Fisher transformation (Fisher and Cornish, 1960) for abs(skew) less than or equal to 2. If abs(skew) > 2 then use Modified Wilson-Hilferty transformation (Kirby,1972).
        /// </remarks>
        public double WilsonHilfertyInverseCDF(double probability)
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
                ValidateParameters(Mu, Sigma, Gamma, true);
            // 
            return Math.Exp((Mu + Sigma * GammaDistribution.FrequencyFactorKp(Gamma, probability)) / K);
        }
      
        /// <summary>
        /// Returns a list containing the variance of each parameter given the sample size.
        /// </summary>
        /// <param name="sampleSize">The sample size.</param>
        /// <param name="estimationMethod">The distribution parameter estimation method.</param>
        public IList<double> ParameterVariance(int sampleSize, ParameterEstimationMethod estimationMethod)
        {
            if (estimationMethod == ParameterEstimationMethod.MethodOfMoments)
            {
                throw new NotImplementedException();
            }
            // Validate parameters
            if (_parametersValid == false)
                ValidateParameters(_mu, _sigma, _gamma, true);
            double alpha = 1d / Beta;
            double lambda = Alpha;
            double A = 2d * Mathematics.SpecialFunctions.Gamma.Trigamma(lambda) - 2d / (lambda - 1d) + 1d / Math.Pow(lambda - 1d, 2d);
            var varList = new List<double>();
            varList.Add((lambda - 2d) / (sampleSize * A) * (1d / Math.Pow(alpha, 2d)) * (Mathematics.SpecialFunctions.Gamma.Trigamma(lambda) * lambda - 1d)); // location
            varList.Add((lambda - 2d) * Math.Pow(alpha, 2d) / (sampleSize * A) * (Mathematics.SpecialFunctions.Gamma.Trigamma(lambda) / (lambda - 2d) - 1d / Math.Pow(lambda - 1d, 2d))); // scale
            varList.Add(2d / (sampleSize * A)); // shape
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
                ValidateParameters(_mu, _sigma, _gamma, true);
            double alpha = 1d / Beta;
            double lambda = Alpha;
            double A = 2d * Mathematics.SpecialFunctions.Gamma.Trigamma(lambda) - 2d / (lambda - 1d) + 1d / Math.Pow(lambda - 1d, 2d);
            var covarList = new List<double>();
            covarList.Add(1d / sampleSize * ((lambda - 2d) / A) * (Mathematics.SpecialFunctions.Gamma.Trigamma(lambda) - 1d / (lambda - 1d))); // location & scale
            covarList.Add((2d - lambda) / (sampleSize * alpha * A * (lambda - 1d))); // location & shape
            covarList.Add(alpha / (sampleSize * A * (lambda - 1d))); // scale & shape
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
                ValidateParameters(Mu, Sigma, Gamma, true);
            double alpha = 1d / Beta;
            double lambda = Alpha;
            double eps = Math.Sign(alpha);
            var partialList = new List<double>();
            partialList.Add(1.0d); // location
            partialList.Add(-lambda / Math.Pow(alpha, 2d) * (1.0d + eps / Math.Sqrt(lambda) * GammaDistribution.FrequencyFactorKp(Gamma, probability))); // scale
            partialList.Add(1.0d / alpha * (1.0d + eps / Math.Sqrt(lambda) * (GammaDistribution.FrequencyFactorKp(Gamma, probability) / 2.0d) - 1.0d / lambda * GammaDistribution.PartialKp(Gamma, probability))); // shape
            return partialList;
        }

        /// <summary>
        /// Returns a list of partial derivatives of X given probability with respect to each moment.
        /// </summary>
        /// <param name="probability">Probability between 0 and 1.</param>
        public IList<double> PartialDerivativesWithMoments(double probability)
        {
            // Validate parameters
            if (_parametersValid == false)
                ValidateParameters(Mu, Sigma, Gamma, true);
            var partialList = new List<double>();
            partialList.Add(Math.Exp((Mu + Sigma * GammaDistribution.FrequencyFactorKp(Gamma, probability)) / K) / K);
            partialList.Add(Math.Exp((Mu + Sigma * GammaDistribution.FrequencyFactorKp(Gamma, probability)) / K) * (GammaDistribution.FrequencyFactorKp(Gamma, probability) / K));
            partialList.Add(Math.Exp((Mu + Sigma * GammaDistribution.FrequencyFactorKp(Gamma, probability)) / K) * (Sigma / K) * GammaDistribution.PartialKp(Gamma, probability));
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
            if (estimationMethod == ParameterEstimationMethod.MethodOfMoments)
            {
                double u2 = _sigma;
                double c = _gamma;
                double c2 = Math.Pow(c, 2d);
                int N = sampleSize;
                double varT = Math.Pow(u2, 2d) / N * (1d + Math.Pow(GammaDistribution.FrequencyFactorKp(c, probability), 2d) / 2d * (1d + 0.75d * c2) + GammaDistribution.FrequencyFactorKp(c, probability) * c + 6d * (1d + 0.25d * c2) * GammaDistribution.PartialKp(c, probability) * (GammaDistribution.PartialKp(c, probability) * (1d + 5d * c2 / 4d) + GammaDistribution.FrequencyFactorKp(c, probability) / 2d * c));
                return varT * Math.Pow(InverseCDF(probability) / K, 2d);
            }
            else if (estimationMethod == ParameterEstimationMethod.MaximumLikelihood)
            {
                double varA = ParameterVariance(sampleSize, estimationMethod)[0];
                double varB = ParameterVariance(sampleSize, estimationMethod)[1];
                double varG = ParameterVariance(sampleSize, estimationMethod)[2];
                double covAB = ParameterCovariance(sampleSize, estimationMethod)[0];
                double covAG = ParameterCovariance(sampleSize, estimationMethod)[1];
                double covBG = ParameterCovariance(sampleSize, estimationMethod)[2];
                double pXA = PartialDerivatives(probability)[0];
                double pXB = PartialDerivatives(probability)[1];
                double pXG = PartialDerivatives(probability)[2];
                double varT = Math.Pow(pXA, 2d) * varA + Math.Pow(pXB, 2d) * varB + Math.Pow(pXG, 2d) * varG + 2d * pXA * pXB * covAB + 2d * pXA * pXG * covAG + 2d * pXB * pXG * covBG;
                return varT * Math.Pow(InverseCDF(probability) / K, 2d);
            }

            return default;
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
            var dXt1 = PartialDerivativesWithMoments(probabilities[0]).ToArray();
            var dXt2 = PartialDerivativesWithMoments(probabilities[1]).ToArray();
            var dXt3 = PartialDerivativesWithMoments(probabilities[2]).ToArray();
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
            return new LogPearsonTypeIII(Mu, Sigma, Gamma);
        }
   
    }
}