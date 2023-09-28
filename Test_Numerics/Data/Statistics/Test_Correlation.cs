using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Data.Statistics;

namespace Data.Statistics
{
    [TestClass]
    public class Test_Correlation
    {
        [TestMethod()]
        public void Test_Pearson()
        {
            var XArray = new double[] { 14d, 8d, 32d, 7d, 3d, 15d };
            var YArray = new double[] { 10d, 5d, 7d, 4d, 3d, 8d };
            double R = Correlation.Pearson(XArray, YArray);
            double trueR = 0.54502739907793d;
            Assert.AreEqual(R, trueR, 0.0000000001d);
        }

        [TestMethod()]
        public void Test_Spearman()
        {
            var XArray = new double[] { 14d, 8d, 32d, 7d, 3d, 15d };
            var YArray = new double[] { 10d, 5d, 7d, 4d, 3d, 8d };
            double R = Correlation.Spearman(XArray, YArray);
            double trueR = 0.771428571428571d;
            Assert.AreEqual(R, trueR, 0.0000000001d);
        }

        [TestMethod()]
        public void Test_KendallsTau()
        {
            var XArray = new double[] { 14d, 8d, 32d, 7d, 3d, 15d };
            var YArray = new double[] { 10d, 5d, 7d, 4d, 3d, 8d };
            double R = Correlation.KendallsTau(XArray, YArray);
            double trueR = 0.6d;
            Assert.AreEqual(R, trueR, 0.0000000001d);
        }
    }
}
