using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Distributions;

namespace Distributions.Univariate
{
    [TestClass]
    public class Test_GeneralizedPareto
    {

        // Reference: "Flood Frequency Analysis", A.R. Rao & K.H. Hamed, CRC Press, 2000.
        // Table 8.3.1 White River at Mt. Carmel (Threshold = 50,000 cfs)
        private double[] sample = new double[] { 126000d, 148000d, 66000d, 156000d, 136000d, 122000d, 183000d, 162000d, 85200d, 56800d, 56600d, 138000d, 81000d, 51800d, 90700d, 139000d, 160000d, 118000d, 50600d, 137000d, 151000d, 172000d, 52600d, 248000d, 152000d, 64500d, 61500d, 143000d, 108000d, 53000d, 134000d, 115000d, 84100d, 105000d, 85400d, 76900d, 99100d, 73700d, 122000d, 62500d, 54300d, 58000d, 144000d, 55800d, 127000d, 55800d, 107000d, 56400d, 128000d, 106000d, 110000d, 232000d, 60400d, 60400d, 50800d, 100000d, 55900d, 167000d, 53700d, 56700d, 126000d, 100000d, 59500d, 164000d, 81800d, 56400d, 124000d, 64600d, 77300d, 65900d, 72500d, 65100d, 80800d, 69800d, 53000d, 195000d, 128000d, 114000d, 110000d, 149000d, 74100d, 75900d, 99300d, 168000d, 70800d, 104000d, 125000d, 77300d, 97300d, 140000d, 54900d, 66000d, 199000d, 99800d, 105000d, 93700d, 277000d, 85700d, 77300d, 122000d, 106000d, 93300d, 79000d, 130000d, 126000d, 57800d, 64700d, 162000d, 71900d, 63500d, 81500d, 51000d, 84400d, 108000d, 185000d, 55800d, 94600d, 82800d, 146000d, 66500d, 57700d, 78700d, 85100d, 129000d, 75700d, 104000d, 139000d, 50600d, 53500d, 178000d, 110000d, 50800d, 76000d, 130000d, 67300d, 149000d, 78400d, 96600d, 83300d, 68400d, 84300d, 56400d, 112000d, 76400d, 116000d, 51400d, 59800d, 63900d, 81900d, 88200d, 62300d, 162000d, 67200d, 85500d, 51000d, 286000d, 73800d, 61300d, 60800d, 91300d, 134000d, 106000d, 70800d, 106000d, 122000d, 149000d, 53700d, 85300d, 144000d, 54800d, 116000d, 67500d, 56500d, 86700d, 91500d, 105000d, 134000d, 97300d, 84000d, 141000d, 52600d, 124000d, 196000d, 84200d, 54500d, 74500d, 104000d, 57200d, 61000d, 155000d, 96500d, 89100d, 77900d, 70500d, 73400d, 180000d, 83700d, 302000d, 133000d, 92100d, 105000d, 235000d, 213000d, 96100d, 77100d, 73900d, 55400d, 55200d, 87800d, 52600d, 106000d, 93000d, 147000d, 61800d, 101000d, 154000d, 52000d, 121000d, 86700d, 57300d, 97500d, 112000d, 88500d, 76200d, 140000d, 87400d, 154000d, 95100d, 131000d, 131000d, 54900d, 78800d, 101000d, 224000d, 54800d, 50900d, 63500d, 63500d, 152000d, 51000d, 285000d, 114000d, 197000d, 106000d, 132000d, 83700d, 67200d, 110000d, 202000d, 127000d, 90600d, 126000d, 73900d, 86500d, 181000d, 141000d, 79700d, 97800d, 57300d, 77200d, 133000d, 82900d, 55000d, 62000d, 51700d, 54500d, 51600d, 103000d, 134000d, 71700d, 57000d, 63900d, 60700d, 81900d, 171000d, 111000d, 50400d, 50500d, 69700d, 88900d, 76600d };

        /// <summary>
        /// Verification of GPA Distribution fit with method of moments.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Reference: "Flood Frequency Analysis", A.R. Rao & K.H. Hamed, CRC Press, 2000.
        /// </para>
        /// <para>
        /// Example 8.3.1 page 279.
        /// </para>
        /// </remarks>
        [TestMethod()]
        public void Test_GPA_MOM_Fit()
        {
            var GPA = new GeneralizedPareto();
            GPA.Estimate(sample, ParameterEstimationMethod.MethodOfMoments);
            double x = GPA.Xi;
            double a = GPA.Alpha;
            double k = GPA.Kappa;
            double true_x = 50169.23d;
            double true_a = 55443d;
            double true_k = 0.0956d;
            Assert.AreEqual((x - true_x) / true_x < 0.01d, true);
            Assert.AreEqual((a - true_a) / true_a < 0.01d, true);
            Assert.AreEqual((k - true_k) / true_k < 0.01d, true);
        }

        [TestMethod()]
        public void Test_GPA_LMOM_Fit()
        {
            var sample = new double[] { 1953d, 1939d, 1677d, 1692d, 2051d, 2371d, 2022d, 1521d, 1448d, 1825d, 1363d, 1760d, 1672d, 1603d, 1244d, 1521d, 1783d, 1560d, 1357d, 1673d, 1625d, 1425d, 1688d, 1577d, 1736d, 1640d, 1584d, 1293d, 1277d, 1742d, 1491d };
            var GPA = new GeneralizedPareto();
            GPA.Estimate(sample, ParameterEstimationMethod.MethodOfLinearMoments);
            double x = GPA.Xi;
            double a = GPA.Alpha;
            double k = GPA.Kappa;
            double true_x = 1285.909d;
            double true_a = 589.7772d;
            double true_k = 0.6251903d;
            Assert.AreEqual(x, true_x, 0.001d);
            Assert.AreEqual(a, true_a, 0.001d);
            Assert.AreEqual(k, true_k, 0.001d);
            var lmom = GPA.LinearMomentsFromParameters(GPA.GetParameters);
            Assert.AreEqual(lmom[0], 1648.806d, 0.001d);
            Assert.AreEqual(lmom[1], 138.2366d, 0.001d);
            Assert.AreEqual(lmom[2], 0.1033903d, 0.001d);
            Assert.AreEqual(lmom[3], 0.03073215d, 0.001d);
        }

