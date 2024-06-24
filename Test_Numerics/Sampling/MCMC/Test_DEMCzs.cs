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
        public void Test_DEMCzs_Coles_EQ6()
        {

            var data = new double[] { 0.75, 0.64, 0.44, 0.53, 1.16, 0.49, 0.97, 0.43, 1.03, 0.91, 1.02, 0.88, 0.77, 2.19, 0.6, 0.58, 1.85, 3.25, 0.53, 0.96, 0.41, 0.83, 1.32, 0.56, 1.27, 0.95, 1.02, 0.95, 0.9, 0.52, 0.8, 0.59, 0.43, 0.51, 0.83, 0.55, 0.74, 0.55, 1.44, 0.94, 0.97, 0.95, 0.53, 1.2, 1.91, 5.96, 0.46, 0.87, 0.8, 1.07, 1.53, 0.44, 0.82, 1.94, 1.1, 0.69, 0.59, 0.83, 0.65, 2.8, 1.2, 1, 0.87, 0.8, 0.43, 0.96, 1, 1.5, 0.95, 1.72, 0.54, 0.61, 2.01, 0.74, 1.78, 1.1, 1.55, 0.53, 0.5, 1.2, 0.73, 0.53, 0.83, 0.95, 0.6, 0.68, 4.33, 2.02, 0.55, 6.54, 0.77, 0.88, 1.22, 1.7, 1.18, 0.57, 1.11, 0.59, 1.12, 0.9, 0.64, 0.77, 2.14, 0.52, 0.67, 2.21, 0.45, 1.4, 0.73, 1.81, 1.09, 1.79, 3.78, 1.7, 0.63, 0.42, 0.56, 1.05, 0.45, 2.99, 0.62, 2.31, 4.1, 1.2, 1.46, 1.7, 2.05, 1, 1.97, 2.86, 9.43, 0.69, 0.51, 0.68, 1.22, 2.31, 0.71, 0.45, 1.52, 1.32, 1.32, 1.09, 1.06, 0.93, 0.54, 0.71, 0.5, 1, 0.99, 0.45, 0.41, 0.5, 1.56, 0.42, 1.62, 0.46, 1.76, 1.06, 2.7, 0.92, 0.48, 0.41, 0.53, 0.86, 3.21, 0.43, 0.56, 2.4, 1.38, 0.57, 0.55, 3.23, 1.04, 2.3, 0.62, 0.96, 0.45, 0.77, 0.63, 0.56, 1.54, 0.94, 0.42, 0.77, 1.71, 0.48, 1.38, 0.77, 1.97, 0.56, 0.54, 1, 0.6, 0.74, 3.85, 2.56, 1.23, 3.04, 4.4, 0.55, 2.46, 0.7, 0.6, 2.9, 1.03, 1.73, 0.41, 0.6, 1.59, 4.81, 1.46, 0.97, 1.99, 0.49, 1.66, 3.12, 1.75, 0.64, 1.16, 1.62, 1.57, 2.12, 0.62, 1.19, 0.84, 0.55, 0.69, 0.55, 0.85, 1.51, 1.8, 0.46, 1.52, 0.42, 0.48, 0.88, 3, 1.45, 1.95, 0.43, 0.56, 0.65, 0.45, 0.6, 0.88, 0.93, 0.43, 2.14, 0.58, 0.52, 0.6, 2.1, 1.6, 0.81, 0.9, 0.54, 0.52, 0.65, 0.56, 0.59, 0.43, 0.47, 2.43, 0.75, 0.72, 1.26, 1.29, 0.49, 0.88, 2.55, 5, 1.19, 1.18, 0.61, 0.73, 1.07, 0.83, 1.05, 1.86, 1.03, 3.18, 0.45, 0.75, 0.83, 0.56, 4.9, 0.85, 0.92, 1.1, 1.11, 1.75, 3.46, 1.1, 1.88, 0.51, 0.63, 0.44, 1.13, 4.05, 1.02, 2.04, 0.57, 0.67, 0.96, 1.22, 1.74, 1.45, 4.62, 0.61, 2.54, 1.7, 2.87, 1.7, 4.37, 0.43, 0.51, 0.65, 0.5, 1.34, 1.1, 1.06, 1.87, 0.94, 0.59, 0.55, 1.08, 0.55, 0.48, 3.43, 0.48, 0.72, 0.75, 1, 0.5, 0.87, 0.58, 2.25, 0.87, 1.18, 1.38, 2.04, 0.8, 1.32, 0.95, 0.47, 0.65, 0.46, 1.68, 2.95, 1.4, 0.88, 1.56, 0.5, 0.67, 0.91, 2.05, 1.9, 1.03, 5.07, 1.18, 2.58, 1.68, 3.77, 2.6, 1.7, 0.51, 2.01, 0.76, 0.79, 0.92, 2.25, 0.49, 0.73, 0.88, 1.1, 0.57, 1.09, 1.42, 0.76, 2.28, 9.4, 0.51, 0.72, 2.06, 1.29, 2.69, 0.52, 1.67, 0.47, 0.69, 0.63, 0.99, 1.42, 3.1, 0.5, 0.64, 0.94, 0.7, 0.77, 2.13, 0.48, 4.73, 0.7, 1.02, 1.22, 0.85, 1, 1.63, 0.81, 0.71, 2.16, 0.92, 2.26, 1.35, 1.11, 0.59, 0.58, 2.88, 0.41, 0.42, 0.8, 0.42, 1.75, 1.23, 0.88, 1.14, 1.1, 2, 0.7, 1.16, 3.27, 0.87, 0.58, 1.56, 0.41, 0.62, 1.41, 1.78, 0.73, 0.91, 2.95, 2.3, 0.97, 0.45, 0.8, 2.1, 3.78, 0.54, 0.66, 0.52, 1.86, 0.64, 0.41, 3.21, 0.44, 2.61, 1.3, 0.5, 0.48, 0.41, 0.67, 0.41, 0.87, 2.54, 0.46, 1.03, 0.93, 0.66, 0.81, 1.06, 0.45, 0.6, 0.9, 0.98, 1.44, 0.43, 0.63, 1.3, 0.6, 1.71, 0.58, 0.77, 0.62, 0.91, 3.39, 0.48, 3.51, 8.25, 0.5, 2.5, 4.1, 1.34, 0.75, 0.45, 2.01, 1.9, 0.8, 0.52, 2.5, 2.3, 1.59, 0.76, 0.65, 2.35, 2.1, 0.97, 0.6, 0.87, 0.92, 1.56, 1.37, 2.7, 0.85, 3.65, 1.8, 0.52, 0.6, 1.33, 2.88, 0.83, 2.78, 0.6, 0.45, 1.54, 0.8, 1.42, 1.46, 3.26, 1.22, 0.65, 0.61, 0.72, 0.56, 1.07, 0.8, 0.42, 0.73, 0.52, 1.03, 1.42, 4.7, 0.47, 1.83, 0.56, 0.68, 0.61, 6.43, 1.22, 0.63, 0.85, 0.65, 0.53, 0.44, 1.3, 2.6, 0.42, 1.12, 0.75, 1.25, 0.93, 0.62, 3.48, 0.51, 0.5, 2.3, 0.45, 3.45, 0.55, 0.68, 2.5, 1.09, 2.03, 0.57, 0.65, 1.21, 2, 2.5, 0.94, 0.91, 1.96, 1.5, 2.14, 0.98, 0.85, 0.89, 0.6, 0.41, 0.65, 0.63, 0.41, 0.43, 0.44, 0.48, 0.43, 0.6, 0.56, 1.56, 2.84, 2.44, 0.7, 0.88, 1.27, 0.44, 0.56, 1.14, 0.84, 1.28, 1.07, 1.36, 0.76, 1.31, 1.42, 0.51, 1.25, 1.62, 0.56, 1.84, 1.58, 0.45, 0.48, 0.54, 0.69, 0.93, 1.17, 0.85, 0.47, 0.85, 0.74, 1.86, 0.44, 0.57, 1.11, 0.8, 0.51, 0.56, 0.87, 0.88, 0.41, 0.42, 3.06, 0.82, 0.55, 0.66, 1.08, 0.5, 0.53, 0.64, 0.52, 0.66, 0.81, 1.17, 1.57, 0.9, 0.8, 0.71, 0.79, 0.64, 0.56, 1.16, 0.42, 0.41, 0.56, 0.94, 0.48, 1.65, 0.62, 2.12, 1.56, 1.68, 1.32, 1.24, 1.8, 0.85, 0.85, 2.2, 1.5, 1, 1.42, 0.74, 0.45, 0.51, 0.58, 0.74, 2.14, 0.46, 0.65, 0.55, 1.04, 1.29, 0.5, 2.46, 3.35, 0.68, 1.15, 1.11, 0.48, 2.57, 1.08, 0.88, 1.15, 1.25, 0.71, 0.64, 0.8, 0.67, 0.56, 0.58, 0.63, 0.47, 2.26, 0.42, 2.28, 0.5, 0.73, 1.88, 1.96, 1.16, 0.56, 0.67, 0.54, 0.98, 0.59, 0.62, 0.85, 0.55, 1.32, 0.6, 1.73, 1.7, 0.92, 1.12, 0.52, 1.49, 0.73, 0.6, 0.64, 0.48, 0.61, 0.6, 0.77, 0.43, 1.27, 0.59, 1.28, 0.6, 0.72, 0.52, 1.91, 0.46, 0.99, 2.18, 0.51, 0.56, 0.58, 0.42, 0.68, 0.84, 0.7, 0.69, 0.56, 0.42, 0.43, 1.25, 0.65, 0.52, 0.52, 0.57, 0.5, 1.76, 0.55, 0.57, 1.31, 0.69, 1.02, 0.41, 0.58, 1.35, 0.72, 1.11, 0.83, 0.58, 0.82, 0.45, 0.6, 0.48, 0.89, 1.08, 0.78, 0.53, 0.86, 0.82, 1, 0.54, 0.7, 1.02, 0.44, 0.75, 0.63, 0.65, 0.5, 0.41, 0.69, 0.43, 0.55, 1.99, 0.5, 0.5, 0.75, 1.41, 0.51, 0.43, 0.71, 0.68, 0.85, 0.5, 0.43, 0.55, 0.44, 0.5, 0.43, 0.52, 0.58, 0.48, 1, 0.75, 0.43, 0.65, 1.45, 0.42, 0.56, 1.02, 0.52, 1.8, 1.18, 0.52, 0.76, 1.04, 1.04, 0.55, 0.55, 0.68, 0.5, 0.5, 0.42, 0.9, 0.63, 0.8, 0.55, 0.42, 0.55, 0.64, 0.91, 0.85, 1.24, 0.74, 0.68, 0.73, 0.59, 1.2, 0.55, 0.69, 0.45, 0.48, 0.48, 1.88, 1.2, 1.12, 0.64, 0.56, 1.03, 0.64, 0.75, 0.53, 0.53, 1.13, 1.43, 1.53, 1.73, 0.48, 0.73, 0.51, 1.2, 2.6, 2.62, 0.5, 1, 0.51, 0.57, 0.62, 0.46, 0.44, 0.82, 0.65, 0.54, 0.84, 0.6, 0.58, 0.9, 1.1, 0.47, 0.48, 0.84, 0.68, 1.59, 0.63, 0.6, 0.83, 0.67, 0.56, 0.41, 0.52, 0.5, 2.34, 2.01, 0.45, 1.27, 1.27, 1.49, 0.44, 0.55, 0.75, 1.3, 0.6, 0.48, 0.5 };
            int N = data.Length; // Total number of data points
            int Ny = 65; // Number of years
            double u = 0.4; // Threshold

            // Prior distributions
            var priors = new List<IUnivariateDistribution>
            {
                new Uniform(-100, 1000),
                new Uniform(0, 100),
                new Uniform(-10, 10),
            };

            // Likelihood function
            double logLikelihood(IList<double> parameters)
            {
                // Get GEV parameters
                double loc = parameters[1];
                double scl = parameters[2];
                double shp = parameters[3];

                // Data likelihood
                double x = 0;
                for (int i = 0; i < data.Length; i++)
                {
                    x += Math.Log(1 + shp * ((data[i] - loc) / scl));
                }
                double logLH = -N * Math.Log(scl) - (1 + 1 / shp) * x - Ny * Math.Pow(1 + shp * ((u - loc) / scl), -1 / shp);

                // Prior likelihood
                for (int i = 0; i < priors.Count; i++)
                {
                    logLH += priors[i].PDF(parameters[i]);
                }

                // Check errors
                if (double.IsNaN(logLH) || double.IsInfinity(logLH)) return double.MinValue;
                return logLH;
            }

            // Set up sampler
            var sampler = new DEMCzs(priors, logLikelihood);

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            // Run sampler
            sampler.Sample();
            var results = new MCMCResults(sampler);


            stopWatch.Stop();
            var timeSpan = stopWatch.Elapsed;
            string elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10d);
            Debug.WriteLine("Runtime: " + elapsedTime);

        }


        /// <summary>
        /// Demonstrates how to use the Differential Evolution Monte Carlo with Snooker Update (DEMCzs) sampler.
        /// </summary>
        [TestMethod]
        public void Test_DEMCzs_Coles_Seasonal_Model()
        {

            var priors = new List<IUnivariateDistribution>
            {
                // Change point parameter K
                new Uniform(1, 366),
                // GEV Season 1
                new Uniform(-100, 1000), 
                new Uniform(0, 100),
                new Uniform(-10, 10),
                // GEV Season 2
                new Uniform(0, 100),
                new Uniform(0, 100),
                new Uniform(-10, 10)
            };

            var data = new double[] { 0.75, 0.64, 0.44, 0.53, 1.16, 0.49, 0.97, 0.43, 1.03, 0.91, 1.02, 0.88, 0.77, 2.19, 0.6, 0.58, 1.85, 3.25, 0.53, 0.96, 0.41, 0.83, 1.32, 0.56, 1.27, 0.95, 1.02, 0.95, 0.9, 0.52, 0.8, 0.59, 0.43, 0.51, 0.83, 0.55, 0.74, 0.55, 1.44, 0.94, 0.97, 0.95, 0.53, 1.2, 1.91, 5.96, 0.46, 0.87, 0.8, 1.07, 1.53, 0.44, 0.82, 1.94, 1.1, 0.69, 0.59, 0.83, 0.65, 2.8, 1.2, 1, 0.87, 0.8, 0.43, 0.96, 1, 1.5, 0.95, 1.72, 0.54, 0.61, 2.01, 0.74, 1.78, 1.1, 1.55, 0.53, 0.5, 1.2, 0.73, 0.53, 0.83, 0.95, 0.6, 0.68, 4.33, 2.02, 0.55, 6.54, 0.77, 0.88, 1.22, 1.7, 1.18, 0.57, 1.11, 0.59, 1.12, 0.9, 0.64, 0.77, 2.14, 0.52, 0.67, 2.21, 0.45, 1.4, 0.73, 1.81, 1.09, 1.79, 3.78, 1.7, 0.63, 0.42, 0.56, 1.05, 0.45, 2.99, 0.62, 2.31, 4.1, 1.2, 1.46, 1.7, 2.05, 1, 1.97, 2.86, 9.43, 0.69, 0.51, 0.68, 1.22, 2.31, 0.71, 0.45, 1.52, 1.32, 1.32, 1.09, 1.06, 0.93, 0.54, 0.71, 0.5, 1, 0.99, 0.45, 0.41, 0.5, 1.56, 0.42, 1.62, 0.46, 1.76, 1.06, 2.7, 0.92, 0.48, 0.41, 0.53, 0.86, 3.21, 0.43, 0.56, 2.4, 1.38, 0.57, 0.55, 3.23, 1.04, 2.3, 0.62, 0.96, 0.45, 0.77, 0.63, 0.56, 1.54, 0.94, 0.42, 0.77, 1.71, 0.48, 1.38, 0.77, 1.97, 0.56, 0.54, 1, 0.6, 0.74, 3.85, 2.56, 1.23, 3.04, 4.4, 0.55, 2.46, 0.7, 0.6, 2.9, 1.03, 1.73, 0.41, 0.6, 1.59, 4.81, 1.46, 0.97, 1.99, 0.49, 1.66, 3.12, 1.75, 0.64, 1.16, 1.62, 1.57, 2.12, 0.62, 1.19, 0.84, 0.55, 0.69, 0.55, 0.85, 1.51, 1.8, 0.46, 1.52, 0.42, 0.48, 0.88, 3, 1.45, 1.95, 0.43, 0.56, 0.65, 0.45, 0.6, 0.88, 0.93, 0.43, 2.14, 0.58, 0.52, 0.6, 2.1, 1.6, 0.81, 0.9, 0.54, 0.52, 0.65, 0.56, 0.59, 0.43, 0.47, 2.43, 0.75, 0.72, 1.26, 1.29, 0.49, 0.88, 2.55, 5, 1.19, 1.18, 0.61, 0.73, 1.07, 0.83, 1.05, 1.86, 1.03, 3.18, 0.45, 0.75, 0.83, 0.56, 4.9, 0.85, 0.92, 1.1, 1.11, 1.75, 3.46, 1.1, 1.88, 0.51, 0.63, 0.44, 1.13, 4.05, 1.02, 2.04, 0.57, 0.67, 0.96, 1.22, 1.74, 1.45, 4.62, 0.61, 2.54, 1.7, 2.87, 1.7, 4.37, 0.43, 0.51, 0.65, 0.5, 1.34, 1.1, 1.06, 1.87, 0.94, 0.59, 0.55, 1.08, 0.55, 0.48, 3.43, 0.48, 0.72, 0.75, 1, 0.5, 0.87, 0.58, 2.25, 0.87, 1.18, 1.38, 2.04, 0.8, 1.32, 0.95, 0.47, 0.65, 0.46, 1.68, 2.95, 1.4, 0.88, 1.56, 0.5, 0.67, 0.91, 2.05, 1.9, 1.03, 5.07, 1.18, 2.58, 1.68, 3.77, 2.6, 1.7, 0.51, 2.01, 0.76, 0.79, 0.92, 2.25, 0.49, 0.73, 0.88, 1.1, 0.57, 1.09, 1.42, 0.76, 2.28, 9.4, 0.51, 0.72, 2.06, 1.29, 2.69, 0.52, 1.67, 0.47, 0.69, 0.63, 0.99, 1.42, 3.1, 0.5, 0.64, 0.94, 0.7, 0.77, 2.13, 0.48, 4.73, 0.7, 1.02, 1.22, 0.85, 1, 1.63, 0.81, 0.71, 2.16, 0.92, 2.26, 1.35, 1.11, 0.59, 0.58, 2.88, 0.41, 0.42, 0.8, 0.42, 1.75, 1.23, 0.88, 1.14, 1.1, 2, 0.7, 1.16, 3.27, 0.87, 0.58, 1.56, 0.41, 0.62, 1.41, 1.78, 0.73, 0.91, 2.95, 2.3, 0.97, 0.45, 0.8, 2.1, 3.78, 0.54, 0.66, 0.52, 1.86, 0.64, 0.41, 3.21, 0.44, 2.61, 1.3, 0.5, 0.48, 0.41, 0.67, 0.41, 0.87, 2.54, 0.46, 1.03, 0.93, 0.66, 0.81, 1.06, 0.45, 0.6, 0.9, 0.98, 1.44, 0.43, 0.63, 1.3, 0.6, 1.71, 0.58, 0.77, 0.62, 0.91, 3.39, 0.48, 3.51, 8.25, 0.5, 2.5, 4.1, 1.34, 0.75, 0.45, 2.01, 1.9, 0.8, 0.52, 2.5, 2.3, 1.59, 0.76, 0.65, 2.35, 2.1, 0.97, 0.6, 0.87, 0.92, 1.56, 1.37, 2.7, 0.85, 3.65, 1.8, 0.52, 0.6, 1.33, 2.88, 0.83, 2.78, 0.6, 0.45, 1.54, 0.8, 1.42, 1.46, 3.26, 1.22, 0.65, 0.61, 0.72, 0.56, 1.07, 0.8, 0.42, 0.73, 0.52, 1.03, 1.42, 4.7, 0.47, 1.83, 0.56, 0.68, 0.61, 6.43, 1.22, 0.63, 0.85, 0.65, 0.53, 0.44, 1.3, 2.6, 0.42, 1.12, 0.75, 1.25, 0.93, 0.62, 3.48, 0.51, 0.5, 2.3, 0.45, 3.45, 0.55, 0.68, 2.5, 1.09, 2.03, 0.57, 0.65, 1.21, 2, 2.5, 0.94, 0.91, 1.96, 1.5, 2.14, 0.98, 0.85, 0.89, 0.6, 0.41, 0.65, 0.63, 0.41, 0.43, 0.44, 0.48, 0.43, 0.6, 0.56, 1.56, 2.84, 2.44, 0.7, 0.88, 1.27, 0.44, 0.56, 1.14, 0.84, 1.28, 1.07, 1.36, 0.76, 1.31, 1.42, 0.51, 1.25, 1.62, 0.56, 1.84, 1.58, 0.45, 0.48, 0.54, 0.69, 0.93, 1.17, 0.85, 0.47, 0.85, 0.74, 1.86, 0.44, 0.57, 1.11, 0.8, 0.51, 0.56, 0.87, 0.88, 0.41, 0.42, 3.06, 0.82, 0.55, 0.66, 1.08, 0.5, 0.53, 0.64, 0.52, 0.66, 0.81, 1.17, 1.57, 0.9, 0.8, 0.71, 0.79, 0.64, 0.56, 1.16, 0.42, 0.41, 0.56, 0.94, 0.48, 1.65, 0.62, 2.12, 1.56, 1.68, 1.32, 1.24, 1.8, 0.85, 0.85, 2.2, 1.5, 1, 1.42, 0.74, 0.45, 0.51, 0.58, 0.74, 2.14, 0.46, 0.65, 0.55, 1.04, 1.29, 0.5, 2.46, 3.35, 0.68, 1.15, 1.11, 0.48, 2.57, 1.08, 0.88, 1.15, 1.25, 0.71, 0.64, 0.8, 0.67, 0.56, 0.58, 0.63, 0.47, 2.26, 0.42, 2.28, 0.5, 0.73, 1.88, 1.96, 1.16, 0.56, 0.67, 0.54, 0.98, 0.59, 0.62, 0.85, 0.55, 1.32, 0.6, 1.73, 1.7, 0.92, 1.12, 0.52, 1.49, 0.73, 0.6, 0.64, 0.48, 0.61, 0.6, 0.77, 0.43, 1.27, 0.59, 1.28, 0.6, 0.72, 0.52, 1.91, 0.46, 0.99, 2.18, 0.51, 0.56, 0.58, 0.42, 0.68, 0.84, 0.7, 0.69, 0.56, 0.42, 0.43, 1.25, 0.65, 0.52, 0.52, 0.57, 0.5, 1.76, 0.55, 0.57, 1.31, 0.69, 1.02, 0.41, 0.58, 1.35, 0.72, 1.11, 0.83, 0.58, 0.82, 0.45, 0.6, 0.48, 0.89, 1.08, 0.78, 0.53, 0.86, 0.82, 1, 0.54, 0.7, 1.02, 0.44, 0.75, 0.63, 0.65, 0.5, 0.41, 0.69, 0.43, 0.55, 1.99, 0.5, 0.5, 0.75, 1.41, 0.51, 0.43, 0.71, 0.68, 0.85, 0.5, 0.43, 0.55, 0.44, 0.5, 0.43, 0.52, 0.58, 0.48, 1, 0.75, 0.43, 0.65, 1.45, 0.42, 0.56, 1.02, 0.52, 1.8, 1.18, 0.52, 0.76, 1.04, 1.04, 0.55, 0.55, 0.68, 0.5, 0.5, 0.42, 0.9, 0.63, 0.8, 0.55, 0.42, 0.55, 0.64, 0.91, 0.85, 1.24, 0.74, 0.68, 0.73, 0.59, 1.2, 0.55, 0.69, 0.45, 0.48, 0.48, 1.88, 1.2, 1.12, 0.64, 0.56, 1.03, 0.64, 0.75, 0.53, 0.53, 1.13, 1.43, 1.53, 1.73, 0.48, 0.73, 0.51, 1.2, 2.6, 2.62, 0.5, 1, 0.51, 0.57, 0.62, 0.46, 0.44, 0.82, 0.65, 0.54, 0.84, 0.6, 0.58, 0.9, 1.1, 0.47, 0.48, 0.84, 0.68, 1.59, 0.63, 0.6, 0.83, 0.67, 0.56, 0.41, 0.52, 0.5, 2.34, 2.01, 0.45, 1.27, 1.27, 1.49, 0.44, 0.55, 0.75, 1.3, 0.6, 0.48, 0.5 };
            var wyDays = new double[] { 1, 1, 5, 5, 5, 6, 10, 10, 12, 13, 16, 17, 17, 17, 18, 19, 20, 20, 20, 23, 23, 24, 24, 25, 25, 26, 27, 28, 29, 29, 30, 30, 31, 31, 32, 33, 34, 36, 37, 38, 38, 39, 39, 39, 39, 39, 40, 40, 40, 41, 41, 41, 41, 42, 42, 42, 43, 43, 43, 43, 43, 45, 45, 46, 46, 47, 47, 47, 48, 48, 48, 49, 49, 49, 50, 51, 51, 51, 51, 51, 52, 52, 52, 52, 52, 52, 53, 53, 53, 54, 55, 56, 56, 56, 56, 56, 57, 57, 57, 57, 58, 59, 59, 59, 59, 60, 60, 60, 60, 60, 61, 61, 61, 61, 61, 62, 62, 62, 62, 63, 63, 63, 64, 64, 64, 65, 65, 65, 65, 66, 67, 67, 67, 67, 67, 68, 68, 68, 68, 68, 68, 68, 69, 69, 69, 69, 70, 70, 70, 70, 70, 71, 71, 72, 72, 73, 73, 73, 74, 74, 74, 75, 75, 75, 75, 76, 76, 76, 77, 77, 77, 77, 78, 78, 78, 78, 78, 78, 78, 78, 79, 79, 79, 79, 80, 80, 80, 80, 80, 81, 81, 81, 81, 81, 81, 82, 82, 82, 82, 83, 83, 83, 83, 83, 83, 84, 84, 84, 84, 85, 85, 85, 85, 86, 86, 86, 86, 86, 86, 87, 87, 87, 87, 88, 88, 89, 89, 89, 89, 89, 89, 89, 90, 90, 90, 90, 90, 90, 91, 91, 91, 91, 92, 92, 92, 93, 93, 93, 94, 94, 94, 94, 95, 95, 95, 95, 95, 96, 96, 96, 96, 96, 97, 97, 97, 97, 97, 97, 97, 97, 97, 97, 97, 98, 98, 98, 98, 98, 98, 98, 98, 98, 99, 99, 99, 99, 99, 99, 99, 99, 100, 100, 100, 100, 100, 101, 101, 101, 101, 101, 101, 101, 102, 102, 102, 102, 102, 102, 102, 102, 103, 103, 103, 103, 103, 104, 105, 105, 105, 105, 105, 106, 106, 106, 106, 106, 106, 106, 107, 107, 107, 107, 107, 107, 107, 108, 108, 108, 109, 109, 109, 109, 109, 109, 110, 110, 110, 110, 110, 110, 111, 111, 111, 111, 111, 112, 112, 112, 112, 112, 113, 113, 113, 113, 113, 114, 114, 114, 114, 114, 114, 114, 115, 115, 115, 116, 116, 116, 116, 117, 117, 117, 117, 117, 118, 118, 118, 118, 118, 119, 119, 119, 119, 119, 119, 120, 120, 120, 120, 120, 121, 121, 121, 121, 121, 122, 122, 122, 122, 122, 123, 123, 123, 124, 124, 125, 125, 125, 125, 126, 126, 126, 127, 127, 127, 128, 128, 128, 129, 129, 129, 129, 129, 129, 130, 130, 130, 131, 131, 131, 131, 131, 131, 132, 132, 132, 132, 132, 132, 132, 132, 132, 133, 133, 133, 133, 133, 133, 133, 133, 133, 133, 134, 134, 134, 134, 134, 134, 134, 134, 135, 135, 135, 135, 135, 135, 136, 136, 136, 136, 136, 136, 136, 137, 137, 137, 137, 137, 138, 138, 138, 138, 139, 139, 139, 139, 139, 139, 140, 140, 140, 140, 140, 141, 141, 141, 141, 141, 141, 142, 142, 142, 142, 142, 142, 142, 143, 143, 143, 143, 143, 143, 143, 143, 143, 144, 144, 144, 144, 144, 144, 144, 145, 145, 146, 146, 146, 146, 146, 146, 147, 147, 147, 147, 147, 148, 148, 148, 149, 149, 149, 149, 149, 149, 150, 150, 150, 150, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 152, 152, 152, 152, 152, 152, 152, 152, 152, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 154, 154, 154, 154, 154, 154, 155, 155, 155, 155, 155, 155, 156, 156, 156, 156, 156, 156, 157, 157, 157, 157, 157, 157, 157, 157, 157, 158, 158, 158, 158, 158, 158, 159, 159, 159, 159, 159, 159, 160, 160, 160, 160, 161, 161, 161, 161, 161, 162, 162, 162, 162, 162, 162, 162, 162, 163, 163, 163, 163, 163, 164, 164, 164, 165, 165, 165, 165, 165, 165, 166, 166, 166, 166, 166, 166, 166, 166, 166, 167, 167, 168, 168, 169, 169, 170, 170, 170, 171, 171, 171, 172, 172, 172, 172, 172, 172, 173, 173, 173, 173, 173, 173, 173, 174, 174, 174, 174, 175, 175, 175, 175, 176, 176, 176, 176, 176, 176, 176, 177, 177, 178, 178, 178, 178, 178, 179, 179, 179, 179, 180, 180, 180, 180, 181, 181, 182, 182, 183, 183, 183, 184, 184, 185, 185, 185, 186, 186, 187, 188, 189, 189, 189, 189, 190, 190, 190, 191, 191, 192, 192, 193, 193, 194, 194, 194, 195, 196, 196, 197, 197, 197, 197, 198, 198, 198, 199, 199, 200, 200, 201, 201, 201, 201, 202, 203, 203, 203, 203, 203, 204, 204, 204, 208, 208, 208, 209, 210, 211, 212, 214, 215, 216, 217, 217, 217, 218, 218, 219, 219, 220, 220, 220, 220, 220, 220, 221, 222, 222, 223, 225, 226, 228, 231, 236, 248, 249, 251, 256, 265, 271, 279, 281, 281, 281, 282, 284, 285, 287, 287, 289, 289, 291, 291, 292, 292, 292, 293, 293, 294, 294, 295, 297, 297, 297, 297, 298, 299, 300, 300, 300, 300, 301, 302, 303, 304, 304, 305, 306, 308, 309, 309, 311, 311, 315, 315, 317, 318, 319, 319, 319, 319, 320, 320, 320, 320, 320, 320, 321, 321, 322, 323, 324, 324, 324, 324, 325, 325, 326, 326, 326, 327, 329, 329, 329, 330, 331, 331, 332, 333, 333, 333, 334, 335, 336, 336, 339, 339, 340, 340, 341, 341, 342, 345, 346, 347, 349, 350, 352, 353, 354, 355, 355, 360, 360, 362, 365 };
            int ny = 65; // Number of years
            double thrsh = 0.4;

            var sampler = new DEMCzs(priors, parameters =>
            {
                
                double k = parameters[0];
                double loc1 = parameters[1];
                double scl1 = parameters[2];
                double shp1 = parameters[3];
                double loc2 = parameters[4];
                double scl2 = parameters[5];
                double shp2 = parameters[6];

                double logLH = 0;
                int d = 0;
                int n = 0;

                // Season 1
                for (int i = 0; i < data.Length; i++)
                {
                    d++;
                    n++;
                    if (wyDays[i] < k)
                    {
                        double ll = Math.Log(1d + shp1 * ((data[i] - loc1) / scl1));
                        ll *= (1d + 1d / shp1);
                        ll += Math.Log(scl1);
                        logLH +=  ll;
                    }
                    else
                    {
                        break;
                    }
                }
                logLH += ny * n / 366d * Math.Pow(1d + shp1 * ((thrsh - loc1) / scl1), -1d / shp1);
                n = 0;
                //
                // Season 2
                for (int i = d; i < data.Length; i++)
                {
                    n++;
                    double ll = Math.Log(1d + shp2 * ((data[i] - loc2) / scl2));
                    ll *= (1d + 1d / shp2);
                    ll += Math.Log(scl2);
                    logLH += ll;
                }
                logLH += ny * n / 366d * Math.Pow(1d + shp2 * ((thrsh - loc2) / scl2), -1d / shp2);
                logLH *= -1;


                // Prior likelihood
                for (int i = 0; i < priors.Count; i++)
                {
                    logLH += priors[i].PDF(parameters[i]);
                }

                if (double.IsNaN(logLH) || double.IsInfinity(logLH)) return double.MinValue;
                return logLH;
            });

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            sampler.Sample();
            var results = new MCMCResults(sampler);



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

            //var sig = Math.Sqrt((Math.Exp(errSigma2 * errSigma2) - 1.0d) * Math.Exp(2d * errMu + errSigma2 * errSigma2));

            var popDist = new Normal(popMU, popSigma);
            var errDist = new Normal(errMu, errSigma);

            var sample = popDist.GenerateRandomValues(12345, n);
            var prng = new MersenneTwister(12345);
            for (int i = 0; i < sample.Length; i++)
            {
                double sig = 0;
                if (Math.Exp(sample[i]) >= 550)
                {
                    sample[i] = sample[i] + errDist.InverseCDF(prng.NextDouble());
                    sig = Math.Sqrt((Math.Exp(errSigma2 * errSigma2) - 1.0d) * Math.Exp(2d * sample[i] + errSigma2 * errSigma2));
                }             
                //Debug.WriteLine(Math.Exp(sample[i]).ToString());
                
                Debug.WriteLine(sig.ToString());
            }


            var mu = Statistics.Mean(sample);
            var sigma = Statistics.StandardDeviation(sample);
            var priors = new List<IUnivariateDistribution> { new Uniform(0, 20), new Uniform(0, 10) };

            var sampler = new DEMCzs(priors, x =>
            {
                var norm = new Normal(x[0], x[1]);
                double w1 = norm.CDF(Math.Log(550));
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

            var muL = results.MAP.Values[0];
            var sigmaL = results.MAP.Values[1];
            var muR = Math.Exp(muL + sigmaL * sigmaL / 2.0d);
            var sigmaR = Math.Sqrt((Math.Exp(sigmaL * sigmaL) - 1.0d) * Math.Exp(2d * muL + sigmaL * sigmaL));

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
            var errDist = new Normal(errMu, errSigma);
            //var errDist = new Uniform(Math.Log(0.7), Math.Log(1.3));

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
                    double a = sample[i] + errDist.InverseCDF(1E-16);
                    double b = sample[i] + errDist.InverseCDF(1 - 1E-16);
                    var ex = Integration.GaussLegendre((q) => { return errDist.PDF(sample[i] - q) * norm.PDF(q); }, a, b);

                    var asr = new AdaptiveSimpsonsRule((q) => { return errDist.PDF(sample[i] - q) * norm.PDF(q); }, a, b);
                    asr.RelativeTolerance = 1E-4;
                    asr.Integrate();
                    var ex2 = asr.Result;

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
