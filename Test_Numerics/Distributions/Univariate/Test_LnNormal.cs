using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Distributions;

namespace Distributions.Univariate
{
    [TestClass]
    public class Test_LnNormal
    {

        // Reference: "Flood Frequency Analysis", A.R. Rao & K.H. Hamed, CRC Press, 2000.
        // Table 1.8.1 Wabash River at Lafayette, IN
        private double[] sample = new double[] { 41500d, 57000d, 44000d, 49000d, 31000d, 45900d, 19000d, 41100d, 37300d, 76000d, 33200d, 61200d, 76000d, 59800d, 44400d, 58400d, 53600d, 59800d, 63300d, 57700d, 64000d, 63500d, 38000d, 74600d, 13100d, 37600d, 67500d, 21700d, 37000d, 93500d, 58500d, 63300d, 74400d, 34200d, 14600d, 44200d, 13100d, 73300d, 46600d, 39400d, 41200d, 41300d, 62000d, 90000d, 50600d, 41900d, 35000d, 16500d, 35300d, 30000d, 52600d, 99000d, 89000d, 39500d, 55400d, 46000d, 63000d, 58300d, 36500d, 14600d, 64900d, 68500d, 69100d, 42600d, 31000d, 39400d, 40700d, 53400d, 36000d, 43900d, 23600d, 50500d, 49700d, 48100d, 44500d, 56400d, 60800d, 40400d, 80400d, 41600d, 14700d, 33300d, 40700d, 53300d, 77400d };

        /// <summary>
        /// Verification of Ln-Normal Distribution fit with method of moments.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Reference: "Flood Frequency Analysis", A.R. Rao & K.H. Hamed, CRC Press, 2000.
        /// </para>
        /// <para>
        /// Example 5.2.1 page 99.
        /// </para>
        /// </remarks>
        [TestMethod()]
        public void Test_LnNormal_MOM_Fit()
        {

            var LN = new LnNormal();
            LN.SetParameters(10, -10);
            LN.Estimate(sample, ParameterEstimationMethod.MethodOfMoments);
            double u1 = LN.Mu;
            double u2 = LN.Sigma;
            double true_u1 = 10.7676d;
            double true_u2 = 0.4544d;
            Assert.AreEqual((u1 - true_u1) / true_u1 < 0.01d, true);
            Assert.AreEqual((u2 - true_u2) / true_u2 < 0.01d, true);
        }

        [TestMethod()]
        public void Test_LnNormal_LMOM_Fit()
        {
            // Air quality - wind data from R
            var data = new double[] { 7.4, 8, 12.6, 11.5, 14.3, 14.9, 8.6, 13.8, 20.1, 8.6, 6.9, 9.7, 9.2, 10.9, 13.2, 11.5, 12, 18.4, 11.5, 9.7, 9.7, 16.6, 9.7, 12, 16.6, 14.9, 8, 12, 14.9, 5.7, 7.4, 8.6, 9.7, 16.1, 9.2, 8.6, 14.3, 9.7, 6.9, 13.8, 11.5, 10.9, 9.2, 8, 13.8, 11.5, 14.9, 20.7, 9.2, 11.5, 10.3, 6.3, 1.7, 4.6, 6.3, 8, 8, 10.3, 11.5, 14.9, 8, 4.1, 9.2, 9.2, 10.9, 4.6, 10.9, 5.1, 6.3, 5.7, 7.4, 8.6, 14.3, 14.9, 14.9, 14.3, 6.9, 10.3, 6.3, 5.1, 11.5, 6.9, 9.7, 11.5, 8.6, 8, 8.6, 12, 7.4, 7.4, 7.4, 9.2, 6.9, 13.8, 7.4, 6.9, 7.4, 4.6, 4, 10.3, 8, 8.6, 11.5, 11.5, 11.5, 9.7, 11.5, 10.3, 6.3, 7.4, 10.9, 10.3, 15.5, 14.3, 12.6, 9.7, 3.4, 8, 5.7, 9.7, 2.3, 6.3, 6.3, 6.9, 5.1, 2.8, 4.6, 7.4, 15.5, 10.9, 10.3, 10.9, 9.7, 14.9, 15.5, 6.3, 10.9, 11.5, 6.9, 13.8, 10.3, 10.3, 8, 12.6, 9.2, 10.3, 10.3, 16.6, 6.9, 13.2, 14.3, 8, 11.5 };
            var norm = new LnNormal();
            norm.Estimate(data, ParameterEstimationMethod.MethodOfLinearMoments);
            double u1 = norm.Mu;
            double u2 = norm.Sigma;
            double true_u1 = 0.9672391d;
            double true_u2 = 0.1675344d;
            //Assert.AreEqual(u1, true_u1, 0.0001d);
            //Assert.AreEqual(u2, true_u2, 0.0001d);
            //var lmom = norm.LinearMomentsFromParameters(norm.GetParameters);
            //Assert.AreEqual(lmom[0], 0.96723909d, 0.0001d);
            //Assert.AreEqual(lmom[1], 0.09452119d, 0.0001d);
            //Assert.AreEqual(lmom[2], 0.00000000d, 0.0001d);
            //Assert.AreEqual(lmom[3], 0.12260172d, 0.0001d);
        }

