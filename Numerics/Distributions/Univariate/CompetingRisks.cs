using Numerics.Data;
using Numerics.Data.Statistics;
using Numerics.Mathematics.Optimization;
using Numerics.Sampling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static Numerics.Data.Statistics.Histogram;

namespace Numerics.Distributions
{
    /// <summary>
    /// A competing risks distribution.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    /// <see href = "https://reliability.readthedocs.io/en/latest/Competing%20risk%20models.html" />
    /// </para>
    /// </remarks>
    [Serializable]
    public class CompetingRisks : UnivariateDistributionBase, IEstimation, IMaximumLikelihoodEstimation, IBootstrappable
    {
        /// <summary>
        /// Construct new competing risks distribution.
        /// </summary>
        /// <param name="distributions">The competing distributions.</param>
        public CompetingRisks(UnivariateDistributionBase[] distributions)
        {
            SetParameters(distributions);
        }

        /// <summary>
        /// Construct new competing risks distribution.
        /// </summary>
        /// <param name="distributions">The competing distributions.</param>
        public CompetingRisks(IUnivariateDistribution[] distributions)
        {
            SetParameters(distributions);
        }

        private UnivariateDistributionBase[] _distributions;
        private EmpiricalDistribution _inverseCDF;
        private bool _parametersValid = true;
        private bool _momentsComputed = false;
        private double u1, u2, u3, u4;
        private bool _inverseCDFCreated = false;

        /// <summary>
        /// Returns the array of univariate probability distributions.
        /// </summary>
        public ReadOnlyCollection<UnivariateDistributionBase> Distributions => new ReadOnlyCollection<UnivariateDistributionBase>(_distributions);

        /// <summary>
        /// Determines the interpolation transform for the X-values.
        /// </summary>
        public Transform XTransform { get; set; } = Transform.None;

        /// <summary>
        /// Determines the interpolation transform for the Probability-values.
        /// </summary>
        public Transform ProbabilityTransform { get; set; } = Transform.NormalZ;

        /// <summary>
        /// If true, the competing risks model computes the minimum of the random variables. If false, it computes the maximum of random variables. 
        /// </summary>
        public bool MinimumOfRandomVariables { get; set; } = true;

        /// <summary>
        /// The dependency between random variables. 
        /// </summary>
        public Probability.DependencyType Dependency { get; set; } = Probability.DependencyType.Independent;

        /// <summary>
        /// Returns the number of distribution parameters.
        /// </summary>
        public override int NumberOfParameters
        {
            get
            {
                int sum = 0;
                for (int i = 0; i < Distributions.Count; i++)
                    sum += Distributions[i].NumberOfParameters;
                return sum;
            }
        }

        /// <summary>
        /// Returns the univariate distribution type.
        /// </summary>
        public override UnivariateDistributionType Type => UnivariateDistributionType.CompetingRisks;

        /// <summary>
        /// Returns the name of the distribution type as a string.
        /// </summary>
        public override string DisplayName => "Competing Risks";

        /// <summary>
        /// Returns the short display name of the distribution as a string.
        /// </summary>
        public override string ShortDisplayName => "CR";

        /// <summary>
        /// Get distribution parameters in 2-column array of string.
        /// </summary>
        public override string[,] ParametersToString
        {
            get
            {
                var parmString = new string[1, 2];
                string Dstring = "{";
                for (int i = 1; i < Distributions.Count - 1; i++)
                {
                    Dstring += Distributions[i].DisplayName;
                    if (i < Distributions.Count - 2)
                    {
                        Dstring += ",";
                    }
                }
                Dstring += "}";
                parmString[0, 0] = "Distributions";
                parmString[0, 1] = Dstring;
                return parmString;
            }
        }

        /// <summary>
        /// Returns the distribution parameter names as an array of string.
        /// </summary>
        public override string[] ParameterNames
        {
            get
            {
                var result = new List<string>();
                for (int i = 0; i < Distributions.Count; i++)
                {
                    result.AddRange(Distributions[i].ParameterNames);
                }
                return result.ToArray();
            }
        }


        /// <summary>
        /// Gets the short form parameter names.
        /// </summary>
        public override string[] ParameterNamesShortForm
        {
            get
            {
                var result = new List<string>();
                for (int i = 0; i < Distributions.Count; i++)
                {
                    result.AddRange(Distributions[i].ParameterNamesShortForm);
                }
                return result.ToArray();
            }
        }

