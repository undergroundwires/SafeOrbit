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