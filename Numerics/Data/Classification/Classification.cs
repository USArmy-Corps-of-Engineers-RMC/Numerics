using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;


namespace Numerics
{
    /// <summary>
    /// Static class for various data classification methods.
    /// </summary>
    public static class Classification
    {

        /// <summary>
        /// Determines classification range break values into equally sized intervals. Returns an array of upper bound break values.
        /// </summary>
        /// <param name="data">The data to be classified.</param>
        /// <param name="nClasses">The number of classes to determine break values.</param>
        /// <param name="dataIsSorted">Boolean value indicating if the data is sorted in ascending order.</param>
        public static double[] EqualInterval(double[] data, int nClasses, bool dataIsSorted)
        {
            // 
            var sortedData = data;
            if (dataIsSorted == false)
            {
                sortedData = data.ToArray();
                Array.Sort(sortedData);
            }
            // ignore NaN or infinity
            if (double.IsInfinity(sortedData[0]) || double.IsNaN(sortedData[0]))
            {
                for (int i = 1; i < sortedData.Length; i++)
                {
                    if (double.IsInfinity(sortedData[i]) == false && double.IsNaN(sortedData[i]) == false) return EqualInterval(sortedData[i], sortedData.Last(), nClasses);
                }
                return Array.Empty<double>();
            }
            else
            {
                return EqualInterval(sortedData[0], sortedData.Last(), nClasses);
            }
        }

        /// <summary>
        /// Determines classification range break values into equally sized intervals. Returns an array of upper bound break values.
        /// </summary>
        /// <param name="minValue">Minimum value for determining classification.</param>
        /// <param name="maxValue">Maximum value for determining classification.</param>
        /// <param name="nClasses">The number of classes to determine break values for.</param>
        public static double[] EqualInterval(double minValue, double maxValue, int nClasses)
        {
            if (nClasses <= 0)
                return Array.Empty<double>();
            if (nClasses == 1)
                return new[] { maxValue };

            var result = new double[nClasses];
            double multiplier = (maxValue - minValue) / nClasses;
            for (int i = 1; i <= nClasses; i++)
                result[i - 1] = minValue + i * multiplier;
            result[nClasses - 1] = maxValue;

            return result;
        }

        /// <summary>
        /// Determines classification range break values for a predefined interval. Returns an array of upper bound break values.
        /// </summary>
        /// <param name="data">The data to be classified.</param>
        /// <param name="intervalSize">The size of the interval to classify break values.</param>
        /// <param name="dataIsSorted">Boolean value indicating if the data is sorted in ascending order.</param>
        /// <returns></returns>
        public static double[] DefinedInterval(IList<double> data, double intervalSize, bool dataIsSorted)
        {
            if (dataIsSorted)
            {
                return DefinedInterval(data[0], data.Last(), intervalSize);
            }
            else
            {
                var sortedData = data.ToArray();
                Array.Sort(sortedData);
                return DefinedInterval(sortedData[0], sortedData.Last(), intervalSize);
            }
        }

        /// <summary>
        /// Determines classification range break values for a predefined interval. Returns an array of upper bound break values.
        /// </summary>
        /// <param name="minValue">Minimum value for determining classification.</param>
        /// <param name="maxValue">Maximum value for determining classification.</param>
        /// <param name="intervalSize">The size of the interval to classify break values.</param>
        /// <returns></returns>
        public static double[] DefinedInterval(double minValue, double maxValue, double intervalSize)
        {
            if (intervalSize <= 0d || double.IsNaN(intervalSize))
                return Array.Empty<double>();

            int nBreaks = (int)Math.Floor((maxValue - minValue) / intervalSize);
            if (Math.Abs((maxValue - minValue) / intervalSize - nBreaks) <= Tools.DoubleMachineEpsilon) nBreaks -= 1;
            var results = new double[nBreaks + 1]; // The last index is for the max value
            for (int i = 0; i <= nBreaks - 1; i++)
                results[i] = minValue + intervalSize * (i + 1);

            results[nBreaks] = maxValue;

            return results;
        }

