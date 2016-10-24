
/*
MIT License

Copyright (c) 2016 Erkin Ekici - undergroundwires@safeorb.it

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SafeOrbit.Random
{
    /// <summary>
    /// A fast random number generator. Does not generate secure numbers, use <see cref="SafeRandom"/> for better security.
    /// 
    /// * It is up to 8x faster than <see cref="System.Random"/> depending on, depending on which methods are called.
    /// * Allows fast re-initialisation with a seed, unlike <see cref="System.Random"/> which accepts a seed at construction
    ///  time which then executes a relatively expensive initialisation routine. This provides a vast speed improvement
    ///  if you need to reset the pseudo-random number sequence many times, e.g. if you want to re-generate the same
    ///  sequence of random numbers many times. An alternative might be to cache random numbers in an array, but that 
    ///  approach is limited by memory capacity and the fact that you may also want a large number of different sequences 
    ///  cached. Each sequence can be represented by a single seed value (int) when using <see cref="FastRandom"/>.
    ///  
    /// </summary>
    /// <seealso cref="SafeOrbit.Random.RandomBase" />
    /// <seealso cref="SafeOrbit.Random.IFastRandom" />
    public class FastRandom : RandomBase, IFastRandom
    {
        /// <summary>
        /// Static instance of <see cref="FastRandom"/>
        /// A static RNG that is used to generate seed values when constructing new instances of <see cref="FastRandom"/>.
        /// This overcomes the problem whereby multiple <see cref="FastRandom"/> instances are instantiated within the same
        /// tick count and thus obtain the same seed, that approach can result in extreme biases occuring 
        /// in some cases depending on how the RNG is used.
        /// </summary>
        public static FastRandom StaticInstance => StaticInstanceLazy.Value;
        private static readonly Lazy<FastRandom> StaticInstanceLazy = new Lazy<FastRandom>(() => new FastRandom((int)Environment.TickCount));
        // The +1 ensures NextDouble doesn't generate 1.0
        private const double RealUnitInt = 1.0 / ((double)int.MaxValue + 1.0);
        private const double RealUnitUint = 1.0 / ((double)uint.MaxValue + 1.0);
        private const uint Y = 842502087, Z = 3579807591, W = 273326509;
        private uint _x, _y, _z, _w;
        // Buffer 32 bits in bitBuffer, return 1 at a time, keep track of how many have been returned
        // with bitMask.
        private uint _bitBuffer;
        private uint _bitMask;
        /// <summary>
        /// Initializes a new instance of the <see cref="FastRandom"/> class using time dependent seed.
        /// </summary>
        public FastRandom() : this (StaticInstance.Next()) { }
        /// <summary>
        /// Initialises a new instance using an int value as seed.
        /// </summary>
        public FastRandom(int seed)
        {
            Reseed(seed);
        }
        public void Reseed(int seed)
        {
            // The only stipulation stated for the xorshift RNG is that at least one of
            // the seeds x,y,z,w is non-zero. We fulfill that requirement by only allowing
            // resetting of the x seed.

            // The first random sample will be very closely related to the value of _x we set here. 
            // Thus setting _x = seed will result in a close correlation between the bit patterns of the seed and
            // the first random sample, therefore if the seed has a pattern (e.g. 1,2,3) then there will also be 
            // a recognisable pattern across the first random samples.
            //
            // Such a strong correlation between the seed and the first random sample is an undesirable
            // charactersitic of a RNG, therefore we significantly weaken any correlation by hashing the seed's bits. 
            // This is achieved by multiplying the seed with four large primes each with bits distributed over the
            // full length of a 32bit value, finally adding the results to give _x.
            _x = (uint)((seed * 1431655781)
                        + (seed * 1183186591)
                        + (seed * 622729787)
                        + (seed * 338294347));

            _y = Y;
            _z = Z;
            _w = W;

            _bitBuffer = 0;
            _bitMask = 1;
        }
        /// <summary>
        /// Gets the fast random int between 0 and <see cref="int.MaxValue"/>
        /// </summary>
        /// <returns></returns>
        public int GetInt()
        {
            uint t = _x ^ (_x << 11);
            _x = _y; _y = _z; _z = _w;
            _w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8));
            // Handle the special case where the value int.MaxValue is generated. This is outside of 
            // the range of permitted values, so we therefore call Next() to try again.
            uint rtn = _w & 0x7FFFFFFF;
            if (rtn == 0x7FFFFFFF)
            {
                return Next();
            }
            return (int)rtn;
        }
        public byte[] GetBytes(int length)
        {
            var buffer = new byte[length];
            // Fill up the bulk of the buffer in chunks of 4 bytes at a time.
            uint x = _x, y = _y, z = _z, w = _w;
            int i = 0;
            uint t;
            for (int bound = buffer.Length - 3; i < bound;)
            {
                // Generate 4 bytes. 
                // Increased performance is achieved by generating 4 random bytes per loop.
                // Also note that no mask needs to be applied to zero out the higher order bytes before
                // casting because the cast ignores thos bytes. Thanks to Stefan Troschütz for pointing this out.
                t = (x ^ (x << 11));
                x = y; y = z; z = w;
                w = (w ^ (w >> 19)) ^ (t ^ (t >> 8));

                buffer[i++] = (byte)w;
                buffer[i++] = (byte)(w >> 8);
                buffer[i++] = (byte)(w >> 16);
                buffer[i++] = (byte)(w >> 24);
            }

            // Fill up any remaining bytes in the buffer.
            if (i < buffer.Length)
            {
                // Generate 4 bytes.
                t = (x ^ (x << 11));
                x = y; y = z; z = w;
                w = (w ^ (w >> 19)) ^ (t ^ (t >> 8));

                buffer[i++] = (byte)w;
                if (i < buffer.Length)
                {
                    buffer[i++] = (byte)(w >> 8);
                    if (i < buffer.Length)
                    {
                        buffer[i++] = (byte)(w >> 16);
                        if (i < buffer.Length)
                        {
                            buffer[i] = (byte)(w >> 24);
                        }
                    }
                }
            }
            this._x = x;
            this._y = y;
            this._z = z;
            this._w = w;
            return buffer;
        }
        
        /// <summary>
        /// Gets an int in the given boundaries.
        /// The method is appr. 2.54x times faster than <see cref="System.Random.Next(int,int)"/>.
        /// </summary>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <returns>Random generated value</returns>
        /// <exception cref="ArgumentOutOfRangeException"><param name="upperBound"/> must be greater than <param name="lowerBound"/></exception>
        public int GetInt(int lowerBound, int upperBound)
        {
            uint t = _x ^ (_x << 11);
            _x = _y; _y = _z; _z = _w;
            // The explicit int cast before the first multiplication gives better performance.
            // Here we can gain a 2x speed improvement by generating a value that can be cast to 
            // an int instead of the more easily available uint. If we then explicitly cast to an 
            // int the compiler will then cast the int to a double to perform the multiplication, 
            // this final cast is a lot faster than casting from a uint to a double. The extra cast
            // to an int is very fast (the allocated bits remain the same) and so the overall effect 
            // of the extra cast is a significant performance improvement.
            // Also note that the loss of one bit of precision is equivalent to what occurs within 
            // System.Random.
            int range = upperBound - lowerBound;
            if (range < 0)
            {   // If range is <0 then an overflow has occured and must resort to using long integer arithmetic instead (slower).
                // We also must use all 32 bits of precision, instead of the normal 31, which again is slower.  
                return lowerBound + (int)((RealUnitUint * (double)(_w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8)))) * (double)((long)upperBound - (long)lowerBound));
            }
            // 31 bits of precision will suffice if range<=int.MaxValue. This allows us to cast to an int and gain
            // a little more performance.
            return lowerBound + (int)((RealUnitInt * (double)(int)(0x7FFFFFFF & (_w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8))))) * (double)range);
        }
        /// <summary>
        /// Generates a single random bit.
        /// This method's performance is improved by generating 32 bits in one operation and storing them
        /// ready for future calls.
        /// </summary>
        public bool GetBool()
        {
            if (_bitMask == 0)
            {
                // Generate 32 more bits.
                uint t = _x ^ (_x << 11);
                _x = _y; _y = _z; _z = _w;
                _bitBuffer = _w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8));
                // Reset the bitMask that tells us which bit to read next.
                _bitMask = 0x80000000;
                return (_bitBuffer & _bitMask) == 0;
            }
            return (_bitBuffer & (_bitMask >>= 1)) == 0;
        }
        /// <summary>
        /// Generates a random int over the range 0 to int.MaxValue-1.
        /// MaxValue is not generated in order to remain functionally equivalent to System.Random.Next().
        /// This does slightly eat into some of the performance gain over System.Random, but not much.
        /// For better performance see:
        /// 
        /// Call <see cref="GetInt()"/> for an int over the range 0 to <see cref="int.MaxValue"/>.
        /// </summary>
        private int Next()
        {
            uint t = _x ^ (_x << 11);
            _x = _y; _y = _z; _z = _w;
            _w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8));

            // Handle the special case where the value int.MaxValue is generated. This is outside of 
            // the range of permitted values, so we therefore call Next() to try again.
            uint rtn = _w & 0x7FFFFFFF;
            if (rtn == 0x7FFFFFFF)
            {
                return Next();
            }
            return (int)rtn;
        }

    }
}
