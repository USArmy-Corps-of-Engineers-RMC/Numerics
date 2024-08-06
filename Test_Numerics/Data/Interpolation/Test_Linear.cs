using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Data;
using System.Diagnostics;

namespace Data.Interpolation
{
    [TestClass]
    public class Test_Linear
    {

        [TestMethod()]
        public void Test_Sequential()
        {
            var values = new double[1000];
            for (int i = 1; i <= 1000; i++)
                values[i - 1] = i;
            var LI = new Linear(values, values);
            var lo = LI.SequentialSearch(872.5d);
            Assert.AreEqual(lo, 871);

            Array.Reverse(values);
            LI = new Linear(values, values, SortOrder.Descending);
            lo = LI.SequentialSearch(872.5);
            Assert.AreEqual(lo, 127);
        }

        [TestMethod]
        public void Test_Bisection()
        {
            var values = new double[1000];
            for (int i = 1; i <= 1000; i++)
                values[i - 1] = i;
            var LI = new Linear(values, values);
            var lo = LI.BisectionSearch(872.5d);
            Assert.AreEqual(lo, 871);

            Array.Reverse(values);
            LI = new Linear(values, values, SortOrder.Descending);
            lo = LI.BisectionSearch(872.5);
            Assert.AreEqual(lo, 127);
        }

        [TestMethod]
        public void Test_Hunt()
        {
            var values = new double[1000];
            for (int i = 1; i <= 1000; i++)
                values[i - 1] = i;
            var LI = new Linear(values, values);
            var lo = LI.HuntSearch(872.5d);
            Assert.AreEqual(lo, 871);

            Array.Reverse(values);
            LI = new Linear(values, values, SortOrder.Descending);
            lo = LI.HuntSearch(872.5);
            Assert.AreEqual(lo, 127);
        }

        [TestMethod()]
        public void Test_Lin()
        {
            var XArray = new double[] { 50d, 100d, 150d, 200d, 250d };
            var YArray = new double[] { 100d, 200d, 300d, 400d, 500d };
            var LI = new Linear(XArray, YArray);
            double X = 75d;
            double Y = LI.Interpolate(X);
            Assert.AreEqual(Y, 150.0d, 1E-6);
        }

        /// <summary>
        /// Test with R approx function
        /// </summary>
        [TestMethod()]
        public void Test_Lin_R()
        {
            var XArray = new double[] { 50, 100, 150, 200, 250, 300 };
            var YArray = new double[] { 0.001, 0.01, 0.1, 0.7, 0.95, 0.99 };
            var LI = new Linear(XArray, YArray);
            var Xout = new double[] { 76, 80, 96, 162, 170, 216 };
            var trueYout = new double[] { 0.00568, 0.0064, 0.00928, 0.244, 0.34, 0.78 };
            for (int i = 0; i < Xout.Length; i++)
            {
                double Y = LI.Interpolate(Xout[i]);
                Assert.AreEqual(trueYout[i], Y, 1E-6);
            }
        }

        [TestMethod()]
        public void Test_LinLog()
        {
            var XArray = new double[] { 50d, 100d, 150d, 200d, 250d };
            var YArray = new double[] { 100d, 200d, 300d, 400d, 500d };
            var LI = new Linear(XArray, YArray) { YTransform = Transform.Logarithmic };
            double X = 75d;
            double Y = LI.Interpolate(X);
            Assert.AreEqual(Y, 141.42135623731d, 1E-6);
        }

        /// <summary>
        /// Test with R approx function
        /// </summary>
        [TestMethod()]
        public void Test_LinLog_R()
        {
            var XArray = new double[] { 50, 100, 150, 200, 250, 300 };
            var YArray = new double[] { 0.001, 0.01, 0.1, 0.7, 0.95, 0.99 };
            var LI = new Linear(XArray, YArray) { YTransform = Transform.Logarithmic };
            var Xout = new double[] { 76, 80, 96, 162, 170, 216 };
            var trueYout = new double[] { 0.003311311, 0.003981072, 0.008317638, 0.159523081, 0.217790642, 0.771859442 };
            for (int i = 0; i < Xout.Length; i++)
            {
                double Y = LI.Interpolate(Xout[i]);
                Assert.AreEqual(trueYout[i], Y, 1E-6);
            }
        }

