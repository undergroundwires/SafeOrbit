using System;
using System.Collections.Generic;
using System.Text;
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
            mock.Setup(m => m.BlockSizeInBytes).Returns(16);
            mock.Setup(m => m.IsInitialized).Returns(() => bytes != null);
            mock.Setup(m => m.Initialize(It.IsAny<byte[]>()))
                .Callback((byte[] rawBytes) =>
                {
                    if (bytes != null) throw new ArgumentException("Already initialized");
                    bytes = rawBytes;
                });
            var sessionMock = new Mock<IDecryptionSession>();
            sessionMock.Setup(s => s.PlainBytes)
                .Returns(() => bytes);
            mock.Setup(m => m.RevealDecryptedBytes())
                .Returns(() =>
                {
                    if (bytes == null) throw new ArgumentException("Not yet initialized");
                    return sessionMock.Object;
                });
            return mock.Object;
        }
    }
}
