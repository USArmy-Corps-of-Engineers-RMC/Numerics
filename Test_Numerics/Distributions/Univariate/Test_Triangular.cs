using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Distributions;

namespace Distributions.Univariate
{
    [TestClass]
    public class Test_Triangular
    {

        /// <summary>
        /// Verified using Accord.Net
        /// </summary>
        [TestMethod()]
        public void Test_TriangularDist()
        {
            double true_mean = 3.3333333333333335d;
            double true_median = 3.2613872124741694d;
            double true_mode = 3.0d;
            double true_stdDev = Math.Sqrt(1.0555555555555556d);
            double true_pdf = 0.20000000000000001d;
            double true_cdf = 0.10000000000000001d;
            double true_icdf = 2.0d;
            var T = new Triangular(1, 3, 6);

            var m = T.Mean * 3 - 6 - 1;

            Assert.AreEqual(T.Mean, true_mean, 0.0001d);
            Assert.AreEqual(T.Median, true_median, 0.0001d);
            Assert.AreEqual(T.Mode, true_mode, 0.0001d);
            Assert.AreEqual(T.StandardDeviation, true_stdDev, 0.0001d);
            Assert.AreEqual(T.PDF(2.0d), true_pdf, 0.0001d);
            Assert.AreEqual(T.CDF(2.0d), true_cdf, 0.0001d);
            Assert.AreEqual(T.InverseCDF(true_cdf), true_icdf, 0.0001d);
        }

        /// <summary>
        /// Verified using R 'mc2d'
        /// </summary>
        [TestMethod]
        public void Test_Triangular_R()
        {
            var x = new double[] { 0.1, 0.25, 0.5, 0.75, 0.9 };
            var p = new double[5];
            var true_p = new double[] { 0.0400000, 0.2500000, 0.6666667, 0.9166667, 0.9866667 };
            var tri = new Triangular(0, 0.25, 1);
            for (int i = 0; i < 5; i++)
            {
                p[i] = tri.CDF(x[i]);
                Assert.AreEqual(true_p[i], p[i], 1E-7);
            }
            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(x[i], tri.InverseCDF(p[i]), 1E-7);
            }
        }

        [TestMethod]
        public void Test_Triangular_MOM()
        {
            var dist = new Triangular(-2, 10, 35);
            var sample = dist.GenerateRandomValues(12345, 1000);
            dist.Estimate(sample, ParameterEstimationMethod.MethodOfMoments);
            Assert.AreEqual(-2, dist.Min, 1);
            Assert.AreEqual(10, dist.MostLikely, 1);
            Assert.AreEqual(35, dist.Max, 1);
        }

        [TestMethod]
        public void Test_Triangular_MLE()
        {
            var dist = new Triangular(-2, 10, 35);
            var sample = dist.GenerateRandomValues(12345, 1000);
            dist.Estimate(sample, ParameterEstimationMethod.MaximumLikelihood);
            Assert.AreEqual(-2, dist.Min, 1);
            Assert.AreEqual(10, dist.MostLikely, 1);
            Assert.AreEqual(35, dist.Max, 1);
        }
    }
}
