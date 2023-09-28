using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Distributions;

namespace Distributions.Univariate
{
    [TestClass]
    public class Test_Beta
    {
        /// <summary>
        /// Verified using Palisade's @Risk and Accord.Net
        /// </summary>
        [TestMethod()]
        public void Test_BetaDist()
        {
            double true_mean = 0.21105527638190955d;
            double true_mode = 0.0d;
            double true_median = 0.11577711097114812d;
            double true_stdDev = Math.Sqrt(0.055689279830523512d);
            double true_skew = 1.2275d;
            double true_kurt = 3.6048d;
            double true_pdf = 0.94644031936694828d;
            double true_cdf = 0.69358638272337991d;
            double true_icdf = 0.27d;
            double true_icdf05 = 0.000459d;
            double true_icdf95 = 0.7238d;
            var B = new BetaDistribution(0.42d, 1.57d);
            Assert.AreEqual(B.Mean, true_mean, 0.0001d);
            Assert.AreEqual(B.Median, true_median, 0.0001d);
            Assert.AreEqual(B.Mode, true_mode, 0.0001d);
            Assert.AreEqual(B.StandardDeviation, true_stdDev, 0.0001d);
            Assert.AreEqual(B.Skew, true_skew, 0.0001d);
            Assert.AreEqual(B.Kurtosis, true_kurt, 0.0001d);
            Assert.AreEqual(B.PDF(0.27d), true_pdf, 0.0001d);
            Assert.AreEqual(B.CDF(0.27d), true_cdf, 0.0001d);
            Assert.AreEqual(B.InverseCDF(B.CDF(0.27d)), true_icdf, 0.0001d);
            Assert.AreEqual(B.InverseCDF(0.05d), true_icdf05, 0.0001d);
            Assert.AreEqual(B.InverseCDF(0.95d), true_icdf95, 0.0001d);

        }
    }
}
