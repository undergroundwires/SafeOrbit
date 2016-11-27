
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

using System.Security.Cryptography;
using SafeOrbit.Cryptography.Random.RandomGenerators;

namespace SafeOrbit.Cryptography.Random
{
    /// <summary>
    ///     <p>
    ///         <see cref="FastRandom" /> returns cryptographically strong random data. It's about 1000x times faster than
    ///         <see cref="SafeRandom" />
    ///     </p>
    ///     <p>
    ///         For general purposes, <see cref="FastRandom" /> is recommended because of its performance
    ///         characteristics, but for extremely strong keys and other things that don't require a large number of bytes
    ///         quickly,  <see cref="SafeRandom" /> is recommended instead.
    ///     </p>
    /// </summary>
    /// <remarks>
    ///     <p>Wrapper for <see cref="FastRandomGenerator" /></p>
    /// </remarks>
    /// <seealso cref="SafeRandom" />
    /// <seealso cref="RandomNumberGenerator" />
    /// <seealso cref="FastRandomGenerator" />
    public class FastRandom : RandomBase, IFastRandom
    {
        /// <summary>
        ///     Gets the static instance of <see cref="FastRandom"/>.
        /// </summary>
        public static IFastRandom StaticInstance = new FastRandom();
        /// <summary>
        ///     Initializes a new instance of the <see cref="FastRandom" /> class.
        /// </summary>
        public FastRandom() : base(FastRandomGenerator.StaticInstance)
        {
        }
    }
}