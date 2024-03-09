using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Distributions;

namespace Distributions.Univariate
{
    [TestClass]
    public class Test_Pert
    {

        [TestMethod]
        public void Test_PertDist()
        {
            var P = new Pert(1d, 2d, 3d);
            var GB = GeneralizedBeta.PERT(1d, 2d, 3d);

            double true_mean = 2d;
            double true_median = 2d;
            double true_mode = 2d;
            double true_pdf = GB.PDF(1.27);
            double true_cdf = GB.CDF(1.27);
            double true_icdf = 1.27d;

            Assert.AreEqual(P.Mean, true_mean, 0.0001d);
            Assert.AreEqual(P.Median, true_median, 0.0001d);
            Assert.AreEqual(P.Mode, true_mode, 0.0001d);
            Assert.AreEqual(P.PDF(1.27d), true_pdf, 0.0001d);
            Assert.AreEqual(P.CDF(1.27d), true_cdf, 0.0001d);
            Assert.AreEqual(P.InverseCDF(P.CDF(1.27d)), true_icdf, 0.0001d);

        }

        /// <summary>
        /// Verified against R "mc2d" package
        /// </summary>
        [TestMethod]
        public void Test_PertDist2()
        {
            var P = new Pert(-1, 0, 1);
            var d = P.PDF(-0.5);
            var p = P.CDF(-0.5);
            var q = P.InverseCDF(p);

            Assert.AreEqual(d, 0.5273438, 1E-5);
            Assert.AreEqual(p, 0.1035156, 1E-5);
            Assert.AreEqual(q, -0.5, 1E-5);

        }

        /// <summary>
        /// Verified using R 'mc2d'
        /// </summary>
        [TestMethod]
        public void Test_Pert_R()
        {
            var x = new double[] { 0.1, 0.25, 0.5, 0.75, 0.9 };
            var p = new double[5];
            var true_p = new double[] { 0.0814600, 0.3671875, 0.8125000, 0.9843750, 0.9995400 };
            var pert = new Pert(0, 0.25, 1);
            for (int i = 0; i < 5; i++)
            {
                p[i] = pert.CDF(x[i]);
                Assert.AreEqual(true_p[i], p[i], 1E-7);
            }
            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(x[i], pert.InverseCDF(p[i]), 1E-7);
            }
        }

        [TestMethod]
        public void Test_Pert_MOM()
        {
            var dist = new Pert(-2, 10, 35);
            var sample = dist.GenerateRandomValues(12345, 1000);
            dist.Estimate(sample, ParameterEstimationMethod.MethodOfMoments);
            Assert.AreEqual(-2, dist.Min, 1);
            Assert.AreEqual(10, dist.MostLikely, 1);
            Assert.AreEqual(35, dist.Max, 5);
        }

        [TestMethod]
        public void Test_Pert_MLE()
        {
            var dist = new Pert(-2, 10, 35);
            var sample = dist.GenerateRandomValues(12345, 1000);
            dist.Estimate(sample, ParameterEstimationMethod.MaximumLikelihood);
            Assert.AreEqual(-2, dist.Min, 1);
            Assert.AreEqual(10, dist.MostLikely, 1);
            Assert.AreEqual(35, dist.Max, 1);
        }
    }
}
