/**
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
* **/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Distributions;

namespace Distributions.Univariate
{
    /// <summary>
    /// Testing the Gamma distribution algorithm.
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
    /// <see href = "https://github.com/mathnet/mathnet-numerics/blob/master/src/Numerics.Tests/DistributionTests/Discrete/BinomialTests.cs" />
    /// </para>
    /// </remarks>

    [TestClass]
    public class Test_GammaDistribution
    {
        // Reference: "The Gamma Family and Derived Distributions Applied in Hydrology", B. Bobee & F. Ashkar, Water Resources Publications, 1991.
        // Table 1.2 Maximum annual peak discharge values in cms, observed at the Harricana River at Amos (Quebec, Canada)
        private double[] sample = new double[] { 122d, 244d, 214d, 173d, 229d, 156d, 212d, 263d, 146d, 183d, 161d, 205d, 135d, 331d, 225d, 174d, 98.8d, 149d, 238d, 262d, 132d, 235d, 216d, 240d, 230d, 192d, 195d, 172d, 173d, 172d, 153d, 142d, 317d, 161d, 201d, 204d, 194d, 164d, 183d, 161d, 167d, 179d, 185d, 117d, 192d, 337d, 125d, 166d, 99.1d, 202d, 230d, 158d, 262d, 154d, 164d, 182d, 164d, 183d, 171d, 250d, 184d, 205d, 237d, 177d, 239d, 187d, 180d, 173d, 174d };

        /// <summary>
        /// Method of Moments.
        /// </summary>
        /// <remarks>
        /// Solution found in the book “The Gamma family and derived distributions applied in hydrology” by Bobée and Ashkar, 1991.
        /// </remarks>
        [TestMethod()]
        public void Test_GammaDist_MOM()
        {
            var G = new GammaDistribution();
            G.Estimate(sample, ParameterEstimationMethod.MethodOfMoments);
            double alpha = 1d / G.Theta;
            double lambda = G.Kappa;
            double trueA = 0.08317d;
            double trueL = 15.91188d;
            Assert.AreEqual((alpha - trueA) / trueA < 0.01d, true);
            Assert.AreEqual((lambda - trueL) / trueL < 0.01d, true);
        }

        [TestMethod()]
        public void Test_GammaDist_LMOM_Fit()
        {
            // Air quality - wind data from R
            var data = new double[] { 7.4, 8, 12.6, 11.5, 14.3, 14.9, 8.6, 13.8, 20.1, 8.6, 6.9, 9.7, 9.2, 10.9, 13.2, 11.5, 12, 18.4, 11.5, 9.7, 9.7, 16.6, 9.7, 12, 16.6, 14.9, 8, 12, 14.9, 5.7, 7.4, 8.6, 9.7, 16.1, 9.2, 8.6, 14.3, 9.7, 6.9, 13.8, 11.5, 10.9, 9.2, 8, 13.8, 11.5, 14.9, 20.7, 9.2, 11.5, 10.3, 6.3, 1.7, 4.6, 6.3, 8, 8, 10.3, 11.5, 14.9, 8, 4.1, 9.2, 9.2, 10.9, 4.6, 10.9, 5.1, 6.3, 5.7, 7.4, 8.6, 14.3, 14.9, 14.9, 14.3, 6.9, 10.3, 6.3, 5.1, 11.5, 6.9, 9.7, 11.5, 8.6, 8, 8.6, 12, 7.4, 7.4, 7.4, 9.2, 6.9, 13.8, 7.4, 6.9, 7.4, 4.6, 4, 10.3, 8, 8.6, 11.5, 11.5, 11.5, 9.7, 11.5, 10.3, 6.3, 7.4, 10.9, 10.3, 15.5, 14.3, 12.6, 9.7, 3.4, 8, 5.7, 9.7, 2.3, 6.3, 6.3, 6.9, 5.1, 2.8, 4.6, 7.4, 15.5, 10.9, 10.3, 10.9, 9.7, 14.9, 15.5, 6.3, 10.9, 11.5, 6.9, 13.8, 10.3, 10.3, 8, 12.6, 9.2, 10.3, 10.3, 16.6, 6.9, 13.2, 14.3, 8, 11.5 };
            var G = new GammaDistribution();
            G.Estimate(data, ParameterEstimationMethod.MethodOfLinearMoments);
            double scale = G.Theta;
            double shape = G.Kappa;
            double true_scale = 1.280143d;
            double true_shape = 7.778442d;
            Assert.AreEqual(scale, true_scale, 0.0001d);
            Assert.AreEqual(shape, true_shape, 0.0001d);
            var lmom = G.LinearMomentsFromParameters(G.GetParameters);
            Assert.AreEqual(lmom[0], 9.9575163d, 0.0001d);
            Assert.AreEqual(lmom[1], 1.9822363d, 0.0001d);
            Assert.AreEqual(lmom[2], 0.1175059d, 0.0001d);
            Assert.AreEqual(lmom[3], 0.1268391d, 0.0001d);
        }

