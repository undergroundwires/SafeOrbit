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
        public void Get_Takes_Less_Than_5MS([Random(0, 256, 1)]byte b)
        {
            //arrange
            const int expectedUpperLimit = 5;
            var sut = GetSut();
            sut.Set(b);
            //act
            var actual = base.Measure(() => sut.Get());
            //assert
            Assert.That(actual, Is.LessThan(expectedUpperLimit));
        }
        protected override ISafeByte GetSut() => new SafeByte();
    }
}