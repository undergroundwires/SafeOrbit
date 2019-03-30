using NUnit.Framework;

namespace SafeOrbit.Cryptography.Random
{
    /// <seealso cref="ISafeRandom" />
    /// <seealso cref="SafeRandom" />
    [TestFixture]
    public class SafeRandomTests : CommonRandomTests<ISafeRandom>
    {
        protected override ISafeRandom GetStaticInstance() => SafeRandom.StaticInstance;
    }
}