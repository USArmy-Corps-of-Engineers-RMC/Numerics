using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Distributions;

namespace Distributions.Univariate
{
    [TestClass]
    public class Test_InverseGamma
    {
        /// <summary>
        /// Verified using Accord.Net
        /// </summary>
        [TestMethod()]
        public void Test_InverseGammaDist()
        {
            double true_mean = -0.86206896551724133d;
            double true_median = 3.1072323347401709d;
            double true_pdf = 0.35679850067181362d;
            double true_cdf = 0.042243552114989695d;
            double true_icdf05 = 0.26999994629410995d;
            var IG = new InverseGamma(0.5d, 0.42d);
            Assert.AreEqual(IG.Mean, true_mean, 0.0001d);
            Assert.AreEqual(IG.Median, true_median, 0.0001d);
            Assert.AreEqual(IG.PDF(0.27d), true_pdf, 0.0001d);
            Assert.AreEqual(IG.CDF(0.27d), true_cdf, 0.0001d);
            Assert.AreEqual(IG.InverseCDF(IG.CDF(0.27d)), true_icdf05, 0.0001d);
        }
    }
}
