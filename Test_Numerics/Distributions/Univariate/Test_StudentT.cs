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
