using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SafeOrbit.Memory.SafeContainerServices.Instance.Providers;

namespace SafeOrbit.Memory.SafeContainerServices.Instance.Validation
{
    /// <seealso cref="SingletonShouldNotDependOnTransientRule" />
    /// <seealso cref="IInstanceProviderRule" />
    [TestFixture]
    public class SingletonShouldNotDependOnTransientRuleTests
    {
        [Test]
        public void WhenSingletonDependsOnTransient_ReturnsError()
        {
            //arrange
            var instanceProviders = new List<IInstanceProvider>();
            var singleton = new SingletonInstanceProvider<Singleton>(InstanceProtectionMode.NoProtection);
            var transient = new TransientInstanceProvider<Transient>(InstanceProtectionMode.NoProtection);
            instanceProviders.Add(singleton);
            instanceProviders.Add(transient);
            var sut = new SingletonShouldNotDependOnTransientRule(instanceProviders);
            //act
            var isSatisfied = sut.IsSatisfiedBy(singleton);
            var errors = sut.Errors;
            //assert
            Assert.That(isSatisfied, Is.False);
            Assert.That(errors, Is.Not.Null.Or.Empty);
            Assert.That(errors.Count(), Is.EqualTo(1));
        }

        private class Transient
        {
        }

        private class Singleton
        {
            private readonly Transient _transient;

            public Singleton()
            {
            }

            internal Singleton(Transient transient)
            {
                _transient = transient;
            }
        }
    }
}