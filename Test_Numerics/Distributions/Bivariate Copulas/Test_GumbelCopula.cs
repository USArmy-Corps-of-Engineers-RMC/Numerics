using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Data.Statistics;
using Numerics.Distributions;
using Numerics.Distributions.Copulas;

namespace Distributions.BivariateCopulas
{
    [TestClass]
    public class Test_GumbelCopula
    {
        /// <summary>
        /// Test PDF
        /// </summary>
        [TestMethod]
        public void Test_PDF()
        {
            var copula = new GumbelCopula(10);
            Assert.AreEqual(copula.PDF(0.2, 0.8), 1.559981e-07, 1E-6);

            copula = new GumbelCopula(20);
            Assert.AreEqual(copula.PDF(0.2, 0.8), 7.954156e-16, 1E-6);

            copula = new GumbelCopula(1.1);
            Assert.AreEqual(copula.PDF(0.2, 0.8), 0.9004251, 1E-6);

            copula = new GumbelCopula(5);
            Assert.AreEqual(copula.PDF(0.2, 0.8), 0.001609717, 1E-6);

        }

        /// <summary>
        /// Test CDF
        /// </summary>
        [TestMethod]
        public void Test_CDF()
        {
            var copula = new GumbelCopula(10);
            Assert.AreEqual(copula.CDF(0.2, 0.8), 0.2, 1E-6);

            copula = new GumbelCopula(20);
            Assert.AreEqual(copula.CDF(0.2, 0.8), 0.2, 1E-6);

            copula = new GumbelCopula(1.1);
            Assert.AreEqual(copula.CDF(0.2, 0.8), 0.1694668, 1E-6);

            copula = new GumbelCopula(5);
            Assert.AreEqual(copula.CDF(0.2, 0.8), 0.1999967, 1E-6);

        }

        /// <summary>
        /// Test InverseCDF and random number generation.
        /// </summary>
        [TestMethod]
        public void Test_Generation()
        {
            var copula = new GumbelCopula(2);
            var samples = copula.GenerateRandomValues(1000, 12345);
            //for (int i = 0; i < samples.GetLength(0); i++)
            //    Debug.WriteLine(samples[i, 0] + "," + samples[i, 1]);

        }

        /// <summary>
        /// Estimate using the method of moments.
        /// </summary>
        [TestMethod]
        public void Test_MOM_Fit()
        {
            var spy = new double[] { 3.9057643, 1.8045245, 2.9633604, 2.7419766, 2.6774066, 3.1893778, 7.8361615, 9.8447683, 5.2786246, 4.1915502, 2.7293447, 2.8973314, 3.0865883, 3.7759493, 2.7534610, 1.5579397, 2.0069851, 2.2461076, 6.5123250, 3.6909549, 1.4625229, 2.5176823, 1.0641868, 2.2663723, 1.9026921, 2.4778238, 1.6084325, 1.2643252, 2.2505305, 2.1005891, 1.9748198, 1.9833167, 1.8059641, 2.0970398, 4.2106869, 2.3625876, 2.4940627, 3.5909080, 2.3934814, 1.2629584, 1.2839498, 1.7744064, 1.5590856, 0.5005273, 4.1822539, 2.2285852, 0.7467401, 3.2402393, 2.3862763, 2.5130236, 3.0073092, 1.7664685, 10.9423735, 5.7648950, 3.4414304, 3.4178928, 1.3613983 };
            var ixc = new double[] { 3.696937, 2.107435, 3.331595, 4.309303, 5.325759, 2.324532, 11.015879, 13.150865, 7.029388, 6.184589, 3.292565, 3.442535, 4.323848, 5.238649, 3.528347, 2.652233, 3.320179, 2.918689, 8.510062, 4.353267, 2.832925, 4.126286, 1.670764, 2.405936, 2.475370, 3.623923, 1.398245, 1.777251, 2.126659, 2.838736, 1.982282, 6.923452, 4.380535, 2.154451, 5.710447, 4.006626, 4.031362, 4.779409, 3.188225, 2.112282, 2.404566, 3.460109, 1.682543, 1.613359, 4.137543, 2.587800, 3.230225, 3.676028, 2.412084, 2.774390, 3.501687, 3.520000, 19.451697, 8.943085, 3.960401, 5.517874, 3.672079 };

            var copula = new GumbelCopula();
            copula.SetThetaFromTau( spy, ixc );

            Assert.AreEqual(copula.Theta, 2.509434, 1E-4);

        }

