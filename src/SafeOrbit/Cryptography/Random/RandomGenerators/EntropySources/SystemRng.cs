
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
using System.Security.Cryptography;

namespace SafeOrbit.Cryptography.Random.RandomGenerators
{
    /// <summary>
    ///     This is just a wrapper around random generator from <see cref="System.Security.Cryptography" /> and adds no value
    ///     whatsoever.
    /// </summary>
    /// <seealso cref="RandomNumberGenerator" />
    public sealed class SystemRng : RandomNumberGenerator
    {
        private readonly RandomNumberGenerator _systemRngProvider;
        public bool IsDisposed { get; private set; }

        public SystemRng() : this(Create())
        {
        }

        /// <exception cref="ArgumentNullException"><paramref name="systemRngProvider" /> is <see langword="null" />.</exception>
        internal SystemRng(RandomNumberGenerator systemRngProvider)
        {
            _systemRngProvider = systemRngProvider ?? throw new ArgumentNullException(nameof(systemRngProvider));
        }

        public override void GetBytes(byte[] data)
        {
            _systemRngProvider.GetBytes(data);
        }

#if !NETCORE
        public override void GetNonZeroBytes(byte[] data)
        {
            _systemRngProvider.GetNonZeroBytes(data);
        }
#endif

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed) return;
            _systemRngProvider.Dispose();
            base.Dispose(disposing);
            IsDisposed = true;
        }

        ~SystemRng()
        {
            Dispose(false);
        }
    }
}