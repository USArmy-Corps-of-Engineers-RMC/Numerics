using Numerics.Data.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numerics.MachineLearning
{
    /// <summary>
    /// Supporting class for a k-Means cluster.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public class KMeansCluster
    {
        /// <summary>
        /// Create a new k-Means cluster.
        /// </summary>
        /// <param name="dimension">The dimensionality (or number of features) of the data space.</param>
        public KMeansCluster(int dimension = 1)
        {
            Dimension = dimension;
            Indices = new List<int>();
            CovarianceMatrix = new RunningCovarianceMatrix(Dimension);
        }

        /// <summary>
        /// The dimensionality (or number of features) of the data space.
        /// </summary>
        public int Dimension { get; }

        /// <summary>
        /// The list of sample indices for this cluster.
        /// </summary>
        public List<int> Indices { get; }

        /// <summary>
        /// The mean vectr and covariance matrix for the cluster.
        /// </summary>
        public RunningCovarianceMatrix CovarianceMatrix { get; }

        /// <summary>
        /// Add a new vector to the running statistics. 
        /// </summary>
        /// <param name="index">The index of the sample values.</param>
        /// <param name="sample">Vector of data sample values.</param>
        public void Push(int index, double[] sample)
        {
            Indices.Add(index);
            CovarianceMatrix.Push(sample);
        }

    }
}
