// Since I use functions from the Accord Math Library, here is the required license header:
// Haden Smith (January 2019)
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
using Numerics.Mathematics.SpecialFunctions;

namespace Numerics.Distributions
{

    /// <summary>
    /// The Student's t probability distribution.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    /// <see href = "https://en.wikipedia.org/wiki/Student%27s_t-distribution" />
    /// </para>
    /// </remarks>
    [Serializable]
    public class StudentT : UnivariateDistributionBase
    {
       
        /// <summary>
        /// Constructs a standard Student's t distribution with 10 degrees of freedom.
        /// </summary>
        public StudentT()
        {
            SetParameters(0d, 1d, 10d);
        }

        /// <summary>
        /// Constructs a standard Student's t distribution with a given degrees of freedom.
        /// </summary>
        /// <param name="degreesOfFreedom">The degrees of freedom.</param>
        public StudentT(double degreesOfFreedom)
        {
            SetParameters(0d, 1d, degreesOfFreedom);
        }

        /// <summary>
        /// Constructs a Student's t distribution with a given location, scale, and degrees of freedom.
        /// </summary>
        /// <param name="location">The location parameter.</param>
        /// <param name="scale">The scale parameter.</param>
        /// <param name="degreesOfFreedom">The degrees of freedom.</param>
        public StudentT(double location, double scale, double degreesOfFreedom)
        {
            SetParameters(location, scale, degreesOfFreedom);
        }

        
        private bool _parametersValid = true;
        private double _mu;
        private double _sigma;
        private int _degreesOfFreedom;

        /// <summary>
        /// Gets and sets the location parameter µ (Mu).
        /// </summary>
        public double Mu
        {
            get { return _mu; }
            set
            {
                _parametersValid = ValidateParameters(value, Sigma, DegreesOfFreedom, false) is null;
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
                _parametersValid = ValidateParameters(Mu, value, DegreesOfFreedom, false) is null;
                _sigma = value;
            }
        }

        /// <summary>
        /// Gets and sets the degrees of freedom ν (nu) of the distribution.
        /// </summary>
        public int DegreesOfFreedom
        {
            get { return _degreesOfFreedom; }
            set
            {
                _parametersValid = ValidateParameters(Mu, Sigma, value, false) is null;
                _degreesOfFreedom = value;
            }
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
            get { return UnivariateDistributionType.StudentT; }
        }

        /// <summary>
        /// Returns the name of the distribution type as a string.
        /// </summary>
        public override string DisplayName
        {
            get { return "Student's t"; }
        }

        /// <summary>
        /// Returns the short display name of the distribution as a string.
        /// </summary>
        public override string ShortDisplayName
        {
            get { return "T"; }
        }

        /// <summary>
        /// Get distribution parameters in 2-column array of string.
        /// </summary>
        public override string[,] ParametersToString
        {
            get
            {
                var parmString = new string[3, 2];
                parmString[0, 0] = "Location (µ)";
                parmString[1, 0] = "Scale (σ)";
                parmString[2, 0] = "Degrees of Freedom (ν)";
                parmString[0, 1] = Mu.ToString();
                parmString[1, 1] = Sigma.ToString();
                parmString[2, 1] = DegreesOfFreedom.ToString();
                return parmString;
            }
        }

        /// <summary>
        /// Gets the short form parameter names.
        /// </summary>
        public override string[] ParameterNamesShortForm
        {
            get { return new[] { "µ", "σ", "ν" }; }
        }

        /// <summary>
        /// Gets the full parameter names.
        /// </summary>
        public override string[] GetParameterPropertyNames
        {
            get { return new[] { nameof(Mu), nameof(Sigma), nameof(DegreesOfFreedom) }; }
        }

