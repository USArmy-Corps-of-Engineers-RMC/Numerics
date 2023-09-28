using System;
using System.Collections.Generic;
using System.Linq;
using Numerics.Distributions;
using Numerics.Sampling;

namespace Numerics.Mathematics.Optimization
{

    /// <summary>
    /// The Shuffled Complex Evolution (SCE-UA) algorithm, which finds a global minima when no gradient is available.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    ///     References:
    /// </para>
    /// <para>
    ///     Implements Duan et al. (1992) SCE-UA probabilistic search algorithm.
    /// </para>
    /// </remarks>
    public class ShuffledComplexEvolution : Optimizer
    {

        /// <summary>
        /// Construct a new shuffled complex evolution (SCE-UA) optimization method. 
        /// </summary>
        /// <param name="objectiveFunction">The objective function to evaluate.</param>
        /// <param name="numberOfParameters">The number of parameters in the objective function.</param>
        /// <param name="lowerBounds">An array of lower bounds (inclusive) of the interval containing the optimal point.</param>
        /// <param name="upperBounds">An array of upper bounds (inclusive) of the interval containing the optimal point.</param>
        public ShuffledComplexEvolution(Func<double[], double> objectiveFunction, int numberOfParameters, IList<double> lowerBounds, IList<double> upperBounds) : base(objectiveFunction, numberOfParameters)
        {
            // Check if the length of the lower and upper bounds equal the number of parameters
            if (lowerBounds.Count != numberOfParameters || upperBounds.Count != numberOfParameters)
            {
                throw new ArgumentOutOfRangeException(nameof(lowerBounds), "The lower and upper bounds must be the same length as the number of parameters.");
            }
            // Check if the lower bounds are less than the upper bounds
            for (int i = 0; i < lowerBounds.Count; i++)
            {
                if (upperBounds[i] <= lowerBounds[i])
                {
                    throw new ArgumentOutOfRangeException(nameof(upperBounds), "The upper bound cannot be less than or equal to the lower bound.");
                }
            }
            LowerBounds = lowerBounds.ToArray();
            UpperBounds = upperBounds.ToArray();
            CCEIterations = 2 * NumberOfParameters + 1;
        }

        /// <summary>
        /// An array of lower bounds (inclusive) of the interval containing the optimal point. 
        /// </summary>
        public double[] LowerBounds { get; private set; }

        /// <summary>
        /// An array of upper bounds (inclusive) of the interval containing the optimal point.
        /// </summary>
        public double[] UpperBounds { get; private set; }

        /// <summary>
        /// The pseudo random number generator (PRNG) seed.
        /// </summary>
        public int PRNGSeed { get; set; } = 12345;

        /// <summary>
        /// The number of complexes. Default = 5.
        /// </summary>
        public int Complexes { get; set; } = 5;

        /// <summary>
        /// The number of iterations in the inner loop (CCE algorithm).  Defaults = 2 * NumberOfParameters + 1, as recommended by Duan et al (1994). 
        /// </summary>
        public int CCEIterations { get; set; }

        /// <summary>
        /// The number of iterations where the improvement is within the relative tolerance required to confirm convergence. Default = 20.
        /// </summary>
        public int ToleranceSteps { get; set; } = 20;

