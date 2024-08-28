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
