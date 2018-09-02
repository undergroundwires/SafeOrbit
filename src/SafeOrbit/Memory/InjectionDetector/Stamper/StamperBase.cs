using SafeOrbit.Cryptography.Hashers;
using SafeOrbit.Memory.Injection;

namespace SafeOrbit.Memory.InjectionServices.Stampers
{
    /// <summary>
    ///     Base class for stampers that returns hash of the bytes from the member class.
    /// </summary>
    internal abstract class StamperBase<TObject> : IStamper<TObject>
    {
        private readonly IFastHasher _fastHasher;

        protected StamperBase(IFastHasher fastHasher)
        {
            _fastHasher = fastHasher;
        }

        public abstract InjectionType InjectionType { get; }

        public IStamp<int> GetStamp(TObject @object)
        {
            var hash = SerializeAndHash(@object);
            return new Stamp(hash);
        }

        protected abstract byte[] GetSerializedBytes(TObject @object);
        protected int Hash(byte[] input) => _fastHasher.ComputeFast(input);

        private int SerializeAndHash(TObject @object)
        {
            var serialization = GetSerializedBytes(@object);
            var hash = _fastHasher.ComputeFast(serialization);
            return hash;
        }
    }
}