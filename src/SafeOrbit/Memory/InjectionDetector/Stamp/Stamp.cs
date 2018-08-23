
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

using System;
using System.Globalization;

namespace SafeOrbit.Memory.Injection
{
    internal class Stamp : IStamp
    {
        public int Hash { get; }
        public Stamp(int hash)
        {
            Hash = hash;
        }

        #region Equality
        public bool Equals(IStamp<int> other)
        {
            if (ReferenceEquals(null, other)) return false;
            return this.Hash == other.Hash;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return (-1521134295 * this.Hash);
            }
        }
        public override bool Equals(object obj) => this.Equals(obj as Stamp);
        public static bool operator ==(Stamp a, Stamp b) => ReferenceEquals(null, a) ? ReferenceEquals(null, b) : a.Equals(b);
        public static bool operator !=(Stamp a, Stamp b) => !(a == b);
        #endregion

        public override string ToString() => $"{nameof(Hash)} = \"{Hash}\"";
    }
}