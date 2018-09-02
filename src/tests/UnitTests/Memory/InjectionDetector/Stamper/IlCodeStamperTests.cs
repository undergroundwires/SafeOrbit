
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
using NUnit.Framework;
using SafeOrbit.Cryptography.Hashers;
using SafeOrbit.Fakes;
using SafeOrbit.Infrastructure.Serialization;
using SafeOrbit.Tests;

namespace SafeOrbit.Memory.InjectionServices.Stampers
{
    [TestFixture]
    internal class IlCodeStamperTests : TestsFor<IStamper<Type>>
    {
        [Test]
        public void InjectionType_Returns_CodeInjection()
        {
            //arrange
            const InjectionType expected = InjectionType.CodeInjection;
            var sut = GetSut();
            //act
            var actual = sut.InjectionType;
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }
        [Test]
        public void GetStamp_ForSameObjectsOfSameTypes_returnsSame()
        {
            var sut = GetSut();
            var type1 = GetSut().GetType();
            var type2 = GetSut().GetType();
            var expected = sut.GetStamp(type1);
            var actual = sut.GetStamp(type2);
            Assert.That(actual, Is.EqualTo(expected));
        }
        [Test]
        public void GetStamp_ForDifferentObjectsOfSameType_returnsSame()
        {
            var sut = GetSut();
            var testClass = new SerializerTests.TestClass {TestBool = true, TestInt = 5};
            var differentTestClass = new SerializerTests.TestClass { TestBool = false, TestInt = 10 };
            var testType = testClass.GetType();
            var sameType = differentTestClass.GetType();
            var expected = sut.GetStamp(testType);
            var actual = sut.GetStamp(sameType);
            Assert.That(actual, Is.EqualTo(expected));
        }
        [Test]
        public void GetStamp_ForDifferentTypes_returnsDifferent()
        {
            var sut = GetSut();
            var testClass = new SerializerTests.TestClass { TestBool = true, TestInt = 5 };
            var differentTypeClass = GetSut();
            var testType = testClass.GetType();
            var differentType = differentTypeClass.GetType();
            var expected = sut.GetStamp(testType);
            var actual = sut.GetStamp(differentType);
            Assert.That(actual, Is.Not.EqualTo(expected));
        }
        protected override IStamper<Type> GetSut()
        {
            return new IlCodeStamper(Stubs.Get<IFastHasher>());
        }
    }
}