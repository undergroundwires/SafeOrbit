using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SafeOrbit.Memory;
using SafeOrbit.Threading;
using SafeOrbit.Tests;

namespace SafeOrbit.Fakes
{
    /// <seealso cref="ISafeBytes" />
    public class SafeBytesFaker : StubProviderBase<ISafeBytes>
    {
        public override ISafeBytes Provide() => new FakeSafeBytes();

        private class FakeSafeBytes : ISafeBytes
        {
            public int Length => _bytes.Count;
            public bool IsDisposed { get; private set; }

            public async Task AppendManyAsync(SafeMemoryStream stream)
            {
                int byteRead;
                while ((byteRead = stream.ReadByte()) != -1)
                    await AppendAsync((byte)byteRead);
            }

            public async Task AppendAsync(ISafeBytes safeBytes)
            {
                _bytes.AddRange(await safeBytes.RevealDecryptedBytesAsync());
            }

            public Task AppendAsync(byte b)
            {
                _bytes.Add(b);
                return Task.FromResult(true);
            }

            public Task<byte> RevealDecryptedByteAsync(int position)
            {
                return Task.FromResult(_bytes.ElementAt(position));
            }

            public void Dispose()
            {
                IsDisposed = true;
            }

            public Task<byte[]> RevealDecryptedBytesAsync()
            {
                return Task.FromResult(_bytes.ToArray());
            }

            public async Task<ISafeBytes> DeepCloneAsync()
            {
                var clone = new FakeSafeBytes();
                foreach (var b in _bytes.ToArray())
                    await clone.AppendAsync(b);
                return clone;
            }

            public override int GetHashCode()
            {
                return _bytes.Aggregate(2, (current, b) => current + b);
            }

            public Task<bool> EqualsAsync(IReadOnlySafeBytes other)
            {
                return Task.FromResult(other != null &&
                       _bytes.AsEnumerable().SequenceEqual(TaskContext.RunSync(other.RevealDecryptedBytesAsync)));
            }

            public Task<bool> EqualsAsync(byte[] other)
            {
                return Task.FromResult(other != null &&
                                       _bytes.AsEnumerable().SequenceEqual(other));
            }

            public FakeSafeBytes()
            {
                _bytes = new List<byte>();
            }

            private readonly List<byte> _bytes;
        }
    }
}