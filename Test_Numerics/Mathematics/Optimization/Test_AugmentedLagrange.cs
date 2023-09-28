using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Numerics.Mathematics.Optimization;
using Numerics;

namespace Mathematics.Optimization
{
    [TestClass]
    public class Test_AugmentedLagrange
    {
        /// <summary>
        /// Solution is from Water Economics graduate course
        /// </summary>
        [TestMethod]
        public void Test_1()
        {
            var constraint = new Constraint((x) => { return Tools.Sum(x); }, 3, 22, ConstraintType.EqualTo);
            Func<double[], double> func = (double[] x) =>
            {
                var NB = new double[3];
                for (int i = 0; i < 3; i++)
                {
                    NB[i] = (20 * x[i] - x[i] * x[i] - 24) / Math.Pow(1.10, i);
                }
                return -Tools.Sum(NB);
            };
            var innerSolver = new BFGS(func, 3, new double[] { 7, 7, 8 }, new double[] { double.MinValue, double.MinValue, double.MinValue }, new double[] { double.MaxValue, double.MaxValue, double.MaxValue });
            var solver = new AugmentedLagrange(func, innerSolver,  new IConstraint[] { constraint });
            solver.Minimize();

            // Point
            Assert.AreEqual(7.583082, solver.BestParameterSet.Values[0], 1E-3);
            Assert.AreEqual(7.341390, solver.BestParameterSet.Values[1], 1E-3);
            Assert.AreEqual(7.075529, solver.BestParameterSet.Values[2], 1E-3);
            // Function
            Assert.AreEqual(188.5655, -solver.BestParameterSet.Fitness, 1E-3);
            // Multiplier
            Assert.AreEqual(4.833835, solver.Lambda[0], 1E-3);
        }

        /// <summary>
        /// Solution is from Water Economics graduate course
        /// </summary>
        [TestMethod]
        public void Test_2()
        {
            var constraint = new Constraint((x) => { return Tools.Sum(x); }, 2, 100, ConstraintType.EqualTo);
            Func<double[], double> func = (double[] x) =>
            {
                var NB = new double[2];
                NB[0] = 60 * x[0] - 0.5 * x[0] * x[0];
                NB[1] = (64 * x[1] - 0.5 * x[1] * x[1]) / 1.5;
                return -Tools.Sum(NB);
            };
            var innerSolver = new BFGS(func, 2, new double[] { 50, 50 }, new double[] { double.MinValue, double.MinValue }, new double[] { double.MaxValue, double.MaxValue });
            var solver = new AugmentedLagrange(func, innerSolver, new IConstraint[] { constraint });
            solver.Minimize();

            // Point
            Assert.AreEqual(50.4, solver.BestParameterSet.Values[0], 1E-3);
            Assert.AreEqual(49.6, solver.BestParameterSet.Values[1], 1E-3);
            // Function
            Assert.AreEqual(3050.133, -solver.BestParameterSet.Fitness, 1E-3);
            // Multiplier
            Assert.AreEqual(9.6, solver.Lambda[0], 1E-3);
        }

        /// <summary>
        /// Example problem 5.2 from "Risk Modeling, Assessment, and Management"
        /// </summary>
        [TestMethod]
        public void Test_Haimes_5_2()
        {        
            Func<double[], double> primaryFunc = (double[] x) =>
            {
                return Math.Pow(x[0] - 2, 2) + Math.Pow(x[1] - 4, 2) + 5;
            };
            Func<double[], double> secondaryFunc = (double[] x) =>
            {
                return Math.Pow(x[0] - 6, 2) + Math.Pow(x[1] - 10, 2) + 6;
            };
            var constraint = new Constraint(secondaryFunc, 2, 13.31, ConstraintType.LesserThanOrEqualTo);
            var innerSolver = new BFGS(primaryFunc, 2, new double[] { 5, 5 }, new double[] { double.MinValue, double.MinValue }, new double[] { double.MaxValue, double.MaxValue });
            var solver = new AugmentedLagrange(primaryFunc, innerSolver, new IConstraint[] { constraint });
            solver.Minimize();

            // Point
            Assert.AreEqual(4.5, solver.BestParameterSet.Values[0], 1E-2);
            Assert.AreEqual(7.75, solver.BestParameterSet.Values[1], 1E-2);
            // Function
            Assert.AreEqual(25.31, solver.BestParameterSet.Fitness, 1E-2);
            // Multiplier
            Assert.AreEqual(1.67, solver.Mu[0], 1E-2);

        }

    }
}