        /// <summary>
        /// Determines classification range break values for intervals with equal counts. Returns an array of upper bound break values.
        /// </summary>
        /// <param name="data">The data to be classified.</param>
        /// <param name="nClasses">The number of classes to determine break values.</param>
        /// <param name="dataIsSorted">Boolean value indicating if the data is sorted in ascending order.</param>
        /// <returns></returns>
        public static double[] Quantiles(IList<double> data, int nClasses, bool dataIsSorted)
        {
            if (nClasses <= 0) return new double[0];
            if ((data == null)) return new double[0];
            if (data.Count == 1) return new double[] { data[0] };
            // Initiate Breaks
            if (nClasses > data.Count)
            {
                if (dataIsSorted == true) return data.ToArray();
                var sortedData = data.ToArray();
                Array.Sort(sortedData);
                return sortedData;
            }//nClasses = data.Count;
            if (nClasses == 1) return dataIsSorted == true ? (new double[] { data.Last() }) : (new double[] { data.Max() });
            // 
            double countPerBin = data.Count / (double)nClasses;
            var results = new double[nClasses];
            if (dataIsSorted)
            {
                for (int i = 1; i < nClasses; i++)
                    results[i - 1] = data[Convert.ToInt32(countPerBin * i - 1)];
                results[nClasses - 1] = data[data.Count - 1];
            }
            else
            {
                var sortedData = data.ToArray();
                Array.Sort(sortedData);
                for (int i = 1; i < nClasses; i++)
                    results[i - 1] = sortedData[Convert.ToInt32(countPerBin * i - 1)];
                results[nClasses - 1] = sortedData[sortedData.Length - 1];
            }
            // 
            return results;
        }

        /// <summary>
        /// Determines classification range break values for intervals using the head/tail classification technique. Returns an array of upper bound break values.
        /// <see href="https://en.wikipedia.org/wiki/Head/tail_breaks"/>
        /// Jiang 2013.
        /// </summary>
        /// <param name="data">The data to be classified.</param>
        /// <param name="dataIsSorted">Boolean value indicating if the data is sorted in ascending order.</param>
        /// <param name="threshold">Proportion of the last bin to stop the iterative classification process.</param>
        /// <returns></returns>
        public static double[] HeadTailInterval(double[] data, bool dataIsSorted, double threshold = 0.4d)
        {
            if ((data == null))
                return Array.Empty<double>();
            if (data.Length == 0)
                return Array.Empty<double>();
            if (data.Length == 1)
                return new[] { data[0] };
            // 
            var sortedData = data;
            if (dataIsSorted == false)
            {
                sortedData = data.ToArray();
                Array.Sort(sortedData);
            }
            // Initialize
            double avg = sortedData.Average();
            var results = new List<double>() { avg };
            int initialSplit = Array.BinarySearch(sortedData, avg);
            if (initialSplit < 0) initialSplit = -1 * initialSplit - 1;
            // 
            //// Tail
            int splitIndex = initialSplit;
            int previousCount = sortedData.Length;
            //// The commented out code performs the process on the tail portion as well. This is not done in the original paper so was removed.
            //// Consider extending
            //while (!(splitIndex / (double)previousCount < threshold | previousCount <= splitIndex) && splitIndex > 0)
            //{
            //    avg = 0d;
            //    for (int i = 0; i < splitIndex; i++)
            //        avg += sortedData[i];
            //    avg = avg / splitIndex;
            //    results.Add(avg);
            //    // 
            //    previousCount = splitIndex;
            //    splitIndex = Array.BinarySearch(sortedData, 0, splitIndex, avg);
            //    if (splitIndex < 0)
            //        splitIndex = -1 * splitIndex - 1;
            //}
            //// Add last average
            //avg = 0d;
            //for (int i = 0; i <= splitIndex; i++)
            //    avg += sortedData[i];
            //avg = avg / (splitIndex+1);
            //results.Add(avg);
            // 
            // Head
            //splitIndex = initialSplit;
            //previousCount = sortedData.Length;
            while ((sortedData.Length - splitIndex) / (double)previousCount <= threshold || (sortedData.Length - splitIndex <= 1))
            {
                avg = 0d;
                for (int i = splitIndex; i < sortedData.Length; i++)
                    avg += sortedData[i];
                avg = avg / (sortedData.Length - splitIndex);
                results.Add(avg);
                // 
                previousCount = sortedData.Length - splitIndex;
                splitIndex = Array.BinarySearch(sortedData, splitIndex, sortedData.Length - splitIndex, avg);
                if (splitIndex < 0)
                    splitIndex = -1 * splitIndex - 1;
                if (sortedData.Length - splitIndex == previousCount)
                    break;
            }
            // 
            // Return results
            if (results.Last() != sortedData.Last()) results.Add(sortedData[sortedData.Length - 1]);
            results.Sort();
            return results.ToArray();
        }

