using System;
using System.Collections.Generic;
using Numerics.Mathematics.SpecialFunctions;

namespace Numerics.Distributions
{

    /// <summary>
    /// The Binomial distribution.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    /// <see href = "https://en.wikipedia.org/wiki/Binomial_distribution" />
    /// </para>
    /// </remarks>
    [Serializable]
    public class Binomial : UnivariateDistributionBase
    {

        
        /// <summary>
        /// Constructs a Binomial distribution with p=0.5 and n=10.
        /// </summary>
        public Binomial()
        {
            SetParameters(new[] { 0.5d, 10d });
        }

        /// <summary>
        /// Constructs a Binomial distribution with a given probability and sample size.
        /// </summary>
        /// <param name="probability">The success probability (p) in each trial. Range: 0 ≤ p ≤ 1.</param>
        /// <param name="numberOfTrials">The number of trials (n). Range: n ≥ 0.</param>
        public Binomial(double probability, int numberOfTrials)
        {
            SetParameters(new[] { probability, numberOfTrials });
        }
     
        private bool _parametersValid = true;
        private double _probabilityOfSuccess;
        private int _numberOfTrials;

        /// <summary>
        /// Gets and sets the success probability in each trial. Range: 0 ≤ p ≤ 1.
        /// </summary>
        public double ProbabilityOfSuccess
        {
            get { return _probabilityOfSuccess; }
            set
            {
                _parametersValid = ValidateParameters(new[] { value, NumberOfTrials }, false) is null;
                _probabilityOfSuccess = value;
            }
        }

        /// <summary>
        /// Gets the complement of the probability.
        /// </summary>
        public double Complement
        {
            get { return 1d - ProbabilityOfSuccess; }
        }

        /// <summary>
        /// Gets and sets the number of trials. Range: n ≥ 0.
        /// </summary>
        public int NumberOfTrials
        {
            get { return _numberOfTrials; }
            set
            {
                _parametersValid = ValidateParameters(new[] { ProbabilityOfSuccess, value }, false) is null;
                _numberOfTrials = value;
            }
        }

        /// <summary>
        /// Returns the number of distribution parameters.
        /// </summary>
        public override int NumberOfParameters
        {
            get { return 2; }
        }

        /// <summary>
        /// Returns the univariate distribution type.
        /// </summary>
        public override UnivariateDistributionType Type
        {
            get { return UnivariateDistributionType.Binomial; }
        }

        /// <summary>
        /// Returns the name of the distribution type as a string.
        /// </summary>
        public override string DisplayName
        {
            get { return "Binomial"; }
        }

        /// <summary>
        /// Returns the short display name of the distribution as a string.
        /// </summary>
        public override string ShortDisplayName
        {
            get { return "Bin"; }
        }

        /// <summary>
        /// Get distribution parameters in 2-column array of string.
        /// </summary>
        public override string[,] ParametersToString
        {
            get
            {
                var parmString = new string[2, 2];
                parmString[0, 0] = "Probability of Success (p)";
                parmString[1, 0] = "Number of Trials (n)";
                parmString[0, 1] = ProbabilityOfSuccess.ToString();
                parmString[1, 1] = NumberOfTrials.ToString();
                return parmString;
            }
        }

        /// <summary>
        /// Gets the short form parameter names.
        /// </summary>
        public override string[] ParameterNamesShortForm
        {
            get { return new[] { "p", "n" }; }
        }

        /// <summary>
        /// Gets the full parameter names.
        /// </summary>
        public override string[] GetParameterPropertyNames
        {
            get { return new[] { nameof(ProbabilityOfSuccess), nameof(NumberOfTrials) }; }
        }

        /// <summary>
        /// Get an array of parameters.
        /// </summary>
        public override double[] GetParameters
        {
            get { return new[] { ProbabilityOfSuccess, NumberOfTrials }; }
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
            get { return NumberOfTrials * ProbabilityOfSuccess; }
        }

        /// <summary>
        /// Gets the median of the distribution.
        /// </summary>
        public override double Median
        {
            get { return Math.Ceiling(NumberOfTrials * ProbabilityOfSuccess); }
        }

        /// <summary>
        /// Gets the mode of the distribution.
        /// </summary>
        public override double Mode
        {
            get
            {
                if (ProbabilityOfSuccess == 1.0d)
                    return NumberOfTrials;
                if (ProbabilityOfSuccess == 0.0d)
                    return 0.0d;
                return (int)Math.Floor((NumberOfTrials + 1) * ProbabilityOfSuccess);
            }
        }

        /// <summary>
        /// Gets the standard deviation of the distribution.
        /// </summary>
        public override double StandardDeviation
        {
            get { return Math.Sqrt(NumberOfTrials * ProbabilityOfSuccess * Complement); }
        }

