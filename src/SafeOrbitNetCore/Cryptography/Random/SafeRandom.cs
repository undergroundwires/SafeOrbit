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
using SafeOrbit.Random;

namespace SafeOrbit.Cryptography.Random
{
    /// <summary>
    ///     <p>
    ///         <see cref="SafeRandom" /> returns cryptographically strong random data, never to exceed the number of
    ///         bytes available from the specified entropy sources.  This can cause slow generation, and is recommended only
    ///         for generating extremely strong keys and other things that don't require a large number of bytes quickly.  This is
    ///         CPU intensive. For general purposes, see <see cref="FastRandom" /> instead.
    ///     </p>
    /// </summary>
    /// <remarks>
    ///     <p>Wrapper for <see cref="SafeRandomGenerator" /></p>
    /// </remarks>
    /// <seealso cref="FastRandom" />
    /// <seealso cref="RandomNumberGenerator" />
    /// <seealso cref="SafeRandomGenerator" />
    public class SafeRandom : RandomBase, ISafeRandom
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FastRandom" /> class.
        /// </summary>
        public SafeRandom() : base(SafeRandomGenerator.StaticInstance)
        {
        }
    }
}