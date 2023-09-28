using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Data;

namespace Data.Interpolation
{
    [TestClass]
    public class Test_Bilinear
    {
        [TestMethod()]
        public void Test_BiLinear()
        {       
            var x1Array = new double[] { 100d, 200d, 300d, 400d, 500d };
            var x2Array = new double[] { 50d, 100d, 150d, 200d, 250d };
            var yArray = new double[5, 5];
            yArray[0, 0] = 850.36d;
            yArray[1, 0] = 865.39d;
            yArray[2, 0] = 867.27d;
            yArray[3, 0] = 869.71d;
            yArray[4, 0] = 871.84d;
            yArray[0, 1] = 868.45d;
            yArray[1, 1] = 878.66d;
            yArray[2, 1] = 880.47d;
            yArray[3, 1] = 881.91d;
            yArray[4, 1] = 883.19d;
            yArray[0, 2] = 896.71d;
            yArray[1, 2] = 901.2d;
            yArray[2, 2] = 901.92d;
            yArray[3, 2] = 902.4d;
            yArray[4, 2] = 903.22d;
            yArray[0, 3] = 914.78d;
            yArray[1, 3] = 918.54d;
            yArray[2, 3] = 919.14d;
            yArray[3, 3] = 919.9d;
            yArray[4, 3] = 920.18d;
            yArray[0, 4] = 928.87d;
            yArray[1, 4] = 929.46d;
            yArray[2, 4] = 929.54d;
            yArray[3, 4] = 929.62d;
            yArray[4, 4] = 929.68d;
            var bilinear = new Bilinear(x1Array, x2Array, yArray);         
            double x1 = 350d;
            double x2 = 75d;
            double y = bilinear.Interpolate(x1, x2);
            Assert.AreEqual(y, 874.84d, 1E-6);
        }

        [TestMethod()]
        public void Test_BiLogLinLin()
        {
            var x1Array = new double[] { 100d, 200d, 300d, 400d, 500d };
            var x2Array = new double[] { 50d, 100d, 150d, 200d, 250d };
            var yArray = new double[5, 5];
            yArray[0, 0] = 850.36d;
            yArray[1, 0] = 865.39d;
            yArray[2, 0] = 867.27d;
            yArray[3, 0] = 869.71d;
            yArray[4, 0] = 871.84d;
            yArray[0, 1] = 868.45d;
            yArray[1, 1] = 878.66d;
            yArray[2, 1] = 880.47d;
            yArray[3, 1] = 881.91d;
            yArray[4, 1] = 883.19d;
            yArray[0, 2] = 896.71d;
            yArray[1, 2] = 901.2d;
            yArray[2, 2] = 901.92d;
            yArray[3, 2] = 902.4d;
            yArray[4, 2] = 903.22d;
            yArray[0, 3] = 914.78d;
            yArray[1, 3] = 918.54d;
            yArray[2, 3] = 919.14d;
            yArray[3, 3] = 919.9d;
            yArray[4, 3] = 920.18d;
            yArray[0, 4] = 928.87d;
            yArray[1, 4] = 929.46d;
            yArray[2, 4] = 929.54d;
            yArray[3, 4] = 929.62d;
            yArray[4, 4] = 929.68d;
            var bilinear = new Bilinear(x1Array, x2Array, yArray) { X1Transform = Transform.Logarithmic };
            double x1 = 350d;
            double x2 = 75d;
            double y = bilinear.Interpolate(x1, x2);          
            Assert.AreEqual(y, 874.909523653025d, 1E-6);
        }

        [TestMethod()]
        public void Test_BiLinLogLin()
        {
            var x1Array = new double[] { 100d, 200d, 300d, 400d, 500d };
            var x2Array = new double[] { 50d, 100d, 150d, 200d, 250d };
            var yArray = new double[5, 5];
            yArray[0, 0] = 850.36d;
            yArray[1, 0] = 865.39d;
            yArray[2, 0] = 867.27d;
            yArray[3, 0] = 869.71d;
            yArray[4, 0] = 871.84d;
            yArray[0, 1] = 868.45d;
            yArray[1, 1] = 878.66d;
            yArray[2, 1] = 880.47d;
            yArray[3, 1] = 881.91d;
            yArray[4, 1] = 883.19d;
            yArray[0, 2] = 896.71d;
            yArray[1, 2] = 901.2d;
            yArray[2, 2] = 901.92d;
            yArray[3, 2] = 902.4d;
            yArray[4, 2] = 903.22d;
            yArray[0, 3] = 914.78d;
            yArray[1, 3] = 918.54d;
            yArray[2, 3] = 919.14d;
            yArray[3, 3] = 919.9d;
            yArray[4, 3] = 920.18d;
            yArray[0, 4] = 928.87d;
            yArray[1, 4] = 929.46d;
            yArray[2, 4] = 929.54d;
            yArray[3, 4] = 929.62d;
            yArray[4, 4] = 929.68d;
            var bilinear = new Bilinear(x1Array, x2Array, yArray) { X2Transform = Transform.Logarithmic };
            double x1 = 350d;
            double x2 = 75d;
            double y = bilinear.Interpolate(x1, x2);
            Assert.AreEqual(y, 875.919023759159d, 1E-6);
        }

