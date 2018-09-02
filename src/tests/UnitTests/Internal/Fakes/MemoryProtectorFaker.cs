using Moq;
using SafeOrbit.Memory.SafeBytesServices.DataProtection;
using SafeOrbit.Tests;

namespace SafeOrbit.Fakes
{
    /// <seealso cref="IByteArrayProtector" />
    internal class MemoryProtectorFaker : StubProviderBase<IByteArrayProtector>
    {
        public override IByteArrayProtector Provide()
        {
            var fake = new Mock<IByteArrayProtector>();
            fake.Setup(x => x.BlockSizeInBytes).Returns(16);
            return fake.Object;
        }
    }
}