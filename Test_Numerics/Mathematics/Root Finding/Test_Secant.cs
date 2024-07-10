using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Mathematics.RootFinding;
using System;

namespace Test_Numerics.Mathematics.Root_Finding
{
    [TestClass]
    public class Test_Secant
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
        /// Quadratic function
        /// </summary>
        public double Quadratic(double x)
        {
            double F = Math.Pow(x, 2) - 2;
            return F;
        }

        /// <summary>
        /// First derivative of Quadratic function
        /// </summary
        public double Quadratic_Deriv(double x)
        {
            double F = 2 * x;
            return F;
        }

        /// <summary>
        /// Testing Secant method with a nonlinear polynomial.
        /// </summary>
        [TestMethod()]
        public void Test_Cubic()
        {
            double lower = -1;
            double upper = 5d;
            double X = Secant.Solve(Cubic_FX, lower, upper);
            double trueX = 1.32472d;
            Assert.AreEqual(X, trueX, 1E-4);
        }

        /// <summary>
        /// Testing Secant method to approximate the square root of 2.
        /// </summary>
        [TestMethod()]
        public void Test_SquareRoot()
        {
            double lower = -6d;
            double upper = 5d;
            double X = Secant.Solve(Quadratic, lower, upper);
            double trueX = Math.Sqrt(2);
            Assert.AreEqual(X, trueX, 1E-4);
        }
        /// <summary>
        /// Testing edge case where the function is discontinuous within the interval        
        /// </summary>
        public double Undefined(double x)
        {
            double F = 1 / x;
            return F;
        }
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException),"Secant method failed to find root")]
        public void Test_Edge()
        {
            double lower = -6d;
            double upper = 5d;
            double X = Secant.Solve(Undefined, lower, upper);
            double trueX = Math.Sqrt(2);
            Assert.AreEqual(X, trueX, 1E-4);
        }

        ///[TestMethod()]
        /// Test_SecantInR()
        /// Recreated Secant method with the Quadratic() function and received the same result in 10 iterations. 
        /// Test passed. Utilized 'pracma' package and secant() function. 
    }
}