        /// <summary>
        /// Verification of Ln-Normal Distribution fit with method of maximum likelihood.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Reference: "Flood Frequency Analysis", A.R. Rao & K.H. Hamed, CRC Press, 2000.
        /// </para>
        /// <para>
        /// Example 5.2.1 page 99.
        /// </para>
        /// </remarks>
        [TestMethod()]
        public void Test_LnNormal_MLE_Fit()
        {
            var LN = new LnNormal();
            LN.Estimate(sample, ParameterEstimationMethod.MaximumLikelihood);
            double u1 = LN.Mu;
            double u2 = LN.Sigma;
            double true_u1 = 10.7711d;
            double true_u2 = 0.4562d;
            Assert.AreEqual((u1 - true_u1) / true_u1 < 0.01d, true);
            Assert.AreEqual((u2 - true_u2) / true_u2 < 0.01d, true);
        }

        /// <summary>
        /// Test the quantile function for the Ln-Normal Distribution.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Reference: "Flood Frequency Analysis", A.R. Rao & K.H. Hamed, CRC Press, 2000.
        /// </para>
        /// <para>
        /// Example 5.2.2 page 102.
        /// </para>
        /// </remarks>
        [TestMethod()]
        public void Test_LnNormal_Quantile()
        {
            var LN = new LnNormal() { Mu = 10.7676d, Sigma = 0.4544d };
            double q100 = LN.InverseCDF(0.99d);
            double true_q100 = 136611d;
            Assert.AreEqual((q100 - true_q100) / true_q100 < 0.01d, true);
            double p = LN.CDF(q100);
            double true_p = 0.99d;
            Assert.AreEqual((p - true_p) / true_p < 0.01d, true);
        }

        /// <summary>
        /// Test the standard error for the Ln-Normal Distribution.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Reference: "Flood Frequency Analysis", A.R. Rao & K.H. Hamed, CRC Press, 2000.
        /// </para>
        /// <para>
        /// Example 5.1.3 page 94.
        /// </para>
        /// </remarks>
        [TestMethod()]
        public void Test_LnNormal_StandardError()
        {
            // Maximum Likelihood
            var LN = new LnNormal() { Mu = 10.7711d, Sigma = 0.4562d };
            LN.Estimate(sample, ParameterEstimationMethod.MaximumLikelihood);
            double qVar99 = Math.Sqrt(LN.QuantileVariance(0.99d, 85, ParameterEstimationMethod.MaximumLikelihood));
            double true_qVar99 = 13113d;
            Assert.AreEqual((qVar99 - true_qVar99) / true_qVar99 < 0.01d, true);
        }


        [TestMethod()]
        public void Test_ConditionalExpectation()
        {
            double mu = 10d;
            double sigma = 5d;
            double alpha = 0.999;
            var LN = new LnNormal(mu, sigma);

            double CE = LN.ConditionalExpectedValue(alpha);
            double true_CE = Math.Exp(LN.Mu + 0.5 * LN.Sigma * LN.Sigma) / (1d - alpha) * (1 - Normal.StandardCDF(Normal.StandardZ(alpha) - LN.Sigma));
            Assert.AreEqual(CE, true_CE, 1E-4);

        }

        [TestMethod()]
        public void Test_CentralMoments()
        {

            var LN = new LnNormal(10, 5);
            var mom = LN.CentralMoments(1E-8);
            double true_mean = LN.Mean;
            double true_stDev = LN.StandardDeviation;
            double true_skew = LN.Skew;
            double true_kurt = LN.Kurtosis;

            Assert.AreEqual(mom[0], true_mean, 1E-4);
            Assert.AreEqual(mom[1], true_stDev, 1E-4);
            Assert.AreEqual(mom[2], true_skew, 1E-4);
            Assert.AreEqual(mom[3], true_kurt, 1E-4);

        }

    }
}
