
using System;
using System.Security.Cryptography;

namespace SafeOrbit.Cryptography.Random.RandomGenerators
{
    /// <summary>
    /// This is just a wrapper around <see cref="RNGCryptoServiceProvider" /> and adds no value 
    /// whatsoever, except to serve as a placeholder in the "EntropySources" folder, just to remember to consider
    /// RNGCryptoServiceProvider explicitly as an entropy source.
    /// </summary>
    /// <seealso cref="RandomNumberGenerator" />
    public sealed class SystemRng : RandomNumberGenerator
    {
        private readonly RandomNumberGenerator _systemRngProvider;
        public bool IsDisposed { get; private set; }

        public SystemRng() : this(RandomNumberGenerator.Create()) { }

        /// <exception cref="ArgumentNullException"><paramref name="systemRngProvider"/> is <see langword="null" />.</exception>
        internal SystemRng(RandomNumberGenerator systemRngProvider)
        {
            if (systemRngProvider == null) throw new ArgumentNullException(nameof(systemRngProvider));
            _systemRngProvider = systemRngProvider;
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
            if (this.IsDisposed) return;
            _systemRngProvider.Dispose();
            base.Dispose(disposing);
            this.IsDisposed = true;
        }
        ~SystemRng()
        {
            Dispose(false);
        }
    }
}
