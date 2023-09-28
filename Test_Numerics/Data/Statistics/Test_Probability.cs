using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics;
using Numerics.Data.Statistics;
using Numerics.Distributions;
using Numerics.Mathematics.LinearAlgebra;
using Numerics.Mathematics.SpecialFunctions;
using Numerics.Sampling;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Data.Statistics
{
    [TestClass]
    public class Test_Probability
    {
        [TestMethod]
        public void Test_JointProbability_PCM_2()
        {
            double A = 0.25;
            double B = 0.35;

            var mean = new double[] { 0, 0 };
            var r = 0;
            var covar = new double[,]
            { { 1, r },
              { r, 1 }};
            var mvn = new MultivariateNormal(mean, covar) { MVNUNI = new MersenneTwister(12345) };

            var probabilities = new double[] { A, B };
            var indicators = new int[] { 1, 0 };
            var jp = Probability.JointProbabilityPCM(probabilities, indicators, mvn);
            var jpe = Probability.JointProbability(probabilities, indicators, mvn);

        }

        [TestMethod]
        public void Test_JointProbability_PCM_3()
        {
            double A = 0.25;
            double B = 0.35;
            double C = 0.5;

            var mean = new double[] { 0, 0, 0 };
            var r = 0.8;
            var covar = new double[,]
            { { 1, r, r },
              { r, 1, r },
              { r, r, 1 }};
            var mvn = new MultivariateNormal(mean, covar) { MVNUNI = new MersenneTwister(12345) };

            var probabilities = new double[] { A, B, C };
            var indicators = new int[] { 1, 1, 1 };
            var jp = Probability.JointProbabilityPCM(probabilities, indicators, mvn);
            var jpe = Probability.JointProbability(probabilities, indicators, mvn);


        }

        [TestMethod]
        public void Test_JointProbability_PCM_4()
        {
            double A = 0.25;
            double B = 0.35;
            double C = 0.5;
            double D = 0.5;
            double E = 0.05;

            var mean = new double[] { 0, 0, 0, 0, 0 };
            var r = -0.24;
            var covar = new double[,]
            { { 1, r, r, r, r },
              { r, 1, r, r, r },
              { r, r, 1, r, r },
              { r, r, r, 1, r },
              { r, r, r, r, 1 }};
            var mvn = new MultivariateNormal(mean, covar) { MVNUNI = new MersenneTwister(12345) };

            var probabilities = new double[] { A, B, C, D, E };
            var indicators = new int[] { 1, 0, 0, 0, 0 };
            var jp = Probability.JointProbabilityPCM(probabilities, indicators, mvn);
            var jpe = Probability.JointProbability(probabilities, indicators, mvn);

        }


        [TestMethod]
        public void Test_JointProbability_PCM_5()
        {
            double A = 0.25;
            double B = 0.35;
            double C = 0.5;
            double D = 0.5;
            double E = 0.05;

            var mean = new double[] { 0, 0, 0, 0, 0 };
            var r = -0.1;
            var covar = new double[,]
            { { 1, r, r, r, r },
              { r, 1, r, r, r },
              { r, r, 1, r, r },
              { r, r, r, 1, r },
              { r, r, r, r, 1 }};
            var mvn = new MultivariateNormal(mean, covar) { MVNUNI = new MersenneTwister(12345) };

            var probabilities = new double[] { A, B, C, D, E};
            var indicators = new int[] { 1, 1, 0, 0, 0 };
            var jp = Probability.JointProbabilityPCM(probabilities, indicators, mvn);
            var jpe = Probability.JointProbability(probabilities, indicators, mvn);

            var jps = Probability.JointProbabilitiesPCM(probabilities, Factorial.AllCombinations(5), mvn);
            for (int i = 0; i < jps.Length; i++)
            {
                Debug.Print(jps[i].ToString());
            }

            var u = Probability.UnionPCM(probabilities, mvn);
            var ue = Probability.Union(probabilities, mvn);

        }


        [TestMethod]
        public void Test_JointProbability_PCM_10()
        {

            var probabilities = new double[] { 0, 0.0000000002142, 0.0000000000306, 0.0000003213, 0.00000000045, 0.00000000009, 0.00000002142, 0.000000045, 0.000000045, 0.00000000000306 };
            var mean = new double[10];
            var r = -1 / Math.Sqrt(9) + Tools.DoubleMachineEpsilon;
            var covar = new double[10, 10];

            for (int i = 0; i < 10; i++)
            {
                mean[i] = 0;
                for (int j = 0; j < 10; j++)
                {
                    if (i == j)
                    {
                        covar[i, j] = 1;
                    }
                    else if ( i == 8 || j == 8)
                    {
                        covar[i, j] = r;
                    }
                    else
                    {
                        covar[i, j] = 0;
                    }
                }
            }

            var mvn = new MultivariateNormal(mean, covar) { MVNUNI = new MersenneTwister(12345) };


            //var indicators = new int[] { 1, 1, 0, 0, 0 };
            //var jp = Probability.JointProbabilityPCM(probabilities, indicators, mvn);
            //var jpe = Probability.JointProbability(probabilities, indicators, mvn);

            //var jps = Probability.JointProbabilitiesPCM(probabilities, Factorial.AllCombinations(10), mvn);
            //for (int i = 0; i < jps.Length; i++)
            //{
            //    Debug.Print(jps[i].ToString());
            //}



            //var ue = Probability.Union(probabilities, mvn);

            var combos = new int[10];
            for (int i = 1; i <= 10; i++)
            {
                combos[i - 1] = (int)Factorial.BinomialCoefficient(10, i);
            }

            var indicators = Factorial.AllCombinations(10);

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var eps = Probability.ExclusivePCM(probabilities, combos, indicators, mvn);
            var sumEps = Tools.Sum(eps);

            stopWatch.Stop();
            var timeSpan = stopWatch.Elapsed;



            stopWatch = new Stopwatch();
            stopWatch.Start();

            var probs = new List<double>();
            var inds = new List<int[]>();

            // Probability.IndependentExclusive(probabilities, combos, indicators, out probs, out inds);

            // Probability.PositivelyDependentExclusive(probabilities, combos, indicators, out probs, out inds);

            Probability.ExclusivePCM(probabilities, combos, indicators, mvn, out probs, out inds);

            var sumProbs = Tools.Sum(probs);

            //var u = Probability.PositivelyDependentUnion(probabilities);

            //var u = Probability.UnionPCM(probabilities, mvn);

            stopWatch.Stop();
            timeSpan = stopWatch.Elapsed;

        }


        [TestMethod]
        public void Test_UnionAB()
        {
            double A = 0.25;
            double B = 0.35;

            var mean = new double[] { 0, 0 };
            var covar = new double[,]
            { { 1, 0 },
              { 0, 1 }};
            var mvn = new MultivariateNormal(mean, covar) { MVNUNI = new MersenneTwister(12345) };

            var union = Probability.Union(new double[] { A, B }, mvn);
            Assert.AreEqual(union, 0.5125, 1E-6);
        }

        [TestMethod]
        public void Test_UnionABC()
        {
            double A = 0.25;
            double B = 0.35;
            double C = 0.5;

            var mean = new double[] { 0, 0, 0 };
            var covar = new double[,]
            { { 1, 0, 0 },
              { 0, 1, 0 },
              { 0, 0, 1 }};
            var mvn = new MultivariateNormal(mean, covar) { MVNUNI = new MersenneTwister(12345) };

            var union = Probability.Union(new double[] { A, B, C }, mvn);
            Assert.AreEqual(union, 0.75625, 1E-6);

        }

        [TestMethod]
        public void Test_UnionABCD()
        {
            double A = 0.25;
            double B = 0.35;
            double C = 0.5;
            double D = 0.5;

            var mean = new double[] { 0, 0, 0, 0 };
            var r = -0.33;
            var covar = new double[,]
            { { 1, r, r, r },
              { r, 1, r, r },
              { r, r, 1, r },
              { r, r, r, 1 }};
            var mvn = new MultivariateNormal(mean, covar) { MVNUNI = new MersenneTwister(12345) };

            var union = Probability.Union(new double[] { A, B, C, D }, mvn);

            //mvn = new MultivariateNormal(20);
            //var z = mvn.IndependentNormal(0).GenerateRandomValues(20);
            //var p = Normal.StandardCDF(z);
            //var v = new Vector(p);
            //v = v * 1E-3;

            //union = Probability.IndependentUnion(v.ToArray());
            //var union2 = Probability.UnionPCM(new double[] { A, B, C, D }, mvn);

            //Assert.AreEqual(union, 0.985016583, 1E-6);

        }

        [TestMethod]
        public void Test_UnionABCD_2()
        {
            
            double A = 0.25;
            double B = 0.35;
            double C = 0.5;
            double D = 0.5;

            int N = 4;
            var combos = new int[N];
            for (int i = 1; i <= N; i++)
            {
                combos[i - 1] = (int)Factorial.BinomialCoefficient(N, i);
            }

            var mean = new double[] { 0, 0, 0, 0 };
            var r = -0.33;
            var covar = new double[,]
            { { 1, r, r, r },
              { r, 1, r, r },
              { r, r, 1, r },
              { r, r, r, 1 }};
            var mvn = new MultivariateNormal(mean, covar) { MVNUNI = new MersenneTwister(12345) };

            var union = Probability.Union(new double[] { A, B, C, D }, combos, Factorial.AllCombinations(N), mvn);
            //Assert.AreEqual(union, 0.985016583, 1E-6);

        }

        [TestMethod]
        public void Test_IndependentUnionABCD()
        {
            double A = 0.25;
            double B = 0.35;
            double C = 0.5;
            double D = 0.5;
            var union = Probability.IndependentUnion(new double[] { A, B, C, D });
            Assert.AreEqual(union, 0.878125, 1E-6);

        }

        [TestMethod]
        public void Test_PositivelyDependentUnionABCD()
        {
            double A = 0.25;
            double B = 0.35;
            double C = 0.5;
            double D = 0.5;
            var union = Probability.PositivelyDependentUnion(new double[] { A, B, C, D });
            Assert.AreEqual(union, 0.5, 1E-6);

        }

        [TestMethod]
        public void Test_NegativelyDependentUnionABCD()
        {
            double A = 0.25;
            double B = 0.35;
            double C = 0.5;
            double D = 0.5;
            var union = Probability.NegativelyDependentUnion(new double[] { A, B, C, D });
            Assert.AreEqual(union, 1.0, 1E-6);

        }

        [TestMethod]
        public void Test_ExclusiveAB()
        {
            double A = 0.25;
            double B = 0.35;

            var mean = new double[] { 0, 0 };
            var covar = new double[,]
            { { 1, 0 },
              { 0, 1 }};
            var mvn = new MultivariateNormal(mean, covar) { MVNUNI = new MersenneTwister(12345) };

            var trueE = new double[] { 0.1625, 0.2625, 0.0875 };
            var exclusive = Probability.Exclusive(new double[] { A, B }, mvn);

            for (int i = 0; i < exclusive.Length; i++)
            {
                Assert.AreEqual(exclusive[i], trueE[i], 1E-6);
            }

            var union = Tools.Sum(exclusive);
            Assert.AreEqual(union, 0.5125, 1E-6);
        }

        [TestMethod]
        public void Test_ExclusiveABC()
        {
            double A = 0.25;
            double B = 0.35;
            double C = 0.5;

            var mean = new double[] { 0, 0, 0 };
            var covar = new double[,]
            { { 1, 0, 0 },
              { 0, 1, 0 },
              { 0, 0, 1 }};
            var mvn = new MultivariateNormal(mean, covar) { MVNUNI = new MersenneTwister(12345) };

            var trueE = new double[] { 0.08125, 0.13125, 0.24375, 0.04375, 0.08125, 0.13125, 0.04375 };
            var exclusive = Probability.Exclusive(new double[] { A, B, C }, mvn);

            for (int i = 0; i < exclusive.Length; i++)
            {
                Assert.AreEqual(exclusive[i], trueE[i], 1E-6);
            }

            var union = Tools.Sum(exclusive);
            Assert.AreEqual(union, 0.75625, 1E-6);

        }

        [TestMethod]
        public void Test_ExclusiveABCD()
        {
            double A = 0.25;
            double B = 0.35;
            double C = 0.5;
            double D = 0.5;

            var mean = new double[] { 0, 0, 0, 0 };
            var r = 0.0;
            var covar = new double[,]
            { { 1, r, r, r },
              { r, 1, r, r },
              { r, r, 1, r },
              { r, r, r, 1 }};
            var mvn = new MultivariateNormal(mean, covar) { MVNUNI = new MersenneTwister(12345) };

            var trueE = new double[] { 0.040625, 0.065625, 0.121875, 0.121875, 0.021875, 0.040625, 0.040625, 0.065625, 0.065625, 0.121875, 0.021875, 0.021875, 0.040625, 0.065625, 0.021875 };
            var exclusive = Probability.Exclusive(new double[] { A, B, C, D }, mvn);

            for (int i = 0; i < exclusive.Length; i++)
            {
                Assert.AreEqual(exclusive[i], trueE[i], 1E-6);
            }

            var union = Tools.Sum(exclusive);
            Assert.AreEqual(union, 0.878125, 1E-6);
        }

        [TestMethod]
        public void Test_ExclusiveABCD_Neg()
        {
            double A = 0.25;
            double B = 0.35;
            double C = 0.5;
            double D = 0.5;

            var mean = new double[] { 0, 0, 0, 0};
            var r = -0.33;
            var covar = new double[,]
            { { 1, r, r, r },
              { r, 1, r, r },
              { r, r, 1, r },
              { r, r, r, 1 }};
            var mvn = new MultivariateNormal(mean, covar) { MVNUNI = new MersenneTwister(12345) };

            var trueE = new double[] { 0.0591144184207316,  0.0841585674381143, 0.137370396630316,  0.137376624193301,  0.0381933344506983, 0.0639822078713156, 0.063975145059157,  0.0962373338116287, 0.0962331282359124, 0.160388585792858,  0.00596032423933968,    0.00596536506423,   0.012809204894465,  0.0232519467600138, 6.27974351606123E-14 };
            var exclusive = Probability.Exclusive(new double[] { A, B, C, D }, mvn);

            for (int i = 0; i < exclusive.Length; i++)
            {
                Assert.AreEqual(exclusive[i], trueE[i], 1E-4);
            }

            var union = Tools.Sum(exclusive);
            Assert.AreEqual(union, 0.985016582862144, 1E-3);
        }

        [TestMethod]
        public void Test_IndependentExclusiveAB()
        {
            int N = 2;
            double A = 0.25;
            double B = 0.35;

            var trueE = new double[] { 0.1625, 0.2625, 0.0875 };
            var exclusive = Probability.IndependentExclusive(new double[] { A, B });

            for (int i = 0; i < exclusive.Length; i++)
            {
                Assert.AreEqual(exclusive[i], trueE[i], 1E-6);
            }

            var union = Tools.Sum(exclusive);
            Assert.AreEqual(union, 0.5125, 1E-6);
        }

        [TestMethod]
        public void Test_IndependentExclusiveABC()
        {
            int N = 3;
            double A = 0.25;
            double B = 0.35;
            double C = 0.5;

            var trueE = new double[] { 0.08125, 0.13125, 0.24375, 0.04375, 0.08125, 0.13125, 0.04375 };
            var exclusive = Probability.IndependentExclusive(new double[] { A, B, C });

            for (int i = 0; i < exclusive.Length; i++)
            {
                Assert.AreEqual(exclusive[i], trueE[i], 1E-6);
            }

            var union = Tools.Sum(exclusive);
            Assert.AreEqual(union, 0.75625, 1E-6);
        }

        [TestMethod]
        public void Test_IndependentExclusiveABCD()
        {
            int N = 4;
            double A = 0.25;
            double B = 0.35;
            double C = 0.5;
            double D = 0.5;

            var trueE = new double[] { 0.040625, 0.065625, 0.121875, 0.121875, 0.021875, 0.040625, 0.040625, 0.065625, 0.065625, 0.121875, 0.021875, 0.021875, 0.040625, 0.065625, 0.021875 };
            var exclusive = Probability.IndependentExclusive(new double[] { A, B, C, D });

            for (int i = 0; i < exclusive.Length; i++)
            {
                Assert.AreEqual(exclusive[i], trueE[i], 1E-6);
            }

            var union = Tools.Sum(exclusive);
            Assert.AreEqual(union, 0.878125, 1E-6);
        }

        [TestMethod]
        public void Test_PositivelyDependentExclusiveAB()
        {
            int N = 2;
            double A = 0.25;
            double B = 0.35;

            var trueE = new double[] { 0, 0.1, 0.25 };
            var exclusive = Probability.PositivelyDependentExclusive(new double[] { A, B });

            for (int i = 0; i < exclusive.Length; i++)
            {
                Assert.AreEqual(exclusive[i], trueE[i], 1E-6);
            }

            var union = Tools.Sum(exclusive);
            Assert.AreEqual(union, 0.35, 1E-6);
        }

        [TestMethod]
        public void Test_PositivelyDependentExclusiveABC()
        {
            int N = 3;
            double A = 0.25;
            double B = 0.35;
            double C = 0.5;

            var trueE = new double[] { 0, 0, 0.15, 0, 0, 0.1, 0.25 };
            var exclusive = Probability.PositivelyDependentExclusive(new double[] { A, B, C });

            for (int i = 0; i < exclusive.Length; i++)
            {
                Assert.AreEqual(exclusive[i], trueE[i], 1E-6);
            }

            var union = Tools.Sum(exclusive);
            Assert.AreEqual(union, 0.5, 1E-6);
        }

        [TestMethod]
        public void Test_PositivelyDependentExclusiveABCD()
        {
            int N = 4;
            double A = 0.25;
            double B = 0.35;
            double C = 0.5;
            double D = 0.5;

            var trueE = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.15, 0, 0, 0, 0.1, 0.25 };
            var exclusive = Probability.PositivelyDependentExclusive(new double[] { A, B, C, D });

            for (int i = 0; i < exclusive.Length; i++)
            {
                Assert.AreEqual(exclusive[i], trueE[i], 1E-6);
            }

            var union = Tools.Sum(exclusive);
            Assert.AreEqual(union, 0.5, 1E-6);
        }

        [TestMethod]
        public void Test_Runtime()
        {
            double A = 0.25;
            double B = 0.35;
            double C = 0.5;
            double D = 0.5;

            var mean = new double[] { 0, 0, 0, 0 };
            var r = -0.33;
            var covar = new double[,]
            { { 1, r, r, r },
              { r, 1, r, r },
              { r, r, 1, r },
              { r, r, r, 1 }};
            var mvn = new MultivariateNormal(mean, covar) { MVNUNI = new MersenneTwister(12345) };

            int N = 4;
            var combos = new int[N];
            for (int i = 1; i <= N; i++)
            {
                combos[i - 1] = (int)Factorial.BinomialCoefficient(N, i);
            }

            var indicators = Factorial.AllCombinations(N);

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            for (int i = 0; i < 1000; i++)
            {
                //var exclusive = Probability.Exclusive(new double[] { A, B, C, D }, combos, indicators, mvn);
                //var exclusive = Probability.ExclusivePCM(new double[] { A, B, C, D }, combos, indicators, mvn);
                var exclusive = Probability.IndependentExclusive(new double[] { A, B, C, D }, indicators);
            }

            stopWatch.Stop();
            var timeSpan = stopWatch.Elapsed;


        }

        [TestMethod]
        public void Test_Runtime_Exclusive()
        {
            int N = 5;

            var rnd = new MersenneTwister(12345);
            var probabilities = new double[N];
            var mean = new double[N];
            var r = -1d/((double)N-1d) + Tools.DoubleMachineEpsilon;
            var covar = new double[N, N];
            for (int i = 0; i < N; i++)
            {
                probabilities[i] = rnd.NextDouble();
                mean[i] = 0d;
                for (int j = 0; j < N; j++)
                {
                    if (i == j)
                    {
                        covar[i, j] = 1d;
                    }
                    else
                    {
                        covar[i, j] = r;
                    }
                }
            }
            var mvn = new MultivariateNormal(mean, covar) { MVNUNI = new MersenneTwister(12345) };

            
            var combos = new int[N];
            for (int i = 1; i <= N; i++)
            {
                combos[i - 1] = (int)Factorial.BinomialCoefficient(N, i);
            }

            var indicators = Factorial.AllCombinations(N);

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            for (int i = 0; i < 100; i++)
            {
                //var exclusive = Probability.Exclusive(probabilities, combos, indicators, mvn);
                var exclusive = Probability.ExclusivePCM(probabilities, combos, indicators, mvn);
                //var exclusive = Probability.IndependentExclusive(probabilities, indicators);
            }

            stopWatch.Stop();
            var timeSpan = stopWatch.Elapsed;


        }

    }
}
