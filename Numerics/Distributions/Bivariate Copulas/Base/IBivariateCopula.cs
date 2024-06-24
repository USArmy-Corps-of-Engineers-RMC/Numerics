using System;
using System.Collections.Generic;

namespace Numerics.Distributions.Copulas
{

    /// <summary>
    /// Interface for Copula Joint Distributions.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public interface IBivariateCopula : IDistribution
    {

        /// <summary>
        /// Returns the Copula type.
        /// </summary>
        CopulaType Type { get; }

        /// <summary>
        /// The dependency parameter, theta θ
        /// </summary>
        double Theta { get; set; }

        /// <summary>
        /// Returns the minimum value allowable for the dependency parameter.
        /// </summary>
        double ThetaMinimum { get; }

        /// <summary>
        /// Returns the maximum values allowable for the dependency parameter.
        /// </summary>
        double ThetaMaximum { get; }

        /// <summary>
        /// Returns a boolean value describing if the current parameters are valid or not.
        /// If not, an ArgumentOutOfRange exception will be thrown when trying to use statistical functions (e.g. CDF())
        /// </summary>
        bool ParametersValid { get; }

        /// <summary>
        /// The X marginal distribution for the copula. 
        /// </summary>
        IUnivariateDistribution MarginalDistributionX { get; set; }

        /// <summary>
        /// The Y marginal distribution for the copula. 
        /// </summary>
        IUnivariateDistribution MarginalDistributionY { get; set; }

        /// <summary>
        /// Test to see if distribution parameters are valid.
        /// </summary>
        /// <param name="parameter">Dependency parameter.</param>
        /// <param name="throwException">Boolean indicating whether to throw the exception or not.</param>
        /// <returns>Nothing if the parameters are valid and the exception if invalid parameters were found.</returns>
        ArgumentOutOfRangeException ValidateParameter(double parameter, bool throwException);

        /// <summary>
        /// Returns the parameter constraints for the dependency parameter given the data samples. 
        /// </summary>
        /// <param name="sampleDataX">The sample data for the X variable.</param>
        /// <param name="sampleDataY">The sample data for the Y variable.</param>
        double[] ParameterConstraints(IList<double> sampleDataX, IList<double> sampleDataY);

        /// <summary>
        /// The probability density function (PDF) of the copula evaluated at reduced variates u and v.
        /// </summary>
        /// <param name="u">The reduced variate between 0 and 1.</param>
        /// <param name="v">The reduced variate between 0 and 1.</param>
        double PDF(double u, double v);

        /// <summary>
        /// Returns the natural log of the PDF.
        /// </summary>
        /// <param name="u">The reduced variate between 0 and 1.</param>
        /// <param name="v">The reduced variate between 0 and 1.</param>
        double LogPDF(double u, double v);

        /// <summary>
        /// The cumulative distribution function (CDF) of the copula evaluated at reduced variates u and v.
        /// </summary>
        /// <param name="u">The reduced variate between 0 and 1.</param>
        /// <param name="v">The reduced variate between 0 and 1.</param>
        double CDF(double u, double v);

        /// <summary>
        /// The inverse cumulative distribution function (InvCDF) of the copula evaluated at probabilities u and v.
        /// </summary>
        /// <param name="u">Probability between 0 and 1.</param>
        /// <param name="v">Probability between 0 and 1.</param>
        double[] InverseCDF(double u, double v);

        /// <summary>
        /// Generate random values of a distribution given a sample size.
        /// </summary>
        /// <param name="sampleSize"> Size of random sample to generate. </param>
        /// <param name="seed">Optional. The prng seed. If negative or zero, then the computer clock is used as a seed.</param>
        double[,] GenerateRandomValues(int sampleSize, int seed = -1);

    }
}