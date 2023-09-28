using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Distributions;

namespace Distributions.Univariate
{
    [TestClass]
    public class Test_Rayleigh
    {

        /// <summary>
        /// Verified using  Accord.Net
        /// </summary>
        [TestMethod()]
        public void Test_RayleighDist()
        {
            double true_mean = 0.52639193767251d;
            double true_median = 0.49451220943852386d;
            double true_stdDev = Math.Sqrt(0.075711527953380237d);
            double true_pdf = 0.030681905868831811d;
            double true_cdf = 0.99613407986052716d;
            double true_icdf = 1.4d;
            var R = new Rayleigh(0.42);
            Assert.AreEqual(R.Mean, true_mean, 0.0001d);
            Assert.AreEqual(R.Median, true_median, 0.0001d);
            Assert.AreEqual(R.StandardDeviation, true_stdDev, 0.0001d);
            Assert.AreEqual(R.PDF(1.4d), true_pdf, 0.0001d);
            Assert.AreEqual(R.CDF(1.4d), true_cdf, 0.0001d);
            Assert.AreEqual(R.InverseCDF(true_cdf), true_icdf, 0.0001d);
        }
    }
}
