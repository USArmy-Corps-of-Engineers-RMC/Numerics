using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Mathematics.RootFinding;
using System;

namespace Test_Numerics.Mathematics.Root_Finding
{
    [TestClass]
    public class Test_Bisection
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
        /// Testing the Bisection method with a nonlinear polynomial.  
        /// </summary>
        [TestMethod()]
        public void Test_Cubic()
        {
            double initial = 1d;
            double lower = -1;
            double upper = 5d;
            double X = Bisection.Solve(Cubic_FX, initial, lower, upper);
            double trueX = 1.32472d;
            Assert.AreEqual(X, trueX, 1E-4);
        }
        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_BisectionEdge1()
        {
            double initial = -1d;
            double lower = 1d;
            double upper = 5d;
            double X = Bisection.Solve(Cubic_FX, initial, lower, upper);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "The upper bound (b) cannot be less than the lower bound (a).")]
        public void Test_BisectionEdge2()
        {
            double initial = 1d;
            double lower = 5;
            double upper = 0d;
            double X = Bisection.Solve(Cubic_FX, initial, lower, upper);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException), "Bisection method failed because the root is not bracketed.")]
        public void Test_BisectionEdge3()
        {
            double initial = 3d;
            double lower = 2;
            double upper = 5d;
            double X = Bisection.Solve(Cubic_FX, initial, lower, upper);
        }
        ///[TestMethod()]
        ///Test_BisectionInR()
        /// Recreated Bisection method in R comparing Test_Cubic(). Test passed.
        /// Used 'pracma' package and bisect() function. Returned in 7 iterations.
    }
}
