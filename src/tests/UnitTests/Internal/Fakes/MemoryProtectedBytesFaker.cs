using System;
using Moq;
using SafeOrbit.Memory.SafeBytesServices.DataProtection;
using SafeOrbit.Tests;

namespace UnitTests.Internal.Fakes
{
    /// <seealso cref="IMemoryProtectedBytes" />
    /// <seealso cref="MemoryProtectedBytes" />
    public class MemoryProtectedBytesFaker : StubProviderBase<IMemoryProtectedBytes>
    {
        public override IMemoryProtectedBytes Provide()
        {
            var mock = new Mock<IMemoryProtectedBytes>();
            byte[] bytes = null;
            mock.SetupGet(m => m.BlockSizeInBytes).Returns(16);
            mock.Setup(m => m.IsInitialized).Returns(() => bytes != null);
            mock.Setup(m => m.InitializeAsync(It.IsAny<byte[]>()))
                .Callback((byte[] rawBytes) =>
                {
                    if (bytes != null) throw new ArgumentException("Already initialized");
                    bytes = rawBytes;
                });
            var sessionMock = new Mock<IDecryptedBytesMarshaler>();
            sessionMock.Setup(s => s.PlainBytes)
                .Returns(() => bytes);
            mock.Setup(m => m.RevealDecryptedBytesAsync())
                .ReturnsAsync(() =>
                {
                    if (bytes == null) throw new ArgumentException("Not yet initialized");
                    return sessionMock.Object;
                });
            mock.Setup(m => m.DeepClone())
                .Returns(() => mock.Object);
            return mock.Object;
        }
    }
}