using System;
using NUnit.Framework;
using SafeOrbit.Memory;

namespace SafeOrbit.Memory.SafeContainerServices.Instance.Providers
{
    /// <seealso cref="SingletonInstanceProvider{TImplementation}"/>
    /// <seealso cref="IInstanceProvider"/>
    [TestFixture]
    public class SingletonInstanceProviderTests
    {
        private static SingletonInstanceProvider<T> GetSut<T>(
            InstanceProtectionMode protectionMode = InstanceProtectionMode.NoProtection) where T : new()
        => new SingletonInstanceProvider<T>(protectionMode);

        [Test]
        public void GetInstance_Returns_Instance_Of_Argument()
        {
            //arrange
            var expected = typeof(DateTime);
            var sut = GetSut<DateTime>();
            //act
            var actual = sut.GetInstance();
            //assert
            Assert.That(actual, Is.InstanceOf(expected));
        }

        [Test]
        public void CanProtectState_Is_True()
        {
            var sut = GetSut<DateTime>();
            Assert.That(sut.CanProtectState, Is.True);
        }

        [Test]
        public void GetInstance_Returns_Same_Instance_Each_Time()
        {
            //arrange
            var sut = GetSut<InstanceTestClass>();
            var expected = sut.GetInstance();
            //act
            var instance1 = sut.GetInstance();
            var instance2 = sut.GetInstance();
            var instance3 = sut.GetInstance();
            //assert
            Assert.That(expected, Is.EqualTo(instance1));
            Assert.That(expected, Is.EqualTo(instance2));
            Assert.That(expected, Is.EqualTo(instance3));
        }

        [Test]
        public void LifeTime_Is_Singleton()
        {
            //arrange
            var expected = LifeTime.Singleton;
            var sut = GetSut<object>();
            //act
            var actual = sut.LifeTime;
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}