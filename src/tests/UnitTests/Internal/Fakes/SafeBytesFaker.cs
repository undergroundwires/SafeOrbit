
/*
MIT License

Copyright (c) 2016-2018 Erkin Ekici - undergroundwires@safeorb.it

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

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