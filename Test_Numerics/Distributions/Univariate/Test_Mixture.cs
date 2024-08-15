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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics;
using Numerics.Data.Statistics;
using Numerics.Distributions;
using Numerics.Sampling.MCMC;

namespace Distributions.Univariate
{
    [TestClass]
    public class Test_Mixture
    {
        [TestMethod]
        public void Test_2D_Mixture()
        {
            var mix = new Mixture(new[] { 0.7, 0.3 }, new[] { new Normal(0, 1), new Normal(3, 0.1) });
            var sample = mix.GenerateRandomValues(12345, 1000);
            mix.Estimate(sample, ParameterEstimationMethod.MaximumLikelihood);

        }

        [TestMethod]
        public void Test_DEMCz_2D_Mixture()
        {

            var mix = new Mixture(new[] { 0.7, 0.3 }, new[] { new Normal(0, 1), new Normal(3, 0.1)});
            var sample = mix.GenerateRandomValues(12345, 1000);

            var tuple = mix.GetParameterConstraints(sample);
            var Lowers = tuple.Item2;
            var Uppers = tuple.Item3;
            var priors = new List<IUnivariateDistribution>();
            for (int i = 0; i < Lowers.Length; i++)
            {
                priors.Add(new Uniform(Lowers[i], Uppers[i]));
            }

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            double logLH(double[] parameters)
            {
                var dist = (Mixture)mix.Clone();
                dist.SetParameters(parameters);
                double lh = dist.LogLikelihood(sample);
                if (double.IsNaN(lh) || double.IsInfinity(lh)) return double.MinValue;
                return lh;
            }

            var sampler = new DEMCzs(priors, logLH);

            sampler.Sample();
            var results = new MCMCResults(sampler);

            // Write out results
            stopWatch.Stop();
            var timeSpan = stopWatch.Elapsed;
            string elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10d);
            Debug.WriteLine("Runtime: " + elapsedTime);

        }

    }
}
