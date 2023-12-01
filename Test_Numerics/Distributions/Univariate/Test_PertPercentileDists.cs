using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Distributions;

namespace Distributions.Univariate
{
    [TestClass]
    public class Test_PertPercentileDists
    {

        /// <summary>
        /// Verified using Palisade's @Risk
        /// </summary>
        [TestMethod()]
        public void Test_PertPercentile()
        {
            double true_min = -2.6453d;
            double true_max = 2.6453d;
            int true_mean = 0;
            int true_mode = 0;
            int true_median = 0;
            double true_stdDev = 0.9998d;
            int true_skew = 0;
            double true_kurt = 2.3333d;
            double true_icdf05 = -1.644;
            double true_icdf50 = 0.0;
            double true_icdf95 = 1.644;
            var pert = new PertPercentile(-1.644d, 0d, 1.644d);
            pert.SolveParameters();
            Assert.AreEqual(pert.Minimum, true_min, 1E-3);
            //Assert.AreEqual(pert.Maximum, true_max, 1E-3);
            //Assert.AreEqual(pert.Mean, true_mean, 1E-3);
            //Assert.AreEqual(pert.Median, true_median, 1E-3);
            //Assert.AreEqual(pert.Mode, true_mode, 1E-3);
            //Assert.AreEqual(pert.StandardDeviation, true_stdDev, 1E-3);
            //Assert.AreEqual(pert.Skew, true_skew, 1E-3);
            //Assert.AreEqual(pert.Kurtosis, true_kurt, 1E-3);
            //Assert.AreEqual(pert.CDF(-1.644), 0.05, 1E-3);
            //Assert.AreEqual(pert.CDF(0.0), 0.5, 1E-3);
            //Assert.AreEqual(pert.CDF(1.644), 0.95, 1E-3);
            //Assert.AreEqual(pert.InverseCDF(0.05d), true_icdf05, 1E-3);
            //Assert.AreEqual(pert.InverseCDF(0.5d), true_icdf50, 1E-3);
            //Assert.AreEqual(pert.InverseCDF(0.95d), true_icdf95, 1E-3);
        }

