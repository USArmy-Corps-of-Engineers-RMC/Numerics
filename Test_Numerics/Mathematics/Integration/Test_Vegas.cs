using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Data.Statistics;
using Numerics.Distributions;
using Numerics.Mathematics.Integration;
using Numerics.Sampling;
using Numerics.Sampling.MCMC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Mathematics.Integration
{
    [TestClass]
    public class Test_Vegas
    {
        [TestMethod()]
        public void Test_PI()
        {
            var vegas = new Vegas((x, y) => { return Integrands.PI(x); }, 2, new double[] { -1, -1 }, new double[] { 1, 1 });
            vegas.Integrate();
            var result = vegas.Result;
            double trueResult = 3.1416;
            Assert.AreEqual(trueResult, result, 1E-3 * trueResult);
        }

        [TestMethod()]
        public void Test_GSL()
        {
            var vegas = new Vegas((x, y) => { return Integrands.GSL(x); }, 3, new double[] { 0, 0, 0 }, new double[] { Math.PI, Math.PI, Math.PI });
            vegas.Integrate();
            var result = vegas.Result;
            double trueResult = 1.3932039296856768591842462603255;
            Assert.AreEqual(trueResult, result, 1E-2 * trueResult);
        }

        [TestMethod()]
        public void Test_SumOfThreeNormals()
        {
            var min = new double[3];
            var max = new double[3];
            for (int i = 0; i < 3; i++)
            {
                min[i] = 1E-16;
                max[i] = 1 - 1E-16;
            }

            var vegas = new Vegas((x, y) => { return Integrands.SumOfNormals(x); }, 3, min, max);
            vegas.Integrate();
            var result = vegas.Result;
            double trueResult = 57;
            Assert.AreEqual(trueResult, result, 1E-3 * trueResult);
        }


        /// <summary>
        /// Demonstrates how to use the Differential Evolution Monte Carlo (DEMCz) sampler.
        /// </summary>
        [TestMethod]
        public void Test_DEMCz_NormalDist()
        {

            //double popMU = 100, popSigma = 15;
            //int n = 200;
            //var normDist = new Normal(popMU, popSigma);
            //var sample = normDist.GenerateRandomValues(12345, n);
            //var mu = Statistics.Mean(sample);
            //var sigma = Statistics.StandardDeviation(sample);
            var priors = new List<IUnivariateDistribution> { new Uniform(-1, 1), new Uniform(-1, 1) };


            double dx = 1;
            for (int i = 0; i < 2; i++)
            {
                dx *= 1 - -1;
            }

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var sampler = new DEMCz(priors, x =>
            {
                return Integrands.PI(x.ToArray());
            });

            ((DEMCz)sampler).Iterations = 1000000;
            sampler.Sample();
            var results = new MCMCResults(sampler);

            double sum = 0;
            for (int i = 0; i < sampler.OutputLength; i++)
            {
                sum += Integrands.PI(results.Output[i].Values) * dx;
            }
            sum /= sampler.OutputLength;

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


        [TestMethod()]
        public void Test_SumOfThreeNormals2()
        {
            double dx = 1;
            var min = new double[5];
            var max = new double[5];
            for (int i = 0; i < 5; i++)
            {
                var norm = new Normal(Integrands.mu20[i], Integrands.sigma20[i]);
                min[i] = norm.InverseCDF(1E-8);
                max[i] = norm.InverseCDF(1 - 1E-8);
                //min[i] = Normal.StandardZ(1E-16);
                //max[i] = Normal.StandardZ(1 - 1E-16);
                dx *= max[i] - min[i];
            }
            Integrands.Volume = dx;

            var vegas = new Vegas(Integrands.SumOfNormals2, 5, min, max);
            vegas.CheckConvergence = false;
            vegas.IndependentEvaluations = 1;
            vegas.Integrate();
            var result = vegas.Result;
            var p = Integrands.TotalP;

            double trueResult = 57;
            Assert.AreEqual(trueResult, result, 1E-3 * trueResult);
        }


        [TestMethod()]
        public void Test_SumOfFiveNormals()
        {
            var min = new double[5];
            var max = new double[5];
            for (int i = 0; i < 5; i++)
            {
                min[i] = 1E-16;
                max[i] = 1 - 1E-16;
            }

            var vegas = new Vegas((x, y) => { return Integrands.SumOfNormals(x); }, 5, min, max);
            vegas.Integrate();
            var result = vegas.Result;
            double trueResult = 224;
            Assert.AreEqual(trueResult, result, 1E-3 * trueResult);
        }

        [TestMethod()]
        public void Test_SumOfTwentyNormals()
        {
            var min = new double[20];
            var max = new double[20];
            for (int i = 0; i < 20; i++)
            {
                min[i] = 1E-16;
                max[i] = 1 - 1E-16;
            }

            var vegas = new Vegas((x, y) => { return Integrands.SumOfNormals(x); }, 20, min, max);
            vegas.Integrate();
            var result = vegas.Result;
            double trueResult = 837;
            Assert.AreEqual(trueResult, result, 1E-3 * trueResult);
        }

    }
}
