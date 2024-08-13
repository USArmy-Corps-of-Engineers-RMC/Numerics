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
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Numerics.Data;
using Numerics.Data.Statistics;

namespace Numerics.Distributions
{

    /// <summary>
    /// The kernel density distribution function.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    /// References:
    /// <list type="bullet">
    /// <item><description>
    /// <see href = "https://en.wikipedia.org/wiki/Kernel_density_estimation" />
    /// </description></item>
    /// </list>
    /// </para>
    /// </remarks>
    [Serializable]
    public class KernelDensity : UnivariateDistributionBase, IBootstrappable
    {
  
        /// <summary>
        /// Constructs a Gaussian Kernel Density distribution from 30 random samples of a standard Normal distribution using the default bandwidth.
        /// </summary>
        public KernelDensity()
        {
            var sample = new Normal().GenerateRandomValues(12345, 30);
            SetSampleData(sample);
            KernelDistribution = KernelType.Gaussian;
            Bandwidth = BandwidthRule(sample);
        }

        /// <summary>
        /// Constructs a Gaussian Kernel Density distribution from a sample of data using the default bandwidth.
        /// </summary>
        /// <param name="sampleData">Sample of data, no sorting is assumed.</param>
        public KernelDensity(IList<double> sampleData)
        {
            SetSampleData(sampleData);
            KernelDistribution = KernelType.Gaussian;
            Bandwidth = BandwidthRule(sampleData);
        }

        /// <summary>
        /// Constructs a Kernel Density distribution from a sample of data with a specified Kernel type using the default bandwidth.
        /// </summary>
        /// <param name="sampleData">Sample of data, no sorting is assumed.</param>
        /// <param name="kernel">The kernel distribution type.</param>
        public KernelDensity(IList<double> sampleData, KernelType kernel)
        {
            SetSampleData(sampleData);
            KernelDistribution = kernel;
            Bandwidth = BandwidthRule(sampleData);
        }

        /// <summary>
        /// Constructs a Kernel Density distribution from a sample of data with a specified Kernel type and bandwidth.
        /// </summary>
        /// <param name="sampleData">Sample of data, no sorting is assumed.</param>
        /// <param name="kernel">The kernel distribution type.</param>
        /// <param name="bandwidthParameter">The bandwidth parameter.</param>
        public KernelDensity(IList<double> sampleData, KernelType kernel, double bandwidthParameter)
        {
            SetSampleData(sampleData);
            KernelDistribution = kernel;
            Bandwidth = bandwidthParameter;
        }
 
        /// <summary>
        /// Kernel distribution type.
        /// </summary>
        public enum KernelType
        {
            /// <summary>
            /// Epanechnikov kernel.
            /// </summary>
            Epanechnikov,
            /// <summary>
            /// Gaussian kernel.
            /// </summary>
            Gaussian,
            /// <summary>
            /// Triangular kernel.
            /// </summary>
            Triangular,
            /// <summary>
            /// Uniform kernel.
            /// </summary>
            Uniform
        }

        private double[] _sampleData;
        private double[] _pValues;
        private double _bandwidth;
        private KernelType _kernelDistribution;
        private IKernel _kernel;
        private bool _dataIsSorted;
        private bool _parametersValid = true;
        private OrderedPairedData opd;
        private double u1, u2, u3, u4;

        /// <summary>
        /// Returns the array of X values. Points On the cumulative curve are specified
        /// with increasing value and increasing probability.
        /// </summary>
        public ReadOnlyCollection<double> SampleData => new ReadOnlyCollection<double>(_sampleData);

        /// <summary>
        /// Returns the array of probability plotting position values.
        /// </summary>
        public ReadOnlyCollection<double> ProbabilityValues => new ReadOnlyCollection<double>(_pValues);

        /// <summary>
        /// Gets and sets the kernel distribution type.
        /// </summary>
        public KernelType KernelDistribution
        {
            get { return _kernelDistribution; }
            set
            {
                _kernelDistribution = value;
                if (_kernelDistribution == KernelType.Epanechnikov)
                {
                    _kernel = new EpanechnikovKernel();
                }
                else if (_kernelDistribution == KernelType.Gaussian)
                {
                    _kernel = new GuassianKernel();
                }
                else if (_kernelDistribution == KernelType.Triangular)
                {
                    _kernel = new TriangularKernel();
                }
                else if (_kernelDistribution == KernelType.Uniform)
                {
                    _kernel = new UniformKernel();
                }
            }
        }

        /// <summary>
        /// Gets and sets the bandwidth parameter used in the kernel density estimation.
        /// </summary>
        public double Bandwidth
        {
            get { return _bandwidth; }
            set
            {
                _parametersValid = ValidateParameters(value, false) is null;
                _bandwidth = value;
            }
        }

        /// <summary>
        /// Gets the sample size of the distribution.
        /// </summary>
        public int SampleSize
        {
            get { return _sampleData.Count(); }
        }

        /// <summary>
        /// Determines the interpolation transform for the sample data values.
        /// </summary>
        public Transform DataTransform { get; set; } = Transform.None;

