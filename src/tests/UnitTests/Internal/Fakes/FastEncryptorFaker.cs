using System;
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
            byte[] Encrypt(byte[] input, byte[] key) =>
                input.Concat(key).Concat(BitConverter.GetBytes(Convert.ToBase64String(input).GetHashCode())).ToArray();
            byte[] Decrypt(byte[] input, byte[] key)
            {
                var plainLength = input.Length - key.Length - sizeof(int);
                var rawBytes = input.Take(plainLength).ToArray();
                var keyBytes = input.Skip(plainLength).Take(key.Length).ToArray();
                if (!key.SequenceEqual(keyBytes))
                    throw new ArgumentException(
                        $"Wrong encryption key.{Environment.NewLine}" +
                        $"Expected was:{Environment.NewLine}" +
                        string.Join(",", key.Select(b => b)) + Environment.NewLine +
                        $"But got: {Environment.NewLine}" +
                        string.Join(",", keyBytes.Select(b => b)));
                return rawBytes;
            }

            var fake = new Mock<IFastEncryptor>();
            fake.Setup(f => f.Encrypt(It.IsAny<byte[]>(), It.IsAny<byte[]>()))
                .Returns<byte[],byte[]>(Encrypt);
            fake.Setup(f => f.Decrypt(It.IsAny<byte[]>(), It.IsAny<byte[]>()))
                .Returns<byte[], byte[]>(Decrypt);
            fake.Setup(f => f.EncryptAsync(It.IsAny<byte[]>(), It.IsAny<byte[]>()))
                .ReturnsAsync((byte[] i, byte[] k) => Encrypt(i, k));
            fake.Setup(f => f.DecryptAsync(It.IsAny<byte[]>(), It.IsAny<byte[]>()))
                .ReturnsAsync((byte[] i, byte[] k) => Decrypt(i, k));
            return fake.Object;
        }
    }
}