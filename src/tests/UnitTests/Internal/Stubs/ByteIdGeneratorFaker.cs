using Moq;
using SafeOrbit.Memory;
using SafeOrbit.Memory.SafeBytesServices.Id;
using SafeOrbit.Tests;

namespace SafeOrbit.UnitTests
{
    /// <seealso cref="IByteIdGenerator" />
    /// <seealso cref="HashedByteIdGenerator" />
    /// <seealso cref="HashedByteIdGeneratorTests" />
    internal class ByteIdGeneratorFaker : StubProviderBase<IByteIdGenerator>
    {
        public override IByteIdGenerator Provide()
        {
            var fake = new Mock<IByteIdGenerator>();
            fake.Setup(x => x.Generate(It.IsAny<byte>())).Returns((byte b) => b);
            return fake.Object;
        }
    }
}