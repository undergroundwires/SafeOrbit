using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SafeOrbit.Cryptography.Hashers
{
    /// <summary>
    ///     MurMurHash3 is not a cryptological hasher. It should be used when a very fast hashing is needed.
    /// </summary>
    public class Murmur32 : IFastHasher
    {
        private const uint DefaultSeed = 0xC58F1A7B;
        private static readonly Lazy<Murmur32> StaticInstanceLazy = new Lazy<Murmur32>();
        public static Murmur32 StaticInstance => StaticInstanceLazy.Value;

        /// <exception cref="ArgumentNullException"><paramref name="input" /> is <see langword="null" />.</exception>
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ComputeFast(byte[] input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            const uint c1 = 0xCC9E2D51, c2 = 0x1B873593, c3 = 0xE6546B64, c4 = 0x85EBCA6B, c5 = 0xC2B2AE35;

            var currentIndex = 0;
            var length = input.Length;
            var h = DefaultSeed;

            while (length >= 4)
            {
                var k = input[currentIndex] |
                        ((uint) input[currentIndex + 1] << 8) |
                        ((uint) input[currentIndex + 2] << 16) |
                        ((uint) input[currentIndex + 3] << 24);

                k *= c1;
                k = (k << 15) | (k >> 17);
                k *= c2;

                h ^= k;
                h = (h << 13) | (h >> 19);
                h = h * 5 + c3;

                currentIndex += 4;
                length -= 4;
            }

            switch (length)
            {
                case 3:
                {
                    var k = ((uint) input[currentIndex + 2] << 16) | ((uint) input[currentIndex + 1] << 8) |
                            input[currentIndex];
                    k *= c1;
                    k = (k << 15) | (k >> 17);
                    k *= c2;
                    h ^= k;
                    break;
                }
                case 2:
                {
                    var k = ((uint) input[currentIndex + 1] << 8) | input[currentIndex];
                    k *= c1;
                    k = (k << 15) | (k >> 17);
                    k *= c2;
                    h ^= k;
                    break;
                }
                case 1:
                {
                    var k = (uint) input[currentIndex];
                    k *= c1;
                    k = (k << 15) | (k >> 17);
                    k *= c2;
                    h ^= k;
                    break;
                }
            }

            h ^= (uint) input.Length;

            h ^= h >> 16;
            h *= c4;
            h ^= h >> 13;
            h *= c5;
            h ^= h >> 16;

            var result = unchecked((int) h);
            return result;
        }

        /// <exception cref="ArgumentNullException"><paramref name="input" /> is <see langword="null" />.</exception>
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ComputeFast(byte[] input, uint seed)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            const uint c1 = 0xCC9E2D51, c2 = 0x1B873593, c3 = 0xE6546B64, c4 = 0x85EBCA6B, c5 = 0xC2B2AE35;

            var currentIndex = 0;
            var length = input.Length;
            var h = seed;

            while (length >= 4)
            {
                var k = input[currentIndex] |
                        ((uint) input[currentIndex + 1] << 8) |
                        ((uint) input[currentIndex + 2] << 16) |
                        ((uint) input[currentIndex + 3] << 24);

                k *= c1;
                k = (k << 15) | (k >> 17);
                k *= c2;

                h ^= k;
                h = (h << 13) | (h >> 19);
                h = h * 5 + c3;

                currentIndex += 4;
                length -= 4;
            }

            switch (length)
            {
                case 3:
                {
                    var k = ((uint) input[currentIndex + 2] << 16) | ((uint) input[currentIndex + 1] << 8) |
                            input[currentIndex];
                    k *= c1;
                    k = (k << 15) | (k >> 17);
                    k *= c2;
                    h ^= k;
                    break;
                }
                case 2:
                {
                    var k = ((uint) input[currentIndex + 1] << 8) | input[currentIndex];
                    k *= c1;
                    k = (k << 15) | (k >> 17);
                    k *= c2;
                    h ^= k;
                    break;
                }
                case 1:
                {
                    var k = (uint) input[currentIndex];
                    k *= c1;
                    k = (k << 15) | (k >> 17);
                    k *= c2;
                    h ^= k;
                    break;
                }
            }

            ;

            h ^= (uint) input.Length;

            h ^= h >> 16;
            h *= c4;
            h ^= h >> 13;
            h *= c5;
            h ^= h >> 16;

            var result = unchecked((int) h);
            return result;
        }
    }
}