        [TestMethod()]
        public void Test_BiLinLogLog()
        {
            var x1Array = new double[] { 100d, 200d, 300d, 400d, 500d };
            var x2Array = new double[] { 50d, 100d, 150d, 200d, 250d };
            var yArray = new double[5, 5];
            yArray[0, 0] = 850.36d;
            yArray[1, 0] = 865.39d;
            yArray[2, 0] = 867.27d;
            yArray[3, 0] = 869.71d;
            yArray[4, 0] = 871.84d;
            yArray[0, 1] = 868.45d;
            yArray[1, 1] = 878.66d;
            yArray[2, 1] = 880.47d;
            yArray[3, 1] = 881.91d;
            yArray[4, 1] = 883.19d;
            yArray[0, 2] = 896.71d;
            yArray[1, 2] = 901.2d;
            yArray[2, 2] = 901.92d;
            yArray[3, 2] = 902.4d;
            yArray[4, 2] = 903.22d;
            yArray[0, 3] = 914.78d;
            yArray[1, 3] = 918.54d;
            yArray[2, 3] = 919.14d;
            yArray[3, 3] = 919.9d;
            yArray[4, 3] = 920.18d;
            yArray[0, 4] = 928.87d;
            yArray[1, 4] = 929.46d;
            yArray[2, 4] = 929.54d;
            yArray[3, 4] = 929.62d;
            yArray[4, 4] = 929.68d;
            var bilinear = new Bilinear(x1Array, x2Array, yArray) { X2Transform = Transform.Logarithmic, YTransform = Transform.Logarithmic };
            double x1 = 350d;
            double x2 = 75d;
            double y = bilinear.Interpolate(x1, x2);
            Assert.AreEqual(y, 875.896104342695d, 1E-6);
        }

        [TestMethod()]
        public void Test_BiLogLogLog()
        {
            var x1Array = new double[] { 100d, 200d, 300d, 400d, 500d };
            var x2Array = new double[] { 50d, 100d, 150d, 200d, 250d };
            var yArray = new double[5, 5];
            yArray[0, 0] = 850.36d;
            yArray[1, 0] = 865.39d;
            yArray[2, 0] = 867.27d;
            yArray[3, 0] = 869.71d;
            yArray[4, 0] = 871.84d;
            yArray[0, 1] = 868.45d;
            yArray[1, 1] = 878.66d;
            yArray[2, 1] = 880.47d;
            yArray[3, 1] = 881.91d;
            yArray[4, 1] = 883.19d;
            yArray[0, 2] = 896.71d;
            yArray[1, 2] = 901.2d;
            yArray[2, 2] = 901.92d;
            yArray[3, 2] = 902.4d;
            yArray[4, 2] = 903.22d;
            yArray[0, 3] = 914.78d;
            yArray[1, 3] = 918.54d;
            yArray[2, 3] = 919.14d;
            yArray[3, 3] = 919.9d;
            yArray[4, 3] = 920.18d;
            yArray[0, 4] = 928.87d;
            yArray[1, 4] = 929.46d;
            yArray[2, 4] = 929.54d;
            yArray[3, 4] = 929.62d;
            yArray[4, 4] = 929.68d;
            var bilinear = new Bilinear(x1Array, x2Array, yArray) { X1Transform = Transform.Logarithmic, X2Transform = Transform.Logarithmic, YTransform = Transform.Logarithmic };
            double x1 = 350d;
            double x2 = 75d;
            double y = bilinear.Interpolate(x1, x2);
            Assert.AreEqual(y, 875.962713889793d, 1E-6);
        }

        [TestMethod()]
        public void Test_BiZLinear()
        {
            var x1Array = new double[] { 100d, 200d, 300d, 400d, 500d };
            var x2Array = new double[] { 50d, 100d, 150d, 200d, 250d };
            var yArray = new double[5, 5];
            yArray[0, 0] = 0.91468032d;
            yArray[1, 0] = 0.930847173d;
            yArray[2, 0] = 0.932869374d;
            yArray[3, 0] = 0.935493933d;
            yArray[4, 0] = 0.937785044d;
            yArray[0, 1] = 0.934138628d;
            yArray[1, 1] = 0.945120902d;
            yArray[2, 1] = 0.947067808d;
            yArray[3, 1] = 0.948616728d;
            yArray[4, 1] = 0.949993546d;
            yArray[0, 2] = 0.964536184d;
            yArray[1, 2] = 0.969365803d;
            yArray[2, 2] = 0.970140263d;
            yArray[3, 2] = 0.97065657;
            yArray[4, 2] = 0.971538594d;
            yArray[0, 3] = 0.98397298d;
            yArray[1, 3] = 0.988017382d;
            yArray[2, 3] = 0.988662766d;
            yArray[3, 3] = 0.989480251d;
            yArray[4, 3] = 0.98978143d;
            yArray[0, 4] = 0.999128732d;
            yArray[1, 4] = 0.999763359d;
            yArray[2, 4] = 0.999849411d;
            yArray[3, 4] = 0.999935462d;
            yArray[4, 4] = 1d;
            var bilinear = new Bilinear(x1Array, x2Array, yArray);
            double x1 = 350d;
            double x2 = 75d;
            double y = bilinear.Interpolate(x1, x2);
            Assert.AreEqual(y, 0.941011961104896d, 1E-6);
        }

