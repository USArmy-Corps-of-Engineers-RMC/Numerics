using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Data;

namespace Data.PairedData
{
    [TestClass]
    public class Test_PairedDataInterpolation
    {
        [TestMethod()]
        public void Test_Sequential()
        {
            // ASC
            var opd = new OrderedPairedData(true, SortOrder.Ascending, false, SortOrder.Ascending);
            for (int i = 1; i <= 1000; i++)
                opd.Add(new Ordinate(i, i));
            // X
            var lo = opd.SequentialSearchX(872.5d);
            Assert.AreEqual(lo, 871);
            // Y
            lo = opd.SequentialSearchY(872.5d);
            Assert.AreEqual(lo, 871);

            // DSC
            opd = new OrderedPairedData(true, SortOrder.Descending, false, SortOrder.Descending);
            for (int i = 1000; i >= 1; i--)
                opd.Add(new Ordinate(i, i));
            // X
            lo = opd.SequentialSearchX(872.5d);
            Assert.AreEqual(lo, 127);
            // Y
            lo = opd.SequentialSearchY(872.5d);
            Assert.AreEqual(lo, 127);

        }

        [TestMethod()]
        public void Test_Bisection()
        {
            // ASC
            var opd = new OrderedPairedData(true, SortOrder.Ascending, false, SortOrder.Ascending);
            for (int i = 1; i <= 1000; i++)
                opd.Add(new Ordinate(i, i));
            // X
            var lo = opd.BisectionSearchX(872.5d);
            Assert.AreEqual(lo, 871);
            // Y
            lo = opd.BisectionSearchY(872.5d);
            Assert.AreEqual(lo, 871);

            // DSC
            opd = new OrderedPairedData(true, SortOrder.Descending, false, SortOrder.Descending);
            for (int i = 1000; i >= 1; i--)
                opd.Add(new Ordinate(i, i));
            // X
            lo = opd.BisectionSearchX(872.5d);
            Assert.AreEqual(lo, 127);
            // Y
            lo = opd.BisectionSearchY(872.5d);
            Assert.AreEqual(lo, 127);

        }

        [TestMethod()]
        public void Test_Hunt()
        {
            // ASC
            var opd = new OrderedPairedData(true, SortOrder.Ascending, false, SortOrder.Ascending);
            for (int i = 1; i <= 1000; i++)
                opd.Add(new Ordinate(i, i));
            // X
            var lo = opd.HuntSearchX(872.5d);
            Assert.AreEqual(lo, 871);
            // Y
            lo = opd.HuntSearchY(872.5d);
            Assert.AreEqual(lo, 871);

            // DSC
            opd = new OrderedPairedData(true, SortOrder.Descending, false, SortOrder.Descending);
            for (int i = 1000; i >= 1; i--)
                opd.Add(new Ordinate(i, i));
            // X
            lo = opd.HuntSearchX(872.5d);
            Assert.AreEqual(lo, 127);
            // Y
            lo = opd.HuntSearchY(872.5d);
            Assert.AreEqual(lo, 127);

        }

        [TestMethod()]
        public void Test_Lin()
        {
            var XArray = new double[] { 50d, 100d, 150d, 200d, 250d };
            var YArray = new double[] { 100d, 200d, 300d, 400d, 500d };

            // Given X
            var opd = new OrderedPairedData(XArray, YArray, true, SortOrder.Ascending, true, SortOrder.Ascending);
            double X = 75d;
            double Y = opd.Interpolate(X);
            Assert.AreEqual(Y, 150.0d, 1E-6);

            // Given Y
            opd = new OrderedPairedData(YArray, XArray, true, SortOrder.Ascending, true, SortOrder.Ascending);
            Y = 75d;
            X = opd.Interpolate(X, false);
            Assert.AreEqual(X, 150.0d, 1E-6);

        }

        [TestMethod()]
        public void Test_LinLog()
        {
            var XArray = new double[] { 50d, 100d, 150d, 200d, 250d };
            var YArray = new double[] { 100d, 200d, 300d, 400d, 500d };

            // Given X
            var opd = new OrderedPairedData(XArray, YArray, true, SortOrder.Ascending, true, SortOrder.Ascending);
            double X = 75d;
            double Y = opd.Interpolate(X, yTransform: Transform.Logarithmic);
            Assert.AreEqual(Y, 141.42135623731d, 1E-6);

            // Given Y
            opd = new OrderedPairedData(YArray, XArray, true, SortOrder.Ascending, true, SortOrder.Ascending);
            Y = 75d;
            X = opd.Interpolate(X, false, xTransform: Transform.Logarithmic);
            Assert.AreEqual(X, 141.42135623731d, 1E-6);
        }

