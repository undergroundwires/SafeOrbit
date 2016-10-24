
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

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using SafeOrbit.Tests;
using SafeOrbit.Tests.Cases;

namespace SafeOrbit.Memory.SafeBytesServices.Id
{
    /// <seealso cref="HashedByteIdGenerator" />
    /// <seealso cref="IByteIdGenerator" />
    internal class HashedByteIdGeneratorTests : TestsFor<HashedByteIdGenerator>
    {
        [Test]
        public void Generate_ResultForAllPossibleBytes_areUnique()
        {
            //Arrange
            var sut = GetSut();
            var hashedValues = new int[256];
            for (var i = 0; i < 256; i++)
                hashedValues[i] = sut.Generate((byte) i);
            //Act
            const int expected = 256;
            var actual = hashedValues.Distinct().Count();
            //Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Generate_SameInstanceForSameBytes_givesSameResults([Random(0, 256, 1)] byte b)
        {
            //Arrange
            var sut = GetSut();
            //Act
            var expected = sut.Generate(b);
            var actual = sut.Generate(b);
            //Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.DifferentBytePairs))]
        public void Generate_ForDifferentBytes_areNotEqual(byte b1, byte b2)
        {
            //Arrange
            var sut = GetSut();
            //Act
            var actual = sut.Generate(b1);
            var expected = sut.Generate(b2);
            //Assert
            Assert.AreNotEqual(actual, expected);
        }
        protected override HashedByteIdGenerator GetSut() => new HashedByteIdGenerator();
    }
}