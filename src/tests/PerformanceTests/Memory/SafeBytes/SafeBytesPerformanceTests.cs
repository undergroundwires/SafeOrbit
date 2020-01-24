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
        public void Append_SingleMegaByteStream_Takes_Less_Than_500ms()
        {
            // Arrange
            SafeOrbitCore.Current.StartEarly();
            const int expectedHigherLimit = 500;
            var sut = GetSut();
            var bytes = new byte[100000]; //1000000
            var stream = new SafeMemoryStream();
            stream.Write(bytes, 0, bytes.Length);
            // Act
            var actualPerformance = Measure(
                () => sut.AppendMany(stream));
            //assert
            Assert.That(actualPerformance, Is.LessThanOrEqualTo(expectedHigherLimit));
        }

        [Test]
        public void ToByteArray_For_100_Bytes_Takes_Less_Than_3000ms()
        {
            //arrange
            SafeOrbitCore.Current.StartEarly();
            var sut = GetSut();
            const int expectedHigherLimit = 3000;
            for (var i = 0; i < 100; i++) sut.Append((byte) i);
            //act
            var actualPerformance = Measure(
                () => sut.ToByteArray());
            //assert
            Assert.That(actualPerformance, Is.LessThanOrEqualTo(expectedHigherLimit));
        }

        [Test]
        public void Adding_100_Bytes_Takes_Less_Than_200ms()
        {
            //arrange
            SafeOrbitCore.Current.StartEarly();
            var sut = GetSut();
            const int expectedHigherLimit = 200;
            //act
            var actualPerformance = Measure(() =>
            {
                for (var i = 0; i < 100; i++) sut.Append((byte) i);
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