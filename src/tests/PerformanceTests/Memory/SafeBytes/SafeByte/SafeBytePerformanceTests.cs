
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