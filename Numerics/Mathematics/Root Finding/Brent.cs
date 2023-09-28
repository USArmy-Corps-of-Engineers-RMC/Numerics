using System;

namespace Numerics.Mathematics.RootFinding
{

    /// <summary>
    /// The Brent root-finding algorithm.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    /// This class contains a shared function for finding the solution to the equation f(x)=0 using Brent's method.
    /// </para>
    /// <para>
    /// References:
    /// </para>
    /// <para>
    /// "Numerical Recipes, Routines and Examples in Basic", J.C. Sprott, Cambridge University Press, 1991.
    /// <see href="https://en.wikipedia.org/wiki/Root-finding_algorithm"/>
    /// <see href="https://en.wikipedia.org/wiki/Brent%27s_method"/>
    /// </para>
    /// </remarks>
    public class Brent
    {

        /// <summary>
        /// Use the Brent method to find a solution of the equation f(x)=0.
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
        public static double Solve(Func<double, double> f, double lowerBound, double upperBound, double tolerance = 1E-8, int maxIterations = 1000, bool reportFailure = true)
        {

            // validate inputs
            if (upperBound < lowerBound)
            {
                throw new ArgumentOutOfRangeException("upperBound", "The upper bound (b) cannot be less than the lower bound (a).");
            }

            // This algorithm is implemented as shown in:
            // "Numerical Recipes, Routines and Examples in Basic", J.C. Sprott, Cambridge University Press, 1991.

            // Define variables
            bool solutionFound = false;
            double EPS = Tools.DoubleMachineEpsilon;
            double a = lowerBound, b = upperBound, c = upperBound, d = 0, e = 0, fa = f(a), fb = f(b), fc, p, q, r, s, tol1, xm, root = 0;
            if ((fa > 0.0 && fb > 0.0) || (fa < 0.0 && fb < 0.0))
            {
                throw new ArgumentException("Brent's method failed because the root is not bracketed.");
            }
            fc = fb;

            // Brent's loop
            for (int i = 1; i <= maxIterations; i++)
            {
                if ((fb > 0.0 && fc > 0.0) || (fb < 0.0 && fc < 0.0))
                {
                    c = a;
                    fc = fa;
                    e = d = b - a;
                }
                if (Math.Abs(fc) < Math.Abs(fb))
                {
                    a = b;
                    b = c;
                    c = a;
                    fa = fb;
                    fb = fc;
                    fc = fa;
                }
                tol1 = 2.0 * EPS * Math.Abs(b) + 0.5 * tolerance;
                xm = 0.5 * (c - b);
                if (Math.Abs(xm) <= tol1 || fb == 0.0)
                {
                    root = b;
                    solutionFound = true;
                    break;
                }
                if (Math.Abs(e) >= tol1 && Math.Abs(fa) > Math.Abs(fb))
                {
                    s = fb / fa;
                    if (a == c)
                    {
                        p = 2.0 * xm * s;
                        q = 1.0 - s;
                    }
                    else
                    {
                        q = fa / fc;
                        r = fb / fc;
                        p = s * (2.0 * xm * q * (q - r) - (b - a) * (r - 1.0));
                        q = (q - 1.0) * (r - 1.0) * (s - 1.0);
                    }
                    if (p > 0.0) q = -q;
                    p = Math.Abs(p);
                    double min1 = 3.0 * xm * q - Math.Abs(tol1 * q);
                    double min2 = Math.Abs(e * q);
                    if (2.0 * p < (min1 < min2 ? min1 : min2))
                    {
                        e = d;
                        d = p / q;
                    }
                    else
                    {
                        d = xm;
                        e = d;
                    }
                }
                else
                {
                    d = xm;
                    e = d;
                }
                a = b;
                fa = fb;
                if (Math.Abs(d) > tol1)
                    b += d;
                else
                    b += Tools.Sign(tol1, xm);
                fb = f(b);
            }

            // return results of solver
            if (solutionFound == false && reportFailure == true)
            {
                throw new ArgumentException("Brent's method failed to find root.");
            }
            else
            {
                return root;
            }
        }

        /// <summary>
        /// Bracket the objective function minimum.
        /// </summary>
        /// <param name="s">Starting step size. Default = 1E-2.</param>
        /// <param name="k">Expansion factor. Default = 2.</param>
        public static bool Bracket(Func<double, double> f, ref double lowerBound, ref double upperBound, double s = 1E-2, double k = 2d)
        {
            int NTRY = 50;
            double FACTOR = 1.6;
            if (lowerBound == upperBound) throw new Exception("Bad initial range in zbrac");
            double f1 = f(lowerBound);
            double f2 = f(lowerBound);
            for (int j = 0; j < NTRY; j++)
            {
                if (f1 * f2 < 0.0) return true;
                if (Math.Abs(f1) < Math.Abs(f2))
                    f1 = f(lowerBound += FACTOR * (lowerBound - upperBound));
                else
                    f2 = f(upperBound += FACTOR * (upperBound - lowerBound));
            }
            return false;

        }



    }
}