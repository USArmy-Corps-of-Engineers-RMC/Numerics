using Numerics.Data.Statistics;
using Numerics.Distributions;
using Numerics.Mathematics.LinearAlgebra;
using Numerics.Sampling;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Numerics.Mathematics.Optimization
{

    /// <summary>
    /// The adaptive simulated annealing algorithm.
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
    ///     Implements routine described by Corana et al. "Minimizing Multimodal Functions of Continuous Variables with 'Simulated Annealing' Algorithm" (1987).
    /// </para>
    /// </remarks>
    public class SimulatedAnnealing : Optimizer
    {

        /// <summary>
        /// Construct a new simulated annealing optimization method. 
        /// </summary>
        /// <param name="objectiveFunction">The objective function to evaluate.</param>
        /// <param name="numberOfParameters">The number of parameters in the objective function.</param>
        /// <param name="lowerBounds">An array of lower bounds (inclusive) of the interval containing the optimal point.</param>
        /// <param name="upperBounds">An array of upper bounds (inclusive) of the interval containing the optimal point.</param>
        public SimulatedAnnealing(Func<double[], double> objectiveFunction, int numberOfParameters, IList<double> lowerBounds, IList<double> upperBounds) : base(objectiveFunction, numberOfParameters)
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
        /// The pseudo random number generator (PRNG) seed.
        /// </summary>
        public int PRNGSeed { get; set; } = 12345;

        /// <summary>
        /// The initial temperature at the start of the algorithm. Default = 10. 
        /// </summary>
        public double InitialTemperature { get; set; } = 10;
        
        /// <summary>
        /// The minimum temperature allowable. The temperature will be held constant when it reaches this point. 
        /// </summary>
        public double MinTemperature { get; set; } = 0.1;

        /// <summary>
        /// The number of cycles before updating the step size. Default = 10.
        /// </summary>
        public int UpdateCycles { get; set; } = 4;

        /// <summary>
        /// The number of cycles before reducing the temperature. 10. 
        /// </summary>
        public int TemperatureCycles { get; set; } = 10;

        /// <summary>
        /// The number of successive temperature reductions to test for termination. Default = 20.
        /// </summary>
        public int ToleranceSteps { get; set; } = 20;

        /// <summary>
        /// Implements the actual optimization algorithm. This method should minimize the objective function. 
        /// </summary>
        protected override void Optimize()
        {
            int i, j, k, D = NumberOfParameters;

            // Validate
            if (InitialTemperature < 1) throw new ArgumentOutOfRangeException(nameof(InitialTemperature), "The initial temperature must be greater than 0.");
            if (UpdateCycles < 4) throw new ArgumentOutOfRangeException(nameof(UpdateCycles), "The number of cycles before updating the step size must be at least 4.");
            if (TemperatureCycles < 4) throw new ArgumentOutOfRangeException(nameof(TemperatureCycles), "The number of cycles before reducing the temperature must be at least 4.");
            if (ToleranceSteps < 4) throw new ArgumentOutOfRangeException(nameof(ToleranceSteps), "The number of tolerance steps must be greater than 3.");
 
            // Setup variables
            double T = InitialTemperature;  
            bool cancel = false;
            var x = new Vector(D);
            var xp = new Vector(D);
            var fitness = new List<double>();
            var r = new MersenneTwister(PRNGSeed);

            // Initial evaluation
            for (j = 0; j < D; j++) x[j] = 0.5 * (LowerBounds[j] + UpperBounds[j]);
            double fx = Evaluate(x.ToArray(), ref cancel);
            double fxp, df; 
            
            // Keep track of fitness statistics to assess convergence
            var statistics = new RunningStatistics();

            // variables for corona update
            var acceptances = new int[D];
            acceptances.Fill(0);
            var v = new double[D];
            v.Fill(1d / InitialTemperature);
            var c = new double[D];
            c.Fill(2);

           // Iterations += 1;

            // Perform adaptive simulated annealing
            while (Iterations < MaxIterations)
            {

                // Reduce temperature
                // Temperature annealing schedule
                T = InitialTemperature / Math.Log((double)Iterations + (Math.Exp(1d) - 1d));
                T = Math.Max(MinTemperature, T);

                for (i = 1; i <= TemperatureCycles; i++)
                {

                    for (j = 1; j <= UpdateCycles; j++)
                    {

                        // Perform a cycle of random moves, each along one coordinate direction. 
                        // Accept or reject each point according to the Metropolis criterion. 
                        // Record the optimum point reached so far.             
                        for (k = 0; k < D; k++)
                        {
                            // Create basis vector
                            var basis = new Vector(D, 0);
                            basis[k] = 1;

                            // Create proposal vector
                            xp = x + basis * v[k] * (r.NextDouble() * 2d - 1d);

                            // If XP is out of bounds, continue
                            if (xp[k] < LowerBounds[k] || xp[k] > UpperBounds[k]) continue;
                                
                            // Evaluate proposal vector
                            fxp = Evaluate(xp.ToArray(), ref cancel);
                            if (cancel) return;
                            df = fxp - fx;

                            // Metropolis rule
                            if (df < 0 || r.NextDouble() < Math.Exp(-df / T))
                            {
                                // Accept proposal
                                acceptances[k] += 1;
                                x = xp.Clone();
                                fx = fxp;
                            }
                        }
                    }

                    // Do Corana et al. update
                    // Goal is to keep the acceptance rate near 50%
                    for (j = 0; j < D; j++)
                    {
                        // Get acceptance rate
                        double rate = (double)acceptances[j] / UpdateCycles;
                        if (rate > 0.6)
                        {
                            v[j] *= 1d + c[j] * (rate - 0.6) / 0.4;
                        }
                        else if (rate < 0.4)
                        {
                            v[j] /= 1d + c[j] * (0.4 - rate) / 0.4;
                        }
                    }
                    acceptances.Fill(0);
                }

                Iterations += 1;

                //statistics.Push(fx);

                // Set current point to the optimum.           
                x = new Vector(BestParameterSet.Values);
                fx = BestParameterSet.Fitness;
                

                //// Number of successive temperature reductions to test for termination
                //if (statistics.Count >= ToleranceSteps)
                //{
                //    // Test for convergence
                //    if (statistics.StandardDeviation < AbsoluteTolerance + RelativeTolerance * Math.Abs(statistics.Mean))
                //    {
                //        UpdateStatus(OptimizationStatus.Success);
                //        return;
                //    }

                //    statistics = new RunningStatistics();
                //}

            }

            // If we made it to here, the maximum iterations were reached.
            UpdateStatus(OptimizationStatus.MaximumIterationsReached);
        }

    }
}
