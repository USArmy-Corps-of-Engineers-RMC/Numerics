using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics;
using Numerics.Data;
using Numerics.Distributions;
using Numerics.Sampling;

namespace Mathematics.Integration
{
    /// <summary>
    /// Test integration methods.
    /// </summary>
    [TestClass()]
    public class Test_Integration
    {

        #region Univariate

        /// <summary>
        /// Test function. The integral of x^3, should equal 0.25
        /// </summary>
        public double FX3(double x)
        {
            return Math.Pow(x, 3d);
        }

        /// <summary>
        /// Test function. The integral of Cos(x), should equal ~ 1.6829419
        /// </summary>
        public double Cosine(double x)
        {
            return Math.Cos(x);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        public double Sine(double x)
        {
            return Math.Sin(x);
        }

        public double FXX(double x)
        {
            return 0.5 + 24 * x + 3 * x * x;
        }

        public double FXXX(double x)
        {
            return 0.5 + 24 * x + 3 * x * x + 8 * x * x * x;
        }

        /// <summary>
        /// Gauss-Legendre.
        /// </summary>
        [TestMethod()]
        public void Test_GaussLegendre()
        {
            double e = Numerics.Mathematics.Integration.Integration.GaussLegendre(FX3, 0d, 1d);
            double val = 0.25d;
            Assert.AreEqual(e, val, 0.01);

            e = Numerics.Mathematics.Integration.Integration.GaussLegendre(Cosine, -1, 1);
            val = 1.6829419d;
            Assert.AreEqual(e, val, 0.01);

        }

        /// <summary>
        /// Trapezoidal Rule.
        /// </summary>
        [TestMethod()]
        public void Test_TrapezoidalRule()
        {
            var tr = new Numerics.Mathematics.Integration.TrapezoidalRule(FX3, 0d, 1d);
            tr.Integrate();
            double e = tr.Result;
            double val = 0.25d;
            Assert.AreEqual(e, val, 0.01);

            tr = new Numerics.Mathematics.Integration.TrapezoidalRule(Cosine, -1, 1);
            tr.Integrate();
            e = tr.Result;
            val = 1.6829419d;
            Assert.AreEqual(e, val, 0.01);

        }

        /// <summary>
        /// Simpson's Rule.
        /// </summary>
        [TestMethod()]
        public void Test_SimpsonsRule()
        {
            var sr = new Numerics.Mathematics.Integration.SimpsonsRule(FX3, 0d, 1d);
            sr.Integrate();
            double e = sr.Result;
            double val = 0.25d;
            Assert.AreEqual(val, e, 1E-6);

            sr = new Numerics.Mathematics.Integration.SimpsonsRule(Cosine, -1, 1);
            sr.Integrate();
            e = sr.Result;
            val = 1.6829419d;
            Assert.AreEqual(val, e, 1E-6);

            sr = new Numerics.Mathematics.Integration.SimpsonsRule(FXX, 0, 2);
            sr.Integrate();
            e = sr.Result;
            val = 57;
            Assert.AreEqual(val, e, 1E-6);

            sr = new Numerics.Mathematics.Integration.SimpsonsRule(FXXX, 0, 2);
            sr.Integrate();
            e = sr.Result;
            val = 89;
            Assert.AreEqual(val, e, 1E-6);

            var gamma = new GammaDistribution(10, 5);
            sr = new Numerics.Mathematics.Integration.SimpsonsRule(x => x * gamma.PDF(x), gamma.InverseCDF(1E-16), gamma.InverseCDF(1 - 1E-16));
            sr.Integrate();
            e = sr.Result;
            val = 50;
            Assert.AreEqual(val, e, 1E-6);

            var ln = new LnNormal(10, 2);
            double alpha = 0.99;
            sr = new Numerics.Mathematics.Integration.SimpsonsRule(x => x * ln.PDF(x), ln.InverseCDF(alpha), ln.InverseCDF(1 - 1E-16));
            sr.Integrate();
            e = sr.Result / (1 - alpha);
            val = Math.Exp(ln.Mu + 0.5 * ln.Sigma * ln.Sigma) / (1d - alpha) * (1 - Normal.StandardCDF(Normal.StandardZ(alpha) - ln.Sigma));

        }

        /// <summary>
        /// Midpoint Method. 
        /// </summary>
        [TestMethod()]
        public void Test_MidPoint()
        {
            double e = Numerics.Mathematics.Integration.Integration.Midpoint(FX3, 0d, 1d, 1000);
            double val = 0.25d;
            Assert.AreEqual(e, val, 0.01);

        }

        /// <summary>
        /// Adaptive Simpson's Rule.
        /// </summary>
        [TestMethod()]
        public void Test_AdaptiveSimpsons()
        {
            var adapt = new Numerics.Mathematics.Integration.AdaptiveSimpsonsRule(FX3, 0d, 1d);
            adapt.Integrate();
            double e = adapt.Result;
            double val = 0.25d;
            Assert.AreEqual(e, val, 0.01);

            adapt = new Numerics.Mathematics.Integration.AdaptiveSimpsonsRule(Cosine, -1, 1);
            adapt.Integrate();
            e = adapt.Result;
            val = 1.6829419d;
            Assert.AreEqual(e, val, 0.01);

            adapt = new Numerics.Mathematics.Integration.AdaptiveSimpsonsRule(Sine, 0, 1);
            adapt.Integrate();
            e = adapt.Result;
            val = 0.459697694131d;
            Assert.AreEqual(e, val, 0.01);

            adapt = new Numerics.Mathematics.Integration.AdaptiveSimpsonsRule(FXX, 0, 2);
            adapt.Integrate();
            e = adapt.Result;
            val = 57;
            Assert.AreEqual(val, e, 1E-6);

            adapt = new Numerics.Mathematics.Integration.AdaptiveSimpsonsRule(FXXX, 0, 2);
            adapt.Integrate();
            e = adapt.Result;
            val = 89;
            Assert.AreEqual(val, e, 1E-6);

            var gamma = new GammaDistribution(10, 5);
            adapt = new Numerics.Mathematics.Integration.AdaptiveSimpsonsRule(x => x * gamma.PDF(x), gamma.InverseCDF(1E-16), gamma.InverseCDF(1 - 1E-16));
            adapt.Integrate();
            e = adapt.Result;
            val = 50;
            Assert.AreEqual(val, e, 1E-6);

            var ln = new LnNormal(10, 2);
            double alpha = 0.99;
            adapt = new Numerics.Mathematics.Integration.AdaptiveSimpsonsRule(x => x * ln.PDF(x), ln.InverseCDF(alpha), ln.InverseCDF(1 - 1E-16));
            adapt.Integrate();
            e = adapt.Result / (1 - alpha);
            val = Math.Exp(ln.Mu + 0.5 * ln.Sigma * ln.Sigma) / (1d - alpha) * (1 - Normal.StandardCDF(Normal.StandardZ(alpha) - ln.Sigma));

        }

        /// <summary>
        /// Adaptive Gauss-Lobatto.
        /// </summary>
        [TestMethod()]
        public void Test_AdaptiveGaussLobatto()
        {
            var adapt = new Numerics.Mathematics.Integration.AdaptiveGaussLobatto(FX3, 0, 1);
            adapt.Integrate();
            double e = adapt.Result;
            double val = 0.25d;
            Assert.AreEqual(e, val, 0.01);

            adapt = new Numerics.Mathematics.Integration.AdaptiveGaussLobatto(Cosine, -1, 1);
            adapt.Integrate();
            e = adapt.Result;
            val = 1.6829419d;
            Assert.AreEqual(e, val, 0.01);

            adapt = new Numerics.Mathematics.Integration.AdaptiveGaussLobatto(Sine, 0, 1);
            adapt.Integrate();
            e = adapt.Result;
            val = 0.459697694131d;
            Assert.AreEqual(e, val, 0.01);

            adapt = new Numerics.Mathematics.Integration.AdaptiveGaussLobatto(FXX, 0, 2);
            adapt.Integrate();
            e = adapt.Result;
            val = 57;
            Assert.AreEqual(val, e, 1E-6);

            adapt = new Numerics.Mathematics.Integration.AdaptiveGaussLobatto(FXXX, 0, 2);
            adapt.Integrate();
            e = adapt.Result;
            val = 89;
            Assert.AreEqual(val, e, 1E-6);

            var gamma = new GammaDistribution(10, 5);
            adapt = new Numerics.Mathematics.Integration.AdaptiveGaussLobatto(x => x * gamma.PDF(x), gamma.InverseCDF(1E-16), gamma.InverseCDF(1 - 1E-16));
            adapt.Integrate();
            e = adapt.Result;
            val = 50;
            Assert.AreEqual(val, e, 1E-6);

            var ln = new LnNormal(10, 2);
            double alpha = 0.99;
            adapt = new Numerics.Mathematics.Integration.AdaptiveGaussLobatto(x => x * ln.PDF(x), ln.InverseCDF(alpha), ln.InverseCDF(1 - 1E-16));
            adapt.Integrate();
            e = adapt.Result / (1 - alpha);
            val = Math.Exp(ln.Mu + 0.5 * ln.Sigma * ln.Sigma) / (1d - alpha) * (1 - Normal.StandardCDF(Normal.StandardZ(alpha) - ln.Sigma));

        }

        #endregion

        #region Multivariate

        /// <summary>
        /// Test function. The integral of Pi. Should equal ~3.14
        /// </summary>
        /// <param name="vals"></param>
        /// <returns></returns>
        public double PI(double[] vals)
        {
            var x = vals[0];
            var y = vals[1];
            return (x * x + y * y < 1) ? 1 : 0;
        }

        /// <summary>
        /// Test function from GNU Scientific Library, GSL. Result should equal 1.3932039296856768591842462603255
        /// </summary>
        public double GSL(double[] x)
        {
            double A = 1.0 / (Math.PI * Math.PI * Math.PI);
            return A / (1.0 - Math.Cos(x[0]) * Math.Cos(x[1]) * Math.Cos(x[2]));
        }


        private static double[] mu = new double[] { 10, 30, 17 };
        private static double[] sigma = new double[] { 2, 15, 5 };
        private static Normal[] dists = new Normal[] { new Normal(mu[0], sigma[0]), new Normal(mu[1], sigma[1]), new Normal(mu[2], sigma[2]) };

        private static double[] mu20 = new double[] { 10, 30, 17, 99, 68, 26, 35, 55, 13, 59, 12, 28, 49, 54, 20, 47, 12, 76, 70, 57 };
        private static double[] sigma20 = new double[] { 2, 15, 5, 14, 7, 24, 29, 22, 22, 1, 3, 28, 19, 18, 4, 24, 23, 26, 26, 19 };


        /// <summary>
        /// Test function. The mean of one independent normal distributions. Should equal 10.
        /// </summary>
        public double MeanOneNormal(double[] x, double w)
        {
            double result = 0;
            //double sum = 0;
            //double prod = 1;
            //for (int i = 0; i < 1; i++)
            //{
            //    //sum += x[i];
            //    //prod *= Normal.StandardPDF(x[i]);

            //    result += mu20[i] + sigma20[i] * Normal.StandardZ(x[i]);
            //    //result += x[i] * Normal.StandardPDF((mu20[i] - x[i]) / sigma20[i]);
            //}
            // result = sum * prod;

            if (x[0] >= 0.99)
            {
                result = mu20[0] + sigma20[0] * Normal.StandardZ(x[0]);            
            }
            

            //Debug.WriteLine(w.ToString());

            return result;
        }

        /// <summary>
        /// Test function. The sum of two independent normal distributions. Should equal 40.
        /// </summary>
        public double SumTwoNormals(double[] x)
        {
            double result = 0;
            double sum = 0;
            double prod = 1;
            for (int i = 0; i < 2; i++)
            {
                sum += x[i];
                prod *= dists[i].PDF(x[i]);
            }
            result = sum * prod;
            return result;
        }

        /// <summary>
        /// Test function. The sum of three independent normal distributions. Should equal 57.
        /// </summary>
        public double SumThreeNormals(double[] x)
        {
            double result = 0;
            double sum = 0;
            double prod = 1;
            for (int i = 0; i < 3; i++)
            {
                var norm = new Normal(mu20[i], sigma20[i]);
                sum += x[i];
                prod *= norm.PDF(x[i]);
            }
            result = sum * prod;
            return result;
        }




        public double SumFiveNormals(double[] p)
        {
            double result = 0;
            for (int i = 0; i < 5; i++)
            {
                result += mu20[i] + sigma20[i] * Normal.StandardZ(p[i]);
            }
            return result;
        }

        public double SumTwentyNormals(double[] p)
        {
            double result = 0;
            for (int i = 0; i < 20; i++)
            {
                //result += mu20[i] + sigma20[i] * Normal.StandardZ(p[i]);
                result += mu20[i] + sigma20[i] * p[i] ;
            }
            return result;
        }


        public double SumOfTwentyNormals(double[] x)
        {
            var z = new double[20];
            var mean = new double[20];
            var covar = new double[20, 20];
            double rho = 0;

            double sum = 0;
            double prod = 1;
            double prodZ = 1;

            for (int i = 0; i < 20; i++)
            {
                var norm = new Normal(mu20[i], sigma20[i]);
                sum += x[i];
                prod *= norm.PDF(x[i]);



                //z[i] = Normal.StandardZ(norm.CDF(x[i]));
                //prodZ *= Normal.StandardPDF(z[i]);

                //mean[i] = 0;
                //for(int j = 0; j < 20; j++)
                //{
                //   if (j == i)
                //   {
                //        covar[i, j] = 1;
                //   }
                //   else
                //   {
                //        covar[i, j] = rho;
                //    }
                //}
            }
            //var mvn = new MultivariateNormal(mean, covar);

            //double result = sum * mvn.PDF(z) / prodZ * prod;

            double result = sum * prod;
            return result;
        }


        private static double[] mean = new double[] { 0, 0.5, 1 };
        private static double[,] cov = new double[,] { { 1, 0.8, 0.5 }, { 0.8, 1, 0.8 }, { 0.5, 0.8, 1 } };
        private static MultivariateNormal mvn = new MultivariateNormal(mean, cov);

        /// <summary>
        /// Monte Carlo Integration.
        /// </summary>
        [TestMethod()]
        public void Test_MonteCarloIntegration()
        {
            var mc = new Numerics.Mathematics.Integration.MonteCarloIntegration(PI, 2, new double[] { -1, -1 }, new double[] { 1, 1 });
            mc.MinIterations = 1000;
            mc.MaxIterations = 1000000;
            mc.RelativeTolerance = 0.001;
            mc.Integrate();
            var e = mc.Result;
            double val = 3.14;

            mc = new Numerics.Mathematics.Integration.MonteCarloIntegration(GSL, 3, new double[] { 0, 0, 0 }, new double[] { Math.PI, Math.PI, Math.PI });
            mc.MinIterations = 1000;
            mc.MaxIterations = 1000000;
            mc.RelativeTolerance = 0.001;
            mc.Integrate();
            e = mc.Result;
            val = 1.3932039296856768591842462603255;


            mc = new Numerics.Mathematics.Integration.MonteCarloIntegration(SumThreeNormals, 3, new double[] { dists[0].InverseCDF(1E-16), dists[1].InverseCDF(1E-16), dists[2].InverseCDF(1E-16) },
                                                                                            new double[] { dists[0].InverseCDF(1 - 1E-16), dists[1].InverseCDF(1 - 1E-16), dists[2].InverseCDF(1 - 1E-16) });
            mc.MinIterations = 1000;
            mc.MaxIterations = 1000000;
            mc.Integrate();
            e = mc.Result;
            val = 57;

        }

        /// <summary>
        /// Miser - recursive stratified sampling.
        /// </summary>
        [TestMethod()]
        public void Test_Miser()
        {
            var mis = new Numerics.Mathematics.Integration.Miser(PI, 2, new double[] { -1, -1 }, new double[] { 1, 1 });
            mis.Integrate();
            var e = mis.Result;
            double val = 3.14;


            mis = new Numerics.Mathematics.Integration.Miser(GSL, 3, new double[] { 0, 0, 0 }, new double[] { Math.PI, Math.PI, Math.PI });
            mis.Integrate();
            e = mis.Result;
            val = 1.3932039296856768591842462603255;

            mis = new Numerics.Mathematics.Integration.Miser(SumThreeNormals, 3, new double[] { dists[0].InverseCDF(1E-16), dists[1].InverseCDF(1E-16), dists[2].InverseCDF(1E-16) },
                                                                                 new double[] { dists[0].InverseCDF(1 - 1E-16), dists[1].InverseCDF(1 - 1E-16), dists[2].InverseCDF(1 - 1E-16) });
            mis.Integrate();
            e = mis.Result;
            val = 57;

            var min = new double[20];
            var max = new double[20];
            for (int i = 0; i < 20; i++)
            {
                min[i] = 1E-16;
                max[i] = 1 - 1E-16;
            }

            mis = new Numerics.Mathematics.Integration.Miser((x) => { return SumTwentyNormals(x); }, 20, min, max);
            mis.Random = new MersenneTwister(12345);
            mis.UseSobolSequence = false;
            mis.MaxFunctionEvaluations = 110000;
            mis.Integrate();
            e = mis.Result;
            val = 837;
            Assert.AreEqual(val, e, val * 0.01);

        }

        [TestMethod()]
        public void Test_Vegas_1()
        {

            var min = new double[1];
            var max = new double[1];
            for (int i = 0; i < 1; i++)
            {
                //var norm = new Normal(mu20[i], sigma20[i]);
                //min[i] = norm.InverseCDF(1E-16);
                //max[i] = norm.InverseCDF(1 - 1E-16);

                min[i] = 1E-16;
                max[i] = 1 - 1E-16;
            }

            var v = new Numerics.Mathematics.Integration.Vegas((x, y) => { return MeanOneNormal(x, y); }, 1, min, max);
            // Warmup
            v.FunctionCalls = 1000;
            v.IndependentEvaluations = 10;
            v.Initialize = 0;
            //v.IsProbabilityDomain = true;
            v.Integrate();
            // Final
            v.FunctionCalls = 10000;
            v.IndependentEvaluations = 1;
            v.Initialize = 1;
            v.Integrate();
            var e = v.Result;
            double val = 837;
            Assert.AreEqual(val, e, val * 0.01);

        }


        /// <summary>
        /// Vegas - Adaptive Monte Carlo integration.
        /// </summary>
        [TestMethod()]
        public void Test_Vegas()
        {
            var v = new Numerics.Mathematics.Integration.Vegas((x, y) => { return PI(x); }, 2, new double[] { -1, -1 }, new double[] { 1, 1 });
            v.Random = new MersenneTwister(12345);
            // Warmup
            v.FunctionCalls = 2000;
            v.IndependentEvaluations = 5;
            v.Initialize = 0;
            v.Integrate();
            // Final
            v.FunctionCalls = 10000;
            v.IndependentEvaluations = 1;
            v.Initialize = 1;
            v.Integrate();
            var e = v.Result;
            double val = Math.PI;
            Assert.AreEqual(val, e, val * 0.01);

            v = new Numerics.Mathematics.Integration.Vegas((x, y) => { return GSL(x); }, 3, new double[] { 0, 0, 0 }, new double[] { Math.PI, Math.PI, Math.PI });
            v.Random = new MersenneTwister(12345);
            // Warmup
            v.FunctionCalls = 3000;
            v.IndependentEvaluations = 5;
            v.Initialize = 0;
            v.Integrate();
            // Final
            v.FunctionCalls = 10000;
            v.IndependentEvaluations = 1;
            v.Initialize = 1;
            v.Integrate();
            e = v.Result;
            val = 1.3932039296856768591842462603255;
            Assert.AreEqual(val, e, val * 0.01);




            //v = new Numerics.Mathematics.Integration.Vegas((x, y) => { return SumTwoNormals(x); }, 2, new double[] { dists[0].InverseCDF(1E-16), dists[1].InverseCDF(1E-16) },
            //                                                       new double[] { dists[0].InverseCDF(1 - 1E-16), dists[1].InverseCDF(1 - 1E-16) });
            //v.Random = new MersenneTwister(12345);
            //// Warmup
            //v.FunctionCalls = 1000;
            //v.IndependentEvaluations = 10;
            //v.Initialize = 1;
            //v.Integrate();

            //// Final
            //v.FunctionCalls = 10000;
            //v.IndependentEvaluations = 1;
            //v.Initialize = 1;
            //v.Integrate();
            //e = v.Result;
            //val = 40;
            //Assert.AreEqual(val, e, val * 0.01);




            v = new Numerics.Mathematics.Integration.Vegas((x, y) => { return SumThreeNormals(x); }, 3, new double[] { dists[0].InverseCDF(1E-16), dists[1].InverseCDF(1E-16), dists[2].InverseCDF(1E-16) },
                                                                               new double[] { dists[0].InverseCDF(1 - 1E-16), dists[1].InverseCDF(1 - 1E-16), dists[2].InverseCDF(1 - 1E-16) });
            v.Random = new MersenneTwister(12345);
            // Warmup
            v.FunctionCalls = 3000;
            v.IndependentEvaluations = 5;
            v.Initialize = 0;
            v.Integrate();
            // Final
            v.FunctionCalls = 10000;
            v.IndependentEvaluations = 1;
            v.Initialize = 1;
            v.Integrate();
            e = v.Result;
            val = 57;
            Assert.AreEqual(val, e, val * 0.01);


            var min = new double[20];
            var max = new double[20];
            for (int i = 0; i < 20; i++)
            {
                min[i] = 1E-16;
                max[i] = 1 - 1E-16;
            }

            v = new Numerics.Mathematics.Integration.Vegas((x, y) => { return SumTwentyNormals(x); }, 20, min, max);
            v.Random = new MersenneTwister(12345);
            v.UseSobolSequence = true;
            // Warmup
            v.FunctionCalls = 20000;
            v.IndependentEvaluations = 5;
            v.Initialize = 0;
            v.Integrate();
            // Final
            v.FunctionCalls = 10000;
            v.IndependentEvaluations = 1;
            v.Initialize = 1;
            v.Integrate();
            e = v.Result;
            val = 837;
            Assert.AreEqual(val, e, val * 0.01);


            for (int i = 0; i < 20; i++)
            {
                var norm = new Normal(mu20[i], sigma20[i]);
                min[i] = norm.InverseCDF(1E-16);
                max[i] = norm.InverseCDF(1 - 1E-16);
            }

            v = new Numerics.Mathematics.Integration.Vegas((x, y) => { return SumOfTwentyNormals(x); }, 20, min, max);
            v.Random = new MersenneTwister(12345);
            v.UseSobolSequence = true;
            // Warmup
            v.FunctionCalls = 1000;
            v.IndependentEvaluations = 10;
            v.Initialize = 0;
            v.Integrate();
            // Final
            v.FunctionCalls = 10000;
            v.IndependentEvaluations = 1;
            v.Initialize = 1;
            v.Integrate();
            e = v.Result;
            val = 837;
            Assert.AreEqual(val, e, val * 0.01);

        }

       

        #endregion

    }
}