﻿using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using SafeOrbit.Fakes;
using SafeOrbit.Memory.InjectionServices;
using SafeOrbit.Memory.InjectionServices.Protectable;
using SafeOrbit.Memory.InjectionServices.Reflection;
using SafeOrbit.Memory.SafeContainerServices.Instance;
using SafeOrbit.Memory.SafeContainerServices.Instance.Providers;
using SafeOrbit.Memory.SafeContainerServices.Instance.Validation;

namespace SafeOrbit.Memory
{
    /// <seealso cref="SafeContainer" />
    /// <seealso cref="ISafeContainer" />
    /// <seealso cref="SafeContainerProtectionMode" />
    [TestFixture]
    public class SafeContainerTests : ProtectableBaseTests<SafeContainerProtectionMode>
    {
        //TODO: Test inner instance validator call with a stub

        [Test]
        [TestCaseSource(typeof(SafeContainerCases), nameof(SafeContainerCases.ProtectionModes))]
        public void Constructor_Sets_ProtectionMode(SafeContainerProtectionMode mode)
        {
            //arrange
            var expected = mode;
            //act
            var sut = GetSut(protectionMode: expected);
            var actual = sut.CurrentProtectionMode;
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCaseSource(typeof(InjectionCases), nameof(InjectionCases.InjectionAlertChannelCases))]
        public void Constructor_Sets_AlertChannel(InjectionAlertChannel alertChannel)
        {
            //arrange
            var expected = alertChannel;
            //act
            var sut = GetSut(alertChannel: expected);
            var actual = sut.AlertChannel;
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCaseSource(typeof(SafeContainerCases),
            nameof(SafeContainerCases.SafeContainerProtectionMode_To_SafeObjectProtectionMode_Vector))]
        public void Constructor_Sets_Inner_SafeObject_From_ProtectionMode(SafeContainerProtectionMode protectionMode,
            SafeObjectProtectionMode expected)
        {
            //arrange
            var safeObjectMock = new Mock<ISafeObject<Dictionary<string, IInstanceProvider>>>();
            safeObjectMock.Setup(m => m.Object).Returns(new Dictionary<string, IInstanceProvider>());
            var safeObjectFactory = new Mock<ISafeObjectFactory>();
            safeObjectFactory.Setup(
                    s => s.Get<Dictionary<string, IInstanceProvider>>(It.IsAny<IInitialSafeObjectSettings>()))
                .Returns<IInitialSafeObjectSettings>(settings => safeObjectMock.Object);
            //act
            var sut = GetSut(protectionMode: protectionMode, safeObjectFactory: safeObjectFactory.Object);
            //assert
            safeObjectFactory.Verify(
                f =>
                    f.Get<Dictionary<string, IInstanceProvider>>(
                        It.Is<IInitialSafeObjectSettings>(s => s.ProtectionMode.Equals(expected))),
                Times.Once);
        }

        [Test]
        [TestCaseSource(typeof(InjectionCases), nameof(InjectionCases.InjectionAlertChannelCases))]
        public void Constructor_AlertChannel_SetsToInnerSafeObject(InjectionAlertChannel alertChannel)
        {
            //arrange
            var safeObjectMock = new Mock<ISafeObject<Dictionary<string, IInstanceProvider>>>();
            safeObjectMock.Setup(m => m.Object).Returns(new Dictionary<string, IInstanceProvider>());
            var safeObjectFactory = new Mock<ISafeObjectFactory>();
            safeObjectFactory.Setup(
                    s => s.Get<Dictionary<string, IInstanceProvider>>(It.IsAny<IInitialSafeObjectSettings>()))
                .Returns<IInitialSafeObjectSettings>(settings => safeObjectMock.Object);
            //act
            var sut = GetSut(alertChannel: alertChannel, safeObjectFactory: safeObjectFactory.Object);
            //assert
            safeObjectMock.VerifySet(o => o.AlertChannel = alertChannel);
        }