        [TestMethod()]
        public void Test_LogLin()
        {
            var XArray = new double[] { 50d, 100d, 150d, 200d, 250d };
            var YArray = new double[] { 100d, 200d, 300d, 400d, 500d };

            // Given X
            var opd = new OrderedPairedData(XArray, YArray, true, SortOrder.Ascending, true, SortOrder.Ascending);
            double X = 75d;
            double Y = opd.Interpolate(X, xTransform: Transform.Logarithmic);
            Assert.AreEqual(Y, 158.496250072116d, 1E-6);

            // Given Y
            opd = new OrderedPairedData(YArray, XArray, true, SortOrder.Ascending, true, SortOrder.Ascending);
            Y = 75d;
            X = opd.Interpolate(X, false, yTransform: Transform.Logarithmic);
            Assert.AreEqual(X, 158.496250072116d, 1E-6);
        }

        [TestMethod()]
        public void Test_LogLog()
        {
            var XArray = new double[] { 50d, 100d, 150d, 200d, 250d };
            var YArray = new double[] { 100d, 200d, 300d, 400d, 500d };

            // Given X
            var opd = new OrderedPairedData(XArray, YArray, true, SortOrder.Ascending, true, SortOrder.Ascending);
            double X = 75d;
            double Y = opd.Interpolate(X, xTransform: Transform.Logarithmic, yTransform: Transform.Logarithmic);
            Assert.AreEqual(Y, 150.0d, 1E-6);

            // Given Y
            opd = new OrderedPairedData(YArray, XArray, true, SortOrder.Ascending, true, SortOrder.Ascending);
            Y = 75d;
            X = opd.Interpolate(X, false, xTransform: Transform.Logarithmic, yTransform: Transform.Logarithmic);
            Assert.AreEqual(X, 150.0d, 1E-6);
        }

        [TestMethod()]
        public void Test_LinZ()
        {
            var XArray = new double[] { 0.05d, 0.1d, 0.15d, 0.2d, 0.25d };
            var YArray = new double[] { 0.1d, 0.2d, 0.3d, 0.4d, 0.5d };

            // Given X
            var opd = new OrderedPairedData(XArray, YArray, true, SortOrder.Ascending, true, SortOrder.Ascending);
            double X = 0.18d;
            double Y = opd.Interpolate(X, yTransform: Transform.NormalZ);
            Assert.AreEqual(Y, 0.358762529d, 1E-6);

            // Given Y
            opd = new OrderedPairedData(YArray, XArray, true, SortOrder.Ascending, true, SortOrder.Ascending);
            Y = 0.18d;
            X = opd.Interpolate(X, false, xTransform: Transform.NormalZ);
            Assert.AreEqual(X, 0.358762529d, 1E-6);
        }

        [TestMethod()]
        public void Test_ZLin()
        {
            var XArray = new double[] { 0.05d, 0.1d, 0.15d, 0.2d, 0.25d };
            var YArray = new double[] { 0.1d, 0.2d, 0.3d, 0.4d, 0.5d };

            // Given X
            var opd = new OrderedPairedData(XArray, YArray, true, SortOrder.Ascending, true, SortOrder.Ascending);
            double X = 0.18d;
            double Y = opd.Interpolate(X, xTransform: Transform.NormalZ);
            Assert.AreEqual(Y, 0.362146174d, 1E-6);

            // Given Y
            opd = new OrderedPairedData(YArray, XArray, true, SortOrder.Ascending, true, SortOrder.Ascending);
            Y = 0.18d;
            X = opd.Interpolate(X, false, yTransform: Transform.NormalZ);
            Assert.AreEqual(X, 0.362146174d, 1E-6);
        }

