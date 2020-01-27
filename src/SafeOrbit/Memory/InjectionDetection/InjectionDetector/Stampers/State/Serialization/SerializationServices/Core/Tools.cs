//This is a modified version of the beautiful SharpSerializer by Pawel Idzikowski (see: http://www.sharpserializer.com)

using System;
using System.Collections;
using System.Reflection;

namespace SafeOrbit.Memory.InjectionServices.Stampers.Serialization.SerializationServices.Core
{
    /// <summary>
    ///     Some help functions for the serializing framework. As these functions are complexer
    ///     they can be converted to single classes.
    /// </summary>
    internal static class Tools
    {
        /// <summary>
        ///     Is the simple type (string, DateTime, TimeSpan, Decimal, Enumeration or other primitive type)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSimple(Type type)
        {
            if (type == typeof(string))
                return true;
            if (type == typeof(DateTime))
                return true;
            if (type == typeof(TimeSpan))
                return true;
            if (type == typeof(decimal))
                return true;
            if (type == typeof(Guid))
                return true;
            var typeInfo = type.GetTypeInfo();
            if (type == typeof(Type) || typeInfo.IsSubclassOf(typeof(Type)))
                return true;
            if (typeInfo.IsEnum)
                return true;
            if (type == typeof(byte[]))
                return true;
            return typeInfo.IsPrimitive;
        }

        /// <summary>
        ///     Is type an IEnumerable
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsEnumerable(Type type)
        {
            var referenceType = typeof(IEnumerable);
            return referenceType.IsAssignableFrom(type);
        }

        /// <summary>
        ///     Is type ICollection
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsCollection(Type type)
        {
            var referenceType = typeof(ICollection);
            return referenceType.IsAssignableFrom(type);
        }

        /// <summary>
        ///     Is type IDictionary
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsDictionary(Type type)
        {
            var referenceType = typeof(IDictionary);
            return referenceType.IsAssignableFrom(type);
        }

        /// <summary>
        ///     Is it array? It does not matter if single dimensional or multidimensional
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsArray(Type type)
        {
            return type.IsArray;
        }

        /// <summary>
        ///     Creates instance from type. There must be a standard constructor (without parameters) in the type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object CreateInstance(Type type)
        {
            if (type == null) return null;

            try
            {
                var result = Activator.CreateInstance(type);
                return result;
            }
            catch (Exception ex)
            {
                throw new CreatingInstanceException(
                    $"Error during creating an object. Please check if the type \"{type.AssemblyQualifiedName}\" has public parameterless constructor, or if the settings IncludeAssemblyVersionInTypeName, IncludeCultureInTypeName, IncludePublicKeyTokenInTypeName are set to true. Details are in the inner exception.",
                    ex);
            }
        }
    }
}