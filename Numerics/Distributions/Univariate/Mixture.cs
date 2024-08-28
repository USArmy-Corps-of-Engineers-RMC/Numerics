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

using Numerics.Data;
using Numerics.Mathematics.Optimization;
using Numerics.Mathematics.RootFinding;
using Numerics.Sampling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace Numerics.Distributions
{
    /// <summary>
    /// A Mixture distribution.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     <b> Authors: </b>
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    /// <b> References: </b>
    /// </para>
    /// <para>
    /// <see href = "https://en.wikipedia.org/wiki/Mixture_distribution" />
    /// </para>
    /// </remarks>
    [Serializable]
    public class Mixture : UnivariateDistributionBase, IEstimation, IMaximumLikelihoodEstimation, IBootstrappable
    {

        /// <summary>
        /// Construct new mixture distribution.
        /// </summary>
        /// <param name="weights">The mixture weights.</param>
        /// <param name="distributions">The mixture distributions.</param>
        public Mixture(double[] weights, UnivariateDistributionBase[] distributions)
        {
            SetParameters(weights, distributions);
        }

        /// <summary>
        /// Construct new mixture distribution.
        /// </summary>
        /// <param name="weights">The mixture weights.</param>
        /// <param name="distributions">The mixture distributions.</param>
        public Mixture(double[] weights, IUnivariateDistribution[] distributions)
        {
            SetParameters(weights, distributions);
        }

        private double[] _weights;
        private UnivariateDistributionBase[] _distributions;
        private EmpiricalDistribution _inverseCDF;
        private bool _momentsComputed = false;
        private double u1, u2, u3, u4;
        private bool _inverseCDFCreated = false;

        /// <summary>
        /// Returns the array of distribution weights.
        /// </summary>
        public double[] Weights => _weights;

        /// <summary>
        /// Returns the array of univariate probability distributions.
        /// </summary>
        public UnivariateDistributionBase[] Distributions => _distributions;

        /// <summary>
        /// Determines whether to use assign a weight to all data points less than or equal to zero.
        /// </summary>
        public bool IsZeroInflated { get; set; } = false;

        /// <summary>
        /// The zero-value weight used if the mixture is zero-inflated.
        /// </summary>
        public double ZeroWeight { get; set; } = 0.0;

        /// <summary>
        /// Determines the interpolation transform for the X-values.
        /// </summary>
        public Transform XTransform { get; set; } = Transform.None;

        /// <summary>
        /// Determines the interpolation transform for the Probability-values.
        /// </summary>
        public Transform ProbabilityTransform { get; set; } = Transform.NormalZ;

        /// <inheritdoc/>
        public override int NumberOfParameters
        {
            get
            {
                int sum = IsZeroInflated ? 1 : 0;
                sum += Distributions.Count();
                for (int i = 0; i < Distributions.Count(); i++)
                    sum += Distributions[i].NumberOfParameters;
                return sum;
            }
        }

        /// <inheritdoc/>
        public override UnivariateDistributionType Type => UnivariateDistributionType.Mixture;

        /// <inheritdoc/>
        public override string DisplayName => "Mixture";

        /// <inheritdoc/>
        public override string ShortDisplayName => "MIX";

        /// <inheritdoc/>
        public override string[,] ParametersToString
        {
            get
            {
                var parmString = new string[2, 2];
                string Wstring = "{";
                string Dstring = "{";
                for (int i = 1; i < Weights.Count() - 1; i++)
                {
                    Wstring += Weights[i].ToString();
                    Dstring += Distributions[i].DisplayName;
                    if (i < Weights.Count() - 2)
                    {
                        Wstring += ",";
                        Dstring += ",";
                    }
                }
                Wstring += "}";
                Dstring += "}";
                parmString[0, 0] = "Weights";
                parmString[1, 0] = "Distributions";
                parmString[0, 1] = Wstring;
                parmString[1, 1] = Dstring;
                return parmString;
            }
        }

        /// <inheritdoc/>
        public override string[] ParameterNames
        {
            get
            {
                var result = new List<string>();
                for (int i = 1; i <= Distributions.Count(); i++)
                {
                    result.Add("Weight " + i.ToString());
                }
                for (int i = 0; i < Distributions.Count(); i++)
                {
                    for (int j = 0; j < Distributions[i].ParameterNames.Length; j++)
                    {
                        result.Add("D" + (i + 1).ToString() + " " + Distributions[i].ParameterNames[j]);
                    }
                }
                return result.ToArray();
            }
        }


        /// <inheritdoc/>
        public override string[] ParameterNamesShortForm
        {
            get
            {
                var result = new List<string>();
                for (int i = 1; i <= Distributions.Count(); i++)
                {
                    result.Add("W" + i.ToString());
                }
                for (int i = 0; i < Distributions.Count(); i++)
                {
                    for (int j = 0; j < Distributions[i].ParameterNamesShortForm.Length; j++)
                    {
                        result.Add("D" + (i + 1).ToString() + " " + Distributions[i].ParameterNamesShortForm[j]);
                    }
                }
                return result.ToArray();
            }
        }

        /// <inheritdoc/>
        public override double[] GetParameters
        {
            get
            {
                var result = new List<double>();
                result.AddRange(Weights);
                for (int i = 0; i < Distributions.Count(); i++)
                {
                    result.AddRange(Distributions[i].GetParameters);
                }                  
                return result.ToArray();
            }
        }

        /// <inheritdoc/>
        public override string[] GetParameterPropertyNames
        {
            get { return [nameof(Weights), nameof(Distributions)]; }
        }

        /// <summary>
        /// Compute central moments of the distribution.
        /// </summary>
        private void ComputeMoments()
        {
            var mom = CentralMoments(1000);
            u1 = mom[0];
            u2 = mom[1];
            u3 = mom[2];
            u4 = mom[3];
            _momentsComputed = true;
        }

        /// <inheritdoc/>
        public override double Mean
        {
            get
            {
                if (!_momentsComputed) 
                    ComputeMoments();
                return u1;
            }
        }

        /// <inheritdoc/>
        public override double Median
        {
            get { return InverseCDF(0.5d); }
        }

        /// <inheritdoc/>
        public override double Mode
        {
            get
            {
                var brent = new BrentSearch(PDF, InverseCDF(0.001), InverseCDF(0.999));
                brent.Maximize();
                return brent.BestParameterSet.Values[0];
            }
        }

        /// <inheritdoc/>
        public override double StandardDeviation
        {
            get
            {
                if (!_momentsComputed) 
                    ComputeMoments();
                return u2;
            }
        }

        /// <inheritdoc/>
        public override double Skewness
        {
            get
            {
                if (!_momentsComputed) 
                    ComputeMoments();
                return u3;
            }
        }

        /// <inheritdoc/>
        public override double Kurtosis
        {
            get
            {
                if (!_momentsComputed) 
                    ComputeMoments();
                return u4;
            }
        }

        /// <inheritdoc/>
        public override double Minimum
        {
            get { return Distributions.Min(p => p.Minimum); }
        }

        /// <inheritdoc/>
        public override double Maximum
        {
            get { return Distributions.Max(p => p.Maximum); }
        }

        /// <inheritdoc/>
        public override double[] MinimumOfParameters
        { 
            get 
            {
                var result = new List<double>();
                if (IsZeroInflated) { result.Add(0.0); }
                for (int i = 0; i < Distributions.Count(); i++)
                {
                    result.Add(0.0);
                }
                for (int i = 0; i < Distributions.Count(); i++)
                {
                    result.AddRange(Distributions[i].MinimumOfParameters);
                }
                return result.ToArray(); 
            }
        }

        /// <inheritdoc/>
        public override double[] MaximumOfParameters
        {
            get
            {
                var result = new List<double>();
                if (IsZeroInflated) { result.Add(1.0); }
                for (int i = 0; i < Distributions.Count(); i++)
                {
                    result.Add(1.0);
                }
                for (int i = 0; i < Distributions.Count(); i++)
                {
                    result.AddRange(Distributions[i].MaximumOfParameters);
                }
                return result.ToArray();
            }
        }

        /// <inheritdoc/>
        public void Estimate(IList<double> sample, ParameterEstimationMethod estimationMethod)
        {
            if (estimationMethod == ParameterEstimationMethod.MaximumLikelihood)
            {
                SetParameters(MLE(sample));
            }
        }

        /// <inheritdoc/>
        public IUnivariateDistribution Bootstrap(ParameterEstimationMethod estimationMethod, int sampleSize, int seed = -1)
        {
            var newDistribution = (Mixture)Clone();
            var sample = newDistribution.GenerateRandomValues(sampleSize, seed);
            newDistribution.Estimate(sample, estimationMethod);
            if (newDistribution.ParametersValid == false)
                throw new Exception("Bootstrapped distribution parameters are invalid.");
            return newDistribution;
        }

        /// <summary>
        /// Set the distribution parameters.
        /// </summary>
        /// <param name="weights">The mixture weights.</param>
        /// <param name="distributions">The mixture distributions.</param>
        public void SetParameters(double[] weights, UnivariateDistributionBase[] distributions)
        {
            if (weights == null) throw new ArgumentNullException(nameof(Weights));
            if (distributions == null) throw new ArgumentNullException(nameof(Distributions));
            if (weights.Length != distributions.Length)
                throw new ArgumentException("The weight and distribution arrays must have the same length.", nameof(Weights));

            _weights = weights;
            _distributions = distributions;
            _momentsComputed = false;
            _inverseCDFCreated = false;
        }

        /// <summary>
        /// Set the distribution parameters.
        /// </summary>
        /// <param name="weights">The mixture weights.</param>
        /// <param name="distributions">The mixture distributions.</param>
        public void SetParameters(double[] weights, IUnivariateDistribution[] distributions)
        {
            if (weights == null) throw new ArgumentNullException(nameof(Weights));
            if (distributions == null) throw new ArgumentNullException(nameof(Distributions));
            if (weights.Length != distributions.Length)
                throw new ArgumentException("The weight and distribution arrays must have the same length.", nameof(Weights));

            _weights = weights;
            _distributions = new UnivariateDistributionBase[distributions.Length];
            for (int i = 0; i < distributions.Length; i++)
            {
                _distributions[i] = (UnivariateDistributionBase)distributions[i];
            }
            _momentsComputed = false;
            _inverseCDFCreated = false;
        }

        /// <inheritdoc/>
        public override void SetParameters(IList<double> parameters)
        {
            // Set weights         
            _weights = parameters.ToArray().Subset(0, Distributions.Length - 1);
            int t = Distributions.Length;

            // Set distribution parameters
            for (int i = 0; i < Distributions.Count(); i++)
            {
                var parms = new List<double>();
                for (int j = t; j < t + Distributions[i].NumberOfParameters; j++)
                {
                    parms.Add(parameters[j]);
                }
                Distributions[i].SetParameters(parms);
                t += Distributions[i].NumberOfParameters;
            }

            //SetParametersFromArray(parameters.ToArray());
            //if (Distributions == null || Distributions.Count() == 0) return;
            //if (Distributions.Count() == 1 && parameters.Count() == Distributions[0].NumberOfParameters)
            //{
            //    if (IsZeroInflated)
            //    {
            //        Weights[0] = 1 - ZeroWeight;
            //    }
            //    else
            //    {
            //        Weights[0] = 1;
            //    }
            //    Distributions[0].SetParameters(parameters);
            //}
            //else if (Distributions.Count() == 2)
            //{
            //    // Get the weights
            //    int k = Distributions.Count();
            //    int t = 1; // keep track of parameter index

            //    // Get weights
            //    if (IsZeroInflated)
            //    {
            //        Weights[0] = parameters[0];
            //        Weights[1] = 1d - parameters[0];
            //        Weights[0] *= (1d - ZeroWeight);
            //        Weights[1] *= (1d - ZeroWeight);
            //        parameters[0] = Weights[0];
            //    }
            //    else
            //    {
            //        Weights[0] = parameters[0];
            //        Weights[1] = 1d - parameters[0];
            //    }

            //    // Set distribution parameters
            //    for (int i = 0; i < Distributions.Count(); i++)
            //    {
            //        var parms = new List<double>();
            //        for (int j = t; j < t + Distributions[i].NumberOfParameters; j++)
            //        {
            //            parms.Add(parameters[j]);
            //        }
            //        Distributions[i].SetParameters(parms);
            //        t += Distributions[i].NumberOfParameters;
            //    }
            //}

            //// Validate parameters
            //_parametersValid = ValidateParameters(parameters, false) is null;
            //_momentsComputed = false;
            //_inverseCDFCreated = false;
        }


        public void SetParametersFromArray(double[] parameters)
        {
            if (Distributions == null || Distributions.Count() == 0) return;
            if (Distributions.Count() == 1 && parameters.Length == Distributions[0].NumberOfParameters)
            {
                if (IsZeroInflated)
                {
                    Weights[0] = 1 - ZeroWeight;
                }
                else
                {
                    Weights[0] = 1;
                }
                Distributions[0].SetParameters(parameters);
            }
            else
            {
                bool sortWeights = true;
                UnivariateDistributionType type = Distributions[0].Type;
                for (int i = 1; i < Distributions.Count(); i++)
                {
                    if (Distributions[i].Type != type) { sortWeights = false; break; }
                }
                    

                // Get the weights
                int k = Distributions.Count();
                int t = 0; // keep track of parameter index

                // Get weights
                double c = 0;
                for (int i = 0; i < k; i++)
                {
                    Weights[i] = parameters[i];
                    c += Weights[i];
                    t++;
                }

                if (c > 0)
                {
                    // Get normalization constant
                    c = IsZeroInflated ? (1d - ZeroWeight) / c : 1d / c;
                    // Normalize weights
                    for (int i = 0; i < k; i++)
                    {
                        Weights[i] *= c;
                        parameters[i] = Weights[i];
                    }
                }
                else
                {
                    // If weights sum to 0, reset to be uniformly distributed
                    double w = IsZeroInflated ? (1d - ZeroWeight) / k : 1d / k;
                    for (int i = 0; i < k; i++)
                    {
                        Weights[i] = w;
                        parameters[i] = Weights[i];
                    }
                }
                if (sortWeights == true)
                {
                    Array.Sort(Weights);
                    for (int i = 0; i < k; i++)
                    {
                        parameters[i] = Weights[i];
                    }
                }

                // Set distribution parameters
                for (int i = 0; i < Distributions.Count(); i++)
                {
                    var parms = new List<double>();
                    for (int j = t; j < t + Distributions[i].NumberOfParameters; j++)
                    {
                        parms.Add(parameters[j]);
                    }
                    Distributions[i].SetParameters(parms);
                    t += Distributions[i].NumberOfParameters;
                }
            }

            // Validate parameters
            _parametersValid = ValidateParameters(parameters, false) is null;
            _momentsComputed = false;
            _inverseCDFCreated = false;
        }

        /// <inheritdoc/>
        public override ArgumentOutOfRangeException ValidateParameters(IList<double> parameters, bool throwException)
        {
            // Check if weights are between 0 and 1.
            if (IsZeroInflated && (ZeroWeight < 0.0 || ZeroWeight > 1.0))
            {
                if (throwException)
                    throw new ArgumentOutOfRangeException(nameof(ZeroWeight), "The zero value weight must be between 0 and 1.");
                return new ArgumentOutOfRangeException(nameof(ZeroWeight), "The zero value weight must be between 0 and 1.");
            }
            for (int i = 0; i < Distributions.Count(); i++)
            {
                if (Weights[i] < 0.0 || Weights[i] > 1.0)
                {
                    if (throwException)
                        throw new ArgumentOutOfRangeException(nameof(Weights), "The weights must be between 0 and 1.");
                    return new ArgumentOutOfRangeException(nameof(Weights), "The weights must be between 0 and 1.");
                }
            }
            // Check if weights sum to 1.
            double sum = IsZeroInflated ? ZeroWeight : 0.0;
            for (int i = 0; i < Distributions.Count(); i++)
                sum += Weights[i];
            if (sum.AlmostEquals(1d) == false)
            {
                if (throwException)
                    throw new ArgumentOutOfRangeException(nameof(Weights), "The weights must sum to 1.0.");
                return new ArgumentOutOfRangeException(nameof(Weights), "The weights must sum to 1.0.");
            }
            // Check if distributions are valid
            for (int i = 0; i < Distributions.Count(); i++)
            {
                if (Distributions[i].ParametersValid == false)
                {
                    if (throwException)
                        throw new ArgumentOutOfRangeException(nameof(Distributions), "Distribution " + (i + 1).ToString() + " has invalid parameters.");
                    return new ArgumentOutOfRangeException(nameof(Distributions), "Distribution " + (i + 1).ToString() + " has invalid parameters.");
                }
            }
            return null;
        }

        /// <inheritdoc/>
        public Tuple<double[], double[], double[]> GetParameterConstraints(IList<double> sample)
        {
            var initialVals = new double[NumberOfParameters];
            var lowerVals = new double[NumberOfParameters];
            var upperVals = new double[NumberOfParameters];

            // Weights are first
            int t = 0;
            for (int i = 0; i < Distributions.Count(); i++)
            {
                initialVals[i] = 0.5;
                lowerVals[i] = 0.0;
                upperVals[i] = 1.0;
                t += 1;
            }

            for (int i = 0; i < Distributions.Count(); i++)
            {
                var tuple = ((IMaximumLikelihoodEstimation)Distributions[i]).GetParameterConstraints(sample);
                var initials = tuple.Item1;
                var lowers = tuple.Item2;
                var uppers = tuple.Item3;

                for (int j = t; j < t + Distributions[i].NumberOfParameters; j++)
                {
                    initialVals[j] = initials[j - t];
                    lowerVals[j] = lowers[j - t];
                    upperVals[j] = uppers[j - t];
                }
                t += Distributions[i].NumberOfParameters;
            }
            return new Tuple<double[], double[], double[]>(initialVals, lowerVals, upperVals);
        }

        /// <inheritdoc/>
        public double[] MLE(IList<double> sample)
        {
            // Set constraints
            var tuple = GetParameterConstraints(sample);
            var Initials = tuple.Item1;
            var Lowers = tuple.Item2;
            var Uppers = tuple.Item3;

            // Solve using Nelder-Mead (Downhill Simplex)
            double logLH(double[] parameters)
            {
                var dist = (Mixture)Clone();
                dist.SetParameters(parameters);
                double lh = dist.LogLikelihood(sample);
                if (double.IsNaN(lh) || double.IsInfinity(lh)) return double.MinValue;
                return lh;
            }
            //var solver = new DifferentialEvolution(logLH, NumberOfParameters, Lowers, Uppers);
            var solver = new NelderMead(logLH, NumberOfParameters, Initials, Lowers, Uppers);

            //var constraint = new Constraint((x) => { return Tools.Sum(x.Subset(0, Distributions.Length - 1)); ; }, 
            //    NumberOfParameters, 1, ConstraintType.EqualTo);
            //var innerSolver = new NelderMead(logLH, NumberOfParameters, Initials, Lowers, Uppers);
            //var solver = new AugmentedLagrange(logLH, innerSolver, new IConstraint[] { constraint });
            solver.Maximize();
            return solver.BestParameterSet.Values;
        }


        /// <inheritdoc/>
        public override double PDF(double x)
        {
            // Validate parameters
            if (_parametersValid == false)
                ValidateParameters(GetParameters, true);

            double f = 0.0;
            if (IsZeroInflated)
            {
                if (x <= 0.0)
                {
                    f = ZeroWeight;
                }
                else
                {
                    for (int i = 0; i < Distributions.Count(); i++)
                        f += Weights[i] * Distributions[i].PDF(x);
                }
            }
            else
            {
                for (int i = 0; i < Distributions.Count(); i++)
                    f += Weights[i] * Distributions[i].PDF(x);
            }
            return f < 0d ? 0d : f;
        }

        /// <inheritdoc/>
        public override double LogPDF(double x)
        {
            // Validate parameters
            if (_parametersValid == false)
                ValidateParameters(GetParameters, true);

            var lnf = new List<double>();
            if (IsZeroInflated)
            {
                if (x <= 0.0)
                {
                    lnf.Add(Math.Log(ZeroWeight));
                }
                else
                {
                    for (int i = 0; i < Distributions.Count(); i++)
                        lnf.Add(Math.Log(Weights[i]) + Distributions[i].LogPDF(x));
                }
            }
            else
            {
                for (int i = 0; i < Distributions.Count(); i++)
                    lnf.Add(Math.Log(Weights[i]) + Distributions[i].LogPDF(x));
            }
            var f = Tools.LogSumExp(lnf);
            return f;
        }

        /// <inheritdoc/>
        public override double CDF(double x)
        {
            // Validate parameters
            if (_parametersValid == false)
                ValidateParameters(GetParameters, true);

            double F = 0.0;
            if (IsZeroInflated)
            {
                F = ZeroWeight;
                if (x > 0.0)
                {
                    for (int i = 0; i < Distributions.Count(); i++)
                        F += Weights[i] * Distributions[i].CDF(x);
                }
            }
            else
            {
                for (int i = 0; i < Distributions.Count(); i++)
                    F += Weights[i] * Distributions[i].CDF(x);
            }
            return F < 0d ? 0d : F > 1d ? 1d : F;
        }

        /// <inheritdoc/>
        public override double LogCDF(double x)
        {
            // Validate parameters
            if (_parametersValid == false)
                ValidateParameters(GetParameters, true);

            var lnF = new List<double>();
            if (IsZeroInflated)
            {
                lnF.Add(Math.Log(ZeroWeight));
                if (x > 0.0)
                {
                    for (int i = 0; i < Distributions.Count(); i++)
                        lnF.Add(Math.Log(Weights[i]) + Distributions[i].LogCDF(x));
                }             
            }
            else
            {
                for (int i = 0; i < Distributions.Count(); i++)
                    lnF.Add(Math.Log(Weights[i]) + Distributions[i].LogCDF(x));
            }
            var F = Tools.LogSumExp(lnF);
            return F;
        }

        /// <inheritdoc/>
        public override double InverseCDF(double probability)
        {
            // Validate probability
            if (probability < 0.0d || probability > 1.0d)
                throw new ArgumentOutOfRangeException("probability", "Probability must be between 0 and 1.");
            if (probability == 0.0d) return Minimum;
            if (probability == 1.0d) return Maximum;
            if (IsZeroInflated && probability <= ZeroWeight) return 0;

            // Validate parameters
            if (_parametersValid == false)
                ValidateParameters(GetParameters, true);

            // If there is only one distribution, return its inverse CDF
            if (Distributions.Count() == 1)
            {
                return Distributions[0].InverseCDF(probability);
            }

            // Otherwise use a root finder to solve the inverse CDF
            var xVals = Distributions.Select(d => d.InverseCDF(probability));
            double minX = xVals.Min();
            double maxX = xVals.Max();
            double x = 0;
            try
            {
                if (IsZeroInflated)
                {
                    Brent.Bracket((y) => { return probability - CDF(y); }, ref minX, ref maxX, out var f1, out var f2);
                }
                x = Brent.Solve((y) => { return probability - CDF(y); }, minX, maxX, 1E-6, 100, true);
            }
            catch (Exception)
            {
                // If the root finder fails, create an empirical inverse CDF
                if (_inverseCDFCreated == false)
                    CreateInverseCDF();
                x = _inverseCDF.InverseCDF(probability);
            }
            double min = Minimum;
            double max = Maximum;
            return x < min ? min : x > max ? max : x;
        }
    
        /// <summary>
        /// Create empirical distribution for the inverse CDF.
        /// </summary>
        private void CreateInverseCDF()
        {
            // Get min & max
            double minP = 1E-16;
            double maxP = 1 - 1E-16;
            double minX = Distributions.Min(d => d.InverseCDF(minP));
            double maxX = Distributions.Max(d => d.InverseCDF(maxP));
            // Get number of bins
            double shift = 0;
            if (minX <= 0) shift = Math.Abs(minX) + 1d;
            double min = minX + shift;
            double max = maxX + shift;
            int order = (int)Math.Floor(Math.Log10(max) - Math.Log10(min));
            int binN = Math.Max(200, 100 * order) - 1;
            // Create bins
            var bins = Stratify.XValues(new StratificationOptions(minX, maxX, binN, false), true);
            var xValues = new List<double>();
            var pValues = new List<double>();
            var x = bins.First().LowerBound;
            var p = CDF(bins.First().LowerBound);
            xValues.Add(x);
            pValues.Add(p);
            for (int i = 1; i < bins.Count; i++)
            {
                x = bins[i].LowerBound;
                p = CDF(x);
                if (x > xValues.Last() && p > pValues.Last())
                {
                    xValues.Add(x);
                    pValues.Add(p);
                }
            }
            x = maxX;
            p = CDF(x);
            if (x > xValues.Last() && p > pValues.Last())
            {
                xValues.Add(x);
                pValues.Add(p);
            }
            _inverseCDF = new EmpiricalDistribution(xValues, pValues) { XTransform = XTransform, ProbabilityTransform = ProbabilityTransform };
            _inverseCDFCreated = true;
        }

        /// <inheritdoc/>
        public override double[] GenerateRandomValues(int sampleSize, int seed = -1)
        {
            // Create PRNG for generating random numbers
            var rnd = seed > 0 ? new MersenneTwister(seed) : new MersenneTwister();
            var weights = new List<double>();
            var distributions = new List<UnivariateDistributionBase>();
            if (IsZeroInflated)
            {
                weights.Add(ZeroWeight);
                distributions.Add(new Deterministic(0.0));
            }
            weights.AddRange(Weights);
            distributions.AddRange(Distributions);

            var sample = new double[sampleSize];
            // Generate values
            for (int i = 0; i < sampleSize; i++)
            {
                var u = rnd.NextDouble();
                var cdfW = new double[distributions.Count()];
                for (int j = 0; j < distributions.Count(); j++)
                {
                    cdfW[j] = j == 0 ? weights[j] : cdfW[j - 1] + weights[j];
                    if (u <= cdfW[j])
                    {
                        sample[i] = distributions[j].InverseCDF(rnd.NextDouble());
                        break;
                    }
                }
            }
            // Return array of random values
            return sample;
        }

        /// <inheritdoc/>
        public override UnivariateDistributionBase Clone()
        {
            var dists = new UnivariateDistributionBase[Distributions.Count()];
            for (int i = 0; i < Distributions.Count(); i++)
                dists[i] = Distributions[i].Clone();

            return new Mixture(Weights.ToArray(), dists)
            {
                IsZeroInflated = IsZeroInflated,
                ZeroWeight = ZeroWeight,
                XTransform = XTransform,
                ProbabilityTransform = ProbabilityTransform
            };
        }

        /// <inheritdoc/>
        public override XElement ToXElement()
        {
            var result = new XElement("Distribution");
            result.SetAttributeValue(nameof(Type), Type.ToString());
            result.SetAttributeValue(nameof(IsZeroInflated), IsZeroInflated.ToString());
            result.SetAttributeValue(nameof(ZeroWeight), ZeroWeight.ToString());
            result.SetAttributeValue(nameof(XTransform), XTransform.ToString());
            result.SetAttributeValue(nameof(ProbabilityTransform), ProbabilityTransform.ToString());
            result.SetAttributeValue(nameof(Weights), String.Join("|", Weights));
            result.SetAttributeValue(nameof(Distributions), String.Join("|", Distributions.Select(x => x.Type)));
            result.SetAttributeValue("Parameters", String.Join("|", GetParameters));
            return result;
        }

        /// <summary>
        /// Create a mixture distribution from XElement.
        /// </summary>
        /// <param name="xElement">The XElement to deserialize.</param>
        /// <returns>A new mixture distribution.</returns>
        public static Mixture FromXElement(XElement xElement)
        {
            UnivariateDistributionType type = UnivariateDistributionType.Deterministic;
            if (xElement.Attribute(nameof(UnivariateDistributionBase.Type)) != null)
            {
                Enum.TryParse(xElement.Attribute(nameof(UnivariateDistributionBase.Type)).Value, out type);

            }
            if (type == UnivariateDistributionType.Mixture)
            {
                var weights = new List<double>();
                var distributions = new List<UnivariateDistributionBase>();
                if (xElement.Attribute(nameof(Weights)) != null)
                {
                    var w = xElement.Attribute(nameof(Weights)).Value.Split('|');
                    for (int i = 0; i < w.Length; i++)
                    {
                        double.TryParse(w[i], out var weight);
                        weights.Add(weight);
                    }
                }
                if (xElement.Attribute(nameof(Distributions)) != null)
                {
                    var types = xElement.Attribute(nameof(Distributions)).Value.Split('|');
                    for (int i = 0; i < types.Length; i++)
                    {
                        Enum.TryParse(types[i], out UnivariateDistributionType distType);
                        distributions.Add(UnivariateDistributionFactory.CreateDistribution(distType));
                    }
                }
                var mixture = new Mixture(weights.ToArray(), distributions.ToArray());

                if (xElement.Attribute(nameof(IsZeroInflated)) != null)
                {
                    bool.TryParse(xElement.Attribute(nameof(IsZeroInflated)).Value, out var isZeroInflated);
                    mixture.IsZeroInflated = isZeroInflated;
                }
                if (xElement.Attribute(nameof(ZeroWeight)) != null)
                {
                    double.TryParse(xElement.Attribute(nameof(ZeroWeight)).Value, out var zeroWeight);
                    mixture.ZeroWeight = zeroWeight;
                }
                if (xElement.Attribute(nameof(XTransform)) != null)
                {
                    Enum.TryParse(xElement.Attribute(nameof(XTransform)).Value, out Transform xTransform);
                    mixture.XTransform = xTransform;
                }
                if (xElement.Attribute(nameof(ProbabilityTransform)) != null)
                {
                    Enum.TryParse(xElement.Attribute(nameof(ProbabilityTransform)).Value, out Transform probabilityTransform);
                    mixture.ProbabilityTransform = probabilityTransform;
                }
                if (xElement.Attribute("Parameters") != null)
                {
                    var vals = xElement.Attribute("Parameters").Value.Split('|');
                    var parameters = new List<double>();
                    for (int i = 0; i < vals.Length; i++)
                    {
                        double.TryParse(vals[i], out var parm);
                        parameters.Add(parm);
                    }
                    mixture.SetParameters(parameters);
                }

                return mixture;
            }
            else
            {
                return null;
            }
        }

    }
}
