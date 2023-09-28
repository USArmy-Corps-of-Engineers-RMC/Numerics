using Numerics.Mathematics.LinearAlgebra;
using System.Collections.Generic;
using System.Linq;

namespace Numerics.Data.Statistics
{
    /// <summary>
    /// A class for keeping track of a running covariance matrix.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public class RunningCovarianceMatrix
    {
        /// <summary>
        /// Construct a covariance matrix.
        /// </summary>
        /// <param name="size">The number of rows and columns.</param>
        public RunningCovarianceMatrix(int size)
        {
            // The mean vector set at zero.
            Mean = new Matrix(size, 1);
            // The initial covariance  is the identity matrix
            Covariance = Matrix.Identity(size);
        }

        /// <summary>
        /// The sample size N.
        /// </summary>
        public int N { get; private set; }

        /// <summary>
        /// The mean vector
        /// </summary>
        public Matrix Mean { get; private set; }

        /// <summary>
        /// The covariance matrix. This is unadjusted by the sample size.
        /// </summary>
        public Matrix Covariance { get; private set; }

        /// <summary>
        /// Add a new vector to the running statistics. 
        /// </summary>
        /// <param name="values">Vector of data values.</param>
        public void Push(IList<double> values)
        {
            N += 1;
            var x = new Matrix(values.ToArray());
            // Update mean
            var oldMean = Mean;
            Mean += N == 1 ? x : (x - Mean) * (1d / N);
            // Update covariance                 
            Covariance += (x - oldMean) * Matrix.Transpose(x - Mean);
        }

    }
}
