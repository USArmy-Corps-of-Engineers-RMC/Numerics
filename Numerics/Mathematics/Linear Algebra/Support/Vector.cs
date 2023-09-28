using System;
using System.Collections.Generic;
using System.Linq;

namespace Numerics.Mathematics.LinearAlgebra
{

    /// <summary>
    /// A simple vector class.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public class Vector
    {
     
        /// <summary>
        /// Construct a new vector with specified length.
        /// </summary>
        /// <param name="length">The number of elements in the vector.</param>
        public Vector(int length)
        {
            _array = new double[length];
        }

        /// <summary>
        /// Construct a new vector with specified length and fill it with a constant value.
        /// </summary>
        /// <param name="length"></param>
        /// <param name="fill">Fill the vector with a constant fill value.</param>
        public Vector(int length, double fill)
        {
            _array = new double[length];
            for(int i = 0; i < length; i++)
            {
                _array[i] = fill;
            }
        }

        /// <summary>
        /// Construct a new vector based an initial array.
        /// </summary>
        /// <param name="initialArray">Initializing array.</param>
        public Vector(double[] initialArray)
        {
            _array = (double[])initialArray.Clone();
        }

           
        private double[] _array;

        /// <summary>
        /// The length of the vector.
        /// </summary>
        public int Length {
            get { return _array.Length; }
        }

        /// <summary>
        /// Get the element at the specific index.
        /// </summary>
        /// <param name="index">The zero-based row index of the element to get or set.</param>
        public double this[int index]
        {
            get { return _array[index]; }
            set { _array[index] = value; }
        }

         /// <summary>
         /// The vector header text. 
         /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Returns the vector as an array.
        /// </summary>
        public double[] ToArray()
        {
            return (double[])_array.Clone();
        }

        /// <summary>
        /// Returns the vector as a list.
        /// </summary>
        public List<double> ToList()
        {
            return _array.ToList();
        }

        /// <summary>
        /// Returns a clone of the vector.
        /// </summary>
        public Vector Clone()
        {
            return new Vector(ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static double DotProduct(Vector A, Vector B)
        {
            if (A.Length != B.Length) throw new ArgumentException(nameof(A.Length), "The vectors must be the same length.");
            double sum = 0;
            for (int i = 0; i < A.Length; i++)
                sum += A[i] * B[i];
            return sum;
        }

        /// <summary>
        /// Multiplies a vector A with a scalar.
        /// </summary>
        /// <param name="A">The vector to multiply.</param>
        /// <param name="scalar">The scalar to multiply by.</param>
        public static Vector operator *(Vector A, double scalar)
        {
            var v = new Vector(A.Length);
            for (int i = 0; i < A.Length; i++)
                v[i] = A[i] * scalar;
            return v;
        }

        /// <summary>
        /// Raises each element of the vector A by the exponent b.
        /// </summary>
        /// <param name="A">The vector to multiply.</param>
        /// <param name="b">The exponent b..</param>
        public static Vector operator ^(Vector A, double b)
        {          
            var v = new Vector(A.Length);
            for (int i = 0; i < A.Length; i++)
                v[i] = Math.Pow(A[i], b);
            return v;
        }

        /// <summary>
        /// Multiplies vectors A and B by multiplying corresponding elements. Vectors A and B must the same size. 
        /// </summary>
        /// <param name="A">Left-side vector.</param>
        /// <param name="B">Right-side vector.</param>
        public static Vector operator *(Vector A, Vector B)
        {
            if (A.Length != B.Length) throw new ArgumentException(nameof(A.Length), "The vectors must be the same length.");
            var v = new Vector(A.Length);
            for (int i = 0; i < A.Length; i++)
                v[i] = A[i] * B[i];
            return v;
        }

        /// <summary>
        /// Adds vectors A and B by summing corresponding elements. Vectors A and B must the same size. 
        /// </summary>
        /// <param name="A">Left-side vector.</param>
        /// <param name="B">Right-side vector.</param>
        public static Vector operator +(Vector A, Vector B)
        {
            if (A.Length != B.Length) throw new ArgumentException(nameof(A.Length), "The vectors must be the same length.");
            var v = new Vector(A.Length);
            for (int i = 0; i < A.Length; i++)
                v[i] = A[i] + B[i];
            return v;
        }



        /// <summary>
        /// Subtracts vectors A and B by subtracting corresponding elements. Vectors A and B must the same size. 
        /// </summary>
        /// <param name="A">Left-side vector.</param>
        /// <param name="B">Right-side vector.</param>
        public static Vector operator -(Vector A, Vector B)
        {
            if (A.Length != B.Length) throw new ArgumentException(nameof(A.Length), "The vectors must be the same length.");
            var v = new Vector(A.Length);
            for (int i = 0; i < A.Length; i++)
                v[i] = A[i] - B[i];
            return v;
        }
    }
}