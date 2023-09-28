
///// <summary>
///// This is the first implementation of a new test algorithm for Jenks Natural Breaks. This solution is different from the others I have found as this provides better solution to minimizing the
///// sum of bin standard deviations whereas the other implementations (commented above) provide very rough approximate solutions to the optimization problem.
///// The current implementation is slow but could be optimized significantly.
///// </summary>
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.VisualBasic;

//namespace Numerics
//{
//    public class NewTest
//    {
//        private double[] _values = null;
//        private readonly int _numClasses;
//        private readonly int _numValues;
//        private LinkedList<Bin> _bins;

//        public NewTest(IList<double> values, int numClasses, double epsilon = 0.0000000001d)
//        {
//            _numClasses = numClasses;
//            _numValues = values.Count;
//            _values = values.ToArray();
//            // 
//            if (_numClasses > _values.Length)
//                _numClasses = _values.Length;
//            if (_numClasses < 2)
//                _numClasses = 2;
//            Array.Sort(_values);
//            // initialize bins
//            // each bin must be initialized with a single unique index in the order of the values indeces.
//            // opportunity here to parameterize optimality by merging bins up front if their addition doesn't increase variance beyond some epsilon.
//            // Dim binArray As New List(Of Bin)(_values.Count)
//            var binArray = new Bin[(_values.Count())];
//            // Dim binToAdd As Bin
//            // binArray.Add(New Bin(0, _values))
//            binArray[0] = new Bin(0, _values);
//            for (int i = 1, loopTo = _values.Count() - 1; i <= loopTo; i++)
//            {
//                // binToAdd = New Bin(i, _values)
//                binArray[i] = new Bin(i, _values);
//                binArray[i - 1].CalculateMergedVariance(binArray[i]); // Calc merged variance up front
//                                                                      // binArray(binArray.Count - 1).CalculateMergedVariance(binToAdd) 'Calc merged variance up front
//                                                                      // If binArray(binArray.Count - 1).MergedVariance < epsilon Then
//                                                                      // binToAdd.MergeBins(binArray(binArray.Count - 1))
//                                                                      // binArray(binArray.Count - 1) = binToAdd
//                                                                      // binArray(binArray.Count - 2).CalculateMergedVariance(binToAdd)
//                                                                      // Else
//                                                                      // binArray.Add(binToAdd)
//                                                                      // End If
//            }

//            _bins = new LinkedList<Bin>(binArray);
//            // 
//            double minVariance = double.MaxValue;
//            var minNode = default(LinkedListNode<Bin>);
//            LinkedListNode<Bin> binNode;
//            double testVariance;
//            while (_bins.Count > numClasses)
//            {
//                // find the minimum variance to merge bins
//                minVariance = double.MaxValue;
//                binNode = _bins.First;
//                while (!Information.IsNothing(binNode))
//                {
//                    if (Information.IsNothing(binNode.Next))
//                        break;
//                    testVariance = binNode.Value.MergedVariance;
//                    if (minVariance > testVariance)
//                    {
//                        minVariance = testVariance;
//                        minNode = binNode;
//                    }

//                    binNode = binNode.Next;
//                }

//                // merge the bins, update the merged variances for testing, and remove the node that has been merged.
//                minNode.Next.Value.MergeBins(minNode.Value);
//                if (!Information.IsNothing(minNode.Previous))
//                    minNode.Previous.Value.CalculateMergedVariance(minNode.Next.Value);
//                if (!Information.IsNothing(minNode.Next.Next))
//                    minNode.Next.Value.CalculateMergedVariance(minNode.Next.Next.Value);
//                _bins.Remove(minNode);

//                // continue until the number of bins is equal to the number of classes.
//            }

//            // Dim counter As Int32 = 0
//            // For Each bin In _bins
//            // For Each index In bin.GetArrayValues
//            // Debug.Print(counter.ToString)
//            // Next
//            // counter += 1
//            // 'Debug.Print(String.Join(vbTab, bin.GetArrayValues))
//            // Next
//        }

//        public Tuple<double, double>[] GetRanges()
//        {
//            var results = new Tuple<double, double>[_bins.Count];
//            int binIndex = 0;
//            foreach (var bin in _bins)
//            {
//                results[binIndex] = bin.GetRange();
//                binIndex += 1;
//            }

//            return results;
//        }

//        public class Bin
//        {
//            private readonly double[] _sourceArray;
//            private List<int> _arrayIndices = new List<int>();

//            public double Mean { get; private set; }
//            public double Variance { get; private set; }
//            public double MergedVariance { get; private set; }
//            public double MergedMean { get; private set; }

//            public Bin(int InitialIndex, double[] sourceArray)
//            {
//                _arrayIndices.Add(InitialIndex);
//                Mean = sourceArray[InitialIndex];
//                Variance = 0d;
//                _sourceArray = sourceArray;
//            }

//            public double[] GetArrayValues()
//            {
//                var arrayValues = new double[_arrayIndices.Count];
//                for (int i = 0, loopTo = _arrayIndices.Count - 1; i <= loopTo; i++)
//                    arrayValues[i] = _sourceArray[_arrayIndices[i]];
//                return arrayValues;
//            }

//            public int[] GetIndexes()
//            {
//                return _arrayIndices.ToArray();
//            }

//            public Tuple<double, double> GetRange()
//            {
//                if (_arrayIndices.Count == 0)
//                    return null;
//                return new Tuple<double, double>(_sourceArray[_arrayIndices[0]], _sourceArray[_arrayIndices.Last()]);
//            }
//            // ''' <summary>
//            // ''' This sub is probably a good target for code optimization.
//            // ''' </summary>
//            // Private Sub CalculateVariance()
//            // _Variance = Statistics.Variance(GetArrayValues())
//            // End Sub
//            public void MergeBins(Bin binToMerge)
//            {
//                Variance = MergedVariance;
//                Mean = MergedMean;
//                // 'Calculate variance
//                // 'http:'en.wikipedia.org/wiki/Algorithms_for_calculating_variance
//                // 'http:'planetmath.org/onepassalgorithmtocomputesamplevariance
//                // Dim nObservations As Int32 = _arrayIndices.Count
//                // For Each observation As Double In binToMerge.GetArrayValues
//                // nObservations += 1
//                // _Variance = (((nObservations - 2) / (nObservations - 1)) * Variance) + ((observation - _Mean) ^ 2) / nObservations
//                // _Mean = _Mean + ((observation - _Mean) / nObservations)
//                // Next

//                // Update array indices
//                binToMerge._arrayIndices.AddRange(_arrayIndices);
//                _arrayIndices = binToMerge._arrayIndices;
//            }

//            public void CalculateMergedVariance(Bin mergeBin)
//            {

//                // Calculate variance
//                // http:'en.wikipedia.org/wiki/Algorithms_for_calculating_variance
//                // http:'planetmath.org/onepassalgorithmtocomputesamplevariance
//                int nObservations = _arrayIndices.Count;
//                MergedVariance = Variance;
//                MergedMean = Mean;
//                foreach (double observation in mergeBin.GetArrayValues())
//                {
//                    nObservations += 1;
//                    MergedVariance = (nObservations - 2) / (double)(nObservations - 1) * MergedVariance + Math.Pow(observation - MergedMean, 2d) / nObservations;
//                    MergedMean = MergedMean + (observation - MergedMean) / nObservations;
//                }

//                // Dim array1 = GetArrayValues()
//                // Dim array2 = mergeBin.GetArrayValues()
//                // Return Statistics.Variance(array1.Concat(array2).ToArray())
//            }
//        }
//    }
//}