        /// <summary>
        /// Determines classification range break values for intervals at a specified number of standard deviations. Returns an array of upper bound break values.
        /// </summary>
        /// <param name="data">The data to be classified.</param>
        /// <param name="nDeviations">The multiplier determining the number of deviations per interval. For example, .5 would be half a deviation.</param>
        /// <param name="dataIsSorted">Boolean value indicating if the data is sorted in ascending order.</param>
        /// <returns></returns>
        public static double[] StandardDeviationInterval(IList<double> data, double nDeviations, bool dataIsSorted)
        {
            if (nDeviations <= 0d)
                return Array.Empty<double>();
            // If nClasses = 1 Then Return (If(dataIsSorted = True, {data.Last}, {data.Max}))
            if ((data == null))
                return Array.Empty<double>();
            if (data.Count == 0)
                return Array.Empty<double>();
            if (data.Count == 1)
                return new[] { data[0] };
            // 
            var sortedData = data;
            if (dataIsSorted == false)
            {
                sortedData = data.ToArray();
                Array.Sort((double[])sortedData);
            }
            // 
            double summaryData = Data.Statistics.Statistics.PopulationStandardDeviation(sortedData);
            return StandardDeviationInterval(sortedData.Average(), summaryData, sortedData[0], sortedData[sortedData.Count - 1], nDeviations);
        }

        /// <summary>
        /// Determines classification range break values for intervals at a specified number of standard deviations. Returns an array of upper bound break values.
        /// </summary>
        /// <param name="mean">The average value.</param>
        /// <param name="standardDeviation">The standard deviation.</param>
        /// <param name="minValue">Minimum value for determining classification.</param>
        /// <param name="maxValue">Maximum value for determining classification.</param>
        /// <param name="nDeviations">The multiplier determining the number of deviations per interval. For example, .5 would be half a deviation.</param>
        /// <returns></returns>
        public static double[] StandardDeviationInterval(double mean, double standardDeviation, double minValue, double maxValue, double nDeviations)
        {
            if (nDeviations <= 0d)
                return Array.Empty<double>();
            if (minValue == maxValue)
                return new[] { maxValue };
            if (double.IsNaN(mean) | double.IsNaN(standardDeviation))
                return new[] { maxValue };
            // If nClasses = 1 Then Return (If(dataIsSorted = True, {data.Last}, {data.Max}))
            // If IsNothing(Data) Then Return {}
            // 
            var results = new List<double>();
            // Going Down
            double @break = mean - standardDeviation * nDeviations / 2d;
            if (@break > minValue)
                results.Add(@break);
            // 
            while (@break >= minValue)
            {
                @break = @break - nDeviations * standardDeviation;
                if (@break > minValue)
                    results.Add(@break);
            }
            // 
            // Going Up
            @break = mean + standardDeviation * nDeviations / 2d;
            if (@break < maxValue)
                results.Add(@break);
            while (@break <= maxValue)
            {
                @break = @break + nDeviations * standardDeviation;
                if (@break < maxValue)
                    results.Add(@break);
            }
            // 
            // Return results
            results.Add(maxValue);
            results.Sort();
            return results.ToArray();
        }

        /// <summary>
        /// Determines classification range break values for intervals geometrically expanding from the mean value. Returns an array of upper bound break values.
        /// </summary>
        /// <param name="data">The data to be classified.</param>
        /// <param name="rValue">The common ratio (r) in a geometric progression.</param>
        /// <param name="dataIsSorted">Boolean value indicating if the data is sorted in ascending order.</param>
        /// <param name="maxClasses">maximum number of classes allowed</param>
        /// <returns></returns>
        public static double[] GeometricInterval(IList<double> data, double rValue, bool dataIsSorted, bool mirror = true, int maxClasses = 20)
        {
            if ((data == null))
                return Array.Empty<double>();
            if (data.Count == 0)
                return Array.Empty<double>();
            if (data.Count == 1)
                return new[] { data[0] };
            //// 
            var sortedData = data;
            if (dataIsSorted == false)
            {
                sortedData = data.ToArray();
                Array.Sort((double[])sortedData);
            }

            // 
            if (mirror == true)
            {
                double[] breaks;
                double avg = sortedData.Average();
                //ensure that the progression extends beyond both the min and max.
                var minToAvg = Math.Abs(avg - sortedData[0]);
                var avgToMax = Math.Abs(sortedData[sortedData.Count - 1] - avg);
                if (minToAvg > avgToMax)
                {
                    breaks = GeometricInterval(rValue, avg, sortedData[sortedData.Count - 1] + (minToAvg - avgToMax), mirror, maxClasses);
                    //remove any bins that are above the max
                    return breaks.Where(x => x < sortedData[sortedData.Count - 1]).ToArray();
                }
                // 
                breaks = GeometricInterval(rValue, avg, sortedData[sortedData.Count - 1], mirror, maxClasses);
                //remove any bins that are below the min
                return breaks.Where(x => x > sortedData[0]).ToArray();
            }
            else
            {
                return GeometricInterval(rValue, sortedData[0], sortedData[sortedData.Count - 1], mirror, maxClasses);
            }
        }

