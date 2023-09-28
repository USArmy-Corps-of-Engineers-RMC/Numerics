using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Mathematics.LinearAlgebra;

namespace Mathematics.LinearAlgebra
{
    [TestClass]
    public class Test_CholeskyDecomp
    {
        /// <summary>
        /// Test Cholesky Decomposition methods.
        /// </summary>
        [TestMethod()]
        public void Test_CholeskyDecompMethods()
        {
            var A = new Matrix(3);
            A[0, 0] = 25d;
            A[0, 1] = 15d;
            A[0, 2] = -5;
            A[1, 0] = 15d;
            A[1, 1] = 18d;
            A[1, 2] = 0d;
            A[2, 0] = -5;
            A[2, 1] = 0d;
            A[2, 2] = 11d;
            var chol = new CholeskyDecomposition(A);
            var lu = new LUDecomposition(A);

            // Test Determinant
            double true_det = Math.Log(lu.Determinant());
            double det = chol.LogDeterminant();
            //double true_det = lu.Determinant();
            //double det = chol.Determinant();
            Assert.AreEqual(det, true_det, 0.0001d);

            // Test Inverse
            var true_invA = lu.InverseA();
            var invA = chol.InverseA();
            for (int i = 0; i < invA.NumberOfRows; i++)
            {
                for (int j = 0; j < invA.NumberOfColumns; j++)
                    Assert.AreEqual(invA[i, j], true_invA[i, j], 0.0001d);
            }

            // Test Solve x given B
            var B = new Vector(new[]{ 6d, -4, 27d });
            var true_x = lu.Solve(B);
            var x = chol.Solve(B);
            for (int i = 0; i < x.Length; i++)
                Assert.AreEqual(x[i], true_x[i], 0.0001d);
        }
    }
}
