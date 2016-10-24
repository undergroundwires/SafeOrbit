using System;
using SafeOrbit.Hash;
using SafeOrbit.Memory.Serialization;

namespace SafeOrbit.Memory.InjectionServices.Stampers
{
    /// <summary>
    ///     Stamps <see cref="object" /> by serializing its state.
    /// </summary>
    /// <seealso cref="StamperBase{Object}" />
    /// <seealso cref="IStamper{Object}" />
    /// <seealso cref="ISerializer"/>
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