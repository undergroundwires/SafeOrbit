//This is a modified version of the beautiful SharpSerializer by Pawel Idzikowski (see: http://www.sharpserializer.com)

using System;
using System.Collections.Generic;
using System.Reflection;

namespace SafeOrbit.Infrastructure.Serialization.SerializationServices.Advanced
{
    ///<summary>
    ///</summary>
    internal class PropertyCache
    {
        private readonly Dictionary<Type, IList<PropertyInfo>> _cache = new Dictionary<Type, IList<PropertyInfo>>();

        /// <summary>
        /// </summary>
        /// <returns>null if the key was not found</returns>
        public IList<PropertyInfo> TryGetPropertyInfos(Type type)
        {
            if (!_cache.ContainsKey(type))
            {
                return null;
            }
            return _cache[type];
        }

        public void Add(Type key, IList<PropertyInfo> value)
        {
            _cache.Add(key, value);
        }
    }
}