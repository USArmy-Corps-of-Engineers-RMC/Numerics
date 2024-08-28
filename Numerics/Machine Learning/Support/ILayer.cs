using Numerics.Mathematics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numerics.MachineLearning
{
    public interface ILayer
    {
        /// <summary>
        /// Returns the output Y for a given input X.
        /// </summary>
        /// <param name="input">The input matrix.</param>
        Matrix ForwardPropogation(Matrix input);

        Matrix BackwardPropogation(Vector error, double learningRate);

    }
}
