using NUnit.Framework;
using SafeOrbit.Memory.SafeBytesServices;
using SafeOrbit.Tests.Cases;

namespace SafeOrbit.Memory
{
    /// <seealso cref="ISafeBytes"/>
    /// <seealso cref="SafeBytes"/>
    /// <seealso cref="SafeByteTests"/>
    [TestFixture]
    public class SafeBytesTests
    {
        [Test]
        public void Can_Append_And_Get_Appended([Random(0, 256, 1)] byte b)
        {
            //arrange
            var expected = b;
            using var sut = GetSut();
            //act
            sut.Append(expected);
            var actual = sut.GetByte(0);
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void ToByteArray_Can_Append_MultipleBytes_And_Get_All(byte[] array)
        {
            //arrange
            var expected = array;
            var sut = GetSut();
            //act
            for (var i = 0; i < array.Length; i++)
            {
                sut.Append(array[i]);
            }
            var actual = sut.ToByteArray();
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        private static ISafeBytes GetSut() => new SafeBytes();
    }
}