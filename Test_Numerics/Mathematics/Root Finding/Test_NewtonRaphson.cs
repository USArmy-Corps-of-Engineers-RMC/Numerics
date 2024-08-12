/*
* NOTICE:
* The U.S. Army Corps of Engineers, Risk Management Center (USACE-RMC) makes no guarantees about
* the results, or appropriateness of outputs, obtained from Numerics.
*
* LIST OF CONDITIONS:
* Redistribution and use in source and binary forms, with or without modification, are permitted
* provided that the following conditions are met:
* ● Redistributions of source code must retain the above notice, this list of conditions, and the
* following disclaimer.
* ● Redistributions in binary form must reproduce the above notice, this list of conditions, and
* the following disclaimer in the documentation and/or other materials provided with the distribution.
* ● The names of the U.S. Government, the U.S. Army Corps of Engineers, the Institute for Water
* Resources, or the Risk Management Center may not be used to endorse or promote products derived
* from this software without specific prior written permission. Nor may the names of its contributors
* be used to endorse or promote products derived from this software without specific prior
* written permission.
*
* DISCLAIMER:
* THIS SOFTWARE IS PROVIDED BY THE U.S. ARMY CORPS OF ENGINEERS RISK MANAGEMENT CENTER
* (USACE-RMC) "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
* THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL USACE-RMC BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
* SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
* PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
* INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
* LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
* THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

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
