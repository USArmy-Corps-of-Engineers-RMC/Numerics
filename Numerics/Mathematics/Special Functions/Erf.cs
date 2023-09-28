using System;
using Numerics.Distributions;

namespace Numerics.Mathematics.SpecialFunctions
{
    /// <summary>
    /// The error function.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public partial class Erf
    {

        /// <summary>
        /// Error function.
        /// </summary>
        public static double Function(double X)
        {
            if (X < 0d)
            {
                return -Gamma.LowerIncomplete(0.5d, X * X);
            }
            else
            {
                return Gamma.LowerIncomplete(0.5d, X * X);
            }
        }

        /// <summary>
        /// Complement of the error function.
        /// </summary>
        public static double Erfc(double X)
        {
            return 1d - Function(X);
        }

        /// <summary>
        /// Inverse error function.
        /// </summary>
        public static double InverseErf(double y)
        {
            double s = Normal.StandardZ(0.5d * y + 0.5d);
            double r = s * Tools.Sqrt2 / 2.0d;
            return r;
        }

        /// <summary>
        /// Inverse complemented error function.
        /// </summary>
        public static double InverseErfc(double y)
        {
            double s = Normal.StandardZ(-0.5d * y + 1d);
            double r = s * Tools.Sqrt2 / 2.0d;
            return r;
        }
    }
}