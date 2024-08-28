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
    public class Test_Pert
    {

        [TestMethod]
        public void Test_PertDist()
        {
            var P = new Pert(1d, 2d, 3d);
            var GB = GeneralizedBeta.PERT(1d, 2d, 3d);

            double true_mean = 2d;
            double true_median = 2d;
            double true_mode = 2d;
            double true_pdf = GB.PDF(1.27);
            double true_cdf = GB.CDF(1.27);
            double true_icdf = 1.27d;

            Assert.AreEqual(P.Mean, true_mean, 0.0001d);
            Assert.AreEqual(P.Median, true_median, 0.0001d);
            Assert.AreEqual(P.Mode, true_mode, 0.0001d);
            Assert.AreEqual(P.PDF(1.27d), true_pdf, 0.0001d);
            Assert.AreEqual(P.CDF(1.27d), true_cdf, 0.0001d);
            Assert.AreEqual(P.InverseCDF(P.CDF(1.27d)), true_icdf, 0.0001d);

        }

        /// <summary>
        /// Verified against R "mc2d" package
        /// </summary>
        [TestMethod]
        public void Test_PertDist2()
        {
            var P = new Pert(-1, 0, 1);
            var d = P.PDF(-0.5);
            var p = P.CDF(-0.5);
            var q = P.InverseCDF(p);

            Assert.AreEqual(d, 0.5273438, 1E-5);
            Assert.AreEqual(p, 0.1035156, 1E-5);
            Assert.AreEqual(q, -0.5, 1E-5);

        }

        /// <summary>
        /// Verified using R 'mc2d'
        /// </summary>
        [TestMethod]
        public void Test_Pert_R()
        {
            var x = new double[] { 0.1, 0.25, 0.5, 0.75, 0.9 };
            var p = new double[5];
            var true_p = new double[] { 0.0814600, 0.3671875, 0.8125000, 0.9843750, 0.9995400 };
            var pert = new Pert(0, 0.25, 1);
            for (int i = 0; i < 5; i++)
            {
                p[i] = pert.CDF(x[i]);
                Assert.AreEqual(true_p[i], p[i], 1E-7);
            }
            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(x[i], pert.InverseCDF(p[i]), 1E-7);
            }
        }

        [TestMethod]
        public void Test_Pert_MOM()
        {
            var dist = new Pert(-2, 10, 35);
            var sample = dist.GenerateRandomValues(12345, 1000);
            dist.Estimate(sample, ParameterEstimationMethod.MethodOfMoments);
            Assert.AreEqual(-2, dist.Min, 1);
            Assert.AreEqual(10, dist.MostLikely, 1);
            Assert.AreEqual(35, dist.Max, 5);
        }

        [TestMethod]
        public void Test_Pert_MLE()
        {
            var dist = new Pert(-2, 10, 35);
            var sample = dist.GenerateRandomValues(12345, 1000);
            dist.Estimate(sample, ParameterEstimationMethod.MaximumLikelihood);
            Assert.AreEqual(-2, dist.Min, 1);
            Assert.AreEqual(10, dist.MostLikely, 1);
            Assert.AreEqual(35, dist.Max, 1);
        }
    }
}