        /// <summary>
        /// Get an array of parameters.
        /// </summary>
        public override double[] GetParameters 
        {
            get
            {
                var result = new List<double>();
                for (int i = 0; i < Distributions.Count; i++)
                    result.AddRange(Distributions[i].GetParameters);
                return result.ToArray();
            }
        }

        /// <summary>
        /// Gets the full parameter names.
        /// </summary>
        public override string[] GetParameterPropertyNames
        {
            get { return new[] { nameof(Distributions) }; }
        }

        /// <summary>
        /// Determines whether the parameters are valid or not.
        /// </summary>
        public override bool ParametersValid
        {
            get { return _parametersValid; }
        }

        /// <summary>
        /// Compute central moments of the distribution.
        /// </summary>
        private void ComputeMoments()
        {
            var mom = CentralMoments(1000);
            u1 = mom[0];
            u2 = mom[1];
            u3 = mom[2];
            u4 = mom[3];
            _momentsComputed = true;
        }

        /// <summary>
        /// Gets the mean of the distribution.
        /// </summary>
        public override double Mean
        {
            get
            {
                if (!_momentsComputed) ComputeMoments();
                return u1;
            }
        }

        /// <summary>
        /// Gets the median of the distribution.
        /// </summary>
        public override double Median
        {
            get { return InverseCDF(0.5d); }
        }

        /// <summary>
        /// Gets the mode of the distribution.
        /// </summary>
        public override double Mode
        {
            get { return double.NaN; }
        }

        /// <summary>
        /// Gets the standard deviation of the distribution.
        /// </summary>
        public override double StandardDeviation
        {
            get
            {
                if (!_momentsComputed) ComputeMoments();
                return u2;
            }
        }

        /// <summary>
        /// Gets the skew of the distribution.
        /// </summary>
        public override double Skew
        {
            get
            {
                if (!_momentsComputed) ComputeMoments();
                return u3;
            }
        }

        /// <summary>
        /// Gets the kurtosis of the distribution.
        /// </summary>
        public override double Kurtosis
        {
            get
            {
                if (!_momentsComputed) ComputeMoments();
                return u4;
            }
        }

        /// <summary>
        /// Gets the minimum of the distribution.
        /// </summary>
        public override double Minimum
        {
            get { return Distributions.Min(p => p.Minimum); }
        }

        /// <summary>
        /// Gets the maximum of the distribution.
        /// </summary>
        public override double Maximum
        {
            get { return Distributions.Max(p => p.Maximum); }
        }

        /// <summary>
        /// Gets the minimum values allowable for each parameter.
        /// </summary>
        public override double[] MinimumOfParameters
        {
            get { return new double[] { double.NaN }; }
        }

        /// <summary>
        /// Gets the maximum values allowable for each parameter.
        /// </summary>
        public override double[] MaximumOfParameters
        {
            get { return new double[] { double.NaN }; }
        }

        /// <summary>
        /// Estimates the parameters of the underlying distribution given a sample of observations.
        /// </summary>
        /// <param name="sample">The array of sample data.</param>
        /// <param name="estimationMethod">The parameter estimation method.</param>
        public void Estimate(IList<double> sample, ParameterEstimationMethod estimationMethod)
        {
            if (estimationMethod == ParameterEstimationMethod.MaximumLikelihood)
            {
                SetParameters(MLE(sample));
            }
        }

        /// <summary>
        /// Bootstrap the distribution based on a sample size and parameter estimation method.
        /// </summary>
        /// <param name="estimationMethod">The parameter estimation method.</param>
        /// <param name="sampleSize">Size of the random sample to generate.</param>
        /// <param name="seed">Optional. Seed for random number generator. Default = 12345.</param>
        /// <returns>
        /// Returns a bootstrapped distribution.
        /// </returns>
        public IUnivariateDistribution Bootstrap(ParameterEstimationMethod estimationMethod, int sampleSize, int seed = 12345)
        {
            var newDistribution = (CompetingRisks)Clone();
            var sample = newDistribution.GenerateRandomValues(seed, sampleSize);
            newDistribution.Estimate(sample, estimationMethod);
            if (newDistribution.ParametersValid == false)
                throw new Exception("Bootstrapped distribution parameters are invalid.");
            return newDistribution;
        }

