using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics;
using Numerics.Mathematics.Optimization;

namespace Mathematics.Optimization
{
    [TestClass]
    public class Test_Optimization
    {

        //#region Test Functions

        ///// <summary>
        ///// Test one-dimensional function
        ///// </summary>
        //public double FX(double x)
        //{
        //    double F = (x + 3d) * Math.Pow(x - 1d, 2d);
        //    return F;
        //}

        ///// <summary>
        ///// The Ackley Function.
        ///// https://en.wikipedia.org/wiki/Test_functions_for_optimization
        ///// f(0, 0) = 0
        ///// </summary>
        //public double Ackley(double[] parms)
        //{
        //    var x = parms[0];
        //    var y = parms[1];
        //    double F = -20 * Math.Exp(-0.2 * Math.Sqrt(0.5 * (x * x + y * y))) - Math.Exp(0.5 * (Math.Cos(2 * Math.PI * x) + Math.Cos(2 * Math.PI * y))) + Math.E + 20;
        //    return F;
        //}

        ///// <summary>
        ///// The Beale Function.
        ///// https://en.wikipedia.org/wiki/Test_functions_for_optimization
        ///// f(3, 0.5) = 0
        ///// </summary>
        //public double Beale(double[] parms)
        //{
        //    var x = parms[0];
        //    var y = parms[1];
        //    double F = Math.Pow(1.5 - x + x * y, 2) + Math.Pow(2.25 - x + x * y * y, 2) + Math.Pow(2.625 - x + x * y * y * y, 2);
        //    return F;
        //}

        ///// <summary>
        ///// The Goldstein-Price Function.
        ///// https://en.wikipedia.org/wiki/Test_functions_for_optimization
        ///// f(0, -1) = 3
        ///// </summary>
        //public double GoldsteinPrice(double[] parms)
        //{
        //    var x = parms[0];
        //    var y = parms[1];
        //    double F = (1 + Math.Pow(x + y + 1, 2) * (19 - 14 * x + 3 * x * x - 14 * y + 6 * x * y + 3 * y * y)) * 
        //               (30 + Math.Pow(2 * x - 3 * y, 2) * (18 - 32 * x + 12 * x * x + 48 * y - 36 * x * y + 27 * y * y));
        //    return F;
        //}

        ///// <summary>
        ///// The Booth Function.
        ///// https://en.wikipedia.org/wiki/Test_functions_for_optimization
        ///// f(1, 3) = 0
        ///// </summary>
        //public double Booth(double[] parms)
        //{
        //    var x = parms[0];
        //    var y = parms[1];
        //    double F = Math.Pow(x + 2 * y - 7, 2) + Math.Pow(2 * x + y - 5, 2);
        //    return F;
        //}

        ///// <summary>
        ///// The Matyas Function.
        ///// https://en.wikipedia.org/wiki/Test_functions_for_optimization
        ///// f(0, 0) = 0
        ///// </summary>
        //public double Matyas(double[] parms)
        //{
        //    var x = parms[0];
        //    var y = parms[1];
        //    double F = 0.26 * (x * x + y * y) - 0.48 * x * y;
        //    return F;
        //}

        ///// <summary>
        ///// The Eggholder Function.
        ///// https://en.wikipedia.org/wiki/Test_functions_for_optimization
        ///// f(512, 404.2319) = -959.6407
        ///// </summary>
        //public double Eggholder(double[] parms)
        //{
        //    var x = parms[0];
        //    var y = parms[1];
        //    double F = -(y + 47) * Math.Sin(Math.Sqrt(Math.Abs((x / 2) + (y + 47)))) - x * Math.Sin(Math.Sqrt(Math.Abs(x - (y + 47))));
        //    return F;
        //}

