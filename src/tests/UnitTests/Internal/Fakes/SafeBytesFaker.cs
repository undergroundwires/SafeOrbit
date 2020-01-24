using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SafeOrbit.Memory;
using SafeOrbit.Parallel;
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

            public async Task AppendManyAsync(SafeMemoryStream bytes)
            {
                int byteRead;
                while ((byteRead = bytes.ReadByte()) != -1)
                    await AppendAsync((byte)byteRead);
            }

            public async Task AppendAsync(ISafeBytes safeBytes)
            {
                _bytes.AddRange(await safeBytes.ToByteArrayAsync());
            }

            public Task AppendAsync(byte b)
            {
                _bytes.Add(b);
                return Task.FromResult(true);
            }

            public Task<byte> GetByteAsync(int position)
            {
                return Task.FromResult(_bytes.ElementAt(position));
            }

            public void Dispose()
            {
                IsDisposed = true;
            }

            public Task<byte[]> ToByteArrayAsync()
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

            public Task<bool> EqualsAsync(byte[] other)
            {
                return Task.FromResult(other != null &&
                       _bytes.ToArray().SequenceEqual(other));
            }

            public override int GetHashCode()
            {
                return _bytes.Aggregate(2, (current, b) => current + b);
            }

            public Task<bool> EqualsAsync(ISafeBytes other)
            {
                return Task.FromResult(other != null &&
                       _bytes.ToArray().SequenceEqual(TaskContext.RunSync(other.ToByteArrayAsync)));
            }

            public FakeSafeBytes()
            {
                _bytes = new List<byte>();
            }

            private readonly List<byte> _bytes;
        }
    }
}