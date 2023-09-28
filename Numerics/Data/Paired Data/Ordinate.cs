using System;
using System.Collections.Generic;
using Numerics.Distributions;

namespace Numerics.Data
{
    /// <summary>
    /// Class to store ordinate information where X and Y are stored as double precision numbers.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Woodrow Fields, USACE Risk Management Center, woodrow.l.fields@usace.army.mil
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    [Serializable]
    public class Ordinate
    {

        #region Construction

        /// <summary>
        /// X-Y ordinate.
        /// </summary>
        /// <param name="xValue">The x-value.</param>
        /// <param name="yValue">The y-value.</param>
        public Ordinate(double xValue, double yValue)
        {
            X = xValue;
            Y = yValue;
            IsValid = true;
            if (double.IsInfinity(X) | double.IsNaN(X) || double.IsInfinity(Y) | double.IsNaN(Y)) IsValid = false;
        }

        #endregion

        #region Members

        /// <summary>
        /// X Value.
        /// </summary>
        public double X;

        /// <summary>
        /// Y Value.
        /// </summary>
        public double Y;

        /// <summary>
        /// Boolean indicating if the ordinate has valid numeric values or not.
        /// </summary>
        public bool IsValid;

        #endregion

        #region Methods
       

        /// <summary>
        /// Test if the ordinate is valid given monotonic criteria with the next/previous ordinate in a series.
        /// </summary>
        /// <param name="ordinateToCompare">Ordinate to compare to the target ordinate.</param>
        /// <param name="strictX">Are the x-values strictly monotonic?</param>
        /// <param name="strictY">Are the y-values strictly monotonic?</param>
        /// <param name="xOrder">The order of the x-values.</param>
        /// <param name="yOrder">The order of the y-values.</param>
        /// <param name="compareOrdinateIsNext">Boolean identifying if the ordinate to compare is the next or previous ordinate in a series.</param>
        /// <returns>a boolean indicating if the ordinate is valid or not given the criteria.</returns>
        public bool OrdinateValid(Ordinate ordinateToCompare, bool strictX, bool strictY, SortOrder xOrder, SortOrder yOrder, bool compareOrdinateIsNext)
        {
            // Check the ordinate itself
            if (IsValid == false) return false;
            // Check for monotonicity.
            if (compareOrdinateIsNext == true)
            {
                // Looking forward
                if (strictY && yOrder != SortOrder.None)
                {
                    if (ordinateToCompare.Y == Y)
                        return false;
                }

                if (yOrder == SortOrder.Descending)
                {
                    if (ordinateToCompare.Y > Y)
                        return false;
                }
                else if (yOrder == SortOrder.Ascending)
                {
                    if (ordinateToCompare.Y < Y)
                        return false;
                }

                if (strictX && xOrder != SortOrder.None)
                {
                    if (ordinateToCompare.X == X)
                        return false;
                }

                if (xOrder == SortOrder.Descending)
                {
                    if (ordinateToCompare.X > X)
                        return false;
                }
                else if (xOrder == SortOrder.Ascending)
                {
                    if (ordinateToCompare.X < X)
                        return false;
                }
            }
            else
            {
                // Looking back
                if (strictY && yOrder != SortOrder.None)
                {
                    if (Y == ordinateToCompare.Y)
                        return false;
                }

                if (yOrder == SortOrder.Descending)
                {
                    if (Y > ordinateToCompare.Y)
                        return false;
                }
                else if (yOrder == SortOrder.Ascending)
                {
                    if (Y < ordinateToCompare.Y)
                        return false;
                }

                if (strictX && xOrder != SortOrder.None)
                {
                    if (X == ordinateToCompare.X)
                        return false;
                }

                if (xOrder == SortOrder.Descending)
                {
                    if (X > ordinateToCompare.X)
                        return false;
                }
                else if (xOrder == SortOrder.Ascending)
                {
                    if (X < ordinateToCompare.X)
                        return false;
                }
            }
            // Passed the test
            return true;
        }

