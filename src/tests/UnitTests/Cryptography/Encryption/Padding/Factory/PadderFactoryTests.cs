using System;
using Moq;
using NUnit.Framework;
using SafeOrbit.Cryptography.Encryption.Padding.Padders;
using SafeOrbit.Fakes;
using SafeOrbit.Tests;

namespace SafeOrbit.Cryptography.Encryption.Padding.Factory
{
    [TestFixture]
    public class PadderFactoryTests : TestsFor<IPadderFactory>
    {
        protected override IPadderFactory GetSut() => new PadderFactory(Stubs.GetFactory<IPkcs7Padder>());

        [Test]
        public void GetPadder_NoneRequested_Throws()
        {
            // Arrange
            var sut = GetSut();
            // Act
            void Action() => sut.GetPadder(PaddingMode.None);
            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Action);
        }

        [Test]
        public void GetPadder_Pkcs7Requested_ReturnsFromFactory()
        {
            // Arrange
            var expected = Stubs.Get<IPkcs7Padder>();
            var factoryMock = new Mock<IFactory<IPkcs7Padder>>();
            factoryMock.Setup(f => f.Create())
                .Returns(expected);
            var sut = GetSut(pkcs7Factory: factoryMock.Object);
            // Act
            var actual = sut.GetPadder(PaddingMode.PKCS7);
            // Assert
            Assert.True(ReferenceEquals(actual, expected));
        }

        private static IPadderFactory GetSut(IFactory<IPkcs7Padder> pkcs7Factory) => new PadderFactory(pkcs7Factory);
    }
}