        /// <summary>
        /// Estimate using the method of maximum pseudo likelihood.
        /// </summary>
        [TestMethod]
        public void Test_MPL_Fit()
        {
            var spy = new double[] { 3.9057643, 1.8045245, 2.9633604, 2.7419766, 2.6774066, 3.1893778, 7.8361615, 9.8447683, 5.2786246, 4.1915502, 2.7293447, 2.8973314, 3.0865883, 3.7759493, 2.7534610, 1.5579397, 2.0069851, 2.2461076, 6.5123250, 3.6909549, 1.4625229, 2.5176823, 1.0641868, 2.2663723, 1.9026921, 2.4778238, 1.6084325, 1.2643252, 2.2505305, 2.1005891, 1.9748198, 1.9833167, 1.8059641, 2.0970398, 4.2106869, 2.3625876, 2.4940627, 3.5909080, 2.3934814, 1.2629584, 1.2839498, 1.7744064, 1.5590856, 0.5005273, 4.1822539, 2.2285852, 0.7467401, 3.2402393, 2.3862763, 2.5130236, 3.0073092, 1.7664685, 10.9423735, 5.7648950, 3.4414304, 3.4178928, 1.3613983 };
            var ixc = new double[] { 3.696937, 2.107435, 3.331595, 4.309303, 5.325759, 2.324532, 11.015879, 13.150865, 7.029388, 6.184589, 3.292565, 3.442535, 4.323848, 5.238649, 3.528347, 2.652233, 3.320179, 2.918689, 8.510062, 4.353267, 2.832925, 4.126286, 1.670764, 2.405936, 2.475370, 3.623923, 1.398245, 1.777251, 2.126659, 2.838736, 1.982282, 6.923452, 4.380535, 2.154451, 5.710447, 4.006626, 4.031362, 4.779409, 3.188225, 2.112282, 2.404566, 3.460109, 1.682543, 1.613359, 4.137543, 2.587800, 3.230225, 3.676028, 2.412084, 2.774390, 3.501687, 3.520000, 19.451697, 8.943085, 3.960401, 5.517874, 3.672079 };

            // get ranks of data
            var rank1 = Statistics.RanksInplace(spy);
            var rank2 = Statistics.RanksInplace(ixc);
            // get plotting positions
            for (int i = 0; i < spy.Length; i++)
            {
                rank1[i] = rank1[i] / (rank1.Length + 1d);
                rank2[i] = rank2[i] / (rank2.Length + 1d);
            }

            BivariateCopula copula = new GumbelCopula();
            BivariateCopulaEstimation.Estimate(ref copula, rank1, rank2 , CopulaEstimationMethod.PseudoLikelihood);

            Assert.AreEqual(copula.Theta, 2.761169, 1E-4);

        }