        /// <summary>
        /// Determines classification range break values for intervals geometrically expanding from the mean value. Returns an array of upper bound break values.
        /// </summary>
        /// <param name="rValue">The common ratio (r) in a geometric progression.</param>
        /// <param name="minValue">Minimum value for determining classification.</param>
        /// <param name="maxValue">Maximum value for determining classification.</param>
        /// <param name="maxClasses">maximum number of classes allowed</param>
        /// <returns></returns>
        public static double[] GeometricInterval(double rValue, double minValue, double maxValue, bool mirror = true, int maxClasses = 20)
        {
            if (rValue <= 1) return Array.Empty<double>();
            if (minValue >= maxValue) return new[] { minValue };
            if (rValue * minValue > maxValue) return new[] { maxValue };
            //if (avg==minValue || avg==maxValue) return new[] { maxValue };

            bool flip = false;
            double min = minValue;
            if (minValue < 0)
            {
                flip = true;
                min = minValue * -1;
            }

            if (min == 0d)
            {
                min = 1d / Math.Pow(10d, (int)Math.Ceiling(Math.Log10(maxValue)));
            }

            double maxClassesR = Math.Pow(maxValue / min, (1.0 / (maxClasses - 1)));
            if (maxClassesR > rValue)
                return GeometricInterval(maxClassesR, min, maxValue, mirror, maxClasses);//GeometricProgression(minValue, maxValue, nClasses);
            // 
            var growthResults = new List<double>();

            int i = 0;
            double binMax = 0;
            while (binMax < maxValue)
            {
                binMax = min * Math.Pow(rValue, i);
                growthResults.Add(binMax);
                i++;
            }

            if (mirror == false)
            {
                if (flip == true)
                {
                    for (i = 0; i < growthResults.Count; i++)
                    {
                        growthResults[i] *= -1;
                    }
                }
                return growthResults.ToArray();
            }

            double[] results = new double[growthResults.Count * 2 - 1];
            int j = growthResults.Count - 1;
            for (i = 0; i < growthResults.Count - 1; i++)
            {
                results[i] = min - (growthResults[j] - min);
                j--;
            }
            // eliminate loop by create growth results as an array to use blockcopy, or test .ToArray() vs loop times.
            //Buffer.BlockCopy(growthResults, 0, results, growthResults.Count, growthResults.Count);
            for (j = 0; j < growthResults.Count; j++)
            {
                results[i] = growthResults[j];
                i++;
            }

            if (flip == true)
            {
                for (i = 0; i < growthResults.Count - 1; i++)
                {
                    results[i] *= -1;
                }
            }

            return results;
        }

        /// <summary>
        /// Classify the geometric progression between two values.
        /// </summary>
        /// <param name="minValue">Minimum value for determining classification.</param>
        /// <param name="maxValue">Maximum value for determining classification.</param>
        /// <param name="n">The number of classes to determine break values.</param>
        /// <returns></returns>
        public static double[] GeometricProgression(double minValue, double maxValue, int n)
        {
            // Calculate common ratio R = (B/A)^(1/(N))
            double r = Math.Pow(maxValue / minValue, 1.0 / n);
            // 
            var results = new double[n];
            // Minimum to Mean
            results[0] = minValue * r;
            for (int i = 1; i <= n - 1; i++)
                results[i] = results[i - 1] * r;
            // 
            return results;
        }


