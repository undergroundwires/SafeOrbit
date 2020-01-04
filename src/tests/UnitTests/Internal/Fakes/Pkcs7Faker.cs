using System.Linq;
using Moq;
using SafeOrbit.Cryptography.Encryption.Padding.Padders;
using SafeOrbit.Tests;

namespace SafeOrbit.Fakes
{
    /// <seealso cref="IPkcs7Padder" />
    internal class Pkcs7Faker : StubProviderBase<IPkcs7Padder>
    {
        public override IPkcs7Padder Provide()
        {
            var fake = new Mock<IPkcs7Padder>();
            fake.Setup(f => f.Pad(It.IsAny<byte[]>(), It.IsAny<int>()))
                .Returns<byte[], int>((bytes, length) => bytes.Concat(new byte[length].Select(b=> (byte)length)).ToArray());
            fake.Setup(f => f.Unpad(It.IsAny<byte[]>()))
                .Returns<byte[]>((bytes) => bytes.Take(bytes.Length - bytes[bytes.Length - 1]).ToArray());
            return fake.Object;
        }
    }
}