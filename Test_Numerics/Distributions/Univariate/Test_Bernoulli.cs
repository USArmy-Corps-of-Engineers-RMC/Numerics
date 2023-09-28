using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Distributions;

namespace Distributions.Univariate
{
    [TestClass]
    public class Test_Bernoulli
    {
        /// <summary>
        /// Verified using Palisade's @Risk
        /// </summary>
        [TestMethod()]
        public void Test_BernoulliDist()
        {
            double true_mean = 0.7d;
            int true_mode = 1;
            int true_median = 1;
            double true_stdDev = 0.4583d;
            double true_skew = -0.8729d;
            double true_kurt = 1.7619d;
            double true_pdf = 0.3d;
            double true_cdf = 0.3d;
            double true_icdf05 = 0.0d;
            double true_icdf95 = 1.0d;
            var B = new Bernoulli(0.7d);
            Assert.AreEqual(B.Mean, true_mean, 0.0001d);
            Assert.AreEqual(B.Median, true_median, 0.0001d);
            Assert.AreEqual(B.Mode, true_mode, 0.0001d);
            Assert.AreEqual(B.StandardDeviation, true_stdDev, 0.0001d);
            Assert.AreEqual(B.Skew, true_skew, 0.0001d);
            Assert.AreEqual(B.Kurtosis, true_kurt, 0.0001d);
            Assert.AreEqual(B.PDF(0.0d), true_pdf, 0.0001d);
            Assert.AreEqual(B.CDF(0.5d), true_cdf, 0.0001d);
            Assert.AreEqual(B.InverseCDF(0.05d), true_icdf05, 0.0001d);
            Assert.AreEqual(B.InverseCDF(0.95d), true_icdf95, 0.0001d);
        }
    }
}
