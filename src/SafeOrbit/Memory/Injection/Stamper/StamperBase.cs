using System;
using SafeOrbit.Hash;
using SafeOrbit.Memory.Injection;

namespace SafeOrbit.Memory.InjectionServices.Stampers
{
    internal abstract class StamperBase<TObject>: IStamper<TObject>
    {
        protected abstract byte[] GetSerializedBytes(TObject @object);
        public abstract InjectionType InjectionType { get; }
        private readonly IFastHasher _fastHasher;
        protected StamperBase(IFastHasher fastHasher)
        {
            _fastHasher = fastHasher;
        }
        protected int Hash(byte[] input) => _fastHasher.ComputeFast(input);
        private int SerializeAndHash(TObject @object)
        {
            var serialization = GetSerializedBytes(@object);
            var hash = _fastHasher.ComputeFast(serialization);
            return hash;
        }
        public IStamp<int> GetStamp(TObject @object)
        {
            var hash = SerializeAndHash(@object);
            return new Stamp(hash);
        }
    }
}