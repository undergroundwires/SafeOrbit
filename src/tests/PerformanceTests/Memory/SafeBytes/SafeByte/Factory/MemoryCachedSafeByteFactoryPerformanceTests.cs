using NUnit.Framework;
using SafeOrbit.Library;
using SafeOrbit.Memory.SafeBytesServices.Id;
using SafeOrbit.Tests;

namespace SafeOrbit.Memory.SafeBytesServices.Factory
{
    /// <seealso cref="ISafeByteFactory" />
    /// <seealso cref="MemoryCachedSafeByteFactory" />
    [TestFixture]
    public class MemoryCachedSafeByteFactoryPerformanceTests : TestsBase
    {
        private ISafeByteFactory _sut;

        [OneTimeSetUp]
        public void Init()
        {
            _sut = new MemoryCachedSafeByteFactory();
            _sut.Initialize();
        }

        [Test]
        public void GetByByte_Takes_Less_Than_2ms()
        {
            const double expectedMax = 100;
            var actual = Measure(() => _sut.GetByByte(5));
            Assert.That(actual, Is.LessThan(expectedMax));
        }

        [Test]
        public void GetById_Takes_Less_Than_2ms()
        {
            const double expectedMax = 50;
            var idGenerator = SafeOrbitCore.Current.Factory.Get<IByteIdGenerator>();
            var id = idGenerator.Generate(5);
            var actual = Measure(() => _sut.GetById(id));
            Assert.That(actual, Is.LessThan(expectedMax));
        }
    }
}