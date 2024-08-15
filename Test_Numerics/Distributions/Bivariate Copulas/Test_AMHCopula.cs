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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics;
using Numerics.Data.Statistics;
using Numerics.Distributions;
using Numerics.Distributions.Copulas;

namespace Distributions.BivariateCopulas
{
    /// <summary>
    /// Tested against the R 'copula' package. 
    /// </summary>
    [TestClass]
    public class Test_AMHCopula
    {

        /// <summary>
        /// Test PDF
        /// </summary>
        [TestMethod]
        public void Test_PDF()
        {
            var copula = new AMHCopula(0.8);
            Assert.AreEqual(copula.PDF(0.2, 0.8), 0.6491167, 1E-6);

            copula = new AMHCopula(0.1);
            Assert.AreEqual(copula.PDF(0.2, 0.8), 0.9630927, 1E-6);

            copula = new AMHCopula(-0.5);
            Assert.AreEqual(copula.PDF(0.2, 0.8), 1.158995, 1E-6);

            copula = new AMHCopula(-0.9);
            Assert.AreEqual(copula.PDF(0.2, 0.8), 1.259423, 1E-6);

        }

        /// <summary>
        /// Test CDF
        /// </summary>
        [TestMethod]
        public void Test_CDF()
        {
            var copula = new AMHCopula(0.8);
            Assert.AreEqual(copula.CDF(0.2, 0.8), 0.1834862, 1E-6);

            copula = new AMHCopula(0.1);
            Assert.AreEqual(copula.CDF(0.2, 0.8), 0.1626016, 1E-6);

            copula = new AMHCopula(-0.5);
            Assert.AreEqual(copula.CDF(0.2, 0.8), 0.1481481, 1E-6);

            copula = new AMHCopula(-0.9);
            Assert.AreEqual(copula.CDF(0.2, 0.8), 0.1398601, 1E-6);

        }

        /// <summary>
        /// Test InverseCDF and random number generation.
        /// </summary>
        [TestMethod]
        public void Test_Generation()
        {
            //var copula = new AMHCopula(0.8);
            //var samples = copula.GenerateRandomValues(1000, 12345);
            //for (int i = 0; i < samples.GetLength(0); i++)
            //    Debug.WriteLine(samples[i, 0] + "," + samples[i, 1]);

            //var copula = new AMHCopula(0.8);
            //copula.MarginalDistributionX = new Normal(100, 15);
            //copula.MarginalDistributionY = new Normal(80, 25);

            //var samples = copula.GenerateRandomValues(100);
            //for (int i = 0; i < samples.GetLength(0); i++)
            //    Debug.WriteLine(samples[i, 0] + "," + samples[i, 1]);

        }

        /// <summary>
        /// Estimate using the method of moments.
        /// </summary>
        [TestMethod]
        public void Test_MOM_Fit()
        {
            var data1 = new double[] { 103.35888, 118.60645, 82.65665, 100.5005, 106.33628, 110.86366, 80.12867, 77.59651, 102.11626, 105.83324, 91.95928, 107.67273, 95.32591, 90.39426, 123.34164, 97.38837, 93.2795, 101.31421, 104.81685, 85.71252, 81.54742, 81.38304, 80.13912, 101.30165, 118.91863, 108.30191, 119.78848, 97.60945, 98.78869, 103.72724, 92.42365, 102.72206, 99.2177, 107.72043, 109.43291, 100.62296, 132.70004, 120.8741, 98.96474, 95.44247, 123.17295, 123.90264, 119.82178, 111.80595, 104.83227, 106.83327, 122.96433, 101.29983, 93.6814, 112.13126, 82.61768, 126.10989, 72.31948, 90.33544, 117.35988, 115.26769, 68.14675, 86.47635, 82.05953, 99.18801, 124.63288, 114.06182, 113.25482, 120.65433, 107.87314, 79.22621, 82.23011, 112.28725, 139.83682, 107.48182, 84.28129, 92.89412, 84.83316, 138.9066, 110.03382, 95.92891, 101.93766, 71.65697, 93.66135, 116.09599, 82.89604, 112.17427, 80.59427, 68.92667, 91.07952, 96.63126, 77.48779, 112.43944, 100.23784, 100.7005, 108.10254, 114.40875, 76.79062, 93.68328, 112.74479, 91.41298, 113.4402, 88.16639, 102.08037, 67.65457 };
            var data2 = new double[] { 5.704166, 15.528819, 13.882485, 15.019548, 14.267921, 16.657361, 7.045734, 7.259652, 11.921819, 10.605115, 10.094632, 12.069054, 27.669735, 7.754541, 11.862623, 21.683318, 11.957386, 13.865843, 9.365382, 19.128899, 8.256407, 13.071849, 12.050606, 11.082313, 12.32147, 30.922919, 11.011777, 4.186213, 7.512387, 8.878237, 9.994674, 20.870344, 16.701791, 16.293253, 7.216593, 5.754389, 32.088737, 18.497179, 12.078118, 21.277432, 15.33095, 17.722765, 26.042905, 21.441997, 13.071626, 15.219579, 20.504248, 27.184325, 5.064813, 18.622192, 13.406463, 16.224917, 37.571957, 13.354813, 16.610846, 18.152655, 11.036751, 11.924343, 4.445298, 18.097828, 9.019293, 8.518562, 13.217072, 9.936002, 10.797278, 13.774978, 18.780847, 9.788386, 20.648658, 14.274315, 11.069956, 17.075933, 22.670651, 16.337241, 15.899911, 10.947108, 11.548897, 28.289972, 9.082438, 13.009808, 13.870945, 20.440472, 8.140589, 12.52854, 16.798324, 20.435226, 11.864913, 8.782708, 23.53069, 4.97201, 19.262794, 22.380103, 15.727572, 7.266394, 10.911861, 7.533538, 10.932582, 9.492285, 12.644561, 5.807184 };

            var copula = new AMHCopula();
            copula.SetThetaFromTau(data1, data2);

            Assert.AreEqual(copula.Theta, 0.5436034, 1E-4);

        }

