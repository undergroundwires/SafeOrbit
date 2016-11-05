
/*
MIT License

Copyright (c) 2016 Erkin Ekici - undergroundwires@safeorb.it

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using NUnit.Framework;
using SafeOrbit.Tests.Cases;

namespace SafeOrbit.Memory
{
    /// <seealso cref="ISafeBytes"/>
    /// <seealso cref="SafeBytes"/>
    [TestFixture]
    public class SafeBytesTests
    {
        [Test]
        public void Can_Append_And_Get_Appended([Random(0, 256, 1)] byte b)
        {
            //arrange
            var expected = b;
            var sut = GetSut();
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