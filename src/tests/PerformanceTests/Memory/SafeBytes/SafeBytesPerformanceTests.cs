using System;
using System.Threading.Tasks;
using NUnit.Framework;
using SafeOrbit.Extensions;
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
        public async Task AppendManyAsync_SingleMegaByteStream_TakesLessThan5000ms()
        {
            // Arrange
            await SafeOrbitCore.Current.StartEarlyAsync();
            const int expectedHigherLimit = 5000;
            var sut = GetSut();
            var bytes = new byte[1000000];
            new Random().NextBytes(bytes);
            var stream = new SafeMemoryStream(bytes);

            // Act
            var actualPerformance = await MeasureAsync(() =>
                sut.AppendManyAsync(stream));

            // Assert
            Assert.That(actualPerformance, Is.LessThanOrEqualTo(expectedHigherLimit));
        }


        [Test]
        public async Task ToDeepCloneAsync_SingleMBStream_TakesLessThan5000ms()
        {
            // Arrange
            await SafeOrbitCore.Current.StartEarlyAsync();
            const int expectedHigherLimit = 5000;
            var sut = GetSut();
            var bytes = new byte[1000000];
            new Random().NextBytes(bytes);
            var stream = new SafeMemoryStream(bytes);
            await sut.AppendManyAsync(stream);

            // Act
            var actualPerformance = await MeasureAsync(() =>
                sut.DeepCloneAsync());

            // Assert
            Assert.That(actualPerformance, Is.LessThanOrEqualTo(expectedHigherLimit));
        }

        [Test]
        public async Task ToByteArrayAsync_1MBBytes_TakesLessThan_2000ms()
        {
            // Arrange
            const int expectedHigherLimit = 2000;
            const int totalBytes = 1000000;
            await SafeOrbitCore.Current.StartEarlyAsync();
            var sut = GetSut();
            var bytes = new byte[totalBytes];
            new Random().NextBytes(bytes);
            var stream = new SafeMemoryStream(bytes);
            await sut.AppendManyAsync(stream);

            // Act
            var actualPerformance = await MeasureAsync(
                () => sut.RevealDecryptedBytesAsync(), 5);

            // Assert
            Assert.That(actualPerformance, Is.LessThanOrEqualTo(expectedHigherLimit));
        }

        [Test]
        public async Task AppendAsync_100SingleBytes_TakesLessThan500ms()
        {
            // Arrange
            await SafeOrbitCore.Current.StartEarlyAsync();
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
        public async Task GetHashCode_1KBBytes_TakesLessThan10ms()
        {
            // Arrange
            const int expectedHigherLimit = 100;
            await SafeOrbitCore.Current.StartEarlyAsync();
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

        [Test]
        public async Task EqualsAsync_10KBSameBytes_TakesLessThan200ms()
        {
            // Arrange
            const int expectedHigherLimit = 200;
            const int totalBytes = 10000;
            await SafeOrbitCore.Current.StartEarlyAsync();
            var sut = GetSut();
            var bytes = new byte[totalBytes];
            new Random().NextBytes(bytes);
            var stream = new SafeMemoryStream(bytes.CopyToNewArray());
            await sut.AppendManyAsync(stream);

            // Act
            var actualPerformance = await MeasureAsync(
                () => sut.EqualsAsync(bytes), 5);

            // Assert
            Assert.That(actualPerformance, Is.LessThanOrEqualTo(expectedHigherLimit));
        }

        [Test]
        public async Task EqualsAsync_10KBSameSafeBytes_TakesLessThan200ms()
        {
            // Arrange
            const int expectedHigherLimit = 200;
            const int totalBytes = 10000;
            await SafeOrbitCore.Current.StartEarlyAsync();

            var bytes = new byte[totalBytes];
            new Random().NextBytes(bytes);            
            
            var sut = GetSut();
            var stream = new SafeMemoryStream(bytes.CopyToNewArray());
            await sut.AppendManyAsync(stream);
           
            var other = new SafeBytes();
            stream = new SafeMemoryStream(bytes);
            await other.AppendManyAsync(stream);

            // Act
            var actualPerformance = await MeasureAsync(
                () => sut.EqualsAsync(other), 5);

            // Assert
            Assert.That(actualPerformance, Is.LessThanOrEqualTo(expectedHigherLimit));
        }

        protected override ISafeBytes GetSut()
        {
            return new SafeBytes();
        }
    }
}