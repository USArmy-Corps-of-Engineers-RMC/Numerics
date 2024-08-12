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
