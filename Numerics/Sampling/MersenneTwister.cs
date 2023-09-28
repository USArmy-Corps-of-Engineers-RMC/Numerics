using System;

//
//   A C-program for MT19937, with initialization improved 2002/1/26.
//  Coded by Takuji Nishimura and Makoto Matsumoto.
//
//  Before using, initialize the state by using init_genrand(seed)
//   or init_by_array(init_key, key_length).
//
//  Copyright (C) 1997 - 2002, Makoto Matsumoto and Takuji Nishimura,
//  All rights reserved.
//
//  Redistribution and use in source and binary forms, with or without
//   modification, are permitted provided that the following conditions
//  are met:
//
//    1. Redistributions of source code must retain the above copyright
//        notice, this list of conditions and the following disclaimer.
//
//    2. Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//
//   3. The names of its contributors may not be used to endorse or promote
//      products derived from this software without specific prior written
//      permission.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
//  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
//  LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
//  A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE COPYRIGHT OWNER OR
//  CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
//  EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
//  PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
//  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
//  LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//
//   Any feedback is very welcome.
//  http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/emt.html
//  email: m-mat @ math.sci.hiroshima-u.ac.jp (remove space)
//

namespace Numerics.Sampling
{

    /// <summary>
    /// The Mersenne Twister pseudo-random number generator.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// <para>
    /// The source code can be found at:
    /// <see href="http://www.math.sci.hiroshima-u.ac.jp/m-mat/MT/MT2002/CODES/mt19937ar.c"/>
    /// </para>
    /// </remarks>
    public class MersenneTwister : System.Random
    {

        /// <summary>
        /// Construct a Mersenne Twister PRNG using the clock to create a random seed.
        /// </summary>
        public MersenneTwister()
        {
            init_genrand((uint)DateTime.UtcNow.Ticks);
        }


        /// <summary>
        /// Construct a Mersenne Twister PRNG given a seed. 
        /// </summary>
        /// <param name="seed">The PRNG seed.</param>
        public MersenneTwister(int seed)
        {
            init_genrand((uint)seed);
        }


        const int N = 624;
        const int M = 397;
        const uint matrixA = 0x9908b0df;     // constant vector a 
        const uint upperMask = 0x80000000;   // most significant w-r bits 
        const uint lowerMask = 0x7fffffff;   // least significant r bits 
        private uint[] mt = new uint[N];
        int mti = N + 1;
        static readonly uint[] mag01 = { 0x0U, matrixA };


        // The following constants, are used within the ADDITIONAL functions genrand_real2b() and
        // genrand_real3b(), equivalent to genrand_real() and genrand_real3(), but that return
        // evenly distributed values in the ranges [0, 1-kMT_Gap] and [0+kMT_Gap, 1-kMT_Gap],
        // respectively. A similar statement is valid also for genrand_real2c(), genrand_real4b()
        // and genrand_real5b(). See the section "Functions and procedures implemented" above,
        // for more details.
        // 
        // If you want to change the value of kMT_Gap, it is suggested to do it so that:
        // 5e-15 <= kMT_Gap <= 5e-2

        const double k2_31 = 2147483648.0;     // 2^31   ==  2147483648 == 80000000
        const double k2_32 = 2.0 * k2_31;      // 2^32   ==  4294967296 == 0
        const double k2_32b = k2_32 - 1.0;     // 2^32-1 ==  4294967295 == FFFFFFFF == -1

        const double kMT_Gap = 0.0000000000005;       // 5.0E-13
        const double kMT_Gap2 = 2.0 * kMT_Gap;         // 1.0E-12
        const double kMT_GapInterval = 1.0 - kMT_Gap2; // 0.9999999999990
        const double kMT_2b = kMT_GapInterval / k2_32b;
        const double kMT_2c = kMT_2b;
        const double kMT_3b = kMT_2b;

