using System;

namespace Numerics.Mathematics.RootFinding
{

    /// <summary>
    /// The secant root-finding algorithm.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    /// This class contains a shared function for finding the solution to the equation f(x)=0 using the secant method.
    /// </para>
    /// <para>
    /// References:
    /// </para>
    /// <para>
    /// "Numerical Recipes, Routines and Examples in Basic", J.C. Sprott, Cambridge University Press, 1991.
    /// <see href="https://en.wikipedia.org/wiki/Root-finding_algorithm"/>
    /// <see href="https://en.wikipedia.org/wiki/Secant_method"/>
    /// </para>
    /// </remarks>
    public class Secant
    {

        /// <summary>
        /// Use the secant method to find a solution of the equation f(x)=0.
        /// </summary>
        /// <param name="f">The function to solve.</param>
        /// <param name="lowerBound">The lower bound (a) of the interval containing the root.</param>
        /// <param name="upperBound">The upper bound (b) of the interval containing the root.</param>
        /// <param name="tolerance">Optional. Desired tolerance for both the root and the function value at the root.
        /// The root will be refined until the tolerance is achieved or the maximum number of iterations is reached. Default = 1e-12.</param>
        /// <param name="maxIterations">Optional. Maximum number of iterations. Default = 1000.</param>
        /// <param name="reportFailure">Optional. If set to true, an exception will be thrown if the routine fails to converge.
        /// If set to false, the root from the last iteration will be returned if the routine fails to converge. Default = True.</param>
        /// <returns>
        /// The root to the equation f(x)=0 given the specified tolerance.
        /// </returns>
        public static double Solve(Func<double, double> f, double lowerBound, double upperBound, double tolerance = 0.000000000001d, int maxIterations = 1000, bool reportFailure = true)
        {

            // validate inputs
            if (upperBound < lowerBound)
            {
                throw new ArgumentOutOfRangeException("upperBound", "The upper bound (b) cannot be less than the lower bound (a).");
            }

            // Define variables
            bool solutionFound = false;
            double root;
            double fl = f(lowerBound);
            double fh = f(upperBound);
            double swap;
            double dx;

            // Pick the bound with the smaller function value as the most recent guess
            if (Math.Abs(fl) < Math.Abs(fh))
            {
                root = lowerBound;
                lowerBound = upperBound;
                swap = fl;
                fl = fh;
                fh = swap;
            }
            else
            {
                root = upperBound;
            }

            // Secant loop
            for (int i = 1; i <= maxIterations; i++)
            {
                dx = (lowerBound - root) * fh / (fh - fl);
                lowerBound = root;
                fl = fh;
                root += dx;
                fh = f(root);
                if (Math.Abs(dx) < tolerance || fh == 0d)
                {
                    solutionFound = true;
                    break;
                }
            }

            // return results of solver
            if (solutionFound == false && reportFailure == true)
            {
                throw new ArgumentException("Secant method failed to find root.");
            }
            else
            {
                return root;
            }

        }
    }
}