        [Test]
        [TestCase(SafeContainerProtectionMode.NonProtection)]
        public void CanAlert_ForNonProtectedMode_returnsFalse(SafeContainerProtectionMode nonProtectedMode)
        {
            //arrange
            const bool expected = false;
            var safeObjectMock = new Mock<ISafeObject<Dictionary<string, IInstanceProvider>>>();
            safeObjectMock.Setup(m => m.Object).Returns(new Dictionary<string, IInstanceProvider>());
            safeObjectMock.Setup(m => m.CanAlert).Returns(true);
            var safeObjectFactory = new Mock<ISafeObjectFactory>();
            safeObjectFactory.Setup(
                    s => s.Get<Dictionary<string, IInstanceProvider>>(It.IsAny<IInitialSafeObjectSettings>()))
                .Returns<IInitialSafeObjectSettings>(settings => safeObjectMock.Object);
            ;
            //act
            var sut = GetSut(protectionMode: nonProtectedMode, safeObjectFactory: safeObjectFactory.Object);
            var actual = sut.CanAlert;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(SafeContainerProtectionMode.FullProtection)]
        public void CanAlert_ForProtectModes_returnsTrue(SafeContainerProtectionMode protectedMode)
        {
            //arrange
            const bool expected = true;
            var safeObjectMock = new Mock<ISafeObject<Dictionary<string, IInstanceProvider>>>();
            safeObjectMock.Setup(m => m.Object).Returns(new Dictionary<string, IInstanceProvider>());
            safeObjectMock.Setup(m => m.CanAlert).Returns(true);
            var safeObjectFactory = new Mock<ISafeObjectFactory>();
            safeObjectFactory.Setup(
                    s => s.Get<Dictionary<string, IInstanceProvider>>(It.IsAny<IInitialSafeObjectSettings>()))
                .Returns<IInitialSafeObjectSettings>(settings => safeObjectMock.Object);
            ;
            //act
            var sut = GetSut(protectionMode: protectedMode, safeObjectFactory: safeObjectFactory.Object);
            var actual = sut.CanAlert;
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCaseSource(typeof(SafeContainerCases),
            nameof(SafeContainerCases.SafeContainerProtectionMode_To_InstanceProtectionMode_Vector))]
        public void SetProtectionMode_SwitchingProtectionMode_UpdatesProtectionModeForAllInnerInstances(
            SafeContainerProtectionMode protectionMode, InstanceProtectionMode instanceProtectionMode)
        {
            //arrange
            var safeObjectMock = new Mock<ISafeObject<Dictionary<string, IInstanceProvider>>>();
            var instanceProviders = new[]
            {
                new Mock<IInstanceProvider>(),
                new Mock<IInstanceProvider>(),
                new Mock<IInstanceProvider>()
            };
            var instancesToReturn = instanceProviders
                .Select(instance => instance.Object)
                .ToDictionary(provider => provider.ToString());
            safeObjectMock
                .Setup(m => m.Object)
                .Returns(instancesToReturn);
            safeObjectMock.Setup(m => m.CanAlert)
                .Returns(true);
            var safeObjectFactory = new Mock<ISafeObjectFactory>();
            safeObjectFactory.Setup(
                    s => s.Get<Dictionary<string, IInstanceProvider>>(It.IsAny<IInitialSafeObjectSettings>()))
                .Returns<IInitialSafeObjectSettings>(settings => safeObjectMock.Object);
            foreach (var provider in instanceProviders)
                provider.Invocations.Clear();
            var otherProtectionMode = protectionMode == SafeContainerProtectionMode.FullProtection
                ? SafeContainerProtectionMode.NonProtection
                : SafeContainerProtectionMode.FullProtection;
            //act
            var sut = GetSut(protectionMode: otherProtectionMode, safeObjectFactory: safeObjectFactory.Object);
            sut.SetProtectionMode(protectionMode);
            //assert
            foreach (var provider in instanceProviders)
                provider.Verify(
                    p => p.SetProtectionMode(
                        It.Is<InstanceProtectionMode>(mode => mode.Equals(instanceProtectionMode))),
                    Times.Once);
        }