        /// <summary>
        /// Determines the interpolation transform for the Probability-values.
        /// </summary>
        public Transform ProbabilityTransform { get; set; } = Transform.None;

        /// <inheritdoc/>
        public override int NumberOfParameters
        {
            get { return 3; }
        }

        /// <inheritdoc/>
        public override UnivariateDistributionType Type
        {
            get { return UnivariateDistributionType.KernelDensity; }
        }

        /// <inheritdoc/>
        public override string DisplayName
        {
            get { return "Kernel Density"; }
        }

        /// <inheritdoc/>
        public override string ShortDisplayName
        {
            get { return "KDE"; }
        }

        /// <inheritdoc/>
        public override string[,] ParametersToString
        {
            get
            {
                var parmString = new string[3, 2];
                string Xstring = "{";
                for (int i = 0; i < _sampleData.Count(); i++)
                {
                    Xstring += _sampleData[i].ToString();
                    if (i < _sampleData.Count() - 1)
                    {
                        Xstring += ",";
                    }
                }

                Xstring += "}";
                parmString[0, 0] = "Sample Data";
                parmString[1, 0] = "Kernel Type";
                parmString[2, 0] = "Bandwidth";
                parmString[0, 1] = Xstring;
                parmString[1, 1] = KernelDistribution.ToString();
                parmString[2, 1] = Bandwidth.ToString();
                return parmString;
            }
        }

        /// <inheritdoc/>
        public override string[] ParameterNamesShortForm
        {
            get { return ["Data()", "Kernel", "BW"]; }
        }

        /// <inheritdoc/>
        public override string[] GetParameterPropertyNames
        {
            get { return [nameof(SampleData), nameof(KernelDistribution), nameof(Bandwidth)]; }
        }

        /// <inheritdoc/>
        public override double[] GetParameters
        {
            get { throw new NotImplementedException(); }
        }

        /// <inheritdoc/>
        public override bool ParametersValid
        {
            get { return _parametersValid; }
        }

        /// <summary>
        /// Set Product (Central) Moments
        /// </summary>
        /// <param name="sampleData">Sample of data, no sorting is assumed.</param>
        private void ComputeMoments(IList<double> sampleData)
        {
            var moments = Statistics.ProductMoments(sampleData);
            u1 = moments[0];
            u2 = moments[1];
            u3 = moments[2];
            u4 = moments[3];
        }

        /// <inheritdoc/>
        public override double Mean
        {
            get { return u1; }
        }

        /// <inheritdoc/>
        public override double Median
        {
            get { return InverseCDF(0.5d); }
        }

        /// <inheritdoc/>
        public override double Mode
        {
            get { return double.NaN; }
        }

        /// <inheritdoc/>
        public override double StandardDeviation
        {
            get { return u2; }
        }

        /// <inheritdoc/>
        public override double Skewness
        {
            get { return u3; }
        }

        /// <inheritdoc/>
        public override double Kurtosis
        {
            get { return u4; }
        }

        /// <inheritdoc/>
        public override double Minimum
        {
            get
            {
                if (_sampleData is null) return double.NaN;
                if (_sampleData.Count() == 0) return double.NaN;
                SortSampleData();
                return _sampleData.First();
            }
        }

        /// <inheritdoc/>
        public override double Maximum
        {
            get
            {
                if (_sampleData is null) return double.NaN;
                if (_sampleData.Count() == 0) return double.NaN;
                SortSampleData();
                return _sampleData.Last();
            }
        }

        /// <inheritdoc/>
        public override double[] MinimumOfParameters
        {
            get { return [double.MinValue, 0d]; }
        }

        /// <inheritdoc/>
        public override double[] MaximumOfParameters
        {
            get { return [double.MaxValue, double.MaxValue]; }
        }

        #region Kernel Distributions

        /// <summary>
        /// Simple interface for kernel functions.
        /// </summary>
        private interface IKernel
        {
            double Function(double x);
        }

        /// <summary>
        /// Epanechnikov kernel with a min of -1 and max of 1.
        /// </summary>
        private class EpanechnikovKernel : IKernel
        {
            public double Function(double x)
            {
                if (Math.Abs(x) <= 1.0d)
                {
                    return 0.75d * (1d - x * x);
                }
                else
                {
                    return 0.0d;
                }
            }
        }

        /// <summary>
        /// Gaussian kernel with a mean of 0 and standard deviation of 1.
        /// This is the default kernel.
        /// </summary>
        private class GuassianKernel : IKernel
        {
            public double Function(double x)
            {
                return Normal.StandardPDF(x);
            }
        }

        /// <summary>
        /// Triangular kernel with a min of -1, mode of 0, and max of 1.
        /// </summary>
        private class TriangularKernel : IKernel
        {
            private Triangular _triangularDist = new Triangular(-1.0d, 0.0d, 1.0d);
            public double Function(double x)
            {
                return _triangularDist.PDF(x);
            }
        }

        /// <summary>
        /// Uniform kernel with a min of -1 and max of 1.
        /// </summary>
        private class UniformKernel : IKernel
        {
            private Uniform _uniformDist = new Uniform(-1.0d, 1.0d);
            public double Function(double x)
            {
                return _uniformDist.PDF(x);
            }
        }

