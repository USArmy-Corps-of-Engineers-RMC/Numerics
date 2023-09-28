using Numerics.Data.Statistics;
using Numerics.Sampling;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Numerics.Mathematics.Optimization
{
    /// <summary>
    /// The Particle Swarm optimization algorithm.
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
    ///     Implements routine described by Kockenderfer and Wheeler. "Algorithms for Optimization" (2019).
    /// </para>
    /// </remarks>
    public class ParticleSwarm : Optimizer
    {

        /// <summary>
        /// Construct a new particle swarm optimization method. 
        /// </summary>
        /// <param name="objectiveFunction">The objective function to evaluate.</param>
        /// <param name="numberOfParameters">The number of parameters in the objective function.</param>
        /// <param name="lowerBounds">An array of lower bounds (inclusive) of the interval containing the optimal point.</param>
        /// <param name="upperBounds">An array of upper bounds (inclusive) of the interval containing the optimal point.</param>
        public ParticleSwarm(Func<double[], double> objectiveFunction, int numberOfParameters, IList<double> lowerBounds, IList<double> upperBounds) : base(objectiveFunction, numberOfParameters)
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
        /// Implements the actual optimization algorithm. This method should minimize the objective function. 
        /// </summary>
        protected override void Optimize()
        {
            if (PopulationSize < 1) throw new ArgumentOutOfRangeException(nameof(PopulationSize), "The population size must be greater than 0.");

            int i, j, D = NumberOfParameters;
            bool cancel = false;

            // These parameters are recommended by Alam "Particle Swarm Optimization: Algorithm and its Codes in MATLAB" (2016)
            double wmin = 0.4;
            double wmax = 0.9;
            double c1 = 2.05;
            double c2 = 2.05;

            // Initialize the population of points
            var r = new MersenneTwister(PRNGSeed);
            var Xp = new List<Particle>();
            for (i = 0; i < PopulationSize; i++)
            {
                var values = new double[D];
                var velocity = new double[D];
                for (j = 0; j < D; j++)
                {
                    values[j] = LowerBounds[j] + r.NextDouble() * (UpperBounds[j] - LowerBounds[j]);
                    velocity[j] = values[j] * 0.1;
                }
                    
                var fitness = Evaluate(values, ref cancel);
                Xp.Add(new Particle { ParameterSet = new ParameterSet(values, fitness),
                                      BestParameterSet = new ParameterSet(values, fitness),
                                      Velocity = velocity});
                if (cancel == true) return;
            }

            Iterations += 1;

            // Perform Particle Swarm
            while (Iterations < MaxIterations)
            {
                // Keep track of population statistics to assess convergence
                var statistics = new RunningStatistics();

                // Update momentum for each particle
                for (i = 0; i < PopulationSize; i++)
                {
                    // Generate trial vector
                    var u = new double[D];
                    var v = new double[D];
                    for (j = 0; j < D; j++)
                    {
                        // New velocity
                        var w = wmax - (wmax - wmin) * Iterations / MaxIterations;
                        v[j] = w * Xp[i].Velocity[j] + c1 * r.NextDouble() * (Xp[i].BestParameterSet.Values[j] - Xp[i].ParameterSet.Values[j]) 
                                                     + c2 * r.NextDouble() * (BestParameterSet.Values[j] - Xp[i].ParameterSet.Values[j]);                      
                        // New position
                        u[j] = Xp[i].ParameterSet.Values[j] + v[j];
                        u[j] = RepairParameter(u[j], LowerBounds[j], UpperBounds[j]);
                    }

                    // Evaluate fitness
                    var fitness = Evaluate(u, ref cancel);
                    if (cancel == true) return;

                    Xp[i].ParameterSet.Values = u.ToArray();
                    Xp[i].ParameterSet.Fitness = fitness;
                    Xp[i].Velocity = v.ToArray();

                    // Update population
                    if (fitness <= Xp[i].BestParameterSet.Fitness)
                    {
                        Xp[i].BestParameterSet.Values = u.ToArray();
                        Xp[i].BestParameterSet.Fitness = fitness;
                    }
             
                    // Keep running stats of population
                    statistics.Push(Xp[i].BestParameterSet.Fitness);
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

        /// <summary>
        /// Class for storing particles. 
        /// </summary>
        private class Particle
        {
            public ParameterSet ParameterSet { get; set; }
            public ParameterSet BestParameterSet { get; set; }
            public double[] Velocity { get; set; }
        }


    }
}
