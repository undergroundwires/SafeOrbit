using Moq;
using SafeOrbit.Memory.SafeBytesServices.DataProtection.Protector;
using SafeOrbit.Tests;

namespace SafeOrbit.Fakes
{
    /// <seealso cref="IByteArrayProtector" />
    internal class ByteArrayProtectorFaker : StubProviderBase<IByteArrayProtector>
    {
        public override IByteArrayProtector Provide()
        {
            var fake = new Mock<IByteArrayProtector>();
            const byte xorConstant = 0x53;
            fake.Setup(x => x.BlockSizeInBytes).Returns(16);
            fake.Setup(x => x.Protect(It.IsAny<byte[]>()))
                .Callback<byte[]>((bytes) =>
                {
                    for (var i = 0; i < bytes.Length; i++)
                        bytes[i] = (byte)(bytes[i] ^ xorConstant);

                });
            fake.Setup(x => x.Unprotect(It.IsAny<byte[]>()))
                .Callback<byte[]>((bytes) =>
                {
                    for (var i = 0; i < bytes.Length; i++)
                        bytes[i] = (byte)(bytes[i] ^ xorConstant);
                });
            return fake.Object;
        }
    }
}