        /// <summary>
        /// Implements the actual optimization algorithm. This method should minimize the objective function. 
        /// </summary>
        protected override void Optimize()
        {
            if (Complexes < 1) throw new ArgumentOutOfRangeException(nameof(Complexes), "The number of complexes must be greater than 0.");
            if (CCEIterations < 1) throw new ArgumentOutOfRangeException(nameof(CCEIterations), "The number of CCE iterations must be greater than 0.");
            if (ToleranceSteps < 1) throw new ArgumentOutOfRangeException(nameof(ToleranceSteps), "The number of tolerance steps must be greater than 0.");

            int i, j, D = NumberOfParameters;
            bool cancel = false;

            int populationSize = Complexes * CCEIterations; 
            int converge = 0, alpha = 1;
            double oldFit, newFit;

            // Set lower and upper bounds and
            // create uniform distributions for each parameter
            var uniformDists = new List<Uniform>();
            for (i = 0; i < D; i++)
            {
                uniformDists.Add(new Uniform(LowerBounds[i], UpperBounds[i]));
            }

            // Initialize the population of points
            var r = new Random(PRNGSeed);
            var Dpoints = new List<PointFitness>();
            for (i = 0; i < populationSize; i++)
            {
                var values = new double[D];
                for (j = 0; j < D; j++)
                    values[j] = uniformDists[j].InverseCDF(r.NextDouble());

                Dpoints.Add(new PointFitness { ParameterSet = new ParameterSet(values, Evaluate(values, ref cancel)), Index = i });
                if (cancel == true) return;
            }
            oldFit = Dpoints[0].ParameterSet.Fitness;

            Iterations += 1;

            // Initialize shuffled complex evolution (SCE) algorithm
            // Create list of complex prngs
            var complexPRNGs = new List<Random>();
            for (i = 0; i < Complexes; i++)
                complexPRNGs.Add(new Random(r.Next()));

            // create trapezoidal cumulative probability for points in complex
            var cdf = Trapezoidal(CCEIterations);

            // Perform the Shuffled complex evolution loop
            while (Iterations < MaxIterations)
            {

                // Partition populations into p complexes distributing points evenly between complexes
                var Acomplex = new List<List<PointFitness>>(); // The A complex
                for (i = 0; i < Complexes; i++)
                {
                    Acomplex.Add(new List<PointFitness>());
                    for (j = 0; j < CCEIterations; j++)
                        Acomplex[i].Add(Dpoints[i + Complexes * j].Clone());
                }

                // Evolve each complex according to competitive complex evolution (CCE) algorithm
                for (i = 0; i < Complexes; i++)
                {
                    var tmp = Acomplex;
                    var argAcomplex = tmp[i];
                    EvolveComplex(alpha, ref argAcomplex, cdf, complexPRNGs[i], ref cancel);
                    tmp[i] = argAcomplex;
                    // Check cancel
                    if (cancel == true) return;
                }

                // Replace A into D
                for (i = 0; i < Complexes; i++)
                {
                    for (j = 0; j < CCEIterations; j++)
                        Dpoints[i + Complexes * j] = Acomplex[i][j].Clone();
                }

                // Rank the points in terms of fitness (i.e., order of increasing function value).
                Dpoints.Sort((x, y) => x.ParameterSet.Fitness.CompareTo(y.ParameterSet.Fitness));

                // Next reset the indeces, so that i = 0 represents the point With the smallest function value.
                for (i = 0; i <= Dpoints.Count - 1; i++)
                    Dpoints[i].Index = i;

                newFit = Dpoints.First().ParameterSet.Fitness;

                // Check convergence
                if (CheckConvergence(oldFit, newFit))
                {
                    // If no improvement increase alpha to increase chance of sub-complexes evolving
                    converge += 1;
                    alpha = Math.Min(3, converge + 1);
                    oldFit = newFit;

                    if (converge >= ToleranceSteps)
                    {
                        UpdateStatus(OptimizationStatus.Success);
                        return;
                    }

                }
                else
                {
                    oldFit = newFit;
                    converge = 0;
                    alpha = 1;
                }

                Iterations += 1;
            }

            // If we made it to here, the maximum iterations were reached.
            UpdateStatus(OptimizationStatus.MaximumIterationsReached);
        }


        /// <summary>
        /// Evolve complex according to the competitive complex evolution (CCE) algorithm.
        /// </summary>
        /// <param name="alpha">The worst point in the complex is reflected or contracted to seek an improvement alpha times.</param>
        /// <param name="Acomplex">The complex to evolve.</param>
        /// <param name="cdf">The trapezoidal cumulative probability for points in complex.</param>
        /// <param name="prng">The prng.</param>
        /// <param name="cancel">By ref. Determines if the solver should be canceled.</param>
        private void EvolveComplex(int alpha, ref List<PointFitness> Acomplex, double[] cdf, Random prng, ref bool cancel)
        {
            int beta = Acomplex.Count;
            int q = NumberOfParameters + 1;
            int i, j, k;
            var g = new double[NumberOfParameters]; // centroid g
            var p = new double[NumberOfParameters];
            double fitness;

            // The Beta loop. Allow sub-complex to evolve beta times.
            for (i = 0; i < beta; i++)
            {

                // Select parents by randomly choosing q distinct points from A complex
                var B = new List<PointFitness>();
                for (j = 1; j <= q; j++)
                {
                    double rnd = prng.NextDouble();
                    int rndi = cdf.Count() - 1;
                    for (k = 0; k <= cdf.Count() - 1; k++)
                    {
                        if (rnd >= cdf[k])
                        {
                            rndi = k;
                            break;
                        }
                    }

                    B.Add(Acomplex[rndi].Clone());
                    B.Last().Index = rndi;
                }

                // Alpha loop. The worst point in the complex is reflected or contracted to seek an improvement alpha times.
                for (j = 0; j < alpha; j++)
                {

                    // Rank the B points in terms of fitness (i.e., order of increasing function value.
                    B.Sort((x, y) => x.ParameterSet.Fitness.CompareTo(y.ParameterSet.Fitness));

                    // Find centroid g excluding worst point and compute reflection of worst point about centroid r = 2g - u(worst)
                    Reflection(ref B, ref g, ref p);

                    // Check if r is feasible
                    if (IsFeasible(p) == true)
                    {
                        // The parameter set is feasible, so evaluate it.
                        fitness = Evaluate(p, ref cancel);
                        if (cancel == true) return;
                    }
                    else
                    {
                        // The parameters were infeasible, so perform mutation
                        // Compute the smallest hypercube enclosing A complex and randomly sample a point within it. 
                        SmallestHypercube(ref Acomplex, ref prng, ref p);
                        // Evaluate random point
                        fitness = Evaluate(p, ref cancel);
                        if (cancel == true) return;
                    }

                    // Now, either replace worst point with better point
                    if (fitness < B.Last().ParameterSet.Fitness)
                    {
                        B.Last().ParameterSet.Values = p.ToArray();
                        B.Last().ParameterSet.Fitness = fitness;
                    }
                    else
                    {
                        // Or contract to midpoint between centroid and worst point
                        Contraction(ref B, ref g, ref p);
                        // Evaluate contracted point
                        fitness = Evaluate(p, ref cancel);
                        if (cancel == true) return;

                        // If better than worst point replace worst point
                        if (fitness < B.Last().ParameterSet.Fitness)
                        {
                            B.Last().ParameterSet.Values = p.ToArray();
                            B.Last().ParameterSet.Fitness = fitness;
                        }
                        else
                        {
                            // Otherwise perform mutation
                            // Compute the smallest hypercube enclosing A complex and randomly sample a point within it. 
                            SmallestHypercube(ref Acomplex, ref prng, ref p);
                            // Evaluate random point
                            fitness = Evaluate(p, ref cancel);
                            if (cancel == true) return;
                            // Replace worst point with new point regardless of its value
                            B.Last().ParameterSet.Values = p.ToArray();
                            B.Last().ParameterSet.Fitness = fitness;
                        }
                    }
                }

                // Replace B into A according to L And sort A in order of increasing function value
                for (j = 0; j < Acomplex.Count; j++)
                {
                    var best = Acomplex[j].Clone();
                    for (k = 0; k <= q - 1; k++)
                    {
                        if (B[k].Index == j)
                        {
                            if (B[k].ParameterSet.Fitness < best.ParameterSet.Fitness)
                            {
                                best = B[k].Clone();
                            }
                        }
                    }
                    Acomplex[j].ParameterSet.Values = best.ParameterSet.Values;
                    Acomplex[j].ParameterSet.Fitness = best.ParameterSet.Fitness;
                }

                Acomplex.Sort((x, y) => x.ParameterSet.Fitness.CompareTo(y.ParameterSet.Fitness));
            }
        }

