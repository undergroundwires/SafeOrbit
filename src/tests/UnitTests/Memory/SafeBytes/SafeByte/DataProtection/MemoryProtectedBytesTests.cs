using System;
using System.Linq;
using System.Security.Cryptography;
using Moq;
using NUnit.Framework;
using SafeOrbit.Extensions;
using SafeOrbit.Fakes;
using SafeOrbit.Memory.SafeBytesServices.DataProtection.Protector;
using SafeOrbit.Tests;

namespace SafeOrbit.Memory.SafeBytesServices.DataProtection
{
    /// <seealso cref="MemoryProtectedBytes" />
    /// <seealso cref="IMemoryProtectedBytes" />
    [TestFixture]
    public class MemoryProtectedBytesTests : TestsFor<IMemoryProtectedBytes>
    {
        [Test]
        public void Initialize_AlreadyInitialized_ThrowsException()
        {
            // Arrange
            var sut = GetSut();
            sut.Initialize(GetValidBytes());

            // Act
            void InitializeAgain() => sut.Initialize(GetValidBytes());

            // Assert
            Assert.Throws<InvalidOperationException>(InitializeAgain);
        }

        [Test]
        public void Initialize_DisposedInstance_ThrowsException()
        {
            // Arrange
            var sut = GetSut();
            sut.Dispose();

            // Act
            void Initialize() => sut.Initialize(GetValidBytes());

            // Assert
            Assert.Throws<ObjectDisposedException>(Initialize);
        }

        [Test]
        public void Initialize_NullBytes_ThrowsException()
        {
            // Arrange
            var sut = GetSut();
            var nullBytes = (byte[]) null;

            // Act
            void Initialize() => sut.Initialize(nullBytes);

            // Assert
            Assert.Throws<ArgumentNullException>(Initialize);
        }

        [Test]
        public void Initialize_EmptyBytes_ThrowsException()
        {
            // Arrange
            var sut = GetSut();
            var emptyBytes = new byte[0];

            // Act
            void Initialize() => sut.Initialize(emptyBytes);

            // Assert
            Assert.Throws<ArgumentException>(Initialize);
        }

        [Test]
        public void Initialize_BytesNotConformingToBlockSize_ThrowsException()
        {
            // Arrange
            var protectorMock = new Mock<IByteArrayProtector>();
            protectorMock.SetupGet(p => p.BlockSizeInBytes).Returns(3);
            var plainBytes = new byte[] {5, 5};
            using var sut = GetSut(protector: protectorMock.Object);

            // Act
            void Initialize() => sut.Initialize(plainBytes);

            // Assert
            Assert.Throws<CryptographicException>(Initialize);
        }

        [Test]
        public void Initialize_ValidBytes_ProtectsPlainBytes()
        {
            // Arrange
            var plainBytes = GetValidBytes();
            var protectorMock = new Mock<IByteArrayProtector>(MockBehavior.Strict);
            protectorMock.SetupGet(m => m.BlockSizeInBytes).Returns(plainBytes.Length);
            protectorMock.Setup(m => m.Protect(plainBytes)).Verifiable();
            using var sut = GetSut(protector: protectorMock.Object);

            // Act
            sut.Initialize(plainBytes);

            // Assert
            protectorMock.Verify(s => s.Protect(plainBytes), Times.Once);
        }

