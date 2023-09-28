using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Mathematics;

namespace Mathematics.Differentiation
{
    [TestClass]
    public class Test_Differentiation
    {
        /// <summary>
        /// Test function
        /// </summary>
        public double FX(double x)
        {
            return Math.Pow(x, 3d);
        }

        public double FXY(double[] points)
        {
            double x = points[0];
            double y = points[1];
            return Math.Pow(x, 2) * Math.Pow(y, 3);
        }

        /// <summary>
        /// Test multi-parameter function
        /// </summary>
        public double FXYZ(double[] points)
        {
            double x = points[0];
            double y = points[1];
            double z = points[2];
            return Math.Pow(x, 3d) + Math.Pow(y, 4d) + Math.Pow(z, 5d);
        }

        /// <summary>
        /// Test multi-parameter function for Hessian
        /// </summary>
        public double FH(double[] points)
        {
            double x = points[0];
            double y = points[1];
            return Math.Pow(x, 3d) - 2 * x * y - Math.Pow(y, 6);
        }

        /// <summary>
        /// Test multi-parameter function for Jacobian
        /// </summary>
        public double FTXYZ(double t, double[] points)
        {
            double x = points[0];
            double y = points[1];
            double z = points[2];
            return t * Math.Pow(x, 3d) + Math.Pow(y, 4d) + Math.Pow(z, 5d);
        }

        /// <summary>
        /// Test derivative.
        /// </summary>
        [TestMethod()]
        public void Test_Derivative()
        {
            double deriv = NumericalDerivative.Derivative(FX, 2d);
            double ans = 12d;
            Assert.AreEqual(deriv, ans, 0.000001d);
        }

        /// <summary>
        /// Test gradient.
        /// </summary>
        [TestMethod()]
        public void Test_Gradient()
        {
            var deriv = NumericalDerivative.Gradient(FXY, new[] { 2d, 2d });
            double x = 2 * 2 * Math.Pow(2, 3);
            double y = 3 * Math.Pow(2, 2) * Math.Pow(2, 2);
            Assert.AreEqual(deriv[0], x, 0.000001d);
            Assert.AreEqual(deriv[1], y, 0.000001d);

            deriv = NumericalDerivative.Gradient(FXYZ, new[] { 2d, 2d, 2d });
            x = 3d * Math.Pow(2d, 2d);
            y = 4d * Math.Pow(2d, 3d);
            double z = 5d * Math.Pow(2d, 4d);
            Assert.AreEqual(deriv[0], x, 0.000001d);
            Assert.AreEqual(deriv[1], y, 0.000001d);
            Assert.AreEqual(deriv[2], z, 0.000001d);
        }


        /// <summary>
        /// Test Ridders' method.
        /// </summary>
        [TestMethod()]
        public void Test_RiddersMethod()
        {
            double deriv = NumericalDerivative.RiddersMethod(FX, 2d);
            double ans = 12d;
            Assert.AreEqual(deriv, ans, 0.000001d);
        }

        /// <summary>
        /// Test Jacobian.
        /// </summary>
        [TestMethod()]
        public void Test_Jacobian()
        {

            var jac = NumericalDerivative.Jacobian(FTXYZ, new[] { 4d, 6d, 8d}, new[] { 2d, 2d, 2d });
            var true_jac = new double[,] { { 12d * Math.Pow(2d, 2d), 4d * Math.Pow(2d, 3d), 5d * Math.Pow(2d, 4d) }, 
                                           { 18d * Math.Pow(2d, 2d), 4d * Math.Pow(2d, 3d), 5d * Math.Pow(2d, 4d) }, 
                                           { 24d * Math.Pow(2d, 2d), 4d * Math.Pow(2d, 3d), 5d * Math.Pow(2d, 4d) } };
            for (int i = 0; i < jac.GetLength(0); i++)
                for (int j = 0; j < jac.GetLength(1); j++)
                    Assert.AreEqual(jac[i, j], true_jac[i, j], 1E-4);

        }

        /// <summary>
        /// Test Hessian.
        /// </summary>
        [TestMethod()]
        public void Test_Hessian()
        {
            var hess = NumericalDerivative.Hessian(FH, new[] { 1d, 2d }, 1E-3);
            var true_hess = new double[,] { { 6, -2 }, { -2, -480 } };
            Assert.AreEqual(hess[0,0], true_hess[0, 0], 1E-3);
            Assert.AreEqual(hess[0,1], true_hess[0, 1], 1E-3);
            Assert.AreEqual(hess[1,0], true_hess[1, 0], 1E-3);
            Assert.AreEqual(hess[1,1], true_hess[1, 1], 1E-3);

            hess = NumericalDerivative.Hessian(FXYZ, new[] { 2d, 2d, 2d }, 1E-3);
            true_hess = new double[,] { { 12d, 0, 0 }, { 0, 48d, 0}, { 0, 0, 160d } };
            for (int i = 0; i < hess.GetLength(0); i++)
                for (int j = 0; j < hess.GetLength(1); j++)
                    Assert.AreEqual(hess[i, j], true_hess[i, j], 1E-3);

        }

    }
}