        ///// <summary>
        ///// The McCormick Function.
        ///// https://en.wikipedia.org/wiki/Test_functions_for_optimization
        ///// f(-0.54719, -1.54719) = -1.9133
        ///// </summary>
        //public double McCormick(double[] parms)
        //{
        //    var x = parms[0];
        //    var y = parms[1];
        //    double F = Math.Sin(x + y) + Math.Pow(x - y, 2) - 1.5 * x + 2.5 * y + 1.0;
        //    return F;
        //}

        ///// <summary>
        ///// Test multidimensional function.
        ///// </summary>
        //public double FXYZ(double[] parms)
        //{
        //    double x = parms[0];
        //    double y = parms[1];
        //    double z = parms[2];
        //    double F = Math.Pow(4d * x - 0.5d, 2d) + Math.Pow(3d * y - 0.6d, 2d) + Math.Pow(2d * z - 0.7d, 2d);
        //    return F;
        //}

        ///// <summary>
        ///// The Rastrigin Function.
        ///// https://en.wikipedia.org/wiki/Test_functions_for_optimization
        ///// f(0..0) = 0
        ///// </summary>
        //public double Rastrigin(double[] parms)
        //{
        //    double A = 10;
        //    int n = parms.Length;
        //    double F = A * n;
        //    for (int i = 0; i < n; i++)
        //       F += parms[i] * parms[i] - A * Math.Cos(2 * Math.PI * parms[i]);
        //    return F;
        //}

        ///// <summary>
        ///// The Rosenbrock Function.
        ///// https://en.wikipedia.org/wiki/Test_functions_for_optimization
        ///// f(1..1) = 0
        ///// </summary>
        //public double Rosenbrock(double[] parms)
        //{
        //    int n = parms.Length;
        //    double F = 0;
        //    for (int i = 0; i < n - 1; i++)
        //        F += 100 * Math.Pow(parms[i + 1] - parms[i] * parms[i], 2) + Math.Pow(1 - parms[i], 2);
        //    return F;
        //}

        ///// <summary>
        ///// tp2 function - Multiple local optima and 2 global minimizers, by virtue of symmetry
        ///// f(2/3, 1) = f(1, 2/3) = 0
        ///// </summary>
        //public double tp2(double[] parms)
        //{
        //    var p1 = parms[0];
        //    var p2 = parms[1];
        //    int i;
        //    double x, F=0;

        //    for (i = 0; i <= 86; i++)
        //    {
        //        x = 3.1 + 0.15 * i;
        //        F = F+ Math.Pow(((Math.Sin(p1 * x) + Math.Sin(p2 * x))- (Math.Sin((2.0/3.0) * x) + Math.Sin(1 * x))),2);
        //    }
        //    return F;
        //}


        //#endregion

        ///// <summary>
        ///// Nelder-Mead downhill simplex, sometimes referred to as Amoeba.
        ///// </summary>
        //[TestMethod()]
        //public void Test_NelderMead()
        //{
        //    var initial = new double[] { 0.5d, 0.5d, 0.5d };
        //    var lower = new double[] { 0d, 0d, 0d };
        //    var upper = new double[] { 1d, 1d, 1d };
        //    var solution = NelderMead.Minimize(FXYZ, initial, lower, upper);
        //    double x = solution[0];
        //    double y = solution[1];
        //    double z = solution[2];
        //    double validX = 0.125d;
        //    double validY = 0.2d;
        //    double validZ = 0.35d;
        //    Assert.AreEqual(x, validX, 1E-4);
        //    Assert.AreEqual(y, validY, 1E-4);
        //    Assert.AreEqual(z, validZ, 1E-4);
        //}

        ///// <summary>
        ///// Test Shuffled Complex Evolution 
        ///// </summary>
        //[TestMethod()]
        //public void Test_SCE()
        //{
        //    // Test 3 parameter function
        //    var lower = new double[] { 0d, 0d, 0d };
        //    var upper = new double[] { 1d, 1d, 1d };
        //    var sce = new ShuffledComplexEvolution(FXYZ, lower.Length, lower, upper);
        //    sce.Minimize();
        //    var solution = sce.BestParameterSet.Values;
        //    double x = solution[0];
        //    double y = solution[1];
        //    double z = solution[2];
        //    double validX = 0.125d;
        //    double validY = 0.2d;
        //    double validZ = 0.35d;
        //    Assert.AreEqual(x, validX, 1E-4);
        //    Assert.AreEqual(y, validY, 1E-4);
        //    Assert.AreEqual(z, validZ, 1E-4);

