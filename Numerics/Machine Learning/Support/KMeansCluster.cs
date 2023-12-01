using Numerics.Data.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numerics.MachineLearning
{
    public class KMeansCluster
    {

        public KMeansCluster(int dimension = 1)
        {
            Dimension = dimension;
            Indices = new List<int>();
            CovarianceMatrix = new RunningCovarianceMatrix(Dimension);
        }

        public int Dimension { get; }

        public List<int> Indices { get; }

        public RunningCovarianceMatrix CovarianceMatrix { get; }

        public void Push(int index, double[] sample)
        {
            Indices.Add(index);
            CovarianceMatrix.Push(sample);
        }

    }
}
