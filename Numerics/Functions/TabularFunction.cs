using Numerics.Data;
using System;
using System.Collections.Generic;

namespace Numerics.Functions
{
    /// <summary>
    /// A class for a tabular, or nonparametric, function.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Authors:
    /// Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public class TabularFunction : IUnivariateFunction
    {

        /// <summary>
        /// Constructs a tabular function from uncertain ordered paired data.
        /// </summary>
        /// <param name="pairedData">The uncertain ordered paired data.</param>
        public TabularFunction(UncertainOrderedPairedData pairedData)
        {
            _pairedData = pairedData;
            opd = _pairedData.CurveSample();
        }

        private UncertainOrderedPairedData _pairedData;
        private OrderedPairedData opd;
        private double _confidenceLevel = -1;

        /// <summary>
        /// The uncertain ordered paired data. 
        /// </summary>
        public UncertainOrderedPairedData PairedData => _pairedData;

        /// <summary>
        /// The transform for the x-values. Default = None.
        /// </summary>
        public Transform XTransform { get; set; } = Transform.None;

        /// <summary>
        /// The transform for the y-values. Default = None.
        /// </summary>
        public Transform YTransform { get; set; } = Transform.None;

        /// <summary>
        /// Returns the number of function parameters.
        /// </summary>
        public int NumberOfParameters => 1;

        /// <summary>
        /// Returns a boolean value describing if the current parameters are valid or not.
        /// If not, an ArgumentOutOfRange exception will be thrown when trying to use function.
        /// </summary>
        public bool ParametersValid => PairedData.IsValid;

        /// <summary>
        /// Gets and sets the minimum X value supported by the function.
        /// </summary>
        public double Minimum { get; set; } = double.MinValue;

        /// <summary>
        /// Gets and sets the maximum X value supported by the function. Default = double.MaxValue.
        /// </summary>
        public double Maximum { get; set; } = double.MaxValue;

        /// <summary>
        /// Gets the minimum values allowable for each parameter.
        /// </summary>
        public double[] MinimumOfParameters => new double[] { double.MinValue };

        /// <summary>
        /// Gets the maximum values allowable for each parameter.
        /// </summary>
        public double[] MaximumOfParameters => new double[] { double.MaxValue};

        /// <summary>
        /// Determines if the function is deterministic or if it has uncertainty. 
        /// </summary>
        public bool IsDeterministic 
        {
            get
            { 
                if (_pairedData.Distribution == Distributions.UnivariateDistributionType.Deterministic)
                {
                    return true;
                }
                else
                {
                    return false;
                }                
            }
            set
            {
                if (value == true)
                {
                    _pairedData = new UncertainOrderedPairedData(_pairedData, _pairedData.StrictX, _pairedData.OrderX, _pairedData.StrictY, _pairedData.OrderY, Distributions.UnivariateDistributionType.Deterministic);
                }
            }
        }

        /// <summary>
        /// The confidence level to estimate when the function has uncertainty. 
        /// </summary>
        public double ConfidenceLevel 
        { 
            get { return _confidenceLevel; }
            set
            {
                _confidenceLevel = value;
                if (_confidenceLevel < 0 )
                {
                    opd = _pairedData.CurveSample();
                }
                else
                {
                    opd = _pairedData.CurveSample(_confidenceLevel);
                }
                
            }
        }

        /// <summary>
        /// Determines if the tabular function can return negative Y values.
        /// </summary>
        public bool AllowNegativeYValues { get; set; } = true;

        /// <summary>
        /// Set the distribution parameters.
        /// </summary>
        /// <param name="parameters">Array of parameters.</param>
        public void SetParameters(IList<double> parameters)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Test to see if function parameters are valid.
        /// </summary>
        /// <param name="parameters">Array of parameters.</param>
        /// <param name="throwException">Boolean indicating whether to throw the exception or not.</param>
        /// <returns>Nothing if the parameters are valid and the exception if invalid parameters were found.</returns>
        public ArgumentOutOfRangeException ValidateParameters(IList<double> parameters, bool throwException)
        {
            var errors = PairedData.GetErrors();
            if (errors.Count > 0)
            {
                if (throwException)
                    throw new ArgumentOutOfRangeException(nameof(PairedData), "The uncertain ordered paired data has errors.");
                return new ArgumentOutOfRangeException(nameof(PairedData), "The uncertain ordered paired data has errors.");
            }
            return null;
        }

        /// <summary>
        /// Returns the function evaluated at a point x. If function is uncertain, the function is computed at the set confidence level. 
        /// </summary>
        /// <param name="x">The x-value in the function to evaluate.</param>
        public double Function(double x)
        {
            // Validate parameters
            if (ParametersValid == false) ValidateParameters(new double[] {0}, true);
            double y = opd.Interpolate(x, true, XTransform, YTransform);
            y = AllowNegativeYValues == false && y < 0 ? 0 : y;
            return y;
        }

        /// <summary>
        /// Returns the inverse function evaluated at a point y. If function is uncertain, the function is computed at the set confidence level. 
        /// </summary>
        /// <param name="y">The y-value in the inverse function to evaluate.</param>
        public double InverseFunction(double y)
        {
            // Validate parameters
            if (ParametersValid == false) ValidateParameters(new double[] { 0 }, true);
            y = AllowNegativeYValues == false && y < 0 ? 0 : y;          
            return opd.Interpolate(y, false, XTransform, YTransform);
        }
    }
}
