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
