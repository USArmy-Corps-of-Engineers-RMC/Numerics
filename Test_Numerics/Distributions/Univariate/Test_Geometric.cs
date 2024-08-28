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
    public class Test_Geometric
    {
        /// <summary>
        /// Verified using Palisade's @Risk and Accord.Net
        /// </summary>
        [TestMethod()]
        public void Test_GeometricDist()
        {
            double true_mean = 1.3809523809523812d;
            double true_mode = 0.0d;
            double true_median = 1.0d;
            double true_stdDev = Math.Sqrt(3.2879818594104315d);
            double true_skew = 2.0746d;
            double true_kurt = 9.3041d;
            double true_pdf = 0.141288d;
            double true_cdf = 0.80488799999999994d;
            double true_icdf05 = 0.0d;
            double true_icdf95 = 5.0d;
            var G = new Geometric(0.42d);
            Assert.AreEqual(G.Mean, true_mean, 0.0001d);
            Assert.AreEqual(G.Median, true_median, 0.0001d);
            Assert.AreEqual(G.Mode, true_mode, 0.0001d);
            Assert.AreEqual(G.StandardDeviation, true_stdDev, 0.0001d);
            Assert.AreEqual(G.Skewness, true_skew, 0.0001d);
            Assert.AreEqual(G.Kurtosis, true_kurt, 0.0001d);
            Assert.AreEqual(G.PDF(2.0d), true_pdf, 0.0001d);
            Assert.AreEqual(G.CDF(2.0d), true_cdf, 0.0001d);
            Assert.AreEqual(G.InverseCDF(0.05d), true_icdf05, 0.0001d);
            Assert.AreEqual(G.InverseCDF(0.95d), true_icdf95, 0.0001d);
        }
    }
}
