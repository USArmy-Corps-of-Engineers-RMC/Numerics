using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics;
using Numerics.Data.Statistics;
using Numerics.Distributions;

namespace Distributions.Univariate
{
    [TestClass]
    public class Test_BootstrapAnalysis
    {

        /// <summary>
        /// Demonstrates how to bootstrap product moments.
        /// </summary>
        [TestMethod]
        public void Test_ProductMoments()
        {
            var norm = new Normal(2.46134734774504, 0.541117546745144);
            var boot = new BootstrapAnalysis(norm, ParameterEstimationMethod.MethodOfMoments, 30);
            var moments = boot.ProductMoments();

            // Mean
            //var kde = new KernelDensity(moments.GetColumn(0));
            //var pdf = kde.CreatePDFGraph();
            //for (int j = 0; j < pdf.GetLength(0); j++)
            //    Debug.Print(pdf[j, 0] + ", " + pdf[j, 1]);

            // Standard Deviation
            //kde = new KernelDensity(moments.GetColumn(1));
            //pdf = kde.CreatePDFGraph();
            //for (int j = 0; j < pdf.GetLength(0); j++)
            //    Debug.Print(pdf[j, 0] + ", " + pdf[j, 1]);
        }

        /// <summary>
        /// Demonstrates how to bootstrap linear moments.
        /// </summary>
        [TestMethod]
        public void Test_LinearMoments()
        {
            var norm = new Normal(2.46134734774504, 0.541117546745144);
            var boot = new BootstrapAnalysis(norm, ParameterEstimationMethod.MethodOfLinearMoments, 30);
            var moments = boot.LinearMoments();

            // Mean
            //var kde = new KernelDensity(moments.GetColumn(0));
            //var pdf = kde.CreatePDFGraph();
            //for (int j = 0; j < pdf.GetLength(0); j++)
            //    Debug.Print(pdf[j, 0] + ", " + pdf[j, 1]);

            // Standard Deviation
            //kde = new KernelDensity(moments.GetColumn(1));
            //pdf = kde.CreatePDFGraph();
            //for (int j = 0; j < pdf.GetLength(0); j++)
            //    Debug.Print(pdf[j, 0] + ", " + pdf[j, 1]);
        }

        /// <summary>
        /// Demonstrate how to bootstrap parameters
        /// </summary>
        [TestMethod]
        public void Test_Parameters()
        {
            var norm = new Normal(2.46134734774504, 0.541117546745144);
            var boot = new BootstrapAnalysis(norm, ParameterEstimationMethod.MaximumLikelihood, 30);
            var parameters = boot.Parameters();

            // Mean
            //var kde = new KernelDensity(parameters.GetColumn(0));
            //var pdf = kde.CreatePDFGraph();
            //for (int j = 0; j < pdf.GetLength(0); j++)
            //    Debug.Print(pdf[j, 0] + ", " + pdf[j, 1]);

            // Standard Deviation
            //kde = new KernelDensity(parameters.GetColumn(1));
            //pdf = kde.CreatePDFGraph();
            //for (int j = 0; j < pdf.GetLength(0); j++)
            //    Debug.Print(pdf[j, 0] + ", " + pdf[j, 1]);
        }

        /// <summary>
        /// Demonstrates how to bootstrap quantiles
        /// </summary>
        [TestMethod]
        public void Test_Quantiles()
        {
            var norm = new Normal(2.46134734774504, 0.541117546745144);
            var boot = new BootstrapAnalysis(norm, ParameterEstimationMethod.MethodOfMoments, 30);
            var quantiles = boot.Quantiles(new double[] { 0.99 });

            // 100-yr quantile
            //var kde = new KernelDensity(quantiles.GetColumn(0));
            //var pdf = kde.CreatePDFGraph();
            //for (int j = 0; j < pdf.GetLength(0); j++)
            //    Debug.Print(pdf[j, 0] + ", " + pdf[j, 1]);

        }

        /// <summary>
        /// Demonstrates how to bootstrap probabilities
        /// </summary>
        [TestMethod]
        public void Test_Probabilities()
        {
            var norm = new Normal(2.46134734774504, 0.541117546745144);
            var boot = new BootstrapAnalysis(norm, ParameterEstimationMethod.MethodOfMoments, 30);
            var probabilities = boot.Probabilities(new double[] { 3.7 });

            //var kde = new KernelDensity(probabilities.GetColumn(0));
            //var pdf = kde.CreatePDFGraph();
            //for (int j = 0; j < pdf.GetLength(0); j++)
            //    Debug.Print(pdf[j, 0] + ", " + pdf[j, 1]);
        }

