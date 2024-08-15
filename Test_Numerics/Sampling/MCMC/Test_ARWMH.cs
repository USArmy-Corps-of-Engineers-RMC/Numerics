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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Data.Statistics;
using Numerics.Distributions;
using Numerics.Sampling.MCMC;

namespace Sampling.MCMC
{
    /// <summary>
    /// A class testing the ARWMH method.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     <b> Authors: </b>
    ///     <list type="bullet"> 
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    ///     <item> Tiki Gonzalez, USACE Risk Management Center, julian.t.gonzalez@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    [TestClass]
    public class Test_ARWMH
    {
        /// <summary>
        /// Demonstrates how to use the adaptive Random Walk Metropolis Hastings (ARWMH) sampler with a normal distribution.
        /// </summary>
        [TestMethod]
        public void Test_ARWMH_NormalDist()
        {
            double popMU = 100, popSigma = 15;
            int n = 200;
            var normDist = new Normal(popMU, popSigma);
            var sample = normDist.GenerateRandomValues(12345, n);
            var mu = Statistics.Mean(sample);
            var sigma = Statistics.StandardDeviation(sample);
            var priors = new List<IUnivariateDistribution> { new Uniform(-10000, 10000), new Uniform(0, 1000) };

            var sampler = new ARWMH(priors, x =>
            {
                var norm = new Normal(x[0], x[1]);
                return norm.LogLikelihood(sample);
            });

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            sampler.InitializeWithMAP = true;
            sampler.Sample();
            var results = new MCMCResults(sampler);

            //// This code block prints the estimated parameter results for the mean ~100
            //var pdf = results.ParameterResults[0].KernelDensity;
            //for (int j = 0; j < pdf.GetLength(0); j++)
            //    Debug.Print(pdf[j, 0] + ", " + pdf[j, 1]);

            ////This code block prints the estimated parameter results for the standard deviation ~15 
            //var pdf = results.ParameterResults[1].KernelDensity;
            //for (int j = 0; j < pdf.GetLength(0); j++)
            //    Debug.Print(pdf[j, 0] + ", " + pdf[j, 1]);

            // Write out results
            stopWatch.Stop();
            var timeSpan = stopWatch.Elapsed;
            string elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10d);
            Debug.WriteLine("Runtime: " + elapsedTime);

        }

        /// <summary>
        /// Demonstrates how to use the adaptive Random Walk Metropolis Hastings (ARWMH) sampler.
        /// </summary>
        [TestMethod]
        public void Test_ARWMH_BetaDist()
        {
            double popMU = 100, popSigma = 15;
            int n = 200;
            var normDist = new Normal(popMU, popSigma);
            var sample = normDist.GenerateRandomValues(12345, n);
            var mu = Statistics.Mean(sample);
            var sigma = Statistics.StandardDeviation(sample);
            var priors = new List<IUnivariateDistribution> { new Uniform(-10000, 10000), new Uniform(0, 1000) };

            var sampler = new ARWMH(priors, x =>
            {
                var norm = new Normal(x[0], x[1]);
                return norm.LogLikelihood(sample);
            });

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            sampler.InitializeWithMAP = true;
            sampler.Sample();
            var results = new MCMCResults(sampler);

            //// This code block prints the estimated parameter results for the mean ~100
            //var pdf = results.ParameterResults[0].KernelDensity;
            //for (int j = 0; j < pdf.GetLength(0); j++)
            //    Debug.Print(pdf[j, 0] + ", " + pdf[j, 1]);

            ////This code block prints the estimated parameter results for the standard deviation ~15 
            //var pdf = results.ParameterResults[1].KernelDensity;
            //for (int j = 0; j < pdf.GetLength(0); j++)
            //    Debug.Print(pdf[j, 0] + ", " + pdf[j, 1]);

            // Write out results
            stopWatch.Stop();
            var timeSpan = stopWatch.Elapsed;
            string elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10d);
            Debug.WriteLine("Runtime: " + elapsedTime);

        }
    }
}
