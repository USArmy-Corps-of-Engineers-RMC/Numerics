using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Mathematics.LinearAlgebra;

namespace Mathematics.LinearAlgebra
{
    [TestClass]
    public class Test_GaussJordanElimination
    {
        [TestMethod()]
        public void Test_GaussJordanElim()
        {
            var _matrix = new double[,] { { 1d, 3d, 3d }, { 1d, 4d, 3d }, { 1d, 3d, 4d } };
            var true_IA = new double[,] { { 7d, -3, -3 }, { -1, 1d, 0d }, { -1, 0d, 1d } };
            var A = new Matrix(_matrix);
            Matrix argB = null;
            GaussJordanElimination.Solve(ref A, B: ref argB);
            for (int i = 0; i < A.NumberOfRows; i++)
            {
                for (int j = 0; j < A.NumberOfColumns - 1; j++)
                    Assert.AreEqual(A[i, j] == true_IA[i, j], true);
            }
        }
    }
}
