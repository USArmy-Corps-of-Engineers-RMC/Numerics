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
// 

using System;
using System.Collections.Generic;
using Numerics.Data.Statistics;
using Numerics.Mathematics;
using Numerics.Mathematics.Optimization;
using Numerics.Mathematics.RootFinding;
using Numerics.Mathematics.SpecialFunctions;

namespace Numerics.Distributions
{

    /// <summary>
    /// Gamma distribution.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    /// The gamma distribution is a two-parameter family of continuous probability
    /// distributions. There are three different parameterizations in common use:</para>
    /// <list type="bullet">
    /// <item><description>
    /// With a shape parameter k and a
    /// scale parameter θ.</description></item>
    /// <item><description>
    /// With a shape parameter α = k and an inverse scale parameter
    /// β = 1/θ, called a rate parameter.</description></item>
    /// <item><description>
    /// With a shape parameter k and a mean
    /// parameter μ = k/β.</description></item>
    /// </list>
    /// <para>
    /// In each of these three forms, both parameters are positive real numbers. The
    /// parameterization with k and θ appears to be more common in econometrics and
    /// certain other applied fields, where e.g. the gamma distribution is frequently
    /// used to model waiting times. For instance, in life testing, the waiting time
    /// until death is a random variable that is frequently modeled with a gamma
    /// distribution. This is the default construction method for this class.
    /// </para>
    /// <para>
    /// This class only uses the parameterization described in the first bullet.
    /// </para>
    /// <para>
    /// If k is an integer, then the distribution represents an Erlang distribution; i.e.,
    /// the sum of k independent exponentially distributed random variables, each of which
    /// has a mean of θ (which is equivalent to a rate parameter of 1/θ).
    /// </para>
    /// <para>
    /// The gamma distribution is the maximum entropy probability distribution for a random
    /// variable X for which E[X] = kθ = α/β is fixed and greater than zero, and <c>E[ln(X)] =
    /// ψ(k) + ln(θ) = ψ(α) − ln(β)</c> is fixed (ψ is the digamma function).
    /// </para>
    /// <para>
    /// References:
    /// This code was developed using two primary sources: 1) Wikipedia; and 2) the Accord Math Library.
    /// <list type="bullet">
    /// <item><description>
    /// Wikipedia, The Free Encyclopedia. Gamma distribution. Available on:
    /// http://en.wikipedia.org/wiki/Gamma_distribution
    /// </description></item>
    /// <item><description>
    /// Accord Math Library, http://accord-framework.net
    /// </description></item>
    /// </list>
    /// </para>
    /// </remarks>
    [Serializable]
    public sealed class GammaDistribution : UnivariateDistributionBase, IColesTawn, IEstimation, IMaximumLikelihoodEstimation, IMomentEstimation, ILinearMomentEstimation, IStandardError, IBootstrappable
    {

        // There are three different parameterizations in common use:
        // 1. With a shape parameter κ and a scale parameter θ.
        // 2. With a shape parameter α = κ and an inverse scale parameter β = 1/θ, called a rate parameter.
        // 3. With a shape parameter κ and a mean parameter μ = kθ = α/β.
        // 
        // This class only uses the parameterization described in #1.

        /// <summary>
        /// Constructs a Gamma distribution with scale θ = 10 and shape κ = 2.
        /// </summary>
        public GammaDistribution()
        {
            SetParameters(10d, 2d);
        }

        /// <summary>
        /// Constructs a Gamma distribution with given parameters θ and shape κ.
        /// </summary>
        /// <param name="scale">The scale parameter θ (theta).</param>
        /// <param name="shape">The shape parameter κ (kappa).</param>
        public GammaDistribution(double scale, double shape)
        {
            SetParameters(scale, shape);
        }

        private bool _parametersValid = true;
        private double _theta; // scale (θ)
        private double _kappa; // shape (κ)

