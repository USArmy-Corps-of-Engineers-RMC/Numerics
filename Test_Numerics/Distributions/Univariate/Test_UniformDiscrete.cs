using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Distributions;

namespace Distributions.Univariate
{
    [TestClass]
    public class Test_UniformDiscrete
    {

        /// <summary>
        /// Verified using Palisade's @Risk and Accord.Net
        /// </summary>
        [TestMethod()]
        public void Test_UniformDiscreteDist()
        {
            double true_mean = 4.0d;
            double true_median = 4.0d;
            double true_stdDev = Math.Sqrt(1.3333333333333333d);
            int true_skew = 0;
            double true_kurt = 1.7d;
            double true_pdf = 0.2d;
            double true_cdf = 0.2d;
            double true_icdf05 = 2.0d;
            double true_icdf95 = 6.0d;
            var U = new UniformDiscrete(2d, 6d);
            Assert.AreEqual(U.Mean, true_mean, 0.0001d);
            Assert.AreEqual(U.Median, true_median, 0.0001d);
            Assert.AreEqual(U.StandardDeviation, true_stdDev, 0.0001d);
            Assert.AreEqual(U.Skewness, true_skew, 0.0001d);
            Assert.AreEqual(U.Kurtosis, true_kurt, 0.0001d);
            Assert.AreEqual(U.PDF(4.0d), true_pdf, 0.0001d);
            Assert.AreEqual(U.CDF(2.0d), true_cdf, 0.0001d);
            Assert.AreEqual(U.InverseCDF(0.17d), true_icdf05, 0.0001d);
            Assert.AreEqual(U.InverseCDF(0.87d), true_icdf95, 0.0001d);
        }
    }
}
