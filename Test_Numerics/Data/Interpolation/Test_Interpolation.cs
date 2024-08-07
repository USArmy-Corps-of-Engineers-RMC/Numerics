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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Data;

namespace Data.Interpolation
{
    /// <summary>
    /// Unit tests for the base Interpolation class
    /// </summary>
    /// <remarks>
    ///      <b> Authors: </b>
    /// <list type="bullet">
    /// <item><description>
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </description></item>
    /// <item><description>
    ///     Sadie Niblett, USACE Risk Management Center, sadie.s.niblett@usace.army.mil
    /// </description></item>
    /// </list>
    /// <b> References: </b>
    /// 
    /// </remarks>
    [TestClass]
    public class Test_Interpolation
    {
        [TestMethod]
        public void Test_LinearSearch()
        {
            var values = new double[1000];
            for (int i = 0; i <= 999; i++)
                values[i] = i + 1;
            long lo = Numerics.Data.Interpolation.LinearSearch(872.5d, values);         // should be sequential search here?
            Assert.AreEqual((int)lo, 871);
        }

        /// <summary>
        /// Testing the bisection search method with one value
        /// </summary>
        [TestMethod]
        public void Test_BisectionSearch()
        {
            var values = new double[1000];
            for (int i = 0; i <= 999; i++)
                values[i] = i + 1;
            long lo = Numerics.Data.Interpolation.BisectionSearch(872.5d, values);
            Assert.AreEqual((int)lo, 871);
        }

        /// <summary>
        /// Testing the binary search method with one value
        /// </summary>
        [TestMethod]
        public void Test_BinarySearch()
        {
            var values = new double[1000];
            for (int i = 0; i <= 999; i++)
                values[i] = i + 1;
            int Bisearch = Array.BinarySearch(values, 872.5d);      // why are we testing this in this class?? it is tied to array
            if (Bisearch < 0)                                       // isn't binary search like bisection?? so could use it to compare/validate bisection instead
            {
                Bisearch = ~Bisearch;
            }

            long lo = Bisearch - 1;
            Assert.AreEqual((int)lo, 871);
        }

        /// <summary>
        /// Test the Hunt search method with one value
        /// </summary>
        [TestMethod]
        public void Test_HuntSearch()
        {
            var values = new double[1000];
            for (int i = 0; i <= 999; i++)
                values[i] = i + 1;
            long lo = Numerics.Data.Interpolation.HuntSearch(872.5d, values);
            Assert.AreEqual((int)lo, 871);
        }

        // everything below is in Test_Linear.cs or Test_Bilinear.cs

        [TestMethod()]
        public void Test_Linear()
        {
            var XArray = new double[] { 50d, 100d, 150d, 200d, 250d };
            var YArray = new double[] { 100d, 200d, 300d, 400d, 500d };
            double X = 75d;
            double Y = Numerics.Data.Interpolation.Linear(X, XArray, YArray);
            Assert.AreEqual(Y, 150.0d, 1E-6);
        }

        [TestMethod()]
        public void Test_LinLog()
        {
            var XArray = new double[] { 50d, 100d, 150d, 200d, 250d };
            var YArray = new double[] { 100d, 200d, 300d, 400d, 500d };
            double X = 75d;
            double Y = Numerics.Data.Interpolation.Linear(X, XArray, YArray, YTransform: Transform.Logarithmic);
            Assert.AreEqual(Y, 141.42135623731d, 1E-6);
        }

        [TestMethod()]
        public void Test_LogLin()
        {
            var XArray = new double[] { 50d, 100d, 150d, 200d, 250d };
            var YArray = new double[] { 100d, 200d, 300d, 400d, 500d };
            double X = 75d;
            double Y = Numerics.Data.Interpolation.Linear(X, XArray, YArray, XTransform: Transform.Logarithmic);
            Assert.AreEqual(Y, 158.496250072116d, 1E-6);
        }

        [TestMethod()]
        public void Test_LogLog()
        {
            var XArray = new double[] { 50d, 100d, 150d, 200d, 250d };
            var YArray = new double[] { 100d, 200d, 300d, 400d, 500d };
            double X = 75d;
            double Y = Numerics.Data.Interpolation.Linear(X, XArray, YArray, XTransform: Transform.Logarithmic, YTransform: Transform.Logarithmic);
            Assert.AreEqual(Y, 150.0d, 1E-6);
        }

        [TestMethod()]
        public void Test_LinZ()
        {
            var XArray = new double[] { 0.05d, 0.1d, 0.15d, 0.2d, 0.25d };
            var YArray = new double[] { 0.1d, 0.2d, 0.3d, 0.4d, 0.5d };
            double X = 0.18d;
            double Y = Numerics.Data.Interpolation.Linear(X, XArray, YArray, YTransform: Transform.NormalZ);
            Assert.AreEqual(Y, 0.358762529d, 1E-6);
        }

