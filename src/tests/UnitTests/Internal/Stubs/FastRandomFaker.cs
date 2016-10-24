using Moq;
using SafeOrbit.Random;
using SafeOrbit.Tests;

namespace SafeOrbit.UnitTests
{
    public class FastRandomFaker : StubProviderBase<IFastRandom>
    {
        public override IFastRandom Provide()
        {
            var fake = new Mock<IFastRandom>();
            fake.Setup(x => x.GetBytes(It.IsAny<int>())).Returns<int>((x) => new byte[x]);
            return fake.Object;
        }
    }
}