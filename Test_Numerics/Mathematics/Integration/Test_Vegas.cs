using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Data.Statistics;
using Numerics.Distributions;
using Numerics.Mathematics.Integration;
using Numerics.Sampling;
using Numerics.Sampling.MCMC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Mathematics.Integration
{
    [TestClass]
    public class Test_Vegas
    {
        [TestMethod()]
        public void Test_PI()
        {
            var vegas = new Vegas((x, y) => { return Integrands.PI(x); }, 2, new double[] { -1, -1 }, new double[] { 1, 1 });
            vegas.Integrate();
            var result = vegas.Result;
            double trueResult = 3.1416;
            Assert.AreEqual(trueResult, result, 1E-3 * trueResult);
        }

        [TestMethod()]
        public void Test_GSL()
        {
            var vegas = new Vegas((x, y) => { return Integrands.GSL(x); }, 3, new double[] { 0, 0, 0 }, new double[] { Math.PI, Math.PI, Math.PI });
            vegas.Integrate();
            var result = vegas.Result;
            double trueResult = 1.3932039296856768591842462603255;
            Assert.AreEqual(trueResult, result, 1E-2 * trueResult);
        }

        [TestMethod()]
        public void Test_SumOfThreeNormals()
        {
            var min = new double[3];
            var max = new double[3];
            for (int i = 0; i < 3; i++)
            {
                min[i] = 1E-16;
                max[i] = 1 - 1E-16;
            }

            var vegas = new Vegas((x, y) => { return Integrands.SumOfNormals(x); }, 3, min, max);
            vegas.Integrate();
            var result = vegas.Result;
            double trueResult = 57;
            Assert.AreEqual(trueResult, result, 1E-3 * trueResult);
        }

        [TestMethod()]
        public void Test_SumOfFiveNormals()
        {
            var min = new double[5];
            var max = new double[5];
            for (int i = 0; i < 5; i++)
            {
                min[i] = 1E-16;
                max[i] = 1 - 1E-16;
            }

            var vegas = new Vegas((x, y) => { return Integrands.SumOfNormals(x); }, 5, min, max);
            vegas.Integrate();
            var result = vegas.Result;
            double trueResult = 224;
            Assert.AreEqual(trueResult, result, 1E-3 * trueResult);
        }

        [TestMethod()]
        public void Test_SumOfTwentyNormals()
        {
            var min = new double[20];
            var max = new double[20];
            for (int i = 0; i < 20; i++)
            {
                min[i] = 1E-16;
                max[i] = 1 - 1E-16;
            }

            var vegas = new Vegas((x, y) => { return Integrands.SumOfNormals(x); }, 20, min, max);
            vegas.Integrate();
            var result = vegas.Result;
            double trueResult = 837;
            Assert.AreEqual(trueResult, result, 1E-3 * trueResult);
        }

    }
}
