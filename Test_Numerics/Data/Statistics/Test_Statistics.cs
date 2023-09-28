using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Data.Statistics;

namespace Data.Statistics
{
    [TestClass]
    public class Test_Statistics
    {
        private double[] sample1 = new double[] { 122, 244, 214, 173, 229, 156, 212, 263, 146, 183, 161, 205, 135, 331, 225, 174, 98.8, 149, 238, 262, 132, 235, 216, 240, 230, 192, 195, 172, 173, 172, 153, 142, 317, 161, 201, 204, 194, 164, 183, 161, 167, 179, 185, 117, 192, 337, 125, 166, 99.1, 202, 230, 158, 262, 154, 164, 182, 164, 183, 171, 250, 184, 205, 237, 177, 239, 187, 180, 173, 174 };
        private double[] sample2 = new double[] { 279d, 105d, 171d, 171d, 129d, 127d, 194d, 234d, 251d, 152d, 207d, 205d, 183d, 137d, 148d, 189d, 182d, 236d, 148d, 150d, 207d, 252d, 237d, 209d, 225d, 137d, 207d, 129d, 148d, 192d, 95d, 231d, 255d, 220d, 205d, 163d, 265d, 190d, 226d, 123d, 108d, 145d, 197d, 233d, 133d, 177d, 211d, 180d, 200d, 197d, 142d, 166d, 251d, 254d, 226d, 197d, 250d, 194d, 190d, 181d, 290d, 185d, 123d, 208d, 238d, 179d, 189d, 225d, 236d };
        private double[] jackKnifeSample = new double[] { 3.35602585719312d, 3.07554696139253d, 3.04921802267018d, 3.07554696139253d, 3.13033376849501d, 3.13033376849501d, 3.31597034545692d, 3.15533603746506d, 3.17897694729317d, 3.29446622616159d, 3.37657695705651d, 3.72015930340596d, 3.25042000230889d, 3.25042000230889d, 3.10380372095596d, 3.45178643552429d, 3.15533603746506d, 3.37657695705651d, 3.45178643552429d, 3.29446622616159d };

        [TestMethod()]
        public void Test_Minimum()
        {
            double val = Numerics.Data.Statistics.Statistics.Minimum(sample1);
            double trueVal = 98.8d;
            Assert.AreEqual(val, trueVal, 1E-10);
        }

        [TestMethod()]
        public void Test_Maximum()
        {
            double val = Numerics.Data.Statistics.Statistics.Maximum(sample1);
            double trueVal = 337.0d;
            Assert.AreEqual(val, trueVal, 1E-10);
        }

        [TestMethod()]
        public void Test_Mean()
        {
            double val = Numerics.Data.Statistics.Statistics.Mean(sample1);
            double trueVal = 191.317391304348d;
            Assert.AreEqual(val, trueVal, 1E-10);
        }

        [TestMethod()]
        public void Test_GeometricMean()
        {
            double val = Numerics.Data.Statistics.Statistics.GeometricMean(sample1);
            double trueVal = 185.685629284673d;
            Assert.AreEqual(val, trueVal, 1E-10);
        }

        [TestMethod()]
        public void Test_HarmonicMean()
        {
            double val = Numerics.Data.Statistics.Statistics.HarmonicMean(sample1);
            double trueVal = 180.183870381546d;
            Assert.AreEqual(val, trueVal, 1E-10);
        }

        [TestMethod()]
        public void Test_Variance()
        {
            double val = Numerics.Data.Statistics.Statistics.Variance(sample1);
            double trueVal = 2300.31616368286d;
            Assert.AreEqual(val, trueVal, 1E-10);
        }

        [TestMethod()]
        public void Test_PopulationVariance()
        {
            double val = Numerics.Data.Statistics.Statistics.PopulationVariance(sample1);
            double trueVal = 2266.97824826717d;
            Assert.AreEqual(val, trueVal, 1E-10);
        }

        [TestMethod()]
        public void Test_StandardDeviation()
        {
            double val = Numerics.Data.Statistics.Statistics.StandardDeviation(sample1);
            double trueVal = 47.9616113541118d;
            Assert.AreEqual(val, trueVal, 1E-10);
        }

        [TestMethod()]
        public void Test_PopulationStandardDeviation()
        {
            double val = Numerics.Data.Statistics.Statistics.PopulationStandardDeviation(sample1);
            double trueVal = 47.6127950058298d;
            Assert.AreEqual(val, trueVal, 1E-10);
        }

        [TestMethod()]
        public void Test_Skewness()
        {
            double val = Numerics.Data.Statistics.Statistics.Skewness(sample1);
            double trueVal = 0.8605451107461d;
            Assert.AreEqual(val, trueVal, 1E-10);
        }

