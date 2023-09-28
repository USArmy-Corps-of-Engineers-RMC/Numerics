using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Mathematics.LinearAlgebra;

namespace Mathematics.LinearAlgebra
{
    [TestClass]
    public class Test_SingularValueDecomp
    {
        /// <summary>
        /// Test Singular Value Decomposition methods.
        /// </summary>
        [TestMethod()]
        public void Test_SingularValueDecompMethods()
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
            var lu = new LUDecomposition(A);
            var svd = new SingularValueDecomposition(A);

            // Test Solve x given B
            var B = new Vector(new[] { 6d, -4, 27d });
            var true_x = lu.Solve(B);
            var x = svd.Solve(B);
            for (int i = 0; i < x.Length; i++)
                Assert.AreEqual(x[i], true_x[i], 0.0001d);
        }
    }
}
