using Moq;
using SafeOrbit.Memory.SafeBytesServices.Id;
using SafeOrbit.Tests;

namespace SafeOrbit.Fakes
{
    /// <seealso cref="IByteIdGenerator" />
    /// <seealso cref="HashedByteIdGenerator" />
    /// <seealso cref="HashedByteIdGeneratorTests" />
    internal class ByteIdGeneratorFaker : StubProviderBase<IByteIdGenerator>
    {
        public override IByteIdGenerator Provide()
        {
            var fake = new Mock<IByteIdGenerator>();
            fake.Setup(x => x.GenerateAsync(It.IsAny<byte>()))
                .ReturnsAsync((byte b) => b);
            return fake.Object;
        }
    }
}