        /// <summary>
        /// Estimate using the method of maximum pseudo likelihood.
        /// </summary>
        [TestMethod]
        public void Test_MPL_Fit()
        {

            var data1 = new double[] { 103.35888, 118.60645, 82.65665, 100.5005, 106.33628, 110.86366, 80.12867, 77.59651, 102.11626, 105.83324, 91.95928, 107.67273, 95.32591, 90.39426, 123.34164, 97.38837, 93.2795, 101.31421, 104.81685, 85.71252, 81.54742, 81.38304, 80.13912, 101.30165, 118.91863, 108.30191, 119.78848, 97.60945, 98.78869, 103.72724, 92.42365, 102.72206, 99.2177, 107.72043, 109.43291, 100.62296, 132.70004, 120.8741, 98.96474, 95.44247, 123.17295, 123.90264, 119.82178, 111.80595, 104.83227, 106.83327, 122.96433, 101.29983, 93.6814, 112.13126, 82.61768, 126.10989, 72.31948, 90.33544, 117.35988, 115.26769, 68.14675, 86.47635, 82.05953, 99.18801, 124.63288, 114.06182, 113.25482, 120.65433, 107.87314, 79.22621, 82.23011, 112.28725, 139.83682, 107.48182, 84.28129, 92.89412, 84.83316, 138.9066, 110.03382, 95.92891, 101.93766, 71.65697, 93.66135, 116.09599, 82.89604, 112.17427, 80.59427, 68.92667, 91.07952, 96.63126, 77.48779, 112.43944, 100.23784, 100.7005, 108.10254, 114.40875, 76.79062, 93.68328, 112.74479, 91.41298, 113.4402, 88.16639, 102.08037, 67.65457 };
            var data2 = new double[] { 5.704166, 15.528819, 13.882485, 15.019548, 14.267921, 16.657361, 7.045734, 7.259652, 11.921819, 10.605115, 10.094632, 12.069054, 27.669735, 7.754541, 11.862623, 21.683318, 11.957386, 13.865843, 9.365382, 19.128899, 8.256407, 13.071849, 12.050606, 11.082313, 12.32147, 30.922919, 11.011777, 4.186213, 7.512387, 8.878237, 9.994674, 20.870344, 16.701791, 16.293253, 7.216593, 5.754389, 32.088737, 18.497179, 12.078118, 21.277432, 15.33095, 17.722765, 26.042905, 21.441997, 13.071626, 15.219579, 20.504248, 27.184325, 5.064813, 18.622192, 13.406463, 16.224917, 37.571957, 13.354813, 16.610846, 18.152655, 11.036751, 11.924343, 4.445298, 18.097828, 9.019293, 8.518562, 13.217072, 9.936002, 10.797278, 13.774978, 18.780847, 9.788386, 20.648658, 14.274315, 11.069956, 17.075933, 22.670651, 16.337241, 15.899911, 10.947108, 11.548897, 28.289972, 9.082438, 13.009808, 13.870945, 20.440472, 8.140589, 12.52854, 16.798324, 20.435226, 11.864913, 8.782708, 23.53069, 4.97201, 19.262794, 22.380103, 15.727572, 7.266394, 10.911861, 7.533538, 10.932582, 9.492285, 12.644561, 5.807184 };

            // get ranks of data
            var rank1 = Statistics.RanksInplace(data1);
            var rank2 = Statistics.RanksInplace(data2);
            // get plotting positions
            for (int i = 0; i < data1.Length; i++)
            {
                rank1[i] = rank1[i] / (rank1.Length + 1d);
                rank2[i] = rank2[i] / (rank2.Length + 1d);
            }

            BivariateCopula copula = new AMHCopula();
            BivariateCopulaEstimation.Estimate(ref copula, rank1, rank2, CopulaEstimationMethod.PseudoLikelihood);

            Assert.AreEqual(copula.Theta, 0.5104257, 1E-4);

        }

