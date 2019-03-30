using NUnit.Framework;

namespace SafeOrbit.Cryptography.Random
{
    /// <seealso cref="IFastRandom" />
    /// <seealso cref="FastRandom" />
    [TestFixture]
    public class FastRandomTests : CommonRandomTests<IFastRandom>
    {
        protected override IFastRandom GetStaticInstance() => FastRandom.StaticInstance;

    }
}