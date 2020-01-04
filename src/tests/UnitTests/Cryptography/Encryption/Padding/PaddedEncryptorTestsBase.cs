using System;
using System.Linq;
using NUnit.Framework;

namespace SafeOrbit.Cryptography.Encryption.Padding
{
    public abstract class PaddedEncryptorTestsBase
    {
        protected abstract IPaddedEncryptor GetEncryptor(PaddingMode mode);

        [Test]
        [TestCaseSource(typeof(PaddedEncryptorTestsBase), nameof(PaddingModes))]
        public void PaddingMode_DifferentModes_CanSet(PaddingMode expected)
        {
            // Arrange
            var sut = GetEncryptor(mode: expected);
            // Act
            sut.Padding = expected;
            // Assert
            var actual = sut.Padding;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCaseSource(typeof(PaddedEncryptorTestsBase), nameof(PaddingModes))]
        public void Ctor_DifferentPaddingModes_Sets(PaddingMode expected)
        {
            // Act
            var sut = GetEncryptor(mode: expected);
            // Assert
            var actual = sut.Padding;
            Assert.AreEqual(expected, actual);
        }

        public static object[] PaddingModes => Enum.GetValues(typeof(PaddingMode)).Cast<object>().ToArray();
    }
}