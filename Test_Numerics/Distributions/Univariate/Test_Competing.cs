using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics;
using Numerics.Distributions;

namespace Distributions.Univariate
{
    [TestClass]
    public class Test_Competing
    {
        [TestMethod]
        public void TestMethod1()
        {

            var D1 = new LogNormal(4, 0.1) { Base = Math.E };
            var D2 = new Weibull(50, 2);

            var cr = new CompetingRisks(new UnivariateDistributionBase[] { D1, D2 });
            cr.MinimumOfRandomVariables = true;
            cr.Dependency = Numerics.Data.Statistics.Probability.DependencyType.PerfectlyPositive;

            var pdf = cr.CreateCDFGraph();
            for (int i = 0; i < pdf.GetLength(0); i++)
            {
                Debug.Print(pdf[i, 0] + ", " + pdf[i, 1]);
            }
        }

        [TestMethod]
        public void Test_CIFs_Independent()
        {

            var D1 = new LogNormal(4, 0.1) { Base = Math.E };
            var D2 = new Weibull(50, 2);

            var cr = new CompetingRisks(new UnivariateDistributionBase[] { D1, D2 });
            cr.MinimumOfRandomVariables = true;
            cr.Dependency = Numerics.Data.Statistics.Probability.DependencyType.Independent;
            var cifs = cr.CumulativeIncidenceFunctions();

            for (int i = 0; i < cifs[0].XValues.Count; i++)
            {
                Debug.Print(cifs[0].XValues[i] + ", " + cifs[0].ProbabilityValues[i] + ", " + cifs[1].ProbabilityValues[i]);
            }
        }

        [TestMethod]
        public void Test_CIFs_Positive()
        {

            var D1 = new LogNormal(4, 0.1) { Base = Math.E };
            var D2 = new Weibull(50, 2);

            var cr = new CompetingRisks(new UnivariateDistributionBase[] { D1, D2 });
            cr.MinimumOfRandomVariables = true;
            cr.Dependency = Numerics.Data.Statistics.Probability.DependencyType.PerfectlyPositive;
            var cifs = cr.CumulativeIncidenceFunctions();

            for (int i = 0; i < cifs[0].XValues.Count; i++)
            {
                Debug.Print(cifs[0].XValues[i] + ", " + cifs[0].ProbabilityValues[i] + ", " + cifs[1].ProbabilityValues[i]);
            }
        }

        [TestMethod]
        public void Test_CIFs_Negative()
        {
            var D1 = new LogNormal(4, 0.1) { Base = Math.E };
            var D2 = new Weibull(50, 2);

            var cr = new CompetingRisks(new UnivariateDistributionBase[] { D1, D2 });
            cr.MinimumOfRandomVariables = true;
            cr.Dependency = Numerics.Data.Statistics.Probability.DependencyType.PerfectlyNegative;
            var cifs = cr.CumulativeIncidenceFunctions();

            for (int i = 0; i < cifs[0].XValues.Count; i++)
            {
                Debug.Print(cifs[0].XValues[i] + ", " + cifs[0].ProbabilityValues[i] + ", " + cifs[1].ProbabilityValues[i]);
            }
        }

        [TestMethod]
        public void Test_CIFs_Correlation()
        {
            var D1 = new LogNormal(4, 0.1) { Base = Math.E };
            var D2 = new Weibull(50, 2);

            var cr = new CompetingRisks(new UnivariateDistributionBase[] { D1, D2 });
            cr.MinimumOfRandomVariables = true;
            cr.Dependency = Numerics.Data.Statistics.Probability.DependencyType.PerfectlyNegative;
            var cifs = cr.CumulativeIncidenceFunctions();

            for (int i = 0; i < cifs[0].XValues.Count; i++)
            {
                Debug.Print(cifs[0].XValues[i] + ", " + cifs[0].ProbabilityValues[i] + ", " + cifs[1].ProbabilityValues[i]);
            }
        }

    }
}
