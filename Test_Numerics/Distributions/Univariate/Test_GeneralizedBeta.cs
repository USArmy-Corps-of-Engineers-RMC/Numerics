using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Distributions;

namespace Distributions.Univariate
{
    [TestClass]
    public class Test_GeneralizedBeta
    {
        /// <summary>
        /// Verified using Accord.Net
        /// </summary>
        [TestMethod]
        public void Test_GenBeta()
        {

            double alpha = 0.42;
            double beta = 1.57;
            var B = new GeneralizedBeta(alpha, beta);

            double true_mean = 0.21105527638190955d;
            double true_median = 0.11577706212908731d;
            double true_mode = 57.999999999999957d;
            double true_var = 0.055689279830523512d;
            double true_pdf = 0.94644031936694828d;
            double true_cdf = 0.69358638272337991d;
            double true_icdf = 0.27d;
            
            Assert.AreEqual(B.Mean, true_mean, 0.0001d);
            Assert.AreEqual(B.Median, true_median, 0.0001d);
            Assert.AreEqual(B.Mode, true_mode, 0.0001d);
            Assert.AreEqual(B.Variance, true_var, 0.0001d);
            Assert.AreEqual(B.PDF(0.27d), true_pdf, 0.0001d);
            Assert.AreEqual(B.CDF(0.27d), true_cdf, 0.0001d);
            Assert.AreEqual(B.InverseCDF(B.CDF(0.27d)), true_icdf, 0.0001d);


        }

        /// <summary>
        /// Verified using R 'mc2d'
        /// </summary>
        [TestMethod]
        public void Test_GenBeta_R()
        {
            var x = new double[] { 0.1, 0.25, 0.5, 0.75, 0.9 };
            var p = new double[5];
            var true_p = new double[] { 0.271000, 0.578125, 0.875000, 0.984375, 0.999000 };
            var B = new GeneralizedBeta(1, 3);
            for (int i = 0; i < 5; i++)
            {
                p[i] = B.CDF(x[i]);
                Assert.AreEqual(true_p[i], p[i], 1E-7);
            }
            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(x[i], B.InverseCDF(p[i]), 1E-7);
            }
        }
    }
}
