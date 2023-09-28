using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Mathematics.RootFinding;

namespace Mathematics.RootFinding
{
    /// <summary>
    /// Test root finding methods. 
    /// </summary>
    [TestClass()]
    public class Test_RootFinding
    {

        /// <summary>
        /// Test function
        /// </summary>
        public double FX(double x)
        {
            double F = x * x * x - x - 1d;
            return F;
        }

        /// <summary>
        /// First derivative of test function.
        /// </summary>
        public double DFX(double x)
        {
            double F = 3d * (x * x) - 1d;
            return F;
        }

        /// <summary>
        /// Bisection method.
        /// </summary>
        [TestMethod()]
        public void Test_Bisection()
        {
            double initial = 1d;
            double lower = -1;
            double upper = 5d;
            double X = Bisection.Solve(FX, initial, lower, upper);
            double trueX = 1.32472d;
            Assert.AreEqual(X, trueX, 1E-4);
        }

        /// <summary>
        /// Brent method.
        /// </summary>
        [TestMethod()]
        public void Test_Brent()
        {
            double lower = -1;
            double upper = 5d;
            double X = Brent.Solve(FX, lower, upper);
            double trueX = 1.32472d;
            Assert.AreEqual(X, trueX, 1E-4);
        }

        /// <summary>
        /// Secant method.
        /// </summary>
        [TestMethod()]
        public void Test_Secant()
        {
            double lower = -1;
            double upper = 5d;
            double X = Secant.Solve(FX, lower, upper);
            double trueX = 1.32472d;
            Assert.AreEqual(X, trueX, 1E-4);
        }

        /// <summary>
        /// Newton-Raphson method.
        /// </summary>
        [TestMethod()]
        public void Test_NewtonRaphson()
        {
            double initial = 1d;
            double X = NewtonRaphson.Solve(FX, DFX, initial);
            double trueX = 1.32472d;
            Assert.AreEqual(X, trueX, 1E-4);
        }

        /// <summary>
        /// Robust Newton-Raphson, falls back to bisection.
        /// </summary>
        [TestMethod()]
        public void Test_RobustNewtonRaphson()
        {
            double initial = 1d;
            double lower = -1;
            double upper = 5d;
            double X = NewtonRaphson.RobustSolve(FX, DFX, initial, lower, upper);
            double trueX = 1.32472d;
            Assert.AreEqual(X, trueX, 1E-4);
        }
    }
}