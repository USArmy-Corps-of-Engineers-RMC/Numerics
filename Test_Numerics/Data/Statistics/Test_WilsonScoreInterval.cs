using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Distributions;
using Stats = Numerics.Data.Statistics.Statistics;

namespace Data.Statistics
{
    /// <summary>
    /// Tests for <see cref="Numerics.Data.Statistics.Statistics.WilsonScoreInterval(double, double, double)"/>.
    /// </summary>
    [TestClass]
    public class Test_WilsonScoreInterval
    {
        /// <summary>
        /// Known values from SciPy 1.18 (scipy.stats.binomtest(s, n).proportion_ci(level, method="wilson")),
        /// including the clamped endpoints at zero and n successes.
        /// </summary>
        [TestMethod]
        public void KnownValues()
        {
            var cases = new (double S, double N, double Level, double Lower, double Upper)[]
            {
                (5, 20, 0.9, 0.12737716852035774, 0.4322017501601245),
                (0, 10, 0.95, 0.0, 0.27753279986288926),
                (10, 10, 0.95, 0.7224672001371109, 1.0),
                (37, 120, 0.99, 0.21219370709039861, 0.42455728939891313),
            };
            foreach (var c in cases)
            {
                var (lower, upper) = Stats.WilsonScoreInterval(c.S, c.N, c.Level);
                Assert.AreEqual(c.Lower, lower, 1E-12, $"{c.S}/{c.N} at {c.Level}");
                Assert.AreEqual(c.Upper, upper, 1E-12, $"{c.S}/{c.N} at {c.Level}");
            }
        }

        /// <summary>
        /// The operation order is the documented one: z from Normal(0, 1).InverseCDF(0.5 + level / 2) and z²/n
        /// formed once, so the bounds are bitwise those of that expression, including fractional counts.
        /// </summary>
        [TestMethod]
        public void OperationOrderIsTheDocumentedOne()
        {
            foreach (var (s, n, level) in new[] { (3.0, 41.0, 0.9), (12.5, 37.25, 0.8), (0.0, 1.0, 0.5), (41.0, 41.0, 0.95) })
            {
                double z = new Normal(0.0, 1.0).InverseCDF(0.5 + level / 2.0);
                double pHat = s / n;
                double zSquaredOverN = z * z / n;
                double denominator = 1.0 + zSquaredOverN;
                double center = (pHat + zSquaredOverN / 2.0) / denominator;
                double halfWidth = z * Math.Sqrt(pHat * (1.0 - pHat) / n + zSquaredOverN / (4.0 * n)) / denominator;
                var (lower, upper) = Stats.WilsonScoreInterval(s, n, level);
                Assert.AreEqual(BitConverter.DoubleToInt64Bits(Math.Max(0.0, center - halfWidth)), BitConverter.DoubleToInt64Bits(lower));
                Assert.AreEqual(BitConverter.DoubleToInt64Bits(Math.Min(1.0, center + halfWidth)), BitConverter.DoubleToInt64Bits(upper));
                Assert.IsLessThanOrEqualTo(upper, lower);
            }
        }

        /// <summary>Counts and levels outside their domains are refused.</summary>
        [TestMethod]
        public void InvalidArgumentsThrow()
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Stats.WilsonScoreInterval(1, 0, 0.9));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Stats.WilsonScoreInterval(1, double.PositiveInfinity, 0.9));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Stats.WilsonScoreInterval(-1, 10, 0.9));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Stats.WilsonScoreInterval(11, 10, 0.9));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Stats.WilsonScoreInterval(double.NaN, 10, 0.9));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Stats.WilsonScoreInterval(1, 10, 0.0));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Stats.WilsonScoreInterval(1, 10, 1.0));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Stats.WilsonScoreInterval(1, 10, double.NaN));
        }
    }
}
