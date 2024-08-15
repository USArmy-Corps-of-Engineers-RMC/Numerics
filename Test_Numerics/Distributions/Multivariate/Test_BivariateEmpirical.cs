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
using Numerics.Data;
using Numerics.Data.Statistics;
using Numerics.Distributions;
using Numerics.Sampling;

namespace Distributions.Multivariate
{
    [TestClass]
    public class Test_BivariateEmpirical
    {
        [TestMethod()]
        public void Test_BivariateEmp()
        {
            var X1vals = new double[] { 1d, 2d, 3d, 4d, 5d };
            var X2vals = new double[] { 6d, 7d, 8d, 9d, 10d };
            var Pvals = new double[,] { { 0.00405294623516298d, 0.0132662170105167d, 0.0207236588305977d, 0.022603272182165d, 0.0227468879771625d }, { 0.0132662170105167d, 0.0625140947096637d, 0.127398206576625d, 0.154872951858602d, 0.158508394165442d }, { 0.0207236588305977d, 0.127398206576625d, 0.333333333333333d, 0.468742952645168d, 0.497973526882419d }, { 0.022603272182165d, 0.154872951858602d, 0.468742952645168d, 0.745203586846751d, 0.831860831130881d }, { 0.0227468879771625d, 0.158508394165442d, 0.497973526882419d, 0.831860831130881d, 0.958552682338805d } };
            var bv = new BivariateEmpirical(X1vals, X2vals, Pvals);

            // Real space
            double true_cdf1 = 0.008659582d;
            double true_cdf2 = 0.388089576d;
            double true_cdf3 = 0.497681221d;
            double cdf1 = bv.CDF(1.5d, 6d);
            double cdf2 = bv.CDF(3.22d, 8.15d);
            double cdf3 = bv.CDF(3d, 9.99d);
            Assert.AreEqual(cdf1, true_cdf1, 1E-6);
            Assert.AreEqual(cdf2, true_cdf2, 1E-6);
            Assert.AreEqual(cdf3, true_cdf3, 1E-6);

            // Log 10 space
            double true_cdf4 = 0.379680109d;
            bv.ProbabilityTransform = Numerics.Data.Transform.Logarithmic;
            double cdf4 = bv.CDF(3.22d, 8.15d);
            Assert.AreEqual(cdf4, true_cdf4, 1E-6);

            // Z space
            double true_cdf5 = 0.386806419d;
            bv.ProbabilityTransform = Numerics.Data.Transform.NormalZ;
            double cdf5 = bv.CDF(3.22d, 8.15d);
            Assert.AreEqual(cdf5, true_cdf5, 1E-6);
        }

        [TestMethod()]
        public void Test_Bivariate_SRP()
        {
            // Seismic Hazard Curve
            var pgaLevels = new double[] { 0.01, 0.05, 0.1, 0.15, 0.2, 0.25, 0.3, 0.35, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1 };
            var pgaAEP = new double[] { 0.216475, 0.023343, 0.005343, 0.002028, 0.001024, 0.000614, 0.000407, 0.000286, 0.000208, 0.000117, 0.0000692, 0.0000421, 0.0000264, 0.0000168, 0.0000107 };
            var pgaFrequency = new EmpiricalDistribution(pgaLevels, pgaAEP, SortOrder.Ascending, SortOrder.Descending) { ProbabilityTransform = Transform.Logarithmic };

            // Stage Duration Curve
            var stageDurationLevels = new double[] { 2529.8, 2529.9, 2530, 2530.3, 2536.1, 2542.1, 2544.9, 2548.3, 2550.9, 2556.7, 2562.5, 2569.3, 2571.7, 2575.5, 2582.4, 2586.7, 2591.3, 2598.5, 2603.3, 2604.5, 2605.3, 2609.1, 2610.5, 2620.6, 2629.2, 2631.7, 2632.8, 2633.4, 2633.5 };
            var stageDurationEP = new double[] { 0.999, 0.998, 0.995, 0.99, 0.98, 0.95, 0.9, 0.85, 0.8, 0.7, 0.6, 0.5, 0.4, 0.3, 0.2, 0.15, 0.1, 0.05, 0.02, 0.01, 0.005, 0.002, 0.001, 0.0000673, 0.00000676, 0.00000347, 0.00000258, 0.0000022, 0.00000214 };
            var stageDuration = new EmpiricalDistribution(stageDurationLevels, stageDurationEP, SortOrder.Ascending, SortOrder.Descending) { ProbabilityTransform = Transform.NormalZ };

            // Bivariate Response Function
            var primaryLevels = new double[] { 0.2, 0.4, 0.6, 0.8 };
            var secondaryLevels = new double[] { 2560, 2585.5, 2590, 2605.5, 2611, 2633.5 };
            var probabilities = new double[,] { { 0.00000054, 0.000279, 0.00101, 0.00412, 0.00648, 0.00816 },
                                                { 0.0000144, 0.00744, 0.027, 0.11, 0.173, 0.218 },
                                                { 0.0000421, 0.0217, 0.0789, 0.321, 0.505, 0.636 },
                                                { 0.0000539, 0.0278, 0.101, 0.411, 0.647, 0.814 }};
            var bivariateResponse = new BivariateEmpirical(primaryLevels, secondaryLevels, probabilities) { ProbabilityTransform = Transform.Logarithmic };


            int realz = 100000000;
            var prng = new MersenneTwister(45678);
            double srp = 0;
            for (int i = 0; i < realz; i++)
            {
                // Sample earthquake
                var pga = pgaFrequency.InverseCDF(prng.NextDouble());
                // Sample reservoir stage
                var stage = stageDuration.InverseCDF(prng.NextDouble());
                // Get SRP given earthquake and stage
                srp += bivariateResponse.CDF(new double[] { 0.8, stage });
            }
            srp /= realz;

        }