        /// <summary>
        /// Demonstrates how to bootstrap quantile confidence intervals with the percentile method.
        /// </summary>
        [TestMethod]
        public void Test_PercentileCI()
        {
            var probabilities = new double[] { 0.999999, 0.999998, 0.999995, 0.99999, 0.99998, 0.99995, 0.9999, 0.9998, 0.9995, 0.999, 0.998, 0.995, 0.99, 0.98, 0.95, 0.9, 0.8, 0.7, 0.5, 0.3, 0.2, 0.1, 0.05, 0.02, 0.01 };
            var norm = new Normal(2.46134734774504, 0.541117546745144);
            var boot = new BootstrapAnalysis(norm, ParameterEstimationMethod.MethodOfMoments, 30);
            var CIs = boot.PercentileQuantileCI(probabilities);
            //for (int i = 0; i < probabilities.Length; i++)
            //    Debug.Print(CIs[i, 0] + ", " + CIs[i, 1]);

            //var ua = boot.Estimate(probabilities);
            //var xe = ua.ToXElement();
            //var ua2 = UncertaintyAnalysisResults.FromXElement(xe);
        }

        [TestMethod]
        public void Test_PercentileCI_Competing_Min()
        {
            var dist1 = new LnNormal(54.87182, 5.5009);
            var dist2 = new Weibull(50, 2);
            var dist3 = new GammaDistribution(30, 1.5);
            var cr = new CompetingRisks(new UnivariateDistributionBase[] { dist1, dist2, dist3 });

            int erl = 100;
            var sample = cr.GenerateRandomValues(12345, erl);
            //Array.Sort(sample);
            //var pp = PlottingPositions.Weibull(erl);
            //for (int i = 0; i < erl; i++)
            //    Debug.Print(sample[i] + ", " + pp[i]);


            var probabilities = new double[] { 0.001, 0.002, 0.005, 0.008, 0.01, 0.02, 0.05, 0.08, 0.1, 0.2, 0.3, 0.5, 0.7, 0.8, 0.9, 0.92, 0.95, 0.98, 0.99, 0.992, 0.995, 0.998, 0.999 };
            var boot = new BootstrapAnalysis(cr, ParameterEstimationMethod.MaximumLikelihood, erl, 1000);
            var CIs = boot.PercentileQuantileCI(probabilities);
            for (int i = 0; i < probabilities.Length; i++)
                Debug.Print(CIs[i, 0] + ", " + CIs[i, 1]);

        }

        [TestMethod]
        public void Test_PercentileCI_Mixture()
        {
            var dist1 = new LnNormal(54.87182, 5.5009);
            var dist2 = new Weibull(50, 2);
            //var dist3 = new GammaDistribution(30, 1.5);
            var mix = new Mixture(new double[] { 0.25, 0.75 }, new UnivariateDistributionBase[] { dist1, dist2 });

            int erl = 100;
            var sample = mix.GenerateRandomValues(12345, erl);
            //Array.Sort(sample);
            //var pp = PlottingPositions.Weibull(erl);
            //for (int i = 0; i < erl; i++)
            //    Debug.Print(sample[i] + ", " + pp[i]);


            var probabilities = new double[] { 0.001, 0.002, 0.005, 0.008, 0.01, 0.02, 0.05, 0.08, 0.1, 0.2, 0.3, 0.5, 0.7, 0.8, 0.9, 0.92, 0.95, 0.98, 0.99, 0.992, 0.995, 0.998, 0.999 };
            var boot = new BootstrapAnalysis(mix, ParameterEstimationMethod.MaximumLikelihood, erl, 1000);
            var CIs = boot.PercentileQuantileCI(probabilities);
            for (int i = 0; i < probabilities.Length; i++)
                Debug.Print(CIs[i, 0] + ", " + CIs[i, 1]);

        }