        /// <summary>
        /// Set the distribution parameters.
        /// </summary>
        /// <param name="distributions">The competing distributions.</param>
        public void SetParameters(UnivariateDistributionBase[] distributions)
        {
            if (distributions == null) throw new ArgumentNullException(nameof(Distributions));
            _distributions = distributions;
            _momentsComputed = false;
            _inverseCDFCreated = false;
        }

        /// <summary>
        /// Set the distribution parameters.
        /// </summary>
        /// <param name="distributions">The competing distributions.</param>
        public void SetParameters(IUnivariateDistribution[] distributions)
        {
            if (distributions == null) throw new ArgumentNullException(nameof(Distributions));
            _distributions = new UnivariateDistributionBase[distributions.Length];
            for (int i = 0; i < distributions.Length; i++)
            {
                _distributions[i] = (UnivariateDistributionBase)distributions[i];
            }
            _momentsComputed = false;
            _inverseCDFCreated = false;
        }

        /// <summary>
        /// Set the distribution parameters.
        /// </summary>
        /// <param name="parameters">A list of parameters.</param>
        public override void SetParameters(IList<double> parameters)
        {
            if (Distributions == null || Distributions.Count == 0) return;

            int t = 0;
            for (int i = 0; i < Distributions.Count; i++)
            {
                var parms = new List<double>();
                for (int j = t; j < t + Distributions[i].NumberOfParameters; j++)
                {
                    parms.Add(parameters[j]);
                }
                Distributions[i].SetParameters(parms);
                t += Distributions[i].NumberOfParameters;
            }
        }

        /// <summary>
        /// Validate the parameters.
        /// </summary>
        /// <param name="parameters">A list of parameters.</param>
        /// <param name="throwException">Determines whether to throw an exception or not.</param>
        public override ArgumentOutOfRangeException ValidateParameters(IList<double> parameters, bool throwException)
        {
            for (int i = 0; i < Distributions.Count; i++)
            {
                if (Distributions[i].ParametersValid == false)
                {
                    if (throwException)
                        throw new ArgumentOutOfRangeException(nameof(Distributions), "One of the distributions have invalid parameters.");
                    return new ArgumentOutOfRangeException(nameof(Distributions), "One of the distributions have invalid parameters.");
                }
            }
            return null;
        }

        /// <summary>
        /// Get the initial, lower, and upper values for the distribution parameters for constrained optimization.
        /// </summary>
        /// <param name="sample">The array of sample data.</param>
        /// <returns>Returns a Tuple of initial, lower, and upper values.</returns>
        public Tuple<double[], double[], double[]> GetParameterConstraints(IList<double> sample)
        {
            var initialVals = new double[NumberOfParameters];
            var lowerVals = new double[NumberOfParameters];
            var upperVals = new double[NumberOfParameters];

            int t = 0;
            for (int i = 0; i < Distributions.Count; i++)
            {
                var tuple = ((IMaximumLikelihoodEstimation)Distributions[i]).GetParameterConstraints(sample);
                var initials = tuple.Item1;
                var lowers = tuple.Item2;
                var uppers = tuple.Item3;

                for (int j = t; j < t + Distributions[i].NumberOfParameters; j++)
                {
                    initialVals[j] = initials[j - t];
                    lowerVals[j] = lowers[j - t];
                    upperVals[j] = uppers[j - t];
                }
                t += Distributions[i].NumberOfParameters;
            }

            return new Tuple<double[], double[], double[]>(initialVals, lowerVals, upperVals);
        }

        /// <summary>
        /// Estimate the distribution parameters using the method of maximum likelihood estimation.
        /// </summary>
        /// <param name="sample">The array of sample data.</param>
        public double[] MLE(IList<double> sample)
        {
            // Set constraints
            var tuple = GetParameterConstraints(sample);
            var Initials = tuple.Item1;
            var Lowers = tuple.Item2;
            var Uppers = tuple.Item3;

            // Solve using Nelder-Mead (Downhill Simplex)
            double logLH(double[] x)
            {
                var dist = (CompetingRisks)Clone();
                dist.SetParameters(x);
                return dist.LogLikelihood(sample);
            }
            var solver = new NelderMead(logLH, NumberOfParameters, Initials, Lowers, Uppers);
            solver.Maximize();
            return solver.BestParameterSet.Values;
        }


