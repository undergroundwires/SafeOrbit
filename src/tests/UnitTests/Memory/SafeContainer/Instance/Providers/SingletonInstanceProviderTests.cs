using System;
using NUnit.Framework;
using SafeOrbit.Fakes;
using SafeOrbit.Memory.InjectionServices;

namespace SafeOrbit.Memory.SafeContainerServices.Instance.Providers
{
    /// <seealso cref="SingletonInstanceProvider{TImplementation}" />
    /// <seealso cref="IInstanceProvider" />
    [TestFixture]
    public class SingletonInstanceProviderTests
    {
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

        private static SingletonInstanceProvider<T> GetSut<T>() where T : new()
        {
            return new SingletonInstanceProvider<T>(
                protectionMode: InstanceProtectionMode.NoProtection,
                injectionDetector: Stubs.Get<IInjectionDetector>(),
                alertChannel: InjectionAlertChannel.ThrowException
            );
        }
    }
}