        #endregion

        /// <summary>
        /// Gets the default estimate of the bandwidth parameter.
        /// </summary>
        /// <param name="sampleData">Sample of data, no sorting is assumed.</param>
        /// <returns>An estimate of the bandwidth parameter.</returns>
        /// <remarks>
        /// This method is based on the practical estimation of the bandwidth as
        /// described here: http://en.wikipedia.org/wiki/Kernel_density_estimation
        /// </remarks>
        public double BandwidthRule(IList<double> sampleData)
        {
            double sigma = Statistics.StandardDeviation(sampleData);
            return sigma * Math.Pow(4.0d / (3.0d * sampleData.Count), 1.0d / 5.0d);
        }

        /// <inheritdoc/>
        public IUnivariateDistribution Bootstrap(ParameterEstimationMethod estimationMethod, int sampleSize, int seed = 12345)
        {
            var sample = GenerateRandomValues(seed, sampleSize);
            return new KernelDensity(sample, KernelDistribution);
        }

        /// <inheritdoc/>
        public override void SetParameters(IList<double> parameters)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override ArgumentOutOfRangeException ValidateParameters(IList<double> parameters, bool throwException)
        {
            return null;
        }

        /// <summary>
        /// Validate the bandwidth parameter.
        /// </summary>
        private ArgumentOutOfRangeException ValidateParameters(double value, bool throwException)
        {
            if (value <= 0d)
            {
                if (throwException)
                    throw new ArgumentOutOfRangeException(nameof(Bandwidth), "The bandwidth must be a positive number!");
                return new ArgumentOutOfRangeException(nameof(Bandwidth), "The bandwidth must be a positive number!");
            }
            return null;
        }

        /// <summary>
        /// Sort the sample data in ascending order.
        /// </summary>
        public void SortSampleData()
        {
            if (!_dataIsSorted)
            {
                Array.Sort(_sampleData);
                opd = new OrderedPairedData(_sampleData, _pValues, true, SortOrder.Ascending, true, SortOrder.Ascending);
                _dataIsSorted = true;
            }
        }

        /// <summary>
        /// Set the sample data for the distribution.
        /// </summary>
        /// <param name="sampleData">Sample of data, no sorting is assumed.</param>
        /// <param name="plottingPostionType">The plotting position formula type. Default = Weibull.</param>
        public void SetSampleData(IList<double> sampleData, PlottingPositions.PlottingPostionType plottingPostionType = PlottingPositions.PlottingPostionType.Weibull)
        {
            _sampleData = sampleData.ToArray();
            _pValues = PlottingPositions.Function(SampleSize, plottingPostionType);
            ComputeMoments(_sampleData);      
            _dataIsSorted = false;
        }

        /// <summary>
        /// Append new data to sample data.
        /// </summary>
        /// <param name="newData">Sample of data, no sorting is assumed.</param>
        /// <param name="plottingPostionType">The plotting position formula type. Default = Weibull.</param>
        public void AppendSampleData(IList<double> newData, PlottingPositions.PlottingPostionType plottingPostionType = PlottingPositions.PlottingPostionType.Weibull)
        {
            int StartIndex = newData.Count;
            int Count = _sampleData.Count();
            Array.Resize(ref _sampleData, Count + StartIndex);
            Array.Copy(newData.ToArray(), 0, _sampleData, StartIndex, StartIndex);
            _pValues = PlottingPositions.Function(SampleSize, plottingPostionType);
            ComputeMoments(_sampleData);
            _dataIsSorted = false;
        }

        /// <summary>
        /// Clear sample data.
        /// </summary>
        public void ClearSampleData()
        {
            Array.Clear(_sampleData, 0, _sampleData.Count());
        }

        /// <inheritdoc/>
        public override double PDF(double x)
        {
            double total = 0d;
            Parallel.For(0, SampleSize, () => 0d, (i, loop, subtotal) =>
            {
                subtotal += _kernel.Function((x - _sampleData[i]) / Bandwidth);
                return subtotal;
            }, z => Tools.ParallelAdd(ref total, z));
            return total / (SampleSize * Bandwidth);
        }

        /// <inheritdoc/>
        public override double CDF(double x)
        {
            SortSampleData();
            if (x <= Minimum) return 0d;
            if (x >= Maximum) return 1d;
            return opd.Interpolate(x, true, DataTransform, ProbabilityTransform);
        }

        /// <inheritdoc/>
        public override double InverseCDF(double probability)
        {
            SortSampleData();
            // Validate probability
            if (probability < 0.0d || probability > 1.0d)
                throw new ArgumentOutOfRangeException("probability", "Probability must be between 0 and 1.");
            if (probability == 0.0d) return Minimum;
            if (probability == 1.0d) return Maximum;
            return opd.Interpolate(probability, false, DataTransform, ProbabilityTransform);
        }

        /// <inheritdoc/>
        public override UnivariateDistributionBase Clone()
        {
            return new KernelDensity(SampleData, KernelDistribution, Bandwidth);
        }
 
    }
}