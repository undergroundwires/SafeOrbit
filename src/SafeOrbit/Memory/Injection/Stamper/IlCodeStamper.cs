using System;
using System.Linq;
using SafeOrbit.Hash;

namespace SafeOrbit.Memory.InjectionServices.Stampers
{
    /// <summary>
    ///     Stamps <see cref="Type" /> of an object by serializing its IL-code.
    /// </summary>
    /// <seealso cref="StamperBase{Type}" />
    /// <seealso cref="IStamper{Type}" />
    internal class IlCodeStamper : StamperBase<Type>
    {
        public static IlCodeStamper StaticInstance = new IlCodeStamper();

        public IlCodeStamper() : this(Murmur32.StaticInstance)
        {
        }

        internal IlCodeStamper(IFastHasher fastHasher) : base(fastHasher)
        {
        }

        public override InjectionType InjectionType { get; } = InjectionType.CodeInjection;

        protected override byte[] GetSerializedBytes(Type @object)
        {
            var result = @object.GetMethods().Select(m => m.GetMethodBody())
                .Where(m => m != null)
                .SelectMany(m => m.GetILAsByteArray()).ToArray();
            return result;
        }
    }
}