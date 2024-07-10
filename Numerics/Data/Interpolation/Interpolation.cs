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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Numerics.Distributions;

namespace Numerics.Data
{

    /// <summary>
    /// Contains interpolation functions.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// This class contains functions for 1-D and 2-D linear interpolation methods.
    /// Data can be interpolated from data tables or arrays.
    /// </remarks>
    /// <authors>
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </authors>
    /// <references>
    /// Visual Basic Macro 'Interpolate Version 2.0.0' November 2017, USACE Risk Management Center
    /// </references>
    public sealed class Interpolation
    {

        ///// <summary>
        ///// Enumeration defining the sort order of X values.
        ///// </summary>
        //public enum SortOrder
        //{
        //    /// <summary>
        //    /// Ascending order.
        //    /// </summary>
        //    Ascending,
        //    /// <summary>
        //    /// Descending order.
        //    /// </summary>
        //    Descending,
        //    /// <summary>
        //    /// Not sorted. 
        //    /// </summary>
        //    None
        //}

        /// <summary>
        /// Enumeration that determines whether to extrapolate.
        /// </summary>
        public enum Extrapolate
        {
            /// <summary>
            /// Yes, do extrapolate. 
            /// </summary>
            Yes,
            /// <summary>
            /// No, do not extrapolate. 
            /// </summary>
            No
        }

        ///// <summary>
        ///// Enumeration of X and Y value transformations.
        ///// </summary>
        //public enum Transform
        //{
        //    /// <summary>
        //    /// Linear, or no transform. 
        //    /// </summary>
        //    Linear,
        //   /// <summary>
        //   /// Logarithmic transform. Values must be greater than 0.
        //   /// </summary>
        //    Logarithmic,
        //    /// <summary>
        //    /// Normal distribution Z-variate transform. Values must be between 0 and 1. 
        //    /// </summary>
        //    NormalZ
        //}

        /// <summary>
        /// Enumeration of search methods.
        /// </summary>
        public enum SearchMethod
        {
            /// <summary>
            /// Search sequentially.
            /// </summary>
            Linear,
            /// <summary>
            /// Search with bisection method.
            /// </summary>
            Bisection,
            /// <summary>
            /// Search using the hunt algorithm.
            /// </summary>
            Hunt
        }

        /// <summary>
        /// Searches for the lower bound of the location of a value using a sequential search method.
        /// </summary>
        /// <param name="value">The value to search for.</param>
        /// <param name="valueArray">Array of values.</param>
        /// <param name="searchStart">Optional. Location to start the search of the arrays. Default = 0.</param>
        /// <param name="order">Optional. Ascending or descending. Default = ASC.</param>
        /// <returns>
        /// The lower bound of the location to be used in interpolation. E.g., X1 is the lower, X2 is the upper (X1 + 1).
        /// </returns>
        public static int LinearSearch(double value, IList<double> valueArray, int searchStart = 0, SortOrder order = SortOrder.Ascending)
        {
            // variables
            int N = valueArray.Count - 1;
            var XLO = default(int);
            bool ASCND;

            // validate
            if (searchStart < 0)
            {
                throw new ArgumentOutOfRangeException("searchStart", "The search starting point must be non-negative.");
            }
            else if (searchStart > N)
            {
                throw new ArgumentOutOfRangeException("searchStart", "The search starting point cannot be greater than the length of the X array.");
            }

            // check if the order is ascending, if the X is outside of the range of the array
            if (order == SortOrder.Ascending)
            {
                ASCND = true;
                if (value < valueArray[0])
                {
                    return -1;
                }
                else if (value == valueArray[0])
                {
                    return 0;
                }
                else if (value == valueArray[N])
                {
                    return N;
                }
                else if (value > valueArray[N])
                {
                    return N + 1;
                }
            }
            else
            {
                ASCND = false;
                if (value > valueArray[0])
                {
                    return -1;
                }
                else if (value == valueArray[0])
                {
                    return 0;
                }
                else if (value == valueArray[N])
                {
                    return N;
                }
                else if (value < valueArray[N])
                {
                    return N + 1;
                }
            }

            // perform linear search method
            if (ASCND == true)
            {
                for (int i = searchStart; i <= N; i++)
                {
                    if (value <= valueArray[i])
                    {
                        XLO = i - 1;
                        break;
                    }
                }
            }
            else
            {
                for (int i = searchStart; i <= N; i++)
                {
                    if (value >= valueArray[i])
                    {
                        XLO = i - 1;
                        break;
                    }
                }
            }
            // Return XLO
            return XLO;
        }

        /// <summary>
        /// Searches for the lower bound of the location of a value using a sequential search method.
        /// </summary>
        /// <param name="xValue">The value to search for.</param>
        /// <param name="orderedPairedData">Ordered paired data.</param>
        /// <param name="searchStart">Optional. Location to start the search of the arrays. Default = 0.</param>
        /// <returns>
        /// The lower bound of the location to be used in interpolation. E.g., X1 is the lower, X2 is the upper (X1 + 1).
        /// </returns>
        public static int LinearSearch(double xValue, OrderedPairedData orderedPairedData, int searchStart = 0)
        {
            // variables
            int N = orderedPairedData.Count - 1;
            var XLO = default(int);

            // validate
            if (searchStart < 0)
            {
                throw new ArgumentOutOfRangeException("searchStart", "The search starting point must be non-negative.");
            }
            else if (searchStart > N)
            {
                throw new ArgumentOutOfRangeException("searchStart", "The search starting point cannot be greater than the length of the X array.");
            }

            // check if the order is ascending, if the X is outside of the range of the array
            if (orderedPairedData.OrderX == SortOrder.Ascending)
            {
                if (xValue < orderedPairedData[0].X)
                {
                    return -1;
                }
                else if (xValue == orderedPairedData[0].X)
                {
                    return 0;
                }
                else if (xValue == orderedPairedData[N].X)
                {
                    return N;
                }
                else if (xValue > orderedPairedData[N].X)
                {
                    return N + 1;
                }
            }
            else if (xValue > orderedPairedData[0].X)
            {
                return -1;
            }
            else if (xValue == orderedPairedData[0].X)
            {
                return 0;
            }
            else if (xValue == orderedPairedData[N].X)
            {
                return N;
            }
            else if (xValue < orderedPairedData[N].X)
            {
                return N + 1;
            }

            // perform linear search method
            if (orderedPairedData.OrderX == SortOrder.Ascending)
            {
                for (int i = searchStart; i <= N; i++)
                {
                    if (xValue <= orderedPairedData[i].X)
                    {
                        XLO = i - 1;
                        break;
                    }
                }
            }
            else
            {
                for (int i = searchStart; i <= N; i++)
                {
                    if (xValue >= orderedPairedData[i].X)
                    {
                        XLO = i - 1;
                        break;
                    }
                }
            }
            // 
            return XLO;
        }

