/*
* NOTICE:
* The U.S. Army Corps of Engineers, Risk Management Center (USACE-RMC) makes no guarantees about
* the results, or appropriateness of outputs, obtained from Numerics.
*
* LIST OF CONDITIONS:
* Redistribution and use in source and binary forms, with or without modification, are permitted
* provided that the following conditions are met:
* ● Redistributions of source code must retain the above notice, this list of conditions, and the
* following disclaimer.
* ● Redistributions in binary form must reproduce the above notice, this list of conditions, and
* the following disclaimer in the documentation and/or other materials provided with the distribution.
* ● The names of the U.S. Government, the U.S. Army Corps of Engineers, the Institute for Water
* Resources, or the Risk Management Center may not be used to endorse or promote products derived
* from this software without specific prior written permission. Nor may the names of its contributors
* be used to endorse or promote products derived from this software without specific prior
* written permission.
*
* DISCLAIMER:
* THIS SOFTWARE IS PROVIDED BY THE U.S. ARMY CORPS OF ENGINEERS RISK MANAGEMENT CENTER
* (USACE-RMC) "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
* THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL USACE-RMC BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
* SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
* PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
* INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
* LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
* THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Distributions;

namespace Distributions.Univariate
{
    /// <summary>
    /// Testing the Log-Pearson Type III distribution algorithm.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     <b> Authors: </b>
    ///     <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    ///     <item> Tiki Gonzalez, USACE Risk Management Center, julian.t.gonzalez@usace.army.mil</item>
    ///     </list> 
    /// </para>
    /// <para>
    /// <b> References: </b>
    /// </para>
    /// <para>
    /// <see href = "https://github.com/mathnet/mathnet-numerics/blob/master/src/Numerics.Tests/DistributionTests" />
    /// </para>
    /// </remarks>
    [TestClass]
    public class Test_LogPearsonTypeIII
    {

        // Reference: "The Gamma Family and Derived Distributions Applied in Hydrology", B. Bobee & F. Ashkar, Water Resources Publications, 1991.
        // Table 1.2 Maximum annual peak discharge values in cms, observed at the Harricana River at Amos (Quebec, Canada)
        private double[] sample = new double[] { 122d, 244d, 214d, 173d, 229d, 156d, 212d, 263d, 146d, 183d, 161d, 205d, 135d, 331d, 225d, 174d, 98.8d, 149d, 238d, 262d, 132d, 235d, 216d, 240d, 230d, 192d, 195d, 172d, 173d, 172d, 153d, 142d, 317d, 161d, 201d, 204d, 194d, 164d, 183d, 161d, 167d, 179d, 185d, 117d, 192d, 337d, 125d, 166d, 99.1d, 202d, 230d, 158d, 262d, 154d, 164d, 182d, 164d, 183d, 171d, 250d, 184d, 205d, 237d, 177d, 239d, 187d, 180d, 173d, 174d };

        /// <summary>
        /// Verification of LPIII fit with indirect method of moments.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Reference: "The Gamma Family and Derived Distributions Applied in Hydrology", B. Bobee & F. Ashkar, Water Resources Publications, 1991.
        /// </para>
        /// <para>
        /// Example 7.4 page 93.
        /// </para>
        /// </remarks>
        [TestMethod()]
        public void Test_LP3_IndirectMOM()
        {
            var LP3 = new LogPearsonTypeIII();
            LP3.Estimate(sample, ParameterEstimationMethod.MethodOfMoments);
            double xi = LP3.Xi;
            double beta = LP3.Beta;
            double alpha = LP3.Alpha;
            double meanOfLog = LP3.Mu;
            double stDevOfLog = LP3.Sigma;
            double skewOfLog = LP3.Gamma;
            double mean = LP3.Mean;
            double stDev = LP3.StandardDeviation;
            double skew = LP3.Skewness;
            double true_xi = 7.53821d;
            double true_beta = 1d / -460.31089d;
            double true_alpha = 2425.57481d;
            double true_meanOfLog = 2.26878d;
            double true_stDevOfLog = 0.10699d;
            double true_skewOfLog = -0.04061d;
            double true_mean = 191.38768d;
            double true_stDev = 47.62977d;
            double true_skew = 0.71589d;
            Assert.AreEqual((xi - true_xi) / true_xi < 0.01d, true);
            Assert.AreEqual((beta - true_beta) / true_beta < 0.01d, true);
            Assert.AreEqual((alpha - true_alpha) / true_alpha < 0.01d, true);
            Assert.AreEqual((meanOfLog - true_meanOfLog) / true_meanOfLog < 0.01d, true);
            Assert.AreEqual((stDevOfLog - true_stDevOfLog) / true_stDevOfLog < 0.01d, true);
            Assert.AreEqual((skewOfLog - true_skewOfLog) / true_skewOfLog < 0.01d, true);
            Assert.AreEqual((mean - true_mean) / true_mean < 0.01d, true);
            Assert.AreEqual((stDev - true_stDev) / true_stDev < 0.01d, true);
            Assert.AreEqual((skew - true_skew) / true_skew < 0.01d, true);
        }

        [TestMethod()]
        public void Test_LP3_LMOM_Fit()
        {
            var sample = new double[] { 1953d, 1939d, 1677d, 1692d, 2051d, 2371d, 2022d, 1521d, 1448d, 1825d, 1363d, 1760d, 1672d, 1603d, 1244d, 1521d, 1783d, 1560d, 1357d, 1673d, 1625d, 1425d, 1688d, 1577d, 1736d, 1640d, 1584d, 1293d, 1277d, 1742d, 1491d };
            var LP3 = new LogPearsonTypeIII();
            LP3.Estimate(sample, ParameterEstimationMethod.MethodOfLinearMoments);
            double x = LP3.Xi;
            double a = LP3.Alpha;
            double b = LP3.Beta;
            double true_x = 863.4104d;
            double true_a = 10.02196d;
            double true_b = 78.36751d;

            //Assert.AreEqual(x, true_x, 0.001);
            //Assert.AreEqual(a, true_a, 0.001);
            //Assert.AreEqual(b, true_b, 0.001);

            // Dim lmom() As Double = LP3.LinearMomentsFromParameters(LP3.GetParameters())
            // Assert.AreEqual(lmom(0), 1648.806, 0.001)
            // Assert.AreEqual(lmom(1), 138.2366, 0.001)
            // Assert.AreEqual(lmom(2), 0.1033889, 0.001)
            // Assert.AreEqual(lmom(3), 0.1258521, 0.001)

        }

        /// <summary>
        /// Verification of LPIII fit with maximum likelihood.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Reference: "The Gamma Family and Derived Distributions Applied in Hydrology", B. Bobee & F. Ashkar, Water Resources Publications, 1991.
        /// </para>
        /// <para>
        /// Example 7.4 page 93.
        /// </para>
        /// </remarks>
        [TestMethod()]
        public void Test_LP3_MLE()
        {
            var LP3 = new LogPearsonTypeIII();
            LP3.Estimate(sample, ParameterEstimationMethod.MaximumLikelihood);
            double xi = LP3.Xi;
            double beta = LP3.Beta;
            double alpha = LP3.Alpha;
            double meanOfLog = LP3.Mu;
            double stDevOfLog = LP3.Sigma;
            double skewOfLog = LP3.Gamma;
            double mean = LP3.Mean;
            double stDev = LP3.StandardDeviation;
            double skew = LP3.Skewness;
            double true_xi = 9.53033d;
            double true_beta = 1d / -643.69408d;
            double true_alpha = 4674.2179d;
            double true_meanOfLog = 2.26878d;
            double true_stDevOfLog = 0.10621d;
            double true_skewOfLog = -0.02925d;
            double true_mean = 191.30891d;
            double true_stDev = 47.32124d;
            double true_skew = 0.72396d;
            Assert.AreEqual((xi - true_xi) / true_xi < 0.01d, true);
            Assert.AreEqual((beta - true_beta) / true_beta < 0.01d, true);
            Assert.AreEqual((alpha - true_alpha) / true_alpha < 0.01d, true);
            Assert.AreEqual((meanOfLog - true_meanOfLog) / true_meanOfLog < 0.01d, true);
            Assert.AreEqual((stDevOfLog - true_stDevOfLog) / true_stDevOfLog < 0.01d, true);
            Assert.AreEqual((skewOfLog - true_skewOfLog) / true_skewOfLog < 0.01d, true);
            Assert.AreEqual((mean - true_mean) / true_mean < 0.01d, true);
            Assert.AreEqual((stDev - true_stDev) / true_stDev < 0.01d, true);
            Assert.AreEqual((skew - true_skew) / true_skew < 0.01d, true);
        }

        /// <summary>
        /// Test the quantile function for the Log-Pearson Type III Distribution.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Reference: "The Gamma Family and Derived Distributions Applied in Hydrology", B. Bobee & F. Ashkar, Water Resources Publications, 1991.
        /// </para>
        /// <para>
        /// Example 7.1 page 87.
        /// </para>
        /// </remarks>
        [TestMethod()]
        public void Test_LP3_Quantile()
        {
            var LP3 = new LogPearsonTypeIII(2.26878d, 0.10621d, -0.02925d);

            double f = LP3.CDF(277);

            double q1000 = LP3.InverseCDF(0.99d);
            double true_q1000 = 326.25d;
            Assert.AreEqual((q1000 - true_q1000) / true_q1000 < 0.01d, true);
            double p = LP3.CDF(q1000);
            double true_p = 0.99d;
            Assert.AreEqual((p - true_p) / true_p < 0.01d, true);
        }

        /// <summary>
        /// Test the standard error for the Log-Pearson Type III Distribution.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Reference: "The Gamma Family and Derived Distributions Applied in Hydrology", B. Bobee & F. Ashkar, Water Resources Publications, 1991.
        /// </para>
        /// <para>
        /// Example 7.1 & 6.4 page 87-93.
        /// </para>
        /// </remarks>
        [TestMethod()]
        public void Test_LP3_StandardError()
        {

            // Method of Moments
            var LP3 = new LogPearsonTypeIII(2.26878d, 0.10699d, -0.04061d);
            double qVar999 = Math.Sqrt(LP3.QuantileVariance(0.99d, 69, ParameterEstimationMethod.MethodOfMoments));
            double true_qVar999 = 25.053d;
            Assert.AreEqual((qVar999 - true_qVar999) / true_qVar999 < 0.01d, true);

            // Maximum Likelihood
            LP3 = new LogPearsonTypeIII(2.26878d, 0.10621d, -0.02925d);
            qVar999 = Math.Sqrt(LP3.QuantileVariance(0.99d, 69, ParameterEstimationMethod.MaximumLikelihood));
            true_qVar999 = 25d;
            Assert.AreEqual((qVar999 - true_qVar999) / true_qVar999 < 0.01d, true);

        }

        /// <summary>
        /// Test multi-parameter function
        /// </summary>
        public double FXYZ(double[] points)
        {
            double x = points[0];
            double y = points[1];
            double z = points[2];
            return Math.Pow(x, 3d) + Math.Pow(y, 4d) + Math.Pow(z, 5d);
        }

        /// <summary>
        /// Verifying that input parameters can create distribution.
        /// </summary>
        [TestMethod()]
        public void CanCreateLP3()
        {
            var LP3 = new LogPearsonTypeIII();
            Assert.AreEqual(LP3.Mu, 3);
            Assert.AreEqual(LP3.Sigma, 0.5);
            Assert.AreEqual(LP3.Gamma, 0);
        }

        /// <summary>
        /// Testing distribution with bad parameters.
        /// </summary>
        [TestMethod()]
        public void LP3Fails()
        {
            var LP3 = new LogPearsonTypeIII(double.NaN, double.NaN, double.NaN);
            Assert.IsFalse(LP3.ParametersValid);

            var LP3ii = new LogPearsonTypeIII(double.PositiveInfinity, double.NegativeInfinity, double.PositiveInfinity);
            Assert.IsFalse(LP3ii.ParametersValid);
        }

        /// <summary>
        /// Testing parameter to string.
        /// </summary>
        [TestMethod()]
        public void ValidateParameterToString()
        {
            var LP3 = new LogPearsonTypeIII();
            Assert.AreEqual(LP3.ParametersToString[0, 0], "Mean (of log) (µ)");
            Assert.AreEqual(LP3.ParametersToString[1, 0], "Std Dev (of log) (σ)");
            Assert.AreEqual(LP3.ParametersToString[2, 0], "Skew (of log) (γ)");
            Assert.AreEqual(LP3.ParametersToString[0, 1], "3");
            Assert.AreEqual(LP3.ParametersToString[1, 1], "0.5");
            Assert.AreEqual(LP3.ParametersToString[2, 1], "0");
        }

        /// <summary>
        /// Testing the mean function.
        /// </summary>
        [TestMethod()]
        public void ValidateMean()
        {
            var LP3 = new LogPearsonTypeIII();
            Assert.AreEqual(LP3.Mean, 1333.5214, 1e-04);

            var LP3ii = new LogPearsonTypeIII(1,1,1);
            Assert.AreEqual(LP3ii.Mean, 190.8665, 1e-04);
        }

        /// <summary>
        /// Testing median.
        /// </summary>
        [TestMethod()]
        public void ValidateMedian()
        {
            var LP3 = new LogPearsonTypeIII();
            Assert.AreEqual(LP3.Median, 1000,1e-04);
        }

        /// <summary>
        /// Testing mode function.
        /// </summary>
        [TestMethod()]
        public void ValidateMode()
        {
            var LP3 = new LogPearsonTypeIII();
            Assert.AreEqual(LP3.Mode, 562.3413, 1e-04);

            var LP3ii = new LogPearsonTypeIII(1, 1, 1);
            Assert.AreEqual(LP3ii.Mode, 3.16227,1e-04);
        }

        /// <summary>
        /// Testing standard deviation.
        /// </summary>
        [TestMethod()]
        public void ValidateStandardDeviation()
        {
            var LP3 = new LogPearsonTypeIII();
            Assert.AreEqual(LP3.StandardDeviation, 1176.4345,1e-04);

            var LP3ii = new LogPearsonTypeIII(1,1,1);
            Assert.AreEqual(LP3ii.StandardDeviation, double.NaN);
        }
        
        /// <summary>
        /// Testing skew.
        /// </summary>
        [TestMethod()]
        public void ValidateSkew()
        {
            var LP3 = new LogPearsonTypeIII();
            Assert.AreEqual(LP3.Skewness, 3.3332,1e-04);

            var LP3ii = new LogPearsonTypeIII(1, 1, 1);
            Assert.AreEqual(LP3ii.Skewness, double.NaN);
        }

        /// <summary>
        /// Testing kurtosis.
        /// </summary>
        [TestMethod()]
        public void ValidateKurtosis()
        {
            var LP3 = new LogPearsonTypeIII();
            Assert.AreEqual(LP3.Kurtosis, 27.73365,1e-04);

            var LP3ii = new LogPearsonTypeIII(1, 1, 1);
            Assert.AreEqual(LP3ii.Kurtosis, -3,1e-04);
        }

        /// <summary>
        /// Testing minimum.
        /// </summary>
        [TestMethod()]
        public void ValidateMinimum()
        {
            var LP3 = new LogPearsonTypeIII();
            Assert.AreEqual(LP3.Minimum, 0);

            var LP3ii = new LogPearsonTypeIII(1,1,1);
            Assert.AreEqual(LP3ii.Minimum, 0.1, 1e-05);

            var LP3iii = new LogPearsonTypeIII(1, -1, 1);
            Assert.AreEqual(LP3iii.Minimum, 0);
        }

        /// <summary>
        /// Testing maximum.
        /// </summary>
        [TestMethod()]
        public void ValidateMaximum() 
        {
            var LP3 = new LogPearsonTypeIII();
            Assert.AreEqual(LP3.Maximum, double.PositiveInfinity);

            var LP3ii = new LogPearsonTypeIII(1,1,1);
            Assert.AreEqual(LP3ii.Maximum, double.PositiveInfinity);

            var LP3iii = new LogPearsonTypeIII(1, -1, 1);
            Assert.AreEqual(LP3iii.Maximum, 1000,1e-04);
        }

        /// <summary>
        /// Testing PDF method.
        /// </summary>
        [TestMethod()]  
        public void ValidatePDF()
        {
            var LP3 = new LogPearsonTypeIII();
            Assert.AreEqual(LP3.PDF(-1), 0);
            Assert.AreEqual(LP3.PDF(1),5.2774e-09,1e-13);
        }

        /// <summary>
        /// Testing CDF method.
        /// </summary>
        [TestMethod()]
        public void ValidateCDF()
        {
            var LP3 = new LogPearsonTypeIII();
            Assert.AreEqual(LP3.CDF(-1), 0);
            Assert.AreEqual(LP3.CDF(1), 9.8658e-10,1e-13);
        }

        /// <summary>
        /// Testing inverse CDF.
        /// </summary>
        [TestMethod()]
        public void ValidateInverseCDF()
        {
            var LP3 = new LogPearsonTypeIII();
            Assert.AreEqual(LP3.InverseCDF(0), 0);
            Assert.AreEqual(LP3.InverseCDF(1), double.PositiveInfinity);
            Assert.AreEqual(LP3.InverseCDF(0.3), 546.7637,1e-04);
        }

        /// <summary>
        /// Testing Wilson-Hilferty Inverse CDF.
        /// </summary>
        [TestMethod()]
        public void ValidateWilsonHilfertyInverseCDF()
        {
            var LP3 = new LogPearsonTypeIII();
            Assert.AreEqual(LP3.WilsonHilfertyInverseCDF(0), 0);
            Assert.AreEqual(LP3.WilsonHilfertyInverseCDF(1),double.PositiveInfinity);
            Assert.AreEqual(LP3.WilsonHilfertyInverseCDF(0.4), 747.01005,1e-05);
        }
    }
}
