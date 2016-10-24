using System;
using NUnit.Framework;
using SafeOrbit.Memory;
using SafeOrbit.Hash;
using SafeOrbit.Memory.Serialization;
using SafeOrbit.Tests;
using SafeOrbit.UnitTests;

namespace SafeOrbit.Memory.InjectionServices.Stampers
{
    [TestFixture]
    internal class IlCodeStamperTests : TestsFor<IStamper<Type>>
    {
        [Test]
        public void InjectionType_Returns_CodeInjection()
        {
            //arrange
            var expected = InjectionType.CodeInjection;
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