using Moq;
using SafeOrbit.Memory;
using SafeOrbit.Memory.SafeBytesServices.DataProtection;
using SafeOrbit.Tests;

namespace SafeOrbit.UnitTests
{
    internal class MemoryProtectorFaker : StubProviderBase<IByteArrayProtector>
    {
        public override IByteArrayProtector Provide()
        {
            var fake = new Mock<IByteArrayProtector>();
            fake.Setup(x => x.BlockSize).Returns(16);
            return fake.Object;
        }
    }
}