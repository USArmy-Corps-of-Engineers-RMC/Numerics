using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Distributions;

namespace Distributions.Univariate
{
    [TestClass]
    public class Test_Uniform
    {

        /// <summary>
        /// Verified using Accord.Net
        /// </summary>
        [TestMethod()]
        public void Test_UniformDist()
        {
            double true_mean = 0.76d;
            double true_median = 0.76d;
            double true_stdDev = Math.Sqrt(0.03853333333333335d);
            double true_pdf = 1.4705882352941173d;
            double true_cdf = 0.70588235294117641d;
            double true_icdf = 0.9d;
            var U = new Uniform(0.42d, 1.1d);
            Assert.AreEqual(U.Mean, true_mean, 0.0001d);
            Assert.AreEqual(U.Median, true_median, 0.0001d);
            Assert.AreEqual(U.StandardDeviation, true_stdDev, 0.0001d);
            Assert.AreEqual(U.PDF(0.9d), true_pdf, 0.0001d);
            Assert.AreEqual(U.CDF(0.9d), true_cdf, 0.0001d);
            Assert.AreEqual(U.InverseCDF(true_cdf), true_icdf, 0.0001d);
        }

        /// <summary>
        /// Verified using R 'stats'
        /// </summary>
        [TestMethod]
        public void Test_Uniform_R()
        {
            var x = new double[] { 0.1, 0.25, 0.5, 0.75, 0.9 };
            var p = new double[5];
            var true_p = new double[] { 0.1, 0.25, 0.5, 0.75, 0.9 };
            var u = new Uniform(0, 1);
            for (int i = 0; i < 5; i++)
            {
                p[i] = u.CDF(x[i]);
                Assert.AreEqual(true_p[i], p[i], 1E-7);
            }
            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(x[i], u.InverseCDF(p[i]), 1E-7);
            }
        }
    }
}
