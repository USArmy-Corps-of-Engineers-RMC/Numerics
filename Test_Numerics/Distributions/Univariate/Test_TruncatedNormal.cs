using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Distributions;

namespace Distributions.Univariate
{
    [TestClass]
    public class Test_TruncatedNormal
    {

        /// <summary>
        /// This method was verified against the R package "truncnorm"
        /// </summary>
        [TestMethod]
        public void Test_TruncatedNormalDist()
        {
            var tn = new TruncatedNormal(2, 1, 1.10, 2.11);
            var d = tn.PDF(1.5);
            var p = tn.CDF(1.5);
            var q = tn.InverseCDF(p);

            Assert.AreEqual(d, 0.9786791, 1E-5);
            Assert.AreEqual(p, 0.3460251, 1E-5);
            Assert.AreEqual(q, 1.5, 1E-5);

            tn = new TruncatedNormal(10, 3, 8, 25);
            d = tn.PDF(12.75);
            p = tn.CDF(12.75);
            q = tn.InverseCDF(p);

            Assert.AreEqual(d, 0.1168717, 1E-5);
            Assert.AreEqual(p, 0.7596566, 1E-5);
            Assert.AreEqual(q, 12.75, 1E-5);


            tn = new TruncatedNormal(0, 3, 0, 9);
            d = tn.PDF(4.5);
            p = tn.CDF(4.5);
            q = tn.InverseCDF(p);

            Assert.AreEqual(d, 0.08657881, 1E-5);
            Assert.AreEqual(p, 0.868731, 1E-5);
            Assert.AreEqual(q, 4.5, 1E-5);


            var KDE = new PearsonTypeIII(-2.372978, 0.338314, -1.5);
            var graph = KDE.CreatePDFGraph();
            for (int i = 0; i < graph.GetLength(0); i++)
            {
                Debug.Print(graph[ i, 0] + ", " + graph[i, 1]);
            }


            var cdf = KDE.CDF(-2);
        }
    }
}