        /// <summary>
        /// Gets the skew of the distribution.
        /// </summary>
        public override double Skew
        {
            get { return (1.0d - 2.0d * ProbabilityOfSuccess) / Math.Sqrt(NumberOfTrials * ProbabilityOfSuccess * Complement); }
        }

        /// <summary>
        /// Gets the kurtosis of the distribution.
        /// </summary>
        public override double Kurtosis
        {
            get { return 3d + (1.0d - 6d * Complement * ProbabilityOfSuccess) / (NumberOfTrials * ProbabilityOfSuccess * Complement); }
        }
    
        /// <summary>
        /// Gets the minimum of the distribution.
        /// </summary>
        public override double Minimum
        {
            get { return 0.0d; }
        }

        /// <summary>
        /// Gets the maximum of the distribution.
        /// </summary>
        public override double Maximum
        {
            get { return NumberOfTrials; }
        }

        /// <summary>
        /// Gets the minimum values allowable for each parameter.
        /// </summary>
        public override double[] MinimumOfParameters
        {
            get { return new[] { 0.0d, 0.0d }; }
        }

        /// <summary>
        /// Gets the maximum values allowable for each parameter.
        /// </summary>
        public override double[] MaximumOfParameters
        {
            get { return new[] { 1.0d, double.PositiveInfinity }; }
        }

        /// <summary>
        /// Set the distribution parameters.
        /// </summary>
        /// <param name="parameters">A list of parameters.</param>
        public override void SetParameters(IList<double> parameters)
        {
            ProbabilityOfSuccess = parameters[0];
            NumberOfTrials = (int)parameters[1];
        }

        /// <summary>
        /// Validate the parameters.
        /// </summary>
        /// <param name="parameters">A list of parameters.</param>
        /// <param name="throwException">Determines whether to throw an exception or not.</param>
        public override ArgumentOutOfRangeException ValidateParameters(IList<double> parameters, bool throwException)
        {
            // Validate probability
            if (parameters[0] < 0.0d || parameters[0] > 1.0d)
            {
                if (throwException)
                    throw new ArgumentOutOfRangeException(nameof(ProbabilityOfSuccess), "Probability must be between 0 and 1.");
                return new ArgumentOutOfRangeException(nameof(ProbabilityOfSuccess), "Probability must be between 0 and 1.");
            }
            if (parameters[1] <= 0.0d)
            {
                if (throwException)
                    throw new ArgumentOutOfRangeException(nameof(ProbabilityOfSuccess), "The number of trials (n) must be positive.");
                return new ArgumentOutOfRangeException(nameof(ProbabilityOfSuccess), "The number of trials (n) must be positive.");
            }
            return null;
        }
       
        /// <summary>
        /// Gets the Probability Density Function (PDF) of the distribution evaluated at a point k.
        /// </summary>
        /// <param name="k">A single point in the distribution range.</param>
        /// <remarks>Returns the Probability Mass Function (PMF) for discrete distributions.</remarks>
        public override double PDF(double k)
        {
            // Validate parameters
            if (_parametersValid == false)
                ValidateParameters(new[] { ProbabilityOfSuccess, NumberOfTrials }, true);
            k = Math.Floor(k);
            if (k < Minimum || k > Maximum) return 0.0d;
            if (ProbabilityOfSuccess == 0.0d)
                return k == 0d ? 1.0d : 0.0d;
            if (ProbabilityOfSuccess == 1.0d)
                return k == NumberOfTrials ? 1.0d : 0.0d;
            return Factorial.BinomialCoefficient(NumberOfTrials, (int)k) * Math.Pow(ProbabilityOfSuccess, k) * Math.Pow(Complement, NumberOfTrials - k);
        }

        /// <summary>
        /// Gets the Cumulative Distribution Function (CDF) for the distribution evaluated at a point k.
        /// </summary>
        /// <param name="k">A single point in the distribution range.</param>
        public override double CDF(double k)
        {
            if (_parametersValid == false)
                ValidateParameters(new[] { ProbabilityOfSuccess, NumberOfTrials }, true);
            k = Math.Floor(k);
            if (k < Minimum)
                return 0.0d;
            if (k > Maximum)
                return 1.0d;
            return Beta.Incomplete(NumberOfTrials - k, k + 1d, Complement);
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
            if (probability == 0.0d)
                return Minimum;
            if (probability == 1.0d)
                return Maximum;
            // Validate parameters
            if (_parametersValid == false)
                ValidateParameters(new[] { probability, NumberOfTrials }, true);
            double k = 0d;
            for (int i = 0; i < NumberOfTrials; i++)
            {
                if (CDF(i) >= probability)
                {
                    k = i;
                    break;
                }
            }
            return k;
        }
    
        /// <summary>
        /// Creates a copy of the distribution.
        /// </summary>
        public override UnivariateDistributionBase Clone()
        {
            return new Binomial(ProbabilityOfSuccess, NumberOfTrials);
        }
     
    }
}