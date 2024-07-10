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

using Numerics.Data;
using Numerics.Mathematics;
using Numerics.Mathematics.Integration;
using Numerics.Sampling;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Numerics.Distributions
{

    /// <summary>
    /// Declares common functionality for all univariate distributions.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    /// This code was inspired by the statistical library developed by Will Lehman (HEC). I have also incorporated concepts used in the Accord.NET and MathNet.Numerics open source libraries.
    /// </para>
    /// </remarks>
    [Serializable]
    public abstract class UnivariateDistributionBase : IUnivariateDistribution
    {
        /// <summary>
        /// Approximation for assessing if a parameter is near zero. 
        /// </summary>
        protected double NearZero = 1E-4;

        /// <summary>
        /// Returns the continuous distribution type.
        /// </summary>
        public abstract UnivariateDistributionType Type { get; }

        /// <summary>
        /// Returns the display name of the distribution type as a string.
        /// </summary>
        public abstract string DisplayName { get; }

        /// <summary>
        /// Returns the short display name of the distribution as a string.
        /// </summary>
        public abstract string ShortDisplayName { get; }

        /// <summary>
        /// Returns a short label of the distribution and parameters as a string.
        /// </summary>
        public string DisplayLabel
        {
            get
            {
                StringBuilder label = new StringBuilder(ShortDisplayName + " (");
                var paramVals = GetParameters;
                for (int i = 0; i < paramVals.Length; i++)
                {
                    label.Append(string.Format("{0:0.#####}", paramVals[i]));
                    if (i != paramVals.Length - 1)
                        label.Append(", ");
                }
                label.Append(")");
                return label.ToString();
            }
        }

        /// <summary>
        /// Returns the number of distribution parameters.
        /// </summary>
        public abstract int NumberOfParameters { get; }

        /// <summary>
        /// Returns the distribution parameters in 2-column array of string.
        /// </summary>
        public abstract string[,] ParametersToString { get; }

        /// <summary>
        /// Returns the distribution parameter names as an array of string.
        /// </summary>
        public virtual string[] ParameterNames
        {
            get { return ParametersToString.GetColumn(0); }
        }

        /// <summary>
        /// Returns the distribution parameter names in short form (e.g. µ, σ) in an array of string.
        /// </summary>
        public abstract string[] ParameterNamesShortForm { get; }

        /// <summary>
        /// Returns the distribution parameters in an array of double.
        /// </summary>
        /// <returns></returns>
        public abstract double[] GetParameters { get; }
        /// <summary>
        /// Returns the distribution parameter property names in an array of string.
        /// </summary>
        /// <returns></returns>
        public abstract string[] GetParameterPropertyNames { get; }

        /// <summary>
        /// Returns a boolean value describing if the current parameters are valid or not.
        /// If not, an ArgumentOutOfRange exception will be thrown when trying to use statistical functions (e.g. CDF())
        /// </summary>
        public abstract bool ParametersValid { get; }

        /// <summary>
        /// Gets the mean of the distribution.
        /// </summary>
        public abstract double Mean { get; }

        /// <summary>
        /// Gets the median of the distribution.
        /// </summary>
        public abstract double Median { get; }

        /// <summary>
        /// Gets the mode of the distribution.
        /// </summary>
        public abstract double Mode { get; }

        /// <summary>
        /// Gets the variance of the distribution.
        /// </summary>
        public double Variance
        {
            get { return Math.Pow(StandardDeviation, 2d); }
        }

        /// <summary>
        /// Gets the standard deviation of the distribution.
        /// </summary>
        public abstract double StandardDeviation { get; }

        /// <summary>
        /// Gets the coefficient of variation.
        /// </summary>
        public double CoefficientOfVariation
        {
            get { return StandardDeviation / Mean; }
        }

        /// <summary>
        /// Gets the skew of the distribution.
        /// </summary>
        public abstract double Skew { get; }

        /// <summary>
        /// Gets the kurtosis of the distribution.
        /// </summary>
        public abstract double Kurtosis { get; }

        /// <summary>
        /// Gets the minimum of the distribution.
        /// </summary>
        public abstract double Minimum { get; }

        /// <summary>
        /// Gets the maximum of the distribution.
        /// </summary>
        public abstract double Maximum { get; }

        /// <summary>
        /// Gets the minimum values allowable for each parameter.
        /// </summary>
        public abstract double[] MinimumOfParameters { get; }

        /// <summary>
        /// Gets the maximum values allowable for each parameter.
        /// </summary>
        public abstract double[] MaximumOfParameters { get; }

        /// <summary>
        /// Set the distribution parameters.
        /// </summary>
        /// <param name="parameters">Array of parameters.</param>
        public abstract void SetParameters(IList<double> parameters);

        /// <summary>
        /// Test to see if distribution parameters are valid.
        /// </summary>
        /// <param name="parameters">Array of parameters.</param>
        /// <param name="throwException">Boolean indicating whether to throw the exception or not.</param>
        /// <returns>Nothing if the parameters are valid and the exception if invalid parameters were found.</returns>
        public abstract ArgumentOutOfRangeException ValidateParameters(IList<double> parameters, bool throwException);

        /// <summary>
        /// The log likelihood function.
        /// </summary>
        /// <param name="sample">Sample of observed data.</param>
        /// <returns>
        /// Log likelihood.
        /// </returns>
        public double LogLikelihood(IList<double> sample)
        {
            var logLH = default(double);
            for (int i = 0; i < sample.Count; i++)
            {        
                logLH += LogPDF(sample[i]);
            }
            if (double.IsNaN(logLH) || double.IsInfinity(logLH)) return double.MinValue;
            return logLH;
        }

        /// <summary>
        /// The Log-Likelihood function for a single data point.
        /// </summary>
        /// <param name="value">A single observed data point.</param>
        /// <returns>
        /// Log likelihood.
        /// </returns>
        public double LogLikelihood(double value)
        {
            return LogPDF(value);
        }

        /// <summary>
        /// The log likelihood function for left censored data.
        /// </summary>
        /// <param name="threshold">The threshold.</param>
        /// <param name="numberBelow">The number of data points below the threshold.</param>
        public double LogLikelihood_LeftCensored(double threshold, long numberBelow)
        {
            return numberBelow * LogCDF(threshold);
        }

        /// <summary>
        /// The log likelihood function for right censored data.
        /// </summary>
        /// <param name="threshold">The threshold.</param>
        /// <param name="numberAbove">The number of data points above the threshold.</param>
        public double LogLikelihood_RightCensored(double threshold, long numberAbove)
        {
            return numberAbove * LogCCDF(threshold);
        }

        /// <summary>
        /// The log likelihood function for interval data.
        /// </summary>
        /// <param name="lowerLimit">The lower limit of the interval.</param>
        /// <param name="upperLimit">The upper limit of the interval.</param>
        public double LogLikelihood_Intervals(double lowerLimit, double upperLimit)
        {
            double interval = CDF(upperLimit) - CDF(lowerLimit);
            return Math.Log(interval);
        }

        /// <summary>
        /// The Probability Density Function (PDF) of the distribution evaluated at a point X.
        /// </summary>
        /// <param name="x">A single point in the distribution range.</param>
        /// <returns>The probability of X occurring in the distribution.</returns>
        /// <remarks>
        /// The PDF describes the probability that X will occur.
        /// </remarks>
        public abstract double PDF(double x);

        /// <summary>
        /// Returns the natural log of the PDF.
        /// </summary>
        /// <param name="x">A single point in the distribution range.</param>
        public virtual double LogPDF(double x)
        {
            double f = PDF(x);
            return Math.Log(f);
        }

        /// <summary>
        /// The Cumulative Distribution Function (CDF) for the distribution evaluated at a point X.
        /// </summary>
        /// <param name="x">A single point in the distribution range.</param>
        /// <returns>The non-exceedance probability given a point X.</returns>
        /// <remarks>
        /// The CDF describes the cumulative probability that a given value or any value smaller than it will occur.
        /// </remarks>
        public abstract double CDF(double x);

        /// <summary>
        /// Returns the hazard function (HF) of the distribution evaluated at a point x.
        /// </summary>
        /// <param name="x">A single point in the distribution range.</param>
        public virtual double HF(double x)
        {
            return PDF(x) / CCDF(x);
        }

        /// <summary>
        /// Returns the natural log of the CDF.
        /// </summary>
        /// <param name="x">A single point in the distribution range.</param>
        public virtual double LogCDF(double x)
        {
            double F = CDF(x);
            return Math.Log(F);
        }

        /// <summary>
        /// The complement of the CDF. This function is also known as the survival function.
        /// </summary>
        /// <param name="x">A single point in the distribution range.</param>
        public virtual double CCDF(double x)
        {
            return 1.0d - CDF(x);
        }

        /// <summary>
        /// Returns the natural log of the CCDF.
        /// </summary>
        /// <param name="x">A single point in the distribution range.</param>
        public virtual double LogCCDF(double x)
        {
            double cF = CCDF(x);
            return Math.Log(cF);
        }

        /// <summary>
        /// The Inverse Cumulative Distribution Function (InvCFD) of the distribution evaluated at a probability.
        /// </summary>
        /// <param name="probability">Probability between 0 and 1.</param>
        /// <returns>
        /// Returns for a given probability in the probability distribution of a random variable,
        /// the value at which the probability of the random variable is less than or equal to the
        /// given probability.
        /// </returns>
        /// <remarks>
        /// This function is also know as the Quantile Function.
        /// </remarks>
        public abstract double InverseCDF(double probability);

        /// <summary>
        /// The PDF evaluated over a list of x values.
        /// </summary>
        /// <param name="xValues">List of x values.</param>
        public double[] PDF(IList<double> xValues)
        {
            var result = new double[xValues.Count];
            for (int i = 0; i < xValues.Count; i++)
                result[i] = PDF(xValues[i]);
            return result;
        }

        /// <summary>
        /// The CDF evaluated over a list of x values.
        /// </summary>
        /// <param name="xValues">List of x values.</param>
        public double[] CDF(IList<double> xValues)
        {
            var result = new double[xValues.Count];
            for (int i = 0; i < xValues.Count; i++)
                result[i] = CDF(xValues[i]);
            return result;
        }

        /// <summary>
        /// The Inverse CDF evaluated over a list of probabilities.
        /// </summary>
        /// <param name="probabilities">List of probability values.</param>
        public double[] InverseCDF(IList<double> probabilities)
        {
            var result = new double[probabilities.Count];
            for (int i = 0; i < probabilities.Count; i++)
                result[i] = InverseCDF(probabilities[i]);
            return result;
        }

        /// <summary>
        /// Returns the central moments {Mean, Standard Deviation, Skew, and Kurtosis} of the distribution using numerical integration with Adaptive Simpson's rule. 
        /// </summary>
        /// <param name="tolerance">The desired tolerance for the solution. Default = ~Sqrt(Machine Epsilon), or 1E-8.</param>
        /// <returns>Mean, Standard Deviation, Skew, and Kurtosis.</returns>
        public virtual double[] CentralMoments(double tolerance = 1E-8)
        {
            double u1 = 0, u2 = 0, u3 = 0, u4 = 0;
            double a = InverseCDF(1E-16);
            double b = InverseCDF(1-1E-16);
            if (a >= b) return new double[] { a, double.NaN, double.NaN, double.NaN };

            // Mean
            var sr = new AdaptiveSimpsonsRule(x => { return x * PDF(x); }, a, b) { RelativeTolerance = tolerance, ReportFailure = false };
            sr.Integrate();
            u1 = sr.Status != IntegrationStatus.Failure ? sr.Result : double.NaN;
            // Standard Deviation
            sr = new AdaptiveSimpsonsRule(x => { return Math.Pow(x - u1, 2d) * PDF(x); }, a, b) { RelativeTolerance = tolerance, ReportFailure = false };
            sr.Integrate();
            u2 = sr.Status != IntegrationStatus.Failure ? sr.Result : double.NaN;
            u2 = Math.Sqrt(u2);
            // Skewness
            sr = new AdaptiveSimpsonsRule(x => { return Math.Pow((x - u1) / u2, 3d) * PDF(x); }, a, b) { RelativeTolerance = tolerance, ReportFailure = false };
            sr.Integrate();
            u3 = sr.Status != IntegrationStatus.Failure ? sr.Result : double.NaN;
            // Kurtosis
            sr = new AdaptiveSimpsonsRule(x => { return Math.Pow((x - u1) / u2, 4d) * PDF(x); }, a, b) { RelativeTolerance = tolerance, ReportFailure = false };
            sr.Integrate();
            u4 = sr.Status != IntegrationStatus.Failure ? sr.Result : double.NaN;

            return new double[] { u1, u2, u3, u4 };
        }

        /// <summary>
        /// Returns the central moments {Mean, Standard Deviation, Skew, and Kurtosis} of the distribution using numerical integration with Trapezoidal rule. 
        /// </summary>
        ///<param name="steps">Number of integration steps.</param>
        public virtual double[] CentralMoments(int steps = 200)
        {
            double a = InverseCDF(1E-8);
            double b = InverseCDF(1 - 1E-8);
            if (a >= b) return new double[] { a, double.NaN, double.NaN, double.NaN };

            var bins = Stratify.XValues(new StratificationOptions(a, b, steps));
            var dFx = new double[steps];
            double u1, u2, u3, u4;
            double sumU1 = 0;
            double sumU2 = 0;
            double sumU3 = 0;
            double sumU4 = 0;

            // First compute the mean and standard deviation
            dFx[0] = CDF(bins[0].UpperBound);
            sumU1 += bins[0].UpperBound * dFx[0];
            sumU2 += Math.Pow(bins[0].UpperBound, 2d) * dFx[0];
            for (int i = 1; i < steps - 1; i++)
            {
                dFx[i] = CDF(bins[i].UpperBound) - CDF(bins[i].LowerBound);
                sumU1 += bins[i].Midpoint * dFx[i];
                sumU2 += Math.Pow(bins[i].Midpoint, 2d) * dFx[i];
            }
            dFx[steps - 1] = 1 - CDF(bins.Last().LowerBound);
            sumU1 += bins.Last().LowerBound * dFx[steps - 1];
            sumU2 += Math.Pow(bins.Last().LowerBound, 2d) * dFx[steps - 1];
            u1 = sumU1;
            u2 = Math.Sqrt(sumU2 - Math.Pow(u1, 2d));

            // Then compute skewness and kurtosis
            sumU3 += Math.Pow((bins[0].UpperBound - u1) / u2, 3d) * dFx[0];
            sumU4 += Math.Pow((bins[0].UpperBound - u1) / u2, 4d) * dFx[0];
            for (int i = 1; i < steps - 1; i++)
            {
                sumU3 += Math.Pow((bins[i].Midpoint - u1) / u2, 3d) * dFx[i];
                sumU4 += Math.Pow((bins[i].Midpoint - u1) / u2, 4d) * dFx[i];
            }
            sumU3 += Math.Pow((bins.Last().LowerBound - u1) / u2, 3d) * dFx[steps - 1];
            sumU4 += Math.Pow((bins.Last().LowerBound - u1) / u2, 4d) * dFx[steps - 1];
            u3 = sumU3;
            u4 = sumU4;
            return new double[] { u1, u2, u3, u4 };
        }

        /// <summary>
        /// Returns the conditional mean of the distribution.
        /// </summary>
        /// <param name="a">The lower integration limit, a.</param>
        /// <param name="b">The upper integration limit, b.</param>
        /// <param name="tolerance">The desired tolerance for the solution. Default = 1E-8.</param>
        public virtual double ConditionalMean(double a, double b, double tolerance = 1E-8)
        {
            double l = CDF(a);
            double u = CDF(b);
            var sr = new AdaptiveSimpsonsRule(x => { return x * PDF(x); }, a, b) { RelativeTolerance = tolerance, ReportFailure = false };
            sr.Integrate();
            double e = sr.Status != IntegrationStatus.Failure ? sr.Result : double.NaN;
            return e / (u - l);
        }

        /// <summary>
        /// Returns the conditional variance of the distribution.
        /// </summary>
        /// <param name="a">The lower integration limit, a.</param>
        /// <param name="b">The upper integration limit, b.</param>
        /// <param name="mean">The mean for computing the central moment. Default = 0.</param>
        /// <param name="tolerance">The desired tolerance for the solution. Default = 1E-8.</param>
        public virtual double ConditionalVariance(double a, double b, double mean = 0, double tolerance = 1E-8)
        {
            double l = CDF(a);
            double u = CDF(b);
            var sr = new AdaptiveSimpsonsRule(x => { return Math.Pow(x - mean, 2d) * PDF(x); }, a, b) { RelativeTolerance = tolerance, ReportFailure = false };
            sr.Integrate();
            double e = sr.Status != IntegrationStatus.Failure ? sr.Result : double.NaN;
            return e / (u - l);
        }

        /// <summary>
        /// Returns the conditional skewness of the distribution.
        /// </summary>
        /// <param name="a">The lower integration limit, a.</param>
        /// <param name="b">The upper integration limit, b.</param>
        /// <param name="mean">The mean for computing the central moment. Default = 0.</param>
        /// <param name="tolerance">The desired tolerance for the solution. Default = 1E-8.</param>
        public virtual double ConditionalSkewness(double a, double b, double mean = 0, double tolerance = 1E-8)
        {
            double l = CDF(a);
            double u = CDF(b);
            var sr = new AdaptiveSimpsonsRule(x => { return Math.Pow(x - mean, 3d) * PDF(x); }, a, b) { RelativeTolerance = tolerance, ReportFailure = false };
            sr.Integrate();
            double e = sr.Status != IntegrationStatus.Failure ? sr.Result : double.NaN;
            return e / (u - l);
        }

        /// <summary>
        /// Returns the conditional kurtosis of the distribution.
        /// </summary>
        /// <param name="a">The lower integration limit, a.</param>
        /// <param name="b">The upper integration limit, b.</param>
        /// <param name="mean">The mean for computing the central moment. Default = 0.</param>
        /// <param name="tolerance">The desired tolerance for the solution. Default = 1E-8.</param>
        public virtual double ConditionalKurtosis(double a, double b, double mean = 0, double tolerance = 1E-8)
        {
            double l = CDF(a);
            double u = CDF(b);
            var sr = new AdaptiveSimpsonsRule(x => { return Math.Pow(x - mean, 4d) * PDF(x); }, a, b) { RelativeTolerance = tolerance, ReportFailure = false };
            sr.Integrate();
            double e = sr.Status != IntegrationStatus.Failure ? sr.Result : double.NaN;
            return e / (u - l);
        }

        /// <summary>
        /// Returns the conditional expected value at the given confidence level. 
        /// </summary>
        /// <param name="alpha">The threshold confidence level, α.</param>
        /// <param name="tolerance">The desired tolerance for the solution. Default = ~Sqrt(Machine Epsilon), or 1E-8.</param>
        public virtual double ConditionalExpectedValue(double alpha, double tolerance = 1E-8)
        {
            double a = InverseCDF(alpha);
            double b = InverseCDF(1 - 1E-16);
            var sr = new AdaptiveSimpsonsRule(x => { return x * PDF(x); }, a, b) { RelativeTolerance = tolerance, ReportFailure = false };
            sr.Integrate();
            double e = sr.Status != IntegrationStatus.Failure ? sr.Result : double.NaN;
            return e / (1 - alpha);
        }

        /// <summary>
        /// Create a PDF table for graphing purposes.
        /// The bounds of the table are automatically determined.
        /// </summary>
        /// <returns>A 2-column array of X and probability density.</returns>
        public virtual double[,] CreatePDFGraph()
        {
            var XValues = Stratify.XValues(new StratificationOptions(InverseCDF(0.0005d), InverseCDF(0.9995d), 1000, false));
            var graph = new double[XValues.Count, 2];
            for (int i = 0; i < XValues.Count; i++)
            {
                graph[i, 0] = XValues[i].Midpoint;
                graph[i, 1] = PDF(XValues[i].Midpoint);
            }
            return graph;
        }

        /// <summary>
        /// Create a PDF table for graphing purposes.
        /// </summary>
        /// <param name="XValues">Stratified X values.</param>
        /// <returns>A 2-column array of X and probability density.</returns>
        public double[,] CreatePDFGraph(List<StratificationBin> XValues)
        {
            var graph = new double[XValues.Count, 2];
            for (int i = 0; i < XValues.Count; i++)
            {
                graph[i, 0] = XValues[i].Midpoint;
                graph[i, 1] = PDF(XValues[i].Midpoint);
            }
            return graph;
        }

        /// <summary>
        /// Create a PDF table for graphing purposes.
        /// </summary>
        /// <param name="XValues">Array of stratified X values.</param>
        /// <returns>A 2-column array of X and probability density.</returns>
        public double[,] CreatePDFGraph(List<StratificationBin>[] XValues)
        {
            var totalBinCount = default(int);
            for (int i = 0; i < XValues.Length; i++)
                totalBinCount += XValues[i].Count;
            var graph = new double[totalBinCount, 2];
            int binCounter = 0;
            for (int i = 0; i < XValues.Length; i++)
            {
                for (int j = 0; j < XValues[i].Count; j++)
                {
                    graph[binCounter, 0] = XValues[i][j].Midpoint;
                    graph[binCounter, 1] = PDF(XValues[i][j].Midpoint);
                    binCounter += 1;
                }
                binCounter += 1;
            }
            return graph;
        }

        /// <summary>
        /// Create a CDF table for graphing purposes.
        /// The bounds of the table are automatically determined.
        /// </summary>
        /// <returns>A 2-column array of X and non-exceedance probability.</returns>
        public double[,] CreateCDFGraph()
        {
            var XValues = Stratify.XValues(new StratificationOptions(InverseCDF(0.0005d), InverseCDF(0.9995d), 1000, false));
            var graph = new double[XValues.Count, 2];
            for (int i = 0; i < XValues.Count; i++)
            {
                graph[i, 0] = XValues[i].Midpoint;
                graph[i, 1] = CDF(XValues[i].Midpoint);
            }
            return graph;
        }

        /// <summary>
        /// Create a CDF table for graphing purposes.
        /// </summary>
        /// <param name="XValues">Stratified X values.</param>
        /// <returns>A 2-column array of X and non-exceedance probability.</returns>
        public double[,] CreateCDFGraph(List<StratificationBin> XValues)
        {
            var graph = new double[XValues.Count, 2];
            for (int i = 0; i < XValues.Count; i++)
            {
                graph[i, 0] = XValues[i].Midpoint;
                graph[i, 1] = CDF(XValues[i].Midpoint);
            }
            return graph;
        }

        /// <summary>
        /// Create a CDF table for graphing purposes.
        /// </summary>
        /// <param name="XValues">Array of stratified X values.</param>
        /// <returns>A 2-column array of X and non-exceedance probability.</returns>
        public double[,] CreateCDFGraph(List<StratificationBin>[] XValues)
        {
            var totalBinCount = default(int);
            for (int i = 0; i < XValues.Length; i++)
                totalBinCount += XValues[i].Count;
            var graph = new double[totalBinCount, 2];
            int binCounter = 0;
            for (int i = 0; i < XValues.Length; i++)
            {
                for (int j = 0; j < XValues[i].Count; j++)
                {
                    graph[binCounter, 0] = XValues[i][j].Midpoint;
                    graph[binCounter, 1] = CDF(XValues[i][j].Midpoint);
                    binCounter += 1;
                }
                binCounter += 1;
            }
            return graph;
        }

        /// <summary>
        /// Create an Inverse CDF table for graphing purposes.
        /// The bounds of the table are automatically determined.
        /// </summary>
        /// <returns>A 2-column array of X and non-exceedance probability.</returns>
        public double[,] CreateInverseCDFGraph()
        {
            var PValues = Stratify.Probabilities(new StratificationOptions(CDF(InverseCDF(0.0005d)), CDF(InverseCDF(0.9995d)), 1000, true));
            var graph = new double[PValues.Count, 2];
            for (int i = 0; i < PValues.Count; i++)
            {
                graph[i, 0] = InverseCDF(PValues[i].Midpoint);
                graph[i, 1] = PValues[i].Midpoint;
            }
            return graph;
        }

        /// <summary>
        /// Create an Inverse CDF table for graphing purposes.
        /// </summary>
        /// <param name="PValues">Stratified probabilities.</param>
        /// <returns>A 2-column array of X and non-exceedance probability.</returns>
        public double[,] CreateInverseCDFGraph(List<StratificationBin> PValues)
        {
            var graph = new double[PValues.Count, 2];
            for (int i = 0; i < PValues.Count; i++)
            {
                graph[i, 0] = InverseCDF(PValues[i].Midpoint);
                graph[i, 1] = PValues[i].Midpoint;
            }
            return graph;
        }

        /// <summary>
        /// Create an Inverse CDF table for graphing purposes.
        /// </summary>
        /// <param name="PValues">Array of stratified probabilities.</param>
        /// <returns>A 2-column array of X and non-exceedance probability.</returns>
        public double[,] CreateInverseCDFGraph(List<StratificationBin>[] PValues)
        {
            var totalBinCount = default(int);
            for (int i = 0; i < PValues.Length; i++)
                totalBinCount += PValues[i].Count;
            var graph = new double[totalBinCount, 2];
            int binCounter = 0;
            for (int i = 0; i < PValues.Length; i++)
            {
                for (int j = 0; j < PValues[i].Count; j++)
                {
                    graph[binCounter, 0] = InverseCDF(PValues[i][j].Midpoint);
                    graph[binCounter, 1] = PValues[i][j].Midpoint;
                    binCounter += 1;
                }
            }
            return graph;
        }

        /// <summary>
        /// Generate random values of a distribution given a sample size.
        /// </summary>
        /// <param name="samplesize"> Size of random sample to generate. </param>
        /// <returns>
        /// Array of random values.
        /// </returns>
        /// <remarks>
        /// The random number generator seed is based on the current date and time according to your system.
        /// </remarks>
        public virtual double[] GenerateRandomValues(int samplesize)
        {
            // Create seed based on date and time
            // Create PRNG for generating random numbers
            var r = new MersenneTwister();
            var sample = new double[samplesize];
            // Generate values
            for (int i = 0; i < samplesize; i++)
                sample[i] = InverseCDF(r.NextDouble());
            // Return array of random values
            return sample;
        }

        /// <summary>
        /// Generate random values of a distribution given a sample size based on a user-defined seed.
        /// </summary>
        /// <param name="seed">Seed for random number generator.</param>
        /// <param name="samplesize"> Size of random sample to generate. </param>
        /// <returns>
        /// Array of random values.
        /// </returns>
        public virtual double[] GenerateRandomValues(int seed, int samplesize)
        {
            // Create PRNG for generating random numbers
            var r = new MersenneTwister(seed);
            var sample = new double[samplesize];
            // Generate values
            for (int i = 0; i < samplesize; i++)
                sample[i] = InverseCDF(r.NextDouble());
            // Return array of random values
            return sample;
        }

        /// <summary>
        /// Creates a copy of the distribution.
        /// </summary>
        public abstract UnivariateDistributionBase Clone();

        /// <summary>
        /// Returns the distribution as XElement (XML). 
        /// </summary>
        public virtual XElement ToXElement()
        {
            var result = new XElement("Distribution");
            result.SetAttributeValue(nameof(Type), Type.ToString());
            var names = GetParameterPropertyNames;
            var parms = GetParameters;
            for (int i = 0; i < NumberOfParameters; i++)
            {
                result.SetAttributeValue(names[i], parms[i].ToString("G17", CultureInfo.InvariantCulture));
            }
            return result;
        }

        /// <summary>
        /// The equality operator to allow comparison of continuous distribution parameters. If distributions are equal the operator will return true, otherwise false.
        /// </summary>
        /// <param name="left">Continuous distribution on the left side of the operator.</param>
        /// <param name="right">Continuous distribution on the right side of the operator.</param>
        /// <returns>Boolean indicating if the equality operator results in a true statement.</returns>
        public static bool operator ==(UnivariateDistributionBase left, UnivariateDistributionBase right)
        {
            // Check for null arguments. Keep in mind null == null
            if (left is null && right is null)
            {
                return true;
            }
            else if (left is null)
            {
                return false;
            }
            else if (right is null)
            {
                return false;
            }
            // Check to see if they are the same distribution
            if (left.Type != right.Type)
                return false;
            // Test for equality of parameters
            // Compare parameters here. 
            // Need to have special cases for kernel density, univariate, and bivariate distributions. Everything else can use the GetParameters method.
            if (left.Type == UnivariateDistributionType.Empirical)
            {
                EmpiricalDistribution leftTyped = (EmpiricalDistribution)left;
                EmpiricalDistribution rightTyped = (EmpiricalDistribution)right;
                // Check X values for equality
                var leftValues = leftTyped.XValues;
                var rightValues = rightTyped.XValues;
                if (leftValues.Count != rightValues.Count)
                    return false;
                for (int i = 0; i < leftValues.Count; i++)
                {
                    if (Math.Abs(leftValues[i] - rightValues[i]) > Tools.DoubleMachineEpsilon)
                        return false;
                }
                // Check P values for equality, no need to test count since it was tested with X values
                leftValues = leftTyped.ProbabilityValues;
                rightValues = rightTyped.ProbabilityValues;
                for (int i = 0; i < leftValues.Count; i++)
                {
                    if (Math.Abs(leftValues[i] - rightValues[i]) > Tools.DoubleMachineEpsilon)
                        return false;
                }
            }
            else if (left.Type == UnivariateDistributionType.KernelDensity)
            {
                throw new NotImplementedException();
            }
            else
            {
                var leftParams = left.GetParameters;
                var rightParams = right.GetParameters;
                for (int i = 0; i < leftParams.Count(); i++)
                {
                    if (Math.Abs(leftParams[i] - rightParams[i]) > Tools.DoubleMachineEpsilon)
                        return false;
                }
            }
            // All tests passed
            return true;
            // 
        }

        /// <summary>
        /// The non-equality operator to allow comparison of continuous distribution parameters. If distributions are not equal the operator will return true, otherwise false.
        /// </summary>
        /// <param name="left">Continuous distribution on the left side of the operator.</param>
        /// <param name="right">Continuous distribution on the right side of the operator.</param>
        /// <returns>Boolean indicating if the non-equality operator results in a true statement.</returns>
        public static bool operator !=(UnivariateDistributionBase left, UnivariateDistributionBase right)
        {
            return !(left == right);
        }

    }
}