        /// <summary>
        /// Get an array of parameters.
        /// </summary>
        public override double[] GetParameters
        {
            get { return new[] { Mu, Sigma, DegreesOfFreedom }; }
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
                if (DegreesOfFreedom > 1)
                {
                    return Mu;
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
            get
            {
                if (DegreesOfFreedom > 2)
                {
                    return Math.Sqrt(Sigma * Sigma * DegreesOfFreedom / (DegreesOfFreedom - 2));
                }
                else if (DegreesOfFreedom > 1 && DegreesOfFreedom <= 2)
                {
                    return double.PositiveInfinity;
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
                if (DegreesOfFreedom > 3)
                {
                    return 0d;
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
                if (DegreesOfFreedom > 4)
                {
                    return 3d + 6d / (DegreesOfFreedom - 4);
                }
                else if (DegreesOfFreedom > 2 && DegreesOfFreedom <= 4)
                {
                    return double.PositiveInfinity;
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
            get { return new[] { double.NegativeInfinity, 0.0d, 1.0d }; }
        }

        /// <summary>
        /// Gets the maximum values allowable for each parameter.
        /// </summary>
        public override double[] MaximumOfParameters
        {
            get { return new[] { double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity }; }
        }

        /// <summary>
        /// Set the distribution parameters.
        /// </summary>
        /// <param name="mu">The location µ (Mu).</param>
        /// <param name="sigma">The scale σ (sigma). Range: σ > 0.</param>
        /// <param name="v">The degrees of freedom ν (nu). Range: ν > 0.</param>
        public void SetParameters(double mu, double sigma, double v)
        {
            Mu = mu;
            Sigma = sigma;
            DegreesOfFreedom = (int)v;
        }

        /// <summary>
        /// Set the distribution parameters.
        /// </summary>
        /// <param name="parameters">Array of parameters.</param>
        public override void SetParameters(IList<double> parameters)
        {
            SetParameters(parameters[0], parameters[1], parameters[2]);
        }

        /// <summary>
        /// Validate the parameters.
        /// </summary>
        /// <param name="location">The location parameter µ (Mu).</param>
        /// <param name="scale">The scale parameter σ (sigma).</param>
        /// <param name="v">The degrees of freedom ν (nu).</param>
        /// <param name="throwException">Determines whether to throw an exception or not.</param>
        public ArgumentOutOfRangeException ValidateParameters(double location, double scale, double v, bool throwException)
        {
            if (double.IsNaN(location) || double.IsInfinity(location))
            {
                if (throwException)
                    throw new ArgumentOutOfRangeException(nameof(Mu), "Mu must be a number.");
                return new ArgumentOutOfRangeException(nameof(Mu), "Mu must be a number.");
            }
            if (double.IsNaN(scale) || double.IsInfinity(scale) || scale <= 0.0d)
            {
                if (throwException)
                    throw new ArgumentOutOfRangeException(nameof(Sigma), "Standard deviation must be positive.");
                return new ArgumentOutOfRangeException(nameof(Sigma), "Standard deviation must be positive.");
            }
            if (v < 1.0d)
            {
                if (throwException)
                    throw new ArgumentOutOfRangeException(nameof(DegreesOfFreedom), "The degrees of freedom ν (nu) must greater than or equal to one.");
                return new ArgumentOutOfRangeException(nameof(DegreesOfFreedom), "The degrees of freedom ν (nu) must greater than or equal to one.");
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
        /// The Probability Density Function (PDF) of the distribution evaluated at a point X.
        /// </summary>
        /// <param name="x">A single point in the distribution range.</param>
        public override double PDF(double x)
        {
            // Validate parameters
            if (_parametersValid == false)
                ValidateParameters(Mu, Sigma, DegreesOfFreedom, true);
            double Z = (x - Mu) / Sigma;
            if (DegreesOfFreedom >= 100000000.0d)
            {
                return Normal.StandardPDF(Z);
            }
            return Math.Exp(Gamma.LogGamma((DegreesOfFreedom + 1.0d) / 2.0d) - Gamma.LogGamma(DegreesOfFreedom / 2.0d)) * Math.Pow(1.0d + Z * Z / DegreesOfFreedom, -0.5d * (DegreesOfFreedom + 1.0d)) / Math.Sqrt(DegreesOfFreedom * Math.PI);
        }

        /// <summary>
        /// The Cumulative Distribution Function (CDF) for the distribution evaluated at a point X.
        /// </summary>
        /// <param name="x">A single point in the distribution range.</param>
        /// <returns>The non-exceedance probability given a point X.</returns>
        /// <remarks>
        /// The CDF describes the cumulative probability that a given value or any value smaller than it will occur.
        /// <para>
        /// References:
        /// This code was copied from the Accord Math Library.
        /// <list type="bullet">
        /// <item><description>
        /// Accord Math Library, http://accord-framework.net
        /// </description></item>
        /// </list>
        /// </para>
        /// </remarks>
        public override double CDF(double x)
        {
            // Validate parameters
            if (_parametersValid == false)
                ValidateParameters(Mu, Sigma, DegreesOfFreedom, true);
            // 
            double Z = (x - Mu) / Sigma;
            if (DegreesOfFreedom >= 100000000.0d)
            {
                return Normal.StandardCDF(Z);
            }
            double betaX = (Z + Math.Sqrt(Z * Z + DegreesOfFreedom)) / (2d * Math.Sqrt(Z * Z + DegreesOfFreedom));
            return Beta.Incomplete(DegreesOfFreedom / 2.0d, DegreesOfFreedom / 2.0d, betaX);
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
        /// <para>
        /// References:
        /// This code was copied from the Accord Math Library.
        /// <list type="bullet">
        /// <item><description>
        /// Accord Math Library, http://accord-framework.net
        /// </description></item>
        /// </list>
        /// </para>
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
                ValidateParameters(Mu, Sigma, DegreesOfFreedom, true);
            // 
            if (probability > 0.25d && probability < 0.75d)
            {
                if (probability == 0.5d)
                {
                    return Mu;
                }

                double z = Beta.IncompleteInverse(0.5d, 0.5d * DegreesOfFreedom, Math.Abs(1.0d - 2.0d * probability));
                double t = Math.Sqrt(DegreesOfFreedom * z / (1.0d - z));
                if (probability < 0.5d)
                {
                    t = -t;
                }

                return Mu + Sigma * t;
            }
            else
            {
                int rflg = -1;
                if (probability >= 0.5d)
                {
                    probability = 1.0d - probability;
                    rflg = 1;
                }

                double z = Beta.IncompleteInverse(0.5d * DegreesOfFreedom, 0.5d, 2.0d * probability);
                double t = Math.Sqrt(DegreesOfFreedom / z - DegreesOfFreedom);
                if (double.MaxValue * z < DegreesOfFreedom)
                {
                    return rflg * double.MaxValue;
                }

                return Mu + Sigma * (rflg * t);
            }
        }
       
        /// <summary>
        /// Creates a copy of the distribution.
        /// </summary>
        public override UnivariateDistributionBase Clone()
        {
            return new StudentT(Mu, Sigma, DegreesOfFreedom);
        }
              
    }
}