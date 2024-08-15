﻿/**
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

using Numerics.Data.Statistics;
using Numerics.Distributions;
using Numerics.Mathematics;
using Numerics.Mathematics.LinearAlgebra;
using Numerics.Mathematics.Optimization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Numerics.Sampling.MCMC
{
    /// <summary>
    /// This class performs Bayesian MCMC using the Hamiltonian Monte Carlo (HMC) method.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     <b> Authors: </b>
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    /// <b> Description:</b>
    /// </para>
    /// <para>
    ///     The optimal acceptance rate for this sampler is 65%, whereas Metropolis-Hastings samplers have an optimal rate of 23.4%. 
    /// </para>
    /// <para>
    /// <b> References: </b>
    /// </para>
    /// <para>
    ///    <see href="https://en.wikipedia.org/wiki/Hamiltonian_Monte_Carlo"/>
    /// </para>
    /// </remarks>
    [Serializable]
    public class HMC : MCMCSampler
    {

        /// <summary>
        /// The function for evaluating the gradient of the log-likelihood function.
        /// </summary>
        /// <param name="parameters">The list of parameters to evaluate.</param>
        /// <returns>Returns the gradient of the log-likelihood function.</returns>
        public delegate Vector Gradient(IList<double> parameters);

        /// <summary>
        /// Constructs a new HMC sampler.
        /// </summary>
        /// <param name="priorDistributions"></param>
        /// <param name="logLikelihoodFunction"></param>
        /// <param name="mass">Optional. The mass vector for the momentum distribution. Default = Identity.</param>
        /// <param name="stepSize">Optional. The leapfrog step size. Default = 1. The rule of thumb is that the step size x steps = 1.</param>
        /// <param name="steps">Optional. The number of leapfrog steps. Default = 1. The rule of thumb is that the step size x steps = 1.</param>
        /// <param name="gradientFunction">Optional. The function for evaluating the gradient of the log-likelihood. Numerical finite difference will be used by default.</param>
        public HMC(List<IUnivariateDistribution> priorDistributions, LogLikelihood logLikelihoodFunction, Vector mass = null, double stepSize = 1, int steps = 1, Gradient gradientFunction = null)
        {
            PriorDistributions = priorDistributions;
            LogLikelihoodFunction = logLikelihoodFunction;
            InitialPopulationLength = 100 * NumberOfParameters;
           
            // Set the mass vector 
            if (mass == null)
            {
                Mass = new Vector(NumberOfParameters, 1d);
            }
            else
            {
                Mass = mass;
            }
       
            // Set the inverse mass vector
            _inverseMass = new Vector(NumberOfParameters);
            for (int i = 0; i < NumberOfParameters; i++)
                _inverseMass[i] = 1d / Mass[i];

            // Set leapfrog inputs
            StepSize = stepSize;
            Steps = steps;
            _stepSizeU = new Uniform(0, 2 * StepSize);
            _stepsU = new Uniform(0, 2 * Steps);

            // Set the gradient function
            if (gradientFunction == null)
            {
                GradientFunction = (x) => new Vector(NumericalDerivative.Gradient((y) => LogLikelihoodFunction(y), x.ToArray()));
            }
            else
            {
                GradientFunction = gradientFunction;
            }

        }

        private Uniform _stepSizeU;
        private Uniform _stepsU;
        private Vector _inverseMass;

        /// <summary>
        /// The mass vector for the momentum distribution.
        /// </summary>
        public Vector Mass { get; set; }

        /// <summary>
        /// The leapfrog step size. The rule of thumb is that the step size x steps = 1.
        /// </summary>
        public double StepSize { get; set; }

        /// <summary>
        /// The number of leapfrog steps. The rule of thumb is that the step size x steps = 1.
        /// </summary>
        public int Steps { get; set; }

        /// <summary>
        /// The function for evaluating the gradient of the log-likelihood.
        /// </summary>
        public Gradient GradientFunction { get; private set; }

        /// <summary>
        /// Validate any custom MCMC sampler settings. 
        /// </summary>
        protected override void ValidateCustomSettings()
        {
            if (Mass.Length != NumberOfParameters) throw new ArgumentException(nameof(Mass), "The mass vector must be the same length as the number of parameters.");
            if (StepSize < 0) throw new ArgumentException(nameof(Mass), "The leapfrog step size must be positive.");
            if (Steps < 1) throw new ArgumentException(nameof(Steps), "The number of leapfrog steps must be at least one.");
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

            // Jigger the step size and number of steps
            var _stepSize = _stepSizeU.InverseCDF(_chainPRNGs[index].NextDouble());
            var _steps = (int)Math.Ceiling(_stepsU.InverseCDF(_chainPRNGs[index].NextDouble()));

            // Step 1. Sample phi from a N~(0,M)
            var phi = new Vector(NumberOfParameters);
            for (int i = 0; i < NumberOfParameters; i++)
                phi[i] = Math.Sqrt(Mass[i]) * Normal.StandardZ(_chainPRNGs[index].NextDouble());

            // Get kinetic energy of the current state
            var logKi = -0.5 * Statistics.Sum((_inverseMass * phi * phi).ToArray());

            // Step 2. Perform leapfrog steps to get proposal vector
            var xp = new Vector(state.Values.ToArray());
            phi += GradientFunction(xp.ToArray()) * _stepSize * 0.5;
            for (int i = 0; i < _steps; i++)
            {
                xp += _inverseMass * phi * _stepSize;

                // Ensure the parameters are feasible (within the constraints)
                for (int j = 0; j < NumberOfParameters; j++)
                {
                    if (xp[j] < PriorDistributions[i].Minimum) 
                        xp[j] = PriorDistributions[i].Minimum + Tools.DoubleMachineEpsilon;
                    if (xp[j] > PriorDistributions[i].Maximum) 
                        xp[j] = PriorDistributions[i].Maximum - Tools.DoubleMachineEpsilon;
                }

                phi += GradientFunction(xp.ToArray()) * _stepSize * (i == _steps-1 ? 0.5: 1.0);
            }
            phi *= -1d;

            // Get kinetic energy of the proposal state
            var logKp = -0.5 * Statistics.Sum((_inverseMass * phi * phi).ToArray());

            // Evaluate fitness
            var logLHp = LogLikelihoodFunction(xp.ToArray());
            var logLHi = state.Fitness;

            // Calculate the Metropolis ratio
            var logRatio = logLHp - logLHi + logKp - logKi;

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