        [TestMethod()]
        public void Test_ZLin()
        {
            var XArray = new double[] { 0.05d, 0.1d, 0.15d, 0.2d, 0.25d };
            var YArray = new double[] { 0.1d, 0.2d, 0.3d, 0.4d, 0.5d };
            double X = 0.18d;
            double Y = Numerics.Data.Interpolation.Linear(X, XArray, YArray, XTransform: Transform.NormalZ);
            Assert.AreEqual(Y, 0.362146174d, 1E-6);
        }

        [TestMethod()]
        public void Test_ZZ()
        {
            var XArray = new double[] { 0.05d, 0.1d, 0.15d, 0.2d, 0.25d };
            var YArray = new double[] { 0.1d, 0.2d, 0.3d, 0.4d, 0.5d };
            double X = 0.18d;
            double Y = Numerics.Data.Interpolation.Linear(X, XArray, YArray, XTransform: Transform.NormalZ, YTransform: Transform.NormalZ);
            Assert.AreEqual(Y, 0.36093855992815d, 1E-6);
        }

        [TestMethod()]
        public void Test_RevLinear()
        {
            var XArray = new double[] { 50d, 100d, 150d, 200d, 250d };
            var YArray = new double[] { 100d, 200d, 300d, 400d, 500d };
            Array.Reverse(XArray);
            Array.Reverse(YArray);
            double X = 75d;
            double Y = Numerics.Data.Interpolation.Linear(X, XArray, YArray, Order: SortOrder.Descending);
            Assert.AreEqual(Y, 150.0d, 1E-6);
        }

        [TestMethod()]
        public void Test_RevLinLog()
        {
            var XArray = new double[] { 50d, 100d, 150d, 200d, 250d };
            var YArray = new double[] { 100d, 200d, 300d, 400d, 500d };
            Array.Reverse(XArray);
            Array.Reverse(YArray);
            double X = 75d;
            double Y = Numerics.Data.Interpolation.Linear(X, XArray, YArray, YTransform: Transform.Logarithmic, Order: SortOrder.Descending);
            Assert.AreEqual(Y, 141.42135623731d, 1E-6);
        }

        [TestMethod()]
        public void Test_RevLogLin()
        {
            var XArray = new double[] { 50d, 100d, 150d, 200d, 250d };
            var YArray = new double[] { 100d, 200d, 300d, 400d, 500d };
            Array.Reverse(XArray);
            Array.Reverse(YArray);
            double X = 75d;
            double Y = Numerics.Data.Interpolation.Linear(X, XArray, YArray, XTransform: Transform.Logarithmic, Order: SortOrder.Descending);
            Assert.AreEqual(Y, 158.496250072116d, 1E-6);
        }

        [TestMethod()]
        public void Test_RevLogLog()
        {
            var XArray = new double[] { 50d, 100d, 150d, 200d, 250d };
            var YArray = new double[] { 100d, 200d, 300d, 400d, 500d };
            Array.Reverse(XArray);
            Array.Reverse(YArray);
            double X = 75d;
            double Y = Numerics.Data.Interpolation.Linear(X, XArray, YArray, XTransform: Transform.Logarithmic, YTransform: Transform.Logarithmic, Order: SortOrder.Descending);
            Assert.AreEqual(Y, 150.0d, 1E-6);
        }

        [TestMethod()]
        public void Test_RevLinZ()
        {
            var XArray = new double[] { 0.05d, 0.1d, 0.15d, 0.2d, 0.25d };
            var YArray = new double[] { 0.1d, 0.2d, 0.3d, 0.4d, 0.5d };
            Array.Reverse(XArray);
            Array.Reverse(YArray);
            double X = 0.18d;
            double Y = Numerics.Data.Interpolation.Linear(X, XArray, YArray, YTransform: Transform.NormalZ, Order: SortOrder.Descending);
            Assert.AreEqual(Y, 0.358762529d, 1E-6);
        }

        [TestMethod()]
        public void Test_RevZLin()
        {
            var XArray = new double[] { 0.05d, 0.1d, 0.15d, 0.2d, 0.25d };
            var YArray = new double[] { 0.1d, 0.2d, 0.3d, 0.4d, 0.5d };
            Array.Reverse(XArray);
            Array.Reverse(YArray);
            double X = 0.18d;
            double Y = Numerics.Data.Interpolation.Linear(X, XArray, YArray, XTransform: Transform.NormalZ, Order: SortOrder.Descending);
            Assert.AreEqual(Y, 0.362146174d, 1E-6);
        }