        //    // Test Ackley function
        //    lower = new double[] { -5d, -5d };
        //    upper = new double[] { 5d, 5d };
        //    sce = new ShuffledComplexEvolution(Ackley, lower.Length, lower, upper);
        //    sce.Minimize();
        //    solution = sce.BestParameterSet.Values;
        //    x = solution[0];
        //    y = solution[1];
        //    validX = 0.0d;
        //    validY = 0.0d;
        //    Assert.AreEqual(x, validX, 1E-4);
        //    Assert.AreEqual(y, validY, 1E-4);

        //    // Test Beale function
        //    lower = new double[] { -4.5d, -4.5d };
        //    upper = new double[] { 4.5d, 4.5d };
        //    sce = new ShuffledComplexEvolution(Beale, lower.Length, lower, upper);
        //    sce.Minimize();
        //    solution = sce.BestParameterSet.Values;
        //    x = solution[0];
        //    y = solution[1];
        //    validX = 3.0d;
        //    validY = 0.5d;
        //    Assert.AreEqual(x, validX, 1E-4);
        //    Assert.AreEqual(y, validY, 1E-4);

        //    // Test Goldstein-Price function
        //    lower = new double[] { -2d, -2d };
        //    upper = new double[] { 2d, 2d };
        //    sce = new ShuffledComplexEvolution(GoldsteinPrice, lower.Length, lower, upper);
        //    sce.Minimize();
        //    solution = sce.BestParameterSet.Values;
        //    x = solution[0];
        //    y = solution[1];
        //    validX = 0.0d;
        //    validY = -1.0d;
        //    Assert.AreEqual(x, validX, 1E-4);
        //    Assert.AreEqual(y, validY, 1E-4);

        //    // Test Booth function
        //    lower = new double[] { -10d, -10d };
        //    upper = new double[] { 10d, 10d };
        //    sce = new ShuffledComplexEvolution(Booth, lower.Length, lower, upper);
        //    sce.Minimize();
        //    solution = sce.BestParameterSet.Values;
        //    x = solution[0];
        //    y = solution[1];
        //    validX = 1.0d;
        //    validY = 3.0d;
        //    Assert.AreEqual(x, validX, 1E-4);
        //    Assert.AreEqual(y, validY, 1E-4);

        //    // Test Matyas function
        //    lower = new double[] { -10d, -10d };
        //    upper = new double[] { 10d, 10d };
        //    sce = new ShuffledComplexEvolution(Matyas, lower.Length, lower, upper);
        //    sce.Minimize();
        //    solution = sce.BestParameterSet.Values;
        //    x = solution[0];
        //    y = solution[1];
        //    validX = 0.0d;
        //    validY = 0.0d;
        //    Assert.AreEqual(x, validX, 1E-4);
        //    Assert.AreEqual(y, validY, 1E-4);

        //    // Test Eggholder function
        //    lower = new double[] { -512d, -512d };
        //    upper = new double[] { 512d, 512d };
        //    sce = new ShuffledComplexEvolution(Eggholder, lower.Length, lower, upper) { Complexes = 20, CCEIterations = 20 };
        //    sce.Minimize();
        //    solution = sce.BestParameterSet.Values;
        //    x = solution[0];
        //    y = solution[1];
        //    validX = 512d;
        //    validY = 404.2319d;
        //    Assert.AreEqual(x, validX, 1E-4);
        //    Assert.AreEqual(y, validY, 1E-4);

        //    //for (int i = 0; i < sce.ParameterSetTrace.Count; i++)
        //    //{
        //    //    Debug.Print(sce.ParameterSetTrace[i].Fitness.ToString());
        //    //}