        /// <summary>
        /// Gets the Probability Density Function (PDF) of the distribution evaluated at a point X.
        /// </summary>
        /// <param name="x">A single point in the distribution range.</param>
        public override double PDF(double x)
        {
       
            double f = double.NaN;

            // Only compute the PDF for independent random variables
            if (Dependency == Probability.DependencyType.Independent)
            {
                double hf = 0d;
                double sf = 1d;
                for (int i = 0; i < Distributions.Count; i++)
                {

                    if (MinimumOfRandomVariables == true)
                    {
                        hf += Distributions[i].HF(x);
                        sf *= Distributions[i].CCDF(x);
                    }
                    else
                    {
                        hf += Distributions[i].PDF(x) / Distributions[i].CDF(x);
                        sf *= Distributions[i].CDF(x);
                    }
                }
                f = hf * sf;
            }

            return f < 0d ? 0d : f;
        }


        /// <summary>
        /// Returns the natural log of the PDF.
        /// </summary>
        /// <param name="x">A single point in the distribution range.</param>
        public override double LogPDF(double x)
        {
            double f = double.NaN;

            // Only compute the PDF for independent random variables
            if (Dependency == Probability.DependencyType.Independent)
            {
                var lnhf = new List<double>();
                double lnsf = 0d;
                for (int i = 0; i < Distributions.Count; i++)
                {

                    if (MinimumOfRandomVariables == true)
                    {
                        lnhf.Add(Distributions[i].LogPDF(x) - Distributions[i].LogCCDF(x));
                        lnsf += Distributions[i].LogCCDF(x);
                    }
                    else
                    {
                        lnhf.Add(Distributions[i].LogPDF(x) - Distributions[i].LogCDF(x));
                        lnsf += Distributions[i].LogCDF(x);
                    }
                }
                f = Tools.LogSumExp(lnhf) + lnsf;
            }
            // If the PDF returns an invalid probability, then return the worst log-probability.
            if (double.IsNaN(f) || double.IsInfinity(f)) return double.MinValue;
            return f;
        }

        /// <summary>
        /// Gets the Cumulative Distribution Function (CDF) for the distribution evaluated at a point X.
        /// </summary>
        /// <param name="x">A single point in the distribution range.</param>
        public override double CDF(double x)
        {
            double p = double.NaN;
            var cdf = new List<double>();
            for (int i = 0; i < Distributions.Count; i++)
                cdf.Add(Distributions[i].CDF(x));
            
            if (MinimumOfRandomVariables == true)
            {
                p = Probability.Union(cdf, Dependency);
            }
            else
            {
                p = Probability.JointProbability(cdf, Dependency);
            }
            return p < 0d ? 0d : p > 1d ? 1d : p;
        }

        /// <summary>
        /// Gets the Inverse Cumulative Distribution Function (ICFD) of the distribution evaluated at a probability.
        /// </summary>
        /// <param name="probability">Probability between 0 and 1.</param>
        /// <returns>
        /// Returns for a given probability in the probability distribution of a random variable,
        /// the value at which the probability of the random variable is less than or equal to the
        /// given probability.
        /// </returns>
        /// <remarks>
        /// This function is also know as the Quantile Function.
        /// </remarks>
        public override double InverseCDF(double probability)
        {
            // Validate probability
            if (probability < 0.0d || probability > 1.0d)
                throw new ArgumentOutOfRangeException("probability", "Probability must be between 0 and 1.");
            if (probability == 0.0d) return Minimum;
            if (probability == 1.0d) return Maximum;
            // Validate parameters
            if (_parametersValid == false)
                ValidateParameters(new double[] { 0 }, true);
            if (_inverseCDFCreated == false)
                CreateInverseCDF();

            double min = Minimum;
            double max = Maximum;
            var x = _inverseCDF.InverseCDF(probability);
            return x < min ? min : x > max ? max : x;
        }

