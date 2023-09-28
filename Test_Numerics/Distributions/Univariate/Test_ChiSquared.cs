using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Distributions;

namespace Distributions.Univariate
{
    [TestClass]
    public class Test_ChiSquared
    {
        /// <summary>
        /// Verified using Accord.Net
        /// </summary>
        [TestMethod()]
        public void Test_ChiSquaredDist()
        {
            double true_mean = 7d;
            double true_median = 6.345811195595612d;
            double true_stdDev = Math.Sqrt(14d);
            double true_pdf = 0.11388708001184455d;
            double true_cdf = 0.49139966433823956d;
            double true_icdf = 6.27d;
            var CHI = new ChiSquared(7);
            Assert.AreEqual(CHI.Mean, true_mean, 0.0001d);
            Assert.AreEqual(CHI.Median, true_median, 0.0001d);
            Assert.AreEqual(CHI.StandardDeviation, true_stdDev, 0.0001d);
            Assert.AreEqual(CHI.PDF(6.27d), true_pdf, 0.0001d);
            Assert.AreEqual(CHI.CDF(6.27d), true_cdf, 0.0001d);
            Assert.AreEqual(CHI.InverseCDF(true_cdf), true_icdf, 0.0001d);
        }
    }
}
