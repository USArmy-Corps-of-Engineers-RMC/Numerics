using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Distributions;

namespace Distributions.Univariate
{

    [TestClass]
    public class Test_GeneralizedExtremeValue
    {

        // Reference: "Flood Frequency Analysis", A.R. Rao & K.H. Hamed, CRC Press, 2000.
        // Table 7.1.2 White River near Nora, IN
        private double[] sample = new double[] { 23200d, 2950d, 10300d, 23200d, 4540d, 9960d, 10800d, 26900d, 23300d, 20400d, 8480d, 3150d, 9380d, 32400d, 20800d, 11100d, 7270d, 9600d, 14600d, 14300d, 22500d, 14700d, 12700d, 9740d, 3050d, 8830d, 12000d, 30400d, 27000d, 15200d, 8040d, 11700d, 20300d, 22700d, 30400d, 9180d, 4870d, 14700d, 12800d, 13700d, 7960d, 9830d, 12500d, 10700d, 13200d, 14700d, 14300d, 4050d, 14600d, 14400d, 19200d, 7160d, 12100d, 8650d, 10600d, 24500d, 14400d, 6300d, 9560d, 15800d, 14300d, 28700d };

        /// <summary>
        /// Verification of GEV Distribution fit with method of moments.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Reference: "Flood Frequency Analysis", A.R. Rao & K.H. Hamed, CRC Press, 2000.
        /// </para>
        /// <para>
        /// Example 7.1.1 page 218.
        /// </para>
        /// </remarks>
        [TestMethod()]
        public void Test_GEV_MOM_Fit()
        {
            var GEV = new GeneralizedExtremeValue();
            GEV.Estimate(sample, ParameterEstimationMethod.MethodOfMoments);
            double x = GEV.Xi;
            double a = GEV.Alpha;
            double k = GEV.Kappa;
            double true_x = 11012d;
            double true_a = 6209.4d;
            double true_k = 0.0736d;
            Assert.AreEqual((x - true_x) / true_x < 0.01d, true);
            Assert.AreEqual((a - true_a) / true_a < 0.01d, true);
            Assert.AreEqual((k - true_k) / true_k < 0.01d, true);
        }

        [TestMethod()]
        public void Test_GEV_LMOM_Fit()
        {
            var sample = new double[] { 1953d, 1939d, 1677d, 1692d, 2051d, 2371d, 2022d, 1521d, 1448d, 1825d, 1363d, 1760d, 1672d, 1603d, 1244d, 1521d, 1783d, 1560d, 1357d, 1673d, 1625d, 1425d, 1688d, 1577d, 1736d, 1640d, 1584d, 1293d, 1277d, 1742d, 1491d };
            var GEV = new GeneralizedExtremeValue();
            GEV.Estimate(sample, ParameterEstimationMethod.MethodOfLinearMoments);
            double x = GEV.Xi;
            double a = GEV.Alpha;
            double k = GEV.Kappa;
            double true_x = 1543.933d;
            double true_a = 218.1148d;
            double true_k = 0.1068473d;
            Assert.AreEqual(x, true_x, 0.001d);
            Assert.AreEqual(a, true_a, 0.001d);
            Assert.AreEqual(k, true_k, 0.001d);
            var lmom = GEV.LinearMomentsFromParameters(GEV.GetParameters);
            Assert.AreEqual(lmom[0], 1648.806d, 0.001d);
            Assert.AreEqual(lmom[1], 138.2366d, 0.001d);
            Assert.AreEqual(lmom[2], 0.1030703d, 0.001d);
            Assert.AreEqual(lmom[3], 0.1277244d, 0.001d);
        }

        /// <summary>
        /// Verification of GEV Distribution fit with method of maximum likelihood.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Reference: "Flood Frequency Analysis", A.R. Rao & K.H. Hamed, CRC Press, 2000.
        /// </para>
        /// <para>
        /// Example 7.1.1 page 219.
        /// </para>
        /// </remarks>
        [TestMethod()]
        public void Test_GEV_MLE_Fit()
        {
            var GEV = new GeneralizedExtremeValue();
            GEV.Estimate(sample, ParameterEstimationMethod.MaximumLikelihood);
            double x = GEV.Xi;
            double a = GEV.Alpha;
            double k = GEV.Kappa;
            double true_x = 10849d;
            double true_a = 5745.6d;
            double true_k = 0.005d;
            Assert.AreEqual((x - true_x) / true_x < 0.01d, true);
            Assert.AreEqual((a - true_a) / true_a < 0.01d, true);
            Assert.AreEqual((k - true_k) / true_k < 0.01d, true);
        }

