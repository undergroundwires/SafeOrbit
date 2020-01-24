using Moq;
using SafeOrbit.Memory;
using SafeOrbit.Memory.SafeBytesServices;
using SafeOrbit.Memory.SafeBytesServices.Factory;
using SafeOrbit.Tests;

namespace SafeOrbit.Fakes
{
    /// <seealso cref="ISafeByteFactory" />
    internal class SafeByteFactoryFaker : StubProviderBase<ISafeByteFactory>
    {
        public override ISafeByteFactory Provide()
        {
            var fake = new Mock<ISafeByteFactory>();
            fake.Setup(x => x.GetByByteAsync(It.IsAny<byte>())).ReturnsAsync(
                (byte b) =>
                {
                    var safeByte = Stubs.Get<ISafeByte>();
                    safeByte.SetAsync(b);
                    return safeByte;
                });
            fake.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(
                (int id) =>
                {
                    var safeByte = Stubs.Get<ISafeByte>();
                    safeByte.SetAsync((byte)id);
                    return safeByte;
                });
            return fake.Object;
        }
    }
}