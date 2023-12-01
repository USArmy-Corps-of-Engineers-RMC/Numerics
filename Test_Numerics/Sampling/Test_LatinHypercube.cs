using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Sampling;
using System.Diagnostics;
using Numerics;
using Numerics.Data.Statistics;
using Numerics.Distributions;

namespace Sampling
{
    [TestClass]
    public class Test_LatinHypercube
    {
        [TestMethod]
        public void Test_LHS()
        {
            // Latin Hypercube
            var lhs = LatinHypercube.Random(10000, 2, 45678);
            //for (int i = 0; i < lhs.GetLength(0); i++)
            //    Debug.WriteLine(lhs[i, 0] + "," + lhs[i, 1]);

            // Naive Monte Carlo for comparison
            //var prng = new MersenneTwister(12345);
            //var y = prng.NextDoubles(10000);
            //// prng = new MersenneTwister(45678);
            // var z = prng.NextDoubles(1000);
            // for (int i = 0; i < y.GetLength(0); i++)
            //     Debug.WriteLine(y[i] + "," + z[i]);

            //var rnd = new Random();
            //var vals = rnd.NextIntegers(0, 10, 4, false);
        }

    }
}