        /// <summary>
        /// Estimate using the inference from margins method.
        /// </summary>
        [TestMethod]
        public void Test_IFM_Fit()
        {
            var data1 = new double[] { 103.35888, 118.60645, 82.65665, 100.5005, 106.33628, 110.86366, 80.12867, 77.59651, 102.11626, 105.83324, 91.95928, 107.67273, 95.32591, 90.39426, 123.34164, 97.38837, 93.2795, 101.31421, 104.81685, 85.71252, 81.54742, 81.38304, 80.13912, 101.30165, 118.91863, 108.30191, 119.78848, 97.60945, 98.78869, 103.72724, 92.42365, 102.72206, 99.2177, 107.72043, 109.43291, 100.62296, 132.70004, 120.8741, 98.96474, 95.44247, 123.17295, 123.90264, 119.82178, 111.80595, 104.83227, 106.83327, 122.96433, 101.29983, 93.6814, 112.13126, 82.61768, 126.10989, 72.31948, 90.33544, 117.35988, 115.26769, 68.14675, 86.47635, 82.05953, 99.18801, 124.63288, 114.06182, 113.25482, 120.65433, 107.87314, 79.22621, 82.23011, 112.28725, 139.83682, 107.48182, 84.28129, 92.89412, 84.83316, 138.9066, 110.03382, 95.92891, 101.93766, 71.65697, 93.66135, 116.09599, 82.89604, 112.17427, 80.59427, 68.92667, 91.07952, 96.63126, 77.48779, 112.43944, 100.23784, 100.7005, 108.10254, 114.40875, 76.79062, 93.68328, 112.74479, 91.41298, 113.4402, 88.16639, 102.08037, 67.65457 };
            var data2 = new double[] { 5.704166, 15.528819, 13.882485, 15.019548, 14.267921, 16.657361, 7.045734, 7.259652, 11.921819, 10.605115, 10.094632, 12.069054, 27.669735, 7.754541, 11.862623, 21.683318, 11.957386, 13.865843, 9.365382, 19.128899, 8.256407, 13.071849, 12.050606, 11.082313, 12.32147, 30.922919, 11.011777, 4.186213, 7.512387, 8.878237, 9.994674, 20.870344, 16.701791, 16.293253, 7.216593, 5.754389, 32.088737, 18.497179, 12.078118, 21.277432, 15.33095, 17.722765, 26.042905, 21.441997, 13.071626, 15.219579, 20.504248, 27.184325, 5.064813, 18.622192, 13.406463, 16.224917, 37.571957, 13.354813, 16.610846, 18.152655, 11.036751, 11.924343, 4.445298, 18.097828, 9.019293, 8.518562, 13.217072, 9.936002, 10.797278, 13.774978, 18.780847, 9.788386, 20.648658, 14.274315, 11.069956, 17.075933, 22.670651, 16.337241, 15.899911, 10.947108, 11.548897, 28.289972, 9.082438, 13.009808, 13.870945, 20.440472, 8.140589, 12.52854, 16.798324, 20.435226, 11.864913, 8.782708, 23.53069, 4.97201, 19.262794, 22.380103, 15.727572, 7.266394, 10.911861, 7.533538, 10.932582, 9.492285, 12.644561, 5.807184 };

            // get ranks of data
            var rank1 = Statistics.RanksInplace(data1);
            var rank2 = Statistics.RanksInplace(data2);
            // get plotting positions
            for (int i = 0; i < data1.Length; i++)
            {
                rank1[i] = rank1[i] / (rank1.Length + 1d);
                rank2[i] = rank2[i] / (rank2.Length + 1d);
            }

            BivariateCopula copula = new AMHCopula();
            copula.MarginalDistributionX = new Uniform(0, 1);
            copula.MarginalDistributionY = new Uniform(0, 1);

            BivariateCopulaEstimation.Estimate(ref copula, rank1, rank2, CopulaEstimationMethod.InferenceFromMargins);

            Assert.AreEqual(copula.Theta, 0.5104257, 1E-4);

        }

