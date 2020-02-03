using NUnit.Framework;
using SafeOrbit.Memory.InjectionServices;
using SafeOrbit.Tests;

namespace SafeOrbit.Memory
{
    /// <seealso cref="SafeContainer" />
    /// <seealso cref="ISafeContainer" />
    /// <seealso cref="SafeContainerProtectionMode" />
    [TestFixture]
    public class SafeContainerTests : TestsFor<SafeContainer>
    {
        [Test]
        public void Get_AfterDisablingProtection_ReturnsInstance()
        {
            // Arrange
            var from = SafeContainerProtectionMode.NonProtection;
            var to = SafeContainerProtectionMode.FullProtection;
            var sut = new SafeContainer(from);
            sut.Register<IInstanceTestClass, InstanceTestClass>(LifeTime.Singleton);
            sut.Verify();

            // Act
            var actual = sut.Get<IInstanceTestClass>();
            sut.SetProtectionMode(to);

            // Assert
            var expected = sut.Get<IInstanceTestClass>();
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Get_AfterEnablingProtection_ReturnsInstance()
        {
            // Arrange
            var from = SafeContainerProtectionMode.FullProtection;
            var to = SafeContainerProtectionMode.NonProtection;
            var sut = new SafeContainer(from);
            sut.Register<IInstanceTestClass, InstanceTestClass>(LifeTime.Singleton);
            sut.Verify();

            // Act
            var actual = sut.Get<IInstanceTestClass>();
            sut.SetProtectionMode(to);

            // Assert
            var expected = sut.Get<IInstanceTestClass>();
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Get_ForSingletonRegistration_ReturnsSameInstance()
        {
            // Arrange
            var sut = GetSut();
            sut.Register<IInstanceTestClass, InstanceTestClass>(LifeTime.Singleton);
            sut.Verify();

            // Act
            var instance = sut.Get<IInstanceTestClass>();
            var instance1 = sut.Get<IInstanceTestClass>();
            var instance2 = sut.Get<IInstanceTestClass>();
            var instance3 = sut.Get<IInstanceTestClass>();
            var instance4 = sut.Get<IInstanceTestClass>();
            var instance5 = sut.Get<IInstanceTestClass>();

            // Assert
            Assert.That(instance1, Is.EqualTo(instance));
            Assert.That(instance2, Is.EqualTo(instance));
            Assert.That(instance3, Is.EqualTo(instance));
            Assert.That(instance4, Is.EqualTo(instance));
            Assert.That(instance5, Is.EqualTo(instance));
        }

        [Test]
        public void Get_ForTransientRegistration_ReturnsDifferentInstances()
        {
            // Arrange
            var sut = GetSut();
            sut.Register<IInstanceTestClass, InstanceTestClass>();
            sut.Verify();

            // Act
            var instance1 = sut.Get<IInstanceTestClass>();
            var instance2 = sut.Get<IInstanceTestClass>();
            var instance3 = sut.Get<IInstanceTestClass>();

            // Assert
            Assert.That(instance1, Is.Not.EqualTo(instance2));
            Assert.That(instance1, Is.Not.EqualTo(instance3));
            Assert.That(instance2, Is.Not.EqualTo(instance3));
        }

        [Test]
        public void NonTypedCustomFuncRegistration_ReturnsResultFromFunc()
        {
            // Arrange
            var sut = GetSut();
            var expected = new InstanceTestClass(88);
            sut.Register(() => expected);
            sut.Verify();

            // Act
            var actual = sut.Get<InstanceTestClass>();

            // Assert
            Assert.That(expected, Is.EqualTo(actual));
        }

        [Test]
        public void TypedCustomFuncRegistration_ReturnsResultFromFunc()
        {
            // Arrange
            var sut = GetSut();
            var expected = new InstanceTestClass(88);
            sut.Register<IInstanceTestClass, InstanceTestClass>(() => expected);
            sut.Verify();

            // Act
            var actual = sut.Get<IInstanceTestClass>();

            // Assert
            Assert.That(expected, Is.EqualTo(actual));
        }

        protected override SafeContainer GetSut() => new SafeContainer();
    }
}