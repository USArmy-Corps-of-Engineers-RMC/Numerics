using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Data.Statistics;
using Numerics.Distributions;
using Numerics.Mathematics.LinearAlgebra;
using Numerics.Mathematics;
using Numerics.Sampling.MCMC;

namespace Sampling.MCMC
{
    [TestClass]
    public class Test_SNIS
    {
        [TestMethod]
        public void Test_SNIS_NormalDist()
        {
            double popMU = 100, popSigma = 15;
            int n = 200;
            var normDist = new Normal(popMU, popSigma);
            var sample = normDist.GenerateRandomValues(12345, n);

            var mu = Statistics.Mean(sample);
            var sigma = Statistics.StandardDeviation(sample);
            var priors = new List<IUnivariateDistribution> { new Uniform(-1000, 1000), new Uniform(0, 100) };

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var sampler = new SNIS(priors, x =>
            {
                var norm = new Normal(x[0], x[1]);
                return norm.LogLikelihood(sample);
            });

            sampler.Iterations = 1000000;
            sampler.OutputLength = 10000;
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

        [TestMethod]
        public void Test_SNIS_NormalDist_Importance()
        {
            double popMU = 100, popSigma = 15;
            int n = 200;
            var normDist = new Normal(popMU, popSigma);
            var sample = normDist.GenerateRandomValues(12345, n);

            var mu = Statistics.Mean(sample);
            var sigma = Statistics.StandardDeviation(sample);
            var priors = new List<IUnivariateDistribution> { new Uniform(-10000, 10000), new Uniform(0, 1000) };

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var sampler = new SNIS(priors, x =>
            {
                var norm = new Normal(x[0], x[1]);
                return norm.LogLikelihood(sample);
            });

            sampler.InitializeWithMAP = true;
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
