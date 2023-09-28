using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Distributions;

namespace Distributions.Univariate
{
    [TestClass]
    public class Test_InverseChiSquared
    {
        /// <summary>
        /// Verified using Accord.Net
        /// </summary>
        [TestMethod()]
        public void Test_InverseChiSquaredDist()
        {
            double true_mean = 0.2;
            double true_median = 6.345811068141737d;
            double true_pdf = 0.0000063457380298844403d;
            double true_cdf = 0.50860033566176044d;
            double true_icdf = 6.27d;
            var IG = new InverseChiSquared(7, (1d / 7d));
            double pdf = IG.PDF(6.27d);
            double cdf = IG.CDF(6.27d);
            double icdf = IG.InverseCDF(cdf);
            Assert.AreEqual(IG.Mean, true_mean, 0.0001d);
            Assert.AreEqual(IG.Median, true_median, 0.0001d);
            Assert.AreEqual(IG.PDF(6.27d), true_pdf, 0.0001d);
            Assert.AreEqual(IG.CDF(6.27d), true_cdf, 0.0001d);
            Assert.AreEqual(IG.InverseCDF(IG.CDF(6.27d)), true_icdf, 0.0001d);

        }
    }
}
