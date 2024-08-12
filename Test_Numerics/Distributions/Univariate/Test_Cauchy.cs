using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Distributions;

namespace Distributions.Univariate
{
    [TestClass]
    public class Test_Cauchy
    {
        /// <summary>
        /// Verified using Palisade's @Risk and Accord.Net
        /// </summary>
        [TestMethod()]
        public void Test_CauchyDist()
        {
            double true_mean = double.NaN;
            double true_mode = 0.42d;
            double true_median = 0.42d;
            double true_stdDev = double.NaN;
            double true_skew = double.NaN;
            double true_kurt = double.NaN;
            double true_pdf = 0.2009112009763413d;
            double true_cdf = 0.46968025841608563d;
            double true_icdf = 1.5130304686978195d;
            var C = new Cauchy(0.42d, 1.57d);
            Assert.AreEqual(C.Mean, true_mean);
            Assert.AreEqual(C.Median, true_median, 0.0001d);
            Assert.AreEqual(C.Mode, true_mode, 0.0001d);
            Assert.AreEqual(C.StandardDeviation, true_stdDev);
            Assert.AreEqual(C.Skewness, true_skew);
            Assert.AreEqual(C.Kurtosis, true_kurt);
            Assert.AreEqual(C.PDF(0.27d), true_pdf, 0.0001d);
            Assert.AreEqual(C.CDF(0.27d), true_cdf, 0.0001d);
            Assert.AreEqual(C.InverseCDF(0.69358638272337991d), true_icdf, 0.0001d);
        }
    }
}