        /// <summary>
        /// Searches for the lower bound of the location of a value using a sequential search method.
        /// </summary>
        /// <param name="xValue">The value to search for.</param>
        /// <param name="ordinateData">List of ordinates.</param>
        /// <param name="searchStart">Optional. Location to start the search of the arrays. Default = 0.</param>
        /// <param name="order">Optional. Ascending or descending. Default = ASC.</param>
        /// <returns>
        /// The lower bound of the location to be used in interpolation. E.g., X1 is the lower, X2 is the upper (X1 + 1).
        /// </returns>
        public static int LinearSearch(double xValue, IList<Ordinate> ordinateData, int searchStart = 0, SortOrder order = SortOrder.Ascending)
        {
            // variables
            int N = ordinateData.Count - 1;
            var XLO = default(int);

            // validate
            if (searchStart < 0)
            {
                throw new ArgumentOutOfRangeException("searchStart", "The search starting point must be non-negative.");
            }
            else if (searchStart > N)
            {
                throw new ArgumentOutOfRangeException("searchStart", "The search starting point cannot be greater than the length of the X array.");
            }

            // check if the order is ascending, if the X is outside of the range of the array
            if (order == SortOrder.Ascending)
            {
                if (xValue < ordinateData[0].X)
                {
                    return -1;
                }
                else if (xValue == ordinateData[0].X)
                {
                    return 0;
                }
                else if (xValue == ordinateData[N].X)
                {
                    return N;
                }
                else if (xValue > ordinateData[N].X)
                {
                    return N + 1;
                }
            }
            else if (xValue > ordinateData[0].X)
            {
                return -1;
            }
            else if (xValue == ordinateData[0].X)
            {
                return 0;
            }
            else if (xValue == ordinateData[N].X)
            {
                return N;
            }
            else if (xValue < ordinateData[N].X)
            {
                return N + 1;
            }

            // perform linear search method
            if (order == SortOrder.Ascending)
            {
                for (int i = searchStart; i <= N; i++)
                {
                    if (xValue <= ordinateData[i].X)
                    {
                        XLO = i - 1;
                        break;
                    }
                }
            }
            else
            {
                for (int i = searchStart; i <= N; i++)
                {
                    if (xValue >= ordinateData[i].X)
                    {
                        XLO = i - 1;
                        break;
                    }
                }
            }
            // Return XLO
            return XLO;
        }

        /// <summary>
        /// Searches for the lower bound of the location of a value using a bisection search method.
        /// </summary>
        /// <param name="value">The value to search for.</param>
        /// <param name="valueArray">Array of values.</param>
        /// <param name="searchStart">Optional. Location to start the search of the arrays. Default = 0.</param>
        /// <param name="order">Optional. Ascending or descending. Default = ASC.</param>
        /// <returns>
        /// The lower bound of the location to be used in interpolation. E.g., X1 is the lower, X2 is the upper (X1 + 1).
        /// </returns>
        public static int BisectionSearch(double value, IList<double> valueArray, int searchStart = 0, SortOrder order = SortOrder.Ascending)
        {

            // variables
            int N = valueArray.Count - 1;
            double X = value;
            int XLO;
            int XHI;
            int XM;
            bool ASCND;

            // validate
            if (searchStart < 0)
            {
                throw new ArgumentOutOfRangeException("searchStart", "The search starting point must be non-negative.");
            }
            else if (searchStart > N)
            {
                throw new ArgumentOutOfRangeException("searchStart", "The search starting point cannot be greater than the length of the X array.");
            }

            // check if the order is ascending, if the X is outside of the range of the array
            if (order == SortOrder.Ascending)
            {
                ASCND = true;
                if (value < valueArray[0])
                {
                    return -1;
                }
                else if (value == valueArray[0])
                {
                    return 0;
                }
                else if (value == valueArray[N])
                {
                    return N;
                }
                else if (value > valueArray[N])
                {
                    return N + 1;
                }
            }
            else
            {
                ASCND = false;
                if (value > valueArray[0])
                {
                    return -1;
                }
                else if (value == valueArray[0])
                {
                    return 0;
                }
                else if (value == valueArray[N])
                {
                    return N;
                }
                else if (value < valueArray[N])
                {
                    return N + 1;
                }
            }

            // Perform bisection search
            // Initialize lower and upper limits
            XLO = searchStart;
            XHI = N;
            while (XHI - XLO > 1)
            {
                XM = XLO + (XHI - XLO >> 1); // (XHI + XLO) \ 2
                if (X >= valueArray[XM] && ASCND == true)
                {
                    XLO = XM;
                }
                else
                {
                    XHI = XM;
                }
            }
            // Return XLO
            return XLO;
        }

        /// <summary>
        /// Searches for the lower bound of the location of a value using a bisection search method.
        /// </summary>
        /// <param name="xValue">The value to search for.</param>
        /// <param name="ordinateData">List of ordinates.</param>
        /// <param name="searchStart">Optional. Location to start the search of the arrays. Default = 0.</param>
        /// <param name="order">Optional. Ascending or descending. Default = ASC.</param>
        /// <returns>
        /// The lower bound of the location to be used in interpolation. E.g., X1 is the lower, X2 is the upper (X1 + 1).
        /// </returns>
        public static int BisectionSearch(double xValue, IList<Ordinate> ordinateData, int searchStart = 0, SortOrder order = SortOrder.Ascending)
        {

            // variables
            int N = ordinateData.Count - 1;
            double X = xValue;
            int XLO;
            int XHI;
            int XM;
            bool ASCND;

            // validate
            if (searchStart < 0)
            {
                throw new ArgumentOutOfRangeException("searchStart", "The search starting point must be non-negative.");
            }
            else if (searchStart > N)
            {
                throw new ArgumentOutOfRangeException("searchStart", "The search starting point cannot be greater than the length of the X array.");
            }

            // check if the order is ascending, if the X is outside of the range of the array
            if (order == SortOrder.Ascending)
            {
                ASCND = true;
                if (xValue < ordinateData[0].X)
                {
                    return -1;
                }
                else if (xValue == ordinateData[0].X)
                {
                    return 0;
                }
                else if (xValue == ordinateData[N].X)
                {
                    return N;
                }
                else if (xValue > ordinateData[N].X)
                {
                    return N + 1;
                }
            }
            else
            {
                ASCND = false;
                if (xValue > ordinateData[0].X)
                {
                    return -1;
                }
                else if (xValue == ordinateData[0].X)
                {
                    return 0;
                }
                else if (xValue == ordinateData[N].X)
                {
                    return N;
                }
                else if (xValue < ordinateData[N].X)
                {
                    return N + 1;
                }
            }

            // Perform bisection search
            // Initialize lower and upper limits
            XLO = searchStart;
            XHI = N;
            while (XHI - XLO > 1)
            {
                XM = XLO + (XHI - XLO >> 1); // (XHI + XLO) \ 2
                if (X >= ordinateData[XM].X && ASCND == true)
                {
                    XLO = XM;
                }
                else
                {
                    XHI = XM;
                }
            }
            // Return XLO
            return XLO;
        }