        [TestMethod()]
        public void Test_BiLinLinZ()
        {
            var x1Array = new double[] { 100d, 200d, 300d, 400d, 500d };
            var x2Array = new double[] { 50d, 100d, 150d, 200d, 250d };
            var yArray = new double[5, 5];
            yArray[0, 0] = 0.91468032d;
            yArray[1, 0] = 0.930847173d;
            yArray[2, 0] = 0.932869374d;
            yArray[3, 0] = 0.935493933d;
            yArray[4, 0] = 0.937785044d;
            yArray[0, 1] = 0.934138628d;
            yArray[1, 1] = 0.945120902d;
            yArray[2, 1] = 0.947067808d;
            yArray[3, 1] = 0.948616728d;
            yArray[4, 1] = 0.949993546d;
            yArray[0, 2] = 0.964536184d;
            yArray[1, 2] = 0.969365803d;
            yArray[2, 2] = 0.970140263d;
            yArray[3, 2] = 0.97065657;
            yArray[4, 2] = 0.971538594d;
            yArray[0, 3] = 0.98397298d;
            yArray[1, 3] = 0.988017382d;
            yArray[2, 3] = 0.988662766d;
            yArray[3, 3] = 0.989480251d;
            yArray[4, 3] = 0.98978143d;
            yArray[0, 4] = 0.999128732d;
            yArray[1, 4] = 0.999763359d;
            yArray[2, 4] = 0.999849411d;
            yArray[3, 4] = 0.999935462d;
            yArray[4, 4] = 1d;
            var bilinear = new Bilinear(x1Array, x2Array, yArray) { YTransform = Transform.NormalZ };
            double x1 = 350d;
            double x2 = 75d;
            double y = bilinear.Interpolate(x1, x2);
            Assert.AreEqual(y, 0.941330600880593d, 1E-6);
        }

        [TestMethod()]
        public void Test_BilinearEdgeCases()
        {
            var x1Array = new double[] { 100d, 200d, 300d, 400d, 500d };
            var x2Array = new double[] { 50d, 100d, 150d, 200d, 250d };
            var yArray = new double[5, 5];
            yArray[0, 0] = 850.36d;
            yArray[1, 0] = 865.39d;
            yArray[2, 0] = 867.27d;
            yArray[3, 0] = 869.71d;
            yArray[4, 0] = 871.84d;
            yArray[0, 1] = 868.45d;
            yArray[1, 1] = 878.66d;
            yArray[2, 1] = 880.47d;
            yArray[3, 1] = 881.91d;
            yArray[4, 1] = 883.19d;
            yArray[0, 2] = 896.71d;
            yArray[1, 2] = 901.2d;
            yArray[2, 2] = 901.92d;
            yArray[3, 2] = 902.4d;
            yArray[4, 2] = 903.22d;
            yArray[0, 3] = 914.78d;
            yArray[1, 3] = 918.54d;
            yArray[2, 3] = 919.14d;
            yArray[3, 3] = 919.9d;
            yArray[4, 3] = 920.18d;
            yArray[0, 4] = 928.87d;
            yArray[1, 4] = 929.46d;
            yArray[2, 4] = 929.54d;
            yArray[3, 4] = 929.62d;
            yArray[4, 4] = 929.68d;
            var bilinear = new Bilinear(x1Array, x2Array, yArray);

            // Ascending - Both out of range
            // Top left
            double x1 = 50;
            double x2 = 28;
            double y = bilinear.Interpolate(x1, x2);
            Assert.AreEqual(y, 850.36, 1E-6);
            // Top Right
            x1 = 50;
            x2 = 300;
            y = bilinear.Interpolate(x1, x2);
            Assert.AreEqual(y, 928.87, 1E-6);
            // Bottom Left
            x1 = 600;
            x2 = 25;
            y = bilinear.Interpolate(x1, x2);
            Assert.AreEqual(y, 871.84, 1E-6);
            // Bottom Right
            x1 = 600;
            x2 = 300;
            y = bilinear.Interpolate(x1, x2);
            Assert.AreEqual(y, 929.68, 1E-6);

            // Ascending - x1 out
            // Top
            x1 = 50;
            x2 = 75;
            y = bilinear.Interpolate(x1, x2);
            Assert.AreEqual(y, 859.405, 1E-6);
            // Bottom
            x1 = 600;
            x2 = 225;
            y = bilinear.Interpolate(x1, x2);
            Assert.AreEqual(y, 924.93, 1E-6);

            // Ascending - x2 out
            // Top
            x1 = 125;
            x2 = 25;
            y = bilinear.Interpolate(x1, x2);
            Assert.AreEqual(y, 854.11750, 1E-6);
            // Bottom
            x1 = 450;
            x2 = 300;
            y = bilinear.Interpolate(x1, x2);
            Assert.AreEqual(y, 929.65000, 1E-6);

        }
    }
}