        /// <summary>
        /// Estimate using the method of maximum likelihood estimation.
        /// </summary>
        [TestMethod]
        public void Test_MLE_Fit()
        {
            var data1 = new double[] { 103.35888, 118.60645, 82.65665, 100.5005, 106.33628, 110.86366, 80.12867, 77.59651, 102.11626, 105.83324, 91.95928, 107.67273, 95.32591, 90.39426, 123.34164, 97.38837, 93.2795, 101.31421, 104.81685, 85.71252, 81.54742, 81.38304, 80.13912, 101.30165, 118.91863, 108.30191, 119.78848, 97.60945, 98.78869, 103.72724, 92.42365, 102.72206, 99.2177, 107.72043, 109.43291, 100.62296, 132.70004, 120.8741, 98.96474, 95.44247, 123.17295, 123.90264, 119.82178, 111.80595, 104.83227, 106.83327, 122.96433, 101.29983, 93.6814, 112.13126, 82.61768, 126.10989, 72.31948, 90.33544, 117.35988, 115.26769, 68.14675, 86.47635, 82.05953, 99.18801, 124.63288, 114.06182, 113.25482, 120.65433, 107.87314, 79.22621, 82.23011, 112.28725, 139.83682, 107.48182, 84.28129, 92.89412, 84.83316, 138.9066, 110.03382, 95.92891, 101.93766, 71.65697, 93.66135, 116.09599, 82.89604, 112.17427, 80.59427, 68.92667, 91.07952, 96.63126, 77.48779, 112.43944, 100.23784, 100.7005, 108.10254, 114.40875, 76.79062, 93.68328, 112.74479, 91.41298, 113.4402, 88.16639, 102.08037, 67.65457 };
            var data2 = new double[] { 5.704166, 15.528819, 13.882485, 15.019548, 14.267921, 16.657361, 7.045734, 7.259652, 11.921819, 10.605115, 10.094632, 12.069054, 27.669735, 7.754541, 11.862623, 21.683318, 11.957386, 13.865843, 9.365382, 19.128899, 8.256407, 13.071849, 12.050606, 11.082313, 12.32147, 30.922919, 11.011777, 4.186213, 7.512387, 8.878237, 9.994674, 20.870344, 16.701791, 16.293253, 7.216593, 5.754389, 32.088737, 18.497179, 12.078118, 21.277432, 15.33095, 17.722765, 26.042905, 21.441997, 13.071626, 15.219579, 20.504248, 27.184325, 5.064813, 18.622192, 13.406463, 16.224917, 37.571957, 13.354813, 16.610846, 18.152655, 11.036751, 11.924343, 4.445298, 18.097828, 9.019293, 8.518562, 13.217072, 9.936002, 10.797278, 13.774978, 18.780847, 9.788386, 20.648658, 14.274315, 11.069956, 17.075933, 22.670651, 16.337241, 15.899911, 10.947108, 11.548897, 28.289972, 9.082438, 13.009808, 13.870945, 20.440472, 8.140589, 12.52854, 16.798324, 20.435226, 11.864913, 8.782708, 23.53069, 4.97201, 19.262794, 22.380103, 15.727572, 7.266394, 10.911861, 7.533538, 10.932582, 9.492285, 12.644561, 5.807184 };

            BivariateCopula copula = new AMHCopula();
            copula.MarginalDistributionX = new Normal();
            copula.MarginalDistributionY = new LogNormal() { Base = Math.E };

            BivariateCopulaEstimation.Estimate(ref copula, data1, data2, CopulaEstimationMethod.FullLikelihood);

            var true_m1 = new Normal();
            true_m1.Estimate(data1, ParameterEstimationMethod.MaximumLikelihood);

            var true_m2 = new LogNormal() { Base = Math.E };
            true_m2.Estimate(data2, ParameterEstimationMethod.MaximumLikelihood);

        }
    }

}