        /// <summary>
        /// Determines if the point is within the feasible parameter space.
        /// </summary>
        /// <param name="point">The point to evaluate.</param>
        private bool IsFeasible(double[] point)
        {
            bool feasible = true;
            for (int i = 0; i <= point.Count() - 1; i++)
            {
                if (point[i] < LowerBounds[i] || point[i] > UpperBounds[i])
                {
                    feasible = false;
                    break;
                }
            }
            return feasible;
        }

        /// <summary>
        /// Compute reflection of worst point about centroid r = 2g - u(worst)
        /// </summary>
        /// <param name="U">The sub-complex.</param>
        /// <param name="g">The centroid.</param>
        /// <param name="r">The reflection point.</param>
        private void Reflection(ref List<PointFitness> U, ref double[] g, ref double[] r)
        {
            for (int i = 0; i < NumberOfParameters; i++)
            {
                g[i] = 0.0d;
                for (int j = 0; j < U.Count - 1; j++)
                    g[i] += U[j].ParameterSet.Values[i];
                g[i] /= U.Count - 1;
                r[i] = 2d * g[i] - U.Last().ParameterSet.Values[i]; // u(worst)
            }
        }

        /// <summary>
        /// Contract to midpoint between centroid and worst point
        /// </summary>
        /// <param name="U">The sub-complex.</param>
        /// <param name="g">The centroid.</param>
        /// <param name="c">The contraction point.</param>
        private void Contraction(ref List<PointFitness> U, ref double[] g, ref double[] c)
        {
            for (int i = 0; i < NumberOfParameters; i++)
                c[i] = 0.5d * (g[i] + U.Last().ParameterSet.Values[i]); // u(worst)
        }

        /// <summary>
        /// Compute the smallest hypercube enclosing A complex and randomly sample a point within it.
        /// </summary>
        /// <param name="Acomplex">The complex.</param>
        /// <param name="prng">The prng.</param>
        /// <param name="z">The randomly generated point.</param>
        private void SmallestHypercube(ref List<PointFitness> Acomplex, ref Random prng, ref double[] z)
        {
            double low, high;
            for (int i = 0; i < NumberOfParameters; i++)
            {
                low = Acomplex[0].ParameterSet.Values[i];
                high = low;
                for (int j = 1; j < Acomplex.Count; j++)
                {
                    low = Math.Min(low, Acomplex[j].ParameterSet.Values[i]);
                    high = Math.Max(high, Acomplex[j].ParameterSet.Values[i]);
                }
                z[i] = low + (high - low) * prng.NextDouble();
            }
        }

        /// <summary>
        /// The Trapezoidal probability distribution.
        /// </summary>
        /// <param name="N">The sample size.</param>
        /// <returns>An array of plotting positions of size N.</returns>
        private double[] Trapezoidal(int N)
        {
            var PP = new double[N];
            for (int i = 1; i <= N; i++)
                PP[i - 1] = 2 * (N + 1 - i) / (double)(N * (N + 1));
            return PP;
        }

        /// <summary>
        /// Class for keeping track of parameter sets with indices. 
        /// </summary>
        private class PointFitness
        {
            public ParameterSet ParameterSet;
            public int Index;
            public PointFitness Clone()
            {
                return new PointFitness { ParameterSet = ParameterSet.Clone(), Index = Index };
            }
        }

    }
}