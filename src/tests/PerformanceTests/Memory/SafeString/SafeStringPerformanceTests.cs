using System.Threading.Tasks;
using NUnit.Framework;
using SafeOrbit.Library;
using SafeOrbit.Tests;

namespace SafeOrbit.Memory
{
    [TestFixture]
    public class SafeStringPerformanceTests : TestsFor<ISafeString>
    {
        [Test]
        public async Task GetHashCode_1000Chars_TakesLessThan100ms()
        {
            // Arrange
            const int expectedHigherLimit = 100;
            await SafeOrbitCore.Current.StartEarlyAsync();
            var sut = GetSut();
            await sut.AppendAsync(new string('m', 1000));

            // Act
            var actualPerformance = Measure(() =>
            {
                _ = sut.GetHashCode();
            }, 10);

            // Assert
            Assert.That(actualPerformance, Is.LessThanOrEqualTo(expectedHigherLimit));
        }


        [Test]
        public async Task RevealDecryptedBytesAsync_1000Chars_TakesLessThan5000ms()
        {
            // Arrange
            const int expectedHigherLimit = 5000;
            await SafeOrbitCore.Current.StartEarlyAsync();
            var sut = GetSut();
            await sut.AppendAsync(new string('m', 1000));

            // Act
            var actualPerformance = await MeasureAsync(async () =>
            {
                _ = await sut.RevealDecryptedBytesAsync();
            }, 2);

            // Assert
            Assert.That(actualPerformance, Is.LessThanOrEqualTo(expectedHigherLimit));
        }

        [Test]
        public async Task EqualsAsync_Same1000CharsString_TakesLessThan5000ms()
        {
            // Arrange
            const int expectedHigherLimit = 5000;
            var expected = new string('m', 1000);
            await SafeOrbitCore.Current.StartEarlyAsync();
            var sut = GetSut();
            await sut.AppendAsync(expected);

            // Act
            var actualPerformance = await MeasureAsync(async () =>
            {
                _ = await sut.EqualsAsync(expected);
            }, 3);

            // Assert
            Assert.That(actualPerformance, Is.LessThanOrEqualTo(expectedHigherLimit));
        }

        [Test]
        public async Task EqualsAsync_Same1000CharsISafeString_TakesLessThan5000ms()
        {
            // Arrange
            const int expectedHigherLimit = 5000;
            var expectedStr = new string('m', 1000);
            await SafeOrbitCore.Current.StartEarlyAsync();
            var sut = GetSut();
            await sut.AppendAsync(expectedStr);
            var other = GetSut();
            await other.AppendAsync(expectedStr);

            // Act
            var actualPerformance = await MeasureAsync(async () =>
            {
                _ = await sut.EqualsAsync(other);
            }, 3);

            // Assert
            Assert.That(actualPerformance, Is.LessThanOrEqualTo(expectedHigherLimit));
        }

        protected override ISafeString GetSut()
        {
            return new SafeString();
        }
    }
}