        [TestMethod()]
        public void Test_Bivariate_Risk()
        {
            // Seismic Hazard Curve
            var pgaLevels = new double[] { 0.01, 0.05, 0.1, 0.15, 0.2, 0.25, 0.3, 0.35, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1 };
            var pgaAEP = new double[] { 0.216475, 0.023343, 0.005343, 0.002028, 0.001024, 0.000614, 0.000407, 0.000286, 0.000208, 0.000117, 0.0000692, 0.0000421, 0.0000264, 0.0000168, 0.0000107 };
            var pgaFrequency = new EmpiricalDistribution(pgaLevels, pgaAEP, SortOrder.Ascending, SortOrder.Descending) { ProbabilityTransform = Transform.Logarithmic };

            // Stage Duration Curve
            var stageDurationLevels = new double[] { 2529.8, 2529.9, 2530, 2530.3, 2536.1, 2542.1, 2544.9, 2548.3, 2550.9, 2556.7, 2562.5, 2569.3, 2571.7, 2575.5, 2582.4, 2586.7, 2591.3, 2598.5, 2603.3, 2604.5, 2605.3, 2609.1, 2610.5, 2620.6, 2629.2, 2631.7, 2632.8, 2633.4, 2633.5 };
            var stageDurationEP = new double[] { 0.999, 0.998, 0.995, 0.99, 0.98, 0.95, 0.9, 0.85, 0.8, 0.7, 0.6, 0.5, 0.4, 0.3, 0.2, 0.15, 0.1, 0.05, 0.02, 0.01, 0.005, 0.002, 0.001, 0.0000673, 0.00000676, 0.00000347, 0.00000258, 0.0000022, 0.00000214 };
            var stageDuration = new EmpiricalDistribution(stageDurationLevels, stageDurationEP, SortOrder.Ascending, SortOrder.Descending) { ProbabilityTransform = Transform.NormalZ };

            // Bivariate Response Function
            var primaryLevels = new double[] { 0.2, 0.4, 0.6, 0.8 };
            var secondaryLevels = new double[] { 2560, 2585.5, 2590, 2605.5, 2611, 2633.5 };
            var probabilities = new double[,] { { 0.00000054, 0.000279, 0.00101, 0.00412, 0.00648, 0.00816 },
                                                { 0.0000144, 0.00744, 0.027, 0.11, 0.173, 0.218 },
                                                { 0.0000421, 0.0217, 0.0789, 0.321, 0.505, 0.636 },
                                                { 0.0000539, 0.0278, 0.101, 0.411, 0.647, 0.814 }};
            var bivariateResponse = new BivariateEmpirical(primaryLevels, secondaryLevels, probabilities) { ProbabilityTransform = Transform.Logarithmic };

            // Life Loss consequence function
            var cfStageLevels = new double[] { 2544.9, 2591.3, 2605.5, 2633.5, 2634.5 };
            var cfLifeLoss = new double[] { 0, 53.95, 194.1, 359.95, 409.15 };
            var lifeLoss = new Linear(cfStageLevels, cfLifeLoss);

            // Monte Carlo simulation
            int realz = 100000000;
            var Cf = new double[realz];
            double Ecf = 0;
            var prng = new MersenneTwister(45678);
            for (int i = 0; i < realz; i++)
            {
                // Sample earthquake
                var pga = pgaFrequency.InverseCDF(prng.NextDouble());
                // Sample reservoir stage
                var stage = stageDuration.InverseCDF(prng.NextDouble());
                // Get SRP given earthquake and stage
                var srp = bivariateResponse.CDF(new double[] { pga, stage });
                // See if there is a failure
                var rnd = prng.NextDouble(); 
                if (rnd <= srp)
                {
                    // Failure occurred, record consequences
                    Cf[i] = lifeLoss.Interpolate(stage);
                    Ecf += Cf[i];
                }          
            }

            // Get mean
            Ecf /= realz;

            // Create FN Curve
            Array.Sort(Cf);
            Array.Reverse(Cf);
            var pp = PlottingPositions.Weibull(realz);
            var pVals = new List<double>();
            var xVals = new List<double>();
            for (int i = 0; i < Cf.Length; i++)
            {
                // Only print when necessary
                if (xVals.Count == 0)
                {
                    pVals.Add(pp[i]);
                    xVals.Add(Cf[i]);
                    Debug.Print(xVals.Last().ToString() + "," + pVals.Last().ToString());
                }
                else if (xVals.Count > 0 && Cf[i] != xVals.Last())
                {
                    pVals.Add(pp[i]);
                    xVals.Add(Cf[i]);
                    Debug.Print(xVals.Last().ToString() + "," + pVals.Last().ToString());
                }
            }

        }

    }
}
