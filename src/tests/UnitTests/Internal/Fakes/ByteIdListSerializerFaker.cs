using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SafeOrbit.Memory.SafeBytesServices.Collection;

namespace SafeOrbit.Fakes
{
    public class ByteIdListSerializerFaker : IByteIdListSerializer<int>
    {
        public Task<byte[]> SerializeAsync(IReadOnlyCollection<int> list)
            => Task.FromResult(Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(list)));

        public Task<IEnumerable<int>> DeserializeAsync(byte[] list)
            => Task.FromResult(JsonConvert.DeserializeObject<Collection<int>>(Encoding.Unicode.GetString(list)).AsEnumerable());
    }
}