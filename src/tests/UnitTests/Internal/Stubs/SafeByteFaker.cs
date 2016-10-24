using Moq;
using SafeOrbit.Memory;
using SafeOrbit.Memory.SafeBytesServices;
using SafeOrbit.Tests;

namespace SafeOrbit.UnitTests
{
    /// <seealso cref="ISafeByte" />
    /// <seealso cref="SafeByte" />
    internal class SafeByteFaker : StubProviderBase<ISafeByte>
    {
        public override ISafeByte Provide()
        {
            var fake = new Mock<ISafeByte>();
            var b = new byte();
            fake.Setup(x => x.Set(It.IsAny<byte>())).Callback<byte>(x => b = x);
            fake.Setup(x => x.Get()).Returns(() => b);
            fake.Setup(x => x.GetHashCode()).Returns(() => b);
            fake.Setup(x => x.Equals(It.IsAny<byte>())).Returns((byte byt) => byt.Equals(b));
            fake.Setup(x => x.Equals(It.IsAny<ISafeByte>())).Returns((ISafeByte safeB) => safeB.Get().Equals(b));
            fake.Setup(x => x.DeepClone()).Returns(() => fake.Object);
            fake.Setup(x => x.Id).Returns(() => (int)b);
            return fake.Object;
        }
    }
}