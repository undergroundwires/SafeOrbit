using Moq;
using SafeOrbit.Encryption.Kdf;
using SafeOrbit.Tests;

namespace SafeOrbit.UnitTests
{
    public class KeyDeriverFaker : StubProviderBase<IKeyDerivationFunction>
    {
        public override IKeyDerivationFunction Provide()
        {
            var fake = new Mock<IKeyDerivationFunction>();
            fake.Setup(f => f.Derive(It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<int>()))
                .Returns((byte[] key, byte[] salt, int length) => new byte[length]);
            return fake.Object;
        }
    }
}