using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Data.Statistics;
using Stats = Numerics.Data.Statistics.Statistics;
using PPType = Numerics.Data.Statistics.PlottingPositions.PlottingPostionType;

namespace Data.Statistics
{
    /// <summary>
    /// Tests for <see cref="Numerics.Data.Statistics.Statistics.Percentile(System.Collections.Generic.IList{double}, double, PlottingPositions.PlottingPostionType, bool)"/>.
    /// </summary>
    [TestClass]
    public class Test_PlottingPositionPercentile
    {
        /// <summary>The sample {3, 1, 4, 1, 5, 9, 2, 6}; sorted {1, 1, 2, 3, 4, 5, 6, 9}.</summary>
        private static readonly double[] Sample = { 3, 1, 4, 1, 5, 9, 2, 6 };

        /// <summary>
        /// Known values of R quantile(x, k, type = 6, 5, 4) for Weibull, Hazen and Kaplan-Meier, including the
        /// held ends.
        /// </summary>
        [TestMethod]
        public void KnownValuesMatchHyndmanFanTypes()
        {
            double[] k = { 0.0, 0.1, 0.25, 0.5, 0.75, 0.9, 1.0 };
            double[] type6 = { 1.0, 1.0, 1.25, 3.5, 5.75, 9.0, 9.0 };
            double[] type5 = { 1.0, 1.0, 1.5, 3.5, 5.5, 8.1, 9.0 };
            double[] type4 = { 1.0, 1.0, 1.0, 3.0, 5.0, 6.6, 9.0 };
            for (int i = 0; i < k.Length; i++)
            {
                Assert.AreEqual(type6[i], Stats.Percentile(Sample, k[i], PPType.Weibull), 1E-12, $"Weibull k = {k[i]}");
                Assert.AreEqual(type5[i], Stats.Percentile(Sample, k[i], PPType.Hazen), 1E-12, $"Hazen k = {k[i]}");
                Assert.AreEqual(type4[i], Stats.Percentile(Sample, k[i], PPType.KaplanMeier), 1E-12, $"Kaplan-Meier k = {k[i]}");
            }
        }

        /// <summary>
        /// At each plotting position the percentile returns the order statistic, for every formula, and the array
        /// overload equals the scalar one bit for bit with sorted or unsorted data.
        /// </summary>
        [TestMethod]
        public void PlottingPositionsMapToOrderStatistics()
        {
            var sorted = (double[])Sample.Clone();
            Array.Sort(sorted);
            foreach (PPType type in Enum.GetValues(typeof(PPType)))
            {
                var positions = PlottingPositions.Function(Sample.Length, type)!;
                for (int i = 0; i < positions.Length; i++)
                    Assert.AreEqual(sorted[i], Stats.Percentile(Sample, positions[i], type), 1E-12, $"{type}, i = {i}");

                var k = new[] { 0.0, 0.05, 0.3, 0.5, 0.77, 0.99, 1.0 };
                var array = Stats.Percentile(Sample, k, type);
                var arraySorted = Stats.Percentile(sorted, k, type, true);
                for (int i = 0; i < k.Length; i++)
                {
                    long scalar = BitConverter.DoubleToInt64Bits(Stats.Percentile(Sample, k[i], type));
                    Assert.AreEqual(scalar, BitConverter.DoubleToInt64Bits(array[i]));
                    Assert.AreEqual(scalar, BitConverter.DoubleToInt64Bits(arraySorted[i]));
                }
            }
        }

        /// <summary>A single value is returned for every k; invalid inputs are refused.</summary>
        [TestMethod]
        public void EdgeCasesAndArgumentErrors()
        {
            Assert.AreEqual(7.0, Stats.Percentile(new[] { 7.0 }, 0.3, PPType.Weibull));
            Assert.IsEmpty(Stats.Percentile(Sample, new double[0], PPType.Weibull));

            Assert.ThrowsExactly<ArgumentNullException>(() => Stats.Percentile(null!, 0.5, PPType.Weibull));
            Assert.ThrowsExactly<ArgumentNullException>(() => Stats.Percentile(Sample, (double[])null!, PPType.Weibull));
            Assert.ThrowsExactly<ArgumentException>(() => Stats.Percentile(new double[0], 0.5, PPType.Weibull));
            Assert.ThrowsExactly<ArgumentException>(() => Stats.Percentile(new double[0], new[] { 0.5 }, PPType.Weibull));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Stats.Percentile(Sample, 1.5, PPType.Weibull));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Stats.Percentile(Sample, double.NaN, PPType.Hazen));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Stats.Percentile(Sample, 0.5, (PPType)99));
        }
    }
}
