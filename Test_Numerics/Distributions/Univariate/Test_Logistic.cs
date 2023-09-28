using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Distributions;

namespace Distributions.Univariate
{
    [TestClass]
    public class Test_Logistic
    {

        // Reference: "Flood Frequency Analysis", A.R. Rao & K.H. Hamed, CRC Press, 2000.
        // Table 5.1.1 Tippecanoe River Near Delphi, Indiana (Station 43) Data
        private double[] sample = new double[] { 6290d, 2700d, 13100d, 16900d, 14600d, 9600d, 7740d, 8490d, 8130d, 12000d, 17200d, 15000d, 12400d, 6960d, 6500d, 5840d, 10400d, 18800d, 21400d, 22600d, 14200d, 11000d, 12800d, 15700d, 4740d, 6950d, 11800d, 12100d, 20600d, 14600d, 14600d, 8900d, 10600d, 14200d, 14100d, 14100d, 12500d, 7530d, 13400d, 17600d, 13400d, 19200d, 16900d, 15500d, 14500d, 21900d, 10400d, 7460d };

        /// <summary>
        /// Verification of Logistic Distribution fit with method of moments.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Reference: "Flood Frequency Analysis", A.R. Rao & K.H. Hamed, CRC Press, 2000.
        /// </para>
        /// <para>
        /// Example 9.1.1 page 295.
        /// </para>
        /// </remarks>
        [TestMethod()]
        public void Test_Logistic_MOM_Fit()
        {
            var LO = new Logistic();
            LO.Estimate(sample, ParameterEstimationMethod.MethodOfMoments);
            double x = LO.Xi;
            double a = LO.Alpha;
            double true_x = 12665d;
            double true_a = 2596.62d;
            Assert.AreEqual((x - true_x) / true_x < 0.01d, true);
            Assert.AreEqual((a - true_a) / true_a < 0.01d, true);
        }

        /// <summary>
        /// Verification of Logistic Distribution fit with method of maximum likelihood.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Reference: "Flood Frequency Analysis", A.R. Rao & K.H. Hamed, CRC Press, 2000.
        /// </para>
        /// <para>
        /// Example 9.1.1 page 295.
        /// </para>
        /// </remarks>
        [TestMethod()]
        public void Test_Logistic_MLE_Fit()
        {
            var LO = new Logistic();
            LO.Estimate(sample, ParameterEstimationMethod.MaximumLikelihood);
            double x = LO.Xi;
            double a = LO.Alpha;
            double true_x = 12628.59d;
            double true_a = 2708.64d;
            Assert.AreEqual((x - true_x) / true_x < 0.01d, true);
            Assert.AreEqual((a - true_a) / true_a < 0.01d, true);
        }

        /// <summary>
        /// Test the quantile function for the Logistic Distribution.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Reference: "Flood Frequency Analysis", A.R. Rao & K.H. Hamed, CRC Press, 2000.
        /// </para>
        /// <para>
        /// Example 9.1.2 page 297.
        /// </para>
        /// </remarks>
        [TestMethod()]
        public void Test_Logistic_Quantile()
        {
            var LO = new Logistic(12665d, 2596.62d);
            double q100 = LO.InverseCDF(0.99d);
            double true_100 = 24597d;
            Assert.AreEqual((q100 - true_100) / true_100 < 0.01d, true);
            double p = LO.CDF(q100);
            double true_p = 0.99d;
            Assert.AreEqual(p == true_p, true);
        }

        /// <summary>
        /// Test the standard error for the Logistic Distribution.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Reference: "Flood Frequency Analysis", A.R. Rao & K.H. Hamed, CRC Press, 2000.
        /// </para>
        /// <para>
        /// Example 9.1.3 page 300.
        /// </para>
        /// </remarks>
        [TestMethod()]
        public void Test_Logistic_StandardError()
        {

            // Method of Moments
            var LO = new Logistic(12665d, 2596.62d);
            double se100 = Math.Sqrt(LO.QuantileVariance(0.99d, 48, ParameterEstimationMethod.MethodOfMoments));
            double true_se100 = 1684d;
            Assert.AreEqual((se100 - true_se100) / true_se100 < 0.01d, true);

            // Maximum Likelihood
            LO = new Logistic(12628.59d, 2708.64d);
            se100 = Math.Sqrt(LO.QuantileVariance(0.99d, 48, ParameterEstimationMethod.MaximumLikelihood));
            true_se100 = 1648d;
            Assert.AreEqual((se100 - true_se100) / true_se100 < 0.01d, true);
        }
    }
}
