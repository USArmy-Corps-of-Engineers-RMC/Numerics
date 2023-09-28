using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Data;
using Numerics.Distributions;
using Numerics.Functions;

namespace Functions
{
    [TestClass]
    public class Test_Functions
    {

        [TestMethod]
        public void Test_Linear_Function()
        {
            var func = new LinearFunction(10, 0.5, 20);
            double y = func.Function(400);
            double x = func.InverseFunction(y);
            Assert.AreEqual(400, x, 1E-6);

            func.ConfidenceLevel = 0.75;
            y = func.Function(400);
            x = func.InverseFunction(y);
            Assert.AreEqual(400, x, 1E-6);
        }

        [TestMethod]
        public void Test_Power_Function()
        {
            var func = new PowerFunction(10, 2, 0,0.1);
            double y = func.Function(400);
            double x = func.InverseFunction(y);
            Assert.AreEqual(400, x, 1E-6);

            func.ConfidenceLevel = 0.75;
            y = func.Function(400);
            x = func.InverseFunction(y);
            Assert.AreEqual(400, x, 1E-6);
        }

        [TestMethod]
        public void Test_InversePower_Function()
        {
            var func = new PowerFunction(10, 2, 0, 0.1);
            func.IsInverse = true;
            double y = func.Function(6);
            double x = func.InverseFunction(y);
            Assert.AreEqual(6, x, 1E-6);

            func.ConfidenceLevel = 0.75;
            y = func.Function(6);
            x = func.InverseFunction(y);
            Assert.AreEqual(6, x, 1E-6);
        }

        [TestMethod]
        public void Test_Tabular_Function()
        {
            var XArray = new double[] { 50, 100, 150, 200, 250 };
            var YArray = new UnivariateDistributionBase[] { new Deterministic(100), new Deterministic(200), new Deterministic(300), new Deterministic(400), new Deterministic(500) };

            
            var opd = new UncertainOrderedPairedData(XArray, YArray, true, SortOrder.Ascending, true, SortOrder.Ascending, UnivariateDistributionType.Deterministic);
            var func = new TabularFunction(opd) { XTransform = Transform.Logarithmic, YTransform = Transform.None };

            // Given X
            double X = 75.0;
            double Y = func.Function(X);
            //Assert.AreEqual(Y, 150.0, 1E-6);

            // Given Y
            //opd = new OrderedPairedData(YArray, XArray, true, SortOrder.Ascending, true, SortOrder.Ascending);
            //Y = 75d;
            X = func.InverseFunction(Y);
            //Assert.AreEqual(X, 75.0, 1E-6);
        }

    }
}
