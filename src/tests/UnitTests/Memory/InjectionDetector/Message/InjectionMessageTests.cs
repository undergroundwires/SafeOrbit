using System.Collections.Generic;
using NUnit.Framework;
using SafeOrbit.Tests.Equality;

namespace SafeOrbit.Memory.Injection
{
    /// <seealso cref="EqualityTestsBase{InjectionMessage}" />
    /// <seealso cref="InjectionMessage" />
    /// <seealso cref="IInjectionMessage" />
    [TestFixture]
    public class InjectionMessageTests : EqualityTestsBase<InjectionMessage>
    {
        protected override InjectionMessage GetSut() => new InjectionMessage(true, false, 5);
        protected override IEnumerable<InjectionMessage> GetDifferentSuts() => new[]
        {
            new InjectionMessage(false,true,5),
            new InjectionMessage(true,true,5),
            new InjectionMessage(true, false, 6),
            new InjectionMessage(true, false, "aq")

        };
    }
}