        /// <summary>
        /// Returns a list of cumulative incidence functions. 
        /// </summary>
        /// <param name="bins">Optional. The stratification bins to integrate over. Default is 200 bins.</param>
        public List<EmpiricalDistribution> CumulativeIncidenceFunctions(List<StratificationBin> bins = null)
        {
            // Get stratification bins
            if (bins == null)
            {
                double minP = 1E-16;
                double maxP = 1 - 1E-16;
                double minX = Distributions.Min(d => d.InverseCDF(minP));
                double maxX = Distributions.Max(d => d.InverseCDF(maxP));
                bins = Stratify.XValues(new StratificationOptions(minX, maxX, 200, false), XTransform == Transform.Logarithmic ? true : false);
            }

            var D = Distributions.Count();
            var mu = new double[D];
            var sigma = new double[D, D];

            if (Dependency == Probability.DependencyType.Independent)
            {
                var CIFs = new List<EmpiricalDistribution>();
                for (int i = 0; i < D; i++)
                {
                    var x = new double[bins.Count + 1];
                    var dF = new double[bins.Count + 1];
                    var p = new double[bins.Count + 1];

                    x[0] = bins[0].LowerBound;
                    double product = Distributions[i].CDF(bins[0].LowerBound);
                    for (int k = 0; k < D; k++)
                        product *= k == i ? 1 : Distributions[k].CCDF(bins[0].LowerBound);
                    dF[0] = product;
                    p[0] = product;

                    Parallel.For(0, bins.Count, (j) =>
                    {
                        x[j + 1] = bins[j].UpperBound;
                        double product = Distributions[i].CDF(bins[j].UpperBound) - Distributions[i].CDF(bins[j].LowerBound);
                        for (int k = 0; k < D; k++)
                            product *= k == i ? 1 : Distributions[k].CCDF(bins[j].Midpoint);
                        dF[j + 1] = product;
                    });

                    // Get cumulative 
                    for (int j = 1; j <= bins.Count; j++)
                        p[j] = p[j - 1] + dF[j];
                    
                    CIFs.Add(new EmpiricalDistribution(x, p));
                }
                return CIFs;

            }
            if (Dependency == Probability.DependencyType.PerfectlyPositive)
            {
                double rho = 1d - Math.Sqrt(Tools.DoubleMachineEpsilon);
                for (int i = 0; i < D; i++)
                {
                    mu[i] = 0d;
                    for (int j = 0; j < D; j++)
                        sigma[i, j] = i == j ? 1d : rho;
                }
            }
            if (Dependency == Probability.DependencyType.PerfectlyNegative)
            {
                double rho = -1d / (D - 1d) + Math.Sqrt(Tools.DoubleMachineEpsilon);
                for (int i = 0; i < D; i++)
                {
                    mu[i] = 0d;
                    for (int j = 0; j < D; j++)
                        sigma[i, j] = i == j ? 1d : rho;
                }
            }
            var mvn = new MultivariateNormal(mu, sigma);
            return CumulativeIncidenceFunctions(mvn, bins);
        }

