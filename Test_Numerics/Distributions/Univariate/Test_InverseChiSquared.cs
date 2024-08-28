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
    public class Test_InverseChiSquared
    {
        /// <summary>
        /// Verified using Accord.Net
        /// </summary>
        [TestMethod()]
        public void Test_InverseChiSquaredDist()
        {
            double true_mean = 0.2;
            double true_median = 6.345811068141737d;
            double true_pdf = 0.0000063457380298844403d;
            double true_cdf = 0.50860033566176044d;
            double true_icdf = 6.27d;
            var IG = new InverseChiSquared(7, (1d / 7d));
            double pdf = IG.PDF(6.27d);
            double cdf = IG.CDF(6.27d);
            double icdf = IG.InverseCDF(cdf);
            Assert.AreEqual(IG.Mean, true_mean, 0.0001d);
            Assert.AreEqual(IG.Median, true_median, 0.0001d);
            Assert.AreEqual(IG.PDF(6.27d), true_pdf, 0.0001d);
            Assert.AreEqual(IG.CDF(6.27d), true_cdf, 0.0001d);
            Assert.AreEqual(IG.InverseCDF(IG.CDF(6.27d)), true_icdf, 0.0001d);

        }
    }
}