        /// <summary>
        /// Searches for the lower bound of the location of a value using the HUNT method.
        /// </summary>
        /// <param name="value">The value to search for.</param>
        /// <param name="valueArray">Array of values.</param>
        /// <param name="searchStart">Optional. Location to start the search of the arrays. Default = 0.</param>
        /// <param name="order">Optional. Ascending or descending. Default = ASC.</param>
        /// <returns>
        /// The lower bound of the location to be used in interpolation. E.g., X1 is the lower, X2 is the upper (X1 + 1).
        /// </returns>
        /// <remarks>
        /// Given an array of X and given a value of X, returns a location J such that X is between XArray(J) and XArray(J+1). The XArray must be
        /// monotonic, either increasing or decreasing. J = 0 or J = XArray.Length is returned to indicate that X is out of range. The SearchStart is
        /// taken as the initial guess for J on output.
        /// </remarks>
        public static int HuntSearch(double value, IList<double> valueArray, int searchStart = 0, SortOrder order = SortOrder.Ascending)
        {
            int N = valueArray.Count - 1;
            double X = value;
            int XLO = searchStart;
            int XHI;
            int XM;
            bool ASCND;
            int INC;

            // validate
            if (searchStart < 0)
            {
                throw new ArgumentOutOfRangeException("searchStart", "The search starting point must be non-negative.");
            }
            else if (searchStart > N)
            {
                throw new ArgumentOutOfRangeException("searchStart", "The search starting point cannot be greater than the length of the X array.");
            }

            // check if the order is ascending, if the X is outside of the range of the array
            if (order == SortOrder.Ascending)
            {
                ASCND = true;
                if (value < valueArray[0])
                {
                    return -1;
                }
                else if (value == valueArray[0])
                {
                    return 0;
                }
                else if (value == valueArray[N])
                {
                    return N;
                }
                else if (value > valueArray[N])
                {
                    return N + 1;
                }
            }
            else
            {
                ASCND = false;
                if (value > valueArray[0])
                {
                    return -1;
                }
                else if (value == valueArray[0])
                {
                    return 0;
                }
                else if (value == valueArray[N])
                {
                    return N;
                }
                else if (value < valueArray[N])
                {
                    return N + 1;
                }
            }

            // XLO is defined by the search start
            XLO = searchStart;

            // Perform the hunt search algorithm
            if (XLO <= 0 | XLO > N)
            {
                // The input guess is not useful.
                // Go immediately to bisection. 
                XLO = 0;
                XHI = N + 1;
            }
            else
            {
                // Set the hunting increment. 
                INC = 1;
                if (X >= valueArray[XLO] == ASCND)
                {
                    // Hunt up
                    XHI = XLO + 1;
                    while (X >= valueArray[XHI] == ASCND)
                    {
                        // Not done hunting so double the increment
                        XLO = XHI;
                        INC += INC;
                        XHI = XLO + INC;
                        if (XHI > N)
                        {
                            // Done hunting since of end of table
                            XHI = N + 1;
                            break;
                        }
                    }
                }
                // Done hunting, value bracketed. 
                else
                {
                    // Hunt down
                    XHI = XLO;
                    XHI = XLO - 1;
                    while (X < valueArray[XLO] == ASCND)
                    {
                        // Not done hunting so double the increment
                        XHI = XLO;
                        INC += INC;
                        XLO = XHI - INC;
                        if (XLO < 1)
                        {
                            // Done hunting since of end of table
                            XLO = 0;
                            break;
                        }
                    }
                    // Done hunting, value bracketed. 
                }
            }
            // Hunt is done, so begin the final bisection
            while (XHI - XLO > 1)
            {
                XM = XLO + (XHI - XLO >> 1);
                if (X >= valueArray[XM] && ASCND == true)
                {
                    XLO = XM;
                }
                else
                {
                    XHI = XM;
                }
            }
            // Return XLO
            return XLO;
        }

        /// <summary>
        /// Searches for the lower bound of the location of a value using the HUNT method.
        /// </summary>
        /// <param name="xValue">The value to search for.</param>
        /// <param name="orderedPairedData">Ordered paired data.</param>
        /// <param name="searchStart">Optional. Location to start the search of the arrays. Default = 0.</param>
        /// <returns>
        /// The lower bound of the location to be used in interpolation. E.g., X1 is the lower, X2 is the upper (X1 + 1).
        /// </returns>
        /// <remarks>
        /// Given an array of X and given a value of X, returns a location J such that X is between XArray(J) and XArray(J+1). The XArray must be
        /// monotonic, either increasing or decreasing. J = 0 or J = XArray.Length is returned to indicate that X is out of range. The SearchStart is
        /// taken as the initial guess for J on output.
        /// </remarks>
        public static int HuntSearch(double xValue, OrderedPairedData orderedPairedData, int searchStart = 0)
        {
            int N = orderedPairedData.Count - 1;
            double X = xValue;
            int XLO = searchStart;
            int XHI;
            int XM;
            var ASCND = default(bool);
            int INC;

            // validate
            if (searchStart < 0)
            {
                throw new ArgumentOutOfRangeException("searchStart", "The search starting point must be non-negative.");
            }
            else if (searchStart > N)
            {
                throw new ArgumentOutOfRangeException("searchStart", "The search starting point cannot be greater than the length of the X array.");
            }

            // check if the order is ascending, if the X is outside of the range of the array
            if (orderedPairedData.OrderX == SortOrder.Ascending)
            {
                if (xValue < orderedPairedData[0].X)
                {
                    return -1;
                }
                else if (xValue == orderedPairedData[0].X)
                {
                    return 0;
                }
                else if (xValue == orderedPairedData[N].X)
                {
                    return N;
                }
                else if (xValue > orderedPairedData[N].X)
                {
                    return N + 1;
                }
            }
            else if (xValue > orderedPairedData[0].X)
            {
                return -1;
            }
            else if (xValue == orderedPairedData[0].X)
            {
                return 0;
            }
            else if (xValue == orderedPairedData[N].X)
            {
                return N;
            }
            else if (xValue < orderedPairedData[N].X)
            {
                return N + 1;
            }

            // XLO is defined by the search start
            XLO = searchStart;

            // Perform the hunt search algorithm
            if (XLO <= 0 | XLO > N)
            {
                // The input guess is not useful.
                // Go immediately to bisection. 
                XLO = 0;
                XHI = N + 1;
            }
            else
            {
                // Set the hunting increment. 
                INC = 1;
                if (X >= orderedPairedData[XLO].X == ASCND)
                {
                    // Hunt up
                    XHI = XLO + 1;
                    while (X >= orderedPairedData[XHI].X == ASCND)
                    {
                        // Not done hunting so double the increment
                        XLO = XHI;
                        INC += INC;
                        XHI = XLO + INC;
                        if (XHI > N)
                        {
                            // Done hunting since of end of table
                            XHI = N + 1;
                            break;
                        }
                    }
                }
                // Done hunting, value bracketed. 
                else
                {
                    // Hunt down
                    XHI = XLO;
                    XHI = XLO - 1;
                    while (X < orderedPairedData[XLO].X == ASCND)
                    {
                        // Not done hunting so double the increment
                        XHI = XLO;
                        INC += INC;
                        XLO = XHI - INC;
                        if (XLO < 1)
                        {
                            // Done hunting since of end of table
                            XLO = 0;
                            break;
                        }
                    }
                    // Done hunting, value bracketed. 
                }
            }
            // Hunt is done, so begin the final bisection
            while (XHI - XLO > 1)
            {
                XM = XLO + (XHI - XLO >> 1);
                if (X >= orderedPairedData[XM].X && ASCND == true)
                {
                    XLO = XM;
                }
                else
                {
                    XHI = XM;
                }
            }
            // Return XLO
            return XLO;
        }

