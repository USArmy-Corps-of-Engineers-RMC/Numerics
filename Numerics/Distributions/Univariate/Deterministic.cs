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

using Numerics.Data.Statistics;
using System;
using System.Collections.Generic;

namespace Numerics.Distributions
{

    /// <summary>
    /// Deterministic point value estimate.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    /// This is used in cases, such as event tree analysis, to represent a value or event with a single probability estimate.
    /// </para>
    /// </remarks>
    [Serializable]
    public class Deterministic : UnivariateDistributionBase, IEstimation
    {
       
        /// <summary>
        /// Constructs a deterministic distribution with default value of 0.5.
        /// </summary>
        /// <remarks>
        /// This is used in cases, such as event tree analysis, to represent a value or event with a single probability estimate.
        /// </remarks>
        public Deterministic()
        {
            SetParameters(0.5d);
        }

        /// <summary>
        /// Constructs a deterministic distribution.
        /// </summary>
        /// <param name="deterministicValue">The constant value.</param>
        /// <remarks>
        /// This is used in cases, such as event tree analysis, to represent a value or event with a single probability estimate.
        /// </remarks>
        public Deterministic(double deterministicValue)
        {
            SetParameters(deterministicValue);
        }
       
        /// <summary>
        /// Gets and sets the point value estimate.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Returns the number of distribution parameters.
        /// </summary>
        public override int NumberOfParameters
        {
            get { return 1; }
        }

        /// <summary>
        /// Returns the continuous distribution type.
        /// </summary>
        public override UnivariateDistributionType Type
        {
            get { return UnivariateDistributionType.Deterministic; }
        }

        /// <summary>
        /// Returns the name of the distribution type as a string.
        /// </summary>
        public override string DisplayName
        {
            get { return "Deterministic"; }
        }

        /// <summary>
        /// Returns the short display name of the distribution as a string.
        /// </summary>
        public override string ShortDisplayName
        {
            get { return "D"; }
        }

        /// <summary>
        /// Get distribution parameters in 2-column array of string.
        /// </summary>
        public override string[,] ParametersToString
        {
            get
            {
                var parmString = new string[1, 2];
                parmString[0, 0] = "Y";
                parmString[0, 1] = Value.ToString();
                return parmString;
            }
        }

        /// <summary>
        /// Gets the short form parameter names.
        /// </summary>
        public override string[] ParameterNamesShortForm
        {
            get { return new[] { "Value" }; }
        }

        /// <summary>
        /// Gets the full parameter names.
        /// </summary>
        public override string[] GetParameterPropertyNames
        {
            get { return new[] { nameof(Value) }; }
        }

        /// <summary>
        /// Get an array of parameters.
        /// </summary>
        public override double[] GetParameters
        {
            get { return new[] { Value }; }
        }

        /// <summary>
        /// Determines whether the parameters are valid or not.
        /// </summary>
        public override bool ParametersValid
        {
            get { return true; }
        }
      
        /// <summary>
        /// Gets the mean of the distribution. Not supported.
        /// </summary>
        public override double Mean
        {
            get { return Value; }
        }

        /// <summary>
        /// Gets the median of the distribution. Not supported.
        /// </summary>
        public override double Median
        {
            get { return Value; }
        }

        /// <summary>
        /// Gets the mode of the distribution. Not supported.
        /// </summary>
        public override double Mode
        {
            get { return Value; }
        }

        /// <summary>
        /// Gets the standard deviation of the distribution. Not supported.
        /// </summary>
        public override double StandardDeviation
        {
            get { return double.NaN; }
        }

        /// <summary>
        /// Gets the skew of the distribution. Not supported.
        /// </summary>
        public override double Skew
        {
            get { return double.NaN; }
        }

        /// <summary>
        /// Gets the kurtosis of the distribution. Not supported.
        /// </summary>
        public override double Kurtosis
        {
            get { return double.NaN; }
        }
 
        /// <summary>
        /// Gets the minimum of the distribution. Not supported.
        /// </summary>
        public override double Minimum
        {
            get { return Value; }
        }

        /// <summary>
        /// Gets the maximum of the distribution. Not supported.
        /// </summary>
        public override double Maximum
        {
            get { return Value; }
        }

        /// <summary>
        /// Gets the minimum values allowable for each parameter.
        /// </summary>
        public override double[] MinimumOfParameters
        {
            get { return new[] { double.NegativeInfinity }; }
        }

        /// <summary>
        /// Gets the maximum values allowable for each parameter.
        /// </summary>
        public override double[] MaximumOfParameters
        {
            get { return new[] { double.PositiveInfinity }; }
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
                SetParameters(Statistics.Mean(sample));
            }
        }

        /// <summary>
        /// Set the distribution parameters.
        /// </summary>
        /// <param name="value">The point value estimate.</param>
        public void SetParameters(double value)
        {
            Value = value;
        }

        /// <summary>
        /// Set the distribution parameters.
        /// </summary>
        /// <param name="parameters">A list of parameters.</param>
        public override void SetParameters(IList<double> parameters)
        {
            SetParameters(parameters[0]);
        }

        /// <summary>
        /// Validate the parameters.
        /// </summary>
        /// <param name="parameters">A list of parameters.</param>
        /// <param name="throwException">Determines whether to throw an exception or not.</param>
        public override ArgumentOutOfRangeException ValidateParameters(IList<double> parameters, bool throwException)
        {
            // Validate probability
            if (double.IsNaN(parameters[0]) || double.IsInfinity(parameters[0]))
            {
                if (throwException)
                    throw new ArgumentOutOfRangeException(nameof(Probability), "The point value must be a number.");
                return new ArgumentOutOfRangeException(nameof(Probability), "The point value must be a number.");
            }
            return null;
        }

        /// <summary>
        /// Create a PDF table for graphing purposes.
        /// The bounds of the table are automatically determined.
        /// </summary>
        /// <returns>A 2-column array of X and probability density.</returns>
        public override double[,] CreatePDFGraph()
        {
            var PDFgraph = new double[5, 2];
            PDFgraph[0, 0] = Value - 1d;
            PDFgraph[0, 1] = 0d;
            PDFgraph[1, 0] = Value;
            PDFgraph[1, 1] = 0d;
            PDFgraph[2, 0] = Value;
            PDFgraph[2, 1] = 1d;
            PDFgraph[3, 0] = Value;
            PDFgraph[3, 1] = 0d;
            PDFgraph[4, 0] = Value + 1d;
            PDFgraph[4, 1] = 0d;
            return PDFgraph;
        }

        /// <summary>
        /// The Probability Density Function (PDF) of the distribution evaluated at a point X.
        /// </summary>
        /// <param name="x">A single point in the distribution range.</param>
        /// <returns>The probability of X occurring in the distribution.</returns>
        /// <remarks>
        /// The PDF describes the probability that X will occur.
        /// </remarks>
        public override double PDF(double X)
        {
            if (X != Value) return 0.0;
            return 1d;
        }

        /// <summary>
        /// The Cumulative Distribution Function (CDF) for the distribution evaluated at a point X.
        /// </summary>
        /// <param name="x">A single point in the distribution range.</param>
        /// <returns>The non-exceedance probability given a point X.</returns>
        /// <remarks>
        /// The CDF describes the cumulative probability that a given value or any value smaller than it will occur.
        /// </remarks>
        public override double CDF(double X)
        {
            if (X < Value) return 0;
            return 1d;
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
            return Value;
        }

        /// <summary>
        /// Creates a copy of the distribution.
        /// </summary>
        public override UnivariateDistributionBase Clone()
        {
            return new Deterministic(Value);
        }
   
    }
}