        /// <summary>
        /// Estimate using the inference from margins method.
        /// </summary>
        [TestMethod]
        public void Test_IFM_Fit()
        {
            var spy = new double[] { 3.9057643, 1.8045245, 2.9633604, 2.7419766, 2.6774066, 3.1893778, 7.8361615, 9.8447683, 5.2786246, 4.1915502, 2.7293447, 2.8973314, 3.0865883, 3.7759493, 2.7534610, 1.5579397, 2.0069851, 2.2461076, 6.5123250, 3.6909549, 1.4625229, 2.5176823, 1.0641868, 2.2663723, 1.9026921, 2.4778238, 1.6084325, 1.2643252, 2.2505305, 2.1005891, 1.9748198, 1.9833167, 1.8059641, 2.0970398, 4.2106869, 2.3625876, 2.4940627, 3.5909080, 2.3934814, 1.2629584, 1.2839498, 1.7744064, 1.5590856, 0.5005273, 4.1822539, 2.2285852, 0.7467401, 3.2402393, 2.3862763, 2.5130236, 3.0073092, 1.7664685, 10.9423735, 5.7648950, 3.4414304, 3.4178928, 1.3613983 };
            var ixc = new double[] { 3.696937, 2.107435, 3.331595, 4.309303, 5.325759, 2.324532, 11.015879, 13.150865, 7.029388, 6.184589, 3.292565, 3.442535, 4.323848, 5.238649, 3.528347, 2.652233, 3.320179, 2.918689, 8.510062, 4.353267, 2.832925, 4.126286, 1.670764, 2.405936, 2.475370, 3.623923, 1.398245, 1.777251, 2.126659, 2.838736, 1.982282, 6.923452, 4.380535, 2.154451, 5.710447, 4.006626, 4.031362, 4.779409, 3.188225, 2.112282, 2.404566, 3.460109, 1.682543, 1.613359, 4.137543, 2.587800, 3.230225, 3.676028, 2.412084, 2.774390, 3.501687, 3.520000, 19.451697, 8.943085, 3.960401, 5.517874, 3.672079 };

            // get ranks of data
            var rank1 = Statistics.RanksInplace(spy);
            var rank2 = Statistics.RanksInplace(ixc);
            // get plotting positions
            for (int i = 0; i < spy.Length; i++)
            {
                rank1[i] = rank1[i] / (rank1.Length + 1d);
                rank2[i] = rank2[i] / (rank2.Length + 1d);
            }

            BivariateCopula copula = new GumbelCopula();
            copula.MarginalDistributionX = new Uniform(0, 1);
            copula.MarginalDistributionY = new Uniform(0, 1);

            BivariateCopulaEstimation.Estimate(ref copula,  rank1, rank2 , CopulaEstimationMethod.InferenceFromMargins);

            Assert.AreEqual(copula.Theta, 2.761169, 1E-4);

        }

        /// <summary>
        /// Estimate using the method of maximum likelihood estimation.
        /// </summary>
        [TestMethod]
        public void Test_MLE_Fit()
        {
            var spy = new double[] { 3.9057643, 1.8045245, 2.9633604, 2.7419766, 2.6774066, 3.1893778, 7.8361615, 9.8447683, 5.2786246, 4.1915502, 2.7293447, 2.8973314, 3.0865883, 3.7759493, 2.7534610, 1.5579397, 2.0069851, 2.2461076, 6.5123250, 3.6909549, 1.4625229, 2.5176823, 1.0641868, 2.2663723, 1.9026921, 2.4778238, 1.6084325, 1.2643252, 2.2505305, 2.1005891, 1.9748198, 1.9833167, 1.8059641, 2.0970398, 4.2106869, 2.3625876, 2.4940627, 3.5909080, 2.3934814, 1.2629584, 1.2839498, 1.7744064, 1.5590856, 0.5005273, 4.1822539, 2.2285852, 0.7467401, 3.2402393, 2.3862763, 2.5130236, 3.0073092, 1.7664685, 10.9423735, 5.7648950, 3.4414304, 3.4178928, 1.3613983 };
            var ixc = new double[] { 3.696937, 2.107435, 3.331595, 4.309303, 5.325759, 2.324532, 11.015879, 13.150865, 7.029388, 6.184589, 3.292565, 3.442535, 4.323848, 5.238649, 3.528347, 2.652233, 3.320179, 2.918689, 8.510062, 4.353267, 2.832925, 4.126286, 1.670764, 2.405936, 2.475370, 3.623923, 1.398245, 1.777251, 2.126659, 2.838736, 1.982282, 6.923452, 4.380535, 2.154451, 5.710447, 4.006626, 4.031362, 4.779409, 3.188225, 2.112282, 2.404566, 3.460109, 1.682543, 1.613359, 4.137543, 2.587800, 3.230225, 3.676028, 2.412084, 2.774390, 3.501687, 3.520000, 19.451697, 8.943085, 3.960401, 5.517874, 3.672079 };

            BivariateCopula copula = new GumbelCopula();
            copula.MarginalDistributionX = new GeneralizedExtremeValue();
            copula.MarginalDistributionY = new GeneralizedExtremeValue();

            BivariateCopulaEstimation.Estimate(ref copula, spy, ixc , CopulaEstimationMethod.FullLikelihood);

            var true_m1 = new GeneralizedExtremeValue();
            true_m1.Estimate(spy, ParameterEstimationMethod.MaximumLikelihood);

            var true_m2 = new GeneralizedExtremeValue();
            true_m2.Estimate(ixc, ParameterEstimationMethod.MaximumLikelihood);

        }


    }
}
