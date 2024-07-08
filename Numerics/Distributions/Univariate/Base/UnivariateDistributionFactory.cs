
using System;
using System.Xml.Linq;

namespace Numerics.Distributions
{

    /// <summary>
    /// A univariate distribution factory class.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public sealed class UnivariateDistributionFactory
    {

        /// <summary>
        /// Create a distribution based on the distribution type.
        /// </summary>
        /// <param name="distributionType">Distribution type.</param>
        /// <returns>
        /// A univariate distribution.
        /// </returns>
        public static UnivariateDistributionBase CreateDistribution(UnivariateDistributionType distributionType)
        {
            var distribution = default(UnivariateDistributionBase);
            if (distributionType == UnivariateDistributionType.Bernoulli)
            {
                distribution = new Bernoulli();
            }
            else if (distributionType == UnivariateDistributionType.Beta)
            {
                distribution = new BetaDistribution();
            }
            else if (distributionType == UnivariateDistributionType.Binomial)
            {
                distribution = new Binomial();
            }
            else if (distributionType == UnivariateDistributionType.Cauchy)
            {
                distribution = new Cauchy();
            }
            else if (distributionType == UnivariateDistributionType.ChiSquared)
            {
                distribution = new ChiSquared();
            }
            else if (distributionType == UnivariateDistributionType.Deterministic)
            {
                distribution = new Deterministic();
            }
            else if (distributionType == UnivariateDistributionType.Exponential)
            {
                distribution = new Exponential();
            }
            else if (distributionType == UnivariateDistributionType.GammaDistribution)
            {
                distribution = new GammaDistribution();
            }
            else if (distributionType == UnivariateDistributionType.GeneralizedBeta)
            {
                distribution = new GeneralizedBeta();
            }
            else if (distributionType == UnivariateDistributionType.GeneralizedExtremeValue)
            {
                distribution = new GeneralizedExtremeValue();
            }
            else if (distributionType == UnivariateDistributionType.GeneralizedLogistic)
            {
                distribution = new GeneralizedLogistic();
            }
            else if (distributionType == UnivariateDistributionType.GeneralizedNormal)
            {
                distribution = new GeneralizedNormal();
            }
            else if (distributionType == UnivariateDistributionType.GeneralizedPareto)
            {
                distribution = new GeneralizedPareto();
            }
            else if (distributionType == UnivariateDistributionType.Geometric)
            {
                distribution = new Geometric();
            }
            else if (distributionType == UnivariateDistributionType.Gumbel)
            {
                distribution = new Gumbel();
            }
            else if (distributionType == UnivariateDistributionType.InverseChiSquared)
            {
                distribution = new InverseChiSquared();
            }
            else if (distributionType == UnivariateDistributionType.InverseGamma)
            {
                distribution = new InverseGamma();
            }
            else if (distributionType == UnivariateDistributionType.KappaFour)
            {
                distribution = new KappaFour();
            }
            else if (distributionType == UnivariateDistributionType.LnNormal)
            {
                distribution = new LnNormal();
            }
            else if (distributionType == UnivariateDistributionType.Logistic)
            {
                distribution = new Logistic();
            }
            else if (distributionType == UnivariateDistributionType.LogNormal)
            {
                distribution = new LogNormal();
            }
            else if (distributionType == UnivariateDistributionType.LogPearsonTypeIII)
            {
                distribution = new LogPearsonTypeIII();
            }
            else if (distributionType == UnivariateDistributionType.NoncentralT)
            {
                distribution = new NoncentralT();
            }
            else if (distributionType == UnivariateDistributionType.Normal)
            {
                distribution = new Normal();
            }
            else if (distributionType == UnivariateDistributionType.Pareto)
            {
                distribution = new Pareto();
            }
            else if (distributionType == UnivariateDistributionType.PearsonTypeIII)
            {
                distribution = new PearsonTypeIII();
            }
            else if (distributionType == UnivariateDistributionType.Pert)
            {
                distribution = new Pert();
            }
            else if (distributionType == UnivariateDistributionType.PertPercentile)
            {
                distribution = new PertPercentile();
            }
            else if (distributionType == UnivariateDistributionType.PertPercentileZ)
            {
                distribution = new PertPercentileZ();
            }
            //else if (distributionType == UnivariateDistributionType.Poisson)
            //{
            //    distribution = new Poisson();
            //}
            else if (distributionType == UnivariateDistributionType.Rayleigh)
            {
                distribution = new Rayleigh();
            }
            else if (distributionType == UnivariateDistributionType.StudentT)
            {
                distribution = new StudentT();
            }
            else if (distributionType == UnivariateDistributionType.Triangular)
            {
                distribution = new Triangular();
            }
            else if (distributionType == UnivariateDistributionType.TruncatedNormal)
            {
                distribution = new TruncatedNormal();
            }
            else if (distributionType == UnivariateDistributionType.Uniform)
            {
                distribution = new Uniform();
            }
            else if (distributionType == UnivariateDistributionType.UniformDiscrete)
            {
                distribution = new UniformDiscrete();
            }
            else if (distributionType == UnivariateDistributionType.Weibull)
            {
                distribution = new Weibull();
            }

            return distribution;
        }

        /// <summary>
        /// Create a distribution from XElement.
        /// </summary>
        /// <param name="xElement">The XElement to deserialize into a univariate distribution.</param>
        /// <returns>
        /// A univariate distribution.
        /// </returns>
        public static UnivariateDistributionBase CreateDistribution(XElement xElement)
        {
            UnivariateDistributionType type;
            UnivariateDistributionBase dist = null;
            if (xElement.Attribute(nameof(UnivariateDistributionBase.Type)) != null)
            {
                Enum.TryParse(xElement.Attribute(nameof(UnivariateDistributionBase.Type)).Value, out type);

                if (type == UnivariateDistributionType.Mixture)
                {
                    dist = Mixture.FromXElement(xElement);
                    return dist;
                }
                else
                {
                    dist = CreateDistribution(type);
                }

            }
            var names = dist.GetParameterPropertyNames;
            var parms = dist.GetParameters;
            var vals = new double[dist.NumberOfParameters];
            for (int i = 0; i < dist.NumberOfParameters; i++)
            {
                if (xElement.Attribute(names[i]) != null)
                {
                    double.TryParse(xElement.Attribute(names[i]).Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out vals[i]);
                }
            }
            dist.SetParameters(vals);
            return dist;
        }


    }
}