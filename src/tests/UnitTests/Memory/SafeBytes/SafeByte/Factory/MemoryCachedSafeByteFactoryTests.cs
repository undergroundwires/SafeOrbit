using Moq;
using NUnit.Framework;
using SafeOrbit.Fakes;
using SafeOrbit.Memory.InjectionServices;
using SafeOrbit.Memory.SafeBytesServices.Id;
using SafeOrbit.Tests.Cases;

namespace SafeOrbit.Memory.SafeBytesServices.Factory
{
    /// <seealso cref="ISafeByteFactory" />
    /// <seealso cref="MemoryCachedSafeByteFactory" />
    [TestFixture]
    internal class MemoryCachedSafeByteFactoryTests
    {
        private ISafeByteFactory _sut;
        [OneTimeSetUp]
        public void Init()
        {
            _sut = GetSut();
        }
        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.AllBytes))]
        public void GetByte_returnsRightSafeByte(byte b)
        {
            //arrange
            var expected = b;
            var sut = _sut;
            //act
            var safeByte = sut.GetByByte(expected);
            var actual = safeByte.Get();
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }
        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.AllBytes))]
        public void GetInt_returnsRightSafeByte(byte b)
        {
            var sut = _sut;
            var expected = b;
            var byteId = Stubs.Get<IByteIdGenerator>().Generate(expected);
            var safeByte = sut.GetById(byteId);
            var actual = safeByte.Get();
            Assert.That(actual, Is.EqualTo(expected));
        }
        [Test]
        public void GetByte_WithoutInitialization_invokesInitializeMethod()
        {
            var mock = GetMock();
            var sut = mock.Object;
            try
            {
                sut.GetByByte((byte) 5);
            }
            catch { /*swallow exceptions*/ }
            mock.Verify(f => f.Initialize(), Times.Once);
        }
        [Test]
        public void GetInt_WithoutInitialization_invokesInitializeMethod()
        {
            var mock = GetMock();
            var sut = mock.Object;
            try
            {
                sut.GetById(5);
            }
            catch { /*swallow exceptions*/ }
            mock.Verify(f => f.Initialize(), Times.Once);
        }
        private static ISafeByteFactory GetSut(SafeObjectProtectionMode innerDictionaryProtectionMode = MemoryCachedSafeByteFactory.DefaultInnerDictionaryProtection,
            InjectionAlertChannel alertChannel = Defaults.AlertChannel)
        {
            return new MemoryCachedSafeByteFactory(
                Stubs.Get<IByteIdGenerator>(),
                Stubs.GetFactory<ISafeByte>(),
                Stubs.Get<ISafeObjectFactory>(),
                innerDictionaryProtectionMode
            );
        }

        private Mock<MemoryCachedSafeByteFactory> GetMock()
            =>
            new Mock<MemoryCachedSafeByteFactory>(
                Stubs.Get<IByteIdGenerator>(),
                Stubs.GetFactory<ISafeByte>(),
                Stubs.Get<ISafeObjectFactory>(),
                MemoryCachedSafeByteFactory.DefaultInnerDictionaryProtection) {CallBase = true};
    }
}