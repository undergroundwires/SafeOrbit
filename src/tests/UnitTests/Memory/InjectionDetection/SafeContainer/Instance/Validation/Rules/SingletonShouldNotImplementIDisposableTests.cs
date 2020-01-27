using System;
using System.Linq;
using NUnit.Framework;
using SafeOrbit.Memory.SafeContainerServices.Instance.Providers;
using SafeOrbit.Tests;

namespace SafeOrbit.Memory.SafeContainerServices.Instance.Validation
{
    /// <seealso cref="SingletonShouldNotImplementIDisposableTests" />
    /// <seealso cref="IInstanceProviderRule" />
    internal class SingletonShouldNotImplementIDisposableTests : TestsFor<SingletonShouldNotImplementIDisposable>
    {
        [Test]
        public void WhenSingletonImplementsIDisposable_ReturnsError()
        {
            //arrange
            var sut = GetSut();
            var instance = new SingletonInstanceProvider<DisposableClass>(InstanceProtectionMode.NoProtection);
            //act
            var isSatisfied = sut.IsSatisfiedBy(instance);
            var errors = sut.Errors;
            //assert
            Assert.That(isSatisfied, Is.False);
            Assert.That(errors, Is.Not.Null.Or.Empty);
            Assert.That(errors.Count(), Is.EqualTo(1));
        }

        [Test]
        public void WhenSingletonDoesNotImplementIDisposable_DoesNotReturnError()
        {
            //arrange
            var sut = GetSut();
            var instance = new SingletonInstanceProvider<NotDisposableClass>(InstanceProtectionMode.NoProtection);
            //act
            var isSatisfied = sut.IsSatisfiedBy(instance);
            var errors = sut.Errors;
            //assert
            Assert.That(isSatisfied, Is.True);
            Assert.That(errors, Is.Null.Or.Empty);
        }


        protected override SingletonShouldNotImplementIDisposable GetSut()
        {
            return new SingletonShouldNotImplementIDisposable();
        }

        private class NotDisposableClass
        {
        }

        private class DisposableClass : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}