using System;
using SafeOrbit.Cryptography.Hashers;
using SafeOrbit.Infrastructure.Serialization;

namespace SafeOrbit.Memory.InjectionServices.Stampers
{
    /// <inheritdoc />
    /// <summary>
    ///     Stamps <see cref="T:System.Object" /> by serializing its state.
    /// </summary>
    /// <seealso cref="T:SafeOrbit.Memory.InjectionServices.Stampers.StamperBase`1" />
    /// <seealso cref="T:SafeOrbit.Memory.InjectionServices.Stampers.IStamper`1" />
    /// <seealso cref="T:SafeOrbit.Infrastructure.Serialization.ISerializer" />
    internal class StateStamper : StamperBase<object>
    {
        private static readonly Lazy<StateStamper> StaticInstanceLazy = new Lazy<StateStamper>();
        private readonly ISerializer _serializer;
        public StateStamper() : this(Serializer.StaticInstance, Murmur32.StaticInstance)
        {
        }
        internal StateStamper(ISerializer serializer, IFastHasher fastHasher) : base(fastHasher)
        {
            _serializer = serializer;
        }
        public static StateStamper StaticInstance => StaticInstanceLazy.Value;
        public override InjectionType InjectionType { get; } = InjectionType.VariableInjection;
        protected override byte[] GetSerializedBytes(object @object)
        {
            return _serializer.Serialize(@object);
        }
    }
}