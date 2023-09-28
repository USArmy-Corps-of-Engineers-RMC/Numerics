using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Data.Statistics;
using Numerics.Distributions;

namespace Data.Statistics
{
    [TestClass]
    public class Test_GoodnessOfFit
    {

        /// <summary>
        /// No code implements this test in this same manner. 
        /// Results from R are close though
        /// </summary>
        [TestMethod]
        public void Test_ChiSquaredTest()
        {
            var norm = new Normal(100, 15);
            var data = new double[30];
            for (int i = 1; i <= 30; i++)
                data[i - 1] = norm.InverseCDF((double)i / 31);
            var result = GoodnessOfFit.ChiSquared(data, norm);
            //Assert.AreEqual(0.032258, result, 1E-6);
        }

        /// <summary>
        /// Tested against R - stats
        /// </summary>
        [TestMethod]
        public void Test_KSTest()
        {
            var norm = new Normal(100, 15);
            var data = new double[30];
            for (int i = 1; i <= 30; i++)
                data[i - 1] = norm.InverseCDF((double)i / 31);
            var result = GoodnessOfFit.KolmogorovSmirnov(data, norm);
            Assert.AreEqual(0.032258, result, 1E-6);
        }

        /// <summary>
        /// Tested against R - nortest
        /// </summary>
        [TestMethod]
        public void Test_ADTest()
        {
            var norm = new Normal(100, 15);
            var data = new double[30];
            for (int i = 1; i <= 30; i++)
                data[i - 1] = norm.InverseCDF((double)i / 31);
            norm.SetParameters(Numerics.Data.Statistics.Statistics.Mean(data), Numerics.Data.Statistics.Statistics.StandardDeviation(data));
            var result = GoodnessOfFit.AndersonDarling(data, norm);
            Assert.AreEqual(0.044781, result, 1E-6);
        }
    }
}
