
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
using System.Security.Cryptography;

namespace SafeOrbit.Random.Tinhat
{
    /// <summary>
    /// This is just a wrapper around <see cref="RNGCryptoServiceProvider" /> and adds no value 
    /// whatsoever, except to serve as a placeholder in the "EntropySources" folder, just to remember to consider
    /// RNGCryptoServiceProvider explicitly as an entropy source.
    /// </summary>
    /// <seealso cref="System.Security.Cryptography.RandomNumberGenerator" />
    /// <seealso cref="SafeOrbit.Random.Library.EntropySources.IEntropySource" />
    public sealed class SystemRngCryptoServiceProvider : RandomNumberGenerator
    {
        private readonly RandomNumberGenerator _systemRngProvider;
        public bool IsDisposed { get; private set; }
        public SystemRngCryptoServiceProvider() : this(new RNGCryptoServiceProvider()) { }
        /// <exception cref="ArgumentNullException"><paramref name="systemRngProvider"/> is <see langword="null" />.</exception>
        internal SystemRngCryptoServiceProvider(RandomNumberGenerator systemRngProvider)
        {
            if (systemRngProvider == null) throw new ArgumentNullException(nameof(systemRngProvider));
            _systemRngProvider = systemRngProvider;
        }
        public override void GetBytes(byte[] data)
        {
            _systemRngProvider.GetBytes(data);
        }
        public override void GetNonZeroBytes(byte[] data)
        {
            _systemRngProvider.GetNonZeroBytes(data);
        }
        protected override void Dispose(bool disposing)
        {
            if (this.IsDisposed) return;
            _systemRngProvider.Dispose();
            base.Dispose(disposing);
            this.IsDisposed = true;
        }
        ~SystemRngCryptoServiceProvider()
        {
            Dispose(false);
        }
    }
}
