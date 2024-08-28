using Numerics.MachineLearning;
using Numerics.Mathematics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numerics.MachineLearning
{
    public class DenseLayer : ILayer
    {

        public DenseLayer(int inputSize, int outputSize) 
        { 
            InputSize = inputSize;
            OutputSize = outputSize;
            var rnd = new Random();
            var weights = rnd.NextDoubles(InputSize, OutputSize);
            var bias = rnd.NextDoubles(1, OutputSize);
            Weights = new Matrix(weights);
            Bias = new Matrix(bias);
        }



        /// <summary>
        /// The number of input neurons.
        /// </summary>
        public int InputSize { get; private set; }

        /// <summary>
        /// The number of output neurons.
        /// </summary>
        public int OutputSize { get; private set; }

        public Matrix Weights { get; private set; }

        public Matrix Bias { get; private set; }


        public Matrix BackwardPropogation(Vector error, double learningRate)
        {
            throw new NotImplementedException();
        }

        public Matrix ForwardPropogation(Matrix input)
        {
            return (Weights * input) + Bias;
        }
    }
}
