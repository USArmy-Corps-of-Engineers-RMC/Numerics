using System;
using System.Collections.Generic;

namespace Numerics.Mathematics.LinearAlgebra
{

    /// <summary>
    /// A simple class for performing Matrix operations.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public class Matrix
    {

        /// <summary>
        /// Construct a new matrix with specified number of rows and columns.
        /// </summary>
        /// <param name="numberOfRows">The number of rows.</param>
        /// <param name="numberOfColumns">The number of columns.</param>
        public Matrix(int numberOfRows, int numberOfColumns)
        {
            _array = new double[numberOfRows, numberOfColumns];
        }

        /// <summary>
        /// Constructs a new square matrix.
        /// </summary>
        /// <param name="size">The number of rows or columns (rows = columns).</param>
        public Matrix(int size)
        {
            _array = new double[size, size];
        }

        /// <summary>
        /// Construct a new matrix based on an initial array.
        /// </summary>
        /// <param name="initialArray">Initializing array.</param>
        public Matrix(double[,] initialArray)
        {
            _array = (double[,])initialArray.Clone();
        }

        /// <summary>
        /// Constructs a new matrix based on a single column array.
        /// </summary>
        /// <param name="initialArray">Initializing array.</param>
        public Matrix(double[] initialArray)
        {
            _array = new double[initialArray.Length, 1];
            for (int i = 0; i < _array.Length; i++) 
                _array[i, 0] = initialArray[i];
        }

        /// <summary>
        /// Constructs a new matrix based on a single column array.
        /// </summary>
        /// <param name="initialArray">Initializing array.</param>
        public Matrix(float[] initialArray)
        {
            _array = new double[initialArray.Length, 1];
            for (int i = 0; i < _array.Length; i++)
                _array[i, 0] = initialArray[i];
        }

        /// <summary>
        /// Constructs a new matrix based on a vector.
        /// </summary>
        /// <param name="initialVector">Initializing vector.</param>
        public Matrix(Vector initialVector)
        {
            _array = new double[initialVector.Length, 1];
            for (int i = 0; i < _array.Length; i++) _array[i, 0] = initialVector[i];
        }

        /// <summary>
        /// Constructs a new matrix based on a list of single column arrays.
        /// </summary>
        /// <param name="listOfArrays">List of initializing arrays.</param>
        public Matrix(List<double[]> listOfArrays)
        {
            _array = new double[listOfArrays[1].Length, listOfArrays.Count];
            for (int i = 0; i < _array.GetLength(0); i++)
            {
                for (int j = 0; j < listOfArrays.Count; j++)
                {
                    _array[i, j] = listOfArrays[j][i];
                }
            }
        }
        private double[,] _array;

        /// <summary>
        /// Gets the number of rows.
        /// </summary>
        public int NumberOfRows
        {
            get { return _array.GetLength(0); }
        }

        /// <summary>
        /// Gets the number of columns.
        /// </summary>
        public int NumberOfColumns
        {
            get { return _array.GetLength(1); }
        }

        /// <summary>
        /// Get and sets the element at the specific row and column index.
        /// </summary>
        /// <param name="rowIndex">The zero-based row index of the element to get or set.</param>
        /// <param name="columnIndex">The zero-based column index of the element to get or set.</param>
        public double this[int rowIndex, int columnIndex]
        {
            get { return _array[rowIndex, columnIndex]; }
            set { _array[rowIndex, columnIndex] = value; }
        }

        /// <summary>
        /// The matrix column header text. 
        /// </summary>
        public string[] Header { get; set; }

        /// <summary>
        /// Evaluates whether this matrix is symmetric.
        /// </summary>
        public bool IsSymmetric
        {
            get
            {
                if (NumberOfRows != NumberOfColumns)
                    return false;
                for (int i = 0; i < NumberOfRows; i++)
                {
                    for (int j = i + 1; j < NumberOfColumns; j++)
                    {
                        if (this[i, j] != this[j, i])
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Determines whether this matrix is square.
        /// </summary>
        public bool IsSquare
        {
            get
            {
                if (NumberOfRows == NumberOfColumns)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Returns the matrix as an array.
        /// </summary>
        public double[,] ToArray()
        {
            return (double[,])_array.Clone();
        }

        /// <summary>
        /// Returns a copy of a specific row.
        /// </summary>
        /// <param name="index">The zero-based row index</param>
        public double[] Row(int index)
        {
            var vector = new double[NumberOfColumns];
            for (int j = 0; j < NumberOfColumns; j++)
                vector[j] = _array[index, j];
            return vector;
        }

        /// <summary>
        /// Returns a copy of a specific column.
        /// </summary>
        /// <param name="index">The zero-based column index</param>
        public double[] Column(int index)
        {
            var vector = new double[NumberOfRows];
            for (int i = 0; i < NumberOfRows; i++)
                vector[i] = _array[i, index];
            return vector;
        }

        /// <summary>
        /// Returns a new matrix containing the upper triangle of this matrix.
        /// </summary>
        public Matrix UpperTriangle()
        {
            var result = new Matrix(NumberOfRows, NumberOfColumns);
            for (int i = 0; i < NumberOfRows; i++)
            {
                for (int j = i; j < NumberOfColumns; j++)
                    result[i, j] = this[i, j];
            }

            return result;
        }

        /// <summary>
        /// Returns a new matrix containing the lower triangle of this matrix.
        /// </summary>
        public Matrix LowerTriangle()
        {
            var result = new Matrix(NumberOfRows, NumberOfColumns);
            for (int i = 0; i < NumberOfRows; i++)
            {
                for (int j = 0; j < NumberOfColumns; j++)
                {
                    if (j > i) break;
                    result[i, j] = this[i, j];
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the elements of the diagonal.
        /// </summary>
        /// <remarks>
        /// For non-square matrices, the method returns Min(Rows, Columns) elements where i = j (i is the row index, and j is the column index).
        /// </remarks>
        public double[] Diagonal()
        {
            int min = Math.Min(NumberOfRows, NumberOfColumns);
            var result = new double[min];
            for (int i = 0; i < min; i++)
                result[i] = this[i, i];
            return result;
        }

        /// <summary>
        /// Returns the diagonal matrix.
        /// </summary>
        /// <param name="A">The square matrix.</param>
        public static Matrix Diagonal(Matrix A)
        {
            if (A.IsSquare == false)
                throw new ArgumentException("The matrix must be square.");
            var D = new Matrix(A.NumberOfColumns);
            for (int i = 0; i < A.NumberOfRows; i++)
            {
                for (int j = 0; j < A.NumberOfColumns; j++)
                {
                    D[i, j] = i == j ? A[i, j] : 0;
                }
            }
            return D;
        }

        /// <summary>
        /// Returns the identity matrix of size n.
        /// </summary>
        /// <param name="size">The number of rows or columns (rows = columns).</param>
        /// <para>
        /// <see href = "https://en.wikipedia.org/wiki/Identity_matrix" />
        /// </para>
        public static Matrix Identity(int size)
        {
            var I = new Matrix(size);
            for (int j = 0; j < size; j++)
                I[j, j] = 1d;
            return I;
        }

        /// <summary>
        /// Returns the transposed matrix.
        /// </summary>
        /// <param name="A">The matrix to transpose.</param>
        public static Matrix Transpose(Matrix A)
        {
            var t = new Matrix(A.NumberOfColumns, A.NumberOfRows);
            for (int i = 0; i < A.NumberOfRows; i++)
            {
                for (int j = 0; j < A.NumberOfColumns; j++)
                    t[j, i] = A[i, j];
            }

            return t;
        }


        /// <summary>
        /// Returns the determinant of the matrix.
        /// </summary>
        /// <param name="A">The matrix.</param>
        public static double Determinant(Matrix A)
        {
            if (A.IsSquare == false)
                throw new ArgumentException("The matrix must be square.");
            if (A.NumberOfRows == 1)
            {
                return A[0, 0];
            }
            else
            {
                int sign = 1;
                double D = 0d;
                for (int i = 0; i < A.NumberOfRows; i++)
                {
                    D += sign * A[0, i] * Determinant(minor(A, 0, i));
                    sign *= -1;
                }
                return D;
            }
        }

        /// <summary>
        /// Returns the permanent of the matrix.
        /// </summary>
        /// <param name="A">The matrix.</param>
        public static double Permanent(Matrix A)
        {
            if (A.IsSquare == false)
                throw new ArgumentException("The matrix must be square.");
            if (A.NumberOfRows == 1)
            {
                return A[0, 0];
            }
            else
            {
                double sum = 0d;
                for (int i = 0; i < A.NumberOfRows; i++)
                    sum = sum + A[0, i] * Permanent(minor(A, 0, i));
                return sum;
            }
        }

        private static Matrix minor(Matrix A, int x, int y)
        {
            int n = A.NumberOfRows - 1;
            var result = new Matrix(n);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i < x && j < y)
                    {
                        result[i, j] = A[i, j];
                    }
                    else if (i >= x && j < y)
                    {
                        result[i, j] = A[i + 1, j];
                    }
                    else if (i < x && j >= y)
                    {
                        result[i, j] = A[i, j + 1];
                    }
                    else
                    {
                        result[i, j] = A[i + 1, j + 1];
                    }
                }
            }
            return result;
        }


        #region Math

        /// <summary>
        /// Computes the Sqrt of the matrix point wise. 
        /// </summary>
        public Matrix Sqrt()
        {
            var result = new Matrix(NumberOfRows, NumberOfColumns);
            for (int i = 0; i < NumberOfRows; i++)
                for (int j = 0; j < NumberOfColumns; j++)
                    result[i, j] = Math.Sqrt(this[i, j]);
            return result;
        }

        /// <summary>
        /// Computes the log of the matrix point wise. 
        /// </summary>
        public Matrix Log()
        {
            var result = new Matrix(NumberOfRows, NumberOfColumns);
            for (int i = 0; i < NumberOfRows; i++)
                for (int j = 0; j < NumberOfColumns; j++)
                    result[i, j] = Math.Log(this[i, j]);
            return result;
        }

        /// <summary>
        /// Computes the exponential of the matrix point wise. 
        /// </summary>
        public Matrix Exp()
        {
            var result = new Matrix(NumberOfRows, NumberOfColumns);
            for (int i = 0; i < NumberOfRows; i++)
                for (int j = 0; j < NumberOfColumns; j++)
                    result[i, j] = Math.Exp(this[i, j]);
            return result;
        }

        /// <summary>
        /// Returns the sum of every point in the matrix.
        /// </summary>
        public double Sum()
        {
            double result = 0;
            for (int i = 0; i < NumberOfRows; i++)
                for (int j = 0; j < NumberOfColumns; j++)
                    result += this[i, j];
            return result;
        }

        #endregion

        /// <summary>
        /// Multiplies matrix A and B and returns the results as a matrix.
        /// </summary>
        /// <param name="A">Left-side matrix.</param>
        /// <param name="B">Right-side matrix.</param>
        public static Matrix operator *(Matrix A, Matrix B)
        {
            int aRows = A.NumberOfRows;
            int bCols = B.NumberOfColumns;
            int aCols = A.NumberOfColumns;
            if (aCols != B.NumberOfRows)
                throw new ArgumentException("The number of rows in matrix B must be equal to the number of columns in matrix A.");
            var result = new Matrix(aRows, bCols);
            for (int i = 0; i < aRows; i++)
            {
                for (int j = 0; j < bCols; j++)
                {
                    double sum = 0.0d;
                    for (int k = 0; k < aCols; k++)
                        sum += A[i, k] * B[k, j];
                    result[i, j] = sum;
                }
            }

            return result;
        }


        /// <summary>
        /// Returns the element-wise multiplication of two matrices. 
        /// </summary>
        /// <param name="A">Left-side matrix.</param>
        /// <param name="B">Right-side matrix.</param>
        public static Matrix Multiply(Matrix A, Matrix B)
        {
            int aRows = A.NumberOfRows;
            int aCols = A.NumberOfColumns;
            if (aCols != B.NumberOfColumns)
                throw new ArgumentException("Matrix A and B must be the same size.");
            if (aRows != B.NumberOfRows)
                throw new ArgumentException("Matrix A and B must be the same size.");
            var result = new Matrix(A.NumberOfRows, A.NumberOfColumns);
            for (int i = 0; i < A.NumberOfRows; i++)
            {
                for (int j = 0; j < A.NumberOfColumns; j++)
                    result[i, j] = A[i, j] * B[i, j];
            }
            return result;
        }




        /// <summary>
        /// Multiplies matrix A with a vector B and returns the results as an array vector.
        /// </summary>
        /// <param name="A">The matrix to multiply.</param>
        /// <param name="vector">The vector to multiply with.</param>
        public static double[] operator *(Matrix A, double[] vector)
        {
            int aRows = A.NumberOfRows;
            int aCols = A.NumberOfColumns;
            if (aCols != vector.Length)
                throw new ArgumentException("The number of rows in vector B must be equal to the number of columns in matrix A.");
            var result = new double[aRows];
            for (int i = 0; i < aRows; i++)
            {
                double sum = 0.0d;
                for (int j = 0; j < aCols; j++)
                    sum += A[i, j] * vector[j];
                result[i] = sum;
            }
            return result;
        }


        /// <summary>
        /// Multiplies matrix A with a vector B and returns the results as an array vector.
        /// </summary>
        /// <param name="A">The matrix to multiply.</param>
        /// <param name="B">The vector to multiply with.</param>
        public static Vector operator *(Matrix A, Vector B)
        {
            int aRows = A.NumberOfRows;
            int aCols = A.NumberOfColumns;
            if (aCols != B.Length)
                throw new ArgumentException("The number of rows in vector B must be equal to the number of columns in matrix A.");
            var result = new double[aRows];
            for (int i = 0; i < aRows; i++)
            {
                double sum = 0.0d;
                for (int j = 0; j < aCols; j++)
                    sum += A[i, j] * B[j];
                result[i] = sum;
            }
            return new Vector(result);
        }

        /// <summary>
        /// Returns the dot product of matrix A and vector x
        /// </summary>
        /// <param name="A">The matrix to multiply.</param>
        /// <param name="x">The vector to multiply with.</param>
        public static double DotProduct(Matrix A, Vector x)
        {
            int aRows = A.NumberOfRows;
            int aCols = A.NumberOfColumns;
            if (aCols != x.Length)
                throw new ArgumentException("The number of rows in vector B must be equal to the number of columns in matrix A.");
            double result = 0;
            for (int i = 0; i < aRows; i++)
            {
                double sum = 0.0d;
                for (int j = 0; j < aCols; j++)
                    sum += A[i, j] * x[j];
                result += sum;
            }
            return result;
        }

        /// <summary>
        /// Returns the dot product of matrix A and vector x
        /// </summary>
        /// <param name="A">The matrix to multiply.</param>
        /// <param name="y">The vector to multiply with.</param>
        public static double DotProduct(Vector x, Matrix A, Vector y)
        {
            int aRows = A.NumberOfRows;
            int aCols = A.NumberOfColumns;
            if (aCols != x.Length)
                throw new ArgumentException("The number of rows in vector B must be equal to the number of columns in matrix A.");
            double result = 0;
            for (int i = 0; i < aRows; i++)
            {
                double sum = 0.0d;
                for (int j = 0; j < aCols; j++)
                    sum += A[i, j] * x[j];
                result += sum;
            }
            return result;
        }

        /// <summary>
        /// Multiplies a matrix A with a scalar and returns the results as a matrix.
        /// </summary>
        /// <param name="A">The matrix to multiply.</param>
        /// <param name="scalar">The scalar to multiply by.</param>
        public static Matrix operator *(Matrix A, double scalar)
        {
            var result = new Matrix(A.NumberOfRows, A.NumberOfColumns);
            for (int i = 0; i < A.NumberOfRows; i++)
            {
                for (int j = 0; j < A.NumberOfColumns; j++)
                    result[i, j] = A[i, j] * scalar;
            }

            return result;
        }

        /// <summary>
        /// Multiplies a matrix A with a scalar and returns the results as a matrix.
        /// </summary>
        /// <param name="scalar">The scalar to multiply by.</param>
        /// <param name="A">The matrix to multiply.</param>
        public static Matrix operator *(double scalar, Matrix A)
        {
            var result = new Matrix(A.NumberOfRows, A.NumberOfColumns);
            for (int i = 0; i < A.NumberOfRows; i++)
            {
                for (int j = 0; j < A.NumberOfColumns; j++)
                    result[i, j] = A[i, j] * scalar;
            }

            return result;
        }

        /// <summary>
        /// Divides a matrix A with a scalar and returns the results as a matrix.
        /// </summary>
        /// <param name="A">The matrix to divide.</param>
        /// <param name="scalar">The scalar to divide by.</param>
        public static Matrix operator /(Matrix A, double scalar)
        {
            var result = new Matrix(A.NumberOfRows, A.NumberOfColumns);
            for (int i = 0; i < A.NumberOfRows; i++)
            {
                for (int j = 0; j < A.NumberOfColumns; j++)
                    result[i, j] = A[i, j] / scalar;
            }

            return result;
        }


        /// <summary>
        /// Add matrix A and B and returns the results as a matrix.
        /// </summary>
        /// <param name="A">Left-side matrix.</param>
        /// <param name="B">Right-side matrix.</param>
        public static Matrix operator +(Matrix A, Matrix B)
        {
            int aRows = A.NumberOfRows;
            int aCols = A.NumberOfColumns;
            if (aCols != B.NumberOfColumns)
                throw new ArgumentException("Matrix A and B must be the same size.");
            if (aRows != B.NumberOfRows)
                throw new ArgumentException("Matrix A and B must be the same size.");
            var result = new Matrix(A.NumberOfRows, A.NumberOfColumns);
            for (int i = 0; i < A.NumberOfRows; i++)
            {
                for (int j = 0; j < A.NumberOfColumns; j++)
                    result[i, j] = A[i, j] + B[i, j];
            }
            return result;
        }

        /// <summary>
        /// Subtract matrix A and B and returns the results as a matrix.
        /// </summary>
        /// <param name="A">Left-side matrix.</param>
        /// <param name="B">Right-side matrix.</param>
        public static Matrix operator -(Matrix A, Matrix B)
        {
            int aRows = A.NumberOfRows;
            int aCols = A.NumberOfColumns;
            if (aCols != B.NumberOfColumns)
                throw new ArgumentException("Matrix A and B must be the same size.");
            if (aRows != B.NumberOfRows)
                throw new ArgumentException("Matrix A and B must be the same size.");
            var result = new Matrix(A.NumberOfRows, A.NumberOfColumns);
            for (int i = 0; i < A.NumberOfRows; i++)
            {
                for (int j = 0; j < A.NumberOfColumns; j++)
                    result[i, j] = A[i, j] - B[i, j];
            }
            return result;
        }
    }
}