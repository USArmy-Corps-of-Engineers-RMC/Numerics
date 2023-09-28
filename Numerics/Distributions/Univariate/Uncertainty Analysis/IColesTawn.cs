using System.Collections.Generic;

namespace Numerics.Distributions
{

    /// <summary>
    /// An interface for calculating the Jacobian of the inverse CDF given a list of probabilities.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public interface IColesTawn
    {

        /// <summary>
        /// Returns the determinant of the Jacobian.
        /// </summary>
        /// <param name="probabilities">List of probabilities, must be the same length as the number of distribution parameters.</param>
        double Jacobian(IList<double> probabilities);
    }
}