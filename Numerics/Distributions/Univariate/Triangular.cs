using Numerics.Mathematics.RootFinding;
using System;
using System.Collections.Generic;

namespace Numerics.Distributions
{

    /// <summary>
    /// The Triangular probability distribution.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    /// <see href = "https://en.wikipedia.org/wiki/Triangular_distribution" />
    /// </para>
    /// </remarks>
    [Serializable]
    public sealed class Triangular : UnivariateDistributionBase, IEstimation, IBootstrappable
    {
       
        /// <summary>
        /// Constructs a Triangular distribution with min = 0.0, max = 1.0, and mode = 0.5.
        /// </summary>
        public Triangular()
        {
            SetParameters(0.0d, 0.5d, 1.0d);
        }

        /// <summary>
        /// Constructs a Triangular distribution with specified min, max, and mode.
        /// </summary>
        /// <param name="min">The minimum possible value of the distribution.</param>
        /// <param name="mode">The mode, or most likely, value of the distribution.</param>
        /// <param name="max">The maximum possible value of the distribution.</param>
        /// <remarks>
        /// In probability theory and statistics, the triangular distribution is a continuous probability distribution
        /// with lower limit a, upper limit b and mode c.
        /// </remarks>
        public Triangular(double min, double mode, double max)
        {
            // Set parameters
            SetParameters(min, mode, max);
        }

        private double _min;
        private double _max;
        private double _mode;
        private bool _parametersValid = true;

        /// <summary>
        /// Get and set the min of the distribution.
        /// </summary>
        public double Min
        {
            get { return _min; }
            set
            {
                _parametersValid = ValidateParameters(value, MostLikely, Max, false) is null;
                _min = value;
            }
        }

        /// <summary>
        /// Get and set the mode of the distribution.
        /// </summary>
        public double MostLikely
        {
            get { return _mode; }
            set
            {
                _parametersValid = ValidateParameters(Min, value, Max, false) is null;
                _mode = value;
            }
        }

        /// <summary>
        /// Get and set the max of the distribution.
        /// </summary>
        public double Max
        {
            get { return _max; }
            set
            {
                _parametersValid = ValidateParameters(Min, MostLikely, value, false) is null;
                _max = value;
            }
        }

        /// <summary>
        /// Returns the number of distribution parameters.
        /// </summary>
        public override int NumberOfParameters
        {
            get { return 3; }
        }

        /// <summary>
        /// Returns the continuous distribution type.
        /// </summary>
        public override UnivariateDistributionType Type
        {
            get { return UnivariateDistributionType.Triangular; }
        }

        /// <summary>
        /// Returns the name of the distribution type as a string.
        /// </summary>
        public override string DisplayName
        {
            get { return "Triangular"; }
        }

        /// <summary>
        /// Returns the short display name of the distribution as a string.
        /// </summary>
        public override string ShortDisplayName
        {
            get { return "TRI"; }
        }

        /// <summary>
        /// Get distribution parameters in 2-column array of string.
        /// </summary>
        public override string[,] ParametersToString
        {
            get
            {
                var parmString = new string[3, 2];
                parmString[0, 0] = "Min (a)";
                parmString[1, 0] = "Most Likely (c)";
                parmString[2, 0] = "Max (b)";
                parmString[0, 1] = Min.ToString();
                parmString[1, 1] = MostLikely.ToString();
                parmString[2, 1] = Max.ToString();
                return parmString;
            }
        }

        /// <summary>
        /// Gets the short form parameter names.
        /// </summary>
        public override string[] ParameterNamesShortForm
        {
            get { return new[] { "a", "c", "b" }; }
        }

        /// <summary>
        /// Gets the full parameter names.
        /// </summary>
        public override string[] GetParameterPropertyNames
        {
            get { return new[] { nameof(Min), nameof(MostLikely), nameof(Max) }; }
        }

        /// <summary>
        /// Get an array of parameters.
        /// </summary>
        public override double[] GetParameters
        {
            get { return new[] { Min, MostLikely, Max }; }
        }

        /// <summary>
        /// Determines whether the parameters are valid or not.
        /// </summary>
        public override bool ParametersValid
        {
            get { return _parametersValid; }
        }
  
        /// <summary>
        /// Gets the mean of the distribution.
        /// </summary>
        public override double Mean
        {
            get { return (_min + _max + _mode) / 3.0d; }
        }

