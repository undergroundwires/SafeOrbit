using Moq;
using SafeOrbit.Memory.SafeBytesServices;
using SafeOrbit.Tests;

namespace SafeOrbit.Fakes
{
    /// <seealso cref="ISafeByte" />
    /// <seealso cref="SafeByte" />
    internal class SafeByteFaker : StubProviderBase<ISafeByte>
    {
        public override ISafeByte Provide()
        {
            var fake = new Mock<ISafeByte>();
            var isSet = false;
            var b = new byte();
            fake.Setup(x => x.SetAsync(It.IsAny<byte>())).Callback<byte>(x => b = x);
            fake.Setup(x => x.GetAsync()).ReturnsAsync(() =>
            {
                isSet = true;
                return b;
            });
            fake.Setup(x => x.IsByteSet)
                .Returns(() => isSet);
            fake.Setup(x => x.GetHashCode())
                .Returns(() => b);
            fake.Setup(x => x.EqualsAsync(It.IsAny<byte>()))
                .ReturnsAsync((byte byt) => byt.Equals(b));
            fake.Setup(x => x.Equals(It.IsAny<ISafeByte>()))
                .Returns((ISafeByte safeB) => safeB.Id == b);
            fake.Setup(x => x.DeepClone())
                .Returns(() => fake.Object);
            fake.Setup(x => x.Id)
                .Returns(() => (int) b);
            return fake.Object;
        }
    }
}