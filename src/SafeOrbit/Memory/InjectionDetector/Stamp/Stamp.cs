namespace SafeOrbit.Memory.Injection
{
    internal class Stamp : IStamp
    {
        public Stamp(int hash)
        {
            Hash = hash;
        }

        public int Hash { get; }

        public override string ToString() => $"{nameof(Hash)} = \"{Hash}\"";

        #region Equality

        public bool Equals(IStamp<int> other)
        {
            if (ReferenceEquals(null, other)) return false;
            return Hash == other.Hash;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return -1521134295 * Hash;
            }
        }

        public override bool Equals(object obj) => Equals(obj as Stamp);

        public static bool operator ==(Stamp a, Stamp b) =>
            ReferenceEquals(null, a) ? ReferenceEquals(null, b) : a.Equals(b);

        public static bool operator !=(Stamp a, Stamp b) => !(a == b);

        #endregion
    }
}