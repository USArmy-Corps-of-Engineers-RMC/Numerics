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
using Numerics.Data;
using Numerics.Distributions;

namespace Numerics.Data.Statistics
{

    /// <summary>
    /// Computes the autocovariance, autocorrelation, or partial autocorrelation function.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public class Autocorrelation
    {

        /// <summary>
        /// Enumeration of the type of autocorrelation.
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// Autocorrelation
            /// </summary>
            Correlation,
            /// <summary>
            /// Autocovariance
            /// </summary>
            Covariance,
            /// <summary>
            /// Partial autocorrelation
            /// </summary>
            Partial
        }

        /// <summary>
        /// Computes the autocovariance, autocorrelation, or partial autocorrelation function.
        /// </summary>
        /// <param name="data">The list of data to evaluate.</param>
        /// <param name="lagMax">The maximum lag at which to estimate the function. Default is 10*log10(N/m) where N is the number of observations.
        /// Will be automatically limited to one less than the number of observations in the series.</param>
        /// <param name="type">The type of function to be computed.</param>
        public static double[,] Function(IList<double> data, int lagMax = -1, Type type = Type.Correlation)
        {
            if (type == Type.Correlation)
            {
                return Correlation(data, lagMax);
            }
            else if (type == Type.Covariance)
            {
                return Covariance(data, lagMax);
            }
            else if (type == Type.Partial)
            {
                return Partial(data, lagMax);
            }

            return null;
        }

        /// <summary>
        /// Computes the autocovariance, autocorrelation, or partial autocorrelation function.
        /// </summary>
        /// <param name="timeSeries">The time-series to evaluate.</param>
        /// <param name="lagMax">The maximum lag at which to estimate the function. Default is 10*log10(N/m) where N is the number of observations.
        /// Will be automatically limited to one less than the number of observations in the series.</param>
        /// <param name="type">The type of function to be computed.</param>
        public static double[,] Function(TimeSeries timeSeries, int lagMax = -1, Type type = Type.Correlation)
        {
            if (type == Type.Correlation)
            {
                return Correlation(timeSeries, lagMax);
            }
            else if (type == Type.Covariance)
            {
                return Covariance(timeSeries, lagMax);
            }
            else if (type == Type.Partial)
            {
                return Partial(timeSeries, lagMax);
            }

            return null;
        }

        /// <summary>
        /// Compute the autocovariance function.
        /// </summary>
        /// <param name="data">The list of data to assess.</param>
        /// <param name="lagMax">The maximum lag.</param>
        private static double[,] Covariance(IList<double> data, int lagMax = -1)
        {
            int n = data.Count;
            if (lagMax < 0) lagMax = (int)Math.Floor(Math.Min(10d * Math.Log10(n), n - 1));
            if (lagMax < 1 || n < 2) return null;
            var acvf = new double[lagMax + 1, 2];
            double mean = Statistics.Mean(data);
            for (int lag = 0; lag <= lagMax; lag++)
            {
                double cov = 0d;
                for (int t = lag; t < n; t++)
                    cov += (data[t] - mean) * (data[t - lag] - mean);
                acvf[lag, 0] = lag;
                acvf[lag, 1] = cov / n;
            }

            return acvf;
        }

        /// <summary>
        /// Compute the autocovariance function.
        /// </summary>
        /// <param name="timeSeries">The time-series to assess.</param>
        /// <param name="lagMax">The maximum lag.</param>
        private static double[,] Covariance(TimeSeries timeSeries, int lagMax = -1)
        {
            int n = timeSeries.Count;
            if (lagMax < 0) lagMax = (int)Math.Floor(Math.Min(10d * Math.Log10(n), n - 1));
            if (lagMax < 1 || n < 2) return null;
            var acvf = new double[lagMax + 1, 2];
            double mean = timeSeries.MeanValue();
            for (int lag = 0; lag <= lagMax; lag++)
            {
                double cov = 0d;
                for (int t = lag; t < n; t++)
                    cov += (timeSeries[t].Value - mean) * (timeSeries[t - lag].Value - mean);
                acvf[lag, 0] = lag;
                acvf[lag, 1] = cov / n;
            }

            return acvf;
        }

        /// <summary>
        /// Compute the autocorrelation function.
        /// </summary>
        /// <param name="data">The list of data to assess.</param>
        /// <param name="lagMax">The maximum lag.</param>
        private static double[,] Correlation(IList<double> data, int lagMax = -1)
        {
            int n = data.Count;
            if (lagMax < 0) lagMax = (int)Math.Floor(Math.Min(10d * Math.Log10(n), n - 1));
            if (lagMax < 1 || n < 2) return null;
            var acf = Covariance(data, lagMax);
            double den = acf[0, 1];
            for (int i = 0; i < acf.GetLength(0); i++)
                acf[i, 1] /= den;
            return acf;
        }

