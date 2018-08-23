
/*
MIT License

Copyright (c) 2018 Erkin Ekici - undergroundwires@safeorb.it

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
using SafeOrbit.Library;
using SafeOrbit.Memory.SafeBytesServices.Id;
using SafeOrbit.Tests.Cases;

namespace SafeOrbit.Memory.SafeBytesServices.Factory
{
    /// <seealso cref="MemoryCachedSafeByteFactory" />
    /// <seealso cref="ISafeByteFactory" />
    [TestFixture]
    internal class MemoryCachedSafeByteFactoryTests
    {
        private ISafeByteFactory _sut;

        [OneTimeSetUp]
        public void Init()
        {
            _sut = new MemoryCachedSafeByteFactory();
            _sut.Initialize();
        }
        [Test, TestCaseSource(typeof(ByteCases), nameof(ByteCases.AllBytes))]
        public void GetByByte_ReturnsRightByte([Random(0, 256, 1)] byte b)
        {
            //arrange
            var expected = b;
            var sut = _sut;
            //act
            var safeByte = sut.GetByByte(expected);
            var actual = safeByte.Get();
           //assert
           Assert.That(actual, Is.EqualTo(expected));
        }
        [Test, TestCaseSource(typeof(ByteCases), nameof(ByteCases.AllBytes))]
        public void GetById_ReturnsRightByte(byte b)
        {
            //arrange
            var sut = _sut;
            var expected = b;
            //act
            var id = SafeOrbitCore.Current.Factory.Get<IByteIdGenerator>().Generate(expected);
            var safeByte = sut.GetById(id);
            var actual = safeByte.Get();
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}