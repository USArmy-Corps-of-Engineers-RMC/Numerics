using System;

namespace Numerics.Mathematics.Optimization
{

    /// <summary>
    /// The Golden-Section optimization algorithm. The function need not be differentiable, and no derivatives are taken.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    /// This class contains a shared function for finding the minimum or maximum of a function using the Golden-Section method.
    /// </para>
    /// <para>
    /// References:
    /// </para>
    /// <para>
    /// "Numerical Recipes, Routines and Examples in Basic", J.C. Sprott, Cambridge University Press, 1991.
    /// <see href="https://en.wikipedia.org/wiki/Golden-section_search"/>
    /// </para>
    /// </remarks>
    public class GoldenSection : Optimizer
    {

        /// <summary>
        /// Construct a new Golden-Section optimization method. 
        /// </summary>
        /// <param name="objectiveFunction">The objective function to evaluate.</param>
        /// <param name="lowerBound">The lower bound (inclusive) of the interval containing the optimal point.</param>
        /// <param name="upperBound">The upper bound (inclusive) of the interval containing the optimal point.</param>
        public GoldenSection(Func<double, double> objectiveFunction, double lowerBound, double upperBound) : base((x) => objectiveFunction(x[0]), 1)
        {
            // validate inputs
            if (upperBound < lowerBound)
            {
                throw new ArgumentOutOfRangeException("upperBound", "The upper bound cannot be less than the lower bound.");
            }
            LowerBound = lowerBound;
            UpperBound = upperBound;
        }

        /// <summary>
        /// The lower bound (inclusive) of the interval containing the optimal point. 
        /// </summary>
        public double LowerBound { get; private set; }

        /// <summary>
        /// The upper bound (inclusive) of the interval containing the optimal point.
        /// </summary>
        public double UpperBound { get; private set; }

        /// <summary>
        /// Implements the actual optimization algorithm. This method should minimize the objective function. 
        /// </summary>
        protected override void Optimize()
        {
            // Define variables
            bool cancel = false;
            // Bracket problem
            double ax = LowerBound, bx = 0.5 * (UpperBound + LowerBound), cx = UpperBound;
            double R = 0.61803399, C = 1.0 - R;
            double x1, x2;
            double x0 = ax;
            double x3 = cx;
            if (Math.Abs(cx - bx) > Math.Abs(bx - ax))
            {
                x1 = bx;
                x2 = bx + C * (cx - bx);
            }
            else
            {
                x2 = bx;
                x1 = bx - C * (bx - ax);
            }
            double f1 = Evaluate(new double[] { x1 }, ref cancel);
            double f2 = Evaluate(new double[] { x2 }, ref cancel);
            while (Math.Abs(x3 - x0) > AbsoluteTolerance || Math.Abs(x3 - x0) > RelativeTolerance * (Math.Abs(x1) + Math.Abs(x2)))
            {
                if (f2 < f1)
                {
                    x0 = x1;
                    x1 = x2;
                    x2 = R * x2 + C * x3;
                    f1 = f2;
                    f2 = Evaluate(new double[] { x2 }, ref cancel);                 
                }
                else
                {
                    x3 = x2;
                    x2 = x1;
                    x1 = R * x1 + C * x0;
                    f2 = f1;
                    f1 = Evaluate(new double[] { x1 }, ref cancel);
                }
                if (cancel == true) return;
                Iterations += 1;
                if (Iterations >= MaxIterations)
                {
                    UpdateStatus(OptimizationStatus.MaximumIterationsReached);
                    return;
                }
            }
            UpdateStatus(OptimizationStatus.Success);
        }

    }
}