        /// <summary>
        /// Returns a list of cumulative incidence functions. 
        /// </summary>
        /// <param name="multivariateNormal">The multivariate normal distribution used for dependency.</param>
        /// <param name="bins">Optional. The stratification bins to integrate over. Default is 200 bins.</param>
        public List<EmpiricalDistribution> CumulativeIncidenceFunctions(MultivariateNormal multivariateNormal, List<StratificationBin> bins = null)
        {
            var D = Distributions.Count();
            if (multivariateNormal.Dimension != D)
                throw new ArgumentException("The number of dimensions in the Multivariate Normal is not the same as the number of distributions.", nameof(multivariateNormal));

            // Get stratification bins
            if (bins == null)
            {
                double minP = 1E-16;
                double maxP = 1 - 1E-16;
                double minX = Distributions.Min(d => d.InverseCDF(minP));
                double maxX = Distributions.Max(d => d.InverseCDF(maxP));
                bins = Stratify.XValues(new StratificationOptions(minX, maxX, 200, false), XTransform == Transform.Logarithmic ? true : false);
            }

            var lower = new double[D];
            var upper = new double[D];
            var CIFs = new List<EmpiricalDistribution>();

            //var trueCorr = multivariateNormal.Covariance;

            for (int i = 0; i < D; i++)
            {

                //// Reorder the Correlation matrix
                //var newCorr = new double[D,D];
                //Array.Copy(trueCorr, newCorr, trueCorr.Length);
                //if (D > 2)
                //{
                //    newCorr.SetRow(i, trueCorr.GetRow(D - 1));
                //    newCorr.SetColumn(i, trueCorr.GetColumn(D - 1));
                //    newCorr.SetRow(D - 1, trueCorr.GetRow(i));
                //    newCorr.SetColumn(D - 1, trueCorr.GetColumn(i));
                //}
                //var mvn = new MultivariateNormal(multivariateNormal.Mean, newCorr);

                //// Reorder distributions
                //var dists = Distributions.ToList();
                //dists[i] = Distributions[D-1];
                //dists[D - 1] = Distributions[i];

                //var zp = new double[D];
                //var zl = new double[D];
                //var zu = new double[D];
                //var ind = new int[D];

                //var x = new List<double>();
                //var p = new List<double>();

                //x.Add(bins[0].LowerBound);
                //for (int k = 0; k < D; k++)
                //{
                //    zp[k] = Distributions[k].CCDF(bins[0].LowerBound);
                //    zl[k] = Distributions[k].CCDF(bins[0].LowerBound);
                //    zu[k] = Distributions[k].CCDF(bins[0].UpperBound);
                //}
                //var cp = new double[D];
                //var Sc = Probability.JointProbabilityPCM(zp, ind, mvn, cp);
                //var Sn = cp[D - 1];
                //var Su = Probability.JointProbabilityPCM(zu, ind, mvn, cp);
                //var Snu = cp[D - 1];
                //var Sl = Probability.JointProbabilityPCM(zl, ind, mvn, cp);
                //var Snl = cp[D - 1];
                //var df = Snl / Sn * Sc;
                //p.Add(df);


                //for (int j = 0; j < bins.Count; j++)
                //{
                //    x.Add(bins[j].UpperBound);
                //    for (int k = 0; k < D; k++)
                //    {
                //        zp[k] = Distributions[k].CCDF(bins[j].Midpoint);
                //        zl[k] = Distributions[k].CCDF(bins[j].LowerBound);
                //        zu[k] = Distributions[k].CCDF(bins[j].UpperBound);
                //    }
                //    cp = new double[D];
                //    Sc = Probability.JointProbabilityPCM(zp, ind, mvn, cp);
                //    Sn = cp[D - 1];
                //    Su = Probability.JointProbabilityPCM(zu, ind, mvn, cp);
                //    Snu = cp[D - 1];
                //    Sl = Probability.JointProbabilityPCM(zl, ind, mvn, cp);
                //    Snl = cp[D - 1];
                //    df = (Snl - Snu) / Sn * Sc;
                //    p.Add(p.Last() + df);
                //}
                //CIFs.Add(new EmpiricalDistribution(x, p));






                /////// Genz Method that works! Do not delete //////
                var x = new List<double>();
                var p = new List<double>();
                x.Add(bins[0].LowerBound);
                for (int k = 0; k < D; k++)
                {
                    lower[k] = k == i ? Normal.StandardZ(1E-16) : Normal.StandardZ(Distributions[k].CDF(bins[0].LowerBound));
                    upper[k] = k == i ? Normal.StandardZ(Distributions[i].CDF(bins[0].LowerBound)) : Normal.StandardZ(1 - 1E-16);
                }
                p.Add(multivariateNormal.Interval(lower, upper));

                for (int j = 0; j < bins.Count; j++)
                {
                    x.Add(bins[j].UpperBound);
                    for (int k = 0; k < D; k++)
                    {
                        lower[k] = k == i ? Normal.StandardZ(Distributions[i].CDF(bins[j].LowerBound)) : Normal.StandardZ(Distributions[k].CDF(bins[j].Midpoint));
                        upper[k] = k == i ? Normal.StandardZ(Distributions[i].CDF(bins[j].UpperBound)) : Normal.StandardZ(1 - 1E-16);
                    }
                    p.Add(p.Last() + multivariateNormal.Interval(lower, upper));
                }
                CIFs.Add(new EmpiricalDistribution(x, p));
            }

            return CIFs;
        }

        
        /// <summary>
        /// Create empirical distribution for the inverse CDF.
        /// </summary>
        private void CreateInverseCDF()
        {
            // Get min & max
            double minP = 1E-16;
            double maxP = 1 - 1E-16;
            double minX = Distributions.Min(d => d.InverseCDF(minP));
            double maxX = Distributions.Max(d => d.InverseCDF(maxP));
            // Get number of bins
            double shift = 0;
            if (minX <= 0) shift = Math.Abs(minX) + 1d;
            double min = minX + shift;
            double max = maxX + shift;
            int order = (int)Math.Floor(Math.Log10(max) - Math.Log10(min));
            int binN = Math.Max(200, 100 * order) - 1;
            // Create bins
            var bins = Stratify.XValues(new StratificationOptions(minX, maxX, binN, false), XTransform == Transform.Logarithmic ? true : false);
            var xValues = new List<double>();
            var pValues = new List<double>();
            var x = bins.First().LowerBound;
            var p = CDF(bins.First().LowerBound);
            xValues.Add(x);
            pValues.Add(p);
            for (int i = 1; i < bins.Count; i++)
            {
                x = bins[i].LowerBound;
                p = CDF(x);
                if (x > xValues.Last() && p > pValues.Last())
                {
                    xValues.Add(x);
                    pValues.Add(p);
                }
            }
            x = maxX;
            p = CDF(x);
            if (x > xValues.Last() && p > pValues.Last())
            {
                xValues.Add(x);
                pValues.Add(p);
            }
            _inverseCDF = new EmpiricalDistribution(xValues, pValues) { XTransform = XTransform, ProbabilityTransform = ProbabilityTransform };
            _inverseCDFCreated = true;
        }