        /// <summary>
        /// Gets the median of the distribution.
        /// </summary>
        public override double Median
        {
            get
            {
                double m = 0;
                if (_mode >= (_min + _max) / 2.0d)
                {
                    m = _min + Math.Sqrt((_max - _min) * (_mode - _min) / 2.0d);
                }
                else if (_mode <= (_min + _max) / 2.0d)
                {
                    m = _max - Math.Sqrt((_max - _min) * (_max - _mode) / 2.0d);
                }
                return m;
            }
        }

        /// <summary>
        /// Gets the mode of the distribution.
        /// </summary>
        public override double Mode
        {
            get { return _mode; }
        }

        /// <summary>
        /// Gets the standard deviation of the distribution.
        /// </summary>
        public override double StandardDeviation
        {
            get { return Math.Sqrt((_min * _min + _max * _max + _mode * _mode - _min * _max - _min * _mode - _max * _mode) / 18.0d); }
        }

        /// <summary>
        /// Get the skew of the distribution.
        /// </summary>
        public override double Skew
        {
            get
            {
                double q = Tools.Sqrt2 * (_min + _max - 2d * _mode) * (2d * _min - _max - _mode) * (_min - 2d * _max + _mode);
                double d = 5d * Math.Pow(_min * _min + _max * _max + _mode * _mode - _min * _max - _min * _mode - _max * _mode, 3.0d / 2.0d);
                return q / d;
            }
        }

        /// <summary>
        /// Get the kurtosis of the distribution.
        /// </summary>
        public override double Kurtosis
        {
            get { return 12d / 5d; }
        }

        /// <summary>
        /// Gets the minimum of the distribution.
        /// </summary>
        public override double Minimum
        {
            get { return _min; }
        }

        /// <summary>
        /// Gets the maximum of the distribution.
        /// </summary>
        public override double Maximum
        {
            get { return _max; }
        }

        /// <summary>
        /// Gets the minimum values allowable for each parameter.
        /// </summary>
        public override double[] MinimumOfParameters
        {
            get { return new[] { double.NegativeInfinity, Min, Mode }; }
        }

        /// <summary>
        /// Gets the maximum values allowable for each parameter.
        /// </summary>
        public override double[] MaximumOfParameters
        {
            get { return new[] { Mode, Max, double.PositiveInfinity }; }
        }

