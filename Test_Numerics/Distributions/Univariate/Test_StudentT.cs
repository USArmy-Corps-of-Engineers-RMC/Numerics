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
    public class Test_StudentT
    {

        [TestMethod()]
        public void Test_StudentT_PDF()
        {
            var t = new StudentT(4.2d);
            double pdf = t.PDF(1.4d);
            double result = 0.138377537135553d;
            Assert.AreEqual(pdf, result, 1E-10);
            t = new StudentT(2.5d, 0.5d, 4.2d);
            pdf = t.PDF(1.4d);
            result = 0.0516476521260042d;
            Assert.AreEqual(pdf, result, 1E-10);
        }

        [TestMethod()]
        public void Test_StudentT_CDF()
        {
            var t = new StudentT(4.2d);
            double cdf = t.CDF(1.4d);
            double result = 0.882949686336585d;
            Assert.AreEqual(cdf, result, 1E-10);
            t = new StudentT(2.5d, 0.5d, 4.2d);
            cdf = t.CDF(1.4d);
            result = 0.0463263350898173d;
            Assert.AreEqual(cdf, result, 1E-10);
        }

        [TestMethod()]
        public void Test_StudentT_InverseCDF()
        {
            var t = new StudentT(4.2d);
            double cdf = t.CDF(1.4d);
            double invcdf = t.InverseCDF(cdf);
            double result = 1.4d;
            Assert.AreEqual(invcdf, result, 1E-2);
            t = new StudentT(2.5d, 0.5d, 4.2d);
            cdf = t.CDF(1.4d);
            invcdf = t.InverseCDF(cdf);
            result = 1.4d;
            Assert.AreEqual(invcdf, result, 1E-2);
        }
    }
}
