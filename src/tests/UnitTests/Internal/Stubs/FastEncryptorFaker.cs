using System.Linq;
using Moq;
using SafeOrbit.Encryption;
using SafeOrbit.Tests;

namespace SafeOrbit.UnitTests
{
    public class FastEncryptorFaker : StubProviderBase<IFastEncryptor>
    {
        public override IFastEncryptor Provide()
        {
            var fake = new Mock<IFastEncryptor>();
            fake.Setup(f => f.Encrypt(It.IsAny<byte[]>(), It.IsAny<byte[]>()))
                .Returns((byte[] input, byte[] key) => input.ToArray());
            fake.Setup(f => f.Decrypt(It.IsAny<byte[]>(), It.IsAny<byte[]>()))
                .Returns((byte[] input, byte[] key) => input.ToArray());
            return fake.Object;
        }
    }
}