        /// <summary>
        /// Verification of GPA Distribution fit with modified method of moments.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Reference: "Flood Frequency Analysis", A.R. Rao & K.H. Hamed, CRC Press, 2000.
        /// </para>
        /// <para>
        /// Example 8.3.1 page 279.
        /// </para>
        /// </remarks>
        [TestMethod()]
        public void Test_GPA_ModMOM_Fit()
        {
            var GPA = new GeneralizedPareto();
            GPA.SetParameters(GPA.ModifiedMethodOfMoments(sample));
            double x = GPA.Xi;
            double a = GPA.Alpha;
            double k = GPA.Kappa;
            double true_x = 50203.04d;
            double true_a = 55365.72d;
            double true_k = 0.0948d;
            Assert.AreEqual((x - true_x) / true_x < 0.01d, true);
            Assert.AreEqual((a - true_a) / true_a < 0.01d, true);
            Assert.AreEqual((k - true_k) / true_k < 0.01d, true);
        }

        /// <summary>
        /// Verification of Generalized Pareto Distribution fit with method of maximum likelihood.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Reference: "Flood Frequency Analysis", A.R. Rao & K.H. Hamed, CRC Press, 2000.
        /// </para>
        /// <para>
        /// Example 8.3.1 page 279.
        /// </para>
        /// </remarks>
        [TestMethod()]
        public void Test_GPA_MLE_Fit()
        {
            var GPA = new GeneralizedPareto();
            GPA.Estimate(sample, ParameterEstimationMethod.MaximumLikelihood);
            double x = GPA.Xi;
            double a = GPA.Alpha;
            double k = GPA.Kappa;
            double true_x = 50400d;
            double true_a = 55142.29d;
            double true_k = 0.0945d;
            Assert.AreEqual((x - true_x) / true_x < 0.01d, true);
            Assert.AreEqual((a - true_a) / true_a < 0.01d, true);
            Assert.AreEqual((k - true_k) / true_k < 0.01d, true);
        }

        /// <summary>
        /// Test the quantile function for the GPA Distribution.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Reference: "Flood Frequency Analysis", A.R. Rao & K.H. Hamed, CRC Press, 2000.
        /// </para>
        /// <para>
        /// Example 8.3.2 page 283.
        /// </para>
        /// </remarks>
        [TestMethod()]
        public void Test_GPA_Quantile()
        {
            var GPA = new GeneralizedPareto(50203.04d, 55365.72d, 0.0948d);
            double q100 = GPA.InverseCDF(0.99d);
            double true_q100 = 256803d;
            Assert.AreEqual((q100 - true_q100) / true_q100 < 0.01d, true);
            double p = GPA.CDF(q100);
            double true_p = 0.99d;
            Assert.AreEqual((p - true_p) / true_p < 0.01d, true);
        }

        /// <summary>
        /// Test the partial derivatives for the Generalized Pareto Distribution.
        /// </summary>
        [TestMethod()]
        public void Test_GPA_Partials()
        {
            var GPA = new GeneralizedPareto(50203.04d, 55365.72d, 0.0948d);
            double dQdLocation = GPA.PartialDerivatives(0.99d)[0];
            double dQdScale = GPA.PartialDerivatives(0.99d)[1];
            double dQdShape = GPA.PartialDerivatives(0.99d)[2];
            double true_dLocation = 1.0d;
            double true_dScale = 3.7315488d;
            double true_dShape = -441209.53d;
            Assert.AreEqual((dQdLocation - true_dLocation) / true_dLocation < 0.01d, true);
            Assert.AreEqual((dQdScale - true_dScale) / true_dScale < 0.01d, true);
            Assert.AreEqual((dQdShape - true_dShape) / true_dShape < 0.01d, true);
        }

        /// <summary>
        /// Test the standard error for the Generalized Pareto Distribution.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Reference: "Flood Frequency Analysis", A.R. Rao & K.H. Hamed, CRC Press, 2000.
        /// </para>
        /// <para>
        /// Example 8.3.3 page 286.
        /// </para>
        /// </remarks>
        [TestMethod()]
        public void Test_GPA_StandardError()
        {

            // Method of Moments
            var GPA = new GeneralizedPareto(50203.04d, 55365.72d, 0.0948d);
            double qVar99 = Math.Sqrt(GPA.QuantileVariance(0.99d, sample.Length, ParameterEstimationMethod.MethodOfMoments));
            double true_qVar99 = 16657d;
            Assert.AreEqual((qVar99 - true_qVar99) / true_qVar99 < 0.01d, true);

            // Maximum Likelihood
            GPA = new GeneralizedPareto(50400d, 55142.29d, 0.0945d);
            qVar99 = Math.Sqrt(GPA.QuantileVariance(0.99d, sample.Length, ParameterEstimationMethod.MaximumLikelihood));
            true_qVar99 = 15938d;
            Assert.AreEqual((qVar99 - true_qVar99) / true_qVar99 < 0.01d, true);
        }
    }
}
