using System;

namespace Numerics.Mathematics.Integration
{
    /// <summary>
    /// A class for Trapezoidal rule integration. Integration steps are refined until convergence.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public class TrapezoidalRule : Integrator
    {
        /// <summary>
        /// Construct a new trapezoidal rule class. 
        /// </summary>
        /// <param name="function">The function to integrate.</param>
        /// <param name="min">The minimum value under which the integral must be computed.</param>
        /// <param name="max">The maximum value under which the integral must be computed.</param>
        public TrapezoidalRule(Func<double, double> function, double min, double max)
        {
            if (function == null) throw new ArgumentNullException(nameof(function), "The function cannot be null.");
            if (max <= min) throw new ArgumentNullException(nameof(max), "The maximum value cannot be less than or equal to the minimum value.");

            Function = function;
            a = min;
            b = max;
        }

        private double a;
        private double b;
        private double _s = 0;

        /// <summary>
        /// The unidimensional function to integrate.
        /// </summary>
        public Func<double, double> Function { get; }

        /// <summary>
        /// The minimum value under which the integral must be computed.
        /// </summary>
        public double Min => a;

        /// <summary>
        /// The maximum value under which the integral must be computed. 
        /// </summary>
        public double Max => b;

        /// <summary>
        /// Evaluates the integral. Integration steps are refined until convergence.
        /// </summary>
        public override void Integrate()
        {
            ClearResults();
            Validate();

            try
            {
                // Integrate
                double s0 = 0, s1 = 0, _s = 0;
                for (int i = 0; i < MaxIterations; i++)
                {
                    s0 = Next();

                    // Check function evaluations
                    if (FunctionEvaluations >= MaxFunctionEvaluations)
                    {
                        UpdateStatus(IntegrationStatus.MaximumFunctionEvaluationsReached);
                        return;
                    }
                       
                    // Check convergence
                    if (i > 2)
                    {
                        //if ((Math.Abs(s0 - s1) < RelativeTolerance * Math.Abs(s1)) || (s0 == 0.0 && s1 == 0.0))
                        if (CheckConvergence(s1, s0) || (s0 == 0.0 && s1 == 0.0))
                        {
                            Result = s0;
                            UpdateStatus(IntegrationStatus.Success);
                            return;
                        }
                    }
                    s1 = s0;
                }
                // If we get to here, then the maximum number of steps were reached before converging. 
                UpdateStatus(IntegrationStatus.MaximumIterationsReached);
            }
            catch (Exception ex)
            {
                UpdateStatus(IntegrationStatus.Failure, ex);
            }       
        }

        /// <summary>
        /// Returns the value of the integral at the nth step of refinement.
        /// </summary>
        private double Next()
        {
            double x, tnm, sum, del;
            int it, j;
            Iterations++;
            if (Iterations == 1)
            {
                _s = 0.5 * (b - a) * (Function(a) + Function(b));
                FunctionEvaluations += 2;
            }
            else
            {
                for (it = 1, j = 1; j < Iterations - 1; j++) it <<= 1;
                tnm = it;
                del = (b - a) / tnm;
                x = a + 0.5 * del;
                for (sum = 0.0, j = 0; j < it; j++, x += del)
                {
                    sum += Function(x);
                    FunctionEvaluations += 1;
                }
                _s = 0.5 * (_s + (b - a) * sum / tnm);
            }
            return _s;
        }

    }
}
