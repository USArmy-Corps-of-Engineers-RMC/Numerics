using System;

namespace Numerics.Mathematics.Integration
{

    /// <summary>
    /// A class that performs adaptive Simpson's integration. 
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public class AdaptiveSimpsonsRule : Integrator
    {

        /// <summary>
        /// Constructs a new adaptive Simpson's rule.
        /// </summary>
        /// <param name="function">The function to integrate.</param>
        /// <param name="min">The minimum value under which the integral must be computed.</param>
        /// <param name="max">The maximum value under which the integral must be computed.</param>
        public AdaptiveSimpsonsRule(Func<double, double> function, double min, double max)
        {
            if (max <= min) throw new ArgumentNullException(nameof(max), "The maximum value cannot be less than or equal to the minimum value.");
            Function = function ?? throw new ArgumentNullException(nameof(function), "The function cannot be null.");
            a = min;
            b = max;
            //MinFunctionEvaluations = 100;
        }

        private double a;
        private double b;

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
        /// The maximum recursion depth. Default = 100.
        /// </summary>
        public int MaxDepth { get; set; } = 100;

        /// <summary>
        /// Returns an approximate measure of the standard error of the integration. 
        /// </summary>
        public double StandardError { get; private set; }

        /// <summary>
        /// Evaluates the integral.
        /// </summary>
        public override void Integrate()
        {
            StandardError = 0;
            ClearResults();
            Validate();

            try
            {
                double fa = Function(a);
                double fb = Function(b);
                FunctionEvaluations += 2;
                double m = 0, fm = 0, whole = Simpsons(Function, a, fa, b, fb, ref m, ref fm);       
                Result = AdaptiveSimpsons(Function, a, fa, b, fb, RelativeTolerance, MaxDepth, whole, m, fm);
                StandardError = Math.Sqrt(StandardError);

                if (FunctionEvaluations >= MaxFunctionEvaluations)
                {
                    Status = IntegrationStatus.MaximumFunctionEvaluationsReached;
                }
                else
                {
                    Status = IntegrationStatus.Success;
                }
                    
            }
            catch (Exception ex)
            {
                Status = IntegrationStatus.Failure;
                if (ReportFailure) throw ex;
            }

        }

        private double Simpsons(Func<double, double> f, double a, double fa, double b, double fb, ref double m, ref double fm)
        {
            m = (a + b) / 2d;
            fm = f(m);
            FunctionEvaluations++;           
            return Math.Abs(b - a) / 6d * (fa + 4d * fm + fb);
        }

        private double AdaptiveSimpsons(Func<double, double> f, double a, double fa, double b, double fb, double epsilon, int depth, double whole, double m, double fm)
        {

            double lm = 0, flm = 0, left = Simpsons(f, a, fa, m, fm, ref lm, ref flm);
            double rm = 0, frm = 0, right = Simpsons(f, m, fm, b, fb, ref rm, ref frm);
            double delta = (left + right - whole) / 15d;

            // Check tolerance
            // - Depth is less than 0 (greater than max recursions)
            // - Abs(a-b) is smaller than machine epsilon
            // - Minimum number of function evaluations and also delta less than tolerance
            if (depth <= 0 || Math.Abs(a - b) <= Tools.DoubleMachineEpsilon || FunctionEvaluations >= MaxFunctionEvaluations ||  (FunctionEvaluations >= MinFunctionEvaluations && Math.Abs(delta) <= epsilon + epsilon * Math.Abs(whole)))
            {
                // convergence is reached
                // Terminate recursion
                StandardError += delta * delta / (b - a);
                return left + right + delta; 
            }
            else
            {
                // Subdivide interval
                var l = AdaptiveSimpsons(f, a, fa, m, fm, epsilon, depth - 1, left, lm, flm);
                var r = AdaptiveSimpsons(f, m, fm, b, fb, epsilon, depth - 1, right, rm, frm);
                return l + r;
            }           
        }


    }
}