        [TestMethod()]
        public void Test_LogLin()
        {
            var XArray = new double[] { 50d, 100d, 150d, 200d, 250d };
            var YArray = new double[] { 100d, 200d, 300d, 400d, 500d };
            var LI = new Linear(XArray, YArray) { XTransform = Transform.Logarithmic };
            double X = 75d;
            double Y = LI.Interpolate(X);
            Assert.AreEqual(Y, 158.496250072116d, 1E-6);
        }


        [TestMethod()]
        public void Test_LogLog()
        {
            var XArray = new double[] { 50d, 100d, 150d, 200d, 250d };
            var YArray = new double[] { 100d, 200d, 300d, 400d, 500d };
            var LI = new Linear(XArray, YArray) { XTransform = Transform.Logarithmic, YTransform = Transform.Logarithmic };
            double X = 75d;
            double Y = LI.Interpolate(X);
            Assert.AreEqual(Y, 150.0d, 1E-6);
        }

        [TestMethod()]
        public void Test_LinZ()
        {
            var XArray = new double[] { 0.05d, 0.1d, 0.15d, 0.2d, 0.25d };
            var YArray = new double[] { 0.1d, 0.2d, 0.3d, 0.4d, 0.5d };
            var LI = new Linear(XArray, YArray) { YTransform = Transform.NormalZ };
            double X = 0.18d;
            double Y = LI.Interpolate(X);
            Assert.AreEqual(Y, 0.358762529d, 1E-6);
        }

        /// <summary>
        /// Test with R approx function
        /// </summary>
        [TestMethod()]
        public void Test_LinZ_R()
        {
            var XArray = new double[] { 50, 100, 150, 200, 250, 300 };
            var YArray = new double[] { 0.001, 0.01, 0.1, 0.7, 0.95, 0.99 };
            var LI = new Linear(XArray, YArray) { YTransform = Transform.NormalZ };
            var Xout = new double[] { 76, 80, 96, 162, 170, 216 };
            var trueYout = new double[] { 0.003540482, 0.004245422, 0.008482656, 0.198184718, 0.288022602, 0.811367143 };
            for (int i = 0; i < Xout.Length; i++)
            {
                double Y = LI.Interpolate(Xout[i]);
                Assert.AreEqual(trueYout[i], Y, 1E-6);
            }
        }

        [TestMethod()]
        public void Test_ZLin()
        {
            var XArray = new double[] { 0.05d, 0.1d, 0.15d, 0.2d, 0.25d };
            var YArray = new double[] { 0.1d, 0.2d, 0.3d, 0.4d, 0.5d };
            var LI = new Linear(XArray, YArray) { XTransform = Transform.NormalZ };
            double X = 0.18d;
            double Y = LI.Interpolate(X);
            Assert.AreEqual(Y, 0.362146174d, 1E-6);
        }

        [TestMethod()]
        public void Test_ZZ()
        {
            var XArray = new double[] { 0.05d, 0.1d, 0.15d, 0.2d, 0.25d };
            var YArray = new double[] { 0.1d, 0.2d, 0.3d, 0.4d, 0.5d };
            var LI = new Linear(XArray, YArray) { XTransform = Transform.NormalZ, YTransform = Transform.NormalZ };
            double X = 0.18d;
            double Y = LI.Interpolate(X);
            Assert.AreEqual(Y, 0.36093855992815d, 1E-6);
        }

        [TestMethod()]
        public void Test_RevLinear()
        {
            var XArray = new double[] { 50d, 100d, 150d, 200d, 250d };
            var YArray = new double[] { 100d, 200d, 300d, 400d, 500d };         
            Array.Reverse(XArray);
            Array.Reverse(YArray);
            var LI = new Linear(XArray, YArray, SortOrder.Descending);
            double X = 75d;
            double Y = LI.Interpolate(X);
            Assert.AreEqual(Y, 150.0d, 1E-6);
        }

        [TestMethod()]
        public void Test_RevLinLog()
        {
            var XArray = new double[] { 50d, 100d, 150d, 200d, 250d };
            var YArray = new double[] { 100d, 200d, 300d, 400d, 500d };
            Array.Reverse(XArray);
            Array.Reverse(YArray);
            var LI = new Linear(XArray, YArray, SortOrder.Descending) { YTransform = Transform.Logarithmic };
            double X = 75d;
            double Y = LI.Interpolate(X);
            Assert.AreEqual(Y, 141.42135623731d, 1E-6);
        }

