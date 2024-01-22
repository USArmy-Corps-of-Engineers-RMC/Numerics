﻿using Numerics.Distributions;
using Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mathematics.Integration
{
    public class Integrands
    {

        /// <summary>
        /// Test function. The integral of x^3, should equal 0.25
        /// </summary>
        public static double FX3(double x)
        {
            return Math.Pow(x, 3d);
        }

        /// <summary>
        /// Test function. The integral of Cos(x), should equal ~ 1.6829419
        /// </summary>
        public static double Cosine(double x)
        {
            return Math.Cos(x);
        }

        /// <summary>
        /// Test function. The integral of Sine(x), should equal ~ 0.459697694131
        /// </summary>
        public static double Sine(double x)
        {
            return Math.Sin(x);
        }

        /// <summary>
        /// Test function. The integral 2rd order polynomial, should equal 57.
        /// </summary>
        public static double FXX(double x)
        {
            return 0.5 + 24 * x + 3 * x * x;
        }

        /// <summary>
        /// Test function. The integral 3rd order polynomial, should equal 89.
        /// </summary>
        public static double FXXX(double x)
        {
            return 0.5 + 24 * x + 3 * x * x + 8 * x * x * x;
        }

        /// <summary>
        /// Test function. The integral of Pi. Should equal ~3.14
        /// </summary>
        /// <param name="vals">Array of values.</param>
        public static double PI(double[] vals)
        {
            var x = vals[0];
            var y = vals[1];
            return (x * x + y * y < 1) ? 1 : 0;
        }

        /// <summary>
        /// Test function from GNU Scientific Library, GSL. Result should equal 1.3932039296856768591842462603255
        /// </summary>
        public static double GSL(double[] x)
        {
            double A = 1.0 / (Math.PI * Math.PI * Math.PI);
            return A / (1.0 - Math.Cos(x[0]) * Math.Cos(x[1]) * Math.Cos(x[2]));
        }

        private static double[] mu20 = new double[] { 10, 30, 17, 99, 68, 26, 35, 55, 13, 59, 12, 28, 49, 54, 20, 47, 12, 76, 70, 57 };
        private static double[] sigma20 = new double[] { 2, 15, 5, 14, 7, 24, 29, 22, 22, 1, 3, 28, 19, 18, 4, 24, 23, 26, 26, 19 };

        /// <summary>
        /// Test function for the sum of normal distributions. Max D=20.
        /// </summary>
        /// <param name="p">Array of probability values.</param>
        public static double SumOfNormals(double[] p)
        {
            double result = 0;
            for (int i = 0; i < p.Length; i++)
            {
                result += mu20[i] + sigma20[i] * Normal.StandardZ(p[i]);
            }
            return result;
        }


        public static double SumOfNormalsZ(double[] z, double w)
        {
            var volume = Math.Pow(Normal.StandardZ(1 - 1E-16) - Normal.StandardZ(1E-16), z.Length);

            double result = 0;
            for (int i = 0; i < z.Length; i++)
            {
                result += mu20[i] + sigma20[i] * z[i];
            }
            return result / volume;
        }

    }
}