        /// <summary>
        /// Gets and sets the scale parameter θ (theta).
        /// </summary>
        public double Theta
        {
            get { return _theta; }
            set
            {
                _parametersValid = ValidateParameters(value, Kappa, false) is null;
                _theta = value;
            }
        }

        /// <summary>
        /// Gets the inverse scale parameter of the distribution, β = 1/θ.
        /// </summary>
        public double Rate
        {
            get { return 1.0d / _theta; }
        }

        /// <summary>
        /// Gets and sets the shape parameter κ (kappa).
        /// </summary>
        public double Kappa
        {
            get { return _kappa; }
            set
            {
                _parametersValid = ValidateParameters(Theta, value, false) is null;
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
            get { return UnivariateDistributionType.GammaDistribution; }
        }

        /// <summary>
        /// Returns the name of the distribution type as a string.
        /// </summary>
        public override string DisplayName
        {
            get { return "Gamma"; }
        }

        /// <summary>
        /// Returns the short display name of the distribution as a string.
        /// </summary>
        public override string ShortDisplayName
        {
            get { return "G2"; }
        }

        /// <summary>
        /// Get distribution parameters in 2-column array of string.
        /// </summary>
        public override string[,] ParametersToString
        {
            get
            {
                var parmString = new string[2, 2];
                parmString[0, 0] = "Scale (θ)";
                parmString[1, 0] = "Shape (κ)";
                parmString[0, 1] = Theta.ToString();
                parmString[1, 1] = Kappa.ToString();
                return parmString;
            }
        }

        /// <summary>
        /// Gets the short form parameter names.
        /// </summary>
        public override string[] ParameterNamesShortForm
        {
            get { return new[] { "θ", "κ" }; }
        }

        /// <summary>
        /// Gets the full parameter names.
        /// </summary>
        public override string[] GetParameterPropertyNames
        {
            get { return new[] { nameof(Theta), nameof(Kappa) }; }
        }

        /// <summary>
        /// Get an array of parameters.
        /// </summary>
        public override double[] GetParameters
        {
            get { return new[] { Theta, Kappa }; }
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
            get { return Kappa * Theta; }
        }

        /// <summary>
        /// Gets the median of the distribution.
        /// </summary>
        /// <remarks>
        public override double Median
        {
            // There is no closed form solution
            get { return InverseCDF(0.5d); }
        }

        /// <summary>
        /// Gets the mode of the distribution.
        /// </summary>
        public override double Mode
        {
            get
            {
                if (Kappa > 1d)
                {
                    return (Kappa - 1d) * Theta;
                }
                else
                {
                    return double.NaN;
                }
            }
        }

        /// <summary>
        /// Gets the standard deviation of the distribution.
        /// </summary>
        public override double StandardDeviation
        {
            get { return Math.Sqrt(Kappa * Math.Pow(Theta, 2d)); }
        }

        /// <summary>
        /// Gets the skew of the distribution.
        /// </summary>
        public override double Skew
        {
            get { return 2.0d / Math.Sqrt(Kappa); }
        }

        /// <summary>
        /// Gets the kurtosis of the distribution.
        /// </summary>
        public override double Kurtosis
        {
            get { return 3d + 6.0d / Kappa; }
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
            var newDistribution = new GammaDistribution(Theta, Kappa);
            var sample = newDistribution.GenerateRandomValues(seed, sampleSize);
            newDistribution.Estimate(sample, estimationMethod);
            if (newDistribution.ParametersValid == false)
                throw new Exception("Bootstrapped distribution parameters are invalid.");
            return newDistribution;
        }

        /// <summary>
        /// Set the distribution parameters.
        /// </summary>
        /// <param name="scale">The scale parameter θ (theta).</param>
        /// <param name="shape">The shape parameter k.</param>
        public void SetParameters(double scale, double shape)
        {
            // Validate parameters
            _parametersValid = ValidateParameters(scale, shape, false) is null;
            _theta = scale;
            _kappa = shape;
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
        /// <param name="scale">The scale parameter θ (theta).</param>
        /// <param name="shape">The shape parameter k.</param>
        /// <param name="throwException">Determines whether to throw an exception or not.</param>
        public ArgumentOutOfRangeException ValidateParameters(double scale, double shape, bool throwException)
        {
            if (double.IsNaN(scale) || double.IsInfinity(scale) || scale <= 0.0d)
            {
                if (throwException)
                    throw new ArgumentOutOfRangeException(nameof(Theta), "The scale parameter θ (theta) must be positive.");
                return new ArgumentOutOfRangeException(nameof(Theta), "The scale parameter θ (theta) must be positive.");
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
            var dist = new GammaDistribution();
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
            double A1 = -0.3080d;
            double A2 = -0.05812d;
            double A3 = 0.01765d;
            double B1 = 0.7213d;
            double B2 = -0.5947d;
            double B3 = -2.1817d;
            double B4 = 1.2113d;
            double L1 = moments[0];
            double L2 = moments[1];
            double CV = L2 / L1;
            double T, theta, kappa;
            if (CV < 0.5)
            {
                T = Math.PI * CV * CV;
                kappa = (1d + A1 * T) / (T * (1d + T * (A2 + T * A3)));
            }
            else
            {
                T = 1d - CV;
                kappa = T * (B1 + T * B2) / (1d + T * (B3 + T * B4));
            }
            theta = L1 / kappa;     
            return new[] { theta, kappa };
        }

        /// <summary>
        /// Returns an array of linear moments given the distribution parameters.
        /// </summary>
        /// <param name="parameters">The list of distribution parameters.</param>
        public double[] LinearMomentsFromParameters(IList<double> parameters)
        {
            double alpha = parameters[1];
            double beta = parameters[0];
            double L1 = alpha * beta;
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
            // Get initial values
            initialVals = ParametersFromMoments(Statistics.ProductMoments(sample));
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
                var GM = new GammaDistribution();
                GM.SetParameters(x);
                return GM.LogLikelihood(sample);
            }
            var solver = new NelderMead(logLH, NumberOfParameters, Initials, Lowers, Uppers);
            solver.ReportFailure = true;
            solver.Maximize();
            return solver.BestParameterSet.Values;
        }

        /// <summary>
        /// Estimates parameters using a Newton-Raphson method.
        /// </summary>
        /// <param name="sample">Array of sample data.</param>
        public void MLE_NR(IList<double> sample)
        {
            double lnsum = 0d;
            for (int i = 0; i < sample.Count; i++)
                lnsum += Math.Log(sample[i]);
            double mean = Statistics.Mean(sample);
            double s = Math.Log(mean) - lnsum / sample.Count;
            if (double.IsNaN(s))
            {
                throw new ArgumentException("Observation vector contains negative values.", "observations");
            }
            double K_hat = (3d - s + Math.Sqrt((s - 3d) * (s - 3d) + 24d * s)) / (12d * s);
            double newK = NewtonRaphson.Solve((k) => Math.Log(k) - Gamma.Digamma(k) - s, (k) => 1d / k - Gamma.Trigamma(k), K_hat, 0.000001d, 100, true);
            double newTheta = mean / Kappa;
            SetParameters(newTheta, newK);
        }

        /// <summary>
        /// Estimates parameters using an approximation proposed by Bobee.
        /// </summary>
        /// <param name="sample">Array of sample data.</param>
        public void MLE_Bobee(IList<double> sample)
        {
            double A = Statistics.Mean(sample);
            double G = Statistics.GeometricMean(sample);
            double U = Math.Log(A) - Math.Log(G);
            double La = (1d + Math.Sqrt(1d + 4d * U / 3d)) / (4d * U);
            double dL = 0.04475d * Math.Pow(0.26d, La);
            double K = La - dL;
            double T = A / K;
            SetParameters(T, K);
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
                ValidateParameters(Theta, Kappa, true);
            if (X < Minimum || X > Maximum) return 0.0d;
            return Math.Exp(-X / Theta + (Kappa - 1.0d) * Math.Log(X) - Kappa * Math.Log(Theta) - Gamma.LogGamma(Kappa));
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
                ValidateParameters(Theta, Kappa, true);
            if (X <= Minimum)
                return 0d;
            if (X >= Maximum)
                return 1d;
            return Gamma.LowerIncomplete(Kappa, X / Theta);
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
                ValidateParameters(Theta, Kappa, true);
            return Gamma.InverseLowerIncomplete(Kappa, probability) * Theta;
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
                ValidateParameters(Theta, Kappa, true);
            return Theta * (FrequencyFactorKp(Skew, probability) * Math.Sqrt(Kappa) + Kappa);
        }

        /// <summary>
        /// Gets the K frequency factor given the skewness coefficient through Cornish-Fisher transformation (Fisher and Cornish, 1960) for abs(skew) less than or equal to 2.
        /// If abs(skew) > 2 then use Modified Wilson-Hilferty transformation (Kirby,1972).
        /// </summary>
        /// <param name="skewness">Coefficient of skewness.</param>
        /// <param name="probability">Probability between 0 and 1.</param>
        public static double FrequencyFactorKp(double skewness, double probability)
        {
            double C = skewness;
            double absC = Math.Abs(C);
            // If skew is sufficiently close to zero, return standard Normal Z variate.
            if (absC < 0.0001d) return Normal.StandardZ(probability);

            // If abs(skew) is less than or equal to 2, use Cornish-Fisher transformation (Fisher and Cornish, 1960)
            if (absC <= 2d)
            {
                double C2 = Math.Pow(C, 2d);
                double C3 = Math.Pow(C, 3d);
                double C4 = Math.Pow(C, 4d);
                double C5 = Math.Pow(C, 5d);
                double C6 = Math.Pow(C, 6d);
                double U = Normal.StandardZ(probability);
                double U2 = Math.Pow(U, 2d);
                double U3 = Math.Pow(U, 3d);
                double U4 = Math.Pow(U, 4d);
                double U5 = Math.Pow(U, 5d);
                double U6 = Math.Pow(U, 6d);
                double U7 = Math.Pow(U, 7d);
                double Kterm0 = U;
                double Kterm1 = C / 2d * ((U2 - 1d) / 3d);
                double Kterm2 = C2 / Math.Pow(2d, 4d) * ((U3 - 7d * U) / 9d);
                double Kterm3 = C3 / Math.Pow(2d, 5d) * ((6d * U4 + 14d * U2 - 32d) / 405d);
                double Kterm4 = C4 / Math.Pow(2d, 7d) * ((9d * U5 + 256d * U3 - 433d * U) / 4860d);
                double Kterm5 = C5 / Math.Pow(2d, 8d) * ((12d * U6 - 143d * U4 - 923d * U2 + 1472d) / 25515d);
                double Kterm6 = C6 / Math.Pow(2d, 10d) * ((3753d * U7 + 4353d * U5 - 289517d * U3 - 289717d * U) / 9185400d);
                return Kterm0 + Kterm1 + Kterm2 - Kterm3 + Kterm4 + Kterm5 - Kterm6;
            }
            else
            {
                // If abs(skew) is greater than 2, use Modified Wilson-Hilferty transformation (Kirby, 1972)
                // Only, valid if abs(skew) <= 9.75. Enforce limits.
                if (C < -9.75d)
                    C = -9.75d;
                if (C > 9.75d)
                    C = 9.75d;

                // Hoshi and Burges (1981b) gave polynomial expressions for 1/A, B, G and H^3 as a function of Cs
                // Compute skew orders
                double C2 = Math.Pow(absC, 2d);
                double C3 = Math.Pow(absC, 3d);
                double C4 = Math.Pow(absC, 4d);
                double C5 = Math.Pow(absC, 5d);
                // Compute A
                double a0 = 0.00199447d;
                double a1 = 0.48489d;
                double a2 = 0.0230935d;
                double a3 = -0.0152435d;
                double a4 = 0.00160597d;
                double a5 = -0.000055869d;
                double A = 1d / (a0 + a1 * C + a2 * C2 + a3 * C3 + a4 * C4 + a5 * C5);
                // Compute B
                double b0 = 0.990562d;
                double b1 = 0.0319647d;
                double b2 = -0.0274231d;
                double b3 = 0.00777405d;
                double b4 = -0.000571184d;
                double b5 = 0.0000142077d;
                double B = b0 + b1 * C + b2 * C2 + b3 * C3 + b4 * C4 + b5 * C5;
                // Compute G
                double g0 = -0.00385205d;
                double g1 = 1.00426d;
                double g2 = 0.00651207d;
                double g3 = -0.0149166d;
                double g4 = 0.00163945d;
                double g5 = -0.0000583804d;
                double G = g0 + g1 * C + g2 * C2 + g3 * C3 + g4 * C4 + g5 * C5;
                // Compute H
                double H = Math.Pow(B - 2.0d / absC / A, 1d / 3d);
                return Math.Sign(C) * A * (Math.Pow(Math.Max(H, 1.0d - Math.Pow(G / 6.0d, 2d) + G / 6.0d * Normal.StandardZ(probability)), 3d) - B);
            }
        }

        /// <summary>
        /// Gets the partial derivative of the frequency factor Kp with respect to skew.
        /// </summary>
        /// <param name="skewness">Coefficient of skewness.</param>
        /// <param name="probability">Probability between 0 and 1.</param>
        public static double PartialKp(double skewness, double probability)
        {
            double C = skewness;
            double absC = Math.Abs(C);
            // If skew is sufficiently close to zero, return standard Normal Z variate.
            if (absC < 0.0001d) return Normal.StandardZ(probability);

            // If abs(skew) is less than or equal to 2, use Cornish-Fisher transformation (Fisher and Cornish, 1960)
            if (absC <= 2d)
            {
                double C2 = Math.Pow(C, 2d);
                double C3 = Math.Pow(C, 3d);
                double C4 = Math.Pow(C, 4d);
                double C5 = Math.Pow(C, 5d);
                double C6 = Math.Pow(C, 6d);
                double U = Normal.StandardZ(probability);
                double U2 = Math.Pow(U, 2d);
                double U3 = Math.Pow(U, 3d);
                double U4 = Math.Pow(U, 4d);
                double U5 = Math.Pow(U, 5d);
                double U6 = Math.Pow(U, 6d);
                double U7 = Math.Pow(U, 7d);
                // Determine the first derivative of K with respect to skew
                double dKterm0 = 0d;
                double dKterm1 = 1d / 2d * ((U2 - 1d) / 3d);
                double dKterm2 = 2d * (C / Math.Pow(2d, 4d)) * ((U3 - 7d * U) / 9d);
                double dKterm3 = 3d * (C2 / Math.Pow(2d, 5d)) * ((6d * U4 + 14d * U2 - 32d) / 405d);
                double dKterm4 = 4d * (C3 / Math.Pow(2d, 7d)) * ((9d * U5 + 256d * U3 - 433d * U) / 4860d);
                double dKterm5 = 5d * (C4 / Math.Pow(2d, 8d)) * ((12d * U6 - 143d * U4 - 923d * U2 + 1472d) / 25515d);
                double dKterm6 = 6d * (C5 / Math.Pow(2d, 10d)) * ((3753d * U7 + 4353d * U5 - 289517d * U3 - 289717d * U) / 9185400d);
                // 
                return dKterm0 + dKterm1 + dKterm2 - dKterm3 + dKterm4 + dKterm5 - dKterm6;
            }
            else
            {
                // If abs(skew) is greater than 2, use Modified Wilson-Hilferty transformation (Kirby, 1972)
                return NumericalDerivative.Derivative((x) => FrequencyFactorKp(x, probability), skewness, 0.0001d);
            }
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
                ValidateParameters(_theta, _kappa, true);
            double alpha = 1d / Theta;
            double lambda = Kappa;
            double NA = Gamma.Trigamma(lambda) - 1d / lambda;
            var varList = new List<double>();
            varList.Add(Math.Pow(alpha, 2d) * Gamma.Trigamma(lambda) / (sampleSize * lambda * NA)); // scale
            varList.Add(1d / (sampleSize * NA)); // shape
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
                ValidateParameters(_theta, _kappa, true);
            double alpha = 1d / Theta;
            double lambda = Kappa;
            double NA = Gamma.Trigamma(lambda) - 1d / lambda;
            var covarList = new List<double>();
            covarList.Add(alpha / (sampleSize * lambda * NA)); // scale & shape
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
                ValidateParameters(_theta, _kappa, true);
            double alpha = 1d / Theta;
            double lambda = Kappa;
            double eps = Math.Sign(alpha);
            var partialList = new List<double>();
            partialList.Add(-lambda / Math.Pow(alpha, 2d) * (1.0d + eps / Math.Sqrt(lambda) * FrequencyFactorKp(Skew, probability))); // scale
            partialList.Add(1.0d / alpha * (1.0d + eps / Math.Sqrt(lambda) * FrequencyFactorKp(Skew, probability) / 2.0d - 1.0d / lambda * PartialKp(Skew, probability))); // shape
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
                double CV = CoefficientOfVariation;
                double V = Variance;
                int N = sampleSize;
                return V / N * (Math.Pow(1d + FrequencyFactorKp(Skew, probability) * CV, 2d) + 0.5d * Math.Pow(FrequencyFactorKp(Skew, probability) + 2d * CV * PartialKp(Skew, probability), 2d) * (1d + Math.Pow(CV, 2d)));
            }
            else if (estimationMethod == ParameterEstimationMethod.MaximumLikelihood)
            {
                double varA = ParameterVariance(sampleSize, estimationMethod)[0];
                double varB = ParameterVariance(sampleSize, estimationMethod)[1];
                double covAB = ParameterCovariance(sampleSize, estimationMethod)[0];
                double pXA = PartialDerivatives(probability)[0];
                double pXB = PartialDerivatives(probability)[1];
                return Math.Pow(pXA, 2d) * varA + Math.Pow(pXB, 2d) * varB + 2d * pXA * pXB * covAB;
            }
            return default;
        }
    
        /// <summary>
        /// Partial derivative with respect to theta.
        /// </summary>
        /// <param name="probability">The probability to evaluate.</param>
        public double PartialforTheta(double probability)
        {
            return FrequencyFactorKp(Skew, probability) * Math.Sqrt(Kappa) + Kappa;
        }

        /// <summary>
        /// Partial derivative with respect to kappa.
        /// </summary>
        /// <param name="probability">The probability to evaluate.</param>
        private double PartialforKappa(double probability)
        {
            return Theta * (FrequencyFactorKp(Skew, probability) / (2.0d * Math.Sqrt(Kappa)) + 1.0d - PartialKp(Skew, probability) / Kappa);
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
            double a = PartialforTheta(probabilities[0]);
            double b = PartialforKappa(probabilities[0]);
            double c = PartialforTheta(probabilities[1]);
            double d = PartialforKappa(probabilities[1]);
            return a * d - b * c;
        }
     
        /// <summary>
        /// Creates a copy of the distribution.
        /// </summary>
        public override UnivariateDistributionBase Clone()
        {
            return new GammaDistribution(Theta, Kappa);
        }
  
    }
}