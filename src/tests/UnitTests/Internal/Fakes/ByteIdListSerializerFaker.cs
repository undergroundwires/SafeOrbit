using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Moq;
using Newtonsoft.Json;
using SafeOrbit.Extensions;
using SafeOrbit.Memory.SafeBytesServices.Collection;
using SafeOrbit.Tests;

namespace SafeOrbit.Fakes
{
    public class ByteIdListSerializerFaker : StubProviderBase<IByteIdListSerializer<int>>
    {
        public override IByteIdListSerializer<int> Provide()
        {
            var mock = new Mock<IByteIdListSerializer<int>>();
            mock.Setup(s => s.SerializeAsync(It.IsAny<IReadOnlyCollection<int>>()))
                .ReturnsAsync((IReadOnlyCollection<int> list) =>
                    Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(list)));
            mock.Setup(s => s.DeserializeAsync(It.IsAny<byte[]>()))
                .ReturnsAsync((byte[] list) =>
                    JsonConvert.DeserializeObject<Collection<int>>(Encoding.Unicode.GetString(list)).EmptyIfNull().AsEnumerable());
            return mock.Object;
        }
    }
}