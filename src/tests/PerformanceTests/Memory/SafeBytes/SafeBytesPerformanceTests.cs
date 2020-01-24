using System.Threading.Tasks;
using NUnit.Framework;
using SafeOrbit.Library;
using SafeOrbit.Tests;

namespace SafeOrbit.Memory
{
    /// <seealso cref="ISafeBytes" />
    /// <seealso cref="SafeBytes" />
    [TestFixture]
    public class SafeBytesPerformanceTests : TestsFor<ISafeBytes>
    {
        [Test]
        public void AppendManyAsync_SingleMegaByteStream_Takes_Less_Than_3000ms()
        {
            // Arrange
            SafeOrbitCore.Current.StartEarly();
            const int expectedHigherLimit = 3000;
            var sut = GetSut();
            var bytes = new byte[1000000];
            var stream = new SafeMemoryStream();
            stream.Write(bytes, 0, bytes.Length);
            // Act
            var actualPerformance = Measure(() =>
                sut.AppendManyAsync(stream));
            //assert
            Assert.That(actualPerformance, Is.LessThanOrEqualTo(expectedHigherLimit));
        }

        [Test]
        public async Task ToByteArrayAsync_For_100_Bytes_Takes_Less_Than_3000ms()
        {
            //arrange
            SafeOrbitCore.Current.StartEarly();
            var sut = GetSut();
            const int expectedHigherLimit = 3000;
            var bytes = new byte[100];
            var stream = new SafeMemoryStream();
            stream.Write(bytes, 0, bytes.Length);
            await sut.AppendManyAsync(stream);
            //act
            var actualPerformance = Measure(
                () => sut.ToByteArrayAsync());
            //assert
            Assert.That(actualPerformance, Is.LessThanOrEqualTo(expectedHigherLimit));
        }

        [Test]
        public async Task Adding_100_single_bytes_takes_less_than_500ms()
        {
            //arrange
            SafeOrbitCore.Current.StartEarly();
            var sut = GetSut();
            const int expectedHigherLimit = 500;
            //act
            var actualPerformance = await MeasureAsync(async () =>
            {
                for (var i = 0; i < 100; i++)
                    await sut.AppendAsync((byte) i);
            });
            //assert
            Assert.That(actualPerformance, Is.LessThanOrEqualTo(expectedHigherLimit));
        }

        protected override ISafeBytes GetSut()
        {
            return new SafeBytes();
        }
    }
}