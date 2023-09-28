using Numerics.Data.Statistics;
using Numerics.Distributions;
using Numerics.Mathematics.LinearAlgebra;
using Numerics.Mathematics.Optimization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Numerics.Sampling.MCMC
{
    /// <summary>
    /// This class performs Bayesian MCMC using the adaptive Differential Evolution Markov Chain with snooker update (DE-MCzs) method.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    ///     References:
    /// <list type="bullet">
    /// <item><description>
    ///     This class is based on an algorithm described in:
    ///     Braak and Vrugt "Differential Evolution Markov Chain with snooker updater
    ///     and fewer chains" (2008) Statistics and Computing.
    /// </description></item>
    /// </list>
    /// </para>
    /// </remarks>
    [Serializable]
    public class DEMCzs : MCMCSampler
    {

        /// <summary>
        /// Constructs a new DEMCzs sampler.
        /// </summary>
        /// <param name="priorDistributions">The list of prior distributions for the model parameters.</param>
        /// <param name="logLikelihoodFunction">The Log-Likelihood function to evaluate.</param>
        public DEMCzs(List<IUnivariateDistribution> priorDistributions, LogLikelihood logLikelihoodFunction)
        {
            PriorDistributions = priorDistributions;
            LogLikelihoodFunction = logLikelihoodFunction;
            InitialPopulationLength = 100 * NumberOfParameters;
            // DE-MCz options
            UpdatePopulationMatrix = true;
            // Jump parameter. Default = 2.38/SQRT(2*D)
            Jump = 2.38d / Math.Sqrt(2.0d * NumberOfParameters);
            // Adaptation threshold. Default = 0.1 or 10% of the time. 
            JumpThreshold = 0.1d;
            // Snooker update. Default = 0.1 or 10% of the time.
            SnookerThreshold = 0.1d;
            _b = new Uniform(-_noise, _noise);
            _g = new Uniform(1.2, 2.2);
        }

        private double _noise = 1E-8;
        private Uniform _b;
        private Uniform _g;

        /// <summary>
        /// The jumping parameter used to jump from one mode region to another in the target distribution.
        /// </summary>
        public double Jump { get; set; }

        /// <summary>
        /// Determines how often the jump parameter switches to 1.0; e.g., 0.10 will result in adaptation 10% of the time.
        /// </summary>
        public double JumpThreshold { get; set; }

        /// <summary>
        /// Determines how often to perform the snooker update; e.g., 0.10 will result in an update 10% of the time. 
        /// </summary>
        public double SnookerThreshold { get; set; }

        /// <summary>
        /// The noise parameter (b).
        /// </summary>
        public double Noise
        {
            get { return _noise; }
            set
            {
                _noise = value;
                _b = new Uniform(-_noise, _noise);
            }
        }

        /// <summary>
        /// Validate any custom MCMC sampler settings. 
        /// </summary>
        protected override void ValidateCustomSettings()
        {
            if (Jump <= 0 || Jump >= 2) throw new ArgumentException(nameof(Jump), "The jump parameter must be between 0 and 2.");
            if (JumpThreshold < 0 || JumpThreshold > 1) throw new ArgumentException(nameof(JumpThreshold), "The jump threshold must be between 0 and 1.");
            if (SnookerThreshold < 0 || SnookerThreshold > 0.5) throw new ArgumentException(nameof(SnookerThreshold), "The snooker threshold must be between 0 and 0.5.");
            if (Noise < 0) throw new ArgumentException(nameof(JumpThreshold), "The noise parameter must be greater than 0.");

        }

        /// <summary>
        /// Returns a proposed MCMC iteration. 
        /// </summary>
        /// <param name="index">The Markov Chain zero-based index.</param>
        /// <param name="state">The current chain state to compare against.</param>
        protected override ParameterSet ChainIteration(int index, ParameterSet state)
        {
            // Update the sample count
            SampleCount[index] += 1;

            // 10% snooker updates and 90% parallel direction updates.
            if (_chainPRNGs[index].NextDouble() <= SnookerThreshold && SampleCount[index] > 2 * ThinningInterval)
            {
                return SnookerUpdate(index, state);
            }
            else
            {

                // The adaptation for the algorithm to allow for 
                // jumps from one mode region to another in the target
                // distribution.         
                var G = _chainPRNGs[index].NextDouble() <= JumpThreshold ? 1.0d : Jump;

                // Sample uniformly at random without replacement two numbers R1 and R2
                // from the numbers 1, 2, ..., M. M = Population (Z) Length.
                int r1, r2, Z = PopulationMatrix.Count;
                r1 = _chainPRNGs[index].Next(0, Z);
                do r2 = _chainPRNGs[index].Next(0, Z); while (r2 == r1);

                // Calculate the proposal vector
                // x* ← xi + γ(zR1 − zR2) + e
                // where zR1 and zR2 are rows R1 and R2 of Z.
                var xp = new double[NumberOfParameters];
                for (int i = 0; i < NumberOfParameters; i++)
                {
                    var xi = state.Values[i];
                    var zr1 = PopulationMatrix[r1].Values[i];
                    var zr2 = PopulationMatrix[r2].Values[i];
                    var e = _b.InverseCDF(_chainPRNGs[index].NextDouble());
                    xp[i] = xi + G * (zr1 - zr2) + e;

                    // Check if the parameter is feasible (within the constraints)
                    if (xp[i] < PriorDistributions[i].Minimum || xp[i] > PriorDistributions[i].Maximum)
                    {
                        // The proposed parameter vector was infeasible, 
                        // so leave xi unchanged.
                        return state;
                    }
                }

                // Evaluate fitness
                var logLHp = LogLikelihoodFunction(xp);
                var logLHi = state.Fitness;

                // Calculate the Metropolis ratio
                var logRatio = logLHp - logLHi;

                // Accept the proposal with probability min(1,r)
                // otherwise leave xi unchanged
                var logU = Math.Log(_chainPRNGs[index].NextDouble());
                if (logU <= logRatio)
                {
                    // The proposal is accepted
                    AcceptCount[index] += 1;
                    return new ParameterSet(xp, logLHp);
                }
                else
                {
                    return state;
                }

            }
        }

        /// <summary>
        /// Returns a proposed MCMC iteration based on the Snooker Update method. 
        /// </summary>
        /// <param name="index">The Markov Chain zero-based index.</param>
        /// <param name="state">The current chain state to compare against.</param>
        private ParameterSet SnookerUpdate(int index, ParameterSet state)
        {
            // Do snooker update
            // Get Jump -- uniform random number between 1.2 and 2.2         
            var G = _g.InverseCDF(_chainPRNGs[index].NextDouble());

            // Select another chain, which is in state z
            int c = index;
            do c = _chainPRNGs[index].Next(0, NumberOfChains); while (c == index);
            // Select two other random chains, zR1 and zR2
            int c1 = c, c2 = c;
            do c1 = _chainPRNGs[index].Next(0, NumberOfChains); while (c1 == c);
            do c2 = _chainPRNGs[index].Next(0, NumberOfChains); while (c2 == c1 || c2 == c);
            int M1 = MarkovChains[c1].Count;
            int M2 = MarkovChains[c2].Count;
            int r1, r2;
            r1 = _chainPRNGs[index].Next(0, M1);
            do r2 = _chainPRNGs[index].Next(0, M2); while (M2 > 1 && r2 == r1);

            // Define z
            var z = new Vector(MarkovChains[c].Last().Values.ToArray());
            var xi = new Vector(state.Values.ToArray());
            // Define projection vector xi - z
            var F = xi - z;
            var D = Math.Max(Vector.DotProduct(F, F), double.Epsilon);
            // Orthogonally project zR1 and zR2 onto F
            var zr1 = new Vector(MarkovChains[c1].Last().Values.ToArray());
            var zr2 = new Vector(MarkovChains[c2].Last().Values.ToArray());
            var zp = F * (Statistics.Sum(((zr1 - zr2) * F).ToArray()) / D);
            // Calculate the proposal vector
            // x* ← xi + γ(zP1 − zP2)
            var xp = new Vector(NumberOfParameters);
            for (int i = 0; i < NumberOfParameters; i++)
            {
                xp[i] = xi[i] + G * zp[i];
                // Check if the parameter is feasible (within the constraints)
                if (xp[i] < PriorDistributions[i].Minimum || xp[i] > PriorDistributions[i].Maximum)
                {
                    // The proposed parameter vector was infeasible, 
                    // so leave xi unchanged.
                    return state;
                }
            }

            // Evaluate fitness
            var logLHp = LogLikelihoodFunction(xp.ToArray());
            var logLHi = state.Fitness;

            // Get the Euclidean distance
            var logEDp = Math.Log(Math.Pow(Math.Sqrt(Statistics.Sum(((xp - z) ^ 2).ToArray())), NumberOfParameters - 1));
            var logEDi = Math.Log(Math.Pow(Math.Sqrt(Statistics.Sum(((xi - z) ^ 2).ToArray())), NumberOfParameters - 1));

            // Calculate the Metropolis ratio
            var logRatio = logLHp + logEDp - logLHi - logEDi;
            // Accept the proposal with probability min(1,r)
            // otherwise leave xi unchanged
            var logU = Math.Log(_chainPRNGs[index].NextDouble());
            if (logU <= logRatio)
            {
                // The proposal is accepted
                AcceptCount[index] += 1;
                return new ParameterSet(xp.ToArray(), logLHp);
            }
            else
            {
                return state;
            }
        }

    }
}
