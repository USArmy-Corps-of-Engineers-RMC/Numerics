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

using Numerics.Data.Statistics;
using System;
using System.Collections.Generic;

namespace Numerics.Distributions.Copulas
{

    /// <summary>
    /// The Frank copula.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public class FrankCopula : ArchimedeanCopula
    {

        /// <summary>
        /// Constructs a Frank copula with a dependency θ = 2.
        /// </summary>
        public FrankCopula()
        {
            Theta = 2d;
        }

        /// <summary>
        /// Constructs a Frank copula with a specified θ.
        /// </summary>
        /// <param name="theta">The dependency parameter, θ.</param>
        public FrankCopula(double theta)
        {
            Theta = theta;
        }

        /// <summary>
        /// Constructs a Frank copula with a specified θ and marginal distributions.
        /// </summary>
        /// <param name="theta">The dependency parameter, θ.</param>
        ///<param name="marginalDistributionX">The X marginal distribution for the copula.</param>
        ///<param name="marginalDistributionY">The Y marginal distribution for the copula.</param>
        public FrankCopula(double theta, IUnivariateDistribution marginalDistributionX, IUnivariateDistribution marginalDistributionY)
        {
            Theta = theta;
            MarginalDistributionX = marginalDistributionX;
            MarginalDistributionY = marginalDistributionY;
        }

        /// <summary>
        /// Returns the Copula type.
        /// </summary>
        public override CopulaType Type
        {
            get { return CopulaType.Frank; }
        }

        /// <summary>
        /// Returns the display name of the Copula distribution type as a string.
        /// </summary>
        public override string DisplayName
        {
            get { return "Frank"; }
        }

        /// <summary>
        /// Returns the short display name of the Copula distribution as a string.
        /// </summary>
        public override string ShortDisplayName
        {
            get { return "F"; }
        }

        /// <summary>
        /// Returns the minimum value allowable for the dependency parameter.
        /// </summary>
        public override double ThetaMinimum
        {
            get { return double.NegativeInfinity; }
        }

        /// <summary>
        /// Returns the maximum values allowable for the dependency parameter.
        /// </summary>
        public override double ThetaMaximum
        {
            get { return double.PositiveInfinity; }
        }

        /// <summary>
        /// Test to see if distribution parameters are valid.
        /// </summary>
        /// <param name="parameter">Dependency parameter.</param>
        /// <param name="throwException">Boolean indicating whether to throw the exception or not.</param>
        /// <returns>Nothing if the parameters are valid and the exception if invalid parameters were found.</returns>
        public override ArgumentOutOfRangeException ValidateParameter(double parameter, bool throwException)
        {
            if (parameter < ThetaMinimum)
            {
                if (throwException) throw new ArgumentOutOfRangeException(nameof(Theta), "The dependency parameter θ (theta) must be greater than or equal to " + ThetaMinimum.ToString() + ".");
                return new ArgumentOutOfRangeException(nameof(Theta), "The dependency parameter θ (theta) must be greater than or equal to " + ThetaMinimum.ToString() + ".");
            }
            if (parameter > ThetaMaximum)
            {
                if (throwException) throw new ArgumentOutOfRangeException(nameof(Theta), "The dependency parameter θ (theta) must be less than or equal to " + ThetaMaximum.ToString() + ".");
                return new ArgumentOutOfRangeException(nameof(Theta), "The dependency parameter θ (theta) must be less than or equal to " + ThetaMaximum.ToString() + ".");
            }
            //if (Math.Abs(parameter) <= 100 * Tools.DoubleMachineEpsilon)
            //{
            //    if (throwException) throw new ArgumentOutOfRangeException(nameof(Theta), "The dependency parameter θ (theta) cannot be zero. This is independence.");
            //    return new ArgumentOutOfRangeException(nameof(Theta), "The dependency parameter θ (theta) cannot be zero. This is independence.");
            //}
            return null;
        }

        /// <summary>
        /// The generator function of the copula.
        /// </summary>
        /// <param name="t">The reduced variate.</param>
        public override double Generator(double t)
        {
            return -Math.Log((Math.Exp(-Theta * t) - 1d) / (Math.Exp(-Theta) - 1d));
        }

        /// <summary>
        /// The inverse of the generator function.
        /// </summary>
        /// <param name="t">The reduced variate.</param>
        public override double GeneratorInverse(double t)
        {
            return -Math.Log(Math.Exp(-Theta - t) - Math.Exp(-t) + 1d) / Theta;
        }

        /// <summary>
        /// The first derivative of the generator function.
        /// </summary>
        /// <param name="t">The reduced variate.</param>
        public override double GeneratorPrime(double t)
        {
            return Theta / (1d - Math.Exp(Theta * t));
        }

        /// <summary>
        /// The second derivative of the generator function.
        /// </summary>
        /// <param name="t">The reduced variate.</param>
        public override double GeneratorPrime2(double t)
        {
            return Theta * Theta * Math.Exp(Theta * t) / Math.Pow(1d - Math.Exp(Theta * t), 2d);
        }

        /// <summary>
        /// The inverse of the first derivative of the generator function.
        /// </summary>
        /// <param name="t">The reduced variate.</param>
        public override double GeneratorPrimeInverse(double t)
        {
            return Math.Log((Theta - t) / -t) / Theta;
        }

        /// <summary>
        /// The probability density function (PDF) of the copula evaluated at reduced variates u and v.
        /// </summary>
        /// <param name="u">The reduced variate between 0 and 1.</param>
        /// <param name="v">The reduced variate between 0 and 1.</param>
        public override double PDF(double u, double v)
        {
            // Validate parameters
            if (_parametersValid == false) ValidateParameter(Theta, true);
            double num = Theta * (Math.Exp(Theta) - 1d) * Math.Exp(Theta * (1d + u + v));
            double den = Math.Pow(Math.Exp(Theta * (u + v)) - Math.Exp(Theta * (1d + u)) - Math.Exp(Theta * (1d + v)) + Math.Exp(Theta), 2d);
            return num / den;
        }

        /// <summary>
        /// The cumulative distribution function (CDF) of the copula evaluated at reduced variates u and v.
        /// </summary>
        /// <param name="u">The reduced variate between 0 and 1.</param>
        /// <param name="v">The reduced variate between 0 and 1.</param>
        public override double CDF(double u, double v)
        {
            // Validate parameters
            if (_parametersValid == false) ValidateParameter(Theta, true);
            return -(1d / Theta) * Math.Log(1d + (Math.Exp(-Theta * u) - 1d) * (Math.Exp(-Theta * v) - 1d) / (Math.Exp(-Theta) - 1d));
        }

        /// <summary>
        /// The inverse cumulative distribution function (InverseCDF) of the copula evaluated at probabilities u and v.
        /// </summary>
        /// <param name="u">Probability between 0 and 1.</param>
        /// <param name="v">Probability between 0 and 1.</param>
        public override double[] InverseCDF(double u, double v)
        {
            // Validate parameters
            if (_parametersValid == false) ValidateParameter(Theta, true);
            double a = -Math.Abs(Theta);
            v = -1d / a * Math.Log((-v * (Math.Exp(-a) - 1d) / (Math.Exp(-a * u) * (v - 1d) - v)) + 1d);
            v = Theta > 0d ? 1d - v : v;
            return new[] { u, v };
        }

        /// <summary>
        /// Create a deep copy of the copula.
        /// </summary>
        public override BivariateCopula Clone()
        {
            return new FrankCopula(Theta, MarginalDistributionX, MarginalDistributionY);
        }

        /// <summary>
        /// Returns the parameter constraints for the dependency parameter given the data samples. 
        /// </summary>
        /// <param name="sampleDataX">The sample data for the X variable.</param>
        /// <param name="sampleDataY">The sample data for the Y variable.</param>
        public override double[] ParameterConstraints(IList<double> sampleDataX, IList<double> sampleDataY)
        {
            var tau = Correlation.KendallsTau(sampleDataX, sampleDataY);
            double L = tau > 0 ? 0.001d : -100d;
            double U = tau > 0 ? 100d : -0.001d;
            return new[] { L, U };
        }

    }
}