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
using System.Threading;

namespace Data.Statistics
{
    [TestClass]
    public class Test_Probability
    {

        [TestMethod]
        public void Test_JointProbability_PCM_5_CDFs()
        {
            var cdf1 = new Normal(140, 30);
            var cdf2 = new Normal(160, 10);
            var cdf3 = new Normal(150, 20);
            var cdf4 = new Normal(130, 35);
            var cdf5 = new Normal(160, 15);

            var cr = new CompetingRisks(new[] { cdf1, cdf2, cdf3, cdf4, cdf5 });
            cr.Dependency = Probability.DependencyType.PerfectlyPositive;


            var corr = new double[,] {{1, 0.03, 0.29, -0.04, 0.85},
                                 {0.03, 1, -0.08, 0.32, 0.05},
                                 {0.29, -0.08, 1, 0.59, -0.18},
                                 {-0.04, 0.32, 0.59, 1, -0.17},
                                 {0.85, 0.05, -0.18, -0.17, 1}};

            var mvn = new MultivariateNormal(new double[] { 0, 0, 0, 0, 0 }, corr);

            var cif = cr.CumulativeIncidenceFunctions();
            for (int i = 0; i < cif[0].ProbabilityValues.Count; i++)
            {
                Debug.Print(cif[0].XValues[i].ToString() + "," + cif[0].ProbabilityValues[i].ToString() + "," + cif[1].ProbabilityValues[i].ToString() + "," + cif[2].ProbabilityValues[i].ToString() + "," + cif[3].ProbabilityValues[i].ToString() + "," + cif[4].ProbabilityValues[i].ToString());
            }


        }

        [TestMethod]
        public void Test_JointProbability_PCM_10_CDFs()
        {
            var cdf1 = new Normal(0, 1);
            var cdf2 = new Normal(0, 1);
            var cdf3 = new Normal(0, 1);
            var cdf4 = new Normal(0, 1);
            var cdf5 = new Normal(0, 1);
            var cdf6 = new Normal(0, 1);
            var cdf7 = new Normal(0, 1);
            var cdf8 = new Normal(0, 1);
            var cdf9 = new Normal(0, 1);
            var cdf10 = new Normal(1, 1);

            var cr = new CompetingRisks(new[] { cdf1, cdf2, cdf3, cdf4, cdf5, cdf6, cdf7, cdf8, cdf9, cdf10 });
            cr.Dependency = Probability.DependencyType.Independent;

            var mu = new double[10];
            var corr = new double[10, 10];
            for (int i = 0; i < 10; i++)
            {
                mu[i] = 0;
                for (int j = 0; j < 10; j++)
                {
                    corr[i,j] = i == j ? 1 : 0.8;
                }
            }
            var mvn = new MultivariateNormal(mu, corr);

            var cif = cr.CumulativeIncidenceFunctions(mvn);
            for (int i = 0; i < cif[0].ProbabilityValues.Count; i++)
            {
                Debug.Print(cif[0].XValues[i].ToString() + "," + 
                    cif[0].ProbabilityValues[i].ToString() + "," + 
                    cif[1].ProbabilityValues[i].ToString() + "," + 
                    cif[2].ProbabilityValues[i].ToString() + "," + 
                    cif[3].ProbabilityValues[i].ToString() + "," + 
                    cif[4].ProbabilityValues[i].ToString() + "," +
                    cif[5].ProbabilityValues[i].ToString()+"," +
                    cif[6].ProbabilityValues[i].ToString() + "," +
                    cif[7].ProbabilityValues[i].ToString() + "," +
                    cif[8].ProbabilityValues[i].ToString() + "," +
                    cif[9].ProbabilityValues[i].ToString());
            }

            var h = new Normal(-4, 1);
            var rnd = new Random(12345);
            double pf = 0;
            for (int i = 0;i < 1E8; i++)
            {
                var x = h.InverseCDF(rnd.NextDouble());
                var z = rnd.NextDouble();
                if (z<= cif[9].CDF(x))
                {
                    pf += 1;
                }
            }
            pf /= 1E8;
        }


