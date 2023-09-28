using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

namespace Numerics.Sampling
{

    /// <summary>
    /// A class for stratification options.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    [Serializable]
    public class StratificationOptions
    {

        /// <summary>
        /// Get the lower bound of the starting bin.
        /// </summary>
        public double LowerBound { get; private set; }

        /// <summary>
        /// Get the upper bound of the last bin.
        /// </summary>
        public double UpperBound { get; private set; }

        /// <summary>
        /// Returns the number of bins. Must be greater than 1.
        /// </summary>
        public int NumberOfBins { get; private set; }

        /// <summary>
        /// Determines if the values to be stratified are probabilities [0,1].
        /// </summary>
        public bool IsProbability { get; private set; }

        /// <summary>
        /// Determines if the stratification options are valid.
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        /// Constructs a stratification options class.
        /// </summary>
        /// <param name="lowerBound">The lower bound of the starting stratification bin.</param>
        /// <param name="upperBound">The upper bound of the last stratification bin.</param>
        /// <param name="numberOfBins">The number of stratification bins.</param>
        /// <param name="isProbability">Optional. Determines if the values to be stratified are probabilities [0,1]. Default = False.</param>
        public StratificationOptions(double lowerBound, double upperBound, int numberOfBins, bool isProbability = false)
        {
            // Set the options
            LowerBound = lowerBound;
            UpperBound = upperBound;
            NumberOfBins = numberOfBins;
            IsProbability = isProbability;
            // Determine if the options are valid
            IsValid = true;
            if (lowerBound >= upperBound || numberOfBins < 2)
                IsValid = false;
            if (isProbability == true && (lowerBound < 0d || upperBound > 1d))
                IsValid = false;
        }

        /// <summary>
        /// Initialize a new instance of the stratification options class from XElement.
        /// </summary>
        /// <param name="element">XElement to deserialize into a stratification options class.</param>
        public StratificationOptions(XElement element)
        {
            // Get required data
            if (element.Attribute(nameof(LowerBound)) != null)
            {
                double argresult = LowerBound;
                double.TryParse(element.Attribute(nameof(LowerBound)).Value, NumberStyles.Any, CultureInfo.InvariantCulture, out argresult);
                LowerBound = argresult;
            }

            if (element.Attribute(nameof(UpperBound)) != null)
            {
                double argresult1 = UpperBound;
                double.TryParse(element.Attribute(nameof(UpperBound)).Value, NumberStyles.Any, CultureInfo.InvariantCulture, out argresult1);
                UpperBound = argresult1;
            }

            if (element.Attribute(nameof(NumberOfBins)) != null)
            {
                int argresult2 = NumberOfBins;
                int.TryParse(element.Attribute(nameof(NumberOfBins)).Value, NumberStyles.Any, CultureInfo.InvariantCulture, out argresult2);
                NumberOfBins = argresult2;
            }

            if (element.Attribute(nameof(IsProbability)) != null)
            {
                bool argresult3 = IsProbability;
                bool.TryParse(element.Attribute(nameof(IsProbability)).Value, out argresult3);
                IsProbability = argresult3;
            }
            // Determine if the options are valid
            IsValid = true;
            if (LowerBound >= UpperBound || NumberOfBins < 2)
                IsValid = false;
            if (IsProbability == true && (LowerBound < 0d || UpperBound > 1d))
                IsValid = false;
        }

        /// <summary>
        /// Returns an XElement of stratification options, can be used for serialization.
        /// </summary>
        public XElement SaveToXElement()
        {
            var result = new XElement("StratificationOptions");
            result.SetAttributeValue(nameof(LowerBound), LowerBound.ToString("G17", CultureInfo.InvariantCulture));
            result.SetAttributeValue(nameof(UpperBound), UpperBound.ToString("G17", CultureInfo.InvariantCulture));
            result.SetAttributeValue(nameof(NumberOfBins), NumberOfBins.ToString(CultureInfo.InvariantCulture));
            result.SetAttributeValue(nameof(IsProbability), IsProbability.ToString());
            return result;
        }

        /// <summary>
        /// Returns an XElement of a list of stratification options, can be used for serialization.
        /// </summary>
        /// <param name="stratificationOptions">Collection of stratification options.</param>
        public static XElement SaveToXElement(IList<StratificationOptions> stratificationOptions)
        {
            var result = new XElement("StratificationOptionsCollection");
            if (stratificationOptions == null) return result;
            foreach (var s in stratificationOptions)
                result.Add(s.SaveToXElement());
            return result;
        }

        /// <summary>
        /// Converts an XElement to a list of stratification options.
        /// </summary>
        /// <param name="element">the XElement that will be deserialized to a list of stratification options.</param>
        public static List<StratificationOptions> XElementToStratificationOptions(XElement element)
        {
            var result = new List<StratificationOptions>();
            if (element == null) return result;
            foreach (XElement s in element.Elements("StratificationOptions"))
                result.Add(new StratificationOptions(s));
            return result;
        }

        /// <summary>
        /// Deep clones the stratification options object.
        /// </summary>
        /// <returns>A deep clone of the stratification options object.</returns>
        public StratificationOptions Clone()
        {
            return new StratificationOptions(LowerBound, UpperBound, NumberOfBins, IsProbability);
        }

        /// <summary>
        /// Deep clones a list of stratification options to a list of stratification options.
        /// </summary>
        /// <param name="stratificationOptions">Collection of stratification options to be deep cloned.</param>
        public static List<StratificationOptions> Clone(List<StratificationOptions> stratificationOptions)
        {
            var result = new List<StratificationOptions>();
            if (stratificationOptions == null) return result;
            foreach (var valueInCollection in stratificationOptions)
                result.Add(valueInCollection.Clone());
            return result;
        }

        /// <summary>
        /// Gets a list of errors for the stratification options.
        /// </summary>
        public List<string> GetErrors()
        {
            var result = new List<string>();
            if (IsValid == true)
                return result;
            if (LowerBound >= UpperBound)
            {
                result.Add("The upper bound '" + UpperBound + "' must be greater than the lower bound '" + LowerBound + "'.");
            }

            if (NumberOfBins < 2.0d)
            {
                result.Add("The number of bins '" + NumberOfBins + "' must be greater than 1.");
            }

            if (IsProbability == true && LowerBound < 0.0d)
            {
                result.Add("The lower bound must be greater than or equal to 0.");
            }

            if (IsProbability == true && UpperBound > 1.0d)
            {
                result.Add("The upper bound must be less than or equal to 1.");
            }

            return result;
        }
    }
}