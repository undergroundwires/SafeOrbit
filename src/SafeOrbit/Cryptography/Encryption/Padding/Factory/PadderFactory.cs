using System;
using SafeOrbit.Cryptography.Encryption.Padding.Padders;
using SafeOrbit.Library;

namespace SafeOrbit.Cryptography.Encryption.Padding.Factory
{
    public class PadderFactory : IPadderFactory
    {
        private readonly IFactory<IPkcs7Padder> _pkcs7Factory;

        public PadderFactory() : this(SafeOrbitCore.Current.Factory.Get<IFactory<IPkcs7Padder>>())
        {
        }

        public PadderFactory(IFactory<IPkcs7Padder> pkcs7Factory)
        {
            _pkcs7Factory = pkcs7Factory ?? throw new ArgumentNullException(nameof(pkcs7Factory));
        }

        /// <exception cref="ArgumentOutOfRangeException">Unknown <paramref name="paddingMode" /> value.</exception>
        public IPadder GetPadder(PaddingMode paddingMode)
        {
            return paddingMode switch
            {
                PaddingMode.PKCS7 => _pkcs7Factory.Create(),
                _ => throw new ArgumentOutOfRangeException(nameof(paddingMode), paddingMode, null)
            };
        }
    }
}