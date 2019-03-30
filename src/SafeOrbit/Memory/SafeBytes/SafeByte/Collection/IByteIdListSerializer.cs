using System.Collections.Generic;
using System.Threading.Tasks;

namespace SafeOrbit.Memory.SafeBytesServices.Collection
{
    public interface IByteIdListSerializer<TByteIdType>
    {
        Task<byte[]> SerializeAsync(IReadOnlyCollection<int> list);
        Task<IEnumerable<int>> DeserializeAsync(byte[] list);
    }
}