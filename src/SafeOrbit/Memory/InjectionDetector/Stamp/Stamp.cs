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