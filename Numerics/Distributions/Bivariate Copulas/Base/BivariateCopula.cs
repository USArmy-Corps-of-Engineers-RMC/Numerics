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

using Numerics.Sampling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numerics.Distributions.Copulas
{
    /// <summary>
    /// Declares common functionality of all Bivariate Copulas.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public abstract class BivariateCopula : IBivariateCopula
    {

        #region Members

        protected double _theta;
        protected bool _parametersValid = true;

        /// <summary>
        /// Returns the Copula type.
        /// </summary>
        public abstract CopulaType Type { get; }

        /// <summary>
        /// The dependency parameter, theta θ
        /// </summary>
        public double Theta
        {
            get { return _theta; }
            set
            {
                _parametersValid = ValidateParameter(value, false) is null;
                _theta = value;
            }
        }

        /// <summary>
        /// Returns the minimum value allowable for the dependency parameter.
        /// </summary>
        public abstract double ThetaMinimum { get; }

        /// <summary>
        /// Returns the maximum values allowable for the dependency parameter.
        /// </summary>
        public abstract double ThetaMaximum { get; }

        /// <summary>
        /// Returns the name and value of the theta parameter in 2-column array of string.
        /// </summary>
        public abstract string[,] ParameterToString { get; }


        /// <summary>
        /// Returns the distribution parameter name in short form (e.g. θ).
        /// </summary>
        public abstract string ParameterNameShortForm { get; }


        /// <summary>
        /// Returns the distribution parameter property name.
        /// </summary>
        public string GetParameterPropertyName
        {
            get { return nameof(Theta); }
        }

        /// <summary>
        /// Returns a boolean value describing if the current parameters are valid or not.
        /// If not, an ArgumentOutOfRange exception will be thrown when trying to use statistical functions (e.g. CDF())
        /// </summary>
        public bool ParametersValid
        {
            get { return _parametersValid; }
        }

        /// <summary>
        /// The X marginal distribution for the copula. 
        /// </summary>
        public virtual IUnivariateDistribution MarginalDistributionX { get; set; }

        /// <summary>
        /// The Y marginal distribution for the copula. 
        /// </summary>
        public virtual IUnivariateDistribution MarginalDistributionY { get; set; }

        /// <summary>
        /// Returns the display name of the distribution type as a string.
        /// </summary>
        public abstract string DisplayName { get; }

        /// <summary>
        /// Returns the short display name of the distribution as a string.
        /// </summary>
        public abstract string ShortDisplayName { get; }

        #endregion

        #region Methods

        /// <summary>
        /// The probability density function (PDF) of the copula evaluated at reduced variates u and v.
        /// </summary>
        /// <param name="u">The reduced variate between 0 and 1.</param>
        /// <param name="v">The reduced variate between 0 and 1.</param>
        public abstract double PDF(double u, double v);

        /// <summary>
        /// Returns the natural log of the PDF.
        /// </summary>
        /// <param name="u">The reduced variate between 0 and 1.</param>
        /// <param name="v">The reduced variate between 0 and 1.</param>
        public double LogPDF(double u, double v)
        {
            double f = PDF(u, v);
            // If the PDF returns an invalid probability, then return the worst log-probability.
            if (double.IsNaN(f) || double.IsInfinity(f) || f <= 0d) return double.MinValue;
            return Math.Log(f);
        }

        /// <summary>
        /// The cumulative distribution function (CDF) of the copula evaluated at reduced variates u and v.
        /// </summary>
        /// <param name="u">The reduced variate between 0 and 1.</param>
        /// <param name="v">The reduced variate between 0 and 1.</param>
        public abstract double CDF(double u, double v);

        /// <summary>
        /// The inverse cumulative distribution function (InvCDF) of the copula evaluated at probabilities u and v.
        /// </summary>
        /// <param name="u">Probability between 0 and 1.</param>
        /// <param name="v">Probability between 0 and 1.</param>
        public abstract double[] InverseCDF(double u, double v);

        /// <summary>
        /// Returns the parameter constraints for the dependency parameter given the data samples. 
        /// </summary>
        /// <param name="sampleDataX">The sample data for the X variable.</param>
        /// <param name="sampleDataY">The sample data for the Y variable.</param>
        public abstract double[] ParameterConstraints(IList<double> sampleDataX, IList<double> sampleDataY);

        /// <summary>
        /// Test to see if distribution parameters are valid.
        /// </summary>
        /// <param name="parameter">Dependency parameter.</param>
        /// <param name="throwException">Boolean indicating whether to throw the exception or not.</param>
        /// <returns>Nothing if the parameters are valid and the exception if invalid parameters were found.</returns>
        public abstract ArgumentOutOfRangeException ValidateParameter(double parameter, bool throwException);

        /// <summary>
        /// Create a deep copy of the copula.
        /// </summary>
        public abstract BivariateCopula Clone();

        /// <summary>
        /// Returns the OR joint exceedance probability. When either of the variables exceeds a particular threshold value
        /// </summary>
        /// <param name="u">The reduced variate between 0 and 1.</param>
        public double ORJointExceedanceProbability(double u, double v)
        {
            return 1 - CDF(u, v);
        }

        /// <summary>
        /// Returns the AND joint exceedance probability. When both variables exceed a particular threshold value simultaneously.
        /// </summary>
        /// <param name="u">The reduced variate between 0 and 1.</param>
        public double ANDJointExceedanceProbability(double u, double v)
        {
            return 1 - u - v + CDF(u, v);
        }

        /// <summary>
        /// Generate random values of a distribution given a sample size.
        /// </summary>
        /// <param name="sampleSize"> Size of random sample to generate. </param>
        /// <param name="seed">Optional. The prng seed. If negative or zero, then the computer clock is used as a seed.</param>
        public double[,] GenerateRandomValues(int sampleSize, int seed = -1)
        {
            var rand = LatinHypercube.Random(sampleSize, 2, seed);
            var sample = new double[sampleSize, 2];
            for (int i = 0; i < sampleSize; i++)
            {
                //var vals = InverseCDF(r.NextDouble(), r.NextDouble());
                var vals = InverseCDF(rand[i, 0], rand[i, 1]);
                if (MarginalDistributionX != null && MarginalDistributionY != null)
                {
                    sample[i, 0] = MarginalDistributionX.InverseCDF(vals[0]);
                    sample[i, 1] = MarginalDistributionY.InverseCDF(vals[1]);
                }
                else
                {
                    sample[i, 0] = vals[0];
                    sample[i, 1] = vals[1];
                }
            }
            return sample;
        }

        /// <summary>
        /// The pseudo log-likelihood function.
        /// </summary>
        /// <param name="sampleDataX">The sample data for the X variable. When estimating with pseudo likelihood, this should be the plotting positions of the data.</param>
        /// <param name="sampleDataY">The sample data for the Y variable. When estimating with pseudo likelihood, this should be the plotting positions of the data.</param>
        public double PseudoLogLikelihood(IList<double> sampleDataX, IList<double> sampleDataY)
        {
            if (sampleDataX.Count < 2 || sampleDataY.Count < 2) throw new ArgumentOutOfRangeException("sample data", "There must be at least two items in each sample.");
            if (sampleDataX.Count != sampleDataY.Count) throw new ArgumentOutOfRangeException("sample data", "The sample data arrays must be the same length.");
           
            double LogLH = 0;
            for (int i = 0; i < sampleDataX.Count; i++)
                LogLH += LogPDF(sampleDataX[i], sampleDataY[i]);
            return LogLH;
        }

        /// <summary>
        /// The inference from margins (IFM) log-likelihood function. The marginal distribution are assumed to have already been estimated independently. 
        /// </summary>
        /// <param name="sampleDataX">The sample data for the X variable.</param>
        /// <param name="sampleDataY">The sample data for the Y variable.</param>
        public double IFMLogLikelihood(IList<double> sampleDataX, IList<double> sampleDataY)
        {
            if (sampleDataX.Count < 2 || sampleDataY.Count < 2) throw new ArgumentOutOfRangeException("sample data", "There must be at least two items in each sample.");
            if (sampleDataX.Count != sampleDataY.Count) throw new ArgumentOutOfRangeException("sample data", "The sample data arrays must be the same length.");
            if (MarginalDistributionX == null || MarginalDistributionY == null) throw new ArgumentOutOfRangeException("marginal distributions", "There must be 2 marginal distributions to evaluate.");

            double LogLH = 0;
            for (int i = 0; i < sampleDataX.Count; i++)
                LogLH += LogPDF(MarginalDistributionX.CDF(sampleDataX[i]), MarginalDistributionY.CDF(sampleDataY[i]));
            return LogLH;
        }

        /// <summary>
        /// The full log-likelihood function. The marginal distributions are estimated simultaneously with the copula. 
        /// </summary>
        /// <param name="sampleDataX">The sample data for the X variable.</param>
        /// <param name="sampleDataY">The sample data for the Y variable.</param>
        public double LogLikelihood(IList<double> sampleDataX, IList<double> sampleDataY)
        {
            if (sampleDataX.Count < 2 || sampleDataY.Count < 2) throw new ArgumentOutOfRangeException("sample data", "There must be at least two items in each sample.");
            if (sampleDataX.Count != sampleDataY.Count) throw new ArgumentOutOfRangeException("sample data", "The sample data arrays must be the same length.");
            if (MarginalDistributionX == null || MarginalDistributionY == null) throw new ArgumentOutOfRangeException("marginal distributions", "There must be 2 marginal distributions to evaluate.");

            double LogLH = 0;
            for (int i = 0; i < sampleDataX.Count; i++)
                LogLH += LogPDF(MarginalDistributionX.CDF(sampleDataX[i]), MarginalDistributionY.CDF(sampleDataY[i])) + MarginalDistributionX.LogPDF(sampleDataX[i]) + MarginalDistributionY.LogPDF(sampleDataY[i]);
            return LogLH;
        }


        #endregion
    }
}