        /// <summary>
        /// Generate random values of a distribution given a sample size.
        /// </summary>
        /// <param name="samplesize"> Size of random sample to generate. </param>
        /// <returns>
        /// Array of random values.
        /// </returns>
        /// <remarks>
        /// The random number generator seed is based on the current date and time according to your system.
        /// </remarks>
        public override double[] GenerateRandomValues(int samplesize)
        {
            // Create seed based on date and time
            // Create PRNG for generating random numbers
            var r = new MersenneTwister();
            var sample = new double[samplesize];
            // Generate values
            for (int i = 0; i < samplesize; i++)
            {
                double xMin = double.MaxValue;
                double xMax = double.MinValue;
                for (int j = 0; j < Distributions.Count; j++)
                {
                    var x = Distributions[j].InverseCDF(r.NextDouble());
                    if (x < xMin) xMin = x;
                    if (x > xMax) xMax = x;
                }
                sample[i] = MinimumOfRandomVariables == true ? xMin : xMax;            
            }
            // Return array of random values
            return sample;
        }

        /// <summary>
        /// Generate random values of a distribution given a sample size based on a user-defined seed.
        /// </summary>
        /// <param name="seed">Seed for random number generator.</param>
        /// <param name="samplesize"> Size of random sample to generate. </param>
        /// <returns>
        /// Array of random values.
        /// </returns>
        public override double[] GenerateRandomValues(int seed, int samplesize)
        {
            // Create PRNG for generating random numbers
            var r = new MersenneTwister(seed);
            var sample = new double[samplesize];
            // Generate values
            for (int i = 0; i < samplesize; i++)
            {
                double xMin = double.MaxValue;
                double xMax = double.MinValue;
                for (int j = 0; j < Distributions.Count; j++)
                {
                    var x = Distributions[j].InverseCDF(r.NextDouble());
                    if (x < xMin) xMin = x;
                    if (x > xMax) xMax = x;
                }
                sample[i] = MinimumOfRandomVariables == true ? xMin : xMax;
            }
            // Return array of random values
            return sample;
        }

        /// <summary>
        /// Creates a copy of the distribution.
        /// </summary>
        public override UnivariateDistributionBase Clone()
        {
            var dists = new UnivariateDistributionBase[Distributions.Count];
            for (int i = 0; i < Distributions.Count; i++)
                dists[i] = Distributions[i].Clone();

            return new CompetingRisks(dists)
            {
                MinimumOfRandomVariables = MinimumOfRandomVariables,
                Dependency = Dependency,
                XTransform = XTransform,
                ProbabilityTransform = ProbabilityTransform
            };
        }

    }
}
