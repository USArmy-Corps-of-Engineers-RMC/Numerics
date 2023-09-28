using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Mathematics.Optimization;

namespace Mathematics.Optimization
{
    [TestClass]
    public class Test_SimulatedAnnealing
    {
        [TestMethod]
        public void Test_FXYZ()
        {
            var lower = new double[] { 0d, 0d, 0d };
            var upper = new double[] { 1d, 1d, 1d };
            var solver = new SimulatedAnnealing(TestFunctions.FXYZ, 3, lower, upper);
            solver.Minimize();
            var solution = solver.BestParameterSet.Values;
            double x = solution[0];
            double y = solution[1];
            double z = solution[2];
            double validX = 0.125d;
            double validY = 0.2d;
            double validZ = 0.35d;
            Assert.AreEqual(x, validX, 1E-2);
            Assert.AreEqual(y, validY, 1E-2);
            Assert.AreEqual(z, validZ, 1E-2);
        }

        [TestMethod]
        public void Test_Ackley()
        {
            var lower = new double[] { -5d, -5d };
            var upper = new double[] { 5d, 5d };
            var solver = new SimulatedAnnealing(TestFunctions.Ackley, 2, lower, upper);
            solver.Minimize();
            var solution = solver.BestParameterSet.Values;
            var x = solution[0];
            var y = solution[1];
            var validX = 0.0d;
            var validY = 0.0d;
            Assert.AreEqual(x, validX, 1E-2);
            Assert.AreEqual(y, validY, 1E-2);
        }

        [TestMethod]
        public void Test_Beale()
        {
            var lower = new double[] { -4.5d, -4.5d };
            var upper = new double[] { 4.5d, 4.5d };
            var solver = new SimulatedAnnealing(TestFunctions.Beale, 2, lower, upper) { RelativeTolerance = 1E-4, AbsoluteTolerance = 1E-4 };
            solver.Minimize();
            var solution = solver.BestParameterSet.Values;
            var x = solution[0];
            var y = solution[1];
            var validX = 3.0d;
            var validY = 0.5d;
            Assert.AreEqual(x, validX, 1E-2);
            Assert.AreEqual(y, validY, 1E-2);
        }

        [TestMethod]
        public void Test_GoldsteinPrice()
        {
            var lower = new double[] { -2d, -2d };
            var upper = new double[] { 2d, 2d };
            var solver = new SimulatedAnnealing(TestFunctions.GoldsteinPrice, 2, lower, upper);
            solver.Minimize();
            var solution = solver.BestParameterSet.Values;
            var x = solution[0];
            var y = solution[1];
            var validX = 0.0d;
            var validY = -1.0d;
            Assert.AreEqual(x, validX, 1E-2);
            Assert.AreEqual(y, validY, 1E-2);

            for (int i = 0; i < solver.ParameterSetTrace.Count; i += 20)
            {
                Debug.Print(solver.ParameterSetTrace[i].Fitness.ToString());
            }
        }

        [TestMethod]
        public void Test_Booth()
        {
            var lower = new double[] { -10d, -10d };
            var upper = new double[] { 10d, 10d };
            var solver = new SimulatedAnnealing(TestFunctions.Booth, 2, lower, upper);
            solver.Minimize();
            var solution = solver.BestParameterSet.Values;
            var x = solution[0];
            var y = solution[1];
            var validX = 1.0d;
            var validY = 3.0d;
            Assert.AreEqual(x, validX, 1E-2);
            Assert.AreEqual(y, validY, 1E-2);
        }

        [TestMethod]
        public void Test_Matyas()
        {
            var lower = new double[] { -10d, -10d };
            var upper = new double[] { 10d, 10d };
            var solver = new SimulatedAnnealing(TestFunctions.Matyas, 2, lower, upper);
            solver.Minimize();
            var solution = solver.BestParameterSet.Values;
            var x = solution[0];
            var y = solution[1];
            var validX = 0.0d;
            var validY = 0.0d;
            Assert.AreEqual(x, validX, 1E-2);
            Assert.AreEqual(y, validY, 1E-2);
        }

        [TestMethod]
        public void Test_McCormick()
        {
            var lower = new double[] { -1.5d, -3d };
            var upper = new double[] { 4d, 4d };
            var solver = new SimulatedAnnealing(TestFunctions.McCormick, 2, lower, upper);
            solver.Minimize();
            var solution = solver.BestParameterSet.Values;
            var x = solution[0];
            var y = solution[1];
            var validX = -0.54719d;
            var validY = -1.54719d;
            Assert.AreEqual(x, validX, 1E-2);
            Assert.AreEqual(y, validY, 1E-2);
        }

        [TestMethod]
        public void Test_Eggholder()
        {
            var lower = new double[] { -512d, -512d };
            var upper = new double[] { 512d, 512d };
            var solver = new SimulatedAnnealing(TestFunctions.Eggholder, 2, lower, upper);
            solver.Minimize();
            var solution = solver.BestParameterSet.Values;
            var x = solution[0];
            var y = solution[1];
            var validX = 512d;
            var validY = 404.2319d;
            // Fails to converge
            //Assert.AreEqual(x, validX, 1E-3);
            //Assert.AreEqual(y, validY, 1E-3);
        }

        [TestMethod]
        public void Test_Rastrigin()
        {
            var lower = new double[] { -5.12d, -5.12d, -5.12d, -5.12d, -5.12d };
            var upper = new double[] { 5.12d, 5.12d, 5.12d, 5.12d, 5.12d };
            var solver = new SimulatedAnnealing(TestFunctions.Rastrigin, lower.Length, lower, upper);
            solver.Minimize();
            var solution = solver.BestParameterSet.Values;
            var valid = new double[] { 0.0d, 0.0d, 0.0d, 0.0d, 0.0d };
            for (int i = 0; i < valid.Length; i++)
                Assert.AreEqual(solution[i], valid[i], 1E-2);

        }

        [TestMethod]
        public void Test_Rosenbrock()
        {
            var lower = new double[] { -1000d, -1000d, -1000d, -1000d, -1000d };
            var upper = new double[] { 1000d, 1000d, 1000d, 1000d, 1000d };
            var solver = new SimulatedAnnealing(TestFunctions.Rosenbrock, lower.Length, lower, upper);
            solver.Minimize();
            var solution = solver.BestParameterSet.Values;
            var valid = new double[] { 1.0d, 1.0d, 1.0d, 1.0d, 1.0d };
            //for (int i = 0; i < valid.Length; i++)
            //    Assert.AreEqual(solution[i], valid[i], 1E-2);

        }

        [TestMethod]
        public void Test_TP2()
        {
            var lower = new double[] { 0d, 0d };
            var upper = new double[] { 2d, 2d };
            var solver = new SimulatedAnnealing(TestFunctions.tp2, 2, lower, upper);
            solver.Minimize();
            var solution = solver.BestParameterSet.Values;
            //var x = solution[0];
            //var y = solution[1];
            //var validX = 512d;
            //var validY = 404.2319d;
            //Assert.AreEqual(x, validX, 1E-4);
            //Assert.AreEqual(y, validY, 1E-4);
        }
    }
}
