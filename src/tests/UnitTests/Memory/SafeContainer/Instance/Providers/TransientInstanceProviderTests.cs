using System;
using NUnit.Framework;
using SafeOrbit.Fakes;
using SafeOrbit.Memory.InjectionServices;

namespace SafeOrbit.Memory.SafeContainerServices.Instance.Providers
{
    /// <seealso cref="TransientInstanceProvider{TImplementation}" />
    /// <seealso cref="IInstanceProvider" />
    [TestFixture]
    public class TransientInstanceProviderTests
    {
        [Test]
        public void LifeTime_Is_Transient()
        {
            //arrange
            var expected = LifeTime.Transient;
            var sut = GetSut<object>();
            //act
            var actual = sut.LifeTime;
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CanProtectState_Is_False()
        {
            var sut = GetSut<DateTime>();
            Assert.That(sut.CanProtectState, Is.False);
        }

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
        public void GetInstance_Returns_New_Instance_On_Each_Call()
        {
            //arrange
            var sut = GetSut<InstanceTestClass>();
            //act
            var instance1 = sut.GetInstance();
            var instance2 = sut.GetInstance();
            var instance3 = sut.GetInstance();
            //assert
            Assert.That(instance1, Is.Not.EqualTo(instance2));
            Assert.That(instance1, Is.Not.EqualTo(instance3));
            Assert.That(instance2, Is.Not.EqualTo(instance3));
        }

        private static TransientInstanceProvider<T> GetSut<T>() where T : new()
        {
            return new TransientInstanceProvider<T>(
                protectionMode: InstanceProtectionMode.NoProtection,
                injectionDetector: Stubs.Get<IInjectionDetector>(),
                alertChannel: InjectionAlertChannel.ThrowException
            );
        }
    }
}