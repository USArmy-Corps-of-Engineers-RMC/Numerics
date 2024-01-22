﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Numerics.Distributions;

namespace Numerics.Data
{
    /// <summary>
    /// Class to store uncertain ordinate information where X is stored as a double precision number and Y is stored as a continuous distribution.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Woodrow Fields, USACE Risk Management Center, woodrow.l.fields@usace.army.mil
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public struct UncertainOrdinate
    {

        #region Construction

        /// <summary>
        /// Constructs new uncertain ordinate.
        /// </summary>
        /// <param name="xValue">The x-value.</param>
        /// <param name="yValue">The y-value distribution.</param>
        public UncertainOrdinate(double xValue, UnivariateDistributionBase yValue)
        {
            X = xValue;
            Y = yValue;
            IsValid = true;
            if (double.IsInfinity(X) || double.IsNaN(X) || Y == null || Y.ParametersValid == false)
                IsValid = false;
        }

        /// <summary>
        /// Constructs new uncertain ordinate from XElement.
        /// </summary>
        /// <param name="xElement">The XElement to deserialize.</param>
        public UncertainOrdinate(XElement xElement)
        {
            double x = 0;
            UnivariateDistributionBase dist = null;
            if (xElement.Attribute(nameof(X)) != null) double.TryParse(xElement.Attribute(nameof(X)).Value, NumberStyles.Any, CultureInfo.InvariantCulture, out x);
            if (xElement.Element("Distribution") != null) { dist = UnivariateDistributionFactory.CreateDistribution(xElement.Element("Distribution")); }
            //
            X = x;
            Y = dist;
            IsValid = true;
            if (double.IsInfinity(X) || double.IsNaN(X) || Y == null || Y.ParametersValid == false)
                IsValid = false;
        }

        /// <summary>
        /// Constructs new uncertain ordinate from XElement.
        /// </summary>
        /// <param name="xElement">The XElement to deserialize.</param>
        /// <param name="distributionType">The probability distribution type.</param>
        public UncertainOrdinate(XElement xElement, UnivariateDistributionType distributionType)
        {
            double x = 0;
            if (xElement.Attribute(nameof(X)) != null) double.TryParse(xElement.Attribute(nameof(X)).Value, out x);
            // backwards compatibility
            var dist = UnivariateDistributionFactory.CreateDistribution(distributionType);
            var props = dist.GetParameterPropertyNames;
            var paramVals = new double[(props.Count())];
            for (int i = 0; i < props.Count(); i++)
            {
                double p = 0;
                if (xElement.Attribute(props[i]) != null) double.TryParse(xElement.Attribute(props[i]).Value, out p);
                paramVals[i] = p;
            }
            dist.SetParameters(paramVals);
            //
            X = x;
            Y = dist;
            IsValid = true;
            if (double.IsInfinity(X) || double.IsNaN(X) || Y == null || Y.ParametersValid == false)
                IsValid = false;
        }


        #endregion

        #region Members

        /// <summary>
        /// X Value.
        /// </summary>
        public double X;

        /// <summary>
        /// Y distribution.
        /// </summary>
        public UnivariateDistributionBase Y;

        /// <summary>
        /// Boolean indicating if the ordinate has valid numeric values or not.
        /// </summary>
        public bool IsValid;

        #endregion

        #region Methods
        

        /// <summary>
        /// Sample the uncertain ordinate to return a 'sampled' ordinate value.
        /// </summary>
        /// <param name="probability">Probability between 0 and 1.</param>
        /// <returns>A 'sampled' ordinate value</returns>
        public Ordinate GetOrdinate(double probability)
        {
            return new Ordinate(X, Y.InverseCDF(probability));
        }

        /// <summary>
        /// Gets the mean value of the uncertain distribution and returns a new ordinate value.
        /// </summary>
        /// <returns>A mean ordinate value</returns>
        public Ordinate GetOrdinate()
        {
            return new Ordinate(X, Y.Mean);
        }

        /// <summary>
        /// Test if the ordinate is valid given monotonic criteria with the next/previous ordinate in a series.
        /// </summary>
        /// <param name="ordinateToCompare">Ordinate to compare to the target ordinate.</param>
        /// <param name="strictX">Are the x-values strictly monotonic?</param>
        /// <param name="strictY">Are the y-values strictly monotonic?</param>
        /// <param name="xOrder">The order of the x-values.</param>
        /// <param name="yOrder">The order of the y-values.</param>
        /// <param name="compareOrdinateIsNext">Boolean identifying if the ordinate to compare is the next or previous ordinate in a series.</param>
        /// <param name="allowDifferentTypes">Allow different distribution types. Default = false.</param>
        /// <returns>a boolean indicating if the ordinate is valid or not given the criteria.</returns>
        public bool OrdinateValid(UncertainOrdinate ordinateToCompare, bool strictX, bool strictY, SortOrder xOrder, SortOrder yOrder, bool compareOrdinateIsNext, bool allowDifferentTypes = false)
        {
            // 
            if (IsValid == false)
                return false;
            if (ordinateToCompare.IsValid == false)
                return false;
            // Check for equivalent distribution types
            if (allowDifferentTypes == false && ordinateToCompare.Y.Type != Y.Type)
                return false;

            double minPercentile = Y.Type == UnivariateDistributionType.PertPercentile || Y.Type == UnivariateDistributionType.PertPercentileZ ? 0.05 : 1E-5;

            // Test reasonable lower bound
            if (GetOrdinate(minPercentile).OrdinateValid(ordinateToCompare.GetOrdinate(minPercentile), strictX, strictY, xOrder, yOrder, compareOrdinateIsNext) == false)
                return false;
            // Test central tendency
            if (GetOrdinate().OrdinateValid(ordinateToCompare.GetOrdinate(), strictX, strictY, xOrder, yOrder, compareOrdinateIsNext) == false)
                return false;
            // Test reasonable upper bound
            if (GetOrdinate(1 - minPercentile).OrdinateValid(ordinateToCompare.GetOrdinate(1 - minPercentile), strictX, strictY, xOrder, yOrder, compareOrdinateIsNext) == false)
                return false;
            // Passed the test
            return true;
        }
        
        /// <summary>
        /// Get all errors associated with the ordinate given monotonic criteria with the next/previous ordinate in a series.
        /// </summary>
        /// <param name="ordinateToCompare">Ordinate to compare to the target ordinate.</param>
        /// <param name="strictX">Are the x-values strictly monotonic?</param>
        /// <param name="strictY">Are the y-values strictly monotonic?</param>
        /// <param name="xOrder">The order of the x-values.</param>
        /// <param name="yOrder">The order of the y-values.</param>
        /// <param name="compareOrdinateIsNext">Boolean identifying if the ordinate to compare is the next or previous ordinate in a series.</param>
        /// <param name="allowDifferentTypes">Allow different distribution types. Default = false.</param>
        /// <returns>a list of error messages given the criteria.</returns>
        public List<string> OrdinateErrors(UncertainOrdinate ordinateToCompare, bool strictX, bool strictY, SortOrder xOrder, SortOrder yOrder, bool compareOrdinateIsNext, bool allowDifferentTypes = false)
        {
            var result = new List<string>();
            // Validate the target ordinate
            result.AddRange(OrdinateErrors());
            // 
            if (ordinateToCompare == null)
                return result;
            // 
            if (ordinateToCompare.IsValid == false)
            {
                if (double.IsInfinity(ordinateToCompare.X))
                    result.Add("Ordinate X value can not be infinity.");
                if (double.IsNaN(ordinateToCompare.X))
                    result.Add("Ordinate X value must be a valid number.");
                if (ordinateToCompare.Y == null)
                {
                    result.Add("Ordinate Y value must be defined.");
                }
                else if (ordinateToCompare.Y.ParametersValid == false)
                {
                    var argError = ordinateToCompare.Y.ValidateParameters(ordinateToCompare.Y.GetParameters, false);
                    if (argError != null) result.Add(argError.Message);
                }
            }
            // Check for equivalent distribution types
            if (allowDifferentTypes == false && ordinateToCompare.Y.Type != Y.Type)
                result.Add("Can't compare two ordinates with different distribution types."); // Return False
            // 
            if (IsValid == true && ordinateToCompare.IsValid == true)
            {
                double minPercentile = Y.Type == UnivariateDistributionType.PertPercentile || Y.Type == UnivariateDistributionType.PertPercentileZ ? 0.05 : 1E-5;

                // Test reasonable lower bound
                result.AddRange(GetOrdinate(minPercentile).OrdinateErrors(ordinateToCompare.GetOrdinate(minPercentile), strictX, strictY, xOrder, yOrder, compareOrdinateIsNext));
                // Test central tendency
                result.AddRange(GetOrdinate(0.5d).OrdinateErrors(ordinateToCompare.GetOrdinate(0.5d), strictX, strictY, xOrder, yOrder, compareOrdinateIsNext));
                // Test reasonable upper bound
                result.AddRange(GetOrdinate(1 - minPercentile).OrdinateErrors(ordinateToCompare.GetOrdinate(1 - minPercentile), strictX, strictY, xOrder, yOrder, compareOrdinateIsNext));
            }
            // Finished Checking
            return result;
        }
       
        /// <summary>
        /// Get errors with the ordinate.
        /// </summary>
        /// <returns></returns>
        public List<string> OrdinateErrors()
        {
            var result = new List<string>();
            // Validate the ordinate
            if (IsValid == false)
            {
                if (double.IsInfinity(X))
                    result.Add("Ordinate X value can not be infinity.");
                if (double.IsNaN(X))
                    result.Add("Ordinate X value must be a valid number.");
                if (Y == null)
                    result.Add("Ordinate Y value must be defined.");
                else if (Y.ParametersValid == false)
                {
                    var argError = Y.ValidateParameters(Y.GetParameters, false);
                    if (argError != null) result.Add(argError.Message);
                }
            }

            return result;
        }

        /// <summary>
        /// Checks if two uncertain ordinates are equal.
        /// </summary>
        /// <param name="left">Uncertain ordinate.</param>
        /// <param name="right">Uncertain ordinate.</param>
        public static bool operator ==(UncertainOrdinate left, UncertainOrdinate right)
        {
            //if (left == null || right == null) return false;
            if (left.X != right.X)
                return false;
            if (left.Y != right.Y)
                return false;
            return true;
        }

        /// <summary>
        /// Checks if two uncertain ordinates are not equal.
        /// </summary>
        /// <param name="left">Uncertain ordinate.</param>
        /// <param name="right">Uncertain ordinate.</param>
        public static bool operator !=(UncertainOrdinate left, UncertainOrdinate right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns the ordinate as XEleemnt.
        /// </summary>
        public XElement ToXElement()
        {
            var result = new XElement(nameof(UncertainOrdinate));
            result.SetAttributeValue(nameof(X), X.ToString("G17", CultureInfo.InvariantCulture));
            result.Add(Y.ToXElement());
            return result;
        }

        #endregion
    }
}