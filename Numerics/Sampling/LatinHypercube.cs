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

using System;
using System.Collections.Generic;
using System.Linq;

namespace Numerics.Sampling
{
    /// <summary>
    /// A class to perform Latin hypercube sampling, or LHS.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    /// Uniform random numbers are sampled by stratifying U(0,1) into n bins of equal probability, 
    /// where n is the number of iterations that are to be sampled. In the first iteration, one of these bins
    /// is selected using a random number. A second random number is then generated to select a value within that bin.
    /// The process is repeated for the second iteration, but the bin used in the first iteration is marked 
    /// as having already been used and therefore will not be selected again. This process is repeated for 
    /// all of the sample iterations.
    /// </para>
    /// </remarks>
    public class LatinHypercube
    {

        /// <summary>
        /// Generate random samples using the Latin hypercube method. 
        /// </summary>
        /// <param name="sampleSize">The number of random samples.</param>
        /// <param name="dimension">Optional. The spatial dimension. Default = 1.</param>
        /// <param name="seed">Optional. The prng seed. If negative or zero, then the computer clock is used as a seed.</param>
        public static double[,] Random(int sampleSize, int dimension = 1, int seed = -1)
        {

            int N = sampleSize;
            var lhs = new double[N, dimension];

            // Create random number generators
            var rndM = seed > 0 ? new MersenneTwister(seed) : new MersenneTwister();
            var rnd = new MersenneTwister[dimension, 2];
            for (int i = 0; i < dimension; i++)
            {
                rnd[i, 0] = new MersenneTwister(rndM.Next(1, dimension * 10000 + 1));
                rnd[i, 1] = new MersenneTwister(rndM.Next(1, dimension * 10000 + 1));
            }

            for (int col = 0; col < dimension; col++)
            {
                // Create N bins of equal probability, 
                // and randomly generate a number within that bin
                var bins = new List<double>();
                for (int i = 1; i <= N; i++)
                    bins.Add((rnd[col, 0].NextDouble() + i - 1d) / N);

                // Sample random bin without replacement
                //var bins = populationBins.ToList();
                for (int row = 0; row < N; row++)
                {
                    int r = rnd[col, 1].Next(0, bins.Count);
                    lhs[row, col] = bins[r];
                    bins.RemoveAt(r);
                }

            }

            return lhs;
        }

        /// <summary>
        /// Generate samples using the median Latin hypercube method. 
        /// </summary>
        /// <param name="sampleSize">The number of samples.</param>
        /// <param name="dimension">Optional. The spatial dimension. Default = 1.</param>
        /// <param name="seed">Optional. The prng seed. If negative or zero, then the computer clock is used as a seed.</param>
        public static double[,] Median(int sampleSize, int dimension = 1, int seed = -1)
        {

            int N = sampleSize;
            var lhs = new double[N, dimension];

            // Create random number generators
            var rndM = seed > 0 ? new MersenneTwister(seed) : new MersenneTwister();
            var rnd = new MersenneTwister[dimension];
            for (int i = 0; i < dimension; i++)
                rnd[i] = new MersenneTwister(rndM.Next(1, dimension * 10000 + 1));

            for (int col = 0; col < dimension; col++)
            {
                // Create N bins of equal probability, 
                // and randomly generate a number within that bin
                var bins = new List<double>();
                for (int i = 1; i <= N; i++)
                    bins.Add((0.5 + i - 1d) / N);

                // Sample random bin without replacement
                //var bins = populationBins.ToList();
                for (int row = 0; row < N; row++)
                {
                    int r = rnd[col].Next(0, bins.Count);
                    lhs[row, col] = bins[r];
                    bins.RemoveAt(r);
                }

            }

            return lhs;
        }


    }
}