        /// <summary>
        /// Compute the autocorrelation function.
        /// </summary>
        /// <param name="timeSeries">The time-series to assess.</param>
        /// <param name="lagMax">The maximum lag.</param>
        private static double[,] Correlation(TimeSeries timeSeries, int lagMax = -1)
        {
            int n = timeSeries.Count;
            if (lagMax < 0) lagMax = (int)Math.Floor(Math.Min(10d * Math.Log10(n), n - 1));
            if (lagMax < 1 || n < 2) return null;
            var acf = Covariance(timeSeries, lagMax);
            double den = acf[0, 1];
            for (int i = 0; i < acf.GetLength(0); i++)
                acf[i, 1] /= den;
            return acf;
        }

        /// <summary>
        /// Compute the partial autocorrelation function.
        /// </summary>
        /// <param name="data">The list of data to assess.</param>
        /// <param name="lagMax">The maximum lag.</param>
        private static double[,] Partial(IList<double> data, int lagMax = -1)
        {
            int n = data.Count;
            if (lagMax < 0) lagMax = (int)Math.Floor(Math.Min(10d * Math.Log10(n), n - 1));
            if (lagMax < 1 || n < 2) return null;
            // First compute the ACVF
            var acvf = Covariance(data, lagMax);
            // Then compute PACF using the Durbin-Levinson algorithm
            int i, j;
            var phis = new double[lagMax + 1];
            var phis2 = new double[lagMax + 1];
            var pacf = new double[lagMax, 2];
            double vi, phinn;
            phis[0] = acvf[1, 1] / acvf[0, 1];
            pacf[0, 0] = 1d;
            pacf[0, 1] = phis[0];
            vi = acvf[0, 1];
            vi *= 1d - phis[0] * phis[0];
            for (i = 2; i <= lagMax; i++)
            {
                for (j = 0; j < i - 1; j++)
                    phis2[j] = phis[i - j - 2];
                phinn = acvf[i, 1];
                for (j = 1; j < i; j++)
                    phinn -= phis[j - 1] * acvf[i - j, 1];
                phinn /= vi;
                for (j = 0; j < i - 1; j++)
                    phis[j] -= phinn * phis2[j];
                vi *= 1d - phinn * phinn;
                phis[i - 1] = phinn;
                pacf[i - 1, 0] = i;
                pacf[i - 1, 1] = phis[i - 1];
            }

            return pacf;
        }

        /// <summary>
        /// Compute the partial autocorrelation function.
        /// </summary>
        /// <param name="timeSeries">The time-series to assess.</param>
        /// <param name="lagMax">The maximum lag.</param>
        private static double[,] Partial(TimeSeries timeSeries, int lagMax = -1)
        {
            int n = timeSeries.Count;
            if (lagMax < 0) lagMax = (int)Math.Floor(Math.Min(10d * Math.Log10(n), n - 1));
            if (lagMax < 1 || n < 2) return null;
            // First compute the ACVF
            var acvf = Covariance(timeSeries, lagMax);
            // Then compute PACF using the Durbin-Levinson algorithm
            int i, j;
            var phis = new double[lagMax + 1];
            var phis2 = new double[lagMax + 1];
            var pacf = new double[lagMax, 2];
            double vi, phinn;
            phis[0] = acvf[1, 1] / acvf[0, 1];
            pacf[0, 0] = 1d;
            pacf[0, 1] = phis[0];
            vi = acvf[0, 1];
            vi *= 1d - phis[0] * phis[0];
            for (i = 2; i <= lagMax; i++)
            {
                for (j = 0; j < i - 1; j++)
                    phis2[j] = phis[i - j - 2];
                phinn = acvf[i, 1];
                for (j = 1; j < i; j++)
                    phinn -= phis[j - 1] * acvf[i - j, 1];
                phinn /= vi;
                for (j = 0; j < i - 1; j++)
                    phis[j] -= phinn * phis2[j];
                vi *= 1d - phinn * phinn;
                phis[i - 1] = phinn;
                pacf[i - 1, 0] = i;
                pacf[i - 1, 1] = phis[i - 1];
            }

            return pacf;
        }

        /// <summary>
        /// Get confidence interval for ACF and PACF rho values.
        /// </summary>
        /// <param name="sampleSize">The sample size.</param>
        /// <param name="interval">The confidence interval width. Default = 0.95, or 95%.</param>
        public static double[] CorrelationConfidenceInterval(int sampleSize, double interval = 0.95d)
        {
            double alpha = 0.5d * (1.0d - interval);
            double lo = Normal.StandardZ(alpha) / Math.Sqrt(sampleSize);
            double hi = Normal.StandardZ(1d - alpha) / Math.Sqrt(sampleSize);
            return new[] { lo, hi };
        }
    }
}