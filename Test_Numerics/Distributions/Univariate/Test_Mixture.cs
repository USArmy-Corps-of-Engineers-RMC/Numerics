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
            var sample = mix.GenerateRandomValues(12345, 200);
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

            //priors.Add(new BetaDistribution(0.5, 0.5));
            //priors.Add(new BetaDistribution(0.5, 0.5));
            //priors.Add(new BetaDistribution(0.5, 0.5));

            for (int i = 0; i < Lowers.Length; i++)
            {
                priors.Add(new Uniform(Lowers[i], Uppers[i]));
            }

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            double logLH(ref IList<double> parameters)
            {
                var dist = (Mixture)mix.Clone();

                // Set distribution parameters
                int k = dist.Distributions.Count;
                var weights = new double[k];
                int t = 0;
                for (int i = 0; i < k; i++)
                {
                    weights[i] = parameters[i];
                    t += 1;
                }
                double sum = Tools.Sum(weights);
                for (int i = 0; i < k; i++)
                {
                    weights[i] /= sum;
                    parameters[i] /= sum;
                }
                


                for (int i = 0; i < k; i++)
                {
                    var parms = new List<double>();
                    for (int j = t; j < t + dist.Distributions[i].NumberOfParameters; j++)
                    {
                        parms.Add(parameters[j]);
                    }
                    dist.Distributions[i].SetParameters(parms);
                    t += dist.Distributions[i].NumberOfParameters;
                }

                double lh = 0;
                for (int i = 0; i < sample.Length; i++)
                {
                    var lnf = new List<double>();
                    for (int j = 0; j < k; j++)
                    {
                        lnf.Add(Math.Log(weights[j]) + dist.Distributions[j].LogPDF(sample[i]));
                    }
                    lh += Tools.LogSumExp(lnf);
                }

                if (double.IsNaN(lh) || double.IsInfinity(lh)) return double.MinValue;
                return lh;
            }

            var sampler = new DEMCzs(priors, (IList<double> x) => logLH(ref x));

            sampler.Sample();
            var results = new MCMCResults(sampler);

            //var pdf = results.ParameterResults[0].KernelDensity;
            //for (int j = 0; j < pdf.GetLength(0); j++)
            //    Debug.Print(pdf[j, 0] + ", " + pdf[j, 1]);

            //pdf = results.ParameterResults[1].KernelDensity;
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
