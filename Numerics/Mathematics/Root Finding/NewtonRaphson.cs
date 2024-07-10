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
* **/

using System;

namespace Numerics.Mathematics.RootFinding
{

    /// <summary>
    /// This class contains a shared function for finding the solution to the equation f(x)=0 using the Newton-Raphson method.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    /// In numerical analysis, Newton's method (also known as the Newton–Raphson method), named after Isaac Newton and Joseph Raphson,
    /// is a method for finding successively better approximations to the roots (or zeros) of a real-valued function.
    /// </para>
    /// <para>
    /// References:
    /// </para>
    /// <para>
    /// "Numerical Recipes, Routines and Examples in Basic", J.C. Sprott, Cambridge University Press, 1991.
    /// <see href="https://en.wikipedia.org/wiki/Root-finding_algorithm"/>
    /// <see href="https://en.wikipedia.org/wiki/Newton%27s_method"/>
    /// <see href="http://www.m-hikari.com/ams/ams-2017/ams-53-56-2017/p/hahmAMS53-56-2017.pdf"/>
    /// </para>
    /// </remarks>
    public class NewtonRaphson
    {

        /// <summary>
        /// Use the basic Newton-Raphson method to find a solution of the equation f(x)=0.
        /// </summary>
        /// <remarks>
        /// The basic algorithm aborts immediately if the root leaves the bound interval.
        /// </remarks>
        /// <param name="f">The function to find roots from.</param>
        /// <param name="df">The first derivative of the function to find roots from.</param>
        /// <param name="firstGuess">This is the first guess at the root (c).</param>
        /// <param name="tolerance">Optional. Desired tolerance for both the root and the function value at the root.
        /// The root will be refined until the tolerance is achieved or the maximum number of iterations is reached. Default = 1e-6.</param>
        /// <param name="maxIterations">Optional. Maximum number of iterations. Default = 100.</param>
        /// <param name="reportFailure">Optional. If set to true, an exception will be thrown if the routine fails to converge.
        /// If set to false, the root from the last iteration will be returned if the routine fails to converge. Default = True.</param>
        /// <returns>
        /// The root to the equation f(x)=0 given the specified tolerance.
        /// </returns>
        public static double Solve(Func<double, double> f, Func<double, double> df, double firstGuess, double tolerance = 0.000001d, int maxIterations = 100, bool reportFailure = true)
        {

            // Define variables
            double root = firstGuess;
            bool solutionFound = false;
            double y;
            double yPrime;
            double x0;
            double x1;
            double eps = Tools.DoubleMachineEpsilon; // Don't want to divide by a number smaller than this

            // Newton-Raphson loop
            for (int i = 1; i <= maxIterations; i++)
            {
                // do Newton's computation
                x0 = root;
                y = f(x0);
                yPrime = df(x0);
                if (Math.Abs(yPrime) < eps)
                {
                    // the denominator is too small
                    solutionFound = false;
                    break;
                }
                x1 = x0 - y / yPrime;
                root = x1;
                // check if the result is within the desired tolerance
                if (Math.Abs(x1 - x0) < tolerance)
                {
                    solutionFound = true;
                    break;
                }
            }

            // return results of solver
            if (solutionFound == false && reportFailure == true)
            {
                throw new ArgumentException("Newton-Raphson method failed to find root.");
            }
            else
            {
                return root;
            }

        }


        /// <summary>
        /// Use the robust Newton-Raphson method to find a solution of the equation f(x)=0.
        /// </summary>
        /// <remarks>
        /// Robust Newton-Raphson method falls back to bisection when over or undershooting the bounds.
        /// </remarks>
        /// <param name="f">The function to find roots from.</param>
        /// <param name="df">The first derivative of the function to find roots from.</param>
        /// <param name="firstGuess">This is the first guess at the root.</param>
        /// <param name="lowerBound">The lower bound of the interval containing the root. Aborts if it leaves the interval.</param>
        /// <param name="upperBound">The upper bound of the interval containing the root. Aborts if it leaves the interval.</param>
        /// <param name="tolerance">Optional. Desired tolerance for both the root and the function value at the root.
        /// The root will be refined until the tolerance is achieved or the maximum number of iterations is reached. Default = 1e-6.</param>
        /// <param name="maxIterations">Optional. Maximum number of iterations. Default = 100.</param>
        /// <param name="reportFailure">Optional. If set to true, an exception will be thrown if the routine fails to converge.
        /// If set to false, the root from the last iteration will be returned if the routine fails to converge. Default = True.</param>
        /// <returns>
        /// The root to the equation f(x)=0 given the specified tolerance.
        /// </returns>
        public static double RobustSolve(Func<double, double> f, Func<double, double> df, double firstGuess, double lowerBound, double upperBound, double tolerance = 0.000001d, int maxIterations = 100, bool reportFailure = true)
        {

            // Define variables
            double root = firstGuess;
            bool solutionFound = false;
            double XL = lowerBound;
            double XH = upperBound;
            double FL = f(XL);
            double FH = f(XH);
            double y;
            double yPrime;
            double x0;
            double x1 = 0;
            double eps = Tools.DoubleMachineEpsilon; // Don't want to divide by a number smaller than this

            // validate inputs
            if (upperBound < lowerBound)
            {
                throw new ArgumentOutOfRangeException("upperBound", "The upper bound (b) cannot be less than the lower bound (a).");
            }

            if (root < lowerBound || root > upperBound)
            {
                throw new ArgumentOutOfRangeException("firstGuess", "The first guess must be between the upper and lower bound.");
            }

            if (FL * FH >= 0d)
            {
                throw new ArgumentException("Robust Newton-Raphson method failed because the root is not bracketed.");
            }

            if (FL < 0d)
            {
                // Orient the search so that f(lowerBound)<0
                XL = lowerBound;
                XH = upperBound;
            }
            else
            {
                XH = lowerBound;
                XL = upperBound;
            }
            // The "step-size before last"
            double DXOLD = Math.Abs(upperBound - lowerBound);
            // The last step
            double DX = DXOLD;
            // Robust Newton-Raphson loop
            for (int i = 1; i <= maxIterations; i++)
            {
                // do Newton's computation
                x0 = root;
                y = f(x0);
                yPrime = df(x0);
                if (Math.Abs(yPrime) < eps)
                {
                    // the denominator is too small
                    solutionFound = false;
                    break;
                }
                double DUM = ((root - XH) * yPrime - y) * ((root - XL) * yPrime - y);
                if (DUM >= 0d || Math.Abs(2d * y) > Math.Abs(DXOLD * yPrime))
                {
                    // Bisect if Newton is out of range, or not decreasing fast enough
                    DXOLD = DX;
                    DX = 0.5d * (XH - XL);
                    root = XL + DX;
                    if (XL == root)
                    {
                        // Change in root is negligible
                        solutionFound = false;
                        break;
                    }
                }
                else
                {
                    DXOLD = DX;
                    DX = y / yPrime;
                    x1 = x0 - DX;
                    root = x1;
                    if (x0 == root)
                    {
                        // Change in root is negligible
                        solutionFound = false;
                        break;
                    }
                }
                // check if the result is within the desired tolerance
                if (Math.Abs(x1 - x0) < tolerance)
                {
                    solutionFound = true;
                    break;
                }
                // Maintain the bracket on the root
                if (f(root) < 0d)
                {
                    XL = root;
                }
                else
                {
                    XH = root;
                }
            }

            // return results of solver
            if (solutionFound == false && reportFailure == true)
            {
                throw new ArgumentException("Robust Newton-Raphson method failed to find root.");
            }
            else
            {
                return root;
            }

        }
    }
}