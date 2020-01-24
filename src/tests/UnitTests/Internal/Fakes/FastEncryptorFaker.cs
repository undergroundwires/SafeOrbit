using System.Linq;
using Moq;
using SafeOrbit.Cryptography.Encryption;
using SafeOrbit.Tests;

namespace SafeOrbit.Fakes
{
    /// <seealso cref="IFastEncryptor" />
    public class FastEncryptorFaker : StubProviderBase<IFastEncryptor>
    {
        public override IFastEncryptor Provide()
        {
            static byte[] Encrypt(byte[] input, byte[] key) =>
                input.Select(b => (byte) (b ^ (key.Sum(k => k) + 1))).ToArray();

            static byte[] Decrypt(byte[] input, byte[] key) =>
                input.Select(b => (byte) (b ^ (key.Sum(k => k) + 1))).ToArray();

            var fake = new Mock<IFastEncryptor>();
            fake.Setup(f => f.EncryptAsync(It.IsAny<byte[]>(), It.IsAny<byte[]>()))
                .ReturnsAsync((byte[] i, byte[] k) => Encrypt(i, k));
            fake.Setup(f => f.DecryptAsync(It.IsAny<byte[]>(), It.IsAny<byte[]>()))
                .ReturnsAsync((byte[] i, byte[] k) => Decrypt(i, k));
            fake.SetupGet(f => f.BlockSizeInBits)
                .Returns(32);
            return fake.Object;
        }
    }
}