        /// <summary>
        /// Searches for the lower bound of the location of a value using the HUNT method.
        /// </summary>
        /// <param name="xValue">The value to search for.</param>
        /// <param name="ordinateData">The list of ordinates.</param>
        /// <param name="searchStart">Optional. Location to start the search of the arrays. Default = 0.</param>
        /// <param name="order">Optional. Ascending or descending. Default = ASC.</param>
        /// <returns>
        /// The lower bound of the location to be used in interpolation. E.g., X1 is the lower, X2 is the upper (X1 + 1).
        /// </returns>
        /// <remarks>
        /// Given an array of X and given a value of X, returns a location J such that X is between XArray(J) and XArray(J+1). The XArray must be
        /// monotonic, either increasing or decreasing. J = 0 or J = XArray.Length is returned to indicate that X is out of range. The SearchStart is
        /// taken as the initial guess for J on output.
        /// </remarks>
        public static int HuntSearch(double xValue, IList<Ordinate> ordinateData, int searchStart = 0, SortOrder order = SortOrder.Ascending)
        {
            int N = ordinateData.Count - 1;
            double X = xValue;
            int XLO = searchStart;
            int XHI;
            int XM;
            bool ASCND;
            int INC;

            // validate
            if (searchStart < 0)
            {
                throw new ArgumentOutOfRangeException("searchStart", "The search starting point must be non-negative.");
            }
            else if (searchStart > N)
            {
                throw new ArgumentOutOfRangeException("searchStart", "The search starting point cannot be greater than the length of the X array.");
            }

            // check if the order is ascending, if the X is outside of the range of the array
            if (order == SortOrder.Ascending)
            {
                ASCND = true;
                if (xValue < ordinateData[0].X)
                {
                    return -1;
                }
                else if (xValue == ordinateData[0].X)
                {
                    return 0;
                }
                else if (xValue == ordinateData[N].X)
                {
                    return N;
                }
                else if (xValue > ordinateData[N].X)
                {
                    return N + 1;
                }
            }
            else
            {
                ASCND = false;
                if (xValue > ordinateData[0].X)
                {
                    return -1;
                }
                else if (xValue == ordinateData[0].X)
                {
                    return 0;
                }
                else if (xValue == ordinateData[N].X)
                {
                    return N;
                }
                else if (xValue < ordinateData[N].X)
                {
                    return N + 1;
                }
            }

            // XLO is defined by the search start
            XLO = searchStart;

            // Perform the hunt search algorithm
            if (XLO <= 0 | XLO > N)
            {
                // The input guess is not useful.
                // Go immediately to bisection. 
                XLO = 0;
                XHI = N + 1;
            }
            else
            {
                // Set the hunting increment. 
                INC = 1;
                if (X >= ordinateData[XLO].X == ASCND)
                {
                    // Hunt up
                    XHI = XLO + 1;
                    while (X >= ordinateData[XHI].X == ASCND)
                    {
                        // Not done hunting so double the increment
                        XLO = XHI;
                        INC += INC;
                        XHI = XLO + INC;
                        if (XHI > N)
                        {
                            // Done hunting since of end of table
                            XHI = N + 1;
                            break;
                        }
                    }
                }
                // Done hunting, value bracketed. 
                else
                {
                    // Hunt down
                    XHI = XLO;
                    XHI = XLO - 1;
                    while (X < ordinateData[XLO].X == ASCND)
                    {
                        // Not done hunting so double the increment
                        XHI = XLO;
                        INC += INC;
                        XLO = XHI - INC;
                        if (XLO < 1)
                        {
                            // Done hunting since of end of table
                            XLO = 0;
                            break;
                        }
                    }
                    // Done hunting, value bracketed. 
                }
            }
            // Hunt is done, so begin the final bisection
            while (XHI - XLO > 1)
            {
                XM = XLO + (XHI - XLO >> 1);
                if (X >= ordinateData[XM].X && ASCND == true)
                {
                    XLO = XM;
                }
                else
                {
                    XHI = XM;
                }
            }
            // Return XLO
            return XLO;
        }

        /// <summary>
        /// Performs linear interpolation.
        /// </summary>
        /// <param name="X">Interpolate Y given X.</param>
        /// <param name="X1">Lower X.</param>
        /// <param name="X2">Upper X.</param>
        /// <param name="Y1">Lower Y.</param>
        /// <param name="Y2">Upper Y.</param>
        /// <returns>
        /// Y given X, X1, X2, Y1, Y2.
        /// </returns>
        public static double LinearInterpolateFunction(double X, double X1, double X2, double Y1, double Y2)
        {
            return Y1 + (X - X1) / (X2 - X1) * (Y2 - Y1);
        }

        /// <summary>
        /// Performs linear extrapolation.
        /// </summary>
        /// <param name="X">Extrapolate Y given X.</param>
        /// <param name="X1">Lower X.</param>
        /// <param name="X2">Upper X.</param>
        /// <param name="Y1">Lower Y.</param>
        /// <param name="Y2">Upper Y.</param>
        /// <returns>
        /// Y given X, X1, X2, Y1, Y2.
        /// </returns>
        public static double LinearExtrapolateFunction(double X, double X1, double X2, double Y1, double Y2)
        {
            return Y1 - (X1 - X) * (Y2 - Y1) / (X2 - X1);
        }

