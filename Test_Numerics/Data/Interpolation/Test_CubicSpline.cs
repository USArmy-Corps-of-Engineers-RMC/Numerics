using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Data;

namespace Data.Interpolation
{
    [TestClass]
    public class Test_CubicSpline
    {
        [TestMethod]
        public void Test_Interpolate()
        {
            var XArray = new double[] { 6d, 24d, 48d, 72d };
            var YArray = new double[] { 9.96d, 22.13d, 32.27d,37.60d };
            var spline = new CubicSpline(XArray, YArray);
            double X = 8d;
            double Y = spline.Interpolate(X);
            Assert.AreEqual(Y, 11.4049889205445d, 1E-6);
        }

        [TestMethod]
        public void Test_Interpolate_List()
        {
            var XArray = new double[] { 6d, 24d, 48d, 72d };
            var YArray = new double[] { 9.96d, 22.13d, 32.27d, 37.60d };
            var spline = new CubicSpline(XArray, YArray);
            var X = new double[34];
            X[0] = 6;
            for (int i = 1; i < X.Length; i++)
                X[i] = X[i - 1] + 2;

            var Y = spline.Interpolate(X);
            var true_Y = new double[] { 9.96, 11.4049889205445, 12.8430203387148, 14.2671367521368, 15.6703806584362, 17.045794555239, 18.3864209401709, 19.6853023108579, 20.9354811649256, 22.13, 23.2634521159782, 24.3366340218424, 25.3518930288462, 26.3115764482431, 27.2180315912868, 28.0736057692308, 28.8806462933286, 29.6415004748338, 30.358515625, 31.0340390550807, 31.6704180763295, 32.27, 32.8352193880579, 33.3688598053181, 33.8737920673077, 34.3528869895537, 34.8090153875831, 35.2450480769231, 35.6638558731007, 36.0683095916429, 36.4612800480769, 36.8456380579297, 37.2242544367284, 37.6};
            for (int i = 0; i < X.Length; i++)
                Assert.AreEqual(Y[i], true_Y[i], 1E-6);
        }

    }
}