        /// <summary>
        /// Get any error messages with the ordinate given monotonic criteria with the next/previous ordinate in a series.
        /// </summary>
        /// <param name="ordinateToCompare">Ordinate to compare to the target ordinate.</param>
        /// <param name="strictX">Are the x-values strictly monotonic?</param>
        /// <param name="strictY">Are the y-values strictly monotonic?</param>
        /// <param name="xOrder">The order of the x-values.</param>
        /// <param name="yOrder">The order of the y-values.</param>
        /// <param name="compareOrdinateIsNext">Boolean identifying if the ordinate to compare is the next or previous ordinate in a series.</param>
        /// <returns>a list of error messages given the criteria.</returns>
        public List<string> OrdinateErrors(Ordinate ordinateToCompare, bool strictX, bool strictY, SortOrder xOrder, SortOrder yOrder, bool compareOrdinateIsNext)
        {
            var result = new List<string>();
            // Check the ordinate itself
            result.AddRange(OrdinateErrors());
 
            // Check for monotonicity.
            if (strictY && yOrder != SortOrder.None)
            {
                if (ordinateToCompare.Y == Y)
                    result.Add("Ordinate Y values must be a strictly monotonic.");
            } 

            if (strictX && xOrder != SortOrder.None)
            {
                if (ordinateToCompare.X == X)
                    result.Add("Ordinate X values must be a strictly monotonic.");
            } 

            if (compareOrdinateIsNext == true)
            {
                // Looking forward
                if (yOrder == SortOrder.Descending)
                {
                    if (ordinateToCompare.Y > Y)
                        result.Add("Ordinate Y values must be monotonically decreasing."); 
                }
                else if (yOrder == SortOrder.Ascending)
                {
                    if (ordinateToCompare.Y < Y)
                        result.Add("Ordinate Y values must be monotonically increasing."); 
                }

                if (xOrder == SortOrder.Descending)
                {
                    if (ordinateToCompare.X > X)
                        result.Add("Ordinate X values must be monotonically decreasing."); 
                }
                else if (xOrder == SortOrder.Ascending)
                {
                    if (ordinateToCompare.X < X)
                        result.Add("Ordinate X values must be monotonically increasing."); 
                }
            }
            else
            {
                // Looking back
                if (yOrder == SortOrder.Descending)
                {
                    if (Y > ordinateToCompare.Y)
                        result.Add("Ordinate Y values must be monotonically decreasing."); 
                }
                else if (yOrder == SortOrder.Ascending)
                {
                    if (Y < ordinateToCompare.Y)
                        result.Add("Ordinate Y values must be monotonically increasing."); 
                }

                if (xOrder == SortOrder.Descending)
                {
                    if (X > ordinateToCompare.X)
                        result.Add("Ordinate X values must be monotonically decreasing.");
                }
                else if (xOrder == SortOrder.Ascending)
                {
                    if (X < ordinateToCompare.X)
                        result.Add("Ordinate X values must be monotonically increasing."); 
                }
            }
            // Done testing
            return result;
        }

        /// <summary>
        /// Get errors in the ordinate data.
        /// </summary>
        public List<string> OrdinateErrors()
        {
            var result = new List<string>();
            if (IsValid == false)
            {
                if (double.IsInfinity(X))
                    result.Add("Ordinate X value can not be infinity.");
                if (double.IsNaN(X))
                    result.Add("Ordinate X value must be a valid number.");
                if (double.IsInfinity(Y))
                    result.Add("Ordinate Y value can not be infinity.");
                if (double.IsNaN(Y))
                    result.Add("Ordinate Y value must be a valid number.");
            }
            return result;
        }

        /// <summary>
        /// Transform the x and y coordinates and return a transformed ordinate.
        /// </summary>
        /// <param name="xTransform">Transform method for the x value.</param>
        /// <param name="yTransform">Transform method for the y value.</param>
        public Ordinate Transform(Transform xTransform, Transform yTransform)
        {
            double transformedX = X, transformedY = Y;
            switch (xTransform)
            {
                case Data.Transform.Logarithmic:
                    {
                        transformedX = Tools.Log10(X);
                        break;
                    }
                case Data.Transform.NormalZ:
                    {
                        transformedX = Normal.StandardZ(X);
                        break;
                    }
            }
            switch (yTransform)
            {
                case Data.Transform.Logarithmic:
                    {
                        transformedY = Tools.Log10(Y);
                        break;
                    }
                case Data.Transform.NormalZ:
                    {
                        transformedY = Normal.StandardZ(Y);
                        break;
                    }
            }
            return new Ordinate(transformedX, transformedY);
        }

        #endregion
    }
}