        //    // Test McCormick function
        //    lower = new double[] { -1.5d, -3d };
        //    upper = new double[] { 4d, 4d };
        //    sce = new ShuffledComplexEvolution(McCormick, lower.Length, lower, upper);
        //    sce.Minimize();
        //    solution = sce.BestParameterSet.Values;
        //    x = solution[0];
        //    y = solution[1];
        //    validX = -0.54719d;
        //    validY = -1.54719d;
        //    Assert.AreEqual(x, validX, 1E-4);
        //    Assert.AreEqual(y, validY, 1E-4);

        //    // Test Rastrigin function
        //    lower = new double[] { -5.12d, -5.12d, -5.12d, -5.12d, -5.12d };
        //    upper = new double[] { 5.12d, 5.12d, 5.12d, 5.12d, 5.12d };
        //    sce = new ShuffledComplexEvolution(Rastrigin, lower.Length, lower, upper) { Complexes = 20 };
        //    sce.Minimize();
        //    solution = sce.BestParameterSet.Values;
        //    var ans = new double[] { 0.0d, 0.0d, 0.0d, 0.0d, 0.0d };
        //    for (int i = 0; i < ans.Length; i++)
        //         Assert.AreEqual(solution[i], ans[i], 1E-4);

        //    // Test Rosenbrock function
        //    lower = new double[] { -1000d, -1000d, -1000d, -1000d, -1000d };
        //    upper = new double[] { 1000d, 1000d, 1000d, 1000d, 1000d };
        //    sce = new ShuffledComplexEvolution(Rosenbrock, lower.Length, lower, upper) { Complexes = 20 };
        //    sce.Minimize();
        //    solution = sce.BestParameterSet.Values;
        //    ans = new double[] { 1.0d, 1.0d, 1.0d, 1.0d, 1.0d };
        //    for (int i = 0; i < ans.Length; i++)
        //        Assert.AreEqual(solution[i], ans[i], 1E-4);

        //    // Test tp2 function
        //    // Compare sorted OptionalOut fitnessLevels since there are 2 points that globally optimize tp2
        //    lower = new double[] { 0d, 0d };
        //    upper = new double[] { 2d, 2d };
        //    sce = new ShuffledComplexEvolution(tp2, lower.Length, lower, upper);
        //    sce.Minimize();
        //    solution = sce.BestParameterSet.Values;

        //}

        ///// <summary>
        ///// Test Differential Evolution
        ///// </summary>
        //[TestMethod()]
        //public void Test_DE()
        //{
        //    // Test 3 parameter function
        //    var lower = new double[] { 0d, 0d, 0d };
        //    var upper = new double[] { 1d, 1d, 1d };
        //    var DE = new DifferentialEvolution(FXYZ, lower.Length, lower, upper);
        //    DE.Minimize();
        //    var solution = DE.BestParameterSet.Values;
        //    double x = solution[0];
        //    double y = solution[1];
        //    double z = solution[2];
        //    double validX = 0.125d;
        //    double validY = 0.2d;
        //    double validZ = 0.35d;
        //    Assert.AreEqual(x, validX, 1E-4);
        //    Assert.AreEqual(y, validY, 1E-4);
        //    Assert.AreEqual(z, validZ, 1E-4);

        //    // Test Ackley function
        //    lower = new double[] { -5d, -5d };
        //    upper = new double[] { 5d, 5d };
        //    DE = new DifferentialEvolution(Ackley, lower.Length, lower, upper);
        //    DE.Minimize();
        //    solution = DE.BestParameterSet.Values;
        //    x = solution[0];
        //    y = solution[1];
        //    validX = 0.0d;
        //    validY = 0.0d;
        //    Assert.AreEqual(x, validX, 1E-4);
        //    Assert.AreEqual(y, validY, 1E-4);

