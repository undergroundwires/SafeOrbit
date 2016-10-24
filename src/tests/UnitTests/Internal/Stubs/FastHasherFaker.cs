using Moq;
using SafeOrbit.Hash;
using SafeOrbit.Tests;

namespace SafeOrbit.UnitTests
{
    public class FastHasherFaker : StubProviderBase<IFastHasher>
    {
        public override IFastHasher Provide()
        {
            var fake = new Mock<IFastHasher>();
            fake.Setup(o => o.ComputeFast(It.IsAny<byte[]>())).Returns((byte[] bytes) => bytes[0] + bytes.Length * 5);
            return fake.Object;
        }
    }
}