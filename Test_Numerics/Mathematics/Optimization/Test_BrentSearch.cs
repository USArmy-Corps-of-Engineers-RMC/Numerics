using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Mathematics.Optimization;

namespace Mathematics.Optimization
{
    [TestClass]
    public class Test_BrentSearch
    {
        [TestMethod]
        public void Test_Minimize()
        {
            double lower = -3d;
            double upper = 3d;
            var golden = new BrentSearch(TestFunctions.FX, lower, upper);
            golden.Minimize();
            double X = golden.BestParameterSet.Values[0];
            double trueX = 1.0d;
            Assert.AreEqual(X, trueX, 0.001d);
        }

        [TestMethod]
        public void Test_Maximize()
        {
            double lower = -3;
            double upper = 3d;
            var golden = new BrentSearch(TestFunctions.FX, lower, upper);
            golden.Maximize();
            double X = golden.BestParameterSet.Values[0];
            double trueX = -1.6667d;
            Assert.AreEqual(X, trueX, 0.001d);
        }
    }
}
