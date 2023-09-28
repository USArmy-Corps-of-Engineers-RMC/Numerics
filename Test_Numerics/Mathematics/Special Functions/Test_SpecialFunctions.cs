using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Mathematics.SpecialFunctions;
using Numerics;
using System.Diagnostics;

namespace Mathematics.SpecialFunctions
{

    /// <summary>
    /// Unit tests for Special Functions.
    /// I am only able to test some of these special functions directly with MS Excel and with values provided by:
    /// "Numerical Recipes, Routines and Examples in Basic", J.C. Sprott, Cambridge University Press, 1991.
    /// Others must be tested indirectly through the continuous distribution classes.
    /// </summary>
    [TestClass()]
    public class Test_SpecialFunctions
    {

        /// <summary>
        /// Test Beta Function
        /// </summary>
        [TestMethod()]
        public void Test_Beta()
        {

            // Define variables
            var testX = new double[] { 1d, 0.2d, 1d, 0.4d, 1d, 0.6d, 0.8d, 6d, 6d, 6d, 6d, 6d, 7d, 5d, 4d, 3d, 2d };
            var testY = new double[] { 1d, 1d, 0.2d, 1d, 0.4d, 1d, 1d, 6d, 5d, 4d, 3d, 2d, 7d, 5d, 4d, 3d, 2d };
            var testValid = new double[] { 1d, 5d, 5d, 2.5d, 2.5d, 1.666667d, 1.25d, 0.0003607504d, 0.0007936508d, 0.001984127d, 0.005952381d, 0.0238095d, 0.00008325008d, 0.001587302d, 0.007142857d, 0.03333333d, 0.1666667d };
            var testResults = new double[testValid.Length];

            // test values
            for (int i = 0; i < testValid.Length; i++)
            {
                testResults[i] = Beta.Function(testX[i], testY[i]);
                Assert.AreEqual(testResults[i], testValid[i], 1E-4);
            }
        }

        /// <summary>
        /// Test Error Function
        /// </summary>
        [TestMethod()]
        public void Test_Erf()
        {

            // Define variables
            var testX = new double[] { 0d, 0.1d, 0.2d, 0.3d, 0.4d, 0.5d, 0.6d, 0.7d, 0.8d, 0.9d, 1d, 1.1d, 1.2d, 1.3d, 1.4d, 1.5d, 1.6d, 1.7d, 1.8d, 1.9d };
            var testValid = new double[] { 0d, 0.112462916018285d, 0.222702589210478d, 0.328626759459127d, 0.428392355046668d, 0.520499877813047d, 0.603856090847926d, 0.677801193837418d, 0.742100964707661d, 0.796908212422832d, 0.842700792949715d, 0.880205069574082d, 0.910313978229635d, 0.934007944940652d, 0.952285119762649d, 0.966105146475311d, 0.976348383344644d, 0.983790458590775d, 0.989090501635731d, 0.992790429235258d };
            var testResults = new double[testValid.Length];

            // test values
            for (int i = 0; i < testValid.Length; i++)
            {
                testResults[i] = Erf.Function(testX[i]);
                Assert.AreEqual(testResults[i],testValid[i], 1E-4);
            }
        }

        /// <summary>
        /// Test Error Complement Function
        /// </summary>
        [TestMethod()]
        public void Test_Erfc()
        {

            // Define variables
            var testX = new double[] { 0d, 0.1d, 0.2d, 0.3d, 0.4d, 0.5d, 0.6d, 0.7d, 0.8d, 0.9d, 1d, 1.1d, 1.2d, 1.3d, 1.4d, 1.5d, 1.6d, 1.7d, 1.8d, 1.9d };
            var testValid = new double[] { 1d, 0.887537083981715d, 0.777297410789522d, 0.671373240540873d, 0.571607644953332d, 0.479500122186953d, 0.396143909152074d, 0.322198806162582d, 0.257899035292339d, 0.203091787577168d, 0.157299207050285d, 0.119794930425918d, 0.0896860217703646d, 0.0659920550593475d, 0.0477148802373512d, 0.0338948535246893d, 0.023651616655356d, 0.0162095414092254d, 0.0109094983642693d, 0.00720957076474253d };
            var testResults = new double[testValid.Length];

            // test values
            for (int i = 0; i < testValid.Length; i++)
            {
                testResults[i] = Erf.Erfc(testX[i]);
                Assert.AreEqual(testResults[i], testValid[i], 1E-4);
            }
        }

        /// <summary>
        /// Test Gamma Function
        /// </summary>
        [TestMethod()]
        public void Test_Gamma()
        {

            // Define variables
            var testX = new double[] { 1d, 1.2d, 1.4d, 1.6d, 1.8d, 2d, 0.2d, 0.4d, 0.6d, 0.8d, -0.2d, -0.4d, -0.6d, -0.8d, 10d, 20d, 30d, 40d, 50d, 60d };
            var testValid = new double[] { 1d, 0.918168742399761d, 0.887263817503075d, 0.89351534928769d, 0.931383770980243d, 1d, 4.5908437119988d, 2.21815954375769d, 1.48919224881282d, 1.1642297137253d, -5.82114856862652d, -3.72298062203204d, -3.69693257292948d, -5.73855463999851d, 362880d, 121645100408832000d, 8.8417619937397E+30d, 2.03978820811974E+46d, 6.08281864034268E+62d, 1.3868311854569E+80d };
            var testResults = new double[testValid.Length];

            // test values
            for (int i = 0; i < testValid.Length; i++)
            {
                testResults[i] = Gamma.Function(testX[i]);
                Assert.AreEqual(Math.Abs(testValid[i]- testResults[i]) / testValid[i] < 0.01, true);
            }
        }

        [TestMethod()]
        public void Test_Debye()
        {
            var testX = new double[] { 0.1, 1.0, 2.8, 9.5, 10, 15, 25, 100};
            var testValid = new double[] { 0.9629999, 0.6744156, 0.3099952, 0.02241066, 0.01929577, 0.005771263, 0.001246836, 1.948182e-05 };
            var testResults = new double[testValid.Length];

            // test values
            for (int i = 0; i < testValid.Length; i++)
            {
                testResults[i] = Debye.Function(testX[i]);
                Assert.AreEqual(testResults[i], testValid[i], 1E-4);
            }
        }

       
        [TestMethod]
        public void Test_Combinations()
        {
            var cc = Factorial.AllCombinations(5);
            for (int i = 0; i < cc.GetLength(0); i++)
            {
                string s = "";
                for (int j = 0; j < cc.GetLength(1); j++)
                {
                    s += cc[i, j].ToString() + " ";

                }
                Debug.WriteLine(s);
            }

        }

        [TestMethod]
        public void Test_BinomialCoefficient()
        {
            var c = Factorial.BinomialCoefficient(4, 3);
        }

    }
}