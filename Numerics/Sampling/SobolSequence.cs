using System;
using System.IO;
using System.Reflection;

namespace Numerics.Sampling
{
    /// <summary>
    /// A class for generating a Sobol sequence.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A Sobol sequence is a low-discrepancy sequence with the property that for all values of N,
    /// its subsequence (x1, ... xN) has a low discrepancy. It can be used to generate pseudo-random
    /// points in a space S, which are equi-distributed.
    /// </para>
    /// <para>
    /// The implementation already comes with support for up to 21201 dimensions with direction numbers
    /// calculated from <see href="http://web.maths.unsw.edu.au/~fkuo/sobol/" />
    /// </para>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// References:
    /// This code was converted from the Apache Math Commons.
    /// <see href = "https://commons.apache.org/proper/commons-math/apidocs/src-html/org/apache/commons/math4/random/SobolSequenceGenerator.html" />
    /// <see href = "http://en.wikipedia.org/wiki/Sobol_sequence" />
    /// <see href = "http://web.maths.unsw.edu.au/~fkuo/sobol/" />
    /// "Numerical Recipes: The art of Scientific Computing, Third Edition. Press et al. 2017.
    /// </remarks>
    public class SobolSequence
    {

        public SobolSequence(int dimension)
        {
            if (dimension < 1 || dimension > MAX_DIMENSION)
            {
                throw new ArgumentException("The dimension must be between 1 and " + MAX_DIMENSION);
            }
            Dimension = dimension;
            _direction = new long[dimension, BITS + 1];
            x = new long[dimension];

            initialize();
        }

        /// <summary>
        /// The number of bits to use. 
        /// </summary>
        private static int BITS = 52;

        /// <summary>
        /// The scaling factor.
        /// </summary>
        private static double SCALE = Math.Pow(2, BITS);

        /// <summary>
        /// The maximum supported space dimension. 
        /// </summary>
        private static int MAX_DIMENSION = 21201;

        /// <summary>
        /// The current index in the sequence. 
        /// </summary>
        private int count; 

        /// <summary>
        /// Space dimension. 
        /// </summary>
        public int Dimension { get; private set; }

        /// <summary>
        /// The direction vector for each component.
        /// </summary>
        private long[,] _direction;

        /// <summary>
        /// The current state.
        /// </summary>
        private long[] x;


        private void initialize()
        {

            // special case: dimension 1 -> use unit initialization
            for (int i = 1; i <= BITS; i++)
            {
                _direction[0, i] = 1L << (BITS - i);
            }

            using (StreamReader reader = new StreamReader(new MemoryStream(My.Resources.Resources.new_joe_kuo_6)))
            {
                // ignore the first line
                reader.ReadLine();

                int index = 1;
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    var st = line.Split(' ');

                    try
                    {
                        int dim;
                        int.TryParse(st[0], out dim);
                        
                       if (dim >= 2 && dim <= Dimension)
                       {
                            // we have found the right dimension
                            int i, s = 0, a = 0;
                            for (i = 1; i < st.Length; i++)
                            {
                                if (st[i] != "")
                                {
                                    int.TryParse(st[i], out s);
                                    break;
                                }
                            }
                            i++;
                            for (; i < st.Length; i++)
                            {
                                if (st[i] != "")
                                {
                                    int.TryParse(st[i], out a);
                                    break;
                                }
                            }
                            i++;
                            int[] m = new int[s + 1];
                            for (; i < st.Length; i++)
                            {
                                if (st[i] != "")
                                {
                                    for (int j = 1; j <= s; j++)
                                    {
                                        int.TryParse(st[i + j - 1], out m[j]);
                                    }
                                    break;
                                }                        
                            }
                            initDirectionVector(index++, a, m);
                        }

                        if (dim > Dimension)
                        {
                           return;
                        }

                    }
                    catch (Exception ex)
                    {
                       throw ex;
                    }
                }

            }

        }


        private void initDirectionVector(int d,  int a,  int[] m)
        {
            int s = m.Length - 1;
            for (int i = 1; i <= s; i++)
            {
                _direction[d, i] = ((long)m[i]) << (BITS - i);
            }
            for (int i = s + 1; i <= BITS; i++)
            {
                _direction[d, i] = _direction[d, i - s] ^ (_direction[d, i - s] >> s);
                for (int k = 1; k <= s - 1; k++)
                {
                    _direction[d, i] ^= ((a >> (s - 1 - k)) & 1) * _direction[d, i - k];
                }
            }
        }

        public double[] NextVector()
        {
            double[] v = new double[Dimension];
            if (count == 0)
            {
                count++;
                return v;
            }

            // find the index c of the rightmost 0
            int c = 1;
            int value = count - 1;
            while ((value & 1) == 1)
            {
                value >>= 1;
                c++;
            }

            for (int i = 0; i < Dimension; i++)
            {
                x[i] ^= _direction[i, c];
                v[i] = (double)x[i] / SCALE;
            }
            count++;
            return v;
        }

        public double[] SkipTo(int index)
        {
            if (index == 0)
            {
                // reset x vector
                x.Fill(0);
            }
            else
            {
                int i = index - 1;
                long grayCode = i ^ (i >> 1); // compute the gray code of i = i XOR floor(i / 2)
                for (int j = 0; j < Dimension; j++)
                {
                    long result = 0;
                    for (int k = 1; k <= BITS; k++)
                    {
                        long shift = grayCode >> (k - 1);
                        if (shift == 0)
                        {
                            // stop, as all remaining bits will be zero
                            break;
                        }
                        // the k-th bit of i
                         long ik = shift & 1;
                        result ^= ik * _direction[j, k];
                    }
                    x[j] = result;
                }
            }
            count = index;
            return NextVector();
        }

    }
}
