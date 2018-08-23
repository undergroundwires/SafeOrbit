
/*
MIT License

Copyright (c) 2016-2018 Erkin Ekici - undergroundwires@safeorb.it

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SafeOrbit.Library;
using SafeOrbit.Memory;
using SafeOrbit.Tests;

namespace SafeOrbit.Memory
{
    /// <seealso cref="ISafeBytes" />
    /// <seealso cref="SafeBytes" />
    [TestFixture]
    public class SafeBytesPerformanceTests : TestsFor<ISafeBytes>
    {
        [Test]
        public void ToByteArray_For_100_Bytes_Takes_Less_Than_3000ms()
        {
            //arrange
            SafeOrbitCore.Current.StartEarly();
            var sut = GetSut();
            var expectedHigherLimit = 3000;
            for (var i = 0; i < 100; i++)
            {
                sut.Append((byte)i);
            }
            //act
            var actualPerformance = base.Measure(
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
            var expectedHigherLimit = 200;
            //act
            long actualPerformance = base.Measure(() =>
            {
                for (var i = 0; i < 100; i++)
                {
                    sut.Append((byte)i);
                }
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
