using System.Collections.Generic;
using System.Linq;
using Moq;
using SafeOrbit.Memory;
using SafeOrbit.Memory.SafeBytesServices;
using SafeOrbit.Memory.SafeBytesServices.Collection;
using SafeOrbit.Tests;

namespace SafeOrbit.UnitTests
{
    internal class SafeByteCollectionFaker : StubProviderBase<ISafeByteCollection>
    {
        public override ISafeByteCollection Provide()
        {
            var fake = new Mock<ISafeByteCollection>();
            var list = new List<ISafeByte>();
            fake.Setup(x => x.Append(It.IsAny<ISafeByte>())).Callback<ISafeByte>(safeByte => list.Add(safeByte));
            fake.Setup(x => x.Length).Returns(() => list.Count);
            fake.Setup(x => x.Get(It.IsAny<int>())).Returns((int index) => list[index]);
            fake.Setup(x => x.ToDecryptedBytes()).Returns(list.Select(safeByte => safeByte.Get()).ToArray());
            fake.Setup(x => x.Dispose()).Callback(() => list.Clear());
            return fake.Object;
        }
    }
}