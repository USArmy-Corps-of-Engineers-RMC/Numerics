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
using Numerics.Mathematics.RootFinding;
using Numerics.Mathematics.SpecialFunctions;

namespace Numerics.Distributions
{

    /// <summary>
    /// The Poisson distribution.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    /// <see href = "https://en.wikipedia.org/wiki/Poisson_distribution" />
    /// </para>
    /// </remarks>
    [Serializable]
    public class Poisson : UnivariateDistributionBase
    {
 
        /// <summary>
        /// Constructs a Poisson distribution with λ=4.
        /// </summary>
        public Poisson()
        {
            SetParameters(new[] { 1.0d });
        }

        /// <summary>
        /// Constructs a Poisson distribution with a given rate λ (lambda).
        /// </summary>
        /// <param name="rate">The rate parameter λ (lambda). Range: λ > 0.</param>
        public Poisson(double rate)
        {
            SetParameters(new[] { rate });
        }

        private bool _parametersValid = true;
        private double _lambda;

        /// <summary>
        /// Gets and sets the Poisson's rate parameter λ (lambda). Range: λ > 0.
        /// </summary>
        public double Lambda
        {
            get { return _lambda; }

            set
            {
                _parametersValid = ValidateParameters(new[] { value }, false) is null;
                _lambda = value;
            }
        }

        /// <summary>
        /// Returns the number of distribution parameters.
        /// </summary>
        public override int NumberOfParameters
        {
            get { return 1; }
        }

        /// <summary>
        /// Returns the univariate distribution type.
        /// </summary>
        public override UnivariateDistributionType Type
        {
            get { return UnivariateDistributionType.Poisson; }
        }

        /// <summary>
        /// Returns the name of the distribution type as a string.
        /// </summary>
        public override string DisplayName
        {
            get { return "Poisson"; }
        }

        /// <summary>
        /// Returns the short display name of the distribution as a string.
        /// </summary>
        public override string ShortDisplayName
        {
            get { return "P"; }
        }

        /// <summary>
        /// Get distribution parameters in 2-column array of string.
        /// </summary>
        public override string[,] ParametersToString
        {
            get
            {
                var parmString = new string[1, 2];
                parmString[0, 0] = "Rate (λ)";
                parmString[0, 1] = Lambda.ToString();
                return parmString;
            }
        }

        /// <summary>
        /// Gets the short form parameter names.
        /// </summary>
        public override string[] ParameterNamesShortForm
        {
            get { return new[] { "λ" }; }
        }

        /// <summary>
        /// Gets the full parameter names.
        /// </summary>
        public override string[] GetParameterPropertyNames
        {
            get { return new[] { nameof(Lambda) }; }
        }

        /// <summary>
        /// Get an array of parameters.
        /// </summary>
        public override double[] GetParameters
        {
            get { return new[] { Lambda }; }
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
            get { return Lambda; }
        }

        /// <summary>
        /// Gets the median of the distribution.
        /// </summary>
        /// <remarks>Approximation, see Wikipedia <a href="http://en.wikipedia.org/wiki/Poisson_distribution">Poisson distribution</a></remarks>
        public override double Median
        {
            get { return Math.Floor(Lambda + 1.0d / 3.0d - 0.02d / Lambda); }
        }

        /// <summary>
        /// Gets the mode of the distribution.
        /// </summary>
        public override double Mode
        {
            get { return Math.Floor(Lambda); }
        }

        /// <summary>
        /// Gets the standard deviation of the distribution.
        /// </summary>
        public override double StandardDeviation
        {
            get { return Math.Sqrt(Lambda); }
        }

        /// <summary>
        /// Gets the skew of the distribution.
        /// </summary>
        public override double Skew
        {
            get { return 1.0d / Math.Sqrt(Lambda); }
        }

        /// <summary>
        /// Gets the kurtosis of the distribution.
        /// </summary>
        public override double Kurtosis
        {
            get { return 3.0d + 1.0d / Lambda; }
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
            get { return new[] { 0.0d }; }
        }

        /// <summary>
        /// Gets the maximum values allowable for each parameter.
        /// </summary>
        public override double[] MaximumOfParameters
        {
            get { return new[] { double.PositiveInfinity }; }
        }

        /// <summary>
        /// Set the distribution parameters.
        /// </summary>
        /// <param name="parameters">A list of parameters.</param>
        public override void SetParameters(IList<double> parameters)
        {
            Lambda = parameters[0];
        }

        /// <summary>
        /// Validate the parameters.
        /// </summary>
        /// <param name="parameters">A list of parameters.</param>
        /// <param name="throwException">Determines whether to throw an exception or not.</param>
        public override ArgumentOutOfRangeException ValidateParameters(IList<double> parameters, bool throwException)
        {
            if (double.IsNaN(parameters[0]) || double.IsInfinity(parameters[0]) || parameters[0] <= 0.0d)
            {
                if (throwException)
                    throw new ArgumentOutOfRangeException(nameof(Lambda), "The rate (λ) must be positive.");
                return new ArgumentOutOfRangeException(nameof(Lambda), "The rate (λ) must be positive.");
            }
            return null;
        }

        /// <summary>
        /// Gets the Probability Density Function (PDF) of the distribution evaluated at a point k.
        /// </summary>
        /// <param name="k">A single point in the distribution range.</param>
        /// <remarks>Returns the Probability Mass Function (PMF) for discrete distributions.</remarks>
        public override double PDF(double k)
        {
            // Validate parameters
            if (_parametersValid == false)
                ValidateParameters(new[] { Lambda }, true);
            k = Math.Floor(k);
            if (k < Minimum || k > Maximum) return 0.0d;
            return Math.Exp(-Lambda + k * Math.Log(Lambda) - Factorial.LogFactorial((int)k));
        }

        /// <summary>
        /// Gets the Cumulative Distribution Function (CDF) for the distribution evaluated at a point k.
        /// </summary>
        /// <param name="k">A single point in the distribution range.</param>
        public override double CDF(double k)
        {
            // Validate parameters
            if (_parametersValid == false)
                ValidateParameters(new[] { Lambda }, true);
            k = Math.Floor(k);
            if (k < Minimum)
                return 0.0d;
            if (k > Maximum)
                return 1.0d;
            return Gamma.UpperIncomplete(k + 1d, Lambda);
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
                ValidateParameters(new[] { Lambda }, true);
            // First bound the problem
            int lower = (int)Minimum;
            int upper = lower + 1;
            double f = CDF(upper);
            while (f < probability)
            {
                upper *= 2;
                f = CDF(upper);
            }
            // Then solve with Brent
            return Brent.Solve(x => CDF(x) - probability, lower, upper);
        }
      
        /// <summary>
        /// Creates a copy of the distribution.
        /// </summary>
        public override UnivariateDistributionBase Clone()
        {
            return new Poisson(Lambda);
        }
    
    }
}