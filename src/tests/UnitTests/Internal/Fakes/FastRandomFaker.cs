using Moq;
using SafeOrbit.Cryptography.Random;
using SafeOrbit.Tests;

namespace SafeOrbit.Fakes
{
    /// <seealso cref="IFastRandom" />
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