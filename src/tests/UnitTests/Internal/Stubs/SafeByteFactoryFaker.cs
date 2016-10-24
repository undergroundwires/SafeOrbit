using Moq;
using SafeOrbit.Memory;
using SafeOrbit.Memory.SafeBytesServices;
using SafeOrbit.Memory.SafeBytesServices.Factory;
using SafeOrbit.Tests;
using SafeOrbit.UnitTests;

namespace SafeOrbit.Memory.SafeBytesServices.Factory
{
    internal class SafeByteFactoryFaker : StubProviderBase<ISafeByteFactory>
    {
        public override ISafeByteFactory Provide()
        {
            var fake = new Mock<ISafeByteFactory>();
            fake.Setup(x => x.GetByByte(It.IsAny<byte>())).Returns(
                (byte b) =>
                {
                    var safeByte = Stubs.Get<ISafeByte>();
                    safeByte.Set(b);
                    return safeByte;
                });
            fake.Setup(x => x.GetById(It.IsAny<int>())).Returns(
                (int id) =>
                {
                    var safeByte = Stubs.Get<ISafeByte>();
                    safeByte.Set((byte)id);
                    return safeByte;
                });
            return fake.Object;
        }
    }
}