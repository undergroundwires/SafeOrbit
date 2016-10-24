using System.Runtime.Serialization;
using NUnit.Framework;

namespace SafeOrbit.Memory.InjectionServices
{
    /// <seealso cref="InstanceIdGenerator"/>
    /// <seealso cref="IObjectIdGenerator"/>
    [TestFixture]
    public class InstanceIdGeneratorTests
    {
        [Test]
        public void GetId_SameInstances_ReturnsSameId()
        {
            //arrange
            var sut = GetSut();
            var instance = new IdGenerationTestClass();
            var instance2 = instance;
            //act
            var id1 = sut.GetStateId(instance);
            var id2 = sut.GetStateId(instance2);
            //assert
            Assert.That(id1, Is.EqualTo(id2));
        }

        [Test]
        public void GetId_SameTypesButDifferentInstances_ReturnsDifferentId()
        {
            //arrange
            var sut = GetSut();
            var instance = new IdGenerationTestClass();
            var instance2 = new IdGenerationTestClass();
            //act
            var id1 = sut.GetStateId(instance);
            var id2 = sut.GetStateId(instance2);
            //assert
            Assert.That(id1, Is.Not.EqualTo(id2));
        }

        private static IObjectIdGenerator GetSut() => new InstanceIdGenerator();

        private class IdGenerationTestClass
        {
            
        }
    }
}