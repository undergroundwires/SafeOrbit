
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
using SafeOrbit.Tests;

namespace SafeOrbit.Memory.SafeBytesServices.Factory
{
    /// <seealso cref="ISafeByteFactory" />
    /// <seealso cref="MemoryCachedSafeByteFactory" />
    [TestFixture]
    public class MemoryCachedSafeByteFactoryPerformanceTests : TestsBase
    {
        private ISafeByteFactory _sut;

        [OneTimeSetUp]
        public void Init()
        {
            _sut = new MemoryCachedSafeByteFactory();
            _sut.Initialize();
        }

        [Test]
        public void GetByByte_Takes_Less_Than_2ms()
        {
            const double expectedMax = 100;
            var actual = base.Measure(() => _sut.GetByByte((byte)5));
            Assert.That(actual, Is.LessThan(expectedMax));
        }

        [Test]
        public void GetById_Takes_Less_Than_2ms()
        {
            const double expectedMax = 50;
            var idGenerator = SafeOrbitCore.Current.Factory.Get<IByteIdGenerator>();
            var id = idGenerator.Generate(5);
            var actual = base.Measure(() => _sut.GetById(id));
            Assert.That(actual, Is.LessThan(expectedMax));
        }
    }
}