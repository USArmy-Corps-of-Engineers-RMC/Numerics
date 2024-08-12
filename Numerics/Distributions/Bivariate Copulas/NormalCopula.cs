/*
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
*/

using System;
using System.Collections.Generic;

namespace Numerics.Distributions.Copulas
{

    /// <summary>
    /// The Gaussian (Normal) copula.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public class NormalCopula : BivariateCopula
    {

        /// <summary>
        /// Constructs a bivariate Gaussian copula with a correlation rho ρ = 0.0.
        /// </summary>
        public NormalCopula()
        {
            Theta = 0.0d;
        }

        /// <summary>
        /// Constructs a bivariate Gaussian copula with a specified correlation rho ρ.
        /// </summary>
        public NormalCopula(double rho)
        {
            Theta = rho;
        }

        /// <summary>
        /// Constructs a bivariate Gaussian copula with a specified θ and marginal distributions.
        /// </summary>
        /// <param name="rho">The dependency parameter, θ.</param>
        ///<param name="marginalDistributionX">The X marginal distribution for the copula.</param>
        ///<param name="marginalDistributionY">The Y marginal distribution for the copula.</param>
        public NormalCopula(double rho, IUnivariateDistribution marginalDistributionX, IUnivariateDistribution marginalDistributionY)
        {
            Theta = rho;
            MarginalDistributionX = marginalDistributionX;
            MarginalDistributionY = marginalDistributionY;
        }

        /// <summary>
        /// Returns the Copula type.
        /// </summary>
        public override CopulaType Type
        {
            get { return CopulaType.Normal; }
        }

        /// <summary>
        /// Returns the display name of the Copula distribution type as a string.
        /// </summary>
        public override string DisplayName
        {
            get { return "Normal"; }
        }

        /// <summary>
        /// Returns the short display name of the Copula distribution as a string.
        /// </summary>
        public override string ShortDisplayName
        {
            get { return "N"; }
        }

        /// <summary>
        /// Returns the name and value of the theta parameter in 2-column array of string.
        /// </summary>
        public override string[,] ParameterToString
        {
            get
            {
                var parmString = new string[2, 2];
                parmString[0, 0] = "Correlation (ρ)";
                parmString[0, 1] = Theta.ToString();
                return parmString;
            }
        }

        /// <summary>
        /// Returns the distribution parameter name in short form (e.g. ρ).
        /// </summary>
        public override string ParameterNameShortForm
        {
            get { return "ρ"; }
        }

        /// <summary>
        /// Returns the minimum value allowable for the dependency parameter.
        /// </summary>
        public override double ThetaMinimum
        {
            get { return -1.0d; }
        }

        /// <summary>
        /// Returns the maximum values allowable for the dependency parameter.
        /// </summary>
        public override double ThetaMaximum
        {
            get { return 1.0d; }
        }

        /// <summary>
        /// Test to see if distribution parameters are valid.
        /// </summary>
        /// <param name="parameter">Correlation parameter.</param>
        /// <param name="throwException">Boolean indicating whether to throw the exception or not.</param>
        /// <returns>Nothing if the parameters are valid and the exception if invalid parameters were found.</returns>
        public override ArgumentOutOfRangeException ValidateParameter(double parameter, bool throwException)
        {
            if (parameter < ThetaMinimum)
            {
                if (throwException) throw new ArgumentOutOfRangeException(nameof(Theta), "The correlation parameter ρ (rho) must be greater than " + ThetaMinimum.ToString() + ".");
                return new ArgumentOutOfRangeException(nameof(Theta), "The correlation parameter ρ (rho) must be greater than " + ThetaMinimum.ToString() + ".");
            }
            if (parameter > ThetaMaximum)
            {
                if (throwException) throw new ArgumentOutOfRangeException(nameof(Theta), "The correlation parameter ρ (rho) must be less than " + ThetaMaximum.ToString() + ".");
                return new ArgumentOutOfRangeException(nameof(Theta), "The correlation parameter ρ (rho) must be less than " + ThetaMaximum.ToString() + ".");
            }
            return null;
        }

        /// <summary>
        /// Returns the parameter constraints for the dependency parameter given the data samples. 
        /// </summary>
        /// <param name="sampleDataX">The sample data for the X variable.</param>
        /// <param name="sampleDataY">The sample data for the Y variable.</param>
        public override double[] ParameterConstraints(IList<double> sampleDataX, IList<double> sampleDataY)
        {
            return new double[] { -1 + Tools.DoubleMachineEpsilon, 1 - Tools.DoubleMachineEpsilon };
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
            var r = _theta;
            var s = Normal.StandardZ(u);
            var t = Normal.StandardZ(v);
            return 1d / Math.Sqrt(1d - r * r) * Math.Exp(-(r * r * s * s + r * r * t * t - 2 * r * s * t) / (2 * (1d - r * r)));
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
            return MultivariateNormal.BivariateCDF(Normal.StandardZ(1 - u), Normal.StandardZ(1 - v), _theta);
        }

        /// <summary>
        /// The inverse cumulative distribution function (InvCDF) of the copula evaluated at probabilities u and v.
        /// </summary>
        /// <param name="u">Probability between 0 and 1.</param>
        /// <param name="v">Probability between 0 and 1.</param>
        public override double[] InverseCDF(double u, double v)
        {
            // Validate parameters
            if (_parametersValid == false) ValidateParameter(Theta, true);
            double z1 = Normal.StandardZ(u);
            double z2 = Normal.StandardZ(v);
            double r = _theta;
            double w2 = r * z1 + Math.Sqrt(1d - r * r) * z2;
            v = Normal.StandardCDF(w2);
            return new[] { u, v };
        }

        /// <summary>
        /// Create a deep copy of the copula.
        /// </summary>
        public override BivariateCopula Clone()
        {
            return new NormalCopula(Theta, MarginalDistributionX, MarginalDistributionY);
        }

    }
}