using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Mathematics.RootFinding;
using System;

namespace Test_Numerics.Mathematics.Root_Finding
{
    [TestClass]
    public class Test_NewtonRaphson
    {
        /// <summary>
        /// Third degree polynomial
        /// </summary>
        public double Cubic_FX(double x)
        {
            double F = x * x * x - x - 1d;
            return F;
        }

        /// <summary>
        /// First derivative of third degree polynomial.
        /// </summary>
        public double Cubic_Deriv(double x)
        {
            double F = 3d * (x * x) - 1d;
            return F;
        }

        /// <summary>
        /// Exponential function.
        /// </summary>
        public double Exponential_FX(double x)
        {
            double F = Math.Exp(-x) - x;
            return F;
        }

        /// <summary>
        /// First derivative of Exponential function.
        /// </summary>
        public double Exponential_Deriv(double x)
        {
            double F = -Math.Exp(-x) - 1;
            return F;
        }

        /// <summary>
        /// Testing Newton-Raphson method with a nonlinear polynomial.
        /// </summary>
        [TestMethod()]
        public void Test_Cubic()
        {
            double initial = 1d;
            double X = NewtonRaphson.Solve(Cubic_FX, Cubic_Deriv, initial);
            double trueX = 1.32472d;
            Assert.AreEqual(X, trueX, 1E-4);
        }

        /// <summary>
        /// Testing Newton-Raphson method with an exponential function.
        /// </summary>
        [TestMethod()]
        public void Test_Exponential()
        {
            double initial = 1d;
            double X = NewtonRaphson.Solve(Exponential_FX, Exponential_Deriv, initial);
            double trueX = 0.567143290d;
            Assert.AreEqual(X, trueX, 1E-4);
        }

        /// <summary>
        /// Robust Newton-Raphson, falls back to bisection with a nonlinear polynomial.
        /// </summary>
        [TestMethod()]
        public void Test_RobustNewtonRaphsonCubic()
        {
            double initial = 1d;
            double lower = -1;
            double upper = 5d;
            double X = NewtonRaphson.RobustSolve(Cubic_FX, Cubic_Deriv, initial, lower, upper);
            double trueX = 1.32472d;
            Assert.AreEqual(X, trueX, 1E-4);
        }

        /// <summary>
        /// Testing Robust Newton-Raphson method with an exponential function.
        /// </summary>
        [TestMethod()]
        public void Test_RobustNewtonRaphsonExponential()
        {
            double initial = 1d;
            double lower = -1;
            double upper = 5d;
            double X = NewtonRaphson.RobustSolve(Exponential_FX, Exponential_Deriv, initial, lower, upper);
            double trueX = 0.567143290d;
            Assert.AreEqual(X, trueX, 1E-4);
        }

        ///// <summary>
        ///// Testing Edge case where Newton's method fails. This is due to local minima and maxima around the root 
        //// or initial guess. 
        ///// </summary>
        //public double Edge_FX(double x)
        //{
        //    double f = 27 * Math.Pow(x, 3) - 3 * x + 1;
            
        //    return f; 
        //}

        //[TestMethod()]
        //public void Test_RobustNewtonRaphsonEdge()
        //{
        //    double initial = 0d;
        //    double lower = -2d;
        //    double upper = 5d;
        //    double X = NewtonRaphson.RobustSolve(Edge_FX, Exponential_Deriv, initial, lower, upper);
        //    double trueX = -0.44157265d;
        //    Assert.AreEqual(X, trueX, 1E-4);
        
        ///[TestMethod()]
        /// Test_NewtonRaphsonInR()
        /// Recreated Robust Newton Raphson method in R with the same exponential function.
        /// All tests passed. Utilized 'pracma' package and newtonRaphson() function. Returned 
        /// result in 4 iterations.
    }
}