        [TestMethod()]
        public void Test_RevLogLin()
        {
            var XArray = new double[] { 50d, 100d, 150d, 200d, 250d };
            var YArray = new double[] { 100d, 200d, 300d, 400d, 500d };
            Array.Reverse(XArray);
            Array.Reverse(YArray);
            var LI = new Linear(XArray, YArray, SortOrder.Descending) { XTransform = Transform.Logarithmic };
            double X = 75d;
            double Y = LI.Interpolate(X);
            Assert.AreEqual(Y, 158.496250072116d, 1E-6);
        }

        [TestMethod()]
        public void Test_RevLogLog()
        {
            var XArray = new double[] { 50d, 100d, 150d, 200d, 250d };
            var YArray = new double[] { 100d, 200d, 300d, 400d, 500d };
            Array.Reverse(XArray);
            Array.Reverse(YArray);
            var LI = new Linear(XArray, YArray, SortOrder.Descending) { XTransform = Transform.Logarithmic, YTransform = Transform.Logarithmic };
            double X = 75d;
            double Y = LI.Interpolate(X);
            Assert.AreEqual(Y, 150.0d, 1E-6);
        }

        [TestMethod()]
        public void Test_RevLinZ()
        {
            var XArray = new double[] { 0.05d, 0.1d, 0.15d, 0.2d, 0.25d };
            var YArray = new double[] { 0.1d, 0.2d, 0.3d, 0.4d, 0.5d };         
            Array.Reverse(XArray);
            Array.Reverse(YArray);
            var LI = new Linear(XArray, YArray, SortOrder.Descending) { YTransform = Transform.NormalZ };
            double X = 0.18d;
            double Y = LI.Interpolate(X);
            Assert.AreEqual(Y, 0.358762529d, 1E-6);
        }

        [TestMethod()]
        public void Test_RevZLin()
        {
            var XArray = new double[] { 0.05d, 0.1d, 0.15d, 0.2d, 0.25d };
            var YArray = new double[] { 0.1d, 0.2d, 0.3d, 0.4d, 0.5d };
            Array.Reverse(XArray);
            Array.Reverse(YArray);
            var LI = new Linear(XArray, YArray, SortOrder.Descending) { XTransform = Transform.NormalZ };
            double X = 0.18d;
            double Y = LI.Interpolate(X);
            Assert.AreEqual(Y, 0.362146174d, 1E-6);
        }

        [TestMethod()]
        public void Test_RevZZ()
        {
            var XArray = new double[] { 0.05d, 0.1d, 0.15d, 0.2d, 0.25d };
            var YArray = new double[] { 0.1d, 0.2d, 0.3d, 0.4d, 0.5d };
            Array.Reverse(XArray);
            Array.Reverse(YArray);
            var LI = new Linear(XArray, YArray, SortOrder.Descending) { XTransform = Transform.NormalZ, YTransform = Transform.NormalZ };
            double X = 0.18d;
            double Y = LI.Interpolate(X);
            Assert.AreEqual(Y, 0.36093855992815d, 1E-6);
        }


        [TestMethod()]
        public void Test_Lin_List()
        {
            var XArray = new double[] { 50d, 100d, 150d, 200d, 250d };
            var YArray = new double[] { 100d, 200d, 300d, 400d, 500d };
            var LI = new Linear(XArray, YArray) { UseSmartSearch = true };

            int N = 10;
            double delta = (260 - 40) / ((double)N - 1);
            var xVals = new double[N];
            xVals[0] = 40;
            for (int i = 1; i < N; i++)
                xVals[i] = xVals[i - 1] + delta;

            var yVals = LI.Interpolate(xVals);
            var trueVals = new double[] { 100, 128.888888888889, 177.777777777778, 226.666666666667, 275.555555555556, 324.444444444444, 373.333333333333, 422.222222222222, 471.111111111111, 500 };
            for (int i = 1; i < N; i++)
                Assert.AreEqual(yVals[i], trueVals[i], 1E-6);
        }


    }
}