        /// <summary>
        /// Performs linear interpolation based on an array of X's and array of Y's
        /// </summary>
        /// <param name="X"> Interpolate Y given X.</param>
        /// <param name="XArray">Array of X values.</param>
        /// <param name="YArray">Array of Y values.</param>
        /// <param name="searchStart">Optional. Location to start the search of the Arrays. Default = 1.</param>
        /// <param name="searchMethod">Optional. Linear, bisection, or hunt. Default = Linear.</param>
        /// <param name="XTransform">Optional. Linear, logarithmic, or normal. Default = Linear.</param>
        /// <param name="YTransform">Optional. Linear, logarithmic, or normal. Default = Linear.</param>
        /// <param name="Order">Optional. Ascending or descending order. Default = ASC.</param>
        /// <param name="Extrap">Optional. Extrapolate or don't. Default = No.</param>
        /// <returns>
        /// Y given X, from array of X and Y values.
        /// </returns>
        /// <remarks>
        /// Nonlinear X and Y data can be transformed to improve accuracy.
        /// </remarks>
        public static double Linear(double X, IList<double> XArray, IList<double> YArray,
                                    OptionalOut<int> searchStart = null, SearchMethod searchMethod = SearchMethod.Linear, 
                                    Transform XTransform = Transform.None, Transform YTransform = Transform.None, 
                                    SortOrder Order = SortOrder.Ascending, Extrapolate Extrap = Extrapolate.No)
        {
            // Variables
            int N = XArray.Count - 1;
            bool outOfRange = false;
            var Y = default(double);
            var X1 = default(double);
            var X2 = default(double);
            var Y1 = default(double);
            var Y2 = default(double);
            if (searchStart == null) { searchStart = new OptionalOut<int>() { Result = 0 }; }

            // Validate 
            if (YArray.Count != XArray.Count)
            {
                throw new ArgumentOutOfRangeException("XArray", "The X and Y arrays must be the same length.");
            }

            // Search for XLO location
            int XLO = 0;
            if (searchMethod == SearchMethod.Linear)
            {
                XLO = LinearSearch(X, XArray, searchStart.Result, Order);
            }
            else if (searchMethod == SearchMethod.Bisection)
            {
                XLO = BisectionSearch(X, XArray, searchStart.Result, Order);
            }
            else if (searchMethod == SearchMethod.Hunt)
            {
                XLO = HuntSearch(X, XArray, searchStart.Result, Order);
            }

            if (XLO == N)
            {
                XLO = N - 1;
            }
            else if (XLO == -1)
            {
                outOfRange = true;
                XLO = 0;
            }
            else if (XLO == N + 1)
            {
                outOfRange = true;
                XLO = N - 1;
            }

            searchStart.Result = XLO;
            int XHI = XLO + 1;

            // Get X transform
            if (XTransform == Transform.None)
            {
                X1 = XArray[XLO];
                X2 = XArray[XHI];
            }
            else if (XTransform == Transform.Logarithmic)
            {
                X = Math.Log10(X);
                X1 = Math.Log10(XArray[XLO]);
                X2 = Math.Log10(XArray[XHI]);
            }
            else if (XTransform == Transform.NormalZ)
            {
                X = Normal.StandardZ(X);
                X1 = Normal.StandardZ(XArray[XLO]);
                X2 = Normal.StandardZ(XArray[XHI]);
            }

            // Get Y transform
            if (YTransform == Transform.None)
            {
                Y1 = YArray[XLO];
                Y2 = YArray[XHI];
            }
            else if (YTransform == Transform.Logarithmic)
            {
                Y1 = Math.Log10(YArray[XLO]);
                Y2 = Math.Log10(YArray[XHI]);
            }
            else if (YTransform == Transform.NormalZ)
            {
                Y1 = Normal.StandardZ(YArray[XLO]);
                Y2 = Normal.StandardZ(YArray[XHI]);
            }

            // Check if the X is outside of the range of the array
            if (outOfRange == true)
            {
                if (Extrap == Extrapolate.No)
                {
                    // Y = first or last Y value
                    if (XLO == 0)
                    {
                        Y = YArray[XLO];
                    }
                    else
                    {
                        Y = YArray[XHI];
                    }
                    // Return Y
                    return Y;
                }
                else if (Extrap == Extrapolate.Yes)
                {
                    // Extrapolate Y
                    if (YTransform == Transform.None)
                    {
                        Y = LinearExtrapolateFunction(X, X1, X2, Y1, Y2);
                    }
                    else if (YTransform == Transform.Logarithmic)
                    {
                        Y = Math.Pow(10d, LinearExtrapolateFunction(X, X1, X2, Y1, Y2));
                    }
                    else if (YTransform == Transform.NormalZ)
                    {
                        Y = Normal.StandardCDF(LinearExtrapolateFunction(X, X1, X2, Y1, Y2));
                    }
                    // Return Y
                    return Y;
                }
            }

            // X must be inside the range of the array, so interpolate
            if (YTransform == Transform.None)
            {
                Y = LinearInterpolateFunction(X, X1, X2, Y1, Y2);
            }
            else if (YTransform == Transform.Logarithmic)
            {
                Y = Math.Pow(10d, LinearInterpolateFunction(X, X1, X2, Y1, Y2));
            }
            else if (YTransform == Transform.NormalZ)
            {
                Y = Normal.StandardCDF(LinearInterpolateFunction(X, X1, X2, Y1, Y2));
            }
            // Return Y
            return Y;
        }

        /// <summary>
        /// Performs linear interpolation based on an ordered paired data.
        /// </summary>
        /// <param name="X">Interpolate Y given X.</param>
        /// <param name="orderedPairedData">Ordered Paired data.</param>
        /// <param name="searchStart">Optional. Location to start the search of the Arrays. Default = 1.</param>
        /// <param name="searchMethod">Optional. Linear, bisection, or hunt. Default = Linear.</param>
        /// <param name="xTransform">Optional. Linear, logarithmic, or normal. Default = Linear.</param>
        /// <param name="yTransform">Optional. Linear, logarithmic, or normal. Default = Linear.</param>
        /// <param name="extrap">Optional. Extrapolate or don't. Default = No.</param>
        /// <returns>
        /// Y given X, from array of X and Y values.
        /// </returns>
        /// <remarks>
        /// Nonlinear X and Y data can be transformed to improve accuracy.
        /// </remarks>
        public static double Linear(double X, OrderedPairedData orderedPairedData,
                                    OptionalOut<int> searchStart = null, SearchMethod searchMethod = SearchMethod.Linear, 
                                    Transform xTransform = Transform.None, Transform yTransform = Transform.None, 
                                    Extrapolate extrap = Extrapolate.No)
        {
            // Variables
            int N = orderedPairedData.Count - 1;
            var Y = default(double);
            bool outOfRange = false;
            if (searchStart == null) { searchStart = new OptionalOut<int>() { Result = 0 }; }

            // Validate 
            if (orderedPairedData.OrderX == SortOrder.None)
                throw new ArgumentOutOfRangeException("xyData", "x values must be in a sorted order.");

            // Search for XLO location
            var XLO = default(int);
            if (searchMethod == SearchMethod.Linear)
            {
                XLO = LinearSearch(X, orderedPairedData, searchStart.Result);
            }
            else if (searchMethod == SearchMethod.Bisection)
            {
                XLO = orderedPairedData.BinarySearchX(X); // BisectionSearch(x, XArray, searchStart, order)
                if (XLO < -1)
                {
                    XLO = -1 * XLO - 2;
                }
                else if (XLO == -1)
                {
                    XLO = 0;
                }
            }
            else if (searchMethod == SearchMethod.Hunt)
            {
                XLO = HuntSearch(X, orderedPairedData, searchStart.Result);
            }

            if (XLO == N)
            {
                XLO = N - 1;
            }
            else if (XLO == -1)
            {
                outOfRange = true;
                XLO = 0;
            }
            else if (XLO == N + 1)
            {
                outOfRange = true;
                XLO = N - 1;
            }

            searchStart.Result = XLO;
            int XHI = XLO + 1;

            // Transform ordinates.
            var p1 = orderedPairedData[XLO].Transform(xTransform, yTransform);
            var p2 = orderedPairedData[XHI].Transform(xTransform, yTransform);

            // Transform X value
            if (xTransform == Transform.Logarithmic)
            {
                X = Math.Log10(X);
            }
            else if (xTransform == Transform.NormalZ)
            {
                X = Normal.StandardZ(X);
            }

            // Check if the X is outside of the range of the array
            if (outOfRange == true)
            {
                if (extrap == Extrapolate.No)
                {
                    // Y = first or last Y value
                    if (XLO == 0)
                    {
                        Y = orderedPairedData[XLO].Y;
                    }
                    else
                    {
                        Y = orderedPairedData[XHI].Y;
                    }
                    // Return Y
                    return Y;
                }
                else if (extrap == Extrapolate.Yes)
                {
                    // Extrapolate Y
                    if (yTransform == Transform.None)
                    {
                        Y = LinearExtrapolateFunction(X, p1.X, p2.X, p1.Y, p2.Y);
                    }
                    else if (yTransform == Transform.Logarithmic)
                    {
                        Y = Math.Pow(10d, LinearExtrapolateFunction(X, p1.X, p2.X, p1.Y, p2.Y));
                    }
                    else if (yTransform == Transform.NormalZ)
                    {
                        Y = Normal.StandardCDF(LinearExtrapolateFunction(X, p1.X, p2.X, p1.Y, p2.Y));
                    }
                    // Return Y
                    return Y;
                }
            }

            // X must be inside the range of the array, so interpolate
            if (yTransform == Transform.None)
            {
                Y = LinearInterpolateFunction(X, p1.X, p2.X, p1.Y, p2.Y);
            }
            else if (yTransform == Transform.Logarithmic)
            {
                Y = Math.Pow(10d, LinearInterpolateFunction(X, p1.X, p2.X, p1.Y, p2.Y));
            }
            else if (yTransform == Transform.NormalZ)
            {
                Y = Normal.StandardCDF(LinearInterpolateFunction(X, p1.X, p2.X, p1.Y, p2.Y));
            }
            // Return Y
            return Y;
        }

