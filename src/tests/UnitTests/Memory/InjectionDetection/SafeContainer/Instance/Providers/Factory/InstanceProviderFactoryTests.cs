using System;
using System.Collections.Generic;
using NUnit.Framework;
using SafeOrbit.Memory.InjectionServices;
using SafeOrbit.Tests;

namespace SafeOrbit.Memory.SafeContainerServices.Instance.Providers
{
    /// <seealso cref="InstanceProviderFactory" />
    /// <seealso cref="IInstanceProviderFactory" />
    [TestFixture]
    internal class InstanceProviderFactoryTests : TestsFor<InstanceProviderFactory>
    {
        [Test]
        public void Get_WithCustomFuncParameter_returnsCustomInstanceProvider()
        {
            //arrange
            var sut = GetSut();
            Func<InstanceProviderFactoryTests> func = () => new InstanceProviderFactoryTests();
            var expectedType = typeof(CustomInstanceProvider<InstanceProviderFactoryTests>);
            //act
            var actual = sut.Get(func, InstanceProtectionMode.NoProtection, InjectionAlertChannel.DebugWrite);
            //assert
            Assert.That(actual, Is.TypeOf(expectedType));
            Assert.That(actual, Is.InstanceOf(expectedType));
        }

        [Test]
        [TestCaseSource(typeof(InstanceProviderFactoryTests), nameof(LifeTimeCases))]
        public void Get_WithCustomFuncParameter_setsRightLifeTime(LifeTime expected)
        {
            //arrange
            var sut = GetSut();
            Func<InstanceProviderFactoryTests> func = () => new InstanceProviderFactoryTests();
            //act
            var actual =
                sut.Get(func, InstanceProtectionMode.NoProtection, InjectionAlertChannel.DebugWrite, expected).LifeTime;
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetGeneric_ForSingletonLifeTime_ReturnsSingletonInstanceProvider()
        {
            //arrange
            var sut = GetSut();
            var lifeTime = LifeTime.Singleton;
            var expectedType = typeof(SingletonInstanceProvider<InstanceProviderFactoryTests>);
            //act
            var actual = sut.Get<InstanceProviderFactoryTests>(lifeTime, InstanceProtectionMode.NoProtection,
                InjectionAlertChannel.DebugWrite);
            //assert
            Assert.That(actual, Is.TypeOf(expectedType));
            Assert.That(actual, Is.InstanceOf(expectedType));
        }

        [Test]
        public void GetGeneric_ForTransientLifeTime_ReturnsTransientInstanceProvider()
        {
            //arrange
            var sut = GetSut();
            var lifeTime = LifeTime.Transient;
            var expectedType = typeof(TransientInstanceProvider<InstanceProviderFactoryTests>);
            //act
            var actual = sut.Get<InstanceProviderFactoryTests>(lifeTime, InstanceProtectionMode.NoProtection,
                InjectionAlertChannel.DebugWrite);
            //assert
            Assert.That(actual, Is.TypeOf(expectedType));
            Assert.That(actual, Is.InstanceOf(expectedType));
        }

        [Test]
        public void GetGeneric_ForUnknownLifeTime_throwsArgumentOutOfRangeException()
        {
            //arrange
            var sut = GetSut();
            var lifeTime = LifeTime.Unknown;
            //act
            TestDelegate forUnknownLifeTime =
                () =>
                    sut.Get<InstanceProviderFactoryTests>(lifeTime, InstanceProtectionMode.NoProtection,
                        InjectionAlertChannel.DebugWrite);
            //assert
            Assert.That(forUnknownLifeTime, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        [TestCaseSource(typeof(InstanceProviderFactoryTests), nameof(InjectionAlertChanelCases))]
        public void Get_SetsInitialInjectionAlertChannel(InjectionAlertChannel expected)
        {
            //arrange
            var sut = GetSut();
            //act
            var instance = sut.Get<InstanceProviderFactoryTests>(LifeTime.Transient,
                InstanceProtectionMode.NoProtection,
                expected);
            var actual = instance.AlertChannel;
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCaseSource(typeof(InstanceProviderFactoryTests), nameof(InjectionAlertChanelCases))]
        public void GetGeneric_SetsInitialInjectionAlertChannel(InjectionAlertChannel expected)
        {
            //arrange
            var sut = GetSut();
            Func<InstanceProviderFactoryTests> func = () => new InstanceProviderFactoryTests();
            //act
            var instance = sut.Get(func, InstanceProtectionMode.NoProtection, expected);
            var actual = instance.AlertChannel;
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCaseSource(typeof(InstanceProviderFactoryTests), nameof(InstanceProtectionModeCases))]
        public void GetGeneric_SetsInitialProtectionMode(InstanceProtectionMode expected)
        {
            //arrange
            var sut = GetSut();
            Func<InstanceProviderFactoryTests> func = () => new InstanceProviderFactoryTests();
            //act
            var instance = sut.Get(func, expected, InjectionAlertChannel.DebugWrite);
            var actual = instance.CurrentProtectionMode;
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCaseSource(typeof(InstanceProviderFactoryTests), nameof(InstanceProtectionModeCases))]
        public void Get_SetsInitialProtectionMode(InstanceProtectionMode expected)
        {
            //arrange
            var sut = GetSut();
            //act
            var instance = sut.Get<InstanceProviderFactoryTests>(LifeTime.Transient, expected,
                InjectionAlertChannel.DebugWrite);
            var actual = instance.CurrentProtectionMode;
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        protected override InstanceProviderFactory GetSut() => new InstanceProviderFactory();

        private static IEnumerable<TestCaseData> InstanceProtectionModeCases
        {
            get
            {
                yield return new TestCaseData(InstanceProtectionMode.NoProtection);
                yield return new TestCaseData(InstanceProtectionMode.JustCode);
                yield return new TestCaseData(InstanceProtectionMode.JustState);
                yield return new TestCaseData(InstanceProtectionMode.StateAndCode);
            }
        }

        private static IEnumerable<TestCaseData> InjectionAlertChanelCases
        {
            get
            {
                yield return new TestCaseData(InjectionAlertChannel.DebugWrite);
                yield return new TestCaseData(InjectionAlertChannel.DebugFail);
                yield return new TestCaseData(InjectionAlertChannel.RaiseEvent);
                yield return new TestCaseData(InjectionAlertChannel.ThrowException);
            }
        }

        private static IEnumerable<TestCaseData> LifeTimeCases
        {
            get
            {
                yield return new TestCaseData(LifeTime.Transient);
                yield return new TestCaseData(LifeTime.Singleton);
                yield return new TestCaseData(LifeTime.Unknown);
            }
        }
    }
}