        /// <summary>
        /// Demonstrates how to bootstrap quantile confidence intervals with the bias-corrected percentile method.
        /// </summary>
        [TestMethod]
        public void Test_BiasCorrectedCI()
        {
            var probabilities = new double[] { 0.999999, 0.999998, 0.999995, 0.99999, 0.99998, 0.99995, 0.9999, 0.9998, 0.9995, 0.999, 0.998, 0.995, 0.99, 0.98, 0.95, 0.9, 0.8, 0.7, 0.5, 0.3, 0.2, 0.1, 0.05, 0.02, 0.01 };
            var norm = new Normal(2.46134734774504, 0.541117546745144);
            var boot = new BootstrapAnalysis(norm, ParameterEstimationMethod.MethodOfMoments, 30);
            var CIs = boot.BiasCorrectedQuantileCI(probabilities);
            //for (int i = 0; i < probabilities.Length; i++)
            //    Debug.Print(CIs[i, 0] + ", " + CIs[i, 1]);
        }

        /// <summary>
        /// Demonstrates how to bootstrap quantile confidence intervals with the standard normal method.
        /// </summary>
        [TestMethod]
        public void Test_NormalCI()
        {
            var probabilities = new double[] { 0.999999, 0.999998, 0.999995, 0.99999, 0.99998, 0.99995, 0.9999, 0.9998, 0.9995, 0.999, 0.998, 0.995, 0.99, 0.98, 0.95, 0.9, 0.8, 0.7, 0.5, 0.3, 0.2, 0.1, 0.05, 0.02, 0.01 };
            var norm = new Normal(2.46134734774504, 0.541117546745144);
            var boot = new BootstrapAnalysis(norm, ParameterEstimationMethod.MethodOfMoments, 30);
            var CIs = boot.NormalQuantileCI(probabilities);
            //for (int i = 0; i < probabilities.Length; i++)
            //    Debug.Print(CIs[i, 0] + ", " + CIs[i, 1]);
        }

        /// <summary>
        /// Demonstrates how to bootstrap quantile confidence intervals with the BCa method.
        /// The BCa method requires data to be fitted and then bootstrapped. 
        /// </summary>
        [TestMethod]
        public void Test_BCaCI()
        {
            var sampleData = new double[] { 2.6271846827571, 2.88599387922652, 3.15258918989136, 1.90904689801013, 2.44704653358702, 1.8799870344503, 2.73671792910246, 3.4417016354996, 1.99665528128517, 3.06141676579974, 1.54430146325307, 1.96949227636995, 2.24071197652694, 2.45455742849388, 1.84144033376402, 3.1441700838306, 2.5645586154462, 1.75859242658038, 2.70544707113096, 3.53258484397659, 2.9976596510263, 1.99583602805771, 2.40368152560237, 2.1922653924694, 2.80944808837791, 1.90863358306604, 2.81681188593418, 1.69020517949545, 2.335358821005, 2.79632392833483 };
            var probabilities = new double[] { 0.999999, 0.999998, 0.999995, 0.99999, 0.99998, 0.99995, 0.9999, 0.9998, 0.9995, 0.999, 0.998, 0.995, 0.99, 0.98, 0.95, 0.9, 0.8, 0.7, 0.5, 0.3, 0.2, 0.1, 0.05, 0.02, 0.01 };
            var norm = new Normal(2.46134734774504, 0.541117546745144);
            var boot = new BootstrapAnalysis(norm, ParameterEstimationMethod.MethodOfMoments, sampleData.Length);
            var CIs = boot.BCaQuantileCI(sampleData, probabilities);
            //for (int i = 0; i < probabilities.Length; i++)
            //    Debug.Print(CIs[i, 0] + ", " + CIs[i, 1]);

        }

        /// <summary>
        /// Demonstrates how to bootstrap confidence intervals with the bootstrap-t method. 
        /// </summary>
        [TestMethod]
        public void Test_BootstrapTCI()
        {
            var probabilities = new double[] { 0.999999, 0.999998, 0.999995, 0.99999, 0.99998, 0.99995, 0.9999, 0.9998, 0.9995, 0.999, 0.998, 0.995, 0.99, 0.98, 0.95, 0.9, 0.8, 0.7, 0.5, 0.3, 0.2, 0.1, 0.05, 0.02, 0.01 };
            var norm = new Normal(2.46134734774504, 0.541117546745144);
            var boot = new BootstrapAnalysis(norm, ParameterEstimationMethod.MethodOfMoments, 30);
            var CIs = boot.BootstrapTQuantileCI(probabilities);
            //for (int i = 0; i < probabilities.Length; i++)
            //    Debug.Print(CIs[i, 0] + ", " + CIs[i, 1]);

        }

    }
}