        /// <summary>
        /// Initialize with seed.
        /// </summary>
        /// <param name="seed">The PRNG seed.</param>
        private void init_genrand(uint seed)     
        {
            mt[0] = seed & 0xffffffff;
            for (mti = 1; mti < N; mti++)
            {
                mt[mti] = 1812433253 * (mt[mti - 1] ^ (mt[mti - 1] >> 30)) + (uint)mti;
                /* See Knuth TAOCP Vol2. 3rd Ed. P.106 for multiplier. */
                /* In the previous versions, MSBs of the seed affect   */
                /* only MSBs of the array _mt[].                       */
                /* 2002/01/09 modified by Makoto Matsumoto             */
                mt[mti] &= 0xffffffff;
                /* for >32 bit machines */
            }
        }

        /// <summary>
        /// Generates a random number on [0,0xffffffff]-interval
        /// </summary>
        /// <returns></returns>
        private uint genrand_int32()  
        {

            uint y;

            /* mag01[x] = x * MATRIX_A  for x=0,1 */

            if (mti >= N)
            {
                /* generate _n words at one time */
                int kk;

                if (mti == N + 1) /* if init_genrand() has not been called, */
                {
                    init_genrand(5489); /* a default initial seed is used */
                }

                for (kk = 0; kk < N - M; kk++)
                {
                    y = (mt[kk] & upperMask) | (mt[kk + 1] & lowerMask);
                    mt[kk] = mt[kk + M] ^ (y >> 1) ^ mag01[y & 0x1];
                }

                for (; kk < N - 1; kk++)
                {
                    y = (mt[kk] & upperMask) | (mt[kk + 1] & lowerMask);
                    mt[kk] = mt[kk + (M - N)] ^ (y >> 1) ^ mag01[y & 0x1];
                }

                y = (mt[N - 1] & upperMask) | (mt[0] & lowerMask);
                mt[N - 1] = mt[M - 1] ^ (y >> 1) ^ mag01[y & 0x1];

                mti = 0;
            }

            y = mt[mti++];

            /* Tempering */
            y ^= y >> 11;
            y ^= (y << 7) & 0x9d2c5680;
            y ^= (y << 15) & 0xefc60000;
            y ^= y >> 18;

            return y;
        }

        /// <summary>
        /// [0.0, 1.0)=[0, 1-(2*gap)] =[0.0, 0.9999999999990] (*) (0.0 included, 1.0 excluded)
        /// </summary>
        public double genrand_real2b()
        {
            return genrand_int32() * kMT_2b;
        }

        /// <summary>
        /// (0.0, 1.0]=[0+(2*gap),1.0]=[1.0e-12, 1.0] (*) (0.0 excluded, 1.0 included)
        /// </summary>
        /// <returns></returns>
        public double genrand_real2c()
        {
            return kMT_Gap2 + (genrand_int32() * kMT_2c);
        }

        /// <summary>
        /// (0.0, 1.0)=[0+gap, 1-gap] =[5.0e-13, 0.9999999999995] (*) (both 0.0 and 1.0 excluded)
        /// </summary>
        public double genrand_real3b()
        {
            return kMT_Gap + (genrand_int32() * kMT_3b);
        }

        /// <summary>
        /// Returns a non-negative random integer./>.
        /// </summary>
        public sealed override int Next()
        {
            uint uint32 = genrand_int32();
            int int31 = (int)(uint32 >> 1);
            if (int31 == int.MaxValue)
            {
                return Next();
            }

            return int31;
        }

        /// <summary>
        /// Returns a non-negative random integer that is less than the specified maximum.
        /// </summary>
        /// <param name="maxExclusive">The exclusive upper bound of the random number returned.</param>
        public sealed override int Next(int maxExclusive)
        {
            return (int)(NextDouble() * maxExclusive);
        }

        /// <summary>
        /// Returns a non-negative random integer that is less than the specified maximum.
        /// </summary>
        /// <param name="minExclusive">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxExclusive">The exclusive upper bound of the random number returned.</param>
        public sealed override int Next(int minExclusive, int maxExclusive)
        {
            return Next(maxExclusive - minExclusive) + minExclusive;
        }

        /// <summary>
        /// Returns a random double greater than or equal to 0 and less than 1.
        /// </summary>
        public sealed override double NextDouble()
        {
            return genrand_real2b();
        }

    }
}