        [TestMethod]
        public void Test_JointProbability_PCM_Sensitivity()
        {
            int D = 5;
            double Z = -3;
            var probabilities = new double[D];
            probabilities.Fill(Normal.StandardCDF(Z));

            probabilities = new double[] { Normal.StandardCDF(-3), Normal.StandardCDF(-2), Normal.StandardCDF(-1), Normal.StandardCDF(1), Normal.StandardCDF(2) };
            var indicators = new int[D];
            indicators.Fill(1);

            for (double r = -0.24; r < 1; r += 0.01)
            {
                var mu = new double[D];
                var covar = new double[D, D];
                for (int i = 0; i < D; i++)
                {
                    mu[i] = 0;
                    for (int j = 0; j < D; j++)
                    {
                        covar[i, j] = i == j ? 1 : r;
                    }
                }

                var mvn = new MultivariateNormal(mu, covar) { MVNUNI = new MersenneTwister(12345) };

                var jpe = Probability.JointProbability(probabilities, indicators, mvn);
                var jp = Probability.JointProbabilityPCM(probabilities, indicators, covar);
                var jpI = Probability.JointProbabilityHPCM(probabilities, indicators, covar);
                Debug.Print(r + "," + jpe + "," + jp + "," + jpI);

            }
        }

        [TestMethod]
        public void Test_JointProbability_PCM_Correlation()
        {
            int D = 5;
            var zin = new double[] { 1, 1.5, 0.75, 1.25, 0.5 };//, 0.75, 1.25, 0.5 };
            var zin2 = new double[] { 1.5, 1.25, 1, 0.75, 0.5 };



            //{ 2, 2, 2, 2, 2 };
            var probabilities = Normal.StandardCDF(zin);
            var indicators = new int[D];
            indicators.Fill(1);

            //var corr = new double[,] {{1, -0.457, 0.377, -0.402, -0.0593},
            //                     {-0.457, 1, -0.772, 0.58, -0.316},
            //                     {0.377, -0.772, 1, -0.732, 0.412},
            //                     {-0.402, 0.58, -0.732, 1, 0.222},
            //                     {-0.0593, -0.316, 0.412, 0.222, 1}};
            var corr = new double[2,2] ;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    corr[i, j] = i == j ? 1 : 0.99;
                }
            }

            var corr2 = new double[D, D];
            for (int i = 0; i < D; i++)
            {
                for (int j = 0; j < D; j++)
                {
                    corr2[i, j] = i == j ? 1 : 1-Tools.DoubleMachineEpsilon;
                }
            }

            var cp = new double[D];
            var R = new double[D, D];
            var jpI = Probability.JointProbabilityHPCM(probabilities, indicators, corr2, cp, R);



            for (int i = 0; i < D; i++)
            {
                Debug.Print(R[i, 0] + "," + R[i, 1] + "," + R[i, 2] + "," + R[i, 3] + "," + R[i, 4]);
            }

            var mvn = new MultivariateNormal(new double[] { 0, 0, 0, 0, 0 }, corr2) { MVNUNI = new Random(12345), AbsoluteError = 1E-8, RelativeError = 1E-8, MaxEvaluations = 1000000000 };
            //var mvn2 = new MultivariateNormal(new double[] { 0, 0, 0, 0 }, corr2) { MVNUNI = new Random(12345), AbsoluteError = 1E-15, RelativeError = 1E-15, MaxEvaluations = 1000000000 };
            var cpp = mvn.CDF(zin);// / mvn.CDF(zin));