        [Test]
        [TestCaseSource(typeof(InjectionCases), nameof(InjectionCases.InjectionAlertChannelCases))]
        public void AlertChannel_Sets_InnerSafeObjectAlertChannel(InjectionAlertChannel alertChannel)
        {
            //arrange
            var expected = alertChannel;
            var safeObjectMock = new Mock<ISafeObject<Dictionary<string, IInstanceProvider>>>();
            safeObjectMock.Setup(m => m.Object).Returns(new Dictionary<string, IInstanceProvider>());
            var safeObjectFactory = new Mock<ISafeObjectFactory>();
            safeObjectFactory.Setup(
                    s => s.Get<Dictionary<string, IInstanceProvider>>(It.IsAny<IInitialSafeObjectSettings>()))
                .Returns<IInitialSafeObjectSettings>(settings => safeObjectMock.Object);
            var sut = GetSut(safeObjectFactory: safeObjectFactory.Object);
            //act
            sut.AlertChannel = alertChannel;
            //assert
            safeObjectMock.VerifySet(
                detector => detector.AlertChannel = expected
            );
        }

        [Test]
        public void Get_ForNonRegisteredInterface_throwsKeyNotFoundException()
        {
            var sut = GetSut();
            sut.Register<IInstanceTestClass, InstanceTestClass>();
            sut.Verify();
            void GetNonRegistered() => sut.Get<IDisposable>();
            Assert.That(GetNonRegistered, Throws.TypeOf<KeyNotFoundException>());
        }

        [Test]
        public void Get_ForSingleRegisteredInstance_returnsAnInstance()
        {
            var sut = GetSut();
            sut.Register<InstanceTestClass>();
            sut.Verify();
            var retrieved = sut.Get<InstanceTestClass>();
            //assert
            Assert.That(retrieved, Is.Not.Null);
            Assert.That(retrieved, Is.TypeOf<InstanceTestClass>());
            Assert.That(retrieved, Is.InstanceOf<InstanceTestClass>());
        }

        [Test]
        public void Get_WithoutVerifying_throwsArgumentException()
        {
            var sut = GetSut();
            void GetWithoutVerify() => sut.Get<IDisposable>();
            Assert.That(GetWithoutVerify, Throws.ArgumentException);
        }

        [Test]
        [TestCaseSource(typeof(InjectionCases), nameof(InjectionCases.InjectionAlertChannelCases))]
        public void Register_GetsInstanceProvider_With_CurrentAlertChannel(InjectionAlertChannel alertChannel)
        {
            //arrange
            var expected = alertChannel;
            var factoryStub = new Mock<IInstanceProviderFactory>();
            factoryStub.Setup(f => f.Get<InstanceTestClass>(
                It.IsAny<LifeTime>(),
                It.IsAny<InstanceProtectionMode>(),
                It.IsAny<InjectionAlertChannel>())).Returns(Mock.Of<IInstanceProvider>());
            var sut = GetSut(providerFactory: factoryStub.Object, alertChannel: expected);
            //act
            sut.Register<InstanceTestClass>();
            //assert
            factoryStub.Verify(f => f.Get<InstanceTestClass>(
                It.IsAny<LifeTime>(),
                It.IsAny<InstanceProtectionMode>(),
                It.Is<InjectionAlertChannel>(a => a.Equals(expected))), Times.Once);
        }