        [TestMethod()]
        public void Test_Kurtsosis()
        {
            double val = Numerics.Data.Statistics.Statistics.Kurtosis(sample1);
            double trueVal = 1.3434868130194d;
            Assert.AreEqual(val, trueVal, 1E-10);
        }

        [TestMethod()]
        public void Test_Covariance()
        {
            double val = Numerics.Data.Statistics.Statistics.Covariance(sample1, sample2);
            double trueVal = -253.54405370844d;
            Assert.AreEqual(val, trueVal, 1E-10);
        }

        [TestMethod()]
        public void Test_PopulationCovariance()
        {
            double val = Numerics.Data.Statistics.Statistics.PopulationCovariance(sample1, sample2);
            double trueVal = -249.869502205419d;
            Assert.AreEqual(val, trueVal, 1E-10);
        }

        [TestMethod()]
        public void Test_ComputeProductMoments()
        {
            var vals = Numerics.Data.Statistics.Statistics.ProductMoments(sample1);
            double trueVal1 = 191.317391304348d;
            double trueVal2 = 47.9616113541118d;
            double trueVal3 = 0.8605451107461d;
            double trueVal4 = 1.3434868130194d;
            Assert.AreEqual(vals[0], trueVal1, 1E-10);
            Assert.AreEqual(vals[1], trueVal2, 1E-10);
            Assert.AreEqual(vals[2], trueVal3, 1E-10);
            Assert.AreEqual(vals[3], trueVal4, 1E-10);
        }

        [TestMethod()]
        public void Test_ComputeLinearMoments()
        {
            var data = new double[] { 1953d, 1939d, 1677d, 1692d, 2051d, 2371d, 2022d, 1521d, 1448d, 1825d, 1363d, 1760d, 1672d, 1603d, 1244d, 1521d, 1783d, 1560d, 1357d, 1673d, 1625d, 1425d, 1688d, 1577d, 1736d, 1640d, 1584d, 1293d, 1277d, 1742d, 1491d };
            var lmoms = Numerics.Data.Statistics.Statistics.LinearMoments(data);
            double trueVal1 = 1648.8064516d;
            double trueVal2 = 138.2365591d;
            double trueVal3 = 0.1033903d;
            double trueVal4 = 0.1940943d;
            Assert.AreEqual(lmoms[0], trueVal1, 1E-7);
            Assert.AreEqual(lmoms[1], trueVal2, 1E-7);
            Assert.AreEqual(lmoms[2], trueVal3, 1E-7);
            Assert.AreEqual(lmoms[3], trueVal4, 1E-7);
        }

        [TestMethod()]
        public void Test_Percentiles()
        {
            double val0 = Numerics.Data.Statistics.Statistics.Percentile(sample1, 0);
            double trueVal0 = 98.8d;
            Assert.AreEqual(val0, trueVal0, 1E-2);
            double val1 = Numerics.Data.Statistics.Statistics.Percentile(sample1, 0.01d);
            double trueVal1 = 99.004d;
            Assert.AreEqual(val1, trueVal1, 1E-2);
            double val2 = Numerics.Data.Statistics.Statistics.Percentile(sample1, 0.05d);
            double trueVal2 = 123.2d;
            Assert.AreEqual(val2, trueVal2, 1E-2);
            double val3 = Numerics.Data.Statistics.Statistics.Percentile(sample1, 0.25d);
            double trueVal3 = 164.0d;
            Assert.AreEqual(val3, trueVal3, 1E-2);
            double val4 = Numerics.Data.Statistics.Statistics.Percentile(sample1, 0.5d);
            double trueVal4 = 183.0d;
            Assert.AreEqual(val4, trueVal4, 1E-2);
            double val5 = Numerics.Data.Statistics.Statistics.Percentile(sample1, 0.75d);
            double trueVal5 = 216.0d;
            Assert.AreEqual(val5, trueVal5, 1E-2);
            double val6 = Numerics.Data.Statistics.Statistics.Percentile(sample1, 0.95d);
            double trueVal6 = 262.6d;
            Assert.AreEqual(val6, trueVal6, 1E-2);
            double val7 = Numerics.Data.Statistics.Statistics.Percentile(sample1, 0.99d);
            double trueVal7 = 332.92d;
            Assert.AreEqual(val7, trueVal7, 1E-2);
            double val8 = Numerics.Data.Statistics.Statistics.Percentile(sample1, 1);
            double trueVal8 = 337d;
            Assert.AreEqual(val8, trueVal8, 1E-2);
        }

        [TestMethod()]
        public void Test_JackKnifeStandardError()
        {
            double val = Numerics.Data.Statistics.Statistics.JackKnifeStandardError(jackKnifeSample, Numerics.Data.Statistics.Statistics.Mean);
            double trueVal = 0.0372182162668589d;
            Assert.AreEqual(val, trueVal, 1E-10);
        }

    }
}
