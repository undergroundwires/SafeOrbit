using System;
using System.Linq;
using NUnit.Framework;
using SafeOrbit.Tests;
using Shared.Extensions;

namespace SafeOrbit.Memory.SafeBytesServices.DataProtection
{
    /// <seealso cref="MemoryProtectedBytes" />
    /// <seealso cref="IMemoryProtectedBytes" />
    [TestFixture]
    public class MemoryProtectedBytesTests : TestsFor<IMemoryProtectedBytes>
    {
        [Test]
        public void RevealDecryptedBytes_RevealedOnce_ReturnsUnprotectedBytes()
        {
            // Arrange
            var sut = GetSut();
            var expected = GetTestBytes(sut.BlockSizeInBytes);
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
        public void RevealDecryptedBytes_RevealedTwice_ReturnsUnprotectedBytes()
        {
            // Arrange
            var sut = GetSut();
            var expected = GetTestBytes(sut.BlockSizeInBytes);
            sut.Initialize(expected.CopyToNewArray());
            // Act
            byte[] actual;
            using (var firstSession = sut.RevealDecryptedBytes())
                actual = firstSession.PlainBytes;
            var secondSession = sut.RevealDecryptedBytes();
            actual = secondSession.PlainBytes;
            // Assert
            Console.WriteLine("Expected:" + Environment.NewLine +
                              string.Join(", ", expected) + Environment.NewLine +
                              "Actual:" + Environment.NewLine +
                              string.Join(", ", actual));
            Assert.True(expected.SequenceEqual(actual));
        }

        private static byte[] GetTestBytes(int length)
        {
            var buffer = new byte[length];
            for (var i = 0; i < length; i++)
                buffer[i] = (byte)i;
            return buffer;
        }

        protected override IMemoryProtectedBytes GetSut() => new MemoryProtectedBytes();
    }
}