        [TestMethod()]
        public void Test_ZZ()
        {
            var XArray = new double[] { 0.05d, 0.1d, 0.15d, 0.2d, 0.25d };
            var YArray = new double[] { 0.1d, 0.2d, 0.3d, 0.4d, 0.5d };

            // Given X
            var opd = new OrderedPairedData(XArray, YArray, true, SortOrder.Ascending, true, SortOrder.Ascending);
            double X = 0.18d;
            double Y = opd.Interpolate(X, xTransform: Transform.NormalZ, yTransform: Transform.NormalZ);
            Assert.AreEqual(Y, 0.36093855992815d, 1E-6);

            // Given Y
            opd = new OrderedPairedData(YArray, XArray, true, SortOrder.Ascending, true, SortOrder.Ascending);
            Y = 0.18d;
            X = opd.Interpolate(X, false, xTransform: Transform.NormalZ, yTransform: Transform.NormalZ);
            Assert.AreEqual(X, 0.36093855992815d, 1E-6);
        }

        [TestMethod()]
        public void Test_RevLinear()
        {
            var XArray = new double[] { 50d, 100d, 150d, 200d, 250d };
            var YArray = new double[] { 100d, 200d, 300d, 400d, 500d };
            Array.Reverse(XArray);
            Array.Reverse(YArray);

            // Given X
            var opd = new OrderedPairedData(XArray, YArray, true, SortOrder.Descending, true, SortOrder.Descending);
            double X = 75d;
            double Y = opd.Interpolate(X);
            Assert.AreEqual(Y, 150.0d, 1E-6);

            // Given Y
            opd = new OrderedPairedData(YArray, XArray, true, SortOrder.Descending, true, SortOrder.Descending);
            Y = 75d;
            X = opd.Interpolate(X, false);
            Assert.AreEqual(X, 150.0d, 1E-6);
        }

        [TestMethod()]
        public void Test_RevLinLog()
        {
            var XArray = new double[] { 50d, 100d, 150d, 200d, 250d };
            var YArray = new double[] { 100d, 200d, 300d, 400d, 500d };
            Array.Reverse(XArray);
            Array.Reverse(YArray);

            // Given X
            var opd = new OrderedPairedData(XArray, YArray, true, SortOrder.Descending, true, SortOrder.Descending);
            double X = 75d;
            double Y = opd.Interpolate(X, yTransform: Transform.Logarithmic);
            Assert.AreEqual(Y, 141.42135623731d, 1E-6);

            // Given Y
            opd = new OrderedPairedData(YArray, XArray, true, SortOrder.Descending, true, SortOrder.Descending);
            Y = 75d;
            X = opd.Interpolate(X, false, xTransform: Transform.Logarithmic);
            Assert.AreEqual(X, 141.42135623731d, 1E-6);
        }

        [TestMethod()]
        public void Test_RevLogLin()
        {
            var XArray = new double[] { 50d, 100d, 150d, 200d, 250d };
            var YArray = new double[] { 100d, 200d, 300d, 400d, 500d };
            Array.Reverse(XArray);
            Array.Reverse(YArray);

            // Given X
            var opd = new OrderedPairedData(XArray, YArray, true, SortOrder.Descending, true, SortOrder.Descending);
            double X = 75d;
            double Y = opd.Interpolate(X, xTransform: Transform.Logarithmic);
            Assert.AreEqual(Y, 158.496250072116d, 1E-6);

            // Given Y
            opd = new OrderedPairedData(YArray, XArray, true, SortOrder.Descending, true, SortOrder.Descending);
            Y = 75d;
            X = opd.Interpolate(X, false, yTransform: Transform.Logarithmic);
            Assert.AreEqual(X, 158.496250072116d, 1E-6);
        }

        [TestMethod()]
        public void Test_RevLogLog()
        {
            var XArray = new double[] { 50d, 100d, 150d, 200d, 250d };
            var YArray = new double[] { 100d, 200d, 300d, 400d, 500d };
            Array.Reverse(XArray);
            Array.Reverse(YArray);

            // Given X
            var opd = new OrderedPairedData(XArray, YArray, true, SortOrder.Descending, true, SortOrder.Descending);
            double X = 75d;
            double Y = opd.Interpolate(X, xTransform: Transform.Logarithmic, yTransform: Transform.Logarithmic);
            Assert.AreEqual(Y, 150.0d, 1E-6);

            // Given Y
            opd = new OrderedPairedData(YArray, XArray, true, SortOrder.Descending, true, SortOrder.Descending);
            Y = 75d;
            X = opd.Interpolate(X, false, xTransform: Transform.Logarithmic, yTransform: Transform.Logarithmic);
            Assert.AreEqual(X, 150.0d, 1E-6);
        }

