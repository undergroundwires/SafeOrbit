using NUnit.Framework;
using SafeOrbit.Library;
using SafeOrbit.Memory.SafeBytesServices.Factory;
using SafeOrbit.Memory.SafeBytesServices.Id;
using SafeOrbit.Tests.Cases;
using SafeOrbit.Tests;

namespace SafeOrbit.Memory.SafeBytesServices.Factory
{
    /// <seealso cref="MemoryCachedSafeByteFactory" />
    /// <seealso cref="ISafeByteFactory" />
    [TestFixture]
    internal class MemoryCachedSafeByteFactoryTests
    {
        private ISafeByteFactory _sut;

        [OneTimeSetUp]
        public void Init()
        {
            _sut = new MemoryCachedSafeByteFactory();
            _sut.Initialize();
        }
        [Test, TestCaseSource(typeof(ByteCases), nameof(ByteCases.AllBytes))]
        public void GetByByte_ReturnsRightByte([Random(0, 256, 1)] byte b)
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
        [Test, TestCaseSource(typeof(ByteCases), nameof(ByteCases.AllBytes))]
        public void GetById_ReturnsRightByte(byte b)
        {
            //arrange
            var sut = _sut;
            var expected = b;
            //act
            var id = LibraryManagement.Factory.Get<IByteIdGenerator>().Generate(expected);
            var safeByte = sut.GetById(id);
            var actual = safeByte.Get();
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}