using Numerics.Data.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Numerics.Distributions
{

    /// <summary>
    /// The PERT probability distribution.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    ///     In probability and statistics, the PERT distribution is a family of continuous probability distributions
    ///     defined by the minimum (a), most likely (b) and maximum (c) values that a variable can take.
    ///     It is a transformation of the four-parameter Beta distribution.
    /// </para>
    /// <para>
    /// References:
    /// <list type="bullet">
    /// <item><description>
    /// Wikipedia contributors, "PERT distribution,". Wikipedia, The Free
    /// Encyclopedia. Available at: https://en.wikipedia.org/wiki/PERT_distribution.
    /// </description></item>
    /// </list>
    /// </para>
    /// </remarks>
    [Serializable]
    public class Pert : UnivariateDistributionBase, IEstimation
    {

        /// <summary>
        /// Constructs a PERT distribution with min = 0.0, max = 1.0, and mode = 0.5.
        /// </summary>
        public Pert()
        {
            SetParameters(0.0d, 0.5d, 1.0d);
        }

        /// <summary>
        /// Constructs a PERT distribution with specified min, max, and mode.
        /// </summary>
        /// <param name="min">The minimum possible value of the distribution.</param>
        /// <param name="mode">The mode, or most likely, value of the distribution.</param>
        /// <param name="max">The maximum possible value of the distribution.</param>
        public Pert(double min, double mode, double max)
        {
            SetParameters(min, mode, max);
        }

        private bool _parametersValid = true;
        private GeneralizedBeta _beta = new GeneralizedBeta();
        private double _min;
        private double _max;
        private double _mode;

        /// <summary>
        /// Gets the underlying Generalized Beta distribution.
        /// </summary>
        public GeneralizedBeta Beta => _beta;

        /// <summary>
        /// Get and set the min of the distribution.
        /// </summary>
        public double Min
        {
            get { return _min; }
            set { SetParameters(value, MostLikely, Max); }
        }

        /// <summary>
        /// Get and set the mode of the distribution.
        /// </summary>
        public double MostLikely
        {
            get { return _mode; }
            set { SetParameters(Min, value, Max); }
        }

        /// <summary>
        /// Get and set the max of the distribution.
        /// </summary>
        public double Max
        {
            get { return _max; }
            set { SetParameters(Min, MostLikely, value); }
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
            get { return UnivariateDistributionType.Pert; }
        }

        /// <summary>
        /// Returns the name of the distribution type as a string.
        /// </summary>
        public override string DisplayName
        {
            get { return "PERT"; }
        }

        /// <summary>
        /// Returns the short display name of the distribution as a string.
        /// </summary>
        public override string ShortDisplayName
        {
            get { return "PERT"; }
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
            get 
            {
                if (_min == _max && _min == _mode) return _min;
                return (Min + 4 * MostLikely + Max) / 6; 
            }
        }

        /// <summary>
        /// Gets the median of the distribution.
        /// </summary>
        public override double Median
        {
            get 
            {
                if (_min == _max && _min == _mode) return _min;
                return (Min + 6 * MostLikely + Max) / 8; 
            }
        }

        /// <summary>
        /// Gets the mode of the distribution.
        /// </summary>
        public override double Mode
        {
            get { return MostLikely; }
        }

        /// <summary>
        /// Gets the standard deviation of the distribution.
        /// </summary>
        public override double StandardDeviation
        {
            get { return Math.Sqrt((Mean - Min) * (Max - Mean) / 7); }
        }

        /// <summary>
        /// Gets the skew of the distribution.
        /// </summary>
        public override double Skew
        {
            get { return _beta.Skew; }
        }

        /// <summary>
        /// Gets the kurtosis of the distribution.
        /// </summary>
        public override double Kurtosis
        {
            get { return _beta.Kurtosis; }
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
            if (estimationMethod == ParameterEstimationMethod.MethodOfPercentiles)
            {
                var data = sample.ToList();
                data.Sort();
                var five = Statistics.Percentile(data, 0.05, true);
                var fifty = Statistics.Percentile(data, 0.5, true);
                var ninetyFive = Statistics.Percentile(data, 0.95, true);
                var pertP = new PertPercentile(five, fifty, ninetyFive);
                pertP.SolveParameters();
                var pert = pertP.ToPert();
                SetParameters(pert.Min, pert.Mode, pert.Max);
            }
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
            // Return PERT
            if (_parametersValid == true) _beta = GeneralizedBeta.PERT(_min, _mode, _max);
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
                if (throwException) throw new ArgumentOutOfRangeException(nameof(Min), "The min cannot be greater than the max.");
                return new ArgumentOutOfRangeException(nameof(Min), "The min cannot be greater than the max.");
            }
            else if (double.IsNaN(mode) || double.IsInfinity(mode) || mode < min || mode > max)
            {
                if (throwException) throw new ArgumentOutOfRangeException(nameof(MostLikely), "The mode (most likely) must be between the min and max.");
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
            if (_parametersValid == false) ValidateParameters(Min, Mode, Max, true);
            // These checks are done specifically for an application where a 
            // user inputs min = max = mode
            if (_min == _max && _min == _mode) return 0.0d;
            if (double.IsNaN(_mode)) return 0.0d;
            return _beta.PDF(x);
        }

        /// <summary>
        /// The Cumulative Distribution Function (CDF) for the distribution evaluated at a point X.
        /// </summary>
        /// <param name="x">A single point in the distribution range.</param>
        /// <returns>The non-exceedance probability given a point X.</returns>
        /// <remarks>
        /// The CDF describes the cumulative probability that a given value or any value smaller than it will occur.
        /// </remarks>
        public override double CDF(double x)
        {
            // Validate parameters
            if (_parametersValid == false) ValidateParameters(Min, Mode, Max, true);
            // These checks are done specifically for an application where a 
            // user inputs min = max = mode
            if (_min == _max && _min == _mode) return 1d;
            if (double.IsNaN(_mode)) return 1;
            return _beta.CDF(x);
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
            // Validate parameters
            if (_parametersValid == false) ValidateParameters(Min, Mode, Max, true);
            // These checks are done specifically for an application where a 
            // user inputs min = max = mode
            if (_min == _max && _min == _mode) return Min;
            if (double.IsNaN(_mode)) return Min;
            return _beta.InverseCDF(probability);
        }

        /// <summary>
        /// Creates a copy of the distribution.
        /// </summary>
        public override UnivariateDistributionBase Clone()
        {
            return new Pert(Min, Mode, Max);
        }


    }
}