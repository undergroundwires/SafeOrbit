
/*
MIT License

Copyright (c) 2016-2018 Erkin Ekici - undergroundwires@safeorb.it

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
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SafeOrbit.Cryptography.Hashers
{
    /// <summary>
    /// MurMurHash3 is not a cryptological hasher. It should be used when a very fast hashing is needed.
    /// </summary>
    public class Murmur32 : IFastHasher
    {
        public static Murmur32 StaticInstance => StaticInstanceLazy.Value;
        private static readonly Lazy<Murmur32> StaticInstanceLazy = new Lazy<Murmur32>();
        private const uint DefaultSeed = 0xC58F1A7B;

        /// <exception cref="ArgumentNullException"><paramref name="input"/> is <see langword="null" />.</exception>
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ComputeFast(byte[] input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            var m_h = DefaultSeed;
            const uint c1 = 0xCC9E2D51, c2 = 0x1B873593, c3 = 0xE6546B64, c4 = 0x85EBCA6B, c5 = 0xC2B2AE35;

            var currentIndex = 0;
            var length = input.Length;

            while (length >= 4)
            {
                var k = (uint)input[currentIndex] |
                          ((uint)input[currentIndex + 1] << 8) |
                          ((uint)input[currentIndex + 2] << 16) |
                          ((uint)input[currentIndex + 3] << 24);

                k *= c1;
                k = (k << 15) | (k >> 17);
                k *= c2;

                m_h ^= k;
                m_h = (m_h << 13) | (m_h >> 19);
                m_h = m_h * 5 + c3;

                currentIndex += 4;
                length -= 4;
            }

            switch (length)
            {
                case 3:
                    {
                        var k = (uint)input[currentIndex + 2] << 16 | ((uint)input[currentIndex + 1] << 8) | (uint)input[currentIndex];
                        k *= c1;
                        k = (k << 15) | (k >> 17);
                        k *= c2;
                        m_h ^= k;
                        break;
                    }
                case 2:
                    {
                        var k = ((uint)input[currentIndex + 1] << 8) | (uint)input[currentIndex];
                        k *= c1;
                        k = (k << 15) | (k >> 17);
                        k *= c2;
                        m_h ^= k;
                        break;
                    }
                case 1:
                    {
                        var k = (uint)input[currentIndex];
                        k *= c1;
                        k = (k << 15) | (k >> 17);
                        k *= c2;
                        m_h ^= k;
                        break;
                    }
            };

            m_h ^= (uint)input.Length;

            m_h ^= m_h >> 16;
            m_h *= c4;
            m_h ^= m_h >> 13;
            m_h *= c5;
            m_h ^= m_h >> 16;

            var result = unchecked((int)m_h);
            return result;
        }

        /// <exception cref="ArgumentNullException"><paramref name="input"/> is <see langword="null" />.</exception>
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ComputeFast(byte[] input, uint seed)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            var m_h = seed;
            const uint c1 = 0xCC9E2D51, c2 = 0x1B873593, c3 = 0xE6546B64, c4 = 0x85EBCA6B, c5 = 0xC2B2AE35;

            var currentIndex = 0;
            var length = input.Length;

            while (length >= 4)
            {
                var k = (uint)input[currentIndex] |
                          ((uint)input[currentIndex + 1] << 8) |
                          ((uint)input[currentIndex + 2] << 16) |
                          ((uint)input[currentIndex + 3] << 24);

                k *= c1;
                k = (k << 15) | (k >> 17);
                k *= c2;

                m_h ^= k;
                m_h = (m_h << 13) | (m_h >> 19);
                m_h = m_h * 5 + c3;

                currentIndex += 4;
                length -= 4;
            }

            switch (length)
            {
                case 3:
                    {
                        var k = (uint)input[currentIndex + 2] << 16 | ((uint)input[currentIndex + 1] << 8) | (uint)input[currentIndex];
                        k *= c1;
                        k = (k << 15) | (k >> 17);
                        k *= c2;
                        m_h ^= k;
                        break;
                    }
                case 2:
                    {
                        var k = ((uint)input[currentIndex + 1] << 8) | (uint)input[currentIndex];
                        k *= c1;
                        k = (k << 15) | (k >> 17);
                        k *= c2;
                        m_h ^= k;
                        break;
                    }
                case 1:
                    {
                        var k = (uint)input[currentIndex];
                        k *= c1;
                        k = (k << 15) | (k >> 17);
                        k *= c2;
                        m_h ^= k;
                        break;
                    }
            };

            m_h ^= (uint)input.Length;

            m_h ^= m_h >> 16;
            m_h *= c4;
            m_h ^= m_h >> 13;
            m_h *= c5;
            m_h ^= m_h >> 16;

            var result = unchecked((int)m_h);
            return result;
        }
    }
}