        [Test]
        [TestCaseSource(typeof(SafeContainerCases),
            nameof(SafeContainerCases.SafeContainerProtectionMode_To_InstanceProtectionMode_Vector))]
        public void Register_GetsInstanceProvider_With_CurrentProtectionMode(SafeContainerProtectionMode protectionMode,
            InstanceProtectionMode expected)
        {
            //arrange
            var factoryStub = new Mock<IInstanceProviderFactory>();
            factoryStub.Setup(f => f.Get<InstanceTestClass>(
                It.IsAny<LifeTime>(),
                It.IsAny<InstanceProtectionMode>(),
                It.IsAny<InjectionAlertChannel>())).Returns(Mock.Of<IInstanceProvider>());
            var sut = GetSut(providerFactory: factoryStub.Object, protectionMode: protectionMode);
            //act
            sut.Register<InstanceTestClass>();
            //assert
            factoryStub.Verify(f => f.Get<InstanceTestClass>(
                It.IsAny<LifeTime>(),
                It.Is<InstanceProtectionMode>(a => a.Equals(expected)),
                It.IsAny<InjectionAlertChannel>()), Times.Once);
        }

        [Test]
        [TestCaseSource(typeof(InstanceCases), nameof(InstanceCases.LifeTimes))]
        public void Register_GetsInstanceProvider_With_RequestedLifeTime(LifeTime lifeTime)
        {
            //arrange
            var expected = lifeTime;
            var factoryStub = new Mock<IInstanceProviderFactory>();
            factoryStub.Setup(f => f.Get<InstanceTestClass>(
                It.IsAny<LifeTime>(),
                It.IsAny<InstanceProtectionMode>(),
                It.IsAny<InjectionAlertChannel>())).Returns(Mock.Of<IInstanceProvider>());
            var sut = GetSut(providerFactory: factoryStub.Object);
            //act
            sut.Register<InstanceTestClass>(lifeTime: lifeTime);
            //assert
            factoryStub.Verify(f => f.Get<InstanceTestClass>(
                It.Is<LifeTime>(a => a.Equals(expected)),
                It.IsAny<InstanceProtectionMode>(),
                It.IsAny<InjectionAlertChannel>()), Times.Once);
        }

        [Test]
        public void Register_RegisteringExistingType_throwsArgumentException()
        {
            var sut = GetSut();
            sut.Register<IInstanceTestClass, InstanceTestClass>();
            void RegisteringExistingType() => sut.Register<IInstanceTestClass, InstanceTestClass>();
            Assert.That(RegisteringExistingType, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Verify_IfAlreadyVerified_throwsArgumentException()
        {
            //arrange
            var sut = GetSut();
            sut.Register<IInstanceTestClass, InstanceTestClass>();
            sut.Verify();

            //act
            void VerifyingTwice() => sut.Verify();
            //assert
            Assert.That(VerifyingTwice, Throws.ArgumentException);
        }

        [Test]
        public void Verify_IfNoTypesAreRegistered_throwsArgumentException()
        {
            var sut = GetSut();
            void VerifyBeforeRegistering() => sut.Verify();
            Assert.That(VerifyBeforeRegistering, Throws.TypeOf<ArgumentException>());
        }

        private static ISafeContainer GetSut(
            SafeContainerProtectionMode protectionMode = SafeContainerProtectionMode.FullProtection
            , IInstanceProviderFactory providerFactory = null, IInstanceValidator instanceValidator = null,
            ISafeObjectFactory safeObjectFactory = null,
            InjectionAlertChannel alertChannel = Defaults.AlertChannel
        )
        {
            return new SafeContainer(
                Stubs.Get<ITypeIdGenerator>(),
                providerFactory ?? Stubs.Get<IInstanceProviderFactory>(),
                instanceValidator ?? Stubs.Get<IInstanceValidator>(),
                safeObjectFactory ?? Stubs.Get<ISafeObjectFactory>(),
                protectionMode,
                alertChannel
            );
        }

        protected override IEnumerable<TestCaseData> GetProtectionModeTestCases()
        {
            yield return
                new TestCaseData(GetSut(SafeContainerProtectionMode.NonProtection),
                    SafeContainerProtectionMode.FullProtection);
            yield return
                new TestCaseData(GetSut(),
                    SafeContainerProtectionMode.NonProtection);
        }
    }
}