        [TestMethod()]
        public void Test_RevZZ()
        {
            var XArray = new double[] { 0.05d, 0.1d, 0.15d, 0.2d, 0.25d };
            var YArray = new double[] { 0.1d, 0.2d, 0.3d, 0.4d, 0.5d };
            Array.Reverse(XArray);
            Array.Reverse(YArray);
            double X = 0.18d;
            double Y = Numerics.Data.Interpolation.Linear(X, XArray, YArray, XTransform: Transform.NormalZ, YTransform: Transform.NormalZ, Order: SortOrder.Descending);
            Assert.AreEqual(Y, 0.36093855992815d, 1E-6);
        }

        [TestMethod()]
        public void Test_BiLinear()
        {
            var XArray = new double[] { 50d, 100d, 150d, 200d, 250d };
            var YArray = new double[] { 100d, 200d, 300d, 400d, 500d };
            var ZArray = new double[5, 5];
            ZArray[0, 0] = 850.36d;
            ZArray[1, 0] = 865.39d;
            ZArray[2, 0] = 867.27d;
            ZArray[3, 0] = 869.71d;
            ZArray[4, 0] = 871.84d;
            ZArray[0, 1] = 868.45d;
            ZArray[1, 1] = 878.66d;
            ZArray[2, 1] = 880.47d;
            ZArray[3, 1] = 881.91d;
            ZArray[4, 1] = 883.19d;
            ZArray[0, 2] = 896.71d;
            ZArray[1, 2] = 901.2d;
            ZArray[2, 2] = 901.92d;
            ZArray[3, 2] = 902.4d;
            ZArray[4, 2] = 903.22d;
            ZArray[0, 3] = 914.78d;
            ZArray[1, 3] = 918.54d;
            ZArray[2, 3] = 919.14d;
            ZArray[3, 3] = 919.9d;
            ZArray[4, 3] = 920.18d;
            ZArray[0, 4] = 928.87d;
            ZArray[1, 4] = 929.46d;
            ZArray[2, 4] = 929.54d;
            ZArray[3, 4] = 929.62d;
            ZArray[4, 4] = 929.68d;
            double X = 75d;
            double Y = 350d;
            double P = Numerics.Data.Interpolation.Bilinear(X, Y, XArray, YArray, ZArray);
            Assert.AreEqual(P, 874.84d, 1E-6);
        }

        [TestMethod()]
        public void Test_BiLogLinLin()
        {
            var XArray = new double[] { 50d, 100d, 150d, 200d, 250d };
            var YArray = new double[] { 100d, 200d, 300d, 400d, 500d };
            var ZArray = new double[5, 5];
            ZArray[0, 0] = 850.36d;
            ZArray[1, 0] = 865.39d;
            ZArray[2, 0] = 867.27d;
            ZArray[3, 0] = 869.71d;
            ZArray[4, 0] = 871.84d;
            ZArray[0, 1] = 868.45d;
            ZArray[1, 1] = 878.66d;
            ZArray[2, 1] = 880.47d;
            ZArray[3, 1] = 881.91d;
            ZArray[4, 1] = 883.19d;
            ZArray[0, 2] = 896.71d;
            ZArray[1, 2] = 901.2d;
            ZArray[2, 2] = 901.92d;
            ZArray[3, 2] = 902.4d;
            ZArray[4, 2] = 903.22d;
            ZArray[0, 3] = 914.78d;
            ZArray[1, 3] = 918.54d;
            ZArray[2, 3] = 919.14d;
            ZArray[3, 3] = 919.9d;
            ZArray[4, 3] = 920.18d;
            ZArray[0, 4] = 928.87d;
            ZArray[1, 4] = 929.46d;
            ZArray[2, 4] = 929.54d;
            ZArray[3, 4] = 929.62d;
            ZArray[4, 4] = 929.68d;
            double X = 75d;
            double Y = 350d;
            double P = Numerics.Data.Interpolation.Bilinear(X, Y, XArray, YArray, ZArray, XTransform: Transform.Logarithmic);
            Assert.AreEqual(P, 875.919023759d, 1E-6);
        }

