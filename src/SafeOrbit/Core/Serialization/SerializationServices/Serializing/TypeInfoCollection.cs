//This is a modified version of the beautiful SharpSerializer by Pawel Idzikowski (see: http://www.sharpserializer.com)

using System;
using System.Collections.ObjectModel;

namespace SafeOrbit.Core.Serialization.SerializationServices.Serializing
{
    internal sealed class TypeInfoCollection : KeyedCollection<Type, InternalTypeInfo>
    {
        /// <summary>
        /// </summary>
        /// <returns>null if the key was not found</returns>
        public InternalTypeInfo TryGetTypeInfo(Type type)
        {
            if (!Contains(type))
                return null;
            return this[type];
        }

        /// <summary>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override Type GetKeyForItem(InternalTypeInfo item)
        {
            return item.Type;
        }
    }
}