        //    //for (int i = 0; i < DE.ParameterSetTrace.Count; i++)
        //    //{
        //    //    Debug.Print(DE.ParameterSetTrace[i].Fitness.ToString());
        //    //}

        //    // Test Beale function
        //    lower = new double[] { -4.5d, -4.5d };
        //    upper = new double[] { 4.5d, 4.5d };
        //    DE = new DifferentialEvolution(Beale, lower.Length, lower, upper);
        //    DE.Minimize();
        //    solution = DE.BestParameterSet.Values;
        //    x = solution[0];
        //    y = solution[1];
        //    validX = 3.0d;
        //    validY = 0.5d;
        //    Assert.AreEqual(x, validX, 1E-4);
        //    Assert.AreEqual(y, validY, 1E-4);

        //    // Test Goldstein-Price function
        //    lower = new double[] { -2d, -2d };
        //    upper = new double[] { 2d, 2d };
        //    DE = new DifferentialEvolution(GoldsteinPrice, lower.Length, lower, upper);
        //    DE.Minimize();
        //    solution = DE.BestParameterSet.Values;
        //    x = solution[0];
        //    y = solution[1];
        //    validX = 0.0d;
        //    validY = -1.0d;
        //    Assert.AreEqual(x, validX, 1E-4);
        //    Assert.AreEqual(y, validY, 1E-4);

        //    // Test Booth function
        //    lower = new double[] { -10d, -10d };
        //    upper = new double[] { 10d, 10d };
        //    DE = new DifferentialEvolution(Booth, lower.Length, lower, upper);
        //    DE.Minimize();
        //    solution = DE.BestParameterSet.Values;
        //    x = solution[0];
        //    y = solution[1];
        //    validX = 1.0d;
        //    validY = 3.0d;
        //    Assert.AreEqual(x, validX, 1E-4);
        //    Assert.AreEqual(y, validY, 1E-4);

        //    // Test Matyas function
        //    lower = new double[] { -10d, -10d };
        //    upper = new double[] { 10d, 10d };
        //    DE = new DifferentialEvolution(Matyas, lower.Length, lower, upper);
        //    DE.Minimize();
        //    solution = DE.BestParameterSet.Values;
        //    x = solution[0];
        //    y = solution[1];
        //    validX = 0.0d;
        //    validY = 0.0d;
        //    Assert.AreEqual(x, validX, 1E-4);
        //    Assert.AreEqual(y, validY, 1E-4);

        //    // This solves but doesn't have as good of precision as other test functions
        //    // Test Eggholder function
        //    lower = new double[] { -512d, -512d };
        //    upper = new double[] { 512d, 512d };
        //    DE = new DifferentialEvolution(Eggholder, lower.Length, lower, upper);
        //    DE.Minimize();
        //    solution = DE.BestParameterSet.Values;
        //    x = solution[0];
        //    y = solution[1];
        //    validX = 512d;
        //    validY = 404.2319d;
        //    Assert.AreEqual(x, validX, 1E-3);
        //    Assert.AreEqual(y, validY, 1E-3);

        //    // Test McCormick function
        //    lower = new double[] { -1.5d, -3d };
        //    upper = new double[] { 4d, 4d };
        //    DE = new DifferentialEvolution(McCormick, lower.Length, lower, upper);
        //    DE.Minimize();
        //    solution = DE.BestParameterSet.Values;
        //    x = solution[0];
        //    y = solution[1];
        //    validX = -0.54719d;
        //    validY = -1.54719d;
        //    Assert.AreEqual(x, validX, 1E-4);
        //    Assert.AreEqual(y, validY, 1E-4);

        //    // Test Rastrigin function
        //    lower = new double[] { -5.12d, -5.12d, -5.12d, -5.12d, -5.12d };
        //    upper = new double[] { 5.12d, 5.12d, 5.12d, 5.12d, 5.12d };
        //    DE = new DifferentialEvolution(Rastrigin, lower.Length, lower, upper);
        //    DE.Minimize();
        //    solution = DE.BestParameterSet.Values;
        //    var ans = new double[] { 0.0d, 0.0d, 0.0d, 0.0d, 0.0d };
        //    for (int i = 0; i < ans.Length; i++)
        //        Assert.AreEqual(solution[i], ans[i], 1E-4);