        /// <summary>
        /// Estimates the parameters of the underlying distribution given a sample of observations.
        /// </summary>
        /// <param name="sample">The array of sample data.</param>
        /// <param name="estimationMethod">The parameter estimation method.</param>
        public void Estimate(IList<double> sample, ParameterEstimationMethod estimationMethod)
        {
            if (estimationMethod == ParameterEstimationMethod.MethodOfMoments)
            {
                var min = Tools.Min(sample);
                var max = Tools.Max(sample);
                var mean = Tools.Mean(sample);
                var mode = mean * 3 - max - min;
                mode = Math.Max(min, mode);
                mode = Math.Min(max, mode);
                SetParameters(min, mode, max);
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
            var newDistribution = (Triangular)Clone();
            var sample = newDistribution.GenerateRandomValues(seed, sampleSize);
            newDistribution.Estimate(sample, estimationMethod);
            if (newDistribution.ParametersValid == false)
                throw new Exception("Bootstrapped distribution parameters are invalid.");
            return newDistribution;
        }

        /// <summary>
        /// Set the distribution parameters.
        /// </summary>
        /// <param name="min">The minimum possible value of the distribution.</param>
        /// <param name="mode">The mode, or most likely, value of the distribution.</param>
        /// <param name="max">The maximum possible value of the distribution.</param>
        public void SetParameters(double min, double mode, double max)
        {
            // validate parameters
            _parametersValid = ValidateParameters(min, mode, max, false) is null;
            // Set parameters
            _min = min;
            _mode = mode;
            _max = max;
        }

        /// <summary>
        /// Set the distribution parameters.
        /// </summary>
        /// <param name="parameters">Array of parameters.</param>
        public override void SetParameters(IList<double> parameters)
        {
            SetParameters(parameters[0], parameters[1], parameters[2]);
        }

        /// <summary>
        /// Validate the parameters.
        /// </summary>
        /// <param name="min">The minimum possible value of the distribution.</param>
        /// <param name="mode">The mode, or most likely, value of the distribution.</param>
        /// <param name="max">The maximum possible value of the distribution.</param>
        /// <param name="throwException">Determines whether to throw an exception or not.</param>
        private ArgumentOutOfRangeException ValidateParameters(double min, double mode, double max, bool throwException)
        {
            if (double.IsNaN(min) || double.IsInfinity(min) ||
                double.IsNaN(max) || double.IsInfinity(max) || min > max)
            {
                if (throwException)
                    throw new ArgumentOutOfRangeException(nameof(Min), "The min cannot be greater than the max.");
                return new ArgumentOutOfRangeException(nameof(Min), "The min cannot be greater than the max.");
            }
            else if (double.IsNaN(mode) || double.IsInfinity(mode) || mode < min || mode > max)
            {
                if (throwException)
                    throw new ArgumentOutOfRangeException(nameof(MostLikely), "The mode (most likely) must be between the min and max.");
                return new ArgumentOutOfRangeException(nameof(MostLikely), "The mode (most likely) must be between the min and max.");
            }
            return null;
        }

        /// <summary>
        /// Validate the parameters.
        /// </summary>
        /// <param name="parameters">A list of parameters.</param>
        /// <param name="throwException">Determines whether to throw an exception or not.</param>
        public override ArgumentOutOfRangeException ValidateParameters(IList<double> parameters, bool throwException)
        {
            return ValidateParameters(parameters[0], parameters[1], parameters[2], throwException);
        }

        /// <summary>
        /// The Probability Density Function (PDF) of the distribution evaluated at a point X.
        /// </summary>
        /// <param name="x">A single point in the distribution range.</param>
        public override double PDF(double x)
        {
            // Validate parameters
            if (_parametersValid == false)
                ValidateParameters(Min, MostLikely, Max, true);
            // 
            if (Min == Max && Min == MostLikely) return 0.0d;
            if (x < Minimum || x > Maximum) return 0.0d;
            if (x >= Min && x < MostLikely)
            {
                return 2.0d * (x - Min) / ((Max - Min) * (MostLikely - Min));
            }
            else if (x > MostLikely && x <= Max)
            {
                return 2.0d * (Max - x) / ((Max - Min) * (Max - MostLikely));
            }
            else if (x == MostLikely)
            {
                return 2.0d / (Max - Min);
            }
            return double.NaN;
        }

        /// <summary>
        /// The Cumulative Distribution Function (CDF) for the distribution evaluated at a point X.
        /// </summary>
        /// <param name="x">A single point in the distribution range.</param>
        public override double CDF(double x)
        {
            // Validate parameters
            if (_parametersValid == false)
                ValidateParameters(Min, MostLikely, Max, true);
            if (Min == Max && Min == MostLikely) return 1d;         
            if (x <= Minimum) return 0d;
            if (x >= Maximum) return 1d;
            if (x > Min && x <= MostLikely)
            {
                return (x - Min) * (x - Min) / ((Max - Min) * (MostLikely - Min));
            }
            else if (x > MostLikely && x < Max)
            {
                return 1d - (Max - x) * (Max - x) / ((Max - Min) * (Max - MostLikely));
            }
            return double.NaN;
        }

        /// <summary>
        /// Gets the Inverse Cumulative Distribution Function (ICFD) of the distribution evaluated at a probability.
        /// </summary>
        /// <param name="probability">Probability between 0 and 1.</param>
        public override double InverseCDF(double probability)
        {
            // Validate probability
            if (probability < 0.0d || probability > 1.0d)
                throw new ArgumentOutOfRangeException("probability", "Probability must be between 0 and 1.");
            if (Min == Max && Min == MostLikely) return Min;
            if (probability == 0.0d) return Minimum;
            if (probability == 1.0d) return Maximum;
            // Validate parameters
            if (_parametersValid == false)
                ValidateParameters(Min, MostLikely, Max, true);
            if (probability < (MostLikely - Min) / (Max - Min))
            {
                return Min + Math.Sqrt(probability * (Max - Min) * (MostLikely - Min));
            }
            else if (probability >= (MostLikely - Min) / (Max - Min))
            {
                return Max - Math.Sqrt((1.0d - probability) * (Max - Min) * (Max - MostLikely));
            }
            else if (Max - Min == 0d)
            {
                return MostLikely;
            }
            return double.NaN;
        }

        /// <summary>
        /// Creates a copy of the distribution.
        /// </summary>
        public override UnivariateDistributionBase Clone()
        {
            return new Triangular(Min, Mode, Max);
        }


    }
}