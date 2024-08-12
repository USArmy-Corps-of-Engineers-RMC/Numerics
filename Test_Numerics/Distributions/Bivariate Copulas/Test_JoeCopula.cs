/*
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
*/

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
    public class Test_JoeCopula
    {

        /// <summary>
        /// Test PDF
        /// </summary>
        [TestMethod]
        public void Test_PDF()
        {
            var copula = new JoeCopula(10);
            Assert.AreEqual(copula.PDF(0.2, 0.8), 4.342727e-05, 1E-6);

            copula = new JoeCopula(20);
            Assert.AreEqual(copula.PDF(0.2, 0.8), 8.645443e-11, 1E-6);

            copula = new JoeCopula(1.1);
            Assert.AreEqual(copula.PDF(0.2, 0.8), 0.9512265, 1E-6);

            copula = new JoeCopula(5);
            Assert.AreEqual(copula.PDF(0.2, 0.8), 0.02110735, 1E-6);

        }

        /// <summary>
        /// Test CDF
        /// </summary>
        [TestMethod]
        public void Test_CDF()
        {
            var copula = new JoeCopula(10);
            Assert.AreEqual(copula.CDF(0.2, 0.8), 0.1999999, 1E-6);

            copula = new JoeCopula(20);
            Assert.AreEqual(copula.CDF(0.2, 0.8), 0.2, 1E-6);

            copula = new JoeCopula(1.1);
            Assert.AreEqual(copula.CDF(0.2, 0.8), 0.1656223, 1E-6);

            copula = new JoeCopula(5);
            Assert.AreEqual(copula.CDF(0.2, 0.8), 0.199895, 1E-6);

        }

        /// <summary>
        /// Test InverseCDF and random number generation.
        /// </summary>
        [TestMethod]
        public void Test_Generation()
        {
            //var copula = new JoeCopula(20);
            //var samples = copula.GenerateRandomValues(1000, 12345);
            //for (int i = 0; i < samples.GetLength(0); i++)
            //    Debug.WriteLine(samples[i, 0] + "," + samples[i, 1]);

            //var copula = new JoeCopula(3);
            //copula.MarginalDistributionX = new Normal(100, 15);
            //copula.MarginalDistributionY = new Normal(80, 25);

            //var samples = copula.GenerateRandomValues(100);
            //for (int i = 0; i < samples.GetLength(0); i++)
            //    Debug.WriteLine(samples[i, 0] + "," + samples[i, 1]);

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

            BivariateCopula copula = new JoeCopula();
            BivariateCopulaEstimation.Estimate(ref copula, rank1, rank2, CopulaEstimationMethod.PseudoLikelihood);

            Assert.AreEqual(copula.Theta, 3.9765, 1E-4);

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

            BivariateCopula copula = new JoeCopula();
            copula.MarginalDistributionX = new Uniform(0, 1);
            copula.MarginalDistributionY = new Uniform(0, 1);

            BivariateCopulaEstimation.Estimate(ref copula, rank1, rank2, CopulaEstimationMethod.InferenceFromMargins);

            Assert.AreEqual(copula.Theta, 3.9765, 1E-4);

        }

        /// <summary>
        /// Estimate using the method of maximum likelihood estimation.
        /// </summary>
        [TestMethod]
        public void Test_MLE_Fit()
        {
            var spy = new double[] { 3.9057643, 1.8045245, 2.9633604, 2.7419766, 2.6774066, 3.1893778, 7.8361615, 9.8447683, 5.2786246, 4.1915502, 2.7293447, 2.8973314, 3.0865883, 3.7759493, 2.7534610, 1.5579397, 2.0069851, 2.2461076, 6.5123250, 3.6909549, 1.4625229, 2.5176823, 1.0641868, 2.2663723, 1.9026921, 2.4778238, 1.6084325, 1.2643252, 2.2505305, 2.1005891, 1.9748198, 1.9833167, 1.8059641, 2.0970398, 4.2106869, 2.3625876, 2.4940627, 3.5909080, 2.3934814, 1.2629584, 1.2839498, 1.7744064, 1.5590856, 0.5005273, 4.1822539, 2.2285852, 0.7467401, 3.2402393, 2.3862763, 2.5130236, 3.0073092, 1.7664685, 10.9423735, 5.7648950, 3.4414304, 3.4178928, 1.3613983 };
            var ixc = new double[] { 3.696937, 2.107435, 3.331595, 4.309303, 5.325759, 2.324532, 11.015879, 13.150865, 7.029388, 6.184589, 3.292565, 3.442535, 4.323848, 5.238649, 3.528347, 2.652233, 3.320179, 2.918689, 8.510062, 4.353267, 2.832925, 4.126286, 1.670764, 2.405936, 2.475370, 3.623923, 1.398245, 1.777251, 2.126659, 2.838736, 1.982282, 6.923452, 4.380535, 2.154451, 5.710447, 4.006626, 4.031362, 4.779409, 3.188225, 2.112282, 2.404566, 3.460109, 1.682543, 1.613359, 4.137543, 2.587800, 3.230225, 3.676028, 2.412084, 2.774390, 3.501687, 3.520000, 19.451697, 8.943085, 3.960401, 5.517874, 3.672079 };

            BivariateCopula copula = new JoeCopula();
            copula.MarginalDistributionX = new GeneralizedExtremeValue();
            copula.MarginalDistributionY = new GeneralizedExtremeValue();

            BivariateCopulaEstimation.Estimate(ref copula,  spy, ixc , CopulaEstimationMethod.FullLikelihood);

            var true_m1 = new GeneralizedExtremeValue();
            true_m1.Estimate(spy, ParameterEstimationMethod.MaximumLikelihood);

            var true_m2 = new GeneralizedExtremeValue();
            true_m2.Estimate(ixc, ParameterEstimationMethod.MaximumLikelihood);

        }

    }
}
