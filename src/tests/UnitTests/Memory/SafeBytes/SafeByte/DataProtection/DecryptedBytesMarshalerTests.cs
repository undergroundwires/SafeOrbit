using System;
using System.Linq;
using NUnit.Framework;

namespace SafeOrbit.Memory.SafeBytesServices.DataProtection
{
    /// <seealso cref="DecryptedBytesMarshaler" />
    /// <seealso cref="IDecryptedBytesMarshaler" />
    [TestFixture]
    public class DecryptedBytesMarshalerTests
    {
        [Test]
        public void Ctor_NullArgument_Throws()
        {
            static void Construct() => GetSut(null);
            Assert.Throws<ArgumentNullException>(Construct);
        }

        [Test]
        public void Ctor_EmptyArgument_Throws()
        {
            static void Construct() => GetSut(new byte[0]);
            Assert.Throws<ArgumentException>(Construct);
        }

        [Test]
        public void PlainBytes_AfterInitialized_IsExpected()
        {
            // Arrange
            var expected = new byte[] {5, 10, 15, 25};
            var sut = GetSut(expected);

            // Act
            var actual = sut.PlainBytes;

            // Assert
            Assert.True(expected.SequenceEqual(actual));
        }

        [Test]
        public void PlainBytes_AfterDisposed_Throws()
        {
            // Arrange
            var expected = new byte[] {5, 10, 15, 25};
            var sut = GetSut(expected);

            // Act
            sut.Dispose();
            void BytesAccessor() => _ = sut.PlainBytes;

            // Assert
            Assert.Throws<ObjectDisposedException>(BytesAccessor);
        }

        [Test]
        public void Dispose_AlreadyDisposed_DoesNotThrow() // The object must not throw an exception if its Dispose method is called multiple times https://docs.microsoft.com/en-us/dotnet/api/system.idisposable.dispose?view=netframework-4.8
        {
            // Arrange
            var expected = new byte[] {5, 10, 15, 25};
            var sut = GetSut(expected);
            sut.Dispose();

            // Act
            void DisposeAgain() => sut.Dispose();

            // Assert
            Assert.DoesNotThrow(DisposeAgain);
        }

        [Test]
        public void Dispose_GivenByteArray_Clears()
        {
            // Arrange
            var bytes = new byte[] {5, 10, 15, 25};
            var sut = GetSut(bytes);

            // Act
            sut.Dispose();

            // Assert
            Assert.True(bytes.All(b => b == 0));
        }

        private static IDecryptedBytesMarshaler GetSut(byte[] decryptedBytes)
        {
            return new DecryptedBytesMarshaler(decryptedBytes);
        }
    }
}