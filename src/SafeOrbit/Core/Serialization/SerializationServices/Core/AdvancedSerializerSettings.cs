//This is a modified version of the beautiful SharpSerializer by Pawel Idzikowski (see: http://www.sharpserializer.com)

using SafeOrbit.Core.Serialization.SerializationServices.Advanced.Serializing;

namespace SafeOrbit.Core.Serialization.SerializationServices.Core
{
    internal class AdvancedSerializerSettings
    {
        public AdvancedSerializerSettings()
        {
            RootName = "Root";
        }

        /// <summary>
        ///     What name has the root item of your serialization. Default is "Root".
        /// </summary>
        public string RootName { get; set; }

        /// <summary>
        ///     Converts Type to string and vice versa. Default is an instance of TypeNameConverter which serializes Types as "type
        ///     name, assembly name"
        ///     If you want to serialize your objects as fully qualified assembly name, you should set this setting with an
        ///     instance of TypeNameConverter
        ///     with overloaded constructor.
        /// </summary>
        public ITypeNameConverter TypeNameConverter { get; set; }
    }
}