        /// <summary>
        /// Determines classification range break values for intervals with natural breaks. Returns an array of upper bound break values.
        /// Several sources were used before finally landing on a combination of methods.
        /// <see href="http:danieljlewis.org/files/2010/06/Jenks.pdf"/>
        /// <see href = "http:danieljlewis.org/2010/06/07/jenks-natural-breaks-algorithm-in-python/"/>
        /// <see href="https:en.wikipedia.org/wiki/Jenks_natural_breaks_optimization"/>
        /// <see href="https:support.esri.com/en/technical-article/000006743"/>
        /// <see href="https://github.com/mthh/jenkspy/blob/master/jenkspy/src/_jenks.c"/>
        /// <see href="https://github.com/pschoepf/naturalbreaks/blob/master/src/main/java/de/pschoepf/naturalbreaks/JenksFisher.java"/>
        /// </summary>
        /// <param name="data">The data to be classified.</param>
        /// <param name="nClasses">The number of classes to determine break values.</param>
        /// <param name="dataIsSorted">Boolean value indicating if the data is sorted in ascending order.</param>
        /// <param name="breakCounts">Returns the total number of observations for each break associated with the return values.</param>
        /// <returns></returns>
        public static double[] JenksNaturalBreaks(double[] data, int nClasses, bool dataIsSorted, ref int[] breakCounts)
        {
            if (nClasses <= 0) return new double[0];
            if (data == null) return new double[0];
            if (data.Length == 0) return new double[0];
            if (data.Length == 1) return new double[] { data[0] };
            // 
            var sortedData = data;
            if (dataIsSorted == false)
            {
                sortedData = data.ToArray();
                Array.Sort(sortedData);
            }
            // 
            if (sortedData.Length <= nClasses)
            {
                if (breakCounts is null || breakCounts.Length != sortedData.Length) breakCounts = new int[sortedData.Length];
                for (int i = 0; i <= breakCounts.Length - 1; i++)
                    breakCounts[i] = 1;
                // 
                return sortedData;
            }
            // 
            // Populate squared values array
            var squaredValues = new double[sortedData.Length];
            for (int i = 0; i <= sortedData.Length - 1; i++)
                squaredValues[i] = sortedData[i] * sortedData[i];
            // SDAM
            double avg = sortedData.Average();
            double SDAM = 0d;
            for (int i = 0; i <= sortedData.Length - 1; i++)
                SDAM += Math.Pow(sortedData[i] - avg, 2d);
            // 
            // Initiate Breaks
            if (nClasses > sortedData.Length) nClasses = sortedData.Length;
            if (nClasses == 1)
            {
                if (breakCounts is null || breakCounts.Length != sortedData.Length) breakCounts = new int[sortedData.Length];
                breakCounts = new int[] { sortedData.Length };
                return new double[] { sortedData[sortedData.Length - 1] };
            }
            // 
            // calculate initial classes.
            var classes = new JenksBreak[nClasses];
            for (int i = 0; i <= nClasses - 1; i++)
                classes[i] = new JenksBreak();
            // 
            var distinctValues = sortedData.Distinct().ToArray();
            if (distinctValues.Length <= 2)
            {
                if (breakCounts is null || breakCounts.Length != sortedData.Length) breakCounts = new int[sortedData.Length];
                breakCounts = new[] { sortedData.Length - 1 };
                return new[] { sortedData[sortedData.Length - 1] };
            }

            if (distinctValues.Length <= nClasses)
            {
                if (breakCounts is null || breakCounts.Length != distinctValues.Length) breakCounts = new int[distinctValues.Length];
                for (int i = 0; i <= breakCounts.Length - 1; i++)
                    breakCounts[i] = 1;
                return distinctValues;
            }
            // 
            int classIdx = 0;
            int lastClassIdx = -1;
            if (distinctValues.Length != sortedData.Length)
            {
                int[] argbreakCounts = null;
                var initialBreaks = JenksNaturalBreaks(distinctValues, nClasses, true, breakCounts: ref argbreakCounts);
                for (int i = 0; i <= sortedData.Length - 1; i++)
                {
                    if (sortedData[i] > initialBreaks[classIdx])
                    {
                        classIdx += 1;
                        if (classIdx > classes.Length) classIdx = classes.Length - 1;
                    }
                    // classIdx = CInt(Math.Floor(i / classCount)) ' Use this method to guarantee equal values per class
                    // 
                    classes[classIdx].Sum += sortedData[i];
                    classes[classIdx].SumofSquares += squaredValues[i];
                    // 
                    // saving bound between classes
                    if (classIdx != lastClassIdx)
                    {
                        classes[classIdx].StartIdx = i;
                        lastClassIdx = classIdx;
                        if (classIdx > 0) classes[classIdx - 1].EndIdx = i - 1;
                    }
                }
            }
            else
            {
                double classCount = sortedData.Length / (double)nClasses;
                for (int i = 0; i <= sortedData.Length - 1; i++)
                {
                    classIdx = Convert.ToInt32(i / classCount); // Use this method to guarantee roughly equal values per class
                    if (classIdx > classes.Length - 1) classIdx = classes.Length - 1;
                    // 
                    classes[classIdx].Sum += sortedData[i];
                    classes[classIdx].SumofSquares += squaredValues[i];
                    // 
                    // saving bound between classes
                    if (classIdx != lastClassIdx)
                    {
                        classes[classIdx].StartIdx = i;
                        lastClassIdx = classIdx;
                        if (classIdx > 0) classes[classIdx - 1].EndIdx = i - 1;
                    }
                }
                // initialBreaks = Quantiles(sortedData, nClasses, True)
            }
            // 
            // SetClasses(classes, sortedData, squaredValues, initialBreaks)
            // 
            classes[nClasses - 1].EndIdx = sortedData.Length - 1;
            // 
            for (int i = 0; i <= nClasses - 1; i++)
                classes[i].RefreshSquareDeviation();
            // 
            // Optimize
            JenksOptimize(classes, sortedData, squaredValues, 0, classes.Length - 1, SDAM);
            // 
            // Get Results
            // Break range counts
            if (breakCounts is null || breakCounts.Length != classes.Length) breakCounts = new int[classes.Length];
            for (int i = 0; i <= classes.Length - 1; i++)
                breakCounts[i] = classes[i].Count;

            // Break upper bounds
            var result = new double[classes.Length];
            for (int i = 0; i < classes.Length; i++)
                result[i] = sortedData[classes[i].EndIdx];
            // 
            return result;
        }

