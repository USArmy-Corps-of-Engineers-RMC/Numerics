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
    public class Test_NoncentralT
    {

        [TestMethod()]
        public void Test_NoncentralT_PDF()
        {
            var t = new NoncentralT(4d, 2.42d);
            double pdf = t.PDF(1.4d);
            double result = 0.23552141805184526d;
            Assert.AreEqual(pdf, result, 1E-6);
        }

        [TestMethod()]
        public void Test_NoncentralT_CDF()
        {
            var t = new NoncentralT(4d, 2.42d);
            double cdf = t.CDF(1.4d);
            double result = 0.15955740661144721d;
            Assert.AreEqual(cdf, result, 1E-6);
        }

        [TestMethod()]
        public void Test_NoncentralT_InverseCDF()
        {
            var t = new NoncentralT(4d, 2.42d);
            double cdf = t.CDF(1.4d);
            double invcdf = t.InverseCDF(cdf);
            double result = 1.4d;
            Assert.AreEqual(invcdf, result, 1E-6);
            var table = new[,] { { 3.0d, 0.0d, 1d, 0.89758361765043326d }, { 3.0d, 0.0d, 2d, 0.9522670169d }, { 3.0d, 0.0d, 3d, 0.97116555718878128d }, { 3.0d, 0.5d, 1d, 0.8231218864d }, { 3.0d, 0.5d, 2d, 0.904902151d }, { 3.0d, 0.5d, 3d, 0.9363471834d }, { 3.0d, 1.0d, 1d, 0.7301025986d }, { 3.0d, 1.0d, 2d, 0.8335594263d }, { 3.0d, 1.0d, 3d, 0.8774010255d }, { 3.0d, 2.0d, 1d, 0.5248571617d }, { 3.0d, 2.0d, 2d, 0.6293856597d }, { 3.0d, 2.0d, 3d, 0.6800271741d }, { 3.0d, 4.0d, 1d, 0.20590131975d }, { 3.0d, 4.0d, 2d, 0.2112148916d }, { 3.0d, 4.0d, 3d, 0.2074730718d }, { 15.0d, 7.0d, 15d, 0.9981130072d }, { 15.0d, 7.0d, 20d, 0.999487385d }, { 15.0d, 7.0d, 25d, 0.9998391562d }, { 0.05d, 1.0d, 1d, 0.168610566972d }, { 0.05d, 1.0d, 2d, 0.16967950985d }, { 0.05d, 1.0d, 3d, 0.1701041003d }, { 4.0d, 2.0d, 10d, 0.9247683363d }, { 4.0d, 3.0d, 10d, 0.7483139269d }, { 4.0d, 4.0d, 10d, 0.4659802096d }, { 5.0d, 2.0d, 10d, 0.9761872541d }, { 5.0d, 3.0d, 10d, 0.8979689357d }, { 5.0d, 4.0d, 10d, 0.7181904627d }, { 6.0d, 2.0d, 10d, 0.9923658945d }, { 6.0d, 3.0d, 10d, 0.9610341649d }, { 6.0d, 4.0d, 10d, 0.868800735d } };
            for (int i = 0; i < table.GetLength(0); i++)
            {
                double x = table[i, 0];
                double delta = table[i, 1];
                double degF = table[i, 2];
                var target = new NoncentralT(degF, delta);
                double expected = table[i, 3];
                double actual = target.CDF(x);
                Assert.AreEqual(expected, actual, 0.000001d);
                double expectedX = target.InverseCDF(actual);
                Assert.AreEqual(expectedX, x, 0.000001d);
            }

            //var quantiles = new double[] { 0.9999995d, 0.999999d, 0.999998d, 0.999995d, 0.99999d, 0.99998d, 0.99995d, 0.9999d, 0.9998d, 0.9995d, 0.999d, 0.998d, 0.995d, 0.99d, 0.98d, 0.95d, 0.9d, 0.85d, 0.8d, 0.7d, 0.6d, 0.5d, 0.4d, 0.3d, 0.2d, 0.1d, 0.05d };
            //var NC = new double[quantiles.Length];
            //double N = 30d;
            //double DF = N - 1d;
            //for (int i = 0, loopTo1 = quantiles.Length - 1; i <= loopTo1; i++)
            //{
            //    var Norm = new Normal();
            //    double Z = Norm.InverseCDF(quantiles[i]);
            //    var NCT = new NoncentralT(DF, Z * Math.Sqrt(N));
            //    var Output = new double[quantiles.Length];
            //    double Za = Norm.InverseCDF(0.95d);
            //    double term1 = 1d / N;
            //    double term2 = Z * Z / (2d * (N - 1d));
            //    double term3 = Za * Za / (2d * N * (N - 1d));
            //    double term4 = 1d - Za * Za / (2d * (N - 1d));
            //    double term5 = Z + Za * Math.Sqrt(term1 + term2 - term3);
            //    double TNC = term5 / term4;
            //    Output[i] = NCT.InverseCDF(0.95d);
            //    // Trace.WriteLine(Output(i) & "," & NCT.CDF(Output(i)))
            //}
        }
    }
}
