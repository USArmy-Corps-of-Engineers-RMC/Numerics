using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Distributions;

namespace Distributions.Univariate
{
    [TestClass]
    public class Test_Binomial
    {
        /// <summary>
        /// Verified using Palisade's @Risk and Accord.Net
        /// </summary>
        [TestMethod()]
        public void Test_BinomialDist()
        {
            double true_mean = 1.92d;
            double true_mode = 2.0d;
            double true_median = 2.0d;
            double true_stdDev = Math.Sqrt(1.6896d);
            double true_skew = 0.5847d;
            double true_kurt = 3.2169d;
            double true_pdf = 0.28218979948821621d;
            double true_cdf = 0.12933699143209909d;
            double true_icdf05 = 0.0d;
            double true_icdf95 = 4.0d;
            var B = new Binomial(0.12d, 16);
            Assert.AreEqual(B.Mean, true_mean, 0.0001d);
            Assert.AreEqual(B.Median, true_median, 0.0001d);
            Assert.AreEqual(B.Mode, true_mode, 0.0001d);
            Assert.AreEqual(B.StandardDeviation, true_stdDev, 0.0001d);
            Assert.AreEqual(B.Skew, true_skew, 0.0001d);
            Assert.AreEqual(B.Kurtosis, true_kurt, 0.0001d);
            Assert.AreEqual(B.PDF(1.0d), true_pdf, 0.0001d);
            Assert.AreEqual(B.CDF(0.0d), true_cdf, 0.0001d);
            Assert.AreEqual(B.InverseCDF(0.05d), true_icdf05, 0.0001d);
            Assert.AreEqual(B.InverseCDF(0.95d), true_icdf95, 0.0001d);
        }
    }
}
