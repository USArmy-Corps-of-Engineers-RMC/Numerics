using System;
using System.Collections.Generic;
using System.Linq;
using Numerics.Data.Statistics;
using Numerics.Mathematics.RootFinding;

namespace Numerics.Distributions.Copulas
{

    /// <summary>
    /// The Ali-Mikhail-Haq (AHM) copula.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public class AMHCopula : ArchimedeanCopula
    {

        /// <summary>
        /// Constructs a Ali-Mikhail-Haq (AHM) copula with a dependency θ = 2.
        /// </summary>
        public AMHCopula()
        {
            Theta = 2d;
        }

        /// <summary>
        /// Constructs a Ali-Mikhail-Haq (AHM) copula with a specified θ.
        /// </summary>
        /// <param name="theta">The dependency parameter, θ.</param>
        public AMHCopula(double theta)
        {
            Theta = theta;
        }

        /// <summary>
        /// Constructs a Ali-Mikhail-Haq (AHM) copula with a specified θ and marginal distributions.
        /// </summary>
        /// <param name="theta">The dependency parameter, θ.</param>
        ///<param name="marginalDistributionX">The X marginal distribution for the copula.</param>
        ///<param name="marginalDistributionY">The Y marginal distribution for the copula.</param>
        public AMHCopula(double theta, IUnivariateDistribution marginalDistributionX, IUnivariateDistribution marginalDistributionY)
        {
            Theta = theta;
            MarginalDistributionX = marginalDistributionX;
            MarginalDistributionY = marginalDistributionY;
        }

        /// <summary>
        /// Returns the Copula type.
        /// </summary>
        public override CopulaType Type
        {
            get { return CopulaType.AliMikhailHaq; }
        }

        /// <summary>
        /// Returns the display name of the Copula distribution type as a string.
        /// </summary>
        public override string DisplayName
        {
            get { return "Ali-Mikhail-Haq"; }
        }

        /// <summary>
        /// Returns the short display name of the Copula distribution as a string.
        /// </summary>
        public override string ShortDisplayName
        {
            get { return "AHM"; }
        }

        /// <summary>
        /// Returns the minimum value allowable for the dependency parameter.
        /// </summary>
        public override double ThetaMinimum
        {
            get { return -1.0d; }
        }

        /// <summary>
        /// Returns the maximum values allowable for the dependency parameter.
        /// </summary>
        public override double ThetaMaximum
        {
            get { return 1.0d; }
        }

        /// <summary>
        /// Test to see if distribution parameters are valid.
        /// </summary>
        /// <param name="parameter">Dependency parameter.</param>
        /// <param name="throwException">Boolean indicating whether to throw the exception or not.</param>
        /// <returns>Nothing if the parameters are valid and the exception if invalid parameters were found.</returns>
        public override ArgumentOutOfRangeException ValidateParameter(double parameter, bool throwException)
        {
            if (parameter < ThetaMinimum)
            {
                if (throwException) throw new ArgumentOutOfRangeException(nameof(Theta), "The dependency parameter θ (theta) must be greater than or equal to " + ThetaMinimum.ToString() + ".");
                return new ArgumentOutOfRangeException(nameof(Theta), "The dependency parameter θ (theta) must be greater than or equal to " + ThetaMinimum.ToString() + ".");
            }
            if (parameter > ThetaMaximum)
            {
                if (throwException) throw new ArgumentOutOfRangeException(nameof(Theta), "The dependency parameter θ (theta) must be less than or equal to " + ThetaMaximum.ToString() + ".");
                return new ArgumentOutOfRangeException(nameof(Theta), "The dependency parameter θ (theta) must be less than or equal to " + ThetaMaximum.ToString() + ".");
            }
            //if (Math.Abs(parameter) <= 100 * Tools.DoubleMachineEpsilon)
            //{
            //    if (throwException) throw new ArgumentOutOfRangeException(nameof(Theta), "The dependency parameter θ (theta) cannot be zero. This is independence.");
            //    return new ArgumentOutOfRangeException(nameof(Theta), "The dependency parameter θ (theta) cannot be zero. This is independence.");
            //}
            return null;
        }

        /// <summary>
        /// The generator function of the copula.
        /// </summary>
        /// <param name="t">The reduced variate.</param>
        public override double Generator(double t)
        {
            return Math.Log((1.0d - Theta * (1.0d - t)) / t);
        }

        /// <summary>
        /// The inverse of the generator function.
        /// </summary>
        /// <param name="t">The reduced variate.</param>
        public override double GeneratorInverse(double t)
        {
            return (1.0d - Theta) / (Math.Exp(t) - Theta);
        }

        /// <summary>
        /// The first derivative of the generator function.
        /// </summary>
        /// <param name="t">The reduced variate.</param>
        public override double GeneratorPrime(double t)
        {
            return (Theta - 1.0d) / (t * (Theta * (t - 1.0d) + 1.0d));
        }

        /// <summary>
        /// The second derivative of the generator function.
        /// </summary>
        /// <param name="t">The reduced variate.</param>
        public override double GeneratorPrime2(double t)
        {
            double num = (Theta - 1.0d) * (Theta * (2.0d * t - 1.0d) + 1.0d);
            double a = Theta * (t - 1.0d) * t + t;
            double den = Math.Sign(a) * Math.Pow(Math.Abs(a), 2d);
            return -num / den;
        }

        /// <summary>
        /// The inverse of the first derivative of the generator function.
        /// </summary>
        /// <param name="t">The reduced variate.</param>
        public override double GeneratorPrimeInverse(double t)
        {
            return Brent.Solve(x => GeneratorPrime(x) - t, 0d, 1d);
        }

        /// <summary>
        /// The inverse cumulative distribution function (InverseCDF) of the copula evaluated at probabilities u and v.
        /// </summary>
        /// <param name="u">Probability between 0 and 1.</param>
        /// <param name="v">Probability between 0 and 1.</param>
        public override double[] InverseCDF(double u, double v)
        {
            // Validate parameters
            if (_parametersValid == false) ValidateParameter(Theta, true);
            //Johnson (1987, p.362).
            double w = v;
            double b = 1d - u;
            double A = w * Math.Pow(Theta * b, 2) - Theta;
            double B = Theta + 1d - 2d * Theta * b * w;
            double C = w - 1d;
            v = (-B + Math.Sqrt(B * B - 4d * A * C)) / 2d / A;
            v = 1d - v;
            return new[] { u, v };
        }

        /// <summary>
        /// Create a deep copy of the copula.
        /// </summary>
        public override BivariateCopula Clone()
        {
            return new AMHCopula(Theta, MarginalDistributionX, MarginalDistributionY);
        }

        /// <summary>
        /// Estimates the dependency parameter using the method of moments.
        /// </summary>
        /// <param name="sampleDataX">The sample data for the X variable.</param>
        /// <param name="sampleDataY">The sample data for the Y variable.</param>
        public void SetThetaFromTau(IList<double> sampleDataX, IList<double> sampleDataY)
        {
            var tau = Correlation.KendallsTau(sampleDataX, sampleDataY);

            if ((tau < (5d - 8d * Math.Log(2d)) / 3d) || (tau > 1d / 3d))
                throw new Exception("For the AMH copula, tau must be in [(5 - 8 log 2) / 3, 1 / 3] ~= [-0.1817, 0.3333]. The dependency in the data is too strong to use the AMH copula.");

            double L = tau > 0 ? 0.001d : -1d + Tools.DoubleMachineEpsilon;
            double U = tau > 0 ? 1d - Tools.DoubleMachineEpsilon : -0.001d;

            Theta = Brent.Solve(t => 
            {
                var x = 1d - 2d * (Math.Pow(1d - t, 2d) * Math.Log(-t + 1d) + t) / (3d * t * t);
                return x - tau;
            }, L, U);

        }

        /// <summary>
        /// Returns the parameter constraints for the dependency parameter given the data samples. 
        /// </summary>
        /// <param name="sampleDataX">The sample data for the X variable.</param>
        /// <param name="sampleDataY">The sample data for the Y variable.</param>
        public override double[] ParameterConstraints(IList<double> sampleDataX, IList<double> sampleDataY)
        {
            //var tau = Correlation.KendallsTau(sampleDataX, sampleDataY);

            //if ((tau < (5d - 8d * Math.Log(2d)) / 3d) || (tau > 1d/3d))
            //    throw new Exception("For the AMH copula, tau must be in [(5 - 8 log 2) / 3, 1 / 3] ~= [-0.1817, 0.3333]. The dependency in the data is too strong to use the AMH copula.");

            //double L = tau > 0 ? 0.001 : -1d + Tools.DoubleMachineEpsilon;
            //double U = tau > 0 ? 1d - Tools.DoubleMachineEpsilon : -0.001d;

            return new double[] { -1 + Tools.DoubleMachineEpsilon, 1 - Tools.DoubleMachineEpsilon };
        }

    }
}