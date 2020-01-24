using System.Threading.Tasks;
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
        public async Task Init()
        {
            _sut = new MemoryCachedSafeByteFactory();
            await _sut.InitializeAsync();
        }

        [Test]
        public void GetByByteAsync_Takes_Less_Than_2ms()
        {
            const double expectedMax = 100;
            var actual = MeasureAsync(() => _sut.GetByByteAsync(5));
            Assert.That(actual, Is.LessThan(expectedMax));
        }

        [Test]
        public async Task GetByIdAsync_Takes_Less_Than_2ms()
        {
            const double expectedMax = 50;
            var idGenerator = SafeOrbitCore.Current.Factory.Get<IByteIdGenerator>();
            var id = await idGenerator.GenerateAsync(5);
            var actual = MeasureAsync(() => _sut.GetByIdAsync(id));
            Assert.That(actual, Is.LessThan(expectedMax));
        }
    }
}