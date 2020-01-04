using System;
using System.Security.Cryptography;
using SafeOrbit.Cryptography.Random.RandomGenerators;

namespace SafeOrbit.Cryptography.Random
{
    /// <summary>
    /// Helper methods for the implementations.
    /// </summary>
    public abstract class RandomBase : ICryptoRandom
    {
        private readonly RandomNumberGenerator _generator;
        protected RandomBase() : this(SafeRandomGenerator.StaticInstance)
        {
        }
        internal RandomBase(RandomNumberGenerator generator)
        {
            _generator = generator;
        }

        /// <inheritdoc />
        public byte[] GetBytes(int length)
        {
            var data = new byte[length];
            _generator.GetBytes(data);
            return data;
        }

        /// <inheritdoc />
        public int GetInt()
        {
            var scale = uint.MaxValue;
            //The code then divides scale by the uint.MaxValue.
            //This produces a value between 0.0 and 1.0, not including 1.0.
            //(This is why I didn’t want scale to be uint.MaxValue–so the result
            //of this division would be less than 1.0.) It multiplies this value
            //by the difference between the maximum and minimum desired values and
            //adds the result to the minimum value.
            while (scale == uint.MaxValue)
            {
                // Get four random bytes.
                var fourBytes = GetBytes(4);
                // Convert that into an uint.
                scale = BitConverter.ToUInt32(fourBytes, 0);
            }
            return (int)scale;
        }

#if !NETSTANDARD1_6
        /// <inheritdoc />
        public byte[] GetNonZeroBytes(int length)
        {
            var data = new byte[length];
            _generator.GetNonZeroBytes(data);
            return data;
        }
#endif

        /// <inheritdoc />
        public int GetInt(int min, int max)
        {
            if (min == max)
                return min;
            var scale = uint.MaxValue;
            //The code then divides scale by the uint.MaxValue.
            //This produces a value between 0.0 and 1.0, not including 1.0.
            //(This is why I didn’t want scale to be uint.MaxValue–so the result
            //of this division would be less than 1.0.) It multiplies this value
            //by the difference between the maximum and minimum desired values and
            //adds the result to the minimum value.
            while (scale == uint.MaxValue)
            {
                // Get four random bytes.
                var fourBytes = GetBytes(4);
                // Convert that into an uint.
                scale = BitConverter.ToUInt32(fourBytes, 0);
            }

            // Add min to the scaled difference between max and min.
            return (int)(min + (max - min) *
                          (scale / (double)uint.MaxValue));
        }

        /// <inheritdoc />
        public bool GetBool()
        {
            var data = new byte[1];
            _generator.GetBytes(data);
            var @byte = data[0];
            return (@byte & 0x80) != 0;
        }
    }
}
