using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics;
using Numerics.Sampling;

namespace Sampling
{
    [TestClass]
    public class Test_SobolSequence
    {
        [TestMethod]
        public void Test_Sobol()
        {
            var sobol = new SobolSequence(2);
            //for (int i = 1; i <=100; i++)
            //{
            //    var rnd = sobol.NextVector();
            //    Debug.Print(rnd[0].ToString() + "," + rnd[1].ToString());
            //}

            var prng = new MersenneTwister(12345);


            var mst = new MersenneTwister(12345);
            var rnds = mst.NextDoubles(100);

            for (int i = 1; i <= 100; i++)
            {
                var rnd = mst.NextDouble();

                Debug.Print(rnd.ToString());
            }
        }
    }
}
