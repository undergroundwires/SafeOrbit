using System;
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
        public async Task AppendManyAsync_SingleMegaByteStream_Takes_Less_Than_5000ms()
        {
            // Arrange
            SafeOrbitCore.Current.StartEarly();
            const int expectedHigherLimit = 5000;
            var sut = GetSut();
            var bytes = new byte[1000000];
            new Random().NextBytes(bytes);
            var stream = new SafeMemoryStream();
            stream.Write(bytes, 0, bytes.Length);

            // Act
            var actualPerformance = await MeasureAsync(() =>
                sut.AppendManyAsync(stream));

            // Assert
            Assert.That(actualPerformance, Is.LessThanOrEqualTo(expectedHigherLimit));
        }


        [Test]
        public async Task ToDeepCloneAsync_SingleMegaByteStream_Takes_Less_Than_5000ms()
        {
            // Arrange
            SafeOrbitCore.Current.StartEarly();
            const int expectedHigherLimit = 5000;
            var sut = GetSut();
            var bytes = new byte[1000000];
            new Random().NextBytes(bytes);
            var stream = new SafeMemoryStream();
            stream.Write(bytes, 0, bytes.Length);
            await sut.AppendManyAsync(stream);

            // Act
            var actualPerformance = await MeasureAsync(() =>
                sut.DeepCloneAsync());

            // Assert
            Assert.That(actualPerformance, Is.LessThanOrEqualTo(expectedHigherLimit));
        }

        [Test]
        public async Task ToByteArrayAsync_For_1MB_Bytes_Takes_Less_Than_2000ms()
        {
            // Arrange
            const int expectedHigherLimit = 2000;
            const int totalBytes = 1000000;
            SafeOrbitCore.Current.StartEarly();
            var sut = GetSut();
            var bytes = new byte[totalBytes];
            new Random().NextBytes(bytes);
            var stream = new SafeMemoryStream();
            stream.Write(bytes, 0, bytes.Length);
            await sut.AppendManyAsync(stream);

            // Act
            var actualPerformance = await MeasureAsync(
                () => sut.ToByteArrayAsync(), 5);

            // Assert
            Assert.That(actualPerformance, Is.LessThanOrEqualTo(expectedHigherLimit));
        }

        [Test]
        public async Task Adding_100_single_bytes_takes_less_than_500ms()
        {
            // Arrange
            SafeOrbitCore.Current.StartEarly();
            var sut = GetSut();
            const int expectedHigherLimit = 500;

            // Act
            var actualPerformance = await MeasureAsync(async () =>
            {
                for (var i = 0; i < 100; i++)
                    await sut.AppendAsync((byte) i);
            });

            // Assert
            Assert.That(actualPerformance, Is.LessThanOrEqualTo(expectedHigherLimit));
        }

        [Test]
        public async Task GetHashCode_1000Bytes_Takes_Less_Than_10ms()
        {
            // Arrange
            const int expectedHigherLimit = 100;
            SafeOrbitCore.Current.StartEarly();
            var sut = GetSut();
            for (var i = 0; i < 1000; i++)
                await sut.AppendAsync((byte)i);

            // Act
            var actualPerformance = Measure(() =>
            {
                _= sut.GetHashCode();
            },10);

            // Assert
            Assert.That(actualPerformance, Is.LessThanOrEqualTo(expectedHigherLimit));
        }

        protected override ISafeBytes GetSut()
        {
            return new SafeBytes();
        }
    }
}