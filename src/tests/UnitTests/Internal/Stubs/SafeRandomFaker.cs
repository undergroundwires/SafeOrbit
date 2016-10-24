using Moq;
using SafeOrbit.Random;
using SafeOrbit.Tests;

namespace SafeOrbit.UnitTests
{
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