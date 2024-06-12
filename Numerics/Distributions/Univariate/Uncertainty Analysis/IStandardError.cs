using System.Collections.Generic;

namespace Numerics.Distributions
{

    /// <summary>
    /// An interface for calculating the standard error for a probability distribution.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public interface IStandardError
    {

        /// <summary>
        /// Returns a list containing the variance of each parameter given the sample size and estimation method.
        /// </summary>
        /// <param name="sampleSize">The sample size.</param>
        /// <param name="estimationMethod">The distribution parameter estimation method.</param>
        IList<double> ParameterVariance(int sampleSize, ParameterEstimationMethod estimationMethod);

        /// <summary>
        /// Returns a list containing the covariances of the parameters given the sample size.
        /// </summary>
        /// <param name="sampleSize">The sample size.</param>
        /// <param name="estimationMethod">The distribution parameter estimation method.</param>
        IList<double> ParameterCovariance(int sampleSize, ParameterEstimationMethod estimationMethod);

        /// <summary>
        /// Returns a list of partial derivatives of X given probability with respect to each parameter.
        /// </summary>
        /// <param name="probability">Probability between 0 and 1.</param>
        IList<double> PartialDerivatives(double probability);




        /// <summary>
        /// Returns the quantile variance given probability and sample size.
        /// </summary>
        /// <param name="probability">Probability between 0 and 1.</param>
        /// <param name="sampleSize">The sample size.</param>
        /// <param name="estimationMethod">The distribution parameter estimation method.</param>
        double QuantileVariance(double probability, int sampleSize, ParameterEstimationMethod estimationMethod);
    }
}