        /// <summary>
        /// Verification of Gamma fit with maximum likelihood.
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
        public void Test_GammaDist_MLE()
        {
            var G = new GammaDistribution();
            G.Estimate(sample, ParameterEstimationMethod.MaximumLikelihood);
            double alpha = 1d / G.Theta;
            double lambda = G.Kappa;
            double trueA = 0.08833d;
            double trueL = 16.89937d;
            Assert.AreEqual((alpha - trueA) / trueA < 0.01d, true);
            Assert.AreEqual((lambda - trueL) / trueL < 0.01d, true);
        }

        /// <summary>
        /// Test the quantile function for the Gamma Distribution.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Reference: "The Gamma Family and Derived Distributions Applied in Hydrology", B. Bobee & F. Ashkar, Water Resources Publications, 1991.
        /// </para>
        /// <para>
        /// Example 5.3 page 52.
        /// </para>
        /// </remarks>
        [TestMethod()]
        public void Test_GammaDist_Quantile()
        {
            var G = new GammaDistribution(1d / 0.08833d, 16.89937d);
            double q1000 = G.InverseCDF(0.99d);
            double true_1000 = 315.87d;
            Assert.AreEqual((q1000 - true_1000) / true_1000 < 0.01d, true);
            double p = G.CDF(q1000);
            double true_p = 0.99d;
            Assert.AreEqual(p == true_p, true);
        }

        /// <summary>
        /// Test the standard error for the Gamma Distribution.
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
        public void Test_GammaDist_StandardError()
        {

            // Method of Moments
            var G = new GammaDistribution(1d / 0.08317d, 15.9118d);
            double se1000 = Math.Sqrt(G.QuantileVariance(0.99d, 69, ParameterEstimationMethod.MethodOfMoments));
            double true_se1000 = 16.024d;
            Assert.AreEqual((se1000 - true_se1000) / true_se1000 < 0.01d, true);

            // Maximum Likelihood
            G = new GammaDistribution(1d / 0.08833d, 16.89937d);
            se1000 = Math.Sqrt(G.QuantileVariance(0.99d, 69, ParameterEstimationMethod.MaximumLikelihood));
            true_se1000 = 15.022d;
            Assert.AreEqual((se1000 - true_se1000) / true_se1000 < 0.01d, true);
        }

        /// <summary>
        /// Testing parameters are creating gamma distribution.
        /// </summary>
        [TestMethod()]
        public void CanCreateGamma()
        {
            var G = new GammaDistribution(2, 10);
            Assert.AreEqual(G.Theta, 2);
            Assert.AreEqual(G.Kappa, 10);

            var G2 = new GammaDistribution(-1, 4);
            Assert.AreEqual(G2.Theta, -1);
            Assert.AreEqual(G2.Kappa, 4);

        }

        /// <summary>
        /// Validating inverse scale parameter.
        /// </summary>
        [TestMethod()]
        public void ValidateRate()
        {
            var G = new GammaDistribution(2, 2);
            Assert.AreEqual(G.Rate, 0.5);

            var G2 = new GammaDistribution();
            Assert.AreEqual(G2.Rate, 0.1);
        }

        /// <summary>
        /// Checking parameters to string function.
        /// </summary>
        [TestMethod()]
        public void ValidateParametersToString()
        {
            var G = new GammaDistribution();
            Assert.AreEqual(G.ParametersToString[0, 0], "Scale (θ)");
            Assert.AreEqual(G.ParametersToString[1, 0], "Shape (κ)");
            Assert.AreEqual(G.ParametersToString[0, 1], "10");
            Assert.AreEqual(G.ParametersToString[1, 1], "2");
        }

        /// <summary>
        /// Checking bad parameters for Gamma distribution.
        /// </summary>
        [TestMethod()]
        public void GammaFails()
        {
            var G = new GammaDistribution(-1, 0);
            Assert.IsFalse(G.ParametersValid);

            var G2 = new GammaDistribution(0, 0);
            Assert.IsFalse(G2.ParametersValid);

            var G3 = new GammaDistribution(0,1);
            Assert.IsFalse(G3.ParametersValid);

            var G4 = new GammaDistribution(1, 0);
            Assert.IsFalse(G4.ParametersValid);
        }

        /// <summary>
        /// Checking mean function.
        /// </summary>
        [TestMethod()]
        public void ValidateMean()
        {
            var G = new GammaDistribution();
            Assert.AreEqual(G.Mean, 20);
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod()]
        public void ValidateMedian()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod()]
        public void ValidateMode()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod()]
        public void ValidateStandardDeviation()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod()]
        public void ValidateSkew()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod()]
        public void ValidateKurtosis()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod()]
        public void ValidateMinimum()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod()]
        public void ValidateMaximum()
        {

        }

    }
}
