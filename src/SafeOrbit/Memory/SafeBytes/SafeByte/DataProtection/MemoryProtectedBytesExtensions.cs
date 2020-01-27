using System;
using System.Threading.Tasks;
using SafeOrbit.Threading;

namespace SafeOrbit.Memory.SafeBytesServices.DataProtection
{
    public static class MemoryProtectedBytesExtensions
    {
        public static async Task<IDecryptedBytesMarshaler> RevealDecryptedBytesAsync(
            this AsyncLazy<IMemoryProtectedBytes> lazyBytes)
        {
            if (lazyBytes == null) throw new ArgumentNullException(nameof(lazyBytes));
            var bytes = await lazyBytes.ValueAsync().ConfigureAwait(false);
            var marshaler = await bytes.RevealDecryptedBytesAsync().ConfigureAwait(false);
            return marshaler;
        }
    }
}
