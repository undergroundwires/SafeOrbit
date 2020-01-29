using NUnit.Framework;
using SafeOrbit.Library;
using SafeOrbit.Memory.SafeBytesServices.Id;
using SafeOrbit.Tests.Cases;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SafeOrbit.Memory.SafeBytesServices.Factory
{
    /// <seealso cref="MemoryCachedSafeByteFactory" />
    /// <seealso cref="ISafeByteFactory" />
    [TestFixture]
    internal class MemoryCachedSafeByteFactoryTests
    {
        private ISafeByteFactory _sut;

        [OneTimeSetUp]
        public async Task Init()
        {
            _sut = new MemoryCachedSafeByteFactory();
            await _sut.InitializeAsync();
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.AllBytes))]
        public async Task GetByByteAsync_ReturnsRightByte([Random(0, 256, 1)] byte b)
        {
            // Arrange
            var expected = b;
            var sut = _sut;

            // Act
            var safeByte = await sut.GetByByteAsync(expected);

            // Assert
            var actual = await safeByte.GetAsync();
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.AllBytes))]
        public async Task GetByIdAsync_ReturnsRightByte(byte b)
        {
            // Arrange
            var sut = _sut;
            var expected = b;

            // Act
            var id = await SafeOrbitCore.Current.Factory.Get<IByteIdGenerator>().GenerateAsync(expected);
            var safeByte = await sut.GetByIdAsync(id);

            // Assert
            var actual = await safeByte.GetAsync();
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task GetByBytes_ReturnsRightBytes()
        {
            // Arrange
            var expected = new Collection<byte>();
            for (var i = 0; i < 256; i++)
                expected.Add((byte) i);
            var stream = new SafeMemoryStream(expected.ToArray());

            // Act
            var safeBytes = await _sut.GetByBytesAsync(stream);

            // Assert
            var actual = await Task.WhenAll(safeBytes.Select(b => b.GetAsync()));
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}