using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Mathematics.LinearAlgebra;

namespace Mathematics.LinearAlgebra
{
    /// <summary>
    /// Test LU Decomposition.
    /// </summary>
    [TestClass]
    public class Test_LUDecomp
    {
        /// <summary>
        /// Test LU Decomposition methods.
        /// </summary>
        [TestMethod()]
        public void Test_LUDecompMethods()
        {
            var A = new Matrix(3);
            A[0, 0] = 1d;
            A[0, 1] = 1d;
            A[0, 2] = 1d;
            A[1, 0] = 0d;
            A[1, 1] = 2d;
            A[1, 2] = 5d;
            A[2, 0] = 2d;
            A[2, 1] = 5d;
            A[2, 2] = -1;

            var B = new Vector(new[] { 6d, -4, 27d });
            var lu = new LUDecomposition(A);
            double true_det = -21;
            double det = lu.Determinant();

            Assert.AreEqual(det, true_det, 0.0001d);
            var true_x = new[] { 5d, 3d, -2 };
            var x = lu.Solve(B);
            for (int i = 0; i < x.Length; i++)
                Assert.AreEqual(x[i], true_x[i], 0.0001d);
            var true_invA = new Matrix(3);
            true_invA[0, 0] = -27d / -21d;
            true_invA[0, 1] = 6d / -21d;
            true_invA[0, 2] = 3d / -21d;
            true_invA[1, 0] = 10d/ -21d;
            true_invA[1, 1] = -3d / -21d;
            true_invA[1, 2] = -5d / -21d;
            true_invA[2, 0] = -4d / -21d;
            true_invA[2, 1] = -3d / -21d;
            true_invA[2, 2] = 2d / -21d;
            var invA = lu.InverseA();
            for (int i = 0; i < invA.NumberOfRows; i++)
            {
                for (int j = 0; j < invA.NumberOfColumns; j++)
                    Assert.AreEqual(invA[i, j], true_invA[i, j], 0.0001d);
            }
        }
    }
}