        private static void JenksOptimize(JenksBreak[] classes, double[] sortedData, double[] squaredValues, int leftIndex, int rightIndex, double SDAM)
        {
            double minValue = 0d;
            bool proceed = true;
            int previousFromIndex = -1;
            int previousToIndex = -1;
            while (proceed)
            {

                // Find which value switch would result in the lowest overall reduction in SDCM
                int fromIndex = 1;
                int toIndex = -1;
                double minSDCM = double.MaxValue;
                double SDCM;
                // Dim nToShift As Int32 = -1
                // Dim classCount As Int32
                // 
                // Check moving observations from each class up to the next class.
                for (int i = leftIndex; i <= rightIndex - 1; i++)
                {
                    // classCount = Math.Max(CInt(classes(i).Count * 0.9), 2)
                    // For j As Int32 = 1 To classCount - 1
                    if (classes[i].Count == 1) continue;
                    MakeShift(classes, sortedData, squaredValues, i, i + 1, 1);
                    SDCM = GetSumSquaredDeviations(classes, leftIndex, rightIndex);
                    if (SDCM < minSDCM)
                    {
                        fromIndex = i;
                        toIndex = i + 1;
                        // nToShift = j
                        minSDCM = SDCM;
                    }
                    // MakeShift(classes, sortedData, squaredValues, i + 1, i, nShifts)
                    // Next
                    MakeShift(classes, sortedData, squaredValues, i + 1, i, 1);
                }
                // 
                // Check moving observations from each class down to the previous class.
                for (int i = rightIndex; i >= leftIndex + 1; i -= 1)
                {
                    // classCount = Math.Max(CInt(classes(i).Count * 0.9), 2)
                    // For j As Int32 = 1 To classCount - 1
                    if (classes[i].Count == 1) continue;
                    MakeShift(classes, sortedData, squaredValues, i, i - 1, 1);
                    SDCM = GetSumSquaredDeviations(classes, leftIndex, rightIndex);
                    if (SDCM < minSDCM)
                    {
                        fromIndex = i;
                        toIndex = i - 1;
                        // nToShift = j
                        minSDCM = SDCM;
                    }

                    // MakeShift(classes, sortedData, squaredValues, i - 1, i, nShifts)
                    // Next
                    MakeShift(classes, sortedData, squaredValues, i - 1, i, 1);
                }
                // 
                if (minSDCM == double.MaxValue) break;
                // Revert to the state that had the lowest overall sum of squared deviations
                MakeShift(classes, sortedData, squaredValues, fromIndex, toIndex, 1);
                // Calculate Goodness of Variance Fit
                double GVF = (SDAM - GetSumSquaredDeviations(classes)) / SDAM;
                // 
                if (GVF > minValue && previousFromIndex != toIndex & previousToIndex != fromIndex)
                {
                    minValue = GVF;
                    proceed = true;
                }
                else
                {
                    // Shift back to the better solution.
                    MakeShift(classes, sortedData, squaredValues, toIndex, fromIndex, 1);
                    // 
                    // Assume local optimum and optimize left and right side
                    int lowerIndex = default, upperIndex = default;
                    if (toIndex > fromIndex)
                    {
                        lowerIndex = fromIndex;
                        upperIndex = toIndex;
                    }
                    else if (toIndex < fromIndex)
                    {
                        lowerIndex = toIndex;
                        upperIndex = fromIndex;
                    }
                    else
                    {
                        // then are equal...proceed is false
                    }

                    if (lowerIndex > leftIndex) JenksOptimize(classes, sortedData, squaredValues, leftIndex, lowerIndex, SDAM);
                    if (upperIndex < rightIndex) JenksOptimize(classes, sortedData, squaredValues, upperIndex, rightIndex, SDAM);
                    // 
                    proceed = false;
                }
                // 
                previousFromIndex = fromIndex;
                previousToIndex = toIndex;
            }
        }

