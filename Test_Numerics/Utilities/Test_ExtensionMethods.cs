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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics;
using System;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Markup;

namespace Utilities
{
    /// <summary>
    /// Testing the ExtensionMethods class
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
    /// Tiki Gonzalez, USACE Risk Management Center, julian.t.gonzalez@usace.army.mil
    /// </para>
    /// </remarks>
 
    /// <summary>
    /// Supporting class for the GetAttribute Testing.
    /// </summary>
    public class DescriptionAttribute : Attribute
    {
        public string Description { get; }
        public DescriptionAttribute(string description)
        {
            Description = description;
        }
    }

    [TestClass]
    public class Test_ExtensionMethods
    {
        [Flags]
        public enum SampleFlags
        {
            [Description("None")]
            None = 0,
            [Description("First Flag")]
            FirstFlag = 1,
            [Description("Second Flag")]
            SecondFlag = 2,
            [Description("Combined Flags")]
            CombinedFlags = FirstFlag | SecondFlag
        }

        /// <summary>
        /// Testing get attribute with the Description Attribute example.
        /// </summary>
        [TestMethod]
        public void Test_GetAttribute()
        {
            var enumVal = SampleFlags.CombinedFlags;
            var expectedDescription = "Combined Flags";

            var attribute = ExtensionMethods.GetAttributeOfType<DescriptionAttribute>(enumVal);

            Assert.AreEqual(expectedDescription, attribute.Description);


        }

        /// <summary>
        /// Testing a random integer array generator. Should output 5 random integers. 
        /// </summary>
        [TestMethod]
        public void Test_NextIntegers()
        {
            Random random = new Random();
            var result = ExtensionMethods.NextIntegers(random, 5);
            for (int i = 0; i < result.Length; i++)
            {
                Debug.WriteLine(result[i]);
            }
        }

        /// <summary>
        /// Should output 5 random integers between 0 and 1000.
        /// </summary>
        [TestMethod]
        public void Test_NextIntegersMinMax()
        {
            Random random = new Random();
            var result = ExtensionMethods.NextIntegers(random, 0, 1000, 5);
            for (int i = 0; i < result.Length; i++)
            {
                Debug.WriteLine(result[i]);
            }
        }

        /// <summary>
        /// Should ouput 5 random integers between 0 and 1000 with replacement.
        /// </summary>
        [TestMethod]
        public void Test_NextIntegersReplacement()
        {
            Random random = new Random();
            var result = ExtensionMethods.NextIntegers(random, 0, 1000, 5);
            for (int i = 0; i < result.Length; i++)
            {
                Debug.WriteLine(result[i]);
            }
        }

        /// <summary>
        /// Should ouput 5 random integers between 0 and 1000 without replacement.
        /// </summary>
        [TestMethod]
        public void Test_NextIntegersWithoutReplacement()
        {
            Random random = new Random();
            bool replace = false;
            var result = ExtensionMethods.NextIntegers(random, 0, 1000, 5, replace);
            for (int i = 0; i < result.Length; i++)
            {
                Debug.WriteLine(result[i]);
            }
        }

        /// <summary>
        /// Should ouput 5 random doubles. 
        /// </summary>
        [TestMethod]
        public void Test_NextDoubles()
        {
            Random random = new Random();
            var result = ExtensionMethods.NextDoubles(random, 5);
            for (int i = 0; i < result.Length; i++)
            {
                Debug.WriteLine(result[i]);
            }
        }

