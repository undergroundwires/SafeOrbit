using System;
using Moq;
using SafeOrbit.Exceptions;
using SafeOrbit.Memory.SafeBytesServices.DataProtection.Protector;
using SafeOrbit.Tests;

namespace SafeOrbit.Fakes
{
    /// <seealso cref="IByteArrayProtector" />
    internal class ByteArrayProtectorFaker : StubProviderBase<IByteArrayProtector>
    {
        public override IByteArrayProtector Provide()
        {
            const int blockSize = 16;
            var fake = new Mock<IByteArrayProtector>();
            const byte xorConstant = 0x53;
            fake.Setup(x => x.BlockSizeInBytes).Returns(blockSize);
            fake.Setup(x => x.Protect(It.IsAny<byte[]>()))
                .Callback<byte[]>((bytes) =>
                {
                    if(bytes.Length % blockSize != 0) throw new ArgumentException();
                    for (var i = 0; i < bytes.Length; i++)
                        bytes[i] = (byte)(bytes[i] ^ xorConstant);

                });
            fake.Setup(x => x.Unprotect(It.IsAny<byte[]>()))
                .Callback<byte[]>((bytes) =>
                {
                    if (bytes.Length % blockSize != 0) throw new ArgumentException();
                    for (var i = 0; i < bytes.Length; i++)
                        bytes[i] = (byte)(bytes[i] ^ xorConstant);
                });
            return fake.Object;
        }
    }
}