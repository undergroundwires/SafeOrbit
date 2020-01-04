//This is a modified version of the beautiful SharpSerializer by Pawel Idzikowski (see: http://www.sharpserializer.com)

using System.Text;
using SafeOrbit.Core.Serialization.SerializationServices.Core;
using SafeOrbit.Memory.SafeObject.SharpSerializer;

namespace SafeOrbit.Core.Serialization.SerializationServices
{
    /// <summary>
    ///     All the most important settings for binary serialization
    /// </summary>
    internal sealed class BinarySettings : SerializerSettings<AdvancedSerializerBinarySettings>
    {
        /// <summary>
        ///     Default constructor. Serialization in <see cref="BinarySerializationMode.SizeOptimized" /> mode.
        ///     For other modes choose an overloaded constructor
        /// </summary>
        /// <seealso cref="BinarySerializationMode" />
        public BinarySettings()
        {
            Encoding = Encoding.UTF8;
        }

        /// <summary>
        ///     Overloaded constructor. Chooses mode in which the data is serialized.
        /// </summary>
        /// <param name="mode">
        ///     <p>
        ///         <see cref="BinarySerializationMode.SizeOptimized" /> - all types are stored in a header, objects only reference
        ///         these types (better for collections).
        ///     </p>
        ///     <p>
        ///         <see cref="BinarySerializationMode.Burst" /> - all types are serialized with their objects (better for
        ///         serializing of single objects).
        ///     </p>
        /// </param>
        /// <seealso cref="BinarySerializationMode" />
        public BinarySettings(BinarySerializationMode mode)
        {
            Encoding = Encoding.UTF8;
            Mode = mode;
        }

        /// <summary>
        ///     How are strings serialized. Default is UTF-8.
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        ///     Default is  <see cref="BinarySerializationMode.SizeOptimized" /> - Types and property names are stored in a header.
        ///     The opposite is <see cref="BinarySerializationMode.Burst" /> mode when all
        ///     types are serialized with their objects.
        /// </summary>
        public BinarySerializationMode Mode { get; set; }
    }
}