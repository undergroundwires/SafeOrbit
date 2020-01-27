//This is a modified version of the beautiful SharpSerializer by Pawel Idzikowski (see: http://www.sharpserializer.com)

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SafeOrbit.Memory.InjectionServices.Stampers.Serialization.SerializationServices.Advanced.Serializing;

namespace SafeOrbit.Memory.InjectionServices.Stampers.Serialization.SerializationServices.Advanced
{
    /// <summary>
    ///     Converts Type to its text representation and vice versa. All types serialize to the
    ///     AssemblyQualifiedName. Use overloaded constructor to shorten type names.
    /// </summary>
    internal sealed class TypeNameConverter : ITypeNameConverter
    {
        private readonly Dictionary<Type, string> _cache = new Dictionary<Type, string>();

        /// <summary>
        ///     Since v.2.12 as default the type name is equal to Type.AssemblyQualifiedName
        /// </summary>
        public TypeNameConverter()
        {
        }

        /// <summary>
        ///     Some values from the Type.AssemblyQualifiedName can be removed
        /// </summary>
        /// <param name="includeAssemblyVersion"></param>
        /// <param name="includeCulture"></param>
        /// <param name="includePublicKeyToken"></param>
        public TypeNameConverter(bool includeAssemblyVersion, bool includeCulture, bool includePublicKeyToken)
        {
            IncludeAssemblyVersion = includeAssemblyVersion;
            IncludeCulture = includeCulture;
            IncludePublicKeyToken = includePublicKeyToken;
        }

        /// <summary>
        ///     Version=x.x.x.x will be inserted to the type name
        /// </summary>
        public bool IncludeAssemblyVersion { get; }

        /// <summary>
        ///     Culture=.... will be inserted to the type name
        /// </summary>
        public bool IncludeCulture { get; }

        /// <summary>
        ///     PublicKeyToken=.... will be inserted to the type name
        /// </summary>
        public bool IncludePublicKeyToken { get; }

        private static string RemovePublicKeyToken(string typeName)
        {
            return Regex.Replace(typeName, @", PublicKeyToken=\w+", string.Empty);
        }

        private static string RemoveCulture(string typeName)
        {
            return Regex.Replace(typeName, @", Culture=\w+", string.Empty);
        }

        private static string RemoveAssemblyVersion(string typeName)
        {
            return Regex.Replace(typeName, @", Version=\d+.\d+.\d+.\d+", string.Empty);
        }

        #region ITypeNameConverter Members

        /// <summary>
        ///     Gives type as text
        /// </summary>
        /// <param name="type"></param>
        /// <returns>string.Empty if the type is null</returns>
        public string ConvertToTypeName(Type type)
        {
            if (type == null) return string.Empty;

            // Search in cache
            if (_cache.ContainsKey(type))
                return _cache[type];

            var typeName = type.AssemblyQualifiedName;

            if (!IncludeAssemblyVersion)
                typeName = RemoveAssemblyVersion(typeName);

            if (!IncludeCulture)
                typeName = RemoveCulture(typeName);

            if (!IncludePublicKeyToken)
                typeName = RemovePublicKeyToken(typeName);

            // Adding to cache
            _cache.Add(type, typeName);

            return typeName;
        }

        /// <summary>
        ///     Gives back Type from the text.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public Type ConvertToType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName)) return null;
            var type = Type.GetType(typeName, true);
            return type;
        }

        #endregion
    }
}