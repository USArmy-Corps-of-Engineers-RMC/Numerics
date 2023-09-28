using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Distributions;

namespace Distributions.Univariate
{
    [TestClass]
    public class Test_Exponential
    {
        // Reference: "Flood Frequency Analysis", A.R. Rao & K.H. Hamed, CRC Press, 2000.
        // Table 1.8.1 Wabash River at Lafayette, IN
        private double[] sample = new double[] { 41500d, 57000d, 44000d, 49000d, 31000d, 45900d, 19000d, 41100d, 37300d, 76000d, 33200d, 61200d, 76000d, 59800d, 44400d, 58400d, 53600d, 59800d, 63300d, 57700d, 64000d, 63500d, 38000d, 74600d, 13100d, 37600d, 67500d, 21700d, 37000d, 93500d, 58500d, 63300d, 74400d, 34200d, 14600d, 44200d, 13100d, 73300d, 46600d, 39400d, 41200d, 41300d, 62000d, 90000d, 50600d, 41900d, 35000d, 16500d, 35300d, 30000d, 52600d, 99000d, 89000d, 39500d, 55400d, 46000d, 63000d, 58300d, 36500d, 14600d, 64900d, 68500d, 69100d, 42600d, 31000d, 39400d, 40700d, 53400d, 36000d, 43900d, 23600d, 50500d, 49700d, 48100d, 44500d, 56400d, 60800d, 40400d, 80400d, 41600d, 14700d, 33300d, 40700d, 53300d, 77400d };

        /// <summary>
        /// Verification of Exponential Distribution fit with method of moments.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Reference: "Flood Frequency Analysis", A.R. Rao & K.H. Hamed, CRC Press, 2000.
        /// </para>
        /// <para>
        /// Example 6.1.1 page 132.
        /// </para>
        /// </remarks>
        [TestMethod()]
        public void Test_EXP_MOM_Fit()
        {
            var EXP = new Exponential();
            EXP.Estimate(sample, ParameterEstimationMethod.MethodOfMoments);
            double x = EXP.Xi;
            double a = EXP.Alpha;
            double true_x = 30314.48d;
            double true_a = 18907.87d;
            Assert.AreEqual((x - true_x) / true_x < 0.01d, true);
            Assert.AreEqual((a - true_a) / true_a < 0.01d, true);
        }

        [TestMethod()]
        public void Test_EXP_LMOM_Fit()
        {
            var sample = new double[] { 1953d, 1939d, 1677d, 1692d, 2051d, 2371d, 2022d, 1521d, 1448d, 1825d, 1363d, 1760d, 1672d, 1603d, 1244d, 1521d, 1783d, 1560d, 1357d, 1673d, 1625d, 1425d, 1688d, 1577d, 1736d, 1640d, 1584d, 1293d, 1277d, 1742d, 1491d };
            var EXP = new Exponential();
            EXP.Estimate(sample, ParameterEstimationMethod.MethodOfLinearMoments);
            double x = EXP.Xi;
            double a = EXP.Alpha;
            double true_x = 1372.333d;
            double true_a = 276.4731d;
            Assert.AreEqual(x, true_x, 0.001d);
            Assert.AreEqual(a, true_a, 0.001d);
            var lmom = EXP.LinearMomentsFromParameters(EXP.GetParameters);
            Assert.AreEqual(lmom[0], 1648.806d, 0.001d);
            Assert.AreEqual(lmom[1], 138.2366d, 0.001d);
            Assert.AreEqual(lmom[2], 0.3333333d, 0.001d);
            Assert.AreEqual(lmom[3], 0.1666667d, 0.001d);
        }

        /// <summary>
        /// Verification of Exponential Distribution fit with method of maximum likelihood.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Reference: "Flood Frequency Analysis", A.R. Rao & K.H. Hamed, CRC Press, 2000.
        /// </para>
        /// <para>
        /// Example 6.1.1 page 132.
        /// </para>
        /// </remarks>
        [TestMethod()]
        public void Test_EXP_MLE_Fit()
        {
            var EXP = new Exponential();
            EXP.Estimate(sample, ParameterEstimationMethod.MaximumLikelihood);
            double x = EXP.Xi;
            double a = EXP.Alpha;
            double true_x = 13100d;
            double true_a = 36122.35d;
            Assert.AreEqual((x - true_x) / true_x < 0.01d, true);
            Assert.AreEqual((a - true_a) / true_a < 0.01d, true);
        }

        /// <summary>
        /// Test the quantile function for the Exponential Distribution.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Reference: "Flood Frequency Analysis", A.R. Rao & K.H. Hamed, CRC Press, 2000.
        /// </para>
        /// <para>
        /// Example 6.1.2 page 134.
        /// </para>
        /// </remarks>
        [TestMethod()]
        public void Test_EXP_Quantile()
        {
            var EXP = new Exponential(27421d, 25200d);
            double q100 = EXP.InverseCDF(0.99d);
            double true_100 = 143471d;
            Assert.AreEqual((q100 - true_100) / true_100 < 0.01d, true);
            double p = EXP.CDF(q100);
            double true_p = 0.99d;
            Assert.AreEqual(p == true_p, true);
        }

        /// <summary>
        /// Test the standard error for the Exponential Distribution.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Reference: "Flood Frequency Analysis", A.R. Rao & K.H. Hamed, CRC Press, 2000.
        /// </para>
        /// <para>
        /// Example 6.1.3 page 138.
        /// </para>
        /// </remarks>
        [TestMethod()]
        public void Test_EXP_StandardError()
        {

            // Method of Moments
            var EXP = new Exponential(27421d, 25200d);
            double se100 = Math.Sqrt(EXP.QuantileVariance(0.99d, 85, ParameterEstimationMethod.MethodOfMoments));
            double true_se100 = 15986d;
            Assert.AreEqual((se100 - true_se100) / true_se100 < 0.01d, true);

            // Maximum Likelihood
            EXP = new Exponential(12629d, 39991d);
            se100 = Math.Sqrt(EXP.QuantileVariance(0.99d, 85, ParameterEstimationMethod.MaximumLikelihood));
            true_se100 = 20048d;
            Assert.AreEqual((se100 - true_se100) / true_se100 < 0.01d, true);
        }

        /// <summary>
        /// Test the partial derivatives for the Exponential Distribution.
        /// </summary>
        [TestMethod()]
        public void Test_EXP_Partials()
        {
            var EXP = new Exponential(30314.48d, 18907.87d);
            double dQdLocation = EXP.PartialDerivatives(0.99d)[0];
            double dQdScale = EXP.PartialDerivatives(0.99d)[1];
            double true_dLocation = 1.0d;
            double true_dScale = 4.60517d;
            Assert.AreEqual((dQdLocation - true_dLocation) / true_dLocation < 0.01d, true);
            Assert.AreEqual((dQdScale - true_dScale) / true_dScale < 0.01d, true);
        }
    }
}
