
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

using Moq;
using NUnit.Framework;
using SafeOrbit.Cryptography.Hashers;
using SafeOrbit.Fakes;
using SafeOrbit.Infrastructure.Serialization;

namespace SafeOrbit.Memory.InjectionServices.Stampers
{
    [TestFixture]
    internal class StateStamperTests
    {
        [Test]
        public void InjectionType_Returns_VariableInjection()
        {
            //arrange
            var expected = InjectionType.VariableInjection;
            var sut = GetSut();
            //act
            var actual = sut.InjectionType;
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }
        [Test]
        public void GetStamp_ForObjectWithDifferentSerialization_returnsDifferent()
        {
            var serializerMock = new Mock<ISerializer>();
            serializerMock.SetupSequence(m => m.Serialize(It.IsAny<object>()))
                .Returns(new byte[] {5, 10, 20, 25, 30})
                .Returns(new byte[] {30, 40, 50, 60});
            var sut = GetSut(serializerMock.Object);
            var testobj = new object();
            var expected = sut.GetStamp(testobj);
            var actual = sut.GetStamp(testobj);
            Assert.That(actual, Is.Not.EqualTo(expected));
        }
        [Test]
        public void GetStamp_ForObjectWithSameSerialization_returnsSame()
        {
            var serializerMock = new Mock<ISerializer>();
            serializerMock.Setup(m => m.Serialize(It.IsAny<object>())).Returns(new byte[] {5, 10, 20, 25, 30});
            var sut = GetSut(serializerMock.Object);
            var testobj = new object();
            var expected = sut.GetStamp(testobj);
            var actual = sut.GetStamp(testobj);
            Assert.That(actual, Is.EqualTo(expected));
        }
        protected IStamper<object> GetSut(ISerializer serializer = null)
        {
            if (serializer == null) serializer = new Mock<ISerializer>().Object;
            return new StateStamper(serializer, Stubs.Get<IFastHasher>());
        }
    }
}