        // perform actual shift
        private static void MakeShift(JenksBreak[] classes, double[] sortedData, double[] squaredValues, int currentClassIndex, int targetId, int nShifts = 1)
        {
            int dataIndex;
            for (int i = 1; i <= nShifts; i++)
            {
                if (targetId < currentClassIndex)
                {
                    if (classes[currentClassIndex].StartIdx < sortedData.Length - 1) classes[currentClassIndex].StartIdx += 1;
                    if (classes[targetId].EndIdx < sortedData.Length - 1) classes[targetId].EndIdx += 1;
                    dataIndex = classes[targetId].EndIdx;
                    // 
                    if (dataIndex == -1) return;
                    // 
                    // removing from the from class
                    classes[currentClassIndex].Sum -= sortedData[dataIndex];
                    classes[currentClassIndex].SumofSquares -= squaredValues[dataIndex];
                    classes[currentClassIndex].RefreshSquareDeviation();

                    // passing to target class
                    classes[targetId].Sum += sortedData[dataIndex];
                    classes[targetId].SumofSquares += squaredValues[dataIndex];
                    classes[targetId].RefreshSquareDeviation();
                }
                else
                {
                    if (classes[currentClassIndex].EndIdx > 0) classes[currentClassIndex].EndIdx -= 1;
                    if (classes[targetId].StartIdx > 0) classes[targetId].StartIdx -= 1;
                    dataIndex = classes[targetId].StartIdx;
                    // 
                    if (dataIndex == -1) return;
                    // 
                    // removing from the from class
                    classes[currentClassIndex].Sum -= sortedData[dataIndex];
                    classes[currentClassIndex].SumofSquares -= squaredValues[dataIndex];
                    classes[currentClassIndex].RefreshSquareDeviation();

                    // passing to target class
                    classes[targetId].Sum += sortedData[dataIndex];
                    classes[targetId].SumofSquares += squaredValues[dataIndex];
                    classes[targetId].RefreshSquareDeviation();
                }
            }
        }

        /// <summary>
        /// Calculates the sum of squared deviations from the class mean through defined classes.
        /// It's the objective function - should be minimized
        /// </summary>
        /// <returns>The sum of standard deviations.</returns>
        private static double GetSumSquaredDeviations(JenksBreak[] classes, int startIndex = -1, int endIndex = -1)
        {
            if (startIndex < 0) startIndex = 0;
            if (endIndex <= 0) endIndex = classes.Length - 1;
            double sum = 0d;
            for (int i = startIndex; i <= endIndex; i++)
                sum += classes[i].SquareDeviation;
            // 
            return sum;
        }

        private class JenksBreak
        {
            public JenksBreak()
            {
                Sum = 0.0d;
                SumofSquares = 0.0d;
                SquareDeviation = 0d;
                StartIdx = -1;
                EndIdx = -1;
            }

