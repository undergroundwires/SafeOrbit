using Moq;
using SafeOrbit.Cryptography.Random;
using SafeOrbit.Tests;

namespace SafeOrbit.Fakes
{
    /// <seealso cref="ISafeRandom" />
    public class SafeRandomFaker : StubProviderBase<ISafeRandom>
    {
        public override ISafeRandom Provide()
        {
            var fake = new Mock<ISafeRandom>();
            fake.Setup(x => x.GetBytes(It.IsAny<int>())).Returns<int>((x) => new byte[x]);
            return fake.Object;
        }
    }
}