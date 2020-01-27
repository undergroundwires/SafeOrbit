using System.Linq;
using Moq;
using SafeOrbit.Memory;
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
            fake.Setup(x => x.GenerateManyAsync(It.IsAny<SafeMemoryStream>()))
                .ReturnsAsync((SafeMemoryStream stream) =>
                {
                    var bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, (int)stream.Length);
                    return bytes.Select(b=> (int)b);
                });
            return fake.Object;
        }
    }
}