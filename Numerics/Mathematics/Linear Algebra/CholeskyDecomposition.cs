using System;

namespace Numerics.Mathematics.LinearAlgebra
{

    /// <summary>
    /// A class for solving a set of linear equations using Cholesky Decomposition.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    /// <see href = "https://en.wikipedia.org/wiki/Cholesky_decomposition" />
    /// "Numerical Recipes: The art of Scientific Computing, Third Edition. Press et al. 2017.
    /// </para>
    /// </remarks>
    public class CholeskyDecomposition
    {
     
        /// <summary>
        /// Constructs new Cholesky Decomposition.
        /// </summary>
        /// <param name="A">The positive-definite symmetric input matrix A [0..n-1][0..n-1] that is to be Cholesky decomposed.</param>
        public CholeskyDecomposition(Matrix A)
        {
            IsPositiveDefinite = false;
            int i, j, k;
            n = A.NumberOfRows;
            this.A = new Matrix(A.ToArray());
            L = new Matrix(A.ToArray());
            double sum;
            if (A.NumberOfColumns != A.NumberOfRows)
            {
                throw new ArgumentOutOfRangeException(nameof(A), "The matrix A must be square.");
            }

            for (i = 0; i < n; i++)
            {
                for (j = i; j < n; j++)
                {
                    sum = L[i, j];
                    for (k = i - 1; k >= 0; k -= 1)
                        sum -= L[i, k] * L[j, k];
                    if (i == j)
                    {
                        if (sum <= 0d)
                            throw new Exception("Cholesky Decomposition failed. The input matrix is not positive-definite.");
                        L[i, i] = Math.Sqrt(sum);
                    }
                    else
                    {
                        L[j, i] = sum / L[i, i];
                    }
                }
            }

            for (i = 0; i < n; i++)
            {
                for (j = 0; j < i; j++)
                    L[j, i] = 0.0d;
            }
            // Failure of the decomposition indicates that the matrix A is not positive-definite. 
            // Success, means it is. 
            IsPositiveDefinite = true;
        }

        private readonly int n; // Number of rows in A

        /// <summary>
        /// Stores the decomposition.
        /// </summary>
        public Matrix L { get; private set; }

        /// <summary>
        /// Stores the input matrix A that was decomposed.
        /// </summary>
        public Matrix A { get; private set; }

        /// <summary>
        /// Determines whether the input matrix A is positive definite.
        /// </summary>
        public bool IsPositiveDefinite { get; private set; }

        /// <summary>
        /// Solves the set of n linear equations A*x=b using the stored Cholesky decomposition of A.
        /// </summary>
        /// <param name="B">Right-hand side vector b [0..n-1].</param>
        public Vector Solve(Vector B)
        {
            int i, k;
            double sum;
            var x = new Vector(n);
            if (B.Length != n)
            {
                throw new ArgumentOutOfRangeException(nameof(B), "The vector b must have the same number of rows as the matrix A.");
            }
            for (i = 0; i < n; i++)
            {
                for (sum = B[i], k = i - 1; k >= 0; k--) sum -= L[i,k] * x[k];
                x[i] = sum / L[i,i];
            }
            for (i = n - 1; i >= 0; i--)
            {
                for (sum = x[i], k = i + 1; k < n; k++) sum -= L[k,i] * x[k];
                x[i] = sum / L[i,i];
            }
            return x;
        }

        /// <summary>
        /// Multiply L*Y=b where L is the lower triangular matrix in the stored Cholesky decomposition.
        /// </summary>
        /// <param name="y">The input vector y [0..n-1].</param>
        public double[] Multiply(double[] y)
        {
            int i, j;
            var b = new double[n];
            if (y.Length != n)
            {
                throw new ArgumentOutOfRangeException(nameof(y), "The vector y must have the same number of rows as the matrix A.");
            }

            for (i = 0; i < n; i++)
            {
                b[i] = 0.0d;
                for (j = 0; j < i; j++)
                    b[i] += L[i, j] * y[j];
            }

            return b;
        }

        /// <summary>
        /// Solves L*y=b where L is the lower triangular matrix in the stored Cholesky decomposition.
        /// </summary>
        /// <param name="b">The right-hand side vector b [0..n-1].</param>
        public double[] SolveYGivenB(double[] b)
        {
            int i, j;
            var y = new double[n];
            double sum;
            if (b.Length != n)
            {
                throw new ArgumentOutOfRangeException(nameof(b), "The vector b must have the same number of rows as the matrix A.");
            }

            for (i = 0; i < n; i++)
            {
                sum = b[i];
                for (j = 0; j < i; j++)
                    sum -= L[i, j] * y[j];
                y[i] = sum / L[i, i];
            }

            return y;
        }

        /// <summary>
        /// Returns the matrix inverse A^-1 using the stored Cholesky decomposition.
        /// </summary>
        public Matrix InverseA()
        {
            int i, j, k;
            var Ainv = new Matrix(n);
            double sum;
            for (i = 0; i < n; i++)
            {
                for (j = 0; j <= i; j++)
                {
                    sum = i == j ? 1.0d : 0.0d;
                    for (k = i - 1; k >= j; k -= 1)
                        sum -= L[i, k] * Ainv[j, k];
                    Ainv[j, i] = sum / L[i, i];
                }
            }

            for (i = n - 1; i >= 0; i -= 1)
            {
                for (j = 0; j <= i; j++)
                {
                    sum = i < j ? 0.0d : Ainv[j, i];
                    for (k = i + 1; k < n; k++)
                        sum -= L[k, i] * Ainv[j, k];
                    Ainv[j, i] = sum / L[i, i];
                    Ainv[i, j] = Ainv[j, i];
                }
            }

            return Ainv;
        }

        /// <summary>
        /// Using the stored Cholesky decomposition, returns the determinant of the matrix A.
        /// </summary>
        public double Determinant()
        {
            double d = 1d;
            for (int i = 0; i < n; i++)
                d *= L[i, i];
            return Math.Pow(d, 2d);
        }

        /// <summary>
        /// Using the stored Cholesky decomposition, returns the logarithm of the determinant of the matrix A.
        /// </summary>
        public double LogDeterminant()
        {
            double sum = 0d;
            for (int i = 0; i < n; i++)
                sum += Math.Log(L[i, i]);
            return 2.0d * sum;
        }

        
    }
}