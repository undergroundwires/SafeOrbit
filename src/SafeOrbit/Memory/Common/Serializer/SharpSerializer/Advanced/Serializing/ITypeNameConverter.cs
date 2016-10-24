//This is a modified version of the beautiful SharpSerializer by Pawel Idzikowski (see: http://www.sharpserializer.com)
using System;

namespace SafeOrbit.Memory.Serialization.SerializationServices.Advanced.Serializing
{
    /// <summary>
    ///   Converts Type to its string representation and vice versa. The default instance used in the Framework is TypeNameConverter
    /// </summary>
    internal interface ITypeNameConverter
    {
        /// <summary>
        ///   Gives back Type as text.
        /// </summary>
        /// <param name = "type"></param>
        /// <returns>string.Empty if the type is null</returns>
        string ConvertToTypeName(Type type);

        /// <summary>
        ///   Gives back Type from the text.
        /// </summary>
        /// <param name = "typeName"></param>
        /// <returns></returns>
        Type ConvertToType(string typeName);
    }
}