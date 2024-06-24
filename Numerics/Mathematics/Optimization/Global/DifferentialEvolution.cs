using Numerics.Data.Statistics;
using Numerics.Distributions;
using Numerics.Sampling;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Numerics.Mathematics.Optimization
{

    /// <summary>
    /// The Differential Evolution (DE) algorithm, which finds a global minima when no gradient is available.
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
    ///     Implements routine described by Price et al. "Differential Evolution: A Practical Approach to Global Optimization" (1998).
    /// </para>
    /// </remarks>
    public class DifferentialEvolution : Optimizer
    {

        /// <summary>
        /// Construct a new differential evolution optimization method. 
        /// </summary>
        /// <param name="objectiveFunction">The objective function to evaluate.</param>
        /// <param name="numberOfParameters">The number of parameters in the objective function.</param>
        /// <param name="lowerBounds">An array of lower bounds (inclusive) of the interval containing the optimal point.</param>
        /// <param name="upperBounds">An array of upper bounds (inclusive) of the interval containing the optimal point.</param>
        public DifferentialEvolution(Func<double[], double> objectiveFunction, int numberOfParameters, IList<double> lowerBounds, IList<double> upperBounds) : base(objectiveFunction, numberOfParameters)
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
        /// The total population size. Default = 30.
        /// </summary>
        public int PopulationSize { get; set; } = 30;

        /// <summary>
        /// The pseudo random number generator (PRNG) seed.
        /// </summary>
        public int PRNGSeed { get; set; } = 12345;

        /// <summary>
        /// The mutation constant or differential weight, in the range [0, 2]. Increasing the mutation constant increases the search radius, 
        /// but will slow down convergence. The default is 0.75. 
        /// </summary>
        public double Mutation { get; set; } = 0.75;

        /// <summary>
        /// Determines how often the mutation constant dithers between 0.5 and 1.0; e.g., 0.90 will result in dithering 90% of the time.
        /// </summary>
        public double DitherRate { get; set; } = 0.9;

        /// <summary>
        /// The crossover probability or recombination constant, in the range [0, 1]. Increasing this value allows a larger number of mutants
        /// to progress into the next generation, but at the risk of population stability. 
        /// </summary>
        public double CrossoverProbability { get; set; } = 0.9;

        /// <summary>
        /// Implements the actual optimization algorithm. This method should minimize the objective function. 
        /// </summary>
        protected override void Optimize()
        {
            if (PopulationSize < 1) throw new ArgumentOutOfRangeException(nameof(PopulationSize), "The population size must be greater than 0.");
            if (Mutation < 0 || Mutation > 2) throw new ArgumentOutOfRangeException(nameof(Mutation), "The mutation parameter must be between 0 and 2.");
            if (DitherRate < 0 || DitherRate > 1) throw new ArgumentOutOfRangeException(nameof(DitherRate), "The dithering rate must be between 0 and 1.");
            if (CrossoverProbability < 0 || CrossoverProbability > 1) throw new ArgumentOutOfRangeException(nameof(CrossoverProbability), "The crossover probability must be between 0 and 1.");

            int i, j, D = NumberOfParameters;
            bool cancel = false;

            // Initialize the population of points
            var r = new MersenneTwister(PRNGSeed);
            var Xp = new List<ParameterSet>();
            for (i = 0; i < PopulationSize; i++)
            {
                var values = new double[D];
                for (j = 0; j < D; j++)
                    values[j] = LowerBounds[j] + r.NextDouble() * (UpperBounds[j] - LowerBounds[j]); 

                Xp.Add(new ParameterSet(values, Evaluate(values, ref cancel)));
                if (cancel == true) return;
            }

            Iterations += 1;

            // Perform Differential Evolution
            while (Iterations < MaxIterations)
            {
                // Keep track of population statistics to assess convergence
                var statistics = new RunningStatistics();

                // Mutate and recombine population
                for (i = 0; i < PopulationSize; i++)
                {

                    // Randomly select three vectors indexes
                    int r0 = i, r1 = i, r2 = i;
                    do r0 = (int)Math.Floor(r.NextDouble() * PopulationSize); while (r0 == i);
                    do r1 = (int)Math.Floor(r.NextDouble() * PopulationSize); while (r1 == i || r1 == r0);
                    do r2 = (int)Math.Floor(r.NextDouble() * PopulationSize); while (r2 == i || r2 == r1 || r2 == r0);

                    // Generate trial vector
                    double G = r.NextDouble() <= DitherRate ? 0.5 + r.NextDouble() * 0.5 : Mutation; // Scale factor
                    double jRand = Math.Floor(r.NextDouble() * D);
                    var u = new double[D];
                    for (j = 0; j < D; j++)
                    {
                        var rr = r.NextDouble();
                        if (rr <= CrossoverProbability || rr == jRand)
                        {
                            u[j] = Xp[r0].Values[j] + G * (Xp[r1].Values[j] - Xp[r2].Values[j]);
                            u[j] = RepairParameter(u[j], LowerBounds[j], UpperBounds[j]);
                        }
                        else
                        {
                            u[j] = Xp[i].Values[j];
                        }
                    }

                    // Evaluate fitness
                    var fitness = Evaluate(u, ref cancel);
                    if (cancel == true) return;

                    // Update population
                    if (fitness <= Xp[i].Fitness)
                    {
                        Xp[i] = new ParameterSet(u, fitness);
                    }
              
                    // Keep running stats of population
                    statistics.Push(Xp[i].Fitness);

                }

                // Evaluate convergence 
                if (Iterations >= 10 && statistics.StandardDeviation < AbsoluteTolerance + RelativeTolerance * Math.Abs(statistics.Mean))
                {
                    UpdateStatus(OptimizationStatus.Success);
                    return;
                }

                Iterations += 1;

            }

            // If we made it to here, the maximum iterations were reached.
            UpdateStatus(OptimizationStatus.MaximumIterationsReached);
        }
        
    }
}
