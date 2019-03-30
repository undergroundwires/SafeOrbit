using System.Collections.Generic;
using System.Linq;
using SafeOrbit.Memory;
using SafeOrbit.Tests;

namespace SafeOrbit.Fakes
{
    /// <seealso cref="ISafeBytes" />
    public class SafeBytesFaker : StubProviderBase<ISafeBytes>
    {
        public override ISafeBytes Provide() => new FakeSafeBytes();
        private class FakeSafeBytes : ISafeBytes
        {
            public int Length => _bytes.Count();
            public bool IsDisposed { get; private set; }
            public void Append(ISafeBytes safeBytes) { _bytes.AddRange(safeBytes.ToByteArray()); }
            public void Append(byte b) { _bytes.Add(b); }
            public byte GetByte(int position) { return _bytes.ElementAt(position); }
            public void Dispose() { IsDisposed = true; }
            public byte[] ToByteArray() { return _bytes.ToArray(); }
            public ISafeBytes DeepClone()
            {
                var clone = new FakeSafeBytes();
                foreach (var b in _bytes.ToArray())
                {
                    clone.Append(b);
                }
                return clone;
            }
            public bool Equals(byte[] other)
            {
                return other != null &&
                    _bytes.ToArray().SequenceEqual(other);
            }
            public override int GetHashCode()
            {
                return _bytes.Aggregate(2, (current, b) => current + b);
            }
            public bool Equals(ISafeBytes other)
            {
                return other != null &&
                    _bytes.ToArray().SequenceEqual(other.ToByteArray());
            }
            public FakeSafeBytes() { _bytes = new List<byte>(); }
            private readonly List<byte> _bytes;
        }
    }
   

}