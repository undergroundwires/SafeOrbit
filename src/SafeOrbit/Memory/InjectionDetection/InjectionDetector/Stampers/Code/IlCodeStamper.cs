using System;
using System.Linq;
using SafeOrbit.Cryptography.Hashers;
using SafeOrbit.Memory.InjectionServices.Reflection;
#if NETSTANDARD1_6
using System.Reflection;
#endif

namespace SafeOrbit.Memory.InjectionServices.Stampers
{
    /// <inheritdoc />
    /// <summary>
    ///     Stamps <see cref="Type" /> of an object by serializing its IL-code.
    /// </summary>
    /// <seealso cref="StamperBase{Type}" />
    /// <seealso cref="IStamper{Type}" />
    internal class IlCodeStamper : StamperBase<Type>
    {
        public static readonly IlCodeStamper StaticInstance = new IlCodeStamper();

        public IlCodeStamper() : this(Murmur32.StaticInstance)
        {
        }

        internal IlCodeStamper(IFastHasher fastHasher) : base(fastHasher)
        {
        }

        public override InjectionType InjectionType { get; } = InjectionType.CodeInjection;

        protected override byte[] GetSerializedBytes(Type @object)
        {
            var methods = @object.GetMethods();
            var bytes = methods.Select(m => m.GetIlBytes()) //get all
                .Where(b => b != null) //remove nulls
                .SelectMany(b => b) //merge
                .ToArray();
            if (bytes == null || !bytes.Any()) return new byte[16]; //return bulk bytes
            return bytes;
        }
    }
}