            int M = 10000000;
            var z = mvn.GenerateRandomValues(M, 12345);
            var Iz = new double[M, D];
            var I = new double[D];
            var Ic = new double[D];
            var zout = new double[D];
            zout[0] = zin[0];
            for (int i = 0; i < M; i++)
            {

                bool under = true;
                for (int j = 0;j < D; j++)
                {
                    if (z[i,j] > zin2[j])
                    {
                        under = false;
                        break;
                    }
                }
                if (under) I[1]++;
                //if (z[i, 0] <= 1.39507637575618 && z[i,1] <= 3.26396998737709)
                //{
                //    //Ic[1]++;

                //    //if (z[i, 1] <= zin[1])
                //      I[1]++;
                //}
            }
            Debug.Print((I[1] / M).ToString());



        }

        [TestMethod]
        public void Test_JointProbability_PCM_2()
        {
            double A = Normal.StandardCDF(-2);// 0.25;
            double B = Normal.StandardCDF(-2); //0.35;

            var mean = new double[] { 0, 0 };
            var r = 0.9;
            var covar = new double[,]
            { { 1, r },
              { r, 1 }};
            var mvn = new MultivariateNormal(mean, covar) { MVNUNI = new MersenneTwister(12345) };

            var probabilities = new double[] { A, B };
            var indicators = new int[] { 1, 1 };
            var jp = Probability.JointProbabilityPCM(probabilities, indicators, covar);
            var jpe = Probability.JointProbability(probabilities, indicators, mvn);
            var jpI = Probability.JointProbabilityIPCM(probabilities, indicators, covar);

        }

        [TestMethod]
        public void Test_JointProbability_PCM_3()
        {
            double A = Normal.StandardCDF(-2); //0.25;
            double B = Normal.StandardCDF(-2); //0.35;
            double C = Normal.StandardCDF(-2); //0.5;

            var mean = new double[] { 0, 0, 0 };
            var r = 0.9;
            var covar = new double[,]
            { { 1, r, r },
              { r, 1, r },
              { r, r, 1 }};
            var mvn = new MultivariateNormal(mean, covar) { MVNUNI = new MersenneTwister(12345) };

            var probabilities = new double[] { A, B, C };
            var indicators = new int[] { 1, 1, 1 };
            var jp = Probability.JointProbabilityPCM(probabilities, indicators, covar);
            var jpe = Probability.JointProbability(probabilities, indicators, mvn);
            var jpI = Probability.JointProbabilityIPCM(probabilities, indicators, covar);
            var jpH = Probability.JointProbabilityHPCM(probabilities, indicators, covar);
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
            var r = 0.8;
            var covar = new double[,]
            { { 1, r, r, r, r },
              { r, 1, r, r, r },
              { r, r, 1, r, r },
              { r, r, r, 1, r },
              { r, r, r, r, 1 }};
            var mvn = new MultivariateNormal(mean, covar) { MVNUNI = new MersenneTwister(12345) };

            var probabilities = new double[] { A, B, C, D, E };
            var indicators = new int[] { 1, 1, 1, 1, 1 };
            var jp = Probability.JointProbabilityPCM(probabilities, indicators, covar);
            var jpe = Probability.JointProbability(probabilities, indicators, mvn);
            var jpI = Probability.JointProbabilityIPCM(probabilities, indicators, covar);
            var jpH = Probability.JointProbabilityHPCM(probabilities, indicators, covar);
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
            var indicators = new int[] { 1, 1, 1, 1, 1 };
            var jp = Probability.JointProbabilityPCM(probabilities, indicators, covar);
            var jpe = Probability.JointProbability(probabilities, indicators, mvn);
            var jpI = Probability.JointProbabilityIPCM(probabilities, indicators, covar);

            var jps = Probability.JointProbabilitiesPCM(probabilities, Factorial.AllCombinations(5), covar);
            for (int i = 0; i < jps.Length; i++)
            {
                Debug.Print(jps[i].ToString());
            }

            var u = Probability.UnionPCM(probabilities, covar);
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

            var eps = Probability.ExclusivePCM(probabilities, combos, indicators, covar);
            var sumEps = Tools.Sum(eps);

            stopWatch.Stop();
            var timeSpan = stopWatch.Elapsed;



            stopWatch = new Stopwatch();
            stopWatch.Start();

            var probs = new List<double>();
            var inds = new List<int[]>();

             Probability.IndependentExclusive(probabilities, combos, indicators, out probs, out inds);

            // Probability.PositivelyDependentExclusive(probabilities, combos, indicators, out probs, out inds);

            Probability.ExclusivePCM(probabilities, combos, indicators, covar, out probs, out inds);

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
            var r = 0.0;
            var covar = new double[,]
            { { 1, r, r, r },
              { r, 1, r, r },
              { r, r, 1, r },
              { r, r, r, 1 }};
            var mvn = new MultivariateNormal(mean, covar) { MVNUNI = new MersenneTwister(12345) };

            var union = Probability.Union(new double[] { A, B, C, D }, mvn);
            var union2 = Probability.UnionPCM(new double[] { A, B, C, D }, covar);

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
            var union2 = Probability.UnionPCM(new double[] { A, B, C, D }, covar);
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
            var u = Tools.Sum(exclusive);
            var u2 = Probability.UnionPCM(new double[] { A, B, C }, covar);


            int N = 3;
            var combos = new int[N];
            for (int i = 1; i <= N; i++)
            {
                combos[i - 1] = (int)Factorial.BinomialCoefficient(N, i);
            }

            var indicators = Factorial.AllCombinations(N);

            var pOut = new List<double>();
            var iOut = new List<int[]>();

            var e = Probability.ExclusivePCM(new double[] { A, B, C }, covar);
            Probability.ExclusivePCM(new double[] { A, B, C }, combos, indicators, covar, out pOut, out iOut);

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
                var exclusive = Probability.ExclusivePCM(probabilities, combos, indicators, covar);
                //var exclusive = Probability.IndependentExclusive(probabilities, indicators);
            }

            stopWatch.Stop();
            var timeSpan = stopWatch.Elapsed;


        }

    }
}