        /// <summary>
        /// Should ouput 
        /// </summary>
        [TestMethod]
        public void Test_NextDoubles2D()
        {
            Random random = new Random();
            var result = ExtensionMethods.NextDoubles(random, 5, 3);
            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    Debug.WriteLine(result[i, j]);
                }

            }
        }

        /// <summary>
        /// Gets specific column from 2D array.
        /// </summary>
        [TestMethod]
        public void Test_GetColumn()
        {
            int[,] array = new int[,]
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 }
            };

            int expectedColumnIndex = 1;
            int[] expectedColumn = new int[] { 2, 5, 8 };

            int[] result = ExtensionMethods.GetColumn(array, expectedColumnIndex);

            CollectionAssert.AreEqual(expectedColumn, result);
        }

        /// <summary>
        /// Gets specific row from 2D array.
        /// </summary>
        [TestMethod]
        public void Test_GetRow()
        {
            int[,] array = new int[,]
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 }
            };

            int expectedRowIndex = 1;
            int[] expectedColumn = new int[] { 4,5,6};

            int[] result = ExtensionMethods.GetRow(array, expectedRowIndex);

            CollectionAssert.AreEqual(expectedColumn, result);
        }

        /// <summary>
        /// Array addition of two arrays.
        /// </summary>
        [TestMethod]
        public void Test_Add()
        {
            double[] array = new double[] { 1, 2, 3 };
            double[] values = new double[] { 1, 2, 3 };

            var result = ExtensionMethods.Add(array, values);
            var true_result = new double[] { 2, 4, 6 };
            CollectionAssert.AreEqual(result, true_result);
        }

        /// <summary>
        /// Edge case of different sized arrays throwing exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_AddEdge()
        {
            double[] array = new double[] { 1, 2, 3 };
            double[] values = new double[] { 1, 2, 3, 4};

            var result = ExtensionMethods.Add(array, values);
        }

        /// <summary>
        /// Array subtraction of two arrays.
        /// </summary>
        [TestMethod]
        public void Test_Subtract()
        {
            var array = new double[] { 3, 2, 1 };
            var values = new double[] { 1, 2, 3 };

            var result = ExtensionMethods.Subtract(array, values);
            var true_result = new double[] { 2, 0, -2 };

            CollectionAssert.AreEqual(result, true_result);
        }

        /// <summary>
        /// Edge case of different sized arrays throwing exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_SubtractEdge()
        {
            var array = new double[] { 3, 2, 1,0 };
            var values = new double[] { 1, 2, 3 };

            var result = ExtensionMethods.Subtract(array, values);
            
        }

        /// <summary>
        /// Multiplying an array by a scalar.
        /// </summary>
        [TestMethod]
        public void Test_Multiply()
        {
            var array = new double[] { 4, 5, 6, 7 };
            var scalar = 2;
            var result = ExtensionMethods.Multiply(array, scalar);
            var true_result = new double[] { 8, 10, 12, 14 };

            CollectionAssert.AreEqual(result, true_result);
        }
        
        /// <summary>
        /// Dividing an array by a scalar. 
        /// </summary>
        [TestMethod]
        public void Test_Divide()
        {
            var array = new double[] { 4, -5, 6, 7 };
            var scalar = 2;
            var result = ExtensionMethods.Divide(array, scalar);
            var true_result = new double[] { 2, -2.5, 3, 3.5 };

            CollectionAssert.AreEqual(result, true_result);
        }
        /// <summary>
        /// Testing a subset of an array. 
        /// </summary>
        [TestMethod]
        public void Test_Subset1()
        {
            var array = new double[] { 1, 2, 3, 4, 5, 6 };
            var startIndex = 3;
            var result = ExtensionMethods.Subset(array, startIndex);
            var true_result = new double[] { 4,5,6 };

            CollectionAssert.IsSubsetOf(result, array);
            CollectionAssert.AreEqual(result, true_result);
        }

        /// <summary>
        /// Testing a subset of an array. 
        /// </summary>
        [TestMethod]
        public void Test_Subset()
        {
            var array = new double[] { 1, 2, 3, 4, 5, 6 };
            var startIndex = 0;
            var endIndex = 2;
            var result = ExtensionMethods.Subset(array, startIndex, endIndex);
            var true_result = new double[] { 1, 2, 3 };

            CollectionAssert.IsSubsetOf(result,array);
            CollectionAssert.AreEqual(result,true_result);
        }

        /// <summary>
        /// Testing the fill function to see if the 2D array was populated with the specified value.
        /// </summary>
        [TestMethod]
        public void Test_Fill2D()
        {
            var array = new double[3, 3];
            var fillValue = 1.2;
            ExtensionMethods.Fill<double>(array, fillValue);

            for (int i = 0; i < array.GetLength(0); i++)
                for(int j = 0; j < array.GetLength(1); j++)
                    Assert.AreEqual(fillValue,array[i,j]);

        }

        /// <summary>
        /// Testing the fill function to see if the array was populated with the specified value.
        /// </summary>
        [TestMethod]
        public void Test_Fill()
        {
            var array = new double[3];
            var fillValue = 1.2;
            ExtensionMethods.Fill<double>(array, fillValue);

            for (int i = 0; i < array.Length; i++)
                    Assert.AreEqual(fillValue, array[i]);

        }

        /// <summary>
        /// Testing almost equals with a false return.
        /// </summary>
        [TestMethod]
        public void Test_AlmostEquals()
        {
            var a = 2;
            var b = 1;
            var result = ExtensionMethods.AlmostEquals(a, b);

            Assert.IsFalse(result);
        }

        //[TestMethod]
        //public void Test_SubsetIndicator()
        //{
        //    var array = new double[] { 1, 2, 3, 4, 5, 6 };
        //    var indicator = new bool[] { true, false, true, false, true, false };

        //    var result = ExtensionMethods.Subset(array, indicator);
        //    var expected = new double[] { 2, 4, 6 };
        //    CollectionAssert.AreEqual(result, expected);

        //    bool useComplement = true;
        //    var result2 = ExtensionMethods.Subset(array, indicator, useComplement);
        //    var expected2 = new double[] { 1, 3, 5 };
        //    CollectionAssert.AreEqual(result2,expected2);
        //}

        [TestMethod]
        public void Test_SampleSplit()
        {

        }

        [TestMethod]
        public void Test_NextNRIntegers()
        {
            var min = 0;
            var max = 100;
            var length = 100;
            var rand = new Random(12345);

            var split = ExtensionMethods.NextNRIntegers(rand, min, max, length);
            for (int i = 0; i < split.Length; i++)
            {
                Debug.WriteLine(split[i]);
            }
        }
    }
}