        /// <summary>
        /// Test the quantile function for the GEV Distribution.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Reference: "Flood Frequency Analysis", A.R. Rao & K.H. Hamed, CRC Press, 2000.
        /// </para>
        /// <para>
        /// Example 7.1.2 page 221.
        /// </para>
        /// </remarks>
        [TestMethod()]
        public void Test_GEV_Quantile()
        {
            var GEV = new GeneralizedExtremeValue(10849d, 5745.6d, 0.005d);
            double q100 = GEV.InverseCDF(0.99d);
            double true_q100 = 36977d;
            Assert.AreEqual((q100 - true_q100) / true_q100 < 0.01d, true);
            double p = GEV.CDF(q100);
            double true_p = 0.99d;
            Assert.AreEqual((p - true_p) / true_p < 0.01d, true);


        }

        /// <summary>
        /// Test the standard error for the GEV Distribution.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Reference: "Flood Frequency Analysis", A.R. Rao & K.H. Hamed, CRC Press, 2000.
        /// </para>
        /// <para>
        /// Example 7.1.3 page 226.
        /// </para>
        /// </remarks>
        [TestMethod()]
        public void Test_GEV_StandardError()
        {
            double u = 10849d;
            double a = 5745.6d;
            double k = 0.005d;
            double true_dXdU = 1.0d;
            double true_dxdA = 4.5472d;
            double true_dxdK = -59861;
            double true_VarU = 664669d;
            double true_VarA = 346400d;
            double true_VarK = 0.007655d;
            double true_CovarUA = 176180d;
            double true_CovarUK = 23.977d;
            double true_CovarAK = 13.8574d;
            double true_QVar = 26445364d;
            double true_QSigma = 5142d;
            var GEV = new GeneralizedExtremeValue(u, a, k);
            var partials = GEV.PartialDerivatives(0.99d).ToArray();
            var vars = GEV.ParameterVariance(sample.Length, ParameterEstimationMethod.MaximumLikelihood).ToArray();
            var covars = GEV.ParameterCovariance(sample.Length, ParameterEstimationMethod.MaximumLikelihood).ToArray();
            double qVar = GEV.QuantileVariance(0.99d, sample.Length, ParameterEstimationMethod.MaximumLikelihood);
            double qSigma = Math.Sqrt(qVar);
            Assert.AreEqual((partials[0] - true_dXdU) / true_dXdU < 0.01d, true);
            Assert.AreEqual((partials[1] - true_dxdA) / true_dxdA < 0.01d, true);
            Assert.AreEqual((partials[2] - true_dxdK) / true_dxdK < 0.01d, true);
            Assert.AreEqual((vars[0] - true_VarU) / true_VarU < 0.01d, true);
            Assert.AreEqual((vars[1] - true_VarA) / true_VarA < 0.01d, true);
            Assert.AreEqual((vars[2] - true_VarK) / true_VarK < 0.01d, true);
            Assert.AreEqual((covars[0] - true_CovarUA) / true_CovarUA < 0.01d, true);
            Assert.AreEqual((covars[1] - true_CovarUK) / true_CovarUK < 0.01d, true);
            Assert.AreEqual((covars[2] - true_CovarAK) / true_CovarAK < 0.01d, true);
            Assert.AreEqual((qVar - true_QVar) / true_QVar < 0.01d, true);
            Assert.AreEqual((qSigma - true_QSigma) / true_QSigma < 0.01d, true);
        }

        [TestMethod()]
        public void Test_GEV_Jacobian()
        {
            var GEV = new GeneralizedExtremeValue(22299.7822d, 11080.8716d, -0.0378d);
            double J = GEV.Jacobian(new[] { 0.9d, 0.99d, 0.999d });
        }
    }
}
