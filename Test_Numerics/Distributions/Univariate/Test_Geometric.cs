using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Distributions;

namespace Distributions.Univariate
{
    [TestClass]
    public class Test_Geometric
    {
        /// <summary>
        /// Verified using Palisade's @Risk and Accord.Net
        /// </summary>
        [TestMethod()]
        public void Test_GeometricDist()
        {
            double true_mean = 1.3809523809523812d;
            double true_mode = 0.0d;
            double true_median = 1.0d;
            double true_stdDev = Math.Sqrt(3.2879818594104315d);
            double true_skew = 2.0746d;
            double true_kurt = 9.3041d;
            double true_pdf = 0.141288d;
            double true_cdf = 0.80488799999999994d;
            double true_icdf05 = 0.0d;
            double true_icdf95 = 5.0d;
            var G = new Geometric(0.42d);
            Assert.AreEqual(G.Mean, true_mean, 0.0001d);
            Assert.AreEqual(G.Median, true_median, 0.0001d);
            Assert.AreEqual(G.Mode, true_mode, 0.0001d);
            Assert.AreEqual(G.StandardDeviation, true_stdDev, 0.0001d);
            Assert.AreEqual(G.Skew, true_skew, 0.0001d);
            Assert.AreEqual(G.Kurtosis, true_kurt, 0.0001d);
            Assert.AreEqual(G.PDF(2.0d), true_pdf, 0.0001d);
            Assert.AreEqual(G.CDF(2.0d), true_cdf, 0.0001d);
            Assert.AreEqual(G.InverseCDF(0.05d), true_icdf05, 0.0001d);
            Assert.AreEqual(G.InverseCDF(0.95d), true_icdf95, 0.0001d);
        }
    }
}
