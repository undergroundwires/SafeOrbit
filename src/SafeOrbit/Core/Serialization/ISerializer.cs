namespace SafeOrbit.Core.Serialization
{
    /// <summary>
    ///     An abstraction for a generic <see cref="object" /> serializer.
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        ///     Serializes the specified object to a byte array.
        /// </summary>
        /// <param name="object">The object to serialize.</param>
        /// <returns>Byte array for the serialization of the object.</returns>
        byte[] Serialize(object @object);

        /// <summary>
        ///     Deserializes the specified byte array to an object.
        /// </summary>
        /// <param name="byteArray">The byte array to deserialize.</param>
        /// <returns>Deserialized object from bytes.</returns>
        T Deserialize<T>(byte[] byteArray);
    }
}