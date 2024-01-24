using Numerics.Data;
using Numerics.Data.Statistics;
using Numerics.Mathematics.Optimization;
using Numerics.Sampling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Numerics.Distributions
{
    /// <summary>
    /// A Mixture distribution.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    /// <see href = "https://en.wikipedia.org/wiki/Mixture_distribution" />
    /// </para>
    /// </remarks>
    [Serializable]
    public class Mixture : UnivariateDistributionBase, IEstimation, IMaximumLikelihoodEstimation, IBootstrappable
    {

        /// <summary>
        /// Construct new mixture distribution.
        /// </summary>
        /// <param name="weights">The mixture weights.</param>
        /// <param name="distributions">The mixture distributions.</param>
        public Mixture(double[] weights, UnivariateDistributionBase[] distributions)
        {
            SetParameters(weights, distributions);
        }

        /// <summary>
        /// Construct new mixture distribution.
        /// </summary>
        /// <param name="weights">The mixture weights.</param>
        /// <param name="distributions">The mixture distributions.</param>
        public Mixture(double[] weights, IUnivariateDistribution[] distributions)
        {
            SetParameters(weights, distributions);
        }

        private double[] _weights;
        private UnivariateDistributionBase[] _distributions;
        private EmpiricalDistribution _inverseCDF;
        private bool _parametersValid = true;
        private bool _momentsComputed = false;
        private double u1, u2, u3, u4;
        private bool _inverseCDFCreated = false;

        /// <summary>
        /// Returns the array of distribution weights.
        /// </summary>
        public ReadOnlyCollection<double> Weights => new ReadOnlyCollection<double>(_weights);

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
        /// Returns the number of distribution parameters.
        /// </summary>
        public override int NumberOfParameters
        {
            get
            {
                int sum = Distributions.Count;
                for (int i = 0; i < Distributions.Count; i++)
                    sum += Distributions[i].NumberOfParameters;
                return sum;
            }
        }

        /// <summary>
        /// Returns the univariate distribution type.
        /// </summary>
        public override UnivariateDistributionType Type => UnivariateDistributionType.Mixture;

        /// <summary>
        /// Returns the name of the distribution type as a string.
        /// </summary>
        public override string DisplayName => "Mixture";

        /// <summary>
        /// Returns the short display name of the distribution as a string.
        /// </summary>
        public override string ShortDisplayName => "MIX";

        /// <summary>
        /// Get distribution parameters in 2-column array of string.
        /// </summary>
        public override string[,] ParametersToString
        {
            get
            {
                var parmString = new string[2, 2];
                string Wstring = "{";
                string Dstring = "{";
                for (int i = 1; i < Weights.Count - 1; i++)
                {
                    Wstring += Weights[i].ToString();
                    Dstring += Distributions[i].DisplayName;
                    if (i < Weights.Count - 2)
                    {
                        Wstring += ",";
                        Dstring += ",";
                    }
                }
                Wstring += "}";
                Dstring += "}";
                parmString[0, 0] = "Weights";
                parmString[1, 0] = "Distributions";
                parmString[0, 1] = Wstring;
                parmString[1, 1] = Dstring;
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
                result.AddRange(Weights);
                for (int i = 0; i < Distributions.Count; i++)
                {
                    result.AddRange(Distributions[i].GetParameters);
                }                  
                return result.ToArray();
            }
        }

        /// <summary>
        /// Gets the full parameter names.
        /// </summary>
        public override string[] GetParameterPropertyNames
        {
            get { return new[] { nameof(Weights), nameof(Distributions) }; }
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
            get { return new double[] { 0, double.NaN }; }
        }

        /// <summary>
        /// Gets the maximum values allowable for each parameter.
        /// </summary>
        public override double[] MaximumOfParameters
        {
            get { return new double[] { 1, double.NaN }; }
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
            var newDistribution = (Mixture)Clone();
            var sample = newDistribution.GenerateRandomValues(seed, sampleSize);
            newDistribution.Estimate(sample, estimationMethod);
            if (newDistribution.ParametersValid == false)
                throw new Exception("Bootstrapped distribution parameters are invalid.");
            return newDistribution;
        }

        /// <summary>
        /// Set the distribution parameters.
        /// </summary>
        /// <param name="weights">The mixture weights.</param>
        /// <param name="distributions">The mixture distributions.</param>
        public void SetParameters(double[] weights, UnivariateDistributionBase[] distributions)
        {
            if (weights == null) throw new ArgumentNullException(nameof(Weights));
            if (distributions == null) throw new ArgumentNullException(nameof(Distributions));
            if (weights.Length != distributions.Length)
                throw new ArgumentException("The weight and distribution arrays must have the same length.", nameof(Weights));

            _weights = weights;
            _distributions = distributions;
            _momentsComputed = false;
            _inverseCDFCreated = false;
        }

        /// <summary>
        /// Set the distribution parameters.
        /// </summary>
        /// <param name="weights">The mixture weights.</param>
        /// <param name="distributions">The mixture distributions.</param>
        public void SetParameters(double[] weights, IUnivariateDistribution[] distributions)
        {
            if (weights == null) throw new ArgumentNullException(nameof(Weights));
            if (distributions == null) throw new ArgumentNullException(nameof(Distributions));
            if (weights.Length != distributions.Length)
                throw new ArgumentException("The weight and distribution arrays must have the same length.", nameof(Weights));

            _weights = weights;
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

            double sum = 0;
            int t = 0;
            for (int i = 0; i < Distributions.Count; i++)
            {
                _weights[i] = parameters[i];
                sum += parameters[i];
                t += 1;
            }
            for (int i = 0; i < Distributions.Count; i++)
            {
                _weights[i] /= sum;
            }
            //_weights[Distributions.Count - 1] = 1 - sum;

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
            double w = 0.0;
            for (int i = 0; i < Distributions.Count; i++)
                w += Weights[i];
            if (Math.Abs(w - 1) > Tools.DoubleMachineEpsilon * 2)
            {
                if (throwException)
                    throw new ArgumentOutOfRangeException(nameof(Weights), "The weights must sum to 1.0.");
                return new ArgumentOutOfRangeException(nameof(Weights), "The weights must sum to 1.0.");
            }
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

            // Weights are first
            int t = 0;
            for (int i = 0; i < Distributions.Count; i++)
            {
                initialVals[i] = 0.5;
                lowerVals[i] = 0;
                upperVals[i] = 1;
                t += 1;
            }

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
            double logLH(double[] parameters)
            {
                var dist = (Mixture)Clone();
                //dist.SetParameters(x);
                //return dist.LogLikelihood(sample);

                // Set distribution parameters
                int k = dist.Distributions.Count;
                var weights = new double[k];
                int t = 0;
                for (int i = 0; i < k; i++)
                {
                    weights[i] = parameters[i];
                    t += 1;
                }
                for (int i = 0; i < k; i++)
                {
                    var parms = new List<double>();
                    for (int j = t; j < t + dist.Distributions[i].NumberOfParameters; j++)
                    {
                        parms.Add(parameters[j]);
                    }
                    dist.Distributions[i].SetParameters(parms);
                    t += dist.Distributions[i].NumberOfParameters;
                }

                double lh = 0;
                for (int i = 0; i < sample.Count; i++)
                {
                    var lnf = new List<double>();
                    for (int j = 0; j < k; j++)
                    {
                        lnf.Add(Math.Log(weights[j]) + dist.Distributions[j].LogPDF(sample[i])) ;
                    }
                    lh += Tools.LogSumExp(lnf);
                }

                if (double.IsNaN(lh) || double.IsInfinity(lh)) return double.MinValue;
                return lh;
            }
            var solver = new NelderMead(logLH, NumberOfParameters, Initials, Lowers, Uppers);
            solver.Maximize();
            return solver.BestParameterSet.Values;
        }

        /// <summary>
        /// Gets the Probability Density Function (PDF) of the distribution evaluated at a point X.
        /// </summary>
        /// <param name="X">A single point in the distribution range.</param>
        public override double PDF(double x)
        {
            double f = 0.0;
            for (int i = 0; i < Distributions.Count; i++)
                f += Weights[i] * Distributions[i].PDF(x);
            return f < 0d ? 0d : f;
        }

        /// <summary>
        /// Returns the natural log of the PDF.
        /// </summary>
        /// <param name="x">A single point in the distribution range.</param>
        public override double LogPDF(double x)
        {
            var lnf = new List<double>();
            for (int i = 0; i < Distributions.Count; i++)
            {
                lnf.Add(Math.Log(Weights[i]) + Distributions[i].LogPDF(x));
            }
            var f = Tools.LogSumExp(lnf);
            // If the PDF returns an invalid probability, then return the worst log-probability.
            if (double.IsNaN(f) || double.IsInfinity(f)) return double.MinValue;
            return f;
        }

        /// <summary>
        /// Gets the Cumulative Distribution Function (CDF) for the distribution evaluated at a point X.
        /// </summary>
        /// <param name="X">A single point in the distribution range.</param>
        public override double CDF(double x)
        {
            double F = 0.0;
            for (int i = 0; i < Distributions.Count; i++)
                F += Weights[i] * Distributions[i].CDF(x);
            return F < 0d ? 0d : F > 1d ? 1d : F;
        }

        /// <summary>
        /// Returns the natural log of the CDF.
        /// </summary>
        /// <param name="x">A single point in the distribution range.</param>
        public override double LogCDF(double x)
        {
            var lnF = new List<double>();
            for (int i = 0; i < Distributions.Count; i++)
            {
                lnF.Add(Math.Log(Weights[i]) + Distributions[i].LogCDF(x));
            }
            var F = Tools.LogSumExp(lnF);
            // If the CDF returns an invalid probability, then return the worst log-probability.
            if (double.IsNaN(F) || double.IsInfinity(F) || F <= 0d) return double.MinValue;
            return F;
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
                var u = r.NextDouble();
                var cdfW = new double[Distributions.Count];
                for (int j = 0; j < Distributions.Count; j++)
                {
                    cdfW[j] = j == 0 ? Weights[j] : cdfW[j - 1] + Weights[j];
                    if (u <= cdfW[j])
                    {
                        sample[i] = Distributions[j].InverseCDF(r.NextDouble());
                        break;
                    }
                }
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
                var u = r.NextDouble();
                var cdfW = new double[Distributions.Count];
                for (int j = 0; j < Distributions.Count; j++)
                {
                    cdfW[j] = j == 0 ? Weights[j] : cdfW[j - 1] + Weights[j];
                    if (u <= cdfW[j])
                    {
                        sample[i] = Distributions[j].InverseCDF(r.NextDouble());
                        break;
                    }
                }
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

            return new Mixture(Weights.ToArray(), dists)
            {
                XTransform = XTransform,
                ProbabilityTransform = ProbabilityTransform
            };
        }

    }
}
