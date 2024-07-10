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

using Numerics.Mathematics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Numerics.Mathematics.Optimization
{

    /// <summary>
    /// Contains the Broyden-Fletcher-Goldfarb-Shanno (BFGS) optimization algorithm. 
    /// </summary>
    /// <remarks>
    /// <para>
    ///     <b> Authors: </b>
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    /// <b> Description: </b>
    /// This is an iterative method for solving unconstrained nonlinear optimization problems. It gradually improves
    /// an approximation to the Hessian matrix of the loss function, obtained from gradient evaluations via a 
    /// generalized secant method.
    /// </para>
    /// <b> References: </b>
    /// <list type="bullet">
    /// <item><description>
    /// "Numerical Recipes, Routines and Examples in Basic", J.C. Sprott, Cambridge University Press, 1991.
    /// <item><description>
    /// </description></item>
    /// "Numerical Recipes: The art of Scientific Computing, Third Edition. Press et al. 2017.
    /// <item><description>
    /// </description></item>
    /// <see href="https://en.wikipedia.org/wiki/Broyden%E2%80%93Fletcher%E2%80%93Goldfarb%E2%80%93Shanno_algorithm"/>
    /// </description></item>
    /// </list>
    /// </remarks>
    public class BFGS : Optimizer
    {

        /// <summary>
        /// Construct a new BFGS optimization method. 
        /// </summary>
        /// <param name="objectiveFunction">The objective function to evaluate.</param>
        /// <param name="numberOfParameters">The number of parameters in the objective function.</param>
        /// <param name="initialValues">An array of initial values to evaluate.</param>
        /// <param name="lowerBounds">An array of lower bounds (inclusive) of the interval containing the optimal point.</param>
        /// <param name="upperBounds">An array of upper bounds (inclusive) of the interval containing the optimal point.</param>
        /// <param name="gradient">Optional. Function to evaluate the gradient. Default uses finite difference.</param>
        public BFGS(Func<double[], double> objectiveFunction, int numberOfParameters, 
                    IList<double> initialValues, IList<double> lowerBounds, IList<double> upperBounds, 
                    Func<double[], double[]> gradient = null) : base(objectiveFunction, numberOfParameters)
        {
            // Check if the length of the initial, lower and upper bounds equal the number of parameters
            if (initialValues.Count != numberOfParameters || lowerBounds.Count != numberOfParameters || upperBounds.Count != numberOfParameters)
            {
                throw new ArgumentOutOfRangeException(nameof(lowerBounds), "The initial values and lower and upper bounds must be the same length as the number of parameters.");
            }
            // Check if the initial values are between the lower and upper values
            for (int j = 0; j < initialValues.Count; j++)
            {
                if (upperBounds[j] < lowerBounds[j])
                {
                    throw new ArgumentOutOfRangeException(nameof(upperBounds), "The upper bound cannot be less than the lower bound.");
                }
                if (initialValues[j] < lowerBounds[j] || initialValues[j] > upperBounds[j])
                {
                    throw new ArgumentOutOfRangeException(nameof(initialValues), "The initial values must be between the upper and lower bounds.");
                }
            }
            InitialValues = initialValues.ToArray();
            LowerBounds = lowerBounds.ToArray();
            UpperBounds = upperBounds.ToArray();
            Gradient = gradient;
        }

        /// <summary>
        /// An array of initial values to evaluate. 
        /// </summary>
        public double[] InitialValues { get; private set; }

        /// <summary>
        /// An array of lower bounds (inclusive) of the interval containing the optimal point. 
        /// </summary>
        public double[] LowerBounds { get; private set; }

        /// <summary>
        /// An array of upper bounds (inclusive) of the interval containing the optimal point.
        /// </summary>
        public double[] UpperBounds { get; private set; }

        /// <summary>
        /// The function for evaluating the gradient of the objective function.
        /// </summary>
        public Func<double[], double[]> Gradient;

        /// <summary>
        /// Implements the actual optimization algorithm. This method should minimize the objective function. 
        /// </summary>
        protected override void Optimize()
        {
            int i, j, D = NumberOfParameters;
            bool cancel = false;
            double EPS = Tools.DoubleMachineEpsilon;
            double TOLX = 4 * EPS, STPMX = 100.0;
            bool check = false;
            double fret = 0, den, fac, fad, fae, fp, stpmax, sum = 0.0, sumdg, sumxi, temp, test;
            var p = InitialValues.ToArray();
            var dg = new double[D];
            var g = new double[D];
            var hdg = new double[D];
            var pnew = new double[D];
            var xi = new double[D];
            var hessin = new Matrix(D, D);

            // Calculate the starting function value and gradient, and initialize the inverse Hessian to the unit matrix.
            fp = Evaluate(p, ref cancel);
            g = Gradient != null ? Gradient(p) : NumericalDerivative.Gradient((x) => Evaluate(x, ref cancel), p);
            for (i = 0; i < D; i++)
            {
                for (j = 0; j < D; j++) hessin[i, j] = 0.0;
                hessin[i, i] = 1.0;
                xi[i] = -g[i];
                sum += p[i] * p[i];
            }
            stpmax = STPMX * Math.Max(Math.Sqrt(sum), D);
            while (Iterations < MaxIterations)
            {
                LineSearch(p, fp, g, ref xi, ref pnew, ref fret, stpmax, ref check, ref cancel);
                if (cancel) return;
                // The new function evaluation occurs in line search; save the function value in fp for the next line search.
                // It is usually safe to ignore the value of check. 
                fp = fret;
                for (i = 0; i < D; i++)
                {
                    // Make sure the parameters are within the bounds.
                    pnew[i] = RepairParameter(pnew[i], LowerBounds[i], UpperBounds[i]);
                    xi[i] = pnew[i] - p[i];
                    p[i] = pnew[i];
                }
                // Test for convergence.
                test = 0.0;
                for (i = 0; i < D; i++)
                {
                    temp = Math.Abs(xi[i]) / Math.Max(Math.Abs(p[i]), 1.0);
                    if (temp > test) test = temp;
                }
                if (test <= RelativeTolerance)
                {
                    UpdateStatus(OptimizationStatus.Success);
                    return;
                }
                // Save the old gradient, and get the new gradient. 
                for (i = 0; i < D; i++) dg[i] = g[i];
                g = Gradient != null ? Gradient(p) : NumericalDerivative.Gradient((x) => Evaluate(x, ref cancel), p);
                if (cancel) return;
                // Test or convergence on zero gradient.
                test = 0.0;
                den = Math.Max(Math.Abs(fret), 1.0);
                for (i = 0; i < D; i++)
                {
                    temp = Math.Abs(g[i]) * Math.Max(Math.Abs(p[i]), 1.0) / den;
                    if (temp > test) test = temp;
                }
                if (test <= AbsoluteTolerance)
                {
                    UpdateStatus(OptimizationStatus.Success);
                    return;
                }
                // Compute difference of gradients.
                for (i = 0; i < D; i++)
                    dg[i] = g[i] - dg[i];
                // And difference times current matrix.
                for (i = 0; i < D; i++)
                {
                    hdg[i] = 0.0;
                    for (j = 0; j < D; j++) hdg[i] += hessin[i, j] * dg[j];
                }
                // Calculate dot products for the denominators.
                fac = fae = sumdg = sumxi = 0.0;
                for (i = 0; i < D; i++)
                {
                    fac += dg[i] * xi[i];
                    fae += dg[i] * hdg[i];
                    sumdg += Tools.Sqr(dg[i]);
                    sumxi += Tools.Sqr(xi[i]);
                }
                // Skip update if fac not sufficiently positive. 
                if (fac > Math.Sqrt(EPS * sumdg * sumxi))
                {
                    fac = 1.0 / fac;
                    fad = 1.0 / fae;
                    for (i = 0; i < D; i++) dg[i] = fac * xi[i] - fad * hdg[i];
                    for (i = 0; i < D; i++)
                    {
                        for (j = i; j < D; j++)
                        {
                            hessin[i, j] += fac * xi[i] * xi[j] - fad * hdg[i] * hdg[j] + fae * dg[i] * dg[j];
                            hessin[j, i] = hessin[i, j];
                        }
                    }
                }
                for (i = 0; i < D; i++)
                {
                    xi[i] = 0.0;
                    for (j = 0; j < D; j++) xi[i] -= hessin[i, j] * g[j];
                }

                Iterations += 1;
            }

            // If we made it to here, the maximum iterations were reached.
            UpdateStatus(OptimizationStatus.MaximumIterationsReached);

        }

        /// <summary>
        /// Auxiliary function for searching a line. 
        /// </summary>
        /// <param name="xold">n-dimensional point [0..n-1].</param>
        /// <param name="fold">Value of the function at xold.</param>
        /// <param name="g">Gradient of function at xold.</param>
        /// <param name="p">A direction to search.</param>
        /// <param name="x">A new point x[0..n-1]</param>
        /// <param name="f">The new function value.</param>
        /// <param name="stpmax">Limits the length of steps.</param>
        /// <param name="check">Check is false on a normal exit, true when x is too close to xold.</param>
        /// <param name="cancel">Determines if the solver should be canceled.</param>
        private void LineSearch(double[] xold, double fold, double[] g, ref double[] p, ref double[] x, ref double f, double stpmax, ref bool check, ref bool cancel)
        {
            double ALF = 1.0e-4, TOLX = Tools.DoubleMachineEpsilon;
            double a, alam, alam2 = 0.0, alamin, b, disc, f2 = 0.0;
            double rhs1, rhs2, slope = 0.0, sum = 0.0, temp, test, tmplam;
            int i, n = xold.Length;
            check = false;
            for (i = 0; i < n; i++) sum += p[i] * p[i];
            sum = Math.Sqrt(sum);
            if (sum > stpmax)
                for (i = 0; i < n; i++)
                    p[i] *= stpmax / sum;
            for (i = 0; i < n; i++)
                slope += g[i] * p[i];
            if (slope == 0.0) return; // If the slope is zero, it is on a flat spit. Exit the routine
            if (slope > 0.0)  throw new Exception("Roundoff problem in line search.");
            test = 0.0;
            for (i = 0; i < n; i++)
            {
                temp = Math.Abs(p[i]) / Math.Max(Math.Abs(xold[i]), 1.0);
                if (temp > test) test = temp;
            }
            alamin = TOLX / test;
            alam = 1.0;
            for (; ; )
            {
                for (i = 0; i < n; i++)
                {
                    x[i] = xold[i] + alam * p[i];
                    // Make sure the parameters are within the bounds.
                    x[i] = RepairParameter(x[i], LowerBounds[i], UpperBounds[i]);
                }
                f = Evaluate(x, ref cancel);
                if (cancel) return;
                if (alam < alamin)
                {
                    for (i = 0; i < n; i++) x[i] = xold[i];
                    check = true;
                    return;
                }
                else if (f <= fold + ALF * alam * slope) return;
                else
                {
                    if (alam == 1.0)
                        tmplam = -slope / (2.0 * (f - fold - slope));
                    else
                    {
                        rhs1 = f - fold - alam * slope;
                        rhs2 = f2 - fold - alam2 * slope;
                        a = (rhs1 / (alam * alam) - rhs2 / (alam2 * alam2)) / (alam - alam2);
                        b = (-alam2 * rhs1 / (alam * alam) + alam * rhs2 / (alam2 * alam2)) / (alam - alam2);
                        if (a == 0.0) tmplam = -slope / (2.0 * b);
                        else
                        {
                            disc = b * b - 3.0 * a * slope;
                            if (disc < 0.0) tmplam = 0.5 * alam;
                            else if (b <= 0.0) tmplam = (-b + Math.Sqrt(disc)) / (3.0 * a);
                            else tmplam = -slope / (b + Math.Sqrt(disc));
                        }
                        if (tmplam > 0.5 * alam)
                            tmplam = 0.5 * alam;
                    }
                }
                alam2 = alam;
                f2 = f;
                alam = Math.Max(tmplam, 0.1 * alam);
            }

        }

    }
}
