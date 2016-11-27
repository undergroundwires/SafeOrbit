
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

using System.IO;
using System.Linq;
using NUnit.Framework;
using SafeOrbit.Infrastructure;
using SafeOrbit.Extensions;

namespace SafeOrbit.Memory
{
    /// <seealso cref="SafeMemoryStream"/>
    [TestFixture]
    public class SafeMemoryStreamTests
    {
        [Test]
        public void Write_Clears_Input_Buffer()
        {
            var sut = GetSut();
            var buffer = new byte[] {5, 10, 15, 20, 25, 30};
            sut.Write(buffer,0, buffer.Length);
            Assert.That(buffer, Is.Empty.Or.All.EqualTo(0));
        }

        [Test]
        public void Read_Clears_Inner_Buffer()
        {
            //arrange
            var expected1 = new byte[] { 5, 10, 15 };
            var expected2 = new byte[] {20, 25, 30};
            var sut = GetSut();
            var buffer = expected1.Combine(expected2);
            sut.Write(buffer, 0, buffer.Length);
            //act
            var part1 = new byte[3];
            sut.Read(part1, 0, 3);
            var part2 = new byte[3];
            sut.Read(part2, 0, 3);
            //assert
            Assert.That(part1, Is.EqualTo(expected1));
            Assert.That(part2, Is.EqualTo(expected2));
            Assert.That(sut.Length, Is.Zero);
        }


        private static Stream GetSut() => new SafeMemoryStream();
    }
}