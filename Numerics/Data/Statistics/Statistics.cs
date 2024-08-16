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

// <copyright file="Statistics.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
// 
// Copyright (c) 2009-2015 Math.NET
// 
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Numerics.Data.Statistics
{

    /// <summary>
    /// Contains functions for computing descriptive statistics of a sample of data.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     <b> Authors: </b>
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    /// <b> References: </b>
    /// <list type="bullet">
    /// <item>
    /// <see href = "https://en.wikipedia.org/wiki/Summary_statistics" />
    /// </item>
    /// <item>
    /// <see href = "https://en.wikipedia.org/wiki/Descriptive_statistics" />
    /// </item>
    /// <item>
    /// This class contains some functions from the Math.NET Numerics library, <see href="http://numerics.mathdotnet.com"/>
    /// </item>
    /// </list>
    /// </para>
    /// </remarks>
    public class Statistics
    {
        /// <summary>
        /// Returns the smallest value from the unsorted data array.
        /// Returns NaN if data is empty or any entry is NaN
        /// </summary>
        /// <param name="sampleData">Sample of data, no sorting is assumed.</param>
        public static double Minimum(IList<double> sampleData)
        {
            if (sampleData.Count == 0) return double.NaN;

            double min = double.MaxValue;
            for (int i = 0; i < sampleData.Count; i++)
            {
                if (sampleData[i] < min || double.IsNaN(sampleData[i]))
                {
                    min = sampleData[i];
                }
            }

            return min;
        }

        /// <summary>
        /// Returns the largest value from the unsorted data array.
        /// Returns NaN if data is empty or any entry is NaN.
        /// </summary>
        /// <param name="sampleData">Sample of data, no sorting is assumed.</param>
        public static double Maximum(IList<double> sampleData)
        {
            if (sampleData.Count == 0) return double.NaN;

            double max = double.MinValue;
            for (int i = 0; i < sampleData.Count; i++)
            {
                if (sampleData[i] > max || double.IsNaN(sampleData[i]))
                {
                    max = sampleData[i];
                }
            }

            return max;
        }

        /// <summary>
        /// Estimates the sum of the unsorted data array.
        /// Returns NaN if data is empty or any entry is NaN.
        /// </summary>
        /// <param name="sampleData">Sample of data, no sorting is assumed.</param>
        public static double Sum(IList<double> sampleData)
        {
            if (sampleData.Count == 0) return double.NaN;

            double sum = 0d;
            for (int i = 0; i < sampleData.Count; i++)
                sum += sampleData[i];
            return sum;
        }

        /// <summary>
        /// Estimates the arithmetic sample mean from the unsorted data array.
        /// Returns NaN if data is empty or any entry is NaN.
        /// </summary>
        /// <param name="sampleData">Sample of data, no sorting is assumed.</param>
        public static double Mean(IList<double> sampleData)
        {
            if (sampleData.Count == 0) return double.NaN;

            double sum = 0d;
            for (int i = 0; i < sampleData.Count; i++)
                sum += sampleData[i];
            return sum / sampleData.Count;
        }

        /// <summary>
        /// Computes the arithmetic sample mean from the unsorted data array by first enabling parallelization of the array.
        /// Returns NaN if data is empty or any entry is NaN.
        /// </summary>
        /// <param name="sampleData">Sample of data, no sorting is assumed.</param>
        public static double ParallelMean(IList<double> sampleData)
        {
            if (sampleData.Count == 0) return double.NaN;
            double sum = sampleData.AsParallel().Sum();
            return sum / sampleData.Count;
        }

        /// <summary>
        /// Evaluates the geometric mean of the unsorted data array.
        /// Returns NaN if data is empty or any entry is NaN.
        /// </summary>
        /// <param name="sampleData">Sample of data, no sorting is assumed.</param>
        public static double GeometricMean(IList<double> sampleData)
        {
            if (sampleData.Count == 0) return double.NaN;

            double sum = 0d;
            for (int i = 0; i < sampleData.Count; i++)
                sum += Math.Log(sampleData[i]);
            return Math.Exp(sum / sampleData.Count);
        }

        /// <summary>
        /// Evaluates the harmonic mean of the unsorted data array.
        /// Returns NaN if data is empty or any entry is NaN.
        /// </summary>
        /// <param name="sampleData">Sample of data, no sorting is assumed.</param>
        public static double HarmonicMean(IList<double> sampleData)
        {
            if (sampleData.Count == 0) return double.NaN;

            double sum = 0d;
            for (int i = 0; i < sampleData.Count; i++)
                sum += 1.0d / sampleData[i];
            return sampleData.Count / sum;
        }

        /// <summary>
        /// Estimates the unbiased population variance from the provided samples as unsorted array.
        /// On a dataset of size N will use an N-1 normalizer (Bessel's correction).
        /// Returns NaN if data has less than two entries or if any entry is NaN.
        /// </summary>
        /// <param name="sampleData">Sample of data, no sorting is assumed.</param>
        public static double Variance(IList<double> sampleData)
        {
            if (sampleData.Count <= 1) return double.NaN;

            double variance_ = 0d;
            double t = sampleData[0];
            for (int i = 1; i < sampleData.Count; i++)
            {
                t += sampleData[i];
                double diff = (i + 1) * sampleData[i] - t;
                variance_ += diff * diff / ((i + 1.0d) * i);
            }
            return variance_ / (sampleData.Count - 1);
        }

        /// <summary>
        /// Evaluates the population variance from the full population provided as unsorted array.
        /// On a dataset of size N will use an N normalizer and would thus be biased if applied to a subset.
        /// Returns NaN if data is empty or if any entry is NaN.
        /// </summary>
        /// <param name="sampleData">Sample of data, no sorting is assumed.</param>
        public static double PopulationVariance(IList<double> sampleData)
        {
            if (sampleData.Count == 0) return double.NaN;

            double variance = 0d;
            double t = sampleData[0];
            for (int i = 1; i < sampleData.Count; i++)
            {
                t += sampleData[i];
                double diff = (i + 1) * sampleData[i] - t;
                variance += diff * diff / ((i + 1.0d) * i);
            }
            return variance / sampleData.Count;
        }

        /// <summary>
        /// Estimates the unbiased population standard deviation from the provided samples as unsorted array.
        /// On a dataset of size N will use an N-1 normalizer (Bessel's correction).
        /// Returns NaN if data has less than two entries or if any entry is NaN.
        /// </summary>
        /// <param name="sampleData">Sample of data, no sorting is assumed.</param>
        public static double StandardDeviation(IList<double> sampleData)
        {
            return Math.Sqrt(Variance(sampleData));
        }

        /// <summary>
        /// Evaluates the population standard deviation from the full population provided as unsorted array.
        /// On a dataset of size N will use an N normalizer and would thus be biased if applied to a subset.
        /// Returns NaN if data is empty or if any entry is NaN.
        /// </summary>
        /// <param name="sampleData">Sample of data, no sorting is assumed.</param>
        public static double PopulationStandardDeviation(IList<double> sampleData)
        {
            return Math.Sqrt(PopulationVariance(sampleData));
        }

        /// <summary>
        /// Estimates the arithmetic sample mean and the unbiased population variance from the provided samples as unsorted array.
        /// On a dataset of size N will use an N-1 normalizer (Bessel's correction).
        /// Returns NaN for mean if data is empty or any entry is NaN and NaN for variance if data has less than two entries or if any entry is NaN.
        /// </summary>
        /// <param name="sampleData">Sample of data, no sorting is assumed.</param>
        public static Tuple<double, double> MeanVariance(IList<double> sampleData)
        {
            return new Tuple<double, double>(Mean(sampleData), Variance(sampleData));
        }

        /// <summary>
        /// Estimates the arithmetic sample mean and the unbiased population standard deviation from the provided samples as unsorted array.
        /// On a dataset of size N will use an N-1 normalizer (Bessel's correction).
        /// Returns NaN for mean if data is empty or any entry is NaN and NaN for standard deviation if data has less than two entries or if any entry is NaN.
        /// </summary>
        /// <param name="sampleData">Sample of data, no sorting is assumed.</param>
        public static Tuple<double, double> MeanStandardDeviation(IList<double> sampleData)
        {
            return new Tuple<double, double>(Mean(sampleData), StandardDeviation(sampleData));
        }

        /// <summary>
        /// Estimates the coefficient of variation from the provided sample of data.
        /// </summary>
        /// <param name="sampleData">Sample of data, no sorting is assumed.</param>
        public static double CoefficientOfVariation(IList<double> sampleData)
        {
            return StandardDeviation(sampleData) / Mean(sampleData);
        }

        /// <summary>
        /// Estimates the skewness coefficient from the unsorted data array.
        /// Returns NaN if data is empty or any entry is NaN.
        /// </summary>
        /// <param name="sampleData">Sample of data, no sorting is assumed.</param>
        public static double Skewness(IList<double> sampleData)
        {
            if (sampleData.Count == 0) return double.NaN;

            double mean = Mean(sampleData);
            int n = sampleData.Count;
            double s2 = 0d, s3 = 0d;
            for (int i = 0; i < n; i++)
            {
                double xm = sampleData[i] - mean;
                s2 += xm * xm;
                s3 += xm * xm * xm;
            }
            double m2 = s2 / n;
            double m3 = s3 / n;
            double g = m3 / Math.Pow(m2, 3.0d / 2.0d);
            double a = Math.Sqrt(n * (n - 1));
            double b = n - 2;
            return a / b * g;
        }

        /// <summary>
        /// Computes the standard error of the statistic, using the jackknife method.
        /// </summary>
        /// <param name="sampleData">Sample of data, no sorting is assumed.</param>
        /// <param name="statistic">The statistic for estimating standard error.</param>
        public static double JackKnifeStandardError(IList<double> sampleData, Func<IList<double>, double> statistic)
        {
            int N = sampleData.Count;
            double theta = statistic(sampleData);
            double I = 0d;
            Parallel.For(0, N, () => 0d, (i, loop, subI) =>
            {
                // Remove data point
                var jackSample = new List<double>(sampleData);
                jackSample.RemoveAt(i);
                // Compute statistic
                subI += Tools.Sqr(statistic(jackSample) - theta);
                return subI;
            }, z => Tools.ParallelAdd(ref I, z));
            return Math.Sqrt((N - 1) / (double)N * I);
        }

        /// <summary>
        /// Returns a jackknifed sample.
        /// </summary>
        /// <param name="sampleData">Sample of data, no sorting is assumed.</param>
        /// <param name="statistic">The statistic for estimating a sample.</param>
        public static double[] JackKnifeSample(IList<double> sampleData, Func<IList<double>, double> statistic)
        {
            int N = sampleData.Count;
            var thetaJack = new double[N];
            // Perform Jackknife
            Parallel.For(0, N, i =>
            {
                // Remove data point
                var jackSample = new List<double>(sampleData);
                jackSample.RemoveAt(i);
                // Compute statistic
                thetaJack[i] = statistic(jackSample);
            });
            return thetaJack;
        }

        /// <summary>
        /// Estimates the kurtosis from the unsorted data array.
        /// Returns NaN if data is empty or any entry is NaN.
        /// </summary>
        /// <param name="sampleData">Sample of data, no sorting is assumed.</param>
        public static double Kurtosis(IList<double> sampleData)
        {
            if (sampleData.Count == 0) return double.NaN;

            double mean = Mean(sampleData);
            int n = sampleData.Count;
            double s2 = 0d, s4 = 0d;
            for (int i = 0; i < n; i++)
            {
                double xm = sampleData[i] - mean;
                s2 += xm * xm;
                s4 += xm * xm * xm * xm;
            }
            double m2 = s2 / n;
            double m4 = s4 / n;
            double v = s2 / (n - 1);
            double a = n * (n + 1) / (double)((n - 1) * (n - 2) * (n - 3));
            double b = s4 / (v * v);
            double c = (n - 1) * (n - 1) / (double)((n - 2) * (n - 3));
            return a * b - 3d * c;
        }

        /// <summary>
        /// Estimates the unbiased population covariance from the provided two sample arrays.
        /// On a dataset of size N will use an N-1 normalizer (Bessel's correction).
        /// Returns NaN if data has less than two entries or if any entry is NaN.
        /// </summary>
        /// <param name="sampleData1">First sample of data, no sorting is assumed.</param>
        /// <param name="sampleData2">Second sample of data, no sorting is assumed.</param>
        public static double Covariance(IList<double> sampleData1, IList<double> sampleData2)
        {
            if (sampleData1.Count != sampleData2.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.");
            }

            if (sampleData1.Count <= 1) return double.NaN;

            double mean1 = Mean(sampleData1);
            double mean2 = Mean(sampleData2);
            double covariance = 0.0d;
            for (int i = 0; i < sampleData1.Count; i++)
                covariance += (sampleData1[i] - mean1) * (sampleData2[i] - mean2);
            return covariance / (sampleData1.Count - 1);
        }

        /// <summary>
        /// Evaluates the population covariance from the full population provided as two arrays.
        /// On a dataset of size N will use an N normalizer and would thus be biased if applied to a subset.
        /// Returns NaN if data is empty or if any entry is NaN.
        /// </summary>
        /// <param name="sampleData1">First sample of data, no sorting is assumed.</param>
        /// <param name="sampleData2">Second sample of data, no sorting is assumed.</param>
        public static double PopulationCovariance(IList<double> sampleData1, IList<double> sampleData2)
        {
            if (sampleData1.Count != sampleData2.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.");
            }

            if (sampleData1.Count == 0) return double.NaN;

            double mean1 = Mean(sampleData1);
            double mean2 = Mean(sampleData2);
            double covariance = 0.0d;
            for (int i = 0; i < sampleData1.Count; i++)
                covariance += (sampleData1[i] - mean1) * (sampleData2[i] - mean2);
            return covariance / sampleData1.Count;
        }

        /// <summary>
        /// Returns the first four product moments of a sample {Mean, Standard Deviation, Skew, and Kurtosis}, or returns NaN if data is empty or any entry is NaN.
        /// </summary>
        /// <param name="sampleData">Sample of data, no sorting is assumed.</param>
        public static double[] ProductMoments(IList<double> sampleData)
        {
            if (sampleData.Count == 0)
            {
                return [double.NaN, double.NaN, double.NaN, double.NaN];
            }

            // Create variables
            double M;         // mean
            double V;         // variance
            double S;         // standard deviation
            double G;         // skew
            double K;         // kurtosis
            double X = 0;     // sum
            double X2 = 0;    // sum of X^2
            double X3 = 0;    // sum of X^3
            double X4 = 0;    // sum of X^4
            double U1, U2, U3, U4;

            // Get sample size
            double N = sampleData.Count;

            // Compute sum
            for (int i = 0; i <= (int)(N - 1L); i++)
            {
                X += sampleData[i];
                X2 += Math.Pow(sampleData[i], 2d);
                X3 += Math.Pow(sampleData[i], 3d);
                X4 += Math.Pow(sampleData[i], 4d);
            }

            // Compute averages
            U1 = X / N;
            U2 = X2 / N;
            U3 = X3 / N;
            U4 = X4 / N;
            // Get mean
            M = U1;
            // Compute variance
            V = (U2 - Math.Pow(U1, 2d)) * (N / (N - 1));
            // Compute sample standard deviation
            S = Math.Sqrt(V);
            // Compute sample skew
            G = Math.Pow(N, 2d) * (U3 - 3d * U1 * U2 + 2d * Math.Pow(U1, 3d)) / ((N - 1L) * (N - 2L) * Math.Pow(S, 3d));
            // Compute sample kurtosis
            K = Math.Pow(N, 2d) * (N + 1L) * (U4 - 4d * U1 * U3 + 6d * U2 * Math.Pow(U1, 2d) - 3d * Math.Pow(U1, 4d)) / ((N - 1L) * (N - 2L) * (N - 3L) * Math.Pow(S, 4d)) - 3d * Math.Pow(N - 1L, 2d) / ((N - 2L) * (N - 3L));
            return [M, S, G, K];
        }

        /// <summary>
        /// Returns the linear moments of a sample {L-Mean (λ1), L-Scale (λ2), L-Skewness (τ3), and L-Kurtosis (τ4)}, or returns NaN if data is empty or any entry is NaN.
        /// </summary>
        /// <param name="sampleData">Sample of data, no sorting is assumed.</param>
        public static double[] LinearMoments(IList<double> sampleData)
        {
            var sample = sampleData.ToArray();
            if (sample.Count() == 0)
            {
                return [double.NaN, double.NaN, double.NaN, double.NaN];
            }
            // 
            double B0 = 0d;
            double B1 = 0d;
            double B2 = 0d;
            double B3 = 0d;
            double N = sample.Count();
            Array.Sort(sample);

            for (int i = 1; i <= N; i++)
            {
                B0 += sample[i - 1];
                if (i > 1)
                    B1 += (i - 1) / (N - 1) * sample[i - 1];
                if (i > 2)
                    B2 += (i - 2) * (i - 1) / ((N - 2) * (N - 1)) * sample[i - 1];
                if (i > 3)
                    B3 += (i - 3) * (i - 2) * (i - 1) / ((N - 3) * (N - 2) * (N - 1)) * sample[i - 1];
            }
 
            B0 /= N;
            B1 /= N;
            B2 /= N;
            B3 /= N;
            // L-Mean (λ1)
            // L-Scale (λ2)
            // L-Skewness (τ3)
            // L-Kurtosis (τ4)
            double L1 = B0;
            double L2 = 2d * B1 - B0;
            double T3 = 2d * (3d * B2 - B0) / (2d * B1 - B0) - 3d;
            double T4 = 5d * (2d * (2d * B3 - 3d * B2) + B0) / (2d * B1 - B0) + 6d;
            return [L1, L2, T3, T4];
        }

        /// <summary>
        /// Returns the k-th percentile of values in a sample.
        /// </summary>
        /// <param name="sampleData">Sample of data.</param>
        /// <param name="k">The k-th percentile to find.</param>
        /// <param name="sampleDataIsSorted">Boolean value indicating if the sample of data is sorted or not. Assumed false, not sorted, by default.</param>
        /// <returns>The k-th percentile.</returns>
        public static double Percentile(IList<double> sampleData, double k, bool sampleDataIsSorted = false)
        {
            int n = sampleData.Count;
            double m = (n - 1) * k + 1d;
            // 
            if (sampleDataIsSorted == false)
            {
                var sortedSampleData = sampleData.ToArray(); // allows the original sampleData to not get sorted. 
                Array.Sort(sortedSampleData);
                // 
                if (m == 1.0d)
                {
                    return sortedSampleData[0];
                }
                else if (m == n)
                {
                    return sortedSampleData[n - 1];
                }
                else
                {
                    int i = (int)Math.Truncate(m);
                    double d = m - i;
                    return sortedSampleData[i - 1] + d * (sortedSampleData[i] - sortedSampleData[i - 1]);
                }
            }
            else if (m == 1.0d)
            {
                return sampleData[0];
            }
            else if (m == n)
            {
                return sampleData[n - 1];
            }
            else
            {
                int i = (int)Math.Truncate(m);
                double d = m - i;
                return sampleData[i - 1] + d * (sampleData[i] - sampleData[i - 1]);
            }
        }

        /// <summary>
        /// Estimates the 5-number summary {min, 25th-percentile, 50th-percentile, 75th-percentile, max} from a sample of data.
        /// </summary>
        /// <param name="sampleData">Sample of data, no sorting is assumed.</param>
        /// <returns>5-number summary statistics.</returns>
        public static double[] FiveNumberSummary(IList<double> sampleData)
        {
            double[] results;
            if (sampleData.Count == 0)
            {
                results = new[] { double.NaN, double.NaN, double.NaN, double.NaN, double.NaN };
                return results;
            }
            // Sort the data, convert to array to so that the input variable does not get changed.
            var sortedSampleData = sampleData.ToArray();
            Array.Sort(sortedSampleData);
            // 
            results = new[] { sortedSampleData[0], Percentile(sortedSampleData, 0.25d, true), Percentile(sortedSampleData, 0.5d, true), Percentile(sortedSampleData, 0.75d, true), sortedSampleData[sortedSampleData.Count() - 1] };
            return results;
        }

        /// <summary>
        /// Estimates the 7-number summary {min, 5th percentile, 25th-percentile, 50th-percentile, 75th-percentile, 95th-percentile, max} from a sample of data.
        /// </summary>
        /// <param name="sampleData">Sample of data, no sorting is assumed.</param>
        /// <returns>7-number summary statistics.</returns>
        public static double[] SevenNumberSummary(IList<double> sampleData)
        {
            double[] results;
            if (sampleData.Count == 0)
            {
                results = new[] { double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN };
                return results;
            }
            // Sort the data, convert to array to so that the input variable does not get changed.
            var sortedSampleData = sampleData.ToArray();
            Array.Sort(sortedSampleData);
            // 
            results = new[] { sortedSampleData[0], Percentile(sortedSampleData, 0.05d, true), Percentile(sortedSampleData, 0.25d, true), Percentile(sortedSampleData, 0.5d, true), Percentile(sortedSampleData, 0.75d, true), Percentile(sortedSampleData, 0.95d, true), sortedSampleData[sortedSampleData.Count() - 1] };
            // 
            return results;
        }

        /// <summary>
        /// Returns the rank of each entry of the unsorted data array.
        /// </summary>
        /// <param name="data">The array of sample of data, no sorting is assumed.</param>
        public static double[] RanksInplace(double[] data)
        {

            var ranks = new double[data.Length];
            var index = new int[data.Length];
            for (int i = 0; i < index.Length; i++)
            {
                index[i] = i;
            }

            Array.Sort(data, index);
            int previousIndex = 0;
            for (int i = 1; i < data.Length; i++)
            {

                if (Math.Abs(data[i] - data[previousIndex]) <= 0d)
                {
                    continue;
                }

                if (i == previousIndex + 1)
                {
                    ranks[index[previousIndex]] = i;
                }
                else
                {
                    RanksTies(ranks, index, previousIndex, i);
                }

                previousIndex = i;
            }

            RanksTies(ranks, index, previousIndex, data.Length);
            return ranks;
        }

        /// <summary>
        /// Returns the rank of each entry of the unsorted data array.
        /// </summary>
        /// <param name="data">The array of sample of data, no sorting is assumed.</param>
        /// <param name="ties">Output. The number of ties in the data.</param>
        public static double[] RanksInPlace(double[] data, out double [] ties)
        {

            var ranks = new double[data.Length];
            ties = new double[data.Length];
            var index = new int[data.Length];
            for (int i = 0; i < index.Length; i++)
            {
                index[i] = i;
            }

            Array.Sort(data, index);
            int previousIndex = 0;
            int t = 0;
            for (int i = 1; i < data.Length; i++)
            {
                if (data[i].AlmostEquals(data[previousIndex], Tools.DoubleMachineEpsilon))
                {
                    t += 1;
                    continue;
                }

                if (i == previousIndex + 1)
                {
                    ranks[index[previousIndex]] = i;
                    t = 0;      
                }
                else
                {
                    RanksTies(ranks, index, previousIndex, i);
                    ties[i - 1] = t;
                    t = 0;
                }

                previousIndex = i;
            }

            RanksTies(ranks, index, previousIndex, data.Length);
            return ranks;
        }

        /// <summary>
        /// Helper function for RanksInplace(double[], out double[])
        /// </summary>
        private static void RanksTies(double[] ranks, int[] index, int a, int b)
        {
            
            double rank = (b + a - 1) / 2d + 1;
            for (int k = a; k < b; k++)
            {
                ranks[index[k]] = rank;
            }
        }

        /// <summary>
        /// Computes the entropy function for a set of numerical values in a given Probability Density Function (pdf).
        /// </summary>
        /// <param name="values">The array of values.</param>
        /// <param name="pdf">A probability distribution function.</param>
        public static double Entropy(double[] values, Func<double, double> pdf)
        {
            double sum = 0;
            for (int i = 0; i < values.Length; i++)
            {
                double p = pdf(values[i]);
                sum += p * Math.Log(p);
            }
            return -sum;
        }
    }
}