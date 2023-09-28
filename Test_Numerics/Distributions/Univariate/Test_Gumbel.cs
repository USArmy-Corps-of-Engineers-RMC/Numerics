using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Distributions;

namespace Distributions.Univariate
{
    [TestClass]
    public class Test_Gumbel
    {

        // Reference: "Flood Frequency Analysis", A.R. Rao & K.H. Hamed, CRC Press, 2000.
        // Table 7.2.1 Sugar Creek at Crawfordsville, IN
        private double[] sample = new double[] { 17600d, 3660d, 903d, 5050d, 24000d, 11400d, 9470d, 8970d, 7710d, 14800d, 13900d, 20800d, 9470d, 7860d, 7860d, 2730d, 6480d, 18200d, 26300d, 15100d, 14600d, 7300d, 8580d, 15100d, 15100d, 21800d, 6200d, 2130d, 11100d, 14300d, 11200d, 6670d, 5440d, 9370d, 6900d, 9680d, 6810d, 7730d, 5290d, 12200d, 9750d, 7390d, 13100d, 7190d, 8850d, 6290d, 18800d, 9740d, 2990d, 6950d, 9390d, 12400d, 21200d };

        /// <summary>
        /// Verification of Gumbel Distribution fit with method of moments.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Reference: "Flood Frequency Analysis", A.R. Rao & K.H. Hamed, CRC Press, 2000.
        /// </para>
        /// <para>
        /// Example 7.2.1 page 234.
        /// </para>
        /// </remarks>
        [TestMethod()]
        public void Test_GUM_MOM_Fit()
        {
            var GUM = new Gumbel();
            GUM.Estimate(sample, ParameterEstimationMethod.MethodOfMoments);
            double x = GUM.Xi;
            double a = GUM.Alpha;
            double true_x = 8074.4d;
            double true_a = 4441.4d;
            Assert.AreEqual((x - true_x) / true_x < 0.01d, true);
            Assert.AreEqual((a - true_a) / true_a < 0.01d, true);
        }

        [TestMethod()]
        public void Test_GUM_LMOM_Fit()
        {
            var sample = new double[] { 1953d, 1939d, 1677d, 1692d, 2051d, 2371d, 2022d, 1521d, 1448d, 1825d, 1363d, 1760d, 1672d, 1603d, 1244d, 1521d, 1783d, 1560d, 1357d, 1673d, 1625d, 1425d, 1688d, 1577d, 1736d, 1640d, 1584d, 1293d, 1277d, 1742d, 1491d };
            var GUM = new Gumbel();
            GUM.Estimate(sample, ParameterEstimationMethod.MethodOfLinearMoments);
            double x = GUM.Xi;
            double a = GUM.Alpha;
            double true_x = 1533.69d;
            double true_a = 199.4332d;
            Assert.AreEqual(x, true_x, 0.001d);
            Assert.AreEqual(a, true_a, 0.001d);
            var lmom = GUM.LinearMomentsFromParameters(GUM.GetParameters);
            Assert.AreEqual(lmom[0], 1648.806d, 0.001d);
            Assert.AreEqual(lmom[1], 138.2366d, 0.001d);
            Assert.AreEqual(lmom[2], 0.169925d, 0.001d);
            Assert.AreEqual(lmom[3], 0.150375d, 0.001d);
        }

        /// <summary>
        /// Verification of Gumbel Distribution fit with method of maximum likelihood.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Reference: "Flood Frequency Analysis", A.R. Rao & K.H. Hamed, CRC Press, 2000.
        /// </para>
        /// <para>
        /// Example 7.2.1 page 234.
        /// </para>
        /// </remarks>
        [TestMethod()]
        public void Test_GUM_MLE_Fit()
        {
            var GUM = new Gumbel();
            GUM.Estimate(sample, ParameterEstimationMethod.MaximumLikelihood);
            double x = GUM.Xi;
            double a = GUM.Alpha;
            double true_x = 8049.6d;
            double true_a = 4478.6d;
            Assert.AreEqual((x - true_x) / true_x < 0.01d, true);
            Assert.AreEqual((a - true_a) / true_a < 0.01d, true);
        }

        /// <summary>
        /// Test the quantile function for the Gumbel Distribution.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Reference: "Flood Frequency Analysis", A.R. Rao & K.H. Hamed, CRC Press, 2000.
        /// </para>
        /// <para>
        /// Example 7.7.2 page 237.
        /// </para>
        /// </remarks>
        [TestMethod()]
        public void Test_Gumbel_Quantile()
        {
            var GUM = new Gumbel(8049.6d, 4478.6d);
            double q100 = GUM.InverseCDF(0.99d);
            double true_q100 = 28652d;
            Assert.AreEqual((q100 - true_q100) / true_q100 < 0.01d, true);
            double p = GUM.CDF(q100);
            double true_p = 0.99d;
            Assert.AreEqual((p - true_p) / true_p < 0.01d, true);

        }

        /// <summary>
        /// Test the standard error for the Gumbel Distribution.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Reference: "Flood Frequency Analysis", A.R. Rao & K.H. Hamed, CRC Press, 2000.
        /// </para>
        /// <para>
        /// Example 7.2.3 page 240.
        /// </para>
        /// </remarks>
        [TestMethod()]
        public void Test_Gumbel_StandardError()
        {

            // Maximum Likelihood
            var GUM = new Gumbel(8049.6d, 4478.6d);
            double qVar99 = Math.Sqrt(GUM.QuantileVariance(0.99d, 53, ParameterEstimationMethod.MaximumLikelihood));
            double true_qVar99 = 2486.5d;
            Assert.AreEqual((qVar99 - true_qVar99) / true_qVar99 < 0.01d, true);
        }
    }
}
