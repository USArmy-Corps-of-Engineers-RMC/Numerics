using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Numerics.Sampling
{

    /// <summary>
    /// A class for stratification bins.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    /// <see href = "https://en.wikipedia.org/wiki/Stratified_sampling" />
    /// </para>
    /// </remarks>
    [Serializable]
    public class StratificationBin : IComparable<StratificationBin>, ICloneable
    {

        
        /// <summary>
        /// Construct new stratification bin.
        /// </summary>
        /// <param name="lowerBound">The lower bound of the bin.</param>
        /// <param name="upperBound">The upper bound of the bin.</param>
        /// <param name="weight">Optional. The weight or probability width of the bin. The weight does not have to be equal to the width.
        /// Default = -1, which will make the weight = width.</param>
        public StratificationBin(double lowerBound, double upperBound, double weight = -1)
        {
            // validate inputs
            if (lowerBound > upperBound)
            {
                throw new ArgumentOutOfRangeException(nameof(LowerBound), "The upper bound must be greater than or equal to the lower bound.");
            }

            LowerBound = lowerBound;
            UpperBound = upperBound;
            if (weight < 0d)
            {
                Weight = upperBound - lowerBound;
            }
            else
            {
                Weight = weight;
            }
        }


        /// <summary>
        /// Initialize a new instance of the stratified X values class.
        /// </summary>
        /// <param name="element">Xelement to deserialized into a stratified x value class.</param>
        public StratificationBin(XElement element)
        {
            // Get required data
            if (element.Attribute(nameof(LowerBound)) != null)
            {
                double argresult = LowerBound;
                double.TryParse(element.Attribute(nameof(LowerBound)).Value, out argresult);
                LowerBound = argresult;
            }

            if (element.Attribute(nameof(UpperBound)) != null)
            {
                double argresult1 = UpperBound;
                double.TryParse(element.Attribute(nameof(UpperBound)).Value, out argresult1);
                UpperBound = argresult1;
            }

            if (element.Attribute(nameof(Weight)) != null)
            {
                double argresult2 = Weight;
                double.TryParse(element.Attribute(nameof(Weight)).Value, out argresult2);
                Weight = argresult2;
            }
        }

        
        
        /// <summary>
        /// Get the lower bound of the bin.
        /// </summary>
        public double LowerBound { get; private set; }

        /// <summary>
        /// Get the upper bound of the bin.
        /// </summary>
        public double UpperBound { get; private set; }

        /// <summary>
        /// Gets the midpoint of the bin.
        /// </summary>
        public double Midpoint
        {
            get
            {
                return (UpperBound + LowerBound) / 2d;
            }
        }

        /// <summary>
        /// The weight given to the stratification bin. This is often the same value as the bin width.
        /// However, end bins can be assigned different weights to ensure unity.
        /// </summary>
        public double Weight { get; set; }

        
        
        /// <summary>
        /// Comparison of two bins. The bins cannot be overlapping.
        /// </summary>
        /// <param name="other">The bin to compare to.</param>
        /// <returns>
        /// 0 if the upper bound and lower bound are bit-for-bit equal.
        /// +1 if this bin is lower than the compared bin.
        /// -1 otherwise.
        /// </returns>
        public int CompareTo(StratificationBin other)
        {
            if (UpperBound > other.LowerBound && LowerBound < other.LowerBound)
            {
                throw new ArgumentException(nameof(other), "The bins cannot be overlapping.");
            }

            if (UpperBound.Equals(other.UpperBound) && LowerBound.Equals(other.LowerBound))
                return 0;
            if (other.UpperBound <= LowerBound)
                return 1;
            return -1;
        }

        /// <summary>
        /// Creates a copy of the stratification bin.
        /// </summary>
        public object Clone()
        {
            return new StratificationBin(LowerBound, UpperBound, Weight);
        }

        /// <summary>
        /// Checks whether two stratification bins are equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is StratificationBin))
                return false;
            StratificationBin bin = (StratificationBin)obj;
            return LowerBound.Equals(bin.LowerBound) && UpperBound.Equals(bin.UpperBound);
        }

        /// <summary>
        /// Returns an XElement of a stratification bin, can be used for serialization.
        /// </summary>
        public XElement SaveToXElement()
        {
            var result = new XElement("StratificationBin");
            result.SetAttributeValue(nameof(LowerBound), LowerBound.ToString());
            result.SetAttributeValue(nameof(UpperBound), UpperBound.ToString());
            result.SetAttributeValue(nameof(Weight), Weight.ToString());
            return result;
        }

        /// <summary>
        /// Returns an XElement of a list of stratification bins, can be used for serialization.
        /// </summary>
        /// <param name="stratificationBinList">Collection of stratification bins.</param>
        public static XElement SaveToXElement(IList<StratificationBin> stratificationBinList)
        {
            var result = new XElement("StratificationBinList");
            if (stratificationBinList == null) return result;
            foreach (var s in stratificationBinList)
                result.Add(s.SaveToXElement());
            return result;
        }

        /// <summary>
        /// Converts an XElement to a list of stratification bins.
        /// </summary>
        /// <param name="element">the XElement that will be deserialized to a list of stratification bins.</param>
        public static List<StratificationBin> XElementToStratificationBinList(XElement element)
        {
            var result = new List<StratificationBin>();
            if (element == null) return result;
            foreach (XElement s in element.Elements("StratificationBin"))
                result.Add(new StratificationBin(s));
            return result;
        }
    }

    
}