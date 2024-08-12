using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Distributions;

namespace Distributions.Univariate
{
    [TestClass]
    public class Test_Poisson
    {

        /// <summary>
        /// Verified using Palisade's @Risk and Accord.Net
        /// </summary>
        [TestMethod()]
        public void Test_PoissonDist()
        {
            double true_mean = 4.2d;
            double true_mode = 4.0d;
            double true_median = 4.0d;
            double true_stdDev = Math.Sqrt(4.2d);
            double true_skew = 0.488d;
            double true_kurt = 3.2381d;
            double true_pdf = 0.19442365170822165d;
            double true_cdf = 0.58982702131057763d;
            double true_icdf05 = 1.0d;
            double true_icdf95 = 8.0d;
            var P = new Poisson(4.2d);
            Assert.AreEqual(P.Mean, true_mean, 0.0001d);
            Assert.AreEqual(P.Median, true_median, 0.0001d);
            Assert.AreEqual(P.Mode, true_mode, 0.0001d);
            Assert.AreEqual(P.StandardDeviation, true_stdDev, 0.0001d);
            Assert.AreEqual(P.Skewness, true_skew, 0.0001d);
            Assert.AreEqual(P.Kurtosis, true_kurt, 0.0001d);
            Assert.AreEqual(P.PDF(4.0d), true_pdf, 0.0001d);
            Assert.AreEqual(P.CDF(4.0d), true_cdf, 0.0001d);
            Assert.AreEqual(P.InverseCDF(0.05d), true_icdf05, 0.0001d);
            Assert.AreEqual(P.InverseCDF(0.95d), true_icdf95, 0.0001d);
        }
    }
}