            public int StartIdx { get; set; }
            public int EndIdx { get; set; }
            public double Average { get; set; }
            public double Variance { get; set; }
            public double SquareDeviation { get; set; }
            public double SumofSquares { get; set; }
            public double Sum { get; set; }
            public int Count { get; set; }

            public void RefreshSquareDeviation()
            {
                Count = EndIdx - StartIdx + 1;
                if (Count <= 0)
                {
                    Average = 0d; // Sum
                    Variance = 0d;
                }
                else if (Count == 1)
                {
                    Average = Sum;
                    Variance = 0d;
                }
                else
                {
                    Average = Sum / Count;
                    Variance = SumofSquares / Count - Math.Pow(Average, 2d);
                    // 
                    // If Variance < 0 AndAlso Variance < -0.01 Then Debug.Print("Why")
                    if (Variance < 0d) Variance = 0d; // This is generally due to very large sum of squares and averages leading to significant precision error.
                }
                // 
                SquareDeviation = Variance * Count;
            }
        }

        // ''' <summary>
        // ''' Method ported from <see href="https://github.com/mthh/jenkspy/blob/master/jenkspy/src/_jenks.c"/>
        // ''' It approximates natural breaks with GVF values less than the method above but I am keeping it here for reference.
        // ''' </summary>
        // ''' <param name="values">data to calculate breaks on. assumes data is already sorted.</param>
        // ''' <param name="nBins">number of classification bins.</param>
        // ''' <returns></returns>
        // Public Function GetJenksBreaks(values() As Double, nBins As Int32) As Double()
        // Dim i3 As Int32
        // Dim i4 As Int32
        // Dim v, val, id As Double
        // '
        // Dim length_d As Int32 = values.Count
        // '
        // Dim mat1(length_d - 1)() As Double
        // Dim mat2(length_d - 1)() As Double
        // 'Set Mat1 start
        // For i As Int32 = 0 To length_d - 1
        // Dim row(nBins - 1) As Double
        // For j As Int32 = 0 To nBins - 1
        // row(j) = 1
        // Next
        // mat1(i) = row
        // Next
        // 'Set Mat2 start 
        // For i As Int32 = 0 To length_d - 1
        // Dim row(nBins - 1) As Double
        // For j As Int32 = 0 To nBins - 1 'leave the zero index as a value of zero
        // row(j) = 0 'Double.MaxValue
        // Next
        // mat2(i) = row
        // Next
        // '
        // v = 0
        // '
        // 'Fill mat2 with initial values
        // For i As Int32 = 1 To length_d - 1
        // For j As Int32 = 0 To nBins - 1
        // mat2(i)(j) = Double.MaxValue
        // Next
        // Next
        // '
        // 'Begin
        // For l As Int32 = 2 To length_d
        // Dim s1 As Double = 0
        // Dim s2 As Double = 0
        // Dim w As Double = 0
        // '
        // For m As Int32 = 1 To l
        // i3 = l - m + 1
        // val = values(i3 - 1)
        // s2 = s2 + val * val
        // s1 = s1 + val
        // w = w + 1
        // v = s2 - (s1 * s1) / w
        // i4 = i3 - 1

        // If (i4 <> 0) Then
        // For j As Int32 = 2 To nBins
        // If mat2(l - 1)(j - 1) >= (v + mat2(i4 - 1)(j - 2)) Then
        // mat1(l - 1)(j - 1) = i3
        // mat2(l - 1)(j - 1) = v + mat2(i4 - 1)(j - 2)
        // End If
        // Next
        // End If

        // Next

        // mat1(l - 1)(0) = 1
        // mat2(l - 1)(0) = v
        // Next
        // '
        // Dim kclass(nBins - 1) As Double
        // For i As Int32 = 1 To nBins
        // kclass(i - 1) = i
        // Next
        // '              
        // kclass(nBins - 1) = length_d
        // Dim k As Int32 = CInt(mat1(length_d - 1)(nBins - 1) - 1) 'length_d
        // '
        // For j As Int32 = nBins To 2 Step -1
        // id = mat1(k - 1)(j - 1) - 1
        // kclass(j - 2) = id
        // k = CInt(id)
        // Next
        // '
        // Dim brks(nBins - 1) As Double
        // 'brks(0) = values(0)
        // '
        // For i As Int32 = 1 To nBins - 1
        // brks(i - 1) = values(CInt(kclass(i - 1)) - 1)
        // Next
        // brks(nBins - 1) = values(values.Count - 1)
        // '
        // Return brks
        // End Function
    }
}