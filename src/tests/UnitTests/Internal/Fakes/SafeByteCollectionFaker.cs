using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Moq;
using SafeOrbit.Memory.SafeBytesServices;
using SafeOrbit.Memory.SafeBytesServices.Collection;
using SafeOrbit.Tests;

namespace SafeOrbit.Fakes
{
    /// <seealso cref="ISafeByteCollection" />
    internal class SafeByteCollectionFaker : StubProviderBase<ISafeByteCollection>
    {
        public override ISafeByteCollection Provide()
        {
            var fake = new Mock<ISafeByteCollection>();
            var list = new List<ISafeByte>();
            fake.Setup(x => x.AppendAsync(It.IsAny<ISafeByte>())).Callback<ISafeByte>(safeByte => list.Add(safeByte));
            fake.Setup(x => x.Length).Returns(() => list.Count);
            fake.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync((int index) => list[index]);
            fake.Setup(x => x.ToDecryptedBytesAsync()).Returns(
                async () =>
                {
                    var result = new Collection<byte>();
                    foreach(var safeByte in list)
                        result.Add(await safeByte.GetAsync());
                    return result.ToArray();
                });
            fake.Setup(x => x.Dispose()).Callback(() => list.Clear());
            return fake.Object;
        }
    }
}