        /// <summary>
        /// Performs linear interpolation based on an array of X's and array of Y's
        /// </summary>
        /// <param name="X"> Interpolate Y given X. </param>
        /// <param name="ordinateData">List of ordinates </param>
        /// <param name="searchStart">Optional. Location to start the search of the Arrays. Default = 1. </param>
        /// <param name="searchMethod">Optional. Linear, bisection, or hunt. Default = Linear. </param>
        /// <param name="XTransform">Optional. Linear, logarithmic, or normal. Default = Linear. </param>
        /// <param name="YTransform">Optional. Linear, logarithmic, or normal. Default = Linear. </param>
        /// <param name="Order">Optional. Ascending or descending order. Default = ASC.</param>
        /// <param name="Extrap">Optional. Extrapolate or don't. Default = No. </param>
        /// <returns>
        /// Y given X, from array of X and Y values.
        /// </returns>
        /// <remarks>
        /// Nonlinear X and Y data can be transformed to improve accuracy.
        /// </remarks>
        public static double Linear(double X, IList<Ordinate> ordinateData,
                                    OptionalOut<int> searchStart = null, SearchMethod searchMethod = SearchMethod.Linear, 
                                    Transform XTransform = Transform.None, Transform YTransform = Transform.None, 
                                    SortOrder Order = SortOrder.Ascending, Extrapolate Extrap = Extrapolate.No)
        {
            // Variables
            int N = ordinateData.Count - 1;
            bool outOfRange = false;
            var Y = default(double);
            var X1 = default(double);
            var X2 = default(double);
            var Y1 = default(double);
            var Y2 = default(double);
            if (searchStart == null) { searchStart = new OptionalOut<int>() { Result = 0 }; }

            // Search for XLO location
            var XLO = default(int);
            if (searchMethod == SearchMethod.Linear)
            {
                XLO = LinearSearch(X, ordinateData, searchStart.Result, Order);
            }
            else if (searchMethod == SearchMethod.Bisection)
            {
                XLO = BisectionSearch(X, ordinateData, searchStart.Result, Order);
            }
            else if (searchMethod == SearchMethod.Hunt)
            {
                XLO = HuntSearch(X, ordinateData, searchStart.Result, Order);
            }

            if (XLO == N)
            {
                XLO = N - 1;
            }
            else if (XLO == -1)
            {
                outOfRange = true;
                XLO = 0;
            }
            else if (XLO == N + 1)
            {
                outOfRange = true;
                XLO = N - 1;
            }

            searchStart.Result = XLO;
            int XHI = XLO + 1;

            // Get X transform
            if (XTransform == Transform.None)
            {
                X1 = ordinateData[XLO].X;
                X2 = ordinateData[XHI].X;
            }
            else if (XTransform == Transform.Logarithmic)
            {
                X = Math.Log10(X);
                X1 = Math.Log10(ordinateData[XLO].X);
                X2 = Math.Log10(ordinateData[XHI].X);
            }
            else if (XTransform == Transform.NormalZ)
            {
                X = Normal.StandardZ(X);
                X1 = Normal.StandardZ(ordinateData[XLO].X);
                X2 = Normal.StandardZ(ordinateData[XHI].X);
            }

            // Get Y transform
            if (YTransform == Transform.None)
            {
                Y1 = ordinateData[XLO].Y;
                Y2 = ordinateData[XHI].Y;
            }
            else if (YTransform == Transform.Logarithmic)
            {
                Y1 = Math.Log10(ordinateData[XLO].Y);
                Y2 = Math.Log10(ordinateData[XHI].Y);
            }
            else if (YTransform == Transform.NormalZ)
            {
                Y1 = Normal.StandardZ(ordinateData[XLO].Y);
                Y2 = Normal.StandardZ(ordinateData[XHI].Y);
            }

            // Check if the X is outside of the range of the array
            if (outOfRange == true)
            {
                if (Extrap == Extrapolate.No)
                {
                    // Y = first or last Y value
                    if (XLO == 0)
                    {
                        Y = ordinateData[XLO].Y;
                    }
                    else
                    {
                        Y = ordinateData[XHI].Y;
                    }
                    // Return Y
                    return Y;
                }
                else if (Extrap == Extrapolate.Yes)
                {
                    // Extrapolate Y
                    if (YTransform == Transform.None)
                    {
                        Y = LinearExtrapolateFunction(X, X1, X2, Y1, Y2);
                    }
                    else if (YTransform == Transform.Logarithmic)
                    {
                        Y = Math.Pow(10d, LinearExtrapolateFunction(X, X1, X2, Y1, Y2));
                    }
                    else if (YTransform == Transform.NormalZ)
                    {
                        Y = Normal.StandardCDF(LinearExtrapolateFunction(X, X1, X2, Y1, Y2));
                    }
                    // Return Y
                    return Y;
                }
            }

            // X must be inside the range of the array, so interpolate
            if (YTransform == Transform.None)
            {
                Y = LinearInterpolateFunction(X, X1, X2, Y1, Y2);
            }
            else if (YTransform == Transform.Logarithmic)
            {
                Y = Math.Pow(10d, LinearInterpolateFunction(X, X1, X2, Y1, Y2));
            }
            else if (YTransform == Transform.NormalZ)
            {
                Y = Normal.StandardCDF(LinearInterpolateFunction(X, X1, X2, Y1, Y2));
            }
            // Return Y
            return Y;
        }

