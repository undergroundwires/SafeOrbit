using System.Threading.Tasks;
using NUnit.Framework;
using SafeOrbit.Tests;
using SafeOrbit.Tests.Cases;

namespace SafeOrbit.Memory.SafeBytesServices
{
    /// <seealso cref="ISafeByte" />
    /// <seealso cref="SafeByte" />
    [TestFixture]
    internal class SafeByteTests : TestsFor<ISafeByte>
    {
        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.AllBytes))]
        public async Task RevealDecryptedByteAsync_ReturnsThePreviouslySetByte_ReturnsTrue(byte expected)
        {
            // Arrange
            var sut = GetSut();

            // Act
            await sut.SetAsync(expected);
            var actual = await sut.RevealDecryptedByteAsync();

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        protected override ISafeByte GetSut() => new SafeByte();
    }
}