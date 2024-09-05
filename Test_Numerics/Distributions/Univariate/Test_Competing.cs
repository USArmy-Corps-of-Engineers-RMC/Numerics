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
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics;
using Numerics.Distributions;

namespace Distributions.Univariate
{
    /// <summary>
    /// Testing the Competing Risks distribution algorithm.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     <b> Authors: </b>
    ///     <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    ///     <item> Tiki Gonzalez, USACE Risk Management Center, julian.t.gonzalez@usace.army.mil</item>
    ///     </list> 
    /// </para>
    /// <para>
    /// <b> References: </b>
    /// </para>
    /// <para>
    /// <see href = "https://github.com/mathnet/mathnet-numerics/blob/master/src/Numerics.Tests/DistributionTests" />
    /// </para>
    /// </remarks>

    [TestClass]
    public class Test_Competing
    {
        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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
