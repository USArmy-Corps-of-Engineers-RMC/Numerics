using Numerics.Sampling;
using System;
using System.Collections.Generic;

namespace Numerics.Mathematics.Integration
{

    /// <summary>
    /// Contains methods for numerical integration.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    /// <see href = "https://en.wikipedia.org/wiki/Numerical_integration" />
    /// </para>
    /// </remarks>
    public sealed class Integration
    {

        /// <summary>
        /// Returns the integral of a function between a and b by ten-point Gauss-Legendre integration. 
        /// </summary>
        /// <param name="f">The function to integrate.</param>
        /// <param name="a">Start point for integration.</param>
        /// <param name="b">End point for integration.</param>
        /// <returns>The value of a definite integral.</returns>
        public static double GaussLegendre(Func<double, double> f, double a, double b)
        {
            var x = new double[] { 0.1488743389816312, 0.4333953941292472, 0.6794095682990244, 0.8650633666889845, 0.9739065285171717 };
            var w = new double[] { 0.2955242247147529, 0.2692667193099963, 0.2190863625159821, 0.1494513491505806, 0.0666713443086881 };
            double xm = 0.5 * (b + a);
            double xr = 0.5 * (b - a);
            double s = 0;
            for (int j = 0; j < 5; j++)
            {
                double dx = xr * x[j];
                s += w[j] * (f(xm + dx) + f(xm - dx));
            }
            return s *= xr;
        }


        /// <summary>
        /// Numerical integration using the Trapezoidal Rule.
        /// </summary>
        /// <param name="f">The function to integrate.</param>
        /// <param name="a">Start point for integration.</param>
        /// <param name="b">End point for integration.</param>
        /// <param name="steps">Number of integration steps. Default = 2.</param>
        /// <returns>The value of a definite integral.</returns>
        public static double TrapezoidalRule(Func<double, double> f, double a, double b, int steps = 2)
        {
            double h = (b - a) / steps;
            double x = a;
            double sum = 0.5d * (f(a) + f(b));
            for (int i = 1; i <= steps - 1; i++)
            {
                x += h;
                sum += f(x);
            }
            return h * sum;
        }


        /// <summary>
        /// Numerical integration using Simpson's Rule.
        /// </summary>
        /// <param name="f">The function to integrate.</param>
        /// <param name="a">Start point for integration.</param>
        /// <param name="b">End point for integration.</param>
        /// <param name="steps">Number of integration steps. Default = 2.</param>
        /// <returns>The value of a definite integral.</returns>
        public static double SimpsonsRule(Func<double, double> f, double a, double b, int steps = 2)
        {
            double h = (b - a) / steps;
            double sum1 = f(a  + h / 2d);
            double sum2 = 0d;
            for (int i = 1; i <= steps - 1; i++)
            {
                sum1 += f(a + h * i + h / 2d);
                sum2 += f(a + h * i);
            }          
            return h / 6d * (f(a) + f(b) + 4d * sum1 + 2d * sum2);
        }

        /// <summary>
        /// Numerical integration using the Midpoint method.
        /// </summary>
        /// <param name="f">The function to integrate.</param>
        /// <param name="a">Start point for integration.</param>
        /// <param name="b">End point for integration.</param>
        /// <param name="steps">Number of integration steps. Default = 2.</param>
        /// <returns>The value of a definite integral.</returns>
        public static double Midpoint(Func<double, double> f, double a, double b, int steps = 2)
        {
            double h = (b - a) / steps;
            double x = a + h / 2d;
            double sum = 0;
            for (int i = 1; i <= steps; i++)
            {
                sum += f(x);
                x += h;
            }
            return h * sum;
        }

        ///// <summary>
        ///// Adaptive Simpson's Rule. 
        ///// </summary>
        ///// <param name="f">The function to integrate.</param>
        ///// <param name="a">Start point for integration.</param>
        ///// <param name="b">End point for integration.</param>
        ///// <param name="tolerance">The desired tolerance for the solution. Default = ~Sqrt(Machine Epsilon), or 1E-8.</param>
        //public static double AdaptiveSimpsonsRule(Func<double, double> f, double a, double b, double tolerance = 1E-8)
        //{
        //    double fa = f(a);
        //    double fb = f(b);
        //    double m = 0, fm = 0, whole = QuadSimpsons(f, a, fa, b, fb, ref m, ref fm);
        //    return QuadASR(f, a, fa, b, fb, tolerance, whole, m, fm);
        //}

        //private static double QuadSimpsons(Func<double, double> f, double a, double fa, double b, double fb, ref double m, ref double fm)
        //{
        //    m = (a + b) / 2d;
        //    fm = f(m);
        //    return Math.Abs(b - a) / 6d * (fa + 4d * fm + fb);
        //}

        //private static double QuadASR(Func<double, double> f, double a, double fa, double b, double fb, double eps, double whole, double m, double fm)
        //{
        //    double lm = 0, flm = 0, left = QuadSimpsons(f, a, fa, m, fm, ref lm, ref flm);
        //    double rm = 0, frm = 0, right = QuadSimpsons(f, m, fm, b, fb, ref rm, ref frm);
        //    double delta = left + right - whole;
        //    if (Math.Abs(delta) <= 15d * eps)
        //        return left + right + delta / 15d;
        //    else
        //        return QuadASR(f, a, fa, m, fm, eps, left, lm, flm) + QuadASR(f, m, fm, b, fb, eps, right, rm, frm);
        //}


    }
}