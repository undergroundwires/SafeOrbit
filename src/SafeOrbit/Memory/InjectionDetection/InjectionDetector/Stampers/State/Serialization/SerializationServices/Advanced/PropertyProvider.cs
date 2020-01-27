//This is a modified version of the beautiful SharpSerializer by Pawel Idzikowski (see: http://www.sharpserializer.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SafeOrbit.Memory.InjectionServices.Stampers.Serialization.SerializationServices.Serializing;

namespace SafeOrbit.Memory.InjectionServices.Stampers.Serialization.SerializationServices.Advanced
{
    /// <summary>
    ///     Provides properties to serialize from source object.
    /// </summary>
    /// <remarks>
    ///     Its methods <see cref="GetAllProperties" /> and <see cref="IgnoreProperty" /> can be overwritten in an inherited
    ///     class to customize its functionality.
    /// </remarks>
    internal class PropertyProvider
    {
#if !PORTABLE
        [ThreadStatic]
#endif
        private static PropertyCache _cache;

        private static PropertyCache Cache => _cache ?? (_cache = new PropertyCache());

        /// <summary>
        ///     Gives all properties back which:
        ///     - are public
        ///     - are not static
        ///     - does not contain ExcludeFromSerializationAttribute
        ///     - have their set and get accessors
        ///     - are not indexers
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        public IList<PropertyInfo> GetProperties(InternalTypeInfo typeInfo)
        {
            // Search in cache
            var propertyInfos = Cache.TryGetPropertyInfos(typeInfo.Type);
            if (propertyInfos != null)
                return propertyInfos;

            // Creating infos
            var properties = GetAllProperties(typeInfo.Type);
            var result = properties
                .Where(property => !IgnoreProperty(typeInfo, property))
                .ToList();

            // adding result to Cache
            Cache.Add(typeInfo.Type, result);

            return result;
        }

        /// <summary>
        ///     Should the property be removed from serialization?
        /// </summary>
        /// <param name="info"></param>
        /// <param name="property"></param>
        /// <returns>
        ///     true if the property:
        ///     - is in the PropertiesToIgnore,
        ///     - contains ExcludeFromSerializationAttribute,
        ///     - does not have it's set or get accessor
        ///     - is indexer
        /// </returns>
        protected virtual bool IgnoreProperty(InternalTypeInfo info, PropertyInfo property)
        {
            if (!property.CanRead || !property.CanWrite)
                return true;

            var indexParameters = property.GetIndexParameters(); // remove indexer
            if (indexParameters.Length > 0)
                return true;

            return false;
        }

        /// <summary>
        ///     Gives all properties back which:
        ///     - are public
        ///     - are not static (instance properties)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected virtual PropertyInfo[] GetAllProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        }
    }
}