        /// <summary>
        /// Performs bilinear interpolation.
        /// </summary>
        /// <param name="X">The X point to perform interpolation.</param>
        /// <param name="Y">The Y point to perform interpolation.</param>
        /// <param name="X1">Lower X.</param>
        /// <param name="X2">Upper X.</param>
        /// <param name="Y1">Lower Y.</param>
        /// <param name="Y2">Upper Y.</param>
        /// <param name="Z11">The Z value at X1, Y1.</param>
        /// <param name="Z12">The Z value at X1, Y2.</param>
        /// <param name="Z21">The Z value at X2, Y1.</param>
        /// <param name="Z22">The Z value at X2, Y2.</param>
        /// <returns>
        /// Z given X, Y, X1, X2, Y1, Y2, Z11, Z12, Z21, Z22.
        /// </returns>
        public static double BilinearInterpolateFunction(double X, double Y, double X1, double X2, double Y1, double Y2, double Z11, double Z12, double Z21, double Z22)
        {
            double F1 = (X2 - X) * (Y2 - Y) / ((X2 - X1) * (Y2 - Y1));
            double F2 = (X - X1) * (Y2 - Y) / ((X2 - X1) * (Y2 - Y1));
            double F3 = (X2 - X) * (Y - Y1) / ((X2 - X1) * (Y2 - Y1));
            double F4 = (X - X1) * (Y - Y1) / ((X2 - X1) * (Y2 - Y1));
            double z = F1 * Z11 + F2 * Z21 + F3 * Z12 + F4 * Z22;
            return F1 * Z11 + F2 * Z21 + F3 * Z12 + F4 * Z22;
        }

        /// <summary>
        /// Preforms bilinear interpolation based on an array of X's, Y's, and Z's.
        /// </summary>
        /// <param name="X">The X point to perform interpolation.</param>
        /// <param name="Y">The Y point to perform interpolation.</param>
        /// <param name="XArray">Array of X values. </param>
        /// <param name="YArray">Array of Y values. </param>
        /// <param name="ZArray">2-Column array of Z values. The first column is the same length as the Y array, and the second column is the same length as the X array.</param>
        /// <param name="searchStartX">Optional. Location to start the search of the X array. Default = 1. </param>
        /// <param name="searchStartY">Optional. Location to start the search of the Y array. Default = 1. </param>
        /// <param name="searchMethodX">Optional. Linear, bisection, or hunt. Default = Linear. </param>
        /// <param name="searchMethodY">Optional. Linear, bisection, or hunt. Default = Linear. </param>
        /// <param name="XTransform">Optional. Linear, logarithmic, or normal. Default = Linear. </param>
        /// <param name="YTransform">Optional. Linear, logarithmic, or normal. Default = Linear. </param>
        /// <param name="ZTransform">Optional. Linear, logarithmic, or normal. Default = Linear. </param>
        /// <param name="orderX">Optional. Ascending or descending order. Default = ASC.</param>
        /// <param name="orderY">Optional. Ascending or descending order. Default = ASC.</param>
        /// <returns>
        /// Z given X and Y, from array of X, Y, and Z values.
        /// </returns>
        public static double Bilinear(double X, double Y, double[] XArray, double[] YArray, double[,] ZArray,
                                        OptionalOut<int> searchStartX = null, SearchMethod searchMethodX = SearchMethod.Linear,
                                        OptionalOut<int> searchStartY = null, SearchMethod searchMethodY = SearchMethod.Linear, 
                                        Transform XTransform = Transform.None, Transform YTransform = Transform.None, Transform ZTransform = Transform.None, 
                                        SortOrder orderX = SortOrder.Ascending, SortOrder orderY = SortOrder.Ascending)
        {

            // TODO: Add extrapolation option.
            // Optional Extrapolation As Extrapolate = Extrapolate.No

            // Variables
            bool XOutOfRange = false;
            bool YOutOfRange = false;
            int XN = XArray.Count() - 1;
            int YN = YArray.Count() - 1;
            var P = default(double);
            var X1 = default(double);
            var X2 = default(double);
            var Y1 = default(double);
            var Y2 = default(double);
            var Z11 = default(double);
            var Z12 = default(double);
            var Z21 = default(double);
            var Z22 = default(double);
            if (searchStartX == null) { searchStartX = new OptionalOut<int>() { Result = 0 }; }
            if (searchStartY == null) { searchStartY = new OptionalOut<int>() { Result = 0 }; }

            // Validate 
            if (ZArray.GetLength(1) != XArray.Count())
            {
                throw new ArgumentOutOfRangeException(nameof(ZArray), "The number of columns in the Z array must be the same length as the X array.");
            }

            if (ZArray.GetLength(0) != YArray.Count())
            {
                throw new ArgumentOutOfRangeException(nameof(ZArray), "The number of rows in the Z array must be the same length as the Y array.");
            }

            if (searchStartX.Result < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(searchStartX), "The search starting point for X must be non-negative.");
            }
            else if (searchStartX.Result > XN)
            {
                throw new ArgumentOutOfRangeException(nameof(searchStartX), "The search starting point for X cannot be greater than the length of the X array.");
            }

            if (searchStartY.Result < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(searchStartY), "The search starting point for Y must be non-negative.");
            }
            else if (searchStartY.Result > YN)
            {
                throw new ArgumentOutOfRangeException(nameof(searchStartY), "The search starting point for Y cannot be greater than the length of the Y array.");
            }

            // Search for XLO location
            var XLO = default(int);
            if (searchMethodX == SearchMethod.Linear)
            {
                XLO = LinearSearch(X, XArray, searchStartX.Result, orderX);
            }
            else if (searchMethodX == SearchMethod.Bisection)
            {
                XLO = BisectionSearch(X, XArray, searchStartX.Result, orderX);
            }
            else if (searchMethodX == SearchMethod.Hunt)
            {
                XLO = HuntSearch(X, XArray, searchStartX.Result, orderX);
            }

            if (XLO == XN)
            {
                XLO = XN - 1;
            }
            else if (XLO == -1)
            {
                XOutOfRange = true;
                XLO = 0;
            }
            else if (XLO == XN + 1)
            {
                XOutOfRange = true;
                XLO = XN - 1;
            }

            searchStartX.Result = XLO;
            int XHI = XLO + 1;

            // Search for YLO location
            var YLO = default(int);
            if (searchMethodY == SearchMethod.Linear)
            {
                YLO = LinearSearch(Y, YArray, searchStartY.Result, orderY);
            }
            else if (searchMethodY == SearchMethod.Bisection)
            {
                YLO = BisectionSearch(Y, YArray, searchStartY.Result, orderY);
            }
            else if (searchMethodY == SearchMethod.Hunt)
            {
                YLO = HuntSearch(Y, YArray, searchStartY.Result, orderY);
            }

            if (YLO == YN)
            {
                YLO = YN - 1;
            }
            else if (YLO == -1)
            {
                YOutOfRange = true;
                YLO = 0;
            }
            else if (YLO == YN + 1)
            {
                YOutOfRange = true;
                YLO = YN - 1;
            }

            searchStartY.Result = YLO;
            int YHI = YLO + 1;


