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
        public async Task GetHashCode_1000Chars_Takes_Less_Than_100ms()
        {
            // Arrange
            const int expectedHigherLimit = 100;
            SafeOrbitCore.Current.StartEarly();
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

        protected override ISafeString GetSut()
        {
            return new SafeString();
        }
    }
}
