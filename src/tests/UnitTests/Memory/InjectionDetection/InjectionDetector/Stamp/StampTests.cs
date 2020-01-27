using System.Collections.Generic;
using NUnit.Framework;
using SafeOrbit.Tests.Equality;

namespace SafeOrbit.Memory.Injection
{
    [TestFixture]
    internal class StampTests : EqualityTestsBase<Stamp>
    {
        [Test]
        public void Constructor_Sets_HashProperty()
        {
            var expected = 55;
            var sut = GetSut(expected);
            var actual = sut.Hash;
            Assert.That(actual, Is.EqualTo(expected));
        }

        protected override Stamp GetSut() => GetSut(5);

        protected override IEnumerable<Stamp> GetDifferentSuts()
        {
            yield return GetSut(15);
            yield return GetSut(0);
        }

        private static Stamp GetSut(int hash) => new Stamp(hash);
    }
}