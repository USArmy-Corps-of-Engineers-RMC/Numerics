using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Data.Statistics;
using Numerics.Distributions;
using Numerics.Sampling.MCMC;

namespace Sampling.MCMC
{
    [TestClass]
    public class Test_Gibbs
    {

        /// <summary>
        /// Demonstrates how to use the Gibbs sampler.
        /// </summary>
        [TestMethod]
        public void Test_Gibbs_NormalDist()
        {
            double popMU = 100, popSigma = 15;
            int n = 200;
            var normDist = new Normal(popMU, popSigma);
            var sample = normDist.GenerateRandomValues(12345, n);
            var mu = Statistics.Mean(sample);
            var sigma = Statistics.StandardDeviation(sample);
            var priors = new List<IUnivariateDistribution> { new Uniform(0, 1000), new Uniform(0, 1000) };
            var invG = new InverseGamma();

            var sampler = new Gibbs(priors, x =>
            {
                var norm = new Normal(x[0], x[1]);
                return norm.LogLikelihood(sample);
            }, (x, prng) =>
            {
                double mu0 = 100, sigma0 = 15 * 15; // priors for mu and sigma
                int n0 = 2; // prior for effective sample size for sigma
                double alpha = 1; // prior for inverse-gamma shape
                double beta = n0 + sigma0 / 2d; // prior for inverse-gamma scale

                // Estimate the conditional proposal for sigma
                double mu1 = x[0];
                double alpha1 = alpha + n / 2d;
                double sse = 0;
                for (int i = 0; i < sample.Length; i++)
                    sse += Math.Pow(sample[i] - mu1, 2);
                double beta1 = beta + sse / 2d;
                invG.SetParameters(new double[] {beta1, alpha1});
                double sig2p = invG.InverseCDF(prng.NextDouble());

                // Estimate the condition proposal for mu
                double sig2 = 1d / (n / sig2p + 1d / sigma0);
                mu1 = sig2 * (n * mu / sig2p + mu0 / sigma0);
                normDist.SetParameters(mu1, Math.Sqrt(sig2));
                double mup = normDist.InverseCDF(prng.NextDouble());
                return new double[] { mup, Math.Sqrt(sig2p) };
            });

            var stopWatch = new Stopwatch();
            stopWatch.Start();

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
