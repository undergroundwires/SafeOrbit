using System;
using System.IO;
using SafeOrbit.Infrastructure.Serialization.SerializationServices;

namespace SafeOrbit.Infrastructure.Serialization
{
    public class Serializer : ISerializer
    {
        private static readonly Lazy<Serializer> StaticInstanceLazy = new Lazy<Serializer>();

        /// <summary>
        ///     Static instance to re-use for better performance.
        /// </summary>
        private static readonly SharpSerializer BinarySerializer = new SharpSerializer();

        /// <summary>
        ///     Gets the static instance of <see cref="Serialize" />.
        /// </summary>
        /// <value>Value of inner <seealso cref="Lazy{Serializer}" /> instance.</value>
        public static Serializer StaticInstance => StaticInstanceLazy.Value;

        /// <summary>
        ///     Serializes the specified object to a byte array.
        /// </summary>
        /// <param name="object">The object to serialize.</param>
        /// <returns>Byte array for the serialization of the object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="object" /> is <see langword="null" />.</exception>
        public byte[] Serialize(object @object)
        {
            if (@object == null) throw new ArgumentNullException(nameof(@object));
            using (var mStream = new MemoryStream())
            {
                BinarySerializer.Serialize(@object, mStream);
                return mStream.ToArray();
            }
        }

        /// <summary>
        ///     Deserializes the specified byte array to an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="byteArray">The byte array to deserialize.</param>
        /// <returns>Deserialized object from bytes.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="byteArray" /> is <see langword="null" />.</exception>
        public T Deserialize<T>(byte[] byteArray)
        {
            if (byteArray == null) throw new ArgumentNullException(nameof(byteArray));
            using (var mStream = new MemoryStream(byteArray))
            {
                return (T) BinarySerializer.Deserialize(mStream);
            }
        }
    }
}