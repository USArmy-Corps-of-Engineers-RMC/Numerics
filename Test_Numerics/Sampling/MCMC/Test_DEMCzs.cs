using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics;
using Numerics.Data.Statistics;
using Numerics.Distributions;
using Numerics.Mathematics;
using Numerics.Mathematics.Integration;
using Numerics.Sampling;
using Numerics.Sampling.MCMC;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Sampling.MCMC
{
    [TestClass]
    public class Test_DEMCzs
    {
        /// <summary>
        /// Demonstrates how to use the Differential Evolution Monte Carlo with Snooker Update (DEMCzs) sampler.
        /// </summary>
        [TestMethod]
        public void Test_DEMCzs_NormalDist()
        {
            double popMU = 100, popSigma = 15;
            int n = 200;
            var normDist = new Normal(popMU, popSigma);
            var sample = normDist.GenerateRandomValues(12345, n);
            var mu = Statistics.Mean(sample);
            var sigma = Statistics.StandardDeviation(sample);
            var priors = new List<IUnivariateDistribution> { new Uniform(-1000, 1000), new Uniform(0, 100) };

            var sampler = new DEMCzs(priors, x =>
            {
                var norm = new Normal(x[0], x[1]);
                return norm.LogLikelihood(sample);
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


        /// <summary>
        /// Demonstrates how to use the Differential Evolution Monte Carlo with Snooker Update (DEMCzs) sampler.
        /// </summary>
        [TestMethod]
        public void Test_DEMCzs_NormalDist_ME()
        {
            int n = 100;
            double popMU = 100, popSigma = 15, epsSigma2 = Math.Log(1 + 0.3 * 0.3);
            double epsMu = -0.5 * epsSigma2;
            double epsSigma = Math.Sqrt(epsSigma2);

            var normDist = new Normal(popMU, popSigma);
            var epsDist = new Normal(epsMu, epsSigma);
            var sample = normDist.GenerateRandomValues(12345, n);
            var prng = new MersenneTwister(12345);
            for (int i = 0; i < sample.Length; i++)
            {
                //sample[i] *= epsDist.InverseCDF(prng.NextDouble());
                sample[i] = Math.Exp(Math.Log(sample[i]) + epsDist.InverseCDF(prng.NextDouble()));
                //Debug.Print(sample[i].ToString());
            }


            var mu = Statistics.Mean(sample);
            var sigma = Statistics.StandardDeviation(sample);
            var priors = new List<IUnivariateDistribution> { new Uniform(-1000, 1000), new Uniform(0, 100) };

            var sampler = new DEMCzs(priors, x =>
            {
                var norm = new Normal(x[0], x[1]);
                double LH = 0;
                for (int i = 0; i < sample.Length; i++)
                {
                    //if (sample[i] >= 100)
                    //{
                    //double a = Math.Exp(Math.Log(sample[i]) + epsDist.InverseCDF(1E-8));
                    //double b = Math.Exp(Math.Log(sample[i]) + epsDist.InverseCDF(1 - 1E-8));
                    double a = norm.InverseCDF(1E-8);
                    double b = norm.InverseCDF(1 - 1E-8);
                    var ex = Integration.SimpsonsRule((q) => { return q * epsDist.PDF(Math.Log(sample[i]) - Math.Log(q)) * norm.PDF(q); }, a, b, 10);
                    LH += Math.Log(ex);
                    //}
                    //else
                    //{
                    //LH += norm.LogPDF(sample[i]);
                    //}
                }
                return LH;
            });

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            sampler.Sample();
            var results = new MCMCResults(sampler);

            var probabilities = new double[] { 0.999999, 0.999998, 0.999995, 0.99999, 0.99998, 0.99995, 0.9999, 0.9998, 0.9995, 0.999, 0.998, 0.995, 0.99, 0.98, 0.95, 0.9, 0.8, 0.7, 0.5, 0.3, 0.2, 0.1, 0.05, 0.02, 0.01 };
            //var norm = new Normal(2.46134734774504, 0.541117546745144);
            //var boot = new BootstrapAnalysis(norm, ParameterEstimationMethod.MethodOfMoments, 30);
            //var CIs = boot.PercentileQuantileCI(probabilities);
            //for (int i = 0; i < probabilities.Length; i++)
            //    Debug.Print(CIs[i, 0] + ", " + CIs[i, 1]);


            var boot = new BootstrapAnalysis(normDist, ParameterEstimationMethod.MaximumLikelihood, n);
            var dists = new IUnivariateDistribution[sampler.OutputLength];
            for (int i = 0; i < sampler.OutputLength; i++)
            {
                dists[i] = new Normal(results.Output[i].Values[0], results.Output[i].Values[1]);
            }
            var CIs = boot.PercentileQuantileCI(probabilities, 0.1, dists);
            for (int i = 0; i < probabilities.Length; i++)
                Debug.Print(probabilities[i] + "," + CIs[i, 0] + ", " + CIs[i, 1]);

            var pdf = results.ParameterResults[0].KernelDensity;
            for (int j = 0; j < pdf.GetLength(0); j++)
                Debug.Print(pdf[j, 0] + ", " + pdf[j, 1]);

            pdf = results.ParameterResults[1].KernelDensity;
            for (int j = 0; j < pdf.GetLength(0); j++)
                Debug.Print(pdf[j, 0] + ", " + pdf[j, 1]);

            // Write out results
            stopWatch.Stop();
            var timeSpan = stopWatch.Elapsed;
            string elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10d);
            Debug.WriteLine("Runtime: " + elapsedTime);

        }


        /// <summary>
        /// Demonstrates how to use the Differential Evolution Monte Carlo with Snooker Update (DEMCzs) sampler.
        /// </summary>
        [TestMethod]
        public void Test_DEMCzs_LogNormalDist_ME()
        {
            int n = 100;

            double realMu = 500;
            double realSigma = 50;

            double variance = Math.Pow(realSigma, 2d);
            double popMU = Math.Log(Math.Pow(realMu, 2d) / Math.Sqrt(variance + Math.Pow(realMu, 2d)));
            double popSigma = Math.Sqrt(Math.Log(1.0d + variance / Math.Pow(realMu, 2d)));

            double errSigma2 = Math.Log(1 + 0.3 * 0.3);
            double errSigma = Math.Sqrt(errSigma2);
            double errMu = -0.5 * errSigma2;
           
            var popDist = new Normal(popMU, popSigma);
            var errDist = new Normal(errMu, errSigma);

            var sample = popDist.GenerateRandomValues(12345, n);
            var prng = new MersenneTwister(12345);
            for (int i = 0; i < sample.Length; i++)
            {
                sample[i] = sample[i] + errDist.InverseCDF(prng.NextDouble());
                //Debug.Print(Math.Exp(sample[i]).ToString());
            }

            //var a = Statistics.Minimum(sample) + errDist.InverseCDF(1E-8);
            //var b = Statistics.Maximum(sample) + errDist.InverseCDF(1-1E-8); 

            var mu = Statistics.Mean(sample);
            var sigma = Statistics.StandardDeviation(sample);
            var priors = new List<IUnivariateDistribution> { new Uniform(0, 10), new Uniform(0, 10) };

            var sampler = new DEMCzs(priors, x =>
            {
                var norm = new Normal(x[0], x[1]);
                double LH = 0;
                for (int i = 0; i < sample.Length; i++)
                {
              
                    var f = 1 / Math.Sqrt(2d * Math.PI * (x[1] * x[1] + errSigma2)) * Math.Exp(-1d / 2d * Math.Pow(sample[i] - x[0] - errMu, 2d) / (x[1] * x[1] + errSigma2));
                    LH += Math.Log(f);

                    //double a = sample[i] + errDist.InverseCDF(1E-8);
                    //double b = sample[i] + errDist.InverseCDF(1 - 1E-8);
                    //var ex = Integration.GaussLegendre((q) => { return errDist.PDF(sample[i] - q) * norm.PDF(q); }, a, b);
                    //LH += Math.Log(ex);

                    //LH += norm.LogPDF(sample[i]);

                }
                return LH;
            });

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            sampler.Sample();
            var results = new MCMCResults(sampler);

            var probabilities = new double[] { 0.999999, 0.999998, 0.999995, 0.99999, 0.99998, 0.99995, 0.9999, 0.9998, 0.9995, 0.999, 0.998, 0.995, 0.99, 0.98, 0.95, 0.9, 0.8, 0.7, 0.5, 0.3, 0.2, 0.1, 0.05, 0.02, 0.01 };
            var boot = new BootstrapAnalysis(popDist, ParameterEstimationMethod.MaximumLikelihood, n);
            var dists = new IUnivariateDistribution[sampler.OutputLength];
            for (int i = 0; i < sampler.OutputLength; i++)
            {
                dists[i] = new Normal(results.Output[i].Values[0], results.Output[i].Values[1]);
            }
            var CIs = boot.PercentileQuantileCI(probabilities, 0.1, dists);
            for (int i = 0; i < probabilities.Length; i++)
                Debug.Print(probabilities[i] + "," + Math.Exp(CIs[i, 0]) + ", " + Math.Exp(CIs[i, 1]));



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


        /// <summary>
        /// Demonstrates how to use the Differential Evolution Monte Carlo with Snooker Update (DEMCzs) sampler.
        /// </summary>
        [TestMethod]
        public void Test_DEMCzs_LogNormalDist_ME_2()
        {
            int n = 100;

            double realMu = 500;
            double realSigma = 50;

            double variance = Math.Pow(realSigma, 2d);
            double popMU = Math.Log(Math.Pow(realMu, 2d) / Math.Sqrt(variance + Math.Pow(realMu, 2d)));
            double popSigma = Math.Sqrt(Math.Log(1.0d + variance / Math.Pow(realMu, 2d)));

            double errSigma2 = Math.Log(1 + 0.3 * 0.3);
            double errSigma = Math.Sqrt(errSigma2);
            double errMu = -0.5 * errSigma2;

            var popDist = new Normal(popMU, popSigma);
            var errDist = new Normal(errMu, errSigma);

            var sample = popDist.GenerateRandomValues(12345, n);
            var prng = new MersenneTwister(12345);
            for (int i = 0; i < sample.Length; i++)
            {
                if (Math.Exp(sample[i]) >= 500)
                {
                    sample[i] = sample[i] + errDist.InverseCDF(prng.NextDouble());
                }             
                //Debug.Print(Math.Exp(sample[i]).ToString());
            }


            var mu = Statistics.Mean(sample);
            var sigma = Statistics.StandardDeviation(sample);
            var priors = new List<IUnivariateDistribution> { new Uniform(0, 20), new Uniform(0, 10) };

            var sampler = new DEMCzs(priors, x =>
            {
                var norm = new Normal(x[0], x[1]);
                double w1 = norm.CDF(Math.Log(500));
                double w2 = 1 - w1;

                double LH = 0;
                for (int i = 0; i < sample.Length; i++)
                {
                    double p1, p2;
                    p1 = norm.LogPDF(sample[i]);

                    var f = 1 / Math.Sqrt(2d * Math.PI * (x[1] * x[1] + errSigma2)) * Math.Exp(-1d / 2d * Math.Pow(sample[i] - x[0] - errMu, 2d) / (x[1] * x[1] + errSigma2));
                    p2 = Math.Log(f);

                    //double a = sample[i] + errDist.InverseCDF(1E-8);
                    //double b = sample[i] + errDist.InverseCDF(1 - 1E-8);
                    //var ex = Integration.SimpsonsRule((q) => { return errDist.PDF(sample[i] - q) * norm.PDF(q); }, a, b, 10);
                    //p2 = Math.Log(ex);

                    LH += Tools.LogSumExp(Math.Log(w1) + p1, Math.Log(w2) + p2);
                }
                return LH;
            });

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            sampler.Sample();
            var results = new MCMCResults(sampler);

            var probabilities = new double[] { 0.999999, 0.999998, 0.999995, 0.99999, 0.99998, 0.99995, 0.9999, 0.9998, 0.9995, 0.999, 0.998, 0.995, 0.99, 0.98, 0.95, 0.9, 0.8, 0.7, 0.5, 0.3, 0.2, 0.1, 0.05, 0.02, 0.01 };
            var boot = new BootstrapAnalysis(popDist, ParameterEstimationMethod.MaximumLikelihood, n);
            var dists = new IUnivariateDistribution[sampler.OutputLength];
            for (int i = 0; i < sampler.OutputLength; i++)
            {
                dists[i] = new Normal(results.Output[i].Values[0], results.Output[i].Values[1]);
            }
            var CIs = boot.PercentileQuantileCI(probabilities, 0.1, dists);
            for (int i = 0; i < probabilities.Length; i++)
                Debug.Print(probabilities[i] + "," + Math.Exp(CIs[i, 0]) + ", " + Math.Exp(CIs[i, 1]));



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

        /// <summary>
        /// Demonstrates how to use the Differential Evolution Monte Carlo with Snooker Update (DEMCzs) sampler.
        /// </summary>
        [TestMethod]
        public void Test_DEMCzs_LogNormalDist_ME_3()
        {
            int n = 100;

            double realMu = 500;
            double realSigma = 50;

            double variance = Math.Pow(realSigma, 2d);
            double popMU = Math.Log(Math.Pow(realMu, 2d) / Math.Sqrt(variance + Math.Pow(realMu, 2d)));
            double popSigma = Math.Sqrt(Math.Log(1.0d + variance / Math.Pow(realMu, 2d)));

            double errSigma2 = Math.Log(1 + 0.3 * 0.3);
            double errSigma = Math.Sqrt(errSigma2);
            double errMu = -0.5 * errSigma2;

            var popDist = new Normal(popMU, popSigma);
            //var errDist = new Normal(errMu, errSigma);
            var errDist = new Uniform(Math.Log(0.7), Math.Log(1.3));

            var sample = popDist.GenerateRandomValues(12345, n);
            var u = new double[n];
            var l = new double[n];
            var prng = new MersenneTwister(12345);
            for (int i = 0; i < sample.Length; i++)
            {
                //u[i] = sample[i] + errDist.Maximum;
                //l[i] = sample[i] + errDist.Minimum;
                sample[i] = sample[i] + errDist.InverseCDF(prng.NextDouble());
                u[i] = sample[i] + errDist.Maximum;
                l[i] = sample[i] + errDist.Minimum;
                //Debug.Print(Math.Exp(sample[i]).ToString());
                //Debug.Print(Math.Exp(u[i]).ToString());
            }


            var mu = Statistics.Mean(sample);
            var sigma = Statistics.StandardDeviation(sample);
            var priors = new List<IUnivariateDistribution> { new Uniform(0, 10), new Uniform(0, 10) };

            var sampler = new DEMCzs(priors, x =>
            {
                var norm = new Normal(x[0], x[1]);
                double LH = 0;
                for (int i = 0; i < sample.Length; i++)
                {
                   double a = sample[i] + errDist.InverseCDF(1E-8);
                    double b = sample[i] + errDist.InverseCDF(1 - 1E-8);
                    var ex = Integration.GaussLegendre((q) => { return errDist.PDF(sample[i] - q) * norm.PDF(q); }, a, b);
                    //var ex = Integration.SimpsonsRule((q) => { return errDist.PDF(sample[i] - q) * norm.PDF(q); }, a, b, 10);
                    LH += Math.Log(ex);

                    //LH += norm.LogPDF(sample[i]);
                    //LH += norm.LogLikelihood_Intervals(l[i], u[i]);

                }
                return LH;
            });

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            sampler.Sample();
            var results = new MCMCResults(sampler);

            var probabilities = new double[] { 0.999999, 0.999998, 0.999995, 0.99999, 0.99998, 0.99995, 0.9999, 0.9998, 0.9995, 0.999, 0.998, 0.995, 0.99, 0.98, 0.95, 0.9, 0.8, 0.7, 0.5, 0.3, 0.2, 0.1, 0.05, 0.02, 0.01 };
            var boot = new BootstrapAnalysis(popDist, ParameterEstimationMethod.MaximumLikelihood, n);
            var dists = new IUnivariateDistribution[sampler.OutputLength];
            for (int i = 0; i < sampler.OutputLength; i++)
            {
                dists[i] = new Normal(results.Output[i].Values[0], results.Output[i].Values[1]);
            }
            var CIs = boot.PercentileQuantileCI(probabilities, 0.1, dists);
            for (int i = 0; i < probabilities.Length; i++)
                Debug.Print(probabilities[i] + "," + Math.Exp(CIs[i, 0]) + ", " + Math.Exp(CIs[i, 1]));

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