        [Test]
        public void BlockSizeInBytes_ForNewInstance_ReturnsFromInnerProtector()
        {
            // Arrange
            const int expected = 5;
            var protector = new Mock<IByteArrayProtector>();
            protector.SetupGet(p => p.BlockSizeInBytes).Returns(expected);
            using var sut = GetSut(protector: protector.Object);

            // Act
            var actual = sut.BlockSizeInBytes;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Dispose_InitializedInstance_ClearsAllBytes()
        {
            // Arrange
            const int expected = 5;
            var protector = new Mock<IByteArrayProtector>();
            protector.SetupGet(p => p.BlockSizeInBytes).Returns(expected);
            using var sut = GetSut(protector: protector.Object);

            // Act
            var actual = sut.BlockSizeInBytes;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Dispose_NotInitializedInstance_DoesNotThrow()
        {
            // Arrange
            var sut = GetSut();

            // Act
            void Dispose() => sut.Dispose();
            // Assert
            Assert.DoesNotThrow(Dispose);
        }

        [Test]
        public void IsDisposed_NotInitialized_ReturnsFalse()
        {
            // Arrange
            var sut = GetSut();
            // Act
            var actual = sut.IsDisposed;
            // Assert
            Assert.False(actual);
        }

        [Test]
        public void IsDisposed_Initialized_ReturnsFalse()
        {
            // Arrange
            var sut = GetSut();
            sut.Initialize(GetValidBytes());
            // Act
            var actual = sut.IsDisposed;
            // Assert
            Assert.False(actual);
        }

        [Test]
        public void IsDisposed_DisposedInstance_ReturnsTrue()
        {
            // Arrange
            var sut = GetSut();
            sut.Dispose();
            // Act
            var actual = sut.IsDisposed;
            // Assert
            Assert.True(actual);
        }

        [Test]
        public void IsInitialized_NewInstance_ReturnsFalse()
        {
            // Arrange
            var sut = GetSut();
            // Act
            var actual = sut.IsInitialized;
            // Assert
            Assert.False(actual);
        }

        [Test]
        public void IsInitialized_InitializedInstance_ReturnsTrue()
        {
            // Arrange
            var sut = GetSut();
            sut.Initialize(GetValidBytes());
            // Act
            var actual = sut.IsInitialized;
            // Assert
            Assert.True(actual);
        }

        [Test]
        public void IsInitialized_InitializedAndDisposedInstance_ReturnsTrue()
        {
            // Arrange
            var sut = GetSut();
            sut.Initialize(GetValidBytes());
            sut.Dispose();
            // Act
            var actual = sut.IsInitialized;
            // Assert
            Assert.True(actual);
        }

        [Test]
        public void RevealDecryptedBytes_NotInitialized_ThrowsException()
        {
            // Arrange
            var sut = GetSut();

            // Act
            void RevealDecryptedBytes() => sut.RevealDecryptedBytes();
            // Assert
            Assert.Throws<InvalidOperationException>(RevealDecryptedBytes);
        }

        [Test]
        public void RevealDecryptedBytes_DisposedInstance_ThrowsException()
        {
            // Arrange
            var sut = GetSut();
            sut.Initialize(GetValidBytes());
            sut.Dispose();

            // Act
            void RevealDecryptedBytes() => sut.RevealDecryptedBytes();
            // Assert
            Assert.Throws<ObjectDisposedException>(RevealDecryptedBytes);
        }

        [Test]
        public void RevealDecryptedBytes_NewInstance_ReturnsUnprotectedBytes()
        {
            // Arrange
            var expected = GetValidBytes();
            var sut = GetSut();
            sut.Initialize(expected.CopyToNewArray());
            // Act
            var session = sut.RevealDecryptedBytes();
            var actual = session.PlainBytes;
            // Assert
            Console.WriteLine("Expected:" + Environment.NewLine +
                              string.Join(", ", expected) + Environment.NewLine +
                              "Actual:" + Environment.NewLine +
                              string.Join(", ", actual));
            Assert.True(expected.SequenceEqual(actual));
        }

        [Test]
        public void RevealDecryptedBytes_RevealedTwiceWithoutDisposing_ReturnsUnprotectedBytes()
        {
            // Arrange
            var expected = GetValidBytes();
            var sut = GetSut();
            sut.Initialize(expected.CopyToNewArray());
            // Act
            using var firstSession = sut.RevealDecryptedBytes();
            using var secondSession = sut.RevealDecryptedBytes();
            var firstBytes = firstSession.PlainBytes;
            var secondBytes = secondSession.PlainBytes;
            // Assert
            Assert.True(expected.SequenceEqual(firstBytes));
            Assert.True(expected.SequenceEqual(secondBytes));
        }

        [Test]
        public void RevealDecryptedBytes_RevealedAndDisposedTwice_ReturnsUnprotectedBytes()
        {
            // Arrange
            var expected = GetValidBytes();
            var sut = GetSut();
            sut.Initialize(expected.CopyToNewArray());
            // Act
            byte[] firstBytes, secondBytes;
            using (var firstSession = sut.RevealDecryptedBytes())
            {
                firstBytes = firstSession.PlainBytes.CopyToNewArray();
            }

            using (var secondSession = sut.RevealDecryptedBytes())
            {
                secondBytes = secondSession.PlainBytes.CopyToNewArray();
            }

            // Assert
            Assert.True(expected.SequenceEqual(firstBytes));
            Assert.True(expected.SequenceEqual(secondBytes));
        }

        [Test]
        public void DeepClone_Disposed_Throws()
        {
            // Arrange
            var sut = GetSut();
            sut.Dispose();

            // Act
            void DeepClone() => sut.DeepClone();
            // Assert
            Assert.Throws<ObjectDisposedException>(DeepClone);
        }

        [Test]
        public void DeepClone_NotInitialized_Clones()
        {
            // Arrange
            var sut = GetSut();
            // Act
            var clone = sut.DeepClone();
            // Assert
            Assert.False(clone.IsInitialized);
            Assert.False(clone.IsDisposed);
        }

        [Test]
        public void DeepClone_Initialized_CloneHasEqualBytes()
        {
            // Arrange
            var expected = GetValidBytes();
            var sut = GetSut();
            sut.Initialize(expected.CopyToNewArray());
            // Act
            var clone = sut.DeepClone();
            var actual = clone.RevealDecryptedBytes().PlainBytes;
            // Assert
            Console.WriteLine("Expected:" + Environment.NewLine +
                              string.Join(", ", expected) + Environment.NewLine +
                              "Actual:" + Environment.NewLine +
                              string.Join(", ", actual));
            Assert.True(expected.SequenceEqual(actual));
        }

        [Test]
        public void DeepClone_OriginalIsDisposed_DoesNotAffectClone()
        {
            // Arrange
            var expected = GetValidBytes();
            var sut = GetSut();
            sut.Initialize(expected.CopyToNewArray());
            // Act
            var clone = sut.DeepClone();
            sut.Dispose();
            var actual = clone.RevealDecryptedBytes().PlainBytes;
            // Assert
            Console.WriteLine("Expected:" + Environment.NewLine +
                              string.Join(",", expected) + Environment.NewLine +
                              "Actual:" + Environment.NewLine +
                              string.Join(",", actual));
            Assert.True(expected.SequenceEqual(actual));
        }

        protected override IMemoryProtectedBytes GetSut() => GetSut(null);

        private static IMemoryProtectedBytes GetSut(IByteArrayProtector protector)
            => new MemoryProtectedBytes(protector ?? Stubs.Get<IByteArrayProtector>());

        private static byte[] GetValidBytes() => new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16};
    }
}