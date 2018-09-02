using System;
using System.Linq;
using SafeOrbit.Infrastructure.Reflection;
using SafeOrbit.Cryptography.Hashers;

namespace SafeOrbit.Memory.InjectionServices.Stampers
{
    /// <inheritdoc />
    /// <summary>
    ///     Stamps <see cref="T:System.Type" /> of an object by serializing its IL-code.
    /// </summary>
    /// <seealso cref="T:SafeOrbit.Memory.InjectionServices.Stampers.StamperBase`1" />
    /// <seealso cref="T:SafeOrbit.Memory.InjectionServices.Stampers.IStamper`1" />
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
            var methods = @object.GetMethods();
            var bytes = methods.Select(m => m.GetIlBytes()) //get all
                .Where(b=> b != null) //remove nulls
                .SelectMany(b=> b) //merge
                .ToArray();
            if ((bytes == null) || !bytes.Any()) return new byte[16]; //return bulk bytes
            return bytes;
        }
    }
}