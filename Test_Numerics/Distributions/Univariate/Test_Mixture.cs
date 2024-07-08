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

            var mix = new Mixture(new[] { 0.15, 0.50, 0.35}, new[] { new Normal(0, 1), new Normal(3, 0.1), new Normal(-3, 0.5) });
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
