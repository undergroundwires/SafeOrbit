//This is a modified version of the beautiful SharpSerializer by Pawel Idzikowski (see: http://www.sharpserializer.com)

using SafeOrbit.Core.Serialization.SerializationServices.Advanced;

namespace SafeOrbit.Memory.SafeObject.SharpSerializer
{
    /// <summary>
    ///     What format has the serialized binary file. It could be SizeOptimized or Burst.
    /// </summary>
    internal enum BinarySerializationMode
    {
        /// <summary>
        ///     All types are serialized to string lists, which are stored in the file header. Duplicates are removed. Serialized
        ///     objects only reference these types. It reduces size especially if serializing collections. Refer to
        ///     <see cref="SizeOptimizedBinaryWriter" /> for more details.
        /// </summary>
        /// <seealso cref="SizeOptimizedBinaryWriter" />
        SizeOptimized = 0,

        /// <summary>
        ///     There are as many type definitions as many objects stored, not regarding if there are duplicate types defined. It
        ///     reduces the overhead if storing single items, but increases the file size if storing collections. See
        ///     <see cref="BurstBinaryWriter" /> for details.
        /// </summary>
        /// <seealso cref="BurstBinaryWriter" />
        Burst
    }
}