        [TestMethod()]
        public void Test_BiLinLogLin()
        {
            var XArray = new double[] { 50d, 100d, 150d, 200d, 250d };
            var YArray = new double[] { 100d, 200d, 300d, 400d, 500d };
            var ZArray = new double[5, 5];
            ZArray[0, 0] = 850.36d;
            ZArray[1, 0] = 865.39d;
            ZArray[2, 0] = 867.27d;
            ZArray[3, 0] = 869.71d;
            ZArray[4, 0] = 871.84d;
            ZArray[0, 1] = 868.45d;
            ZArray[1, 1] = 878.66d;
            ZArray[2, 1] = 880.47d;
            ZArray[3, 1] = 881.91d;
            ZArray[4, 1] = 883.19d;
            ZArray[0, 2] = 896.71d;
            ZArray[1, 2] = 901.2d;
            ZArray[2, 2] = 901.92d;
            ZArray[3, 2] = 902.4d;
            ZArray[4, 2] = 903.22d;
            ZArray[0, 3] = 914.78d;
            ZArray[1, 3] = 918.54d;
            ZArray[2, 3] = 919.14d;
            ZArray[3, 3] = 919.9d;
            ZArray[4, 3] = 920.18d;
            ZArray[0, 4] = 928.87d;
            ZArray[1, 4] = 929.46d;
            ZArray[2, 4] = 929.54d;
            ZArray[3, 4] = 929.62d;
            ZArray[4, 4] = 929.68d;
            double X = 75d;
            double Y = 350d;
            double P = Numerics.Data.Interpolation.Bilinear(X, Y, XArray, YArray, ZArray, YTransform: Transform.Logarithmic);
            Assert.AreEqual(P, 874.909523653d, 1E-6);
        }

        [TestMethod()]
        public void Test_BiLinLogLog()
        {
            var XArray = new double[] { 50d, 100d, 150d, 200d, 250d };
            var YArray = new double[] { 100d, 200d, 300d, 400d, 500d };
            var ZArray = new double[5, 5];
            ZArray[0, 0] = 850.36d;
            ZArray[1, 0] = 865.39d;
            ZArray[2, 0] = 867.27d;
            ZArray[3, 0] = 869.71d;
            ZArray[4, 0] = 871.84d;
            ZArray[0, 1] = 868.45d;
            ZArray[1, 1] = 878.66d;
            ZArray[2, 1] = 880.47d;
            ZArray[3, 1] = 881.91d;
            ZArray[4, 1] = 883.19d;
            ZArray[0, 2] = 896.71d;
            ZArray[1, 2] = 901.2d;
            ZArray[2, 2] = 901.92d;
            ZArray[3, 2] = 902.4d;
            ZArray[4, 2] = 903.22d;
            ZArray[0, 3] = 914.78d;
            ZArray[1, 3] = 918.54d;
            ZArray[2, 3] = 919.14d;
            ZArray[3, 3] = 919.9d;
            ZArray[4, 3] = 920.18d;
            ZArray[0, 4] = 928.87d;
            ZArray[1, 4] = 929.46d;
            ZArray[2, 4] = 929.54d;
            ZArray[3, 4] = 929.62d;
            ZArray[4, 4] = 929.68d;
            double X = 75d;
            double Y = 350d;
            double P = Numerics.Data.Interpolation.Bilinear(X, Y, XArray, YArray, ZArray, YTransform: Transform.Logarithmic, ZTransform: Transform.Logarithmic);
            Assert.AreEqual(P, 874.886034788d, 1E-6);
        }

        [TestMethod()]
        public void Test_BiLogLogLog()
        {
            var XArray = new double[] { 50d, 100d, 150d, 200d, 250d };
            var YArray = new double[] { 100d, 200d, 300d, 400d, 500d };
            var ZArray = new double[5, 5];
            ZArray[0, 0] = 850.36d;
            ZArray[1, 0] = 865.39d;
            ZArray[2, 0] = 867.27d;
            ZArray[3, 0] = 869.71d;
            ZArray[4, 0] = 871.84d;
            ZArray[0, 1] = 868.45d;
            ZArray[1, 1] = 878.66d;
            ZArray[2, 1] = 880.47d;
            ZArray[3, 1] = 881.91d;
            ZArray[4, 1] = 883.19d;
            ZArray[0, 2] = 896.71d;
            ZArray[1, 2] = 901.2d;
            ZArray[2, 2] = 901.92d;
            ZArray[3, 2] = 902.4d;
            ZArray[4, 2] = 903.22d;
            ZArray[0, 3] = 914.78d;
            ZArray[1, 3] = 918.54d;
            ZArray[2, 3] = 919.14d;
            ZArray[3, 3] = 919.9d;
            ZArray[4, 3] = 920.18d;
            ZArray[0, 4] = 928.87d;
            ZArray[1, 4] = 929.46d;
            ZArray[2, 4] = 929.54d;
            ZArray[3, 4] = 929.62d;
            ZArray[4, 4] = 929.68d;
            double X = 75d;
            double Y = 350d;
            double P = Numerics.Data.Interpolation.Bilinear(X, Y, XArray, YArray, ZArray, XTransform: Transform.Logarithmic, YTransform: Transform.Logarithmic, ZTransform: Transform.Logarithmic);
            Assert.AreEqual(P, 875.96271389d, 1E-6);
        }
    }
}