        /// <summary>
        /// Verified using Palisade's @Risk
        /// </summary>
        [TestMethod()]
        public void Test_PertPercentile_2()
        {
            double fifth = 12;
            double fiftieth = 21;
            double ninetyfifth = 32;
            double true_mean = 21;
            var pert = new PertPercentile(fifth, fiftieth, ninetyfifth);
            pert.SolveParameters();
            var p1 = pert.CDF(fifth);
            var p2 = pert.CDF(fiftieth);
            var p3 = pert.CDF(ninetyfifth);

            //Assert.AreEqual(pert.Mean, true_mean, 1);
            //Assert.AreEqual(pert.CDF(fifth), 0.05, 1E-2);
            //Assert.AreEqual(pert.CDF(fiftieth), 0.5, 1E-2);
            //Assert.AreEqual(pert.CDF(ninetyfifth), 0.95, 1E-2);
            //Assert.AreEqual(pert.InverseCDF(0.05d), fifth, 1E-1);
            //Assert.AreEqual(pert.InverseCDF(0.5d), fiftieth, 1E-1);
            //Assert.AreEqual(pert.InverseCDF(0.95d), ninetyfifth, 1E-1);

            fifth = 0;
            fiftieth = 3;
            ninetyfifth = 6;
            true_mean = 3;
            pert = new PertPercentile(fifth, fiftieth, ninetyfifth);
            p1 = pert.CDF(fifth);
            p2 = pert.CDF(fiftieth);
            p3 = pert.CDF(ninetyfifth);
            //Assert.AreEqual(pert.Mean, true_mean, 1);
            //Assert.AreEqual(pert.CDF(fifth), 0.05, 1E-2);
            //Assert.AreEqual(pert.CDF(fiftieth), 0.5, 1E-2);
            //Assert.AreEqual(pert.CDF(ninetyfifth), 0.95, 1E-2);
            //Assert.AreEqual(pert.InverseCDF(0.05d), fifth, 1E-1);
            //Assert.AreEqual(pert.InverseCDF(0.5d), fiftieth, 1E-1);
            //Assert.AreEqual(pert.InverseCDF(0.95d), ninetyfifth, 1E-1);

            fifth = 0;
            fiftieth = 1;
            ninetyfifth = 3;
            true_mean = 1;
            pert = new PertPercentile(fifth, fiftieth, ninetyfifth);
            p1 = pert.CDF(fifth);
            p2 = pert.CDF(fiftieth);
            p3 = pert.CDF(ninetyfifth);
            //Assert.AreEqual(pert.Mean, true_mean, 1);
            //Assert.AreEqual(pert.CDF(fifth), 0.05, 1E-2);
            //Assert.AreEqual(pert.CDF(fiftieth), 0.5, 1E-2);
            //Assert.AreEqual(pert.CDF(ninetyfifth), 0.95, 1E-2);
            //Assert.AreEqual(pert.InverseCDF(0.05d), fifth, 1E-1);
            //Assert.AreEqual(pert.InverseCDF(0.5d), fiftieth, 1E-1);
            //Assert.AreEqual(pert.InverseCDF(0.95d), ninetyfifth, 1E-1);

            fifth = 0;
            fiftieth = 0;
            ninetyfifth = 2;
            true_mean = 0;
            pert = new PertPercentile(fifth, fiftieth, ninetyfifth);
            //Assert.AreEqual(pert.Mean, true_mean, 1);
            //Assert.AreEqual(pert.CDF(fiftieth), 0.5, 1E-2);
            //Assert.AreEqual(pert.CDF(ninetyfifth), 0.95, 1E-2);
            //Assert.AreEqual(pert.InverseCDF(0.5d), fiftieth, 1E-1);
            //Assert.AreEqual(pert.InverseCDF(0.95d), ninetyfifth, 1E-1);
            p1 = pert.CDF(fifth);
            p2 = pert.CDF(fiftieth);
            p3 = pert.CDF(ninetyfifth);

            fifth = 0;
            fiftieth = 2;
            ninetyfifth = 2;
            true_mean = 1;
            pert = new PertPercentile(fifth, fiftieth, ninetyfifth);
            //Assert.AreEqual(pert.Mean, true_mean, 1);
            //Assert.AreEqual(pert.CDF(fifth), 0.05, 1E-2);
            //Assert.AreEqual(pert.CDF(ninetyfifth), 0.95, 1E-2);
            //Assert.AreEqual(pert.InverseCDF(0.05d), fifth, 1E-2);
            //Assert.AreEqual(pert.InverseCDF(0.95d), ninetyfifth, 1E-1);
            p1 = pert.CDF(fifth);
            p2 = pert.CDF(fiftieth);
            p3 = pert.CDF(ninetyfifth);

            fifth = 14;
            fiftieth = 25;
            ninetyfifth = 36;
            true_mean = 1;
            pert = new PertPercentile(fifth, fiftieth, ninetyfifth);
            p1 = pert.CDF(fifth);
            p2 = pert.CDF(fiftieth);
            p3 = pert.CDF(ninetyfifth);

            //Assert.AreEqual(pert.Mean, true_mean, 1);
            //Assert.AreEqual(pert.CDF(fifth), 0.05, 1E-2);
            //Assert.AreEqual(pert.CDF(ninetyfifth), 0.95, 1E-2);
            //Assert.AreEqual(pert.InverseCDF(0.05d), fifth, 1E-2);
            //Assert.AreEqual(pert.InverseCDF(0.95d), ninetyfifth, 1E-2);

        }

        /// <summary>
        /// Verified using Palisade's @Risk
        /// </summary>
        [TestMethod()]
        public void Test_PertPercentileZ()
        {
            double true_min = -2.6453d;
            double true_max = 2.6453d;
            int true_mean = 0;
            int true_mode = 0;
            int true_median = 0;
            double true_stdDev = 0.9998d;
            int true_skew = 0;
            double true_kurt = 2.3333d;
            double true_icdf05 = 0.05d;
            double true_icdf50 = 0.5d;
            double true_icdf95 = 0.95d;
            var pert = new PertPercentileZ(-1.644d, 0d, 1.644d);
            //Assert.AreEqual(pert.Minimum, true_min, 0.0001d);
            //Assert.AreEqual(pert.Maximum, true_max, 0.0001d);
            //Assert.AreEqual(pert.Mean, true_mean, 0.0001d);
            //Assert.AreEqual(pert.Median, true_median, 0.0001d);
            //Assert.AreEqual(pert.Mode, true_mode, 0.0001d);
            //Assert.AreEqual(pert.StandardDeviation, true_stdDev, 0.0001d);
            //Assert.AreEqual(pert.Skew, true_skew, 0.0001d);
            //Assert.AreEqual(pert.Kurtosis, true_kurt, 0.0001d);
            //Assert.AreEqual(pert.CDF(0.05d), 0.05, 0.001d);
            //Assert.AreEqual(pert.CDF(0.5d), 0.5, 0.001d);
            //Assert.AreEqual(pert.CDF(0.95d), 0.95, 0.001d);
            //Assert.AreEqual(pert.InverseCDF(0.05d), true_icdf05, 0.001d);
            //Assert.AreEqual(pert.InverseCDF(0.5d), true_icdf50, 0.001d);
            //Assert.AreEqual(pert.InverseCDF(0.95d), true_icdf95, 0.001d);
        }
    }
}
