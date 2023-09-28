using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Distributions;

namespace Distributions.Univariate
{
    [TestClass]
    public class Test_Pareto
    {

        /// <summary>
        /// Verified using Palisade's @Risk and Accord.Net
        /// </summary>
        [TestMethod()]
        public void Test_ParetoDist()
        {
            double true_mean = 0.63d;
            double true_mode = 0.42d;
            double true_median = 0.52916684095584676d;
            double true_stdDev = Math.Sqrt(0.13229999999999997d);
            double true_pdf = 0.057857142857142857d;
            double true_cdf = 0.973d;
            double true_icdf = 1.4d;
            double true_icdf05 = 0.4272d;
            double true_icdf95 = 1.1401d;
            var PA = new Pareto(0.42d, 3d);
            Assert.AreEqual(PA.Mean, true_mean, 0.0001d);
            Assert.AreEqual(PA.Median, true_median, 0.0001d);
            Assert.AreEqual(PA.Mode, true_mode, 0.0001d);
            Assert.AreEqual(PA.StandardDeviation, true_stdDev, 0.0001d);
            Assert.AreEqual(PA.PDF(1.4d), true_pdf, 0.0001d);
            Assert.AreEqual(PA.CDF(1.4d), true_cdf, 0.0001d);
            Assert.AreEqual(PA.InverseCDF(PA.CDF(1.4d)), true_icdf, 0.0001d);
            Assert.AreEqual(PA.InverseCDF(0.05d), true_icdf05, 0.0001d);
            Assert.AreEqual(PA.InverseCDF(0.95d), true_icdf95, 0.0001d);
            PA.SetParameters(new[] { 1d, 10d });
            double true_skew = 2.8111d;
            double true_kurt = 17.8286d;
            Assert.AreEqual(PA.Skew, true_skew, 0.0001d);
            Assert.AreEqual(PA.Kurtosis, true_kurt, 0.0001d);
        }
    }
}
