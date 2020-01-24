using System.Threading.Tasks;
using NUnit.Framework;
using SafeOrbit.Tests;

namespace SafeOrbit.Memory.SafeBytesServices
{
    /// <seealso cref="ISafeByte" />
    /// <seealso cref="SafeByte" />
    [TestFixture]
    internal class SafeBytePerformanceTests : TestsFor<ISafeByte>
    {
        [Test]
        public async Task Get_Takes_Less_Than_5MS([Random(0, 256, 1)] byte b)
        {
            // Arrange
            const int expectedUpperLimit = 5;
            var sut = GetSut();
            await sut.SetAsync(b);
            
            // Act
            var actual = Measure(() => sut.GetAsync());
            
            // Assert
            Assert.That(actual, Is.LessThan(expectedUpperLimit));
        }

        protected override ISafeByte GetSut() => new SafeByte();
    }
}