using System;
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
            // Arrange
            var expected = mode;

            // Act
            var sut = GetSut(protectionMode: expected);
            var actual = sut.CurrentProtectionMode;

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCaseSource(typeof(InjectionCases), nameof(InjectionCases.InjectionAlertChannelCases))]
        public void Constructor_Sets_AlertChannel(InjectionAlertChannel alertChannel)
        {
            // Arrange
            var expected = alertChannel;

            // Act
            var sut = GetSut(alertChannel: expected);
            var actual = sut.AlertChannel;

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCaseSource(typeof(SafeContainerCases),
            nameof(SafeContainerCases.SafeContainerProtectionMode_To_SafeObjectProtectionMode_Vector))]
        public void Constructor_Sets_Inner_SafeObject_From_ProtectionMode(SafeContainerProtectionMode protectionMode,
            SafeObjectProtectionMode expected)
        {
            // Arrange
            var safeObjectMock = new Mock<ISafeObject<Dictionary<string, IInstanceProvider>>>();
            safeObjectMock.Setup(m => m.Object).Returns(new Dictionary<string, IInstanceProvider>());
            var safeObjectFactory = new Mock<ISafeObjectFactory>();
            safeObjectFactory.Setup(
                    s => s.Get<Dictionary<string, IInstanceProvider>>(It.IsAny<IInitialSafeObjectSettings>()))
                .Returns<IInitialSafeObjectSettings>(settings => safeObjectMock.Object);

            // Act
            _ = GetSut(protectionMode: protectionMode, safeObjectFactory: safeObjectFactory.Object);

            // Assert
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
            // Arrange
            var safeObjectMock = new Mock<ISafeObject<Dictionary<string, IInstanceProvider>>>();
            safeObjectMock.Setup(m => m.Object).Returns(new Dictionary<string, IInstanceProvider>());
            var safeObjectFactory = new Mock<ISafeObjectFactory>();
            safeObjectFactory.Setup(
                    s => s.Get<Dictionary<string, IInstanceProvider>>(It.IsAny<IInitialSafeObjectSettings>()))
                .Returns<IInitialSafeObjectSettings>(settings => safeObjectMock.Object);

            // Act
            _ = GetSut(alertChannel: alertChannel, safeObjectFactory: safeObjectFactory.Object);

            // Assert
            safeObjectMock.VerifySet(o => o.AlertChannel = alertChannel);
        }

        [Test]
        [TestCase(SafeContainerProtectionMode.NonProtection)]
        public void CanAlert_ForNonProtectedMode_ReturnsFalse(SafeContainerProtectionMode nonProtectedMode)
        {
            // Arrange
            const bool expected = false;
            var safeObjectMock = new Mock<ISafeObject<Dictionary<string, IInstanceProvider>>>();
            safeObjectMock.Setup(m => m.Object).Returns(new Dictionary<string, IInstanceProvider>());
            safeObjectMock.Setup(m => m.CanAlert).Returns(true);
            var safeObjectFactory = new Mock<ISafeObjectFactory>();
            safeObjectFactory.Setup(
                    s => s.Get<Dictionary<string, IInstanceProvider>>(It.IsAny<IInitialSafeObjectSettings>()))
                .Returns<IInitialSafeObjectSettings>(settings => safeObjectMock.Object);

            // Act
            var sut = GetSut(protectionMode: nonProtectedMode, safeObjectFactory: safeObjectFactory.Object);
            var actual = sut.CanAlert;

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(SafeContainerProtectionMode.FullProtection)]
        public void CanAlert_ForProtectModes_ReturnsTrue(SafeContainerProtectionMode protectedMode)
        {
            // Arrange
            const bool expected = true;
            var safeObjectMock = new Mock<ISafeObject<Dictionary<string, IInstanceProvider>>>();
            safeObjectMock.Setup(m => m.Object).Returns(new Dictionary<string, IInstanceProvider>());
            safeObjectMock.Setup(m => m.CanAlert).Returns(true);
            var safeObjectFactory = new Mock<ISafeObjectFactory>();
            safeObjectFactory.Setup(
                    s => s.Get<Dictionary<string, IInstanceProvider>>(It.IsAny<IInitialSafeObjectSettings>()))
                .Returns<IInitialSafeObjectSettings>(settings => safeObjectMock.Object);
            
            // Act
            var sut = GetSut(protectionMode: protectedMode, safeObjectFactory: safeObjectFactory.Object);
            var actual = sut.CanAlert;

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCaseSource(typeof(SafeContainerCases),
            nameof(SafeContainerCases.SafeContainerProtectionMode_To_InstanceProtectionMode_Vector))]
        public void SetProtectionMode_SwitchingProtectionMode_UpdatesProtectionModeForAllInnerInstances(
            SafeContainerProtectionMode protectionMode, InstanceProtectionMode instanceProtectionMode)
        {
            // Arrange
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

            // Act
            var sut = GetSut(protectionMode: otherProtectionMode, safeObjectFactory: safeObjectFactory.Object);
            sut.SetProtectionMode(protectionMode);

            // Assert
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
            // Arrange
            var expected = alertChannel;
            var safeObjectMock = new Mock<ISafeObject<Dictionary<string, IInstanceProvider>>>();
            safeObjectMock.Setup(m => m.Object).Returns(new Dictionary<string, IInstanceProvider>());
            var safeObjectFactory = new Mock<ISafeObjectFactory>();
            safeObjectFactory.Setup(
                    s => s.Get<Dictionary<string, IInstanceProvider>>(It.IsAny<IInitialSafeObjectSettings>()))
                .Returns<IInitialSafeObjectSettings>(settings => safeObjectMock.Object);
            var sut = GetSut(safeObjectFactory: safeObjectFactory.Object);

            // Act
            sut.AlertChannel = alertChannel;

            // Assert
            safeObjectMock.VerifySet(
                detector => detector.AlertChannel = expected
            );
        }

        [Test]
        public void AlertChannel_VerifiedInstance_ChangeItself()
        {
            // Arrange
            const InjectionAlertChannel expected = InjectionAlertChannel.DebugFail;
            var sut = GetSut();
            sut.Register<IInstanceTestClass, InstanceTestClass>(LifeTime.Singleton);
            sut.Verify();

            // Act
            sut.AlertChannel = expected;

            // Assert
            var actual = sut.AlertChannel;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Get_ForNonRegisteredInterface_ThrowsKeyNotFoundException()
        {
            // Arrange
            var sut = GetSut();

            // Act
            sut.Register<IInstanceTestClass, InstanceTestClass>();
            sut.Verify();
            void GetNonRegistered() => sut.Get<IDisposable>();

            // Assert
            Assert.That(GetNonRegistered, Throws.TypeOf<KeyNotFoundException>());
        }

        [Test]
        public void Get_ForSingleRegisteredInstance_ReturnsAnInstance()
        {
            // Arrange
            var sut = GetSut();

            // Act
            sut.Register<InstanceTestClass>();
            sut.Verify();
            var retrieved = sut.Get<InstanceTestClass>();

            // Assert
            Assert.That(retrieved, Is.Not.Null);
            Assert.That(retrieved, Is.TypeOf<InstanceTestClass>());
            Assert.That(retrieved, Is.InstanceOf<InstanceTestClass>());
        }

        [Test]
        public void Get_WithoutVerifying_ThrowsArgumentException()
        {
            // Arrange
            var sut = GetSut();

            // Assert
            void GetWithoutVerify() => sut.Get<IDisposable>();

            // Act
            Assert.That(GetWithoutVerify, Throws.ArgumentException);
        }

        [Test]
        [TestCaseSource(typeof(InjectionCases), nameof(InjectionCases.InjectionAlertChannelCases))]
        public void Register_GetsInstanceProvider_With_CurrentAlertChannel(InjectionAlertChannel alertChannel)
        {
            // Arrange
            var expected = alertChannel;
            var factoryStub = new Mock<IInstanceProviderFactory>();
            factoryStub.Setup(f => f.Get<InstanceTestClass>(
                It.IsAny<LifeTime>(),
                It.IsAny<InstanceProtectionMode>(),
                It.IsAny<InjectionAlertChannel>())).Returns(Mock.Of<IInstanceProvider>());
            var sut = GetSut(providerFactory: factoryStub.Object, alertChannel: expected);

            // Act
            sut.Register<InstanceTestClass>();

            // Assert
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
            // Arrange
            var factoryStub = new Mock<IInstanceProviderFactory>();
            factoryStub.Setup(f => f.Get<InstanceTestClass>(
                It.IsAny<LifeTime>(),
                It.IsAny<InstanceProtectionMode>(),
                It.IsAny<InjectionAlertChannel>())).Returns(Mock.Of<IInstanceProvider>());
            var sut = GetSut(providerFactory: factoryStub.Object, protectionMode: protectionMode);

            // Act
            sut.Register<InstanceTestClass>();

            // Assert
            factoryStub.Verify(f => f.Get<InstanceTestClass>(
                It.IsAny<LifeTime>(),
                It.Is<InstanceProtectionMode>(a => a.Equals(expected)),
                It.IsAny<InjectionAlertChannel>()), Times.Once);
        }

        [Test]
        [TestCaseSource(typeof(InstanceCases), nameof(InstanceCases.LifeTimes))]
        public void Register_GetsInstanceProvider_With_RequestedLifeTime(LifeTime lifeTime)
        {
            // Arrange
            var expected = lifeTime;
            var factoryStub = new Mock<IInstanceProviderFactory>();
            factoryStub.Setup(f => f.Get<InstanceTestClass>(
                It.IsAny<LifeTime>(),
                It.IsAny<InstanceProtectionMode>(),
                It.IsAny<InjectionAlertChannel>())).Returns(Mock.Of<IInstanceProvider>());
            var sut = GetSut(providerFactory: factoryStub.Object);

            // Act
            sut.Register<InstanceTestClass>(lifeTime: lifeTime);

            // Assert
            factoryStub.Verify(f => f.Get<InstanceTestClass>(
                It.Is<LifeTime>(a => a.Equals(expected)),
                It.IsAny<InstanceProtectionMode>(),
                It.IsAny<InjectionAlertChannel>()), Times.Once);
        }

        [Test]
        public void Register_RegisteringExistingType_ThrowsArgumentException()
        {
            // Arrange
            var sut = GetSut();

            // Act
            sut.Register<IInstanceTestClass, InstanceTestClass>();
            void RegisteringExistingType() => sut.Register<IInstanceTestClass, InstanceTestClass>();

            // Assert
            Assert.That(RegisteringExistingType, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Verify_IfAlreadyVerified_ThrowsArgumentException()
        {
            // Arrange
            var sut = GetSut();
            sut.Register<IInstanceTestClass, InstanceTestClass>();
            sut.Verify();

            // Act
            void VerifyingTwice() => sut.Verify();

            // Assert
            Assert.That(VerifyingTwice, Throws.ArgumentException);
        }

        [Test]
        public void Verify_IfNoTypesAreRegistered_ThrowsArgumentException()
        {
            // Arrange
            var sut = GetSut();

            // Act
            void VerifyBeforeRegistering() => sut.Verify();

            // Assert
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