        //    // Test Rosenbrock function
        //    lower = new double[] { -1000d, -1000d, -1000d, -1000d, -1000d };
        //    upper = new double[] { 1000d, 1000d, 1000d, 1000d, 1000d };
        //    DE = new DifferentialEvolution(Rosenbrock, lower.Length, lower, upper);
        //    DE.Minimize();
        //    solution = DE.BestParameterSet.Values;
        //    ans = new double[] { 1.0d, 1.0d, 1.0d, 1.0d, 1.0d };
        //    for (int i = 0; i < ans.Length; i++)
        //        Assert.AreEqual(solution[i], ans[i], 1E-4);

        //    // Test tp2 function
        //    // Compare sorted OptionalOut fitnessLevels since there are 2 points that globally optimize tp2
        //    lower = new double[] { 0d, 0d };
        //    upper = new double[] { 2d, 2d };
        //    DE = new DifferentialEvolution(tp2, lower.Length, lower, upper);
        //    DE.Minimize();
        //    solution = DE.BestParameterSet.Values;
        //    Assert.AreEqual(DE.BestParameterSet.Fitness, 0.0, 1E-8);
        //}

        ///// <summary>
        ///// Test Random Start Nelder-Mead
        ///// </summary>
        //[TestMethod()]
        //public void Test_RandomNelderMead()
        //{
        //    // Test 3 parameter function
        //    var lower = new double[] { 0d, 0d, 0d };
        //    var upper = new double[] { 1d, 1d, 1d };
        //    var solution = RandomNelderMead.Minimize(FXYZ, lower, upper);
        //    double x = solution[0];
        //    double y = solution[1];
        //    double z = solution[2];
        //    double validX = 0.125d;
        //    double validY = 0.2d;
        //    double validZ = 0.35d;
        //    Assert.AreEqual(x, validX, 1E-4);
        //    Assert.AreEqual(y, validY, 1E-4);
        //    Assert.AreEqual(z, validZ, 1E-4);

        //    // Test Ackley function
        //    lower = new double[] { -5d, -5d };
        //    upper = new double[] { 5d, 5d };
        //    solution = RandomNelderMead.Minimize(Ackley, lower, upper);
        //    x = solution[0];
        //    y = solution[1];
        //    validX = 0.0d;
        //    validY = 0.0d;
        //    Assert.AreEqual(x, validX, 1E-4);
        //    Assert.AreEqual(y, validY, 1E-4);

        //    // Test Beale function
        //    lower = new double[] { -4.5d, -4.5d };
        //    upper = new double[] { 4.5d, 4.5d };
        //    solution = RandomNelderMead.Minimize(Beale, lower, upper);
        //    x = solution[0];
        //    y = solution[1];
        //    validX = 3.0d;
        //    validY = 0.5d;
        //    Assert.AreEqual(x, validX, 1E-4);
        //    Assert.AreEqual(y, validY, 1E-4);

        //    // Test Goldstein-Price function
        //    lower = new double[] { -2d, -2d };
        //    upper = new double[] { 2d, 2d };
        //    solution = RandomNelderMead.Minimize(GoldsteinPrice, lower, upper);
        //    x = solution[0];
        //    y = solution[1];
        //    validX = 0.0d;
        //    validY = -1.0d;
        //    Assert.AreEqual(x, validX, 1E-4);
        //    Assert.AreEqual(y, validY, 1E-4);

        //    // Test Booth function
        //    lower = new double[] { -10d, -10d };
        //    upper = new double[] { 10d, 10d };
        //    solution = RandomNelderMead.Minimize(Booth, lower, upper);
        //    x = solution[0];
        //    y = solution[1];
        //    validX = 1.0d;
        //    validY = 3.0d;
        //    Assert.AreEqual(x, validX, 1E-4);
        //    Assert.AreEqual(y, validY, 1E-4);

