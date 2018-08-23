
/*
MIT License

Copyright (c) 2018 Erkin Ekici - undergroundwires@safeorb.it

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

//This is a modified version of the beautiful SharpSerializer by Pawel Idzikowski (see: http://www.sharpserializer.com)

using System;
using System.Linq;
using System.Reflection;
using SafeOrbit.Infrastructure.Serialization.SerializationServices.Core;

namespace SafeOrbit.Infrastructure.Serialization.SerializationServices.Serializing
{
    /// <summary>
    ///     Gives extended information about a Type
    /// </summary>
    internal sealed class InternalTypeInfo
    {
        /// <summary>
        ///     Cache stores type info and spares time be recall the info every time it is needed
        /// </summary>
#if !PORTABLE
        [ThreadStatic]
#endif
        private static TypeInfoCollection _cache;

        ///<summary>
        ///</summary>
        public bool IsSimple { get; set; }

        ///<summary>
        ///</summary>
        public bool IsArray { get; set; }

        ///<summary>
        ///</summary>
        public bool IsEnumerable { get; set; }

        ///<summary>
        ///</summary>
        public bool IsCollection { get; set; }

        ///<summary>
        ///</summary>
        public bool IsDictionary { get; set; }

        /// <summary>
        ///     Of what type are elements of Array, Collection or values in a Dictionary
        /// </summary>
        public Type ElementType { get; set; }

        /// <summary>
        ///     Of what type are dictionary keys
        /// </summary>
        public Type KeyType { get; set; }

        /// <summary>
        ///     Valid dimensions start with 1
        /// </summary>
        public int ArrayDimensionCount { get; set; }

        /// <summary>
        ///     Property type
        /// </summary>
        public Type Type { get; set; }

        private static TypeInfoCollection Cache
        {
            get
            {
                if (_cache == null)
                    _cache = new TypeInfoCollection();
                return _cache;
            }
        }


        /// <summary>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static InternalTypeInfo GetTypeInfo(object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            var type = obj.GetType();
            return GetTypeInfo(type);
        }


        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static InternalTypeInfo GetTypeInfo(Type type)
        {
            // check if Info is in cache
            var typeInfo = Cache.TryGetTypeInfo(type);
            if (typeInfo == null)
            {
                // no info in cache yet
                typeInfo = new InternalTypeInfo
                {
                    Type = type,
                    IsSimple = Tools.IsSimple(type)
                };

                // check if array of byte
                if (type == typeof(byte[]))
                    typeInfo.ElementType = typeof(byte);

                // Only not simple types can be Collections
                if (!typeInfo.IsSimple)
                {
                    // check if it is an Array
                    typeInfo.IsArray = Tools.IsArray(type);

                    if (typeInfo.IsArray)
                    {
                        // Array? What is its element type?
                        if (type.HasElementType)
                            typeInfo.ElementType = type.GetElementType();

                        // How many dimensions
                        typeInfo.ArrayDimensionCount = type.GetArrayRank();
                    }
                    else
                    {
                        // It is not Array, maybe Enumerable?
                        typeInfo.IsEnumerable = Tools.IsEnumerable(type);
                        if (typeInfo.IsEnumerable)
                        {
                            // it is Enumerable maybe Collection?
                            typeInfo.IsCollection = Tools.IsCollection(type);

                            if (typeInfo.IsCollection)
                            {
                                // Sure it is a Collection, but maybe Dictionary also?
                                typeInfo.IsDictionary = Tools.IsDictionary(type);

                                // Fill its key and value types, if the listing is generic
                                bool elementTypeDefinitionFound;
                                var examinedType = type;
                                do
                                {
                                    elementTypeDefinitionFound = FillKeyAndElementType(typeInfo, examinedType);
                                    examinedType = examinedType.GetTypeInfo().BaseType;
                                    // until key and element definition was found, or the base typ is an object
                                } while (!elementTypeDefinitionFound && (examinedType != null) &&
                                         (examinedType != typeof(object)));
                            }
                        }
                    }
                }
                if (!Cache.Contains(typeInfo))
                {
                    Cache.Add(typeInfo);
                }
            }

            return typeInfo;
        }

        /// <summary>
        ///     Fills the <see cref="InternalTypeInfo.KeyType" /> and <see cref="InternalTypeInfo.ElementType" /> properties
        ///     to target <see paramref="target" /> from <paramref name="source" />.
        /// </summary>
        /// <returns><c>TRUE</c> if the key and value definition was found, otherwise <c>FALSE</c></returns>
        private static bool FillKeyAndElementType(InternalTypeInfo source, Type target)
        {
            var targetInfo = target.GetTypeInfo();
            if (!targetInfo.IsGenericType) return false;
            var arguments = target.GetGenericArguments();
            if (source.IsDictionary)
            {
                // in Dictionary there are keys and values
                source.KeyType = arguments[0];
                source.ElementType = arguments[1];
            }
            else
            {
                // In Collection there are only items
                source.ElementType = arguments[0];
            }
            return arguments.Length > 0;
        }
    }
}