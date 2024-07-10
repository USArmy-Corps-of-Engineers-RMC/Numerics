using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Mathematics.RootFinding;
using System;

namespace Test_Numerics.Mathematics.Root_Finding
{
    [TestClass]
    public class Test_Brent
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
        /// Trignometric function.
        /// </summary>
        public double Trig_FX(double x)
        {
            double F = 2 * Math.Sin(x) - 3 * Math.Cos(x) - 0.5;
            return F;
        }

        /// <summary>
        /// First derivative of Trignometric function.
        /// </summary>
        public double Trig_Deriv(double x)
        {
            double F = 2 * Math.Cos(x) + 3 * Math.Sin(x);
            return F;
        }

        /// <summary>
        /// Testing Brent's method with a nonlinear polynomial.
        /// </summary>
        [TestMethod()]
        public void Test_Cubic()
        {
            double lower = -1;
            double upper = 5d;
            double X = Brent.Solve(Cubic_FX, lower, upper);
            double trueX = 1.32472d;
            Assert.AreEqual(X, trueX, 1E-4);
        }

        /// <summary>
        /// Testing Brent's method with trignometric function.
        /// </summary>
        [TestMethod()]
        public void Test_Trig()
        {
            double lower = 0;
            double upper = 2.5;
            double X = Brent.Solve(Trig_FX, lower, upper);
            double trueX = 1.12191713d;
            Assert.AreEqual(X, trueX, 1E-4);
        }

        /// [TestMethod()]
        /// Test_BrentInR()
        /// Recreated Brent's method in R with the trig function used above 
        /// and it returned the same root in 6 calls. Utilized  the 'pracma' package
        /// and the brent() function.


    }
}
