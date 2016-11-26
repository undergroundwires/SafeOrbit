
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

using System.Collections.Generic;
using NUnit.Framework;
using SafeOrbit.Tests.Equality;

namespace SafeOrbit.Memory.Injection
{
    [TestFixture]
    internal class StampTests : EqualityTestsBase<Stamp>
    {
        [Test]
        public void Constructor_Sets_HashProperty()
        {
            var expected = 55;
            var sut = GetSut(expected);
            var actual = sut.Hash;
            Assert.That(actual, Is.EqualTo(expected));
        }
        protected override Stamp GetSut() => GetSut(5);

        protected override IEnumerable<Stamp> GetDifferentSuts()
        {
            yield return GetSut(15);
            yield return GetSut(0);
        }
        private static Stamp GetSut(int hash) => new Stamp(hash);
    }
}