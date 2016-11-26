
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

using System.Linq;
using NUnit.Framework;
using SafeOrbit.Tests;

namespace SafeOrbit.Memory.SafeBytesServices.DataProtection
{
    [TestFixture]
    public class MemoryProtectorTests : TestsFor<MemoryProtector>
    {
        protected override MemoryProtector GetSut() => new MemoryProtector();

        [Test]
        public void Protect_WhenByteArrayIsProtected_ByteArrayIsNotMemoryReadable()
        {
            //Arrange
            var sut = GetSut();
            var notExpected = new byte[]
            {
                10, 20, 30, 40, 50, 60, 80, 90, 100,
                110, 120, 130, 140, 150, 160, 170
            };
            var actual = notExpected.ToArray();
            //Act
            sut.Protect(actual);
            //Assert
            Assert.That(actual, Is.Not.EqualTo(notExpected));
        }

        [Test]
        public void Unprotect_AfterByteArrayIsProtected_BringsBackTheByteArray()
        {
            //Arrange
            var sut = GetSut();
            var expected = new byte[]
            {
                10, 20, 30, 40, 50, 60, 80, 90, 100,
                110, 120, 130, 140, 150, 160, 170
            };
            var actual = expected.ToArray();
            //Act
            sut.Protect(actual);
            sut.Unprotect(actual);
            //Assert
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}