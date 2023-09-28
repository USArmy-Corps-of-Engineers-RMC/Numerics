namespace Numerics.Mathematics.SpecialFunctions
{

    /// <summary>
    /// Evaluation functions useful for computing polynomials. 
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public class Evaluate
    {

        /// <summary>
        /// Evaluates a double precision polynomial.
        /// </summary>
        /// <remarks>
        /// For sanity's sake, the value of N indicates the NUMBER of
        /// coefficients, or more precisely, the ORDER of the polynomial,
        /// rather than the DEGREE Of the polynomial. The two quantities
        /// differ by 1, but cause a great deal Of confusion.
        /// <para>
        /// References: Based on a function contained in algorithm AS241, Applied Statistics, 1988, Vol. 37, No. 3.
        /// </para>
        /// </remarks>
        /// <param name="coefficients">The coefficients of the polynomial. Item[0] is the constant term. </param>
        /// <param name="x">The point at which the polynomial is to be evaluated.</param>
        public static double Polynomial(double[] coefficients, double x)
        {
            int n = coefficients.Length;
            double value = coefficients[n - 1];
            for (int i = n - 2; i >= 0; i -= 1)
            {
                value *= x;
                value += coefficients[i];
            }

            return value;
        }

        public static double Polynomial(double[] coefficients, double x, int n)
        {
            double val = coefficients[0];
            for (int i = 1; i <= n; i++)
                val = val * x + coefficients[i];
            return val;
        }

        /// <summary>
        /// Evaluates a double precision polynomial. Coefficients are in reverse order.
        /// </summary>
        /// <param name="coefficients">The coefficients of the polynomial. Item[0] is the constant term.</param>
        /// <param name="x">The point at which the polynomial is to be evaluated.</param>
        /// <returns></returns>
        public static double PolynomialRev(double[] coefficients, double x)
        {
            int n = coefficients.Length;
            double value = coefficients[0];
            for (int i = 1; i < n; i++)
            {
                value *= x;
                value += coefficients[i];
            }

            return value;
        }

        /// <summary>
        /// Evaluates a double precision polynomial. Coefficients are in reverse order, and coefficient(N) = 1.0.
        /// </summary>
        public static double PolynomialRev_1(double[] coefficients, double x)
        {
            int n = coefficients.Length;
            double value = x + coefficients[0];
            for (int i = 1; i < n; i++)
            {
                value *= x;
                value += coefficients[i];
            }
            return value;
        }
    }
}