            // Get X transform
            if (XTransform == Transform.None)
            {
                X1 = XArray[XLO];
                X2 = XArray[XHI];
            }
            else if (XTransform == Transform.Logarithmic)
            {
                X = Math.Log10(X);
                X1 = Math.Log10(XArray[XLO]);
                X2 = Math.Log10(XArray[XHI]);
            }
            else if (XTransform == Transform.NormalZ)
            {
                X = Normal.StandardZ(X);
                X1 = Normal.StandardZ(XArray[XLO]);
                X2 = Normal.StandardZ(XArray[XHI]);
            }

            // Get Y transform
            if (YTransform == Transform.None)
            {
                Y1 = YArray[YLO];
                Y2 = YArray[YHI];
            }
            else if (YTransform == Transform.Logarithmic)
            {
                Y = Math.Log10(Y);
                Y1 = Math.Log10(YArray[YLO]);
                Y2 = Math.Log10(YArray[YHI]);
            }
            else if (YTransform == Transform.NormalZ)
            {
                Y = Normal.StandardZ(Y);
                Y1 = Normal.StandardZ(YArray[YLO]);
                Y2 = Normal.StandardZ(YArray[YHI]);
            }

            // Get Z transform
            if (ZTransform == Transform.None)
            {
                Z11 = ZArray[YLO, XLO];
                Z12 = ZArray[YHI, XLO];
                Z21 = ZArray[YLO, XHI];
                Z22 = ZArray[YHI, XHI];
            }
            else if (ZTransform == Transform.Logarithmic)
            {
                Z11 = Math.Log10(ZArray[YLO, XLO]);
                Z12 = Math.Log10(ZArray[YHI, XLO]);
                Z21 = Math.Log10(ZArray[YLO, XHI]);
                Z22 = Math.Log10(ZArray[YHI, XHI]);
            }
            else if (ZTransform == Transform.NormalZ)
            {
                Z11 = Normal.StandardZ(ZArray[YLO, XLO]);
                Z12 = Normal.StandardZ(ZArray[YHI, XLO]);
                Z21 = Normal.StandardZ(ZArray[YLO, XHI]);
                Z22 = Normal.StandardZ(ZArray[YHI, XHI]);
            }


            // Assign Z value if x or y are both out of range. 
            if (XOutOfRange == true && YOutOfRange)
            {

                // Both x and y are out of range
                // Ascending order
                if (orderX == SortOrder.Ascending && X < XArray.First() && orderY == SortOrder.Ascending && Y < YArray.First())
                {
                    return ZArray[0, 0]; // Z(Y,X)
                }

                if (orderX == SortOrder.Ascending && X < XArray.First() && orderY == SortOrder.Ascending && Y > YArray.Last())
                {
                    return ZArray[YArray.Count() - 1, 0];
                }

                if (orderX == SortOrder.Ascending && X > XArray.Last() && orderY == SortOrder.Ascending && Y < YArray.First())
                {
                    return ZArray[0, XArray.Count() - 1];
                }

                if (orderX == SortOrder.Ascending && X > XArray.Last() && orderY == SortOrder.Ascending && Y > YArray.Last())
                {
                    return ZArray[YArray.Count() - 1, XArray.Count() - 1];
                }

                // Ascending X, Descending Y order
                if (orderX == SortOrder.Ascending && X < XArray.First() && orderY == SortOrder.Descending && Y < YArray.First())
                {
                    return ZArray[0, 0];
                }

                if (orderX == SortOrder.Ascending && X < XArray.First() && orderY == SortOrder.Descending && Y > YArray.Last())
                {
                    return ZArray[YArray.Count() - 1, 0];
                }

                if (orderX == SortOrder.Ascending && X > XArray.Last() && orderY == SortOrder.Descending && Y < YArray.First())
                {
                    return ZArray[0, XArray.Count() - 1];
                }

                if (orderX == SortOrder.Ascending && X > XArray.Last() && orderY == SortOrder.Descending && Y > YArray.Last())
                {
                    return ZArray[YArray.Count() - 1, XArray.Count() - 1];
                }

                // Descending X, Ascending Y order
                if (orderX == SortOrder.Descending && X < XArray.First() && orderY == SortOrder.Ascending && Y < YArray.First())
                {
                    return ZArray[0, 0];
                }

                if (orderX == SortOrder.Descending && X < XArray.First() && orderY == SortOrder.Ascending && Y > YArray.Last())
                {
                    return ZArray[YArray.Count() - 1, 0];
                }

                if (orderX == SortOrder.Descending && X > XArray.Last() && orderY == SortOrder.Ascending && Y < YArray.First())
                {
                    return ZArray[0, XArray.Count() - 1];
                }

                if (orderX == SortOrder.Descending && X > XArray.Last() && orderY == SortOrder.Ascending && Y > YArray.Last())
                {
                    return ZArray[YArray.Count() - 1, XArray.Count() - 1];
                }

                // Descending order
                if (orderX == SortOrder.Descending && X < XArray.First() && orderY == SortOrder.Descending && Y < YArray.First())
                {
                    return ZArray[0, 0];
                }

                if (orderX == SortOrder.Descending && X < XArray.First() && orderY == SortOrder.Descending && Y > YArray.Last())
                {
                    return ZArray[YArray.Count() - 1, 0];
                }

                if (orderX == SortOrder.Descending && X > XArray.Last() && orderY == SortOrder.Descending && Y < YArray.First())
                {
                    return ZArray[0, XArray.Count() - 1];
                }

                if (orderX == SortOrder.Descending && X > XArray.Last() && orderY == SortOrder.Descending && Y > YArray.Last())
                {
                    return ZArray[YArray.Count() - 1, XArray.Count() - 1];
                }
            }

            // X is out of range
            // Interpolate Z value from first or last column if X is out of range
            if (XOutOfRange == true && YOutOfRange == false)
            {
                if (X < XArray.First())
                {
                    return LinearInterpolateFunction(Y, Y1, Y2, Z11, Z12);
                }

                if (X > XArray.Last())
                {
                    return LinearInterpolateFunction(Y, Y1, Y2, Z21, Z22);
                }
            }

            // Y is out of range
            // Interpolate Z value from first or last row if Y is out of range
            if (XOutOfRange == false && YOutOfRange == true)
            {
                if (Y < YArray.First())
                {
                    return LinearInterpolateFunction(X, X1, X2, Z11, Z21);
                }

                if (Y > YArray.Last())
                {
                    return LinearInterpolateFunction(X, X1, X2, Z12, Z22);
                }
            }


            // X and Y must be inside the range of the array, so interpolate
            if (ZTransform == Transform.None)
            {
                P = BilinearInterpolateFunction(X, Y, X1, X2, Y1, Y2, Z11, Z12, Z21, Z22);
            }
            else if (ZTransform == Transform.Logarithmic)
            {
                P = Math.Pow(10d, BilinearInterpolateFunction(X, Y, X1, X2, Y1, Y2, Z11, Z12, Z21, Z22));
            }
            else if (ZTransform == Transform.NormalZ)
            {
                P = Normal.StandardCDF(BilinearInterpolateFunction(X, Y, X1, X2, Y1, Y2, Z11, Z12, Z21, Z22));
            }
            // Return P
            return P;
        }

    }
}