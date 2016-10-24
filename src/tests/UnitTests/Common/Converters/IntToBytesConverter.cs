using System.Collections;
using NUnit.Framework;
using SafeOrbit.Tests;

namespace SafeOrbit.Converters
{
    public class IntToBytesConverterTests : TestsFor<IntToBytesConverter>
    {
        protected override IntToBytesConverter GetSut()
        {
            return new IntToBytesConverter();
        }

        [Test]
        [TestCaseSource(typeof (IntToBytesConverterTests), nameof(LittleEndianTestVectors))]
        public byte[] Convert_Returns_LittleEndianTestVectors(int test)
        {
            var sut = GetSut();
            var result = sut.Convert(test);
            return result;
        }

        public static IEnumerable LittleEndianTestVectors
        {
            get
            {
                yield return new TestCaseData(32).Returns(new byte[] {32, 0, 0, 0});
                yield return new TestCaseData(0).Returns(new byte[] {0, 0, 0, 0});
                yield return new TestCaseData(-1).Returns(new byte[] {255, 255, 255, 255});
                yield return new TestCaseData(555555554).Returns(new byte[] {226, 26, 29, 33});
                yield return new TestCaseData(-200).Returns(new byte[] {56, 255, 255, 255});
                yield return new TestCaseData(0x7fffffff).Returns(new byte[] {255, 255, 255, 127});
                yield return new TestCaseData(-2147483648).Returns(new byte[] {0, 0, 0, 128});

            }
        }
    }
}
