using System;
using SafeOrbit.Cryptography.Encryption.Padding.Factory;
using SafeOrbit.Cryptography.Encryption.Padding.Padders;
using SafeOrbit.Cryptography.Random;

namespace SafeOrbit.Cryptography.Encryption.Padding
{
    public abstract class PaddedEncryptorBase: EncryptorBase, IPaddedEncryptor
    {
        private readonly IPadderFactory _factory;

        /// <exception cref="ArgumentNullException"><paramref name="factory"/> is <see langword="null"/></exception>
        protected PaddedEncryptorBase(ICryptoRandom random, PaddingMode padding, IPadderFactory factory) : base(random)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            Padding = padding;
        }
        public PaddingMode Padding { get; set; }
        protected byte[] AddPadding(byte[] input)
        {
            if (this.Padding == PaddingMode.None)
                return input;
            var blockSizeInBytes = BlockSizeInBits / 8;
            var length = blockSizeInBytes - input.Length % blockSizeInBytes;
            return _factory.GetPadder(Padding).Pad(input, length);
        }
        protected byte[] Unpad(byte[] decrypted)
        {
            return this.Padding == PaddingMode.None ? decrypted
                : _factory.GetPadder(Padding).Unpad(decrypted);
        }
    }
}
