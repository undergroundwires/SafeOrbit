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
        public void Get_ReturnsThePreviouslySetByte_returnsTrue(byte expected)
        {
            //Arrange
            var sut = GetSut();
            //Act
            sut.Set(expected);
            var actual = sut.Get();
            //Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        protected override ISafeByte GetSut() => new SafeByte();
    }
}