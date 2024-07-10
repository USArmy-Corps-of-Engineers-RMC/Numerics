using Numerics.Mathematics.LinearAlgebra;
using Numerics.Sampling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Numerics
{
    /// <summary>
    /// A class for extension methods.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public static class ExtensionMethods
    {

        /// <summary>
        /// Gets an attribute on an enum field value
        /// </summary>
        /// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
        /// <param name="enumValue">The enum value</param>
        /// <returns>The attribute of type T that exists on the enum value</returns>
        public static T GetAttributeOfType<T>(this Enum enumValue) where T : Attribute
        {
            var type = enumValue.GetType();
            var memInfo = type.GetMember(enumValue.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return (attributes.Length > 0) ? (T)attributes[0] : null;
        }

        /// <summary>
        /// Returns an array of random integers.
        /// </summary>
        /// <param name="random">A random number generator.</param>
        /// <param name="length">The number of samples to return.</param>
        public static int[] NextIntegers(this Random random, int length)
        {
            var values = new int[length];
            for (int i = 0; i < length; i++)
                values[i] = random.Next();
            return values;
        }

        /// <summary>
        /// Returns an array of random integers between a min and max value. 
        /// </summary>
        /// <param name="random">A random number generator.</param>
        /// <param name="minValue">The minimum value to sample between.</param>
        /// <param name="maxValue">The maximum value to sample between.</param>
        /// <param name="length">The number of samples to return.</param>
        public static int[] NextIntegers(this Random random, int minValue, int maxValue, int length)
        {
            var values = new int[length];
            for (int i = 0; i < length; i++)
                values[i] = random.Next(minValue, maxValue);
            return values;
        }

        /// <summary>
        /// Returns an array of random integers between a min and max value. 
        /// </summary>
        /// <param name="random">A random number generator.</param>
        /// <param name="minValue">The minimum value to sample between.</param>
        /// <param name="maxValue">The maximum value to sample between.</param>
        /// <param name="length">The number of samples to return.</param>
        /// <param name="replace">Determines whether or not to sample with replacement. Default = true.</param>
        public static int[] NextIntegers(this Random random, int minValue, int maxValue, int length, bool replace = true)
        {
            if (replace == true)
            {
                var values = new int[length];
                for (int i = 0; i < length; i++)
                    values[i] = random.Next(minValue, maxValue);
                return values;
            }
            else
            {              
                var bins = new List<int>();
                for (int i = minValue; i < maxValue; i++)
                    bins.Add(i);

                // Sample random bin without replacement
                var values = new int[length];
                for (int i = 0; i < length; i++)
                {
                    int r = random.Next(0, bins.Count);
                    values[i] = bins[r];
                    bins.RemoveAt(r);
                }
                return values;
            }

        }


        /// <summary>
        /// Returns an array of random doubles.
        /// </summary>
        /// <param name="random">A random number generator.</param>
        /// <param name="length">The number of samples to return.</param>
        public static double[] NextDoubles(this Random random, int length)
        {
            var values = new double[length];
            for (int i = 0; i < length; i++)
                values[i] = random.NextDouble();
            return values;
        }

        /// <summary>
        /// Returns a 2-D array of random doubles.
        /// </summary>
        /// <param name="random">A random number generator.</param>
        /// <param name="length">The number of samples to return.</param>
        /// <param name="dimension">The spatial dimension</param>
        public static double[,] NextDoubles(this Random random, int length, int dimension)
        {
            var values = new double[length, dimension];
            var prngs = new MersenneTwister[dimension];
            for (int i = 0; i < dimension; i++)
            {
                prngs[i] = new MersenneTwister(random.Next());
                for (int j = 0; j < length; j++)
                {
                    values[j, i] = prngs[i].NextDouble();
                }
            }
            return values;
        }


        /// <summary>
        /// Gets a specific column from a 2-D array.
        /// </summary>
        /// <typeparam name="T">The array value type.</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="index">Zero-based index of the column.</param>
        public static T[] GetColumn<T>(this T[,] array, int index)
        {
            var col = new T[array.GetLength(0)];
            for (int i = 0; i < array.GetLength(0); i++)
                col[i] = array[i, index];
            return col;
        }

        /// <summary>
        /// Gets a specific row from a 2-D array.
        /// </summary>
        /// <typeparam name="T">The array value type.</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="index">Zero-based index of the row.</param>
        public static T[] GetRow<T>(this T[,] array, int index)
        {
            var row = new T[array.GetLength(1)];
            for (int i = 0; i < array.GetLength(1); i++)
                row[i] = array[index, i];
            return row;
        }

        /// <summary>
        /// Sets a specific row in a 2-D array.
        /// </summary>
        /// <typeparam name="T">The array value type.</typeparam>
        /// <param name="array">The array</param>
        /// <param name="index">Zero-based index of the row.</param>
        /// <param name="values">The new values.</param>
        public static void SetRow<T>(this T[,] array, int index, T[] values)
        {
            for (int i = 0; i < array.GetLength(1); i++)
                array[index, i] = values[i];
        }

        /// <summary>
        /// Sets a specific column in a 2-D array.
        /// </summary>
        /// <typeparam name="T">The array value type.</typeparam>
        /// <param name="array">The array</param>
        /// <param name="index">Zero-based index of the column.</param>
        /// <param name="values">The new values.</param>
        public static void SetColumn<T>(this T[,] array, int index, T[] values)
        {
            for (int i = 0; i < array.GetLength(0); i++)
                array[i, index] = values[i];
        }

        /// <summary>
        /// Adds corresponding elements of arrays.
        /// </summary>
        /// <param name="array">The array</param>
        /// <param name="values">The values to add.</param>
        /// <returns>The array after addition.</returns>
        public static double[] Add(this double[] array, double[] values)
        {
            if (array.Length != values.Length) throw new ArgumentException(nameof(array), "The arrays must be the same length.");
            var result = new double[array.Length];
            for (int i = 0; i < array.Length; i++)
                result[i] = array[i] + values[i];
            return result;
        }

        /// <summary>
        /// Subtracts corresponding elements of arrays.
        /// </summary>
        /// <param name="array">The array</param>
        /// <param name="values">The values to subtract.</param>
        /// <returns>The array after subtraction.</returns>
        public static double[] Subtract(this double[] array, double[] values)
        {
            if (array.Length != values.Length) throw new ArgumentException(nameof(array), "The arrays must be the same length.");
            var result = new double[array.Length];
            for (int i = 0; i < array.Length; i++)
                result[i] = array[i] - values[i];
            return result;
        }

        /// <summary>
        /// Multiplies an array with a scalar.
        /// </summary>
        /// <param name="array">The array</param>
        /// <param name="scalar">The scalar to multiply by.</param>
        /// <returns>The array after multiplication.</returns>
        public static double[] Multiply(this double[] array, double scalar)
        {
            var result = new double[array.Length];
            for (int i = 0; i < array.Length; i++)
                result[i] = array[i] * scalar;
            return result;
        }

        /// <summary>
        /// Divides an array with a scalar.
        /// </summary>
        /// <param name="array">The array</param>
        /// <param name="scalar">The scalar to divide by.</param>
        /// <returns>The array after division.</returns>
        public static double[] Divide(this double[] array, double scalar)
        {
            var result = new double[array.Length];
            for (int i = 0; i < array.Length; i++)
                result[i] = array[i] / scalar;
            return result;
        }

        /// <summary>
        /// Returns a subset of the array.
        /// </summary>
        /// <typeparam name="T">The array value type.</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="startIndex">The start index.</param>
        public static T[] Subset<T>(this T[] array, int startIndex)
        {
            var sub = new List<T>();
            for (int i = startIndex; i < array.Length; i++)
            {
                sub.Add(array[i]);
            }
            return sub.ToArray();
        }

        /// <summary>
        /// Returns a subset of the array.
        /// </summary>
        /// <typeparam name="T">The array value type.</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        public static T[] Subset<T>(this T[] array, int startIndex, int endIndex)
        {
            var sub = new List<T>();
            for (int i = startIndex; i <= endIndex; i++)
            {
                sub.Add(array[i]);
            }
            return sub.ToArray();
        }

        /// <summary>
        /// Fills a 2-D array with the specified value.
        /// </summary>
        /// <typeparam name="T">The array value type.</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="value">The fill value.</param>
        public static void Fill<T>(this T[,] array, T value)
        {
            for (int i = 0; i < array.GetLength(0); i++)
                for (int j = 0; j < array.GetLength(1); j++)
                    array[i, j] = value;
        }

        /// <summary>
        /// Fills an array with the specified value.
        /// </summary>
        /// <typeparam name="T">The array value type.</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="value">The fill value.</param>
        public static void Fill<T>(this T[] array, T value)
        {
            for (int i = 0; i < array.Length; i++)
                array[i] = value;
        }


        /// <summary>
        /// Determines if the values are almost equal to each other.
        /// </summary>
        /// <param name="a">The left-side value.</param>
        /// <param name="b">The right-side value.</param>
        /// <param name="epsilon">The absolute tolerance level. Default = 1E-15.</param>
        public static bool AlmostEquals(this double a, double b, double epsilon = 1E-15)
        {
            return Math.Abs(a - b) < epsilon;
        }
    }
}
