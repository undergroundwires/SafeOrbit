﻿using System;
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