/*
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
*/

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
    ///     <b> Authors: </b>
    ///     <list type="bullet"> 
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    ///     <item> Tiki Gonzalez, USACE Risk Management Center, julian.t.gonzalez@usace.army.mil </item>
    ///     </list>
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

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="array"></param>
        ///// <param name="startindex"></param>
        ///// <param name="endIndex"></param>
        ///// <param name="indicators"></param>
        ///// <returns></returns>
        //public static double[] Subset(double[] array, bool[] indicators,bool useComplement = false)
        //{
        //    var subTrue = new List<double>();
        //    var subFalse = new List<double>();
        //    //int idx = 0;

        //    for ( int i = 0; i < indicators.Length; i++)
        //    {
        //        if (indicators[i] == true)
        //        {
        //            subTrue.Add(array[i]);
        //            //idx = i;
        //            //break;
        //        }
        //        else if (indicators[i] == false)
        //        {
        //            subFalse.Add(array[i]);
        //        }
        //    }
        //    //for (int j = idx + 1; j < array.Length; j++)
        //    //{

        //    //    if (indicators[j] == (useComplement ? false : true))
        //    //    {
        //    //        sub.Add(array[j]);
        //    //    }
        //    //    idx++;
 
        //    //}
        //    if (useComplement == false)
        //    {
        //        return subFalse.ToArray();
        //    }
        //    else 
        //    {
        //        return subTrue.ToArray();
        //    }
            
        //}

        /// <summary>
        /// Specific method to extract target feature from split.
        /// </summary>
        /// <param name="split"> Split data from Iris dataset. </param>
        /// <returns></returns>
        public static double[] SubsetTarget(double[][] split)
        {
            return split[4];
        }

        /// <summary>
        /// Assuming there is 4 features and 1 target to subset for X training or testing
        /// we only extract the 4 features.
        /// </summary>
        /// <param name="split"> Split data from Iris dataset. </param>
        /// <returns>2D array of the values from the dictionary.</returns>
        public static List<double[]> SubsetFeature(double[][] split)
        {
            var feature = new List<double[]>();
            int length = split[0].Length;
            for (int i = 0; i < length;i++)
            {
                double[] valuesForFeature = new double[4];
                for(int j = 0; j < 4; j++)
                {
                    //valuesForFeature[j] = split[j][i];
                    valuesForFeature[j] = split[i][j];
                }
                feature.Add(valuesForFeature);
            }
            return feature;
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

        ///// <summary>
        ///// Splits the data into a training and testing set 70/30 respectively.
        ///// </summary>
        ///// <param name="data"> Original data or data table we want to split.</param>
        ///// <returns> The training set in a dictionary. </returns>
        //public static Dictionary<string, List<double>> SplitDataTrain(Dictionary<string, List<double>> data)
        //{
        //    var random = new Random();
        //    var train = new Dictionary<string, List<double>>();
        //    var test = new Dictionary<string, List<double>>();

        //    foreach (var dataClass in data)
        //    {
        //        train[dataClass.Key] = dataClass.Value.ToList();
        //        test[dataClass.Key] = new List<double>();

        //        for (int i = 0; i<Math.Ceiling(0.3*dataClass.Value.Count); i++)
        //        {
        //            if (train[dataClass.Key].Count == 0) break;
        //            int idx = random.Next(train[dataClass.Key].Count);
        //            test[dataClass.Key].Add(train[dataClass.Key][idx]);
        //            train[dataClass.Key].RemoveAt(idx);
        //        }
        //    }
        //    return train;
        //}

        ///// <summary>
        /////  Splits the data into a training and testing set 70/30 respectively.
        ///// </summary>
        ///// <param name="data"> Original data or data table we want to split. </param>
        ///// <returns> The testing set in a dictionary.</returns>
        //public static Dictionary<string, List<double>> SplitDataTest(Dictionary<string, List<double>> data)
        //{
        //    var random = new Random();
        //    var train = new Dictionary<string, List<double>>();
        //    var test = new Dictionary<string, List<double>>();

        //    foreach (var dataClass in data)
        //    {
        //        train[dataClass.Key] = dataClass.Value.ToList();
        //        test[dataClass.Key] = new List<double>();

        //        for (int i = 0; i < Math.Ceiling(0.3 * dataClass.Value.Count); i++)
        //        {
        //            if (train[dataClass.Key].Count == 0) break;
        //            int idx = random.Next(train[dataClass.Key].Count);
        //            test[dataClass.Key].Add(train[dataClass.Key][idx]);
        //            train[dataClass.Key].RemoveAt(idx);
        //        }
        //    }
        //    return test;
        //}

        /// <summary>
        /// Random number generates integers without replacement.
        /// </summary>
        /// <param name="random"> Random number generator.</param>
        /// <param name="min">Lower bound.</param>
        /// <param name="max"> Higher bound.</param>
        /// <param name="length">Amount of numbers to be generated.</param>
        /// <returns></returns>
        public static int[] NextNRIntegers(Random random, int min, int max, int length )
        {
            var integers = new List<int>();
            for(int i = min; i < max; i++)
            {
                integers.Add(i);
            }

            var vals = new int[length];
            for (int i = 0; i < length; i++)
            {
                var r = random.Next(0,integers.Count);
                vals[i] = integers[r];
                integers.RemoveAt(r);
            }
            return vals;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rng"> Random number generator for the indices using NextNRIntegers()</param>
        /// <param name="data"></param>
        /// <param name="testing"></param>
        /// <returns></returns>
        public static double[][] TrainTestSplit(int[] rng,int dataSize, double[][] data,bool testing = false)
        {
            // iterate through indices and then split 
            //int dataSize = data[0].Length; //amount of columns (150)
            int subSampleTraining = (int)Math.Ceiling(0.7 * dataSize); // 70% training split (105)
            int subSampleTesting = dataSize - subSampleTraining; // 45

            // Calling rng for indices with seed
            //var rand = new Random(12345);
            //var rng = NextNRIntegers(rand, 0, dataSize, dataSize);
            var indicesTraining = new int[subSampleTraining]; //70% of the rng indices to training
            for (int i = 0; i < subSampleTraining; i++)
            {
                indicesTraining[i] = rng[i];
            }
            
            var indicesTesting = new int[subSampleTesting];
            for (int i = 0; i < subSampleTesting; i++)
            {
                indicesTesting[i] = rng[i + subSampleTraining];
            }
            //var indicesTesting = rng.Except(indicesTraining).ToArray(); // allocates the other indices for testing

            var trainingData = new double[subSampleTraining][];
            var testingData = new double[subSampleTesting][];

            for (int i = 0; i < subSampleTraining; i++)
            {
                trainingData[i] = new double[5]; //5 features in the dataset
                for(int j =0; j < 5; j++)
                {
                    //trainingData[i][j] = data[indicesTraining[i]][j];
                    trainingData[i][j] = data[j][indicesTraining[i]];
                    //trainingData[i] = data[indicesTraining[i]];
                }
            }

            for(int i =0; i < subSampleTesting; i++)
            {
                testingData[i] = new double[5];
                for(int j = 0; j < 5; j++)
                {
                    //testingData[i][j]= data[indicesTesting[i]][j];
                    testingData[i][j] = data[j][indicesTesting[i]];
                    //testingData[i][j] = data[indicesTesting[i]];
                }
            }

            if (testing)
            {
                return testingData;
            }
            else
            {
                return trainingData;
            }
        }
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <returns></returns>
        //public static bool[] SampleSplit(double[][] array, double SplitRatio = 0.7)
        //{
        //    int nSamp = array.Count();
        //    //int nGroup = group.Length;
        //    bool[] bins = new bool[nSamp];
        //    SplitRatio = Math.Abs(SplitRatio);

        //    //if (nGroup > 0 && nGroup != nSamp)
        //    //    throw new ArgumentException("Arrays 'array' and 'group' have to have the same length");
        //    if (SplitRatio >= nSamp) 
        //        throw new ArgumentException("'SplitRatio' parameter has to be in [0, 1] range or [1, length(array)] range");

        //    var distinctLabels = array.Distinct();
        //    distinctLabels = distinctLabels.ToArray();
        //    int nDistinctLabels = distinctLabels.Count();

        //    if (2 * nDistinctLabels > nSamp || nDistinctLabels == 1)
        //    {
        //        if (SplitRatio >= 1)
        //        {
        //            double n = SplitRatio;
        //        }
        //        else
        //        {
        //            double n = SplitRatio * nSamp;
        //        }
        //        Random random = new Random();
        //        List<int> rnd = new List<int>(nSamp);
        //        for (int i = 0; i < rnd.Count; i++)
        //        {
        //            rnd.Add(random.Next(0, nSamp));
        //        }

        //        rnd.Sort();

        //        for (int j = 0; j < rnd.Count; j++)
        //        {
        //            bins[rnd[j]] = true;
        //        }
        //    }
        //    else
        //    {
        //        if(SplitRatio >= 1)
        //        {
        //            double rat = SplitRatio/nSamp;
        //        }
        //        else
        //        {
        //            double rat = SplitRatio;
        //        }
        //        for (int i = 0; i < nDistinctLabels; i++)
        //        {
        //            var idx = Enumerable.Range(0, array.Length)
        //                       .Where(i => array[i] == distinctLabels.ElementAt(i))
        //                       .ToArray();

        //            int n = (int)Math.Round(idx.Length * 0.5); // Adjust the ratio as needed

        //            //var idx[i] = Array.IndexOf(array, distinctLabels.ElementAt(i));
        //            //int n = (int)Math.Round(idx * 0.5);

        //            Random random = new Random();
        //            double[] rnd = idx.Select(i => random.NextDouble()).ToArray();

        //            Array.Sort(rnd, idx);

        //            for (int ii = 0; ii < n; ii++)
        //            {
        //                bins[idx[ii]] = true;
        //            }
        //        }

        //        if(SplitRatio >= 1)
        //        {
        //            double n = bins.Length - SplitRatio;

        //            if(n > 0)
        //            {
        //                var binOneIdx = Enumerable.Range(0, bins.Length).Where(i => bins[i]).ToArray();
        //                var randIdx = binOneIdx.OrderBy(i=>Guid.NewGuid()).Take((int)n).ToArray();

        //                foreach(var idx in randIdx)
        //                {
        //                    bins[idx] = false;
        //                }
        //            }
        //            else if (n < 0)
        //            {
        //                var binTwoIdx = Enumerable.Range(0, bins.Length).Where(i => !bins[i]).ToArray();
        //                var randIdx = binTwoIdx.OrderBy(_ => Guid.NewGuid()).Take(-(int)n).ToArray();

        //                foreach (var idx in randIdx)
        //                {
        //                    bins[idx] = true;
        //                }
        //            }
        //        }


        //    }
        //    return bins;
        //}
    }
}
