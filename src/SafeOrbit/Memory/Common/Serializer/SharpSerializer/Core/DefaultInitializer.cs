//This is a modified version of the beautiful SharpSerializer by Pawel Idzikowski (see: http://www.sharpserializer.com)

using System.Globalization;
using System.Text;
using System.Xml;
using SafeOrbit.Memory.Serialization.SerializationServices.Advanced;
using SafeOrbit.Memory.Serialization.SerializationServices.Advanced.Serializing;

namespace SafeOrbit.Memory.Serialization.SerializationServices.Core
{
    /// <summary>
    ///   Gives standard settings for the framework. Is used only internally.
    /// </summary>
    internal static class DefaultInitializer
    {
        public static XmlWriterSettings GetXmlWriterSettings()
        {
            return GetXmlWriterSettings(Encoding.UTF8);
        }


        public static XmlWriterSettings GetXmlWriterSettings(Encoding encoding)
        {
            var settings = new XmlWriterSettings
            {
                Encoding = encoding,
                Indent = true,
                OmitXmlDeclaration = true
            };
            return settings;
        }

        public static XmlReaderSettings GetXmlReaderSettings()
        {
            var settings = new XmlReaderSettings
            {
                IgnoreComments = true,
                IgnoreWhitespace = true
            };
            return settings;
        }

        public static ITypeNameConverter GetTypeNameConverter(bool includeAssemblyVersion, bool includeCulture,
                                                              bool includePublicKeyToken)
        {
            return new TypeNameConverter(includeAssemblyVersion, includeCulture, includePublicKeyToken);
        }
    }
}