        //    // Fails to solve !!!!!!!!!!

        //    // Test Matyas function
        //    //lower = new double[] { -10d, -10d };
        //    //upper = new double[] { 10d, 10d };
        //    //solution = RandomNelderMead.Minimize(Matyas, lower, upper);
        //    //x = solution[0];
        //    //y = solution[1];
        //    //validX = 0.0d;
        //    //validY = 0.0d;
        //    //Assert.AreEqual(x, validX, 1E-4);
        //    //Assert.AreEqual(y, validY, 1E-4);

        //    // Test Eggholder function
        //    //lower = new double[] { -512d, -512d };
        //    //upper = new double[] { 512d, 512d };
        //    //solution = RandomNelderMead.Minimize(Eggholder, lower, upper);
        //    //x = solution[0];
        //    //y = solution[1];
        //    //validX = 512d;
        //    //validY = 404.2319d;
        //    //Assert.AreEqual(x, validX, 1E-4);
        //    //Assert.AreEqual(y, validY, 1E-4);

        //    // Test McCormick function
        //    lower = new double[] { -1.5d, -3d };
        //    upper = new double[] { 4d, 4d };
        //    solution = RandomNelderMead.Minimize(McCormick, lower, upper);
        //    x = solution[0];
        //    y = solution[1];
        //    validX = -0.54719d;
        //    validY = -1.54719d;
        //    Assert.AreEqual(x, validX, 1E-4);
        //    Assert.AreEqual(y, validY, 1E-4);

        //    // These two fail to solve within a reasonable number of evaluations. !!!!!!!!!!!!!

        //    // Test Rastrigin function
        //    //lower = new double[] { -5.12d, -5.12d, -5.12d, -5.12d, -5.12d };
        //    //upper = new double[] { 5.12d, 5.12d, 5.12d, 5.12d, 5.12d };
        //    //solution = RandomNelderMead.Minimize(Rastrigin, lower, upper, population: 1000);
        //    //var ans = new double[] { 0.0d, 0.0d, 0.0d, 0.0d, 0.0d };
        //    //for (int i = 0; i < ans.Length; i++)
        //    //    Assert.AreEqual(solution[i], ans[i], 1E-4);

        //    // Test Rosenbrock function
        //    //lower = new double[] { -1000d, -1000d, -1000d, -1000d, -1000d };
        //    //upper = new double[] { 1000d, 1000d, 1000d, 1000d, 1000d };
        //    //solution = RandomNelderMead.Minimize(Rosenbrock, lower, upper, population: 1000);
        //    //var ans = new double[] { 1.0d, 1.0d, 1.0d, 1.0d, 1.0d };
        //    //for (int i = 0; i < ans.Length; i++)
        //    //    Assert.AreEqual(solution[i], ans[i], 1E-4);

        //}

        ///// <summary>
        ///// Test Multistart
        ///// </summary>
        //[TestMethod()]
        //public void Test_Multistart()
        //{
        //    int i;
        //    double minf = double.PositiveInfinity;
        //    // Test tp2 function
        //    var lower = new double[] { 0d, 0d };
        //    var upper = new double[] { 2d, 2d };

        //    var iters = new OptionalOut<int>() { Result = 0 };
        //    var fitLevels = new OptionalOut<List<List<double>>>();

        //    var solution = MultiStart.Minimize(tp2, lower, upper, iterations: iters, fitnessLevels: fitLevels);

        //    // I (Brian Skahill) don't assume the OptionalOut fitnessLevels from Multistart are sorted 
        //    for (i = 0; i < fitLevels.Result.Count; i++)
        //        if ( fitLevels.Result[i][0] < minf ) minf = fitLevels.Result[i][0];
        //    Assert.AreEqual(minf, 0.0, 1E-8);
        //}
    }

}
