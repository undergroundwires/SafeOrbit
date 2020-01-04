using System;
using System.Collections.Generic;
using NUnit.Framework;
using SafeOrbit.Fakes;
using SafeOrbit.Memory.InjectionServices;

namespace SafeOrbit.Memory.SafeContainerServices.Instance.Providers
{
    /// <seealso cref="CustomInstanceProvider{TImplementation}" />
    /// <seealso cref="IInstanceProvider" />
    [TestFixture]
    public class CustomInstanceProviderTests
    {
        [Test]
        public void Default_LifeTime_Is_Unknown()
        {
            //arrange
            var expected = LifeTime.Unknown;
            var dummyObject = new DateTime();
            var sut = GetSut(() => dummyObject);
            //act
            var actual = sut.LifeTime;
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCaseSource(typeof(InstanceCases), nameof(InstanceCases.LifeTimes))]
        public void Constructor_Sets_LifeTime(LifeTime lifeTime)
        {
            //arrange
            var expected = lifeTime;
            var dummyObject = new DateTime();
            var sut = GetSut(() => dummyObject, lifeTime);
            //act
            var actual = sut.LifeTime;
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCaseSource(typeof(CustomInstanceProviderTests), nameof(LifeTime_Argument_Returns_CanProtectState))]
        public bool CanProtectState_DifferentLifeTimes_ReturnsVector(LifeTime lifeTime)
        {
            var dummyObject = new DateTime();
            var sut = GetSut(() => dummyObject, lifeTime);
            return sut.CanProtectState;
        }

        [Test]
        public void GetInstance_Returns_Instance_Of_Argument()
        {
            //arrange
            var expected = typeof(DateTime);
            var dummyObject = new DateTime();
            var sut = GetSut(() => dummyObject);
            //act
            var actual = sut.GetInstance();
            //assert
            Assert.That(actual, Is.InstanceOf(expected));
        }

        [Test]
        public void GetInstance_Returns_The_Result_Of_Func_Each_time()
        {
            //arrange
            var dummyObject = new InstanceTestClass();
            var func = new Func<InstanceTestClass>(() => dummyObject);
            var sut = GetSut(func);
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

        private static CustomInstanceProvider<T> GetSut<T>(Func<T> func, LifeTime? lifeTime = null) where T : new()
        {
            return
                lifeTime.HasValue
                    ? new CustomInstanceProvider<T>(
                        lifeTime: lifeTime.Value,
                        instanceGetter: func,
                        protectionMode: InstanceProtectionMode.NoProtection,
                        injectionDetector: Stubs.Get<IInjectionDetector>(),
                        alertChannel: InjectionAlertChannel.ThrowException)
                    : new CustomInstanceProvider<T>(
                        instanceGetter: func,
                        protectionMode: InstanceProtectionMode.NoProtection,
                        injectionDetector: Stubs.Get<IInjectionDetector>(),
                        alertChannel: InjectionAlertChannel.ThrowException);
        }

        private static IEnumerable<TestCaseData> LifeTime_Argument_Returns_CanProtectState
        {
            get
            {
                yield return new TestCaseData(LifeTime.Singleton).Returns(true);
                yield return new TestCaseData(LifeTime.Transient).Returns(false);
                yield return new TestCaseData(LifeTime.Unknown).Returns(false);
            }
        }
    }
}