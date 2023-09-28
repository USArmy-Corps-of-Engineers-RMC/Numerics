using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Data.Statistics;

namespace Data.Statistics
{
    [TestClass]
    public class Test_Historgram
    {

        [TestMethod()]
        public void Test_Histogram_Specified()
        {
            var sample1 = new double[] { 122d, 244d, 214d, 173d, 229d, 156d, 212d, 263d, 146d, 183d, 161d, 205d, 135d, 331d, 225d, 174d, 98.8d, 149d, 238d, 262d, 132d, 235d, 216d, 240d, 230d, 192d, 195d, 172d, 173d, 172d, 153d, 142d, 317d, 161d, 201d, 204d, 194d, 164d, 183d, 161d, 167d, 179d, 185d, 117d, 192d, 337d, 125d, 166d, 99.1d, 202d, 230d, 158d, 262d, 154d, 164d, 182d, 164d, 183d, 171d, 250d, 184d, 205d, 237d, 177d, 239d, 187d, 180d, 173d, 174d };

            // These values come from Microsoft Excel
            var lower = new double[] { 98.8d, 110.71d, 122.62d, 134.53d, 146.44d, 158.35d, 170.26d, 182.17d, 194.08d, 205.99d, 217.9d, 229.81d, 241.72d, 253.63d, 265.54d, 277.45d, 289.36d, 301.27d, 313.18d, 325.09d };
            var upper = new double[] { 110.71d, 122.62d, 134.53d, 146.44d, 158.35d, 170.26d, 182.17d, 194.08d, 205.99d, 217.9d, 229.81d, 241.72d, 253.63d, 265.54d, 277.45d, 289.36d, 301.27d, 313.18d, 325.09d, 337.0d };
            var frequency = new int[] { 2, 2, 2, 3, 5, 8, 12, 9, 6, 3, 2, 7, 2, 3, 0, 0, 0, 0, 1, 2 };
            var hist = new Histogram(sample1, 20);
            for (int i = 0; i < hist.NumberOfBins; i++)
            {
                Assert.AreEqual(hist[i].LowerBound, lower[i], 1E-3);
                Assert.AreEqual(hist[i].UpperBound, upper[i], 1E-3);
                Assert.AreEqual(hist[i].Frequency, frequency[i], 1E-3);
            }
        }
    }
}