        [TestMethod()]
        public void Test_RevLinZ()
        {
            var XArray = new double[] { 0.05d, 0.1d, 0.15d, 0.2d, 0.25d };
            var YArray = new double[] { 0.1d, 0.2d, 0.3d, 0.4d, 0.5d };
            Array.Reverse(XArray);
            Array.Reverse(YArray);

            // Given X
            var opd = new OrderedPairedData(XArray, YArray, true, SortOrder.Descending, true, SortOrder.Descending);
            double X = 0.18d;
            double Y = opd.Interpolate(X, yTransform: Transform.NormalZ);
            Assert.AreEqual(Y, 0.358762529d, 1E-6);

            // Given Y
            opd = new OrderedPairedData(YArray, XArray, true, SortOrder.Descending, true, SortOrder.Descending);
            Y = 0.18d;
            X = opd.Interpolate(X, false, xTransform: Transform.NormalZ);
            Assert.AreEqual(X, 0.358762529d, 1E-6);
        }

        [TestMethod()]
        public void Test_RevZLin()
        {
            var XArray = new double[] { 0.05d, 0.1d, 0.15d, 0.2d, 0.25d };
            var YArray = new double[] { 0.1d, 0.2d, 0.3d, 0.4d, 0.5d };
            Array.Reverse(XArray);
            Array.Reverse(YArray);

            // Given X
            var opd = new OrderedPairedData(XArray, YArray, true, SortOrder.Descending, true, SortOrder.Descending);
            double X = 0.18d;
            double Y = opd.Interpolate(X, xTransform: Transform.NormalZ);
            Assert.AreEqual(Y, 0.362146174d, 1E-6);

            // Given Y
            opd = new OrderedPairedData(YArray, XArray, true, SortOrder.Descending, true, SortOrder.Descending);
            Y = 0.18d;
            X = opd.Interpolate(X, false, yTransform: Transform.NormalZ);
            Assert.AreEqual(X, 0.362146174d, 1E-6);
        }

        [TestMethod()]
        public void Test_RevZZ()
        {
            var XArray = new double[] { 0.05d, 0.1d, 0.15d, 0.2d, 0.25d };
            var YArray = new double[] { 0.1d, 0.2d, 0.3d, 0.4d, 0.5d };
            Array.Reverse(XArray);
            Array.Reverse(YArray);

            // Given X
            var opd = new OrderedPairedData(XArray, YArray, true, SortOrder.Descending, true, SortOrder.Descending);
            double X = 0.18d;
            double Y = opd.Interpolate(X, xTransform: Transform.NormalZ, yTransform: Transform.NormalZ);
            Assert.AreEqual(Y, 0.36093855992815d, 1E-6);

            // Given Y
            opd = new OrderedPairedData(YArray, XArray, true, SortOrder.Descending, true, SortOrder.Descending);
            Y = 0.18d;
            X = opd.Interpolate(X, false, xTransform: Transform.NormalZ, yTransform: Transform.NormalZ);
            Assert.AreEqual(X, 0.36093855992815d, 1E-6);
        }

        [TestMethod()]
        public void Test_Lin_List()
        {
            var XArray = new double[] { 50d, 100d, 150d, 200d, 250d };
            var YArray = new double[] { 100d, 200d, 300d, 400d, 500d };

            // Given X
            var opd = new OrderedPairedData(XArray, YArray, true, SortOrder.Ascending, true, SortOrder.Ascending);
            double X = 75d;
            double Y = opd.Interpolate(X);
            Assert.AreEqual(Y, 150.0d, 1E-6);

            int N = 10;
            double delta = (260 - 40) / ((double)N - 1);
            var xVals = new double[N];
            xVals[0] = 40;
            for (int i = 1; i < N; i++)
                xVals[i] = xVals[i - 1] + delta;

            var yVals = opd.Interpolate(xVals);
            var trueVals = new double[] { 100, 128.888888888889, 177.777777777778, 226.666666666667, 275.555555555556, 324.444444444444, 373.333333333333, 422.222222222222, 471.111111111111, 500 };
            for (int i = 1; i < N; i++)
                Assert.AreEqual(yVals[i], trueVals[i], 1E-6);

            // Given Y
            opd = new OrderedPairedData(YArray, XArray, true, SortOrder.Ascending, true, SortOrder.Ascending);
            yVals = opd.Interpolate(xVals, false);
            for (int i = 1; i < N; i++)
                Assert.AreEqual(yVals[i], trueVals[i], 1E-6);
        }

    }
}
