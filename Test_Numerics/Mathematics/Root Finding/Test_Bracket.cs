/**
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
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Mathematics.RootFinding;
using System;

namespace Mathematics.RootFinding
{
    /// <summary>
    /// Test root finding methods. 
    /// </summary>
    [TestClass()]
    public class Test_Bracket
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
        /// Higher order polynomial
        /// </summary>
        public double PowTen_FX(double x)
        {
            double F = Math.Pow(x, 10) - 1;
            return F;
        }

        /// <summary>
        /// First derivative of Higher order polynomial
        /// </summary>
        public double PowTen_Deriv(double x)
        {
            double F = 10 * Math.Pow(x, 9);
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

        [TestMethod()]
        public void Test_BracketCubic()
        {
            double lower = 2;
            double upper = 5d;
            bool X = Brent.Bracket(Cubic_FX, ref lower, ref upper, out double f1, out double f2);
            Assert.IsTrue(X);
        }
        [TestMethod()]
        public void Test_BracketTrig()
        {
            double lower = Math.PI / 2;
            double upper = Math.PI;
            bool X = Brent.Bracket(Trig_FX, ref lower, ref upper, out double f1, out double f2);
            Assert.IsTrue(X);
        }

        [TestMethod()]
        public void Test_BracketExp()
        {
            double lower = 0;
            double upper = -1;
            bool X = Brent.Bracket(Exponential_FX, ref lower, ref upper, out double f1, out double f2);
            Assert.IsTrue(X);
        }

        [TestMethod()]
        public void Test_BracketPowTen()
        {
            double lower = 0;
            double upper = 1;
            bool X = Brent.Bracket(PowTen_FX, ref lower, ref upper, out double f1, out double f2);
            Assert.IsTrue(X);
        }

        [TestMethod()]
        public void Test_BracketQuadratic()
        {
            double lower = 0;
            double upper = 1;
            bool X = Brent.Bracket(Quadratic, ref lower, ref upper, out double f1, out double f2);
            Assert.IsTrue(X);
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception), "Bad initial range in zbrac")]
        public void Test_BracketEdge()
        {
            double lower = 1;
            double upper = 1;
            bool X = Brent.Bracket(Quadratic, ref lower, ref upper, out double f1, out double f2);
        }
    }
}