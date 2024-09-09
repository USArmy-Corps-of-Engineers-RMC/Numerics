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

using System;
using System.Collections.Generic;
using Numerics.Mathematics.Optimization;

namespace Numerics.Data.Statistics
{
    /// <summary>
    /// Class for performing Box-Cox transformation.
    /// </summary>
    /// <remarks>
    ///     <b> Authors: </b>
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// <para>
    /// <b> Description: </b>
    /// This method transforms non-normal dependent variables into a normal shape.
    /// </para>
    /// </remarks>
    public class BoxCox
    {

        /// <summary>
        /// Fit the transformation parameters using maximum likelihood estimation.
        /// </summary>
        /// <param name="values">The list of values to transform.</param>
        /// <param name="lambda1">Output. The transformation exponent. Range -5 to +5.</param>
        /// <param name="lambda2">Output. The transformation shift for negative values.</param>
        /// <remarks>
        /// https://www.rdocumentation.org/packages/EnvStats/versions/2.4.0/topics/boxcox
        /// </remarks>
        public static void FitLambda(IList<double> values, out double lambda1, out double lambda2)
        {
            int n = values.Count;
            double l1 = 0d;
            double l2 = 0d;
            double min = Statistics.Minimum(values);
            if (min <= 0d) l2 = 0d - min + Tools.DoubleMachineEpsilon;
            
            Func<double, double> func = lambda =>
            {
                return LogLikelihood(values, lambda, l2);
            };

            // Solve with Brent 
            var brent = new BrentSearch(func, -5d, 5d);
            brent.Maximize();
            l1 = brent.BestParameterSet.Values[0];
            // Set parameters
            lambda1 = l1;
            lambda2 = l2;
        }

        /// <summary>
        /// The log-likelihood function. The transformed observations are assumed to come from a
        /// normal distribution. The change of variable formula is used to write the log-likelihood function.
        /// </summary>
        /// <param name="values">The list of values to transform.</param>
        /// <param name="lambda1">The transformation exponent. Range -5 to +5.</param>
        /// <param name="lambda2">The transformation shift for negative values.</param>
        /// <returns>
        /// The value of log-likelihood function evaluated at the given values and lambdas.
        /// </returns>
        public static double LogLikelihood(IList<double> values, double lambda1, double lambda2)
        {
            int n = values.Count;
            var y = new double[n];
            double mu = 0d;
            var sumX = 0d;
            for (int i = 0; i < n; i++)
            {
                y[i] = Transform(values[i], lambda1, lambda2);
                mu += y[i];
                sumX += Math.Log(values[i] + lambda2);
            }
            mu = mu / n;
            double sse = 0d;
            for (int i = 0; i < n; i++)
                sse += Math.Pow(y[i] - mu, 2d);
            double sigma = Math.Sqrt(sse / n);
            double ll = -n / 2.0d * Tools.LogSqrt2PI - n / 2.0d * Math.Log(sigma * sigma) - 1.0d / (2d * sigma * sigma) * sse + (lambda1 - 1d) * sumX;
            return ll;
        }

        /// <summary>
        /// Returns the Box-Cox transformation of the value.
        /// </summary>
        /// <param name="value">The value to transform.</param>
        /// <param name="lambda1">The transformation exponent. Range -5 to +5.</param>
        /// <param name="lambda2">The transformation shift for negative values.</param>
        public static double Transform(double value, double lambda1, double lambda2 = 0.0d)
        {
            if (Math.Abs(lambda1) > 5d) return double.NaN;
            if (lambda1 == 0d) return Math.Log(value + lambda2);
            return (Math.Pow(value + lambda2, lambda1) - 1.0d) / lambda1;
        }

        /// <summary>
        /// Returns the Box-Cox transformation of each value in the list.
        /// </summary>
        /// <param name="values">The list of values to transform.</param>
        /// <param name="lambda1">The transformation exponent. Range -5 to +5.</param>
        /// <param name="lambda2">The transformation shift for negative values.</param>
        public static List<double> Transform(IList<double> values, double lambda1, double lambda2 = 0.0d)
        {
            var newValues = new List<double>();
            for (int i = 0; i < values.Count; i++)
                newValues.Add(Transform(values[i], lambda1, lambda2));
            return newValues;
        }

        /// <summary>
        /// Returns the reverse of the Box-Cox transformed value.
        /// </summary>
        /// <param name="value">The value to reverse transform.</param>
        /// <param name="lambda1">The transformation exponent. Range -5 to +5.</param>
        /// <param name="lambda2">The transformation shift for negative values.</param>
        public static double ReverseTransform(double value, double lambda1, double lambda2 = 0.0d)
        {
            if (Math.Abs(lambda1) > 5d) return double.NaN;
            if (lambda1 == 0d) return Math.Exp(value) - lambda2;
            return Math.Pow(value * lambda1 + 1.0d, 1.0d / lambda1) - lambda2;
        }

        /// <summary>
        /// Returns the reverse of each Box-Cox transformed value in the list.
        /// </summary>
        /// <param name="values">The list of values to reverse transform.</param>
        /// <param name="lambda1">The transformation exponent. Range -5 to +5.</param>
        /// <param name="lambda2">The transformation shift for negative values.</param>
        public static List<double> ReverseTransform(IList<double> values, double lambda1, double lambda2 = 0.0d)
        {
            var newValues = new List<double>();
            for (int i = 0; i < values.Count; i++)
                newValues.Add(ReverseTransform(values[i], lambda1, lambda2));
            return newValues;
        }
    }
}