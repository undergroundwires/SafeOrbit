
/*
MIT License

Copyright (c) 2016 Erkin Ekici - undergroundwires@safeorb.it

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
using SafeOrbit.Infrastructure.Serialization.SerializationServices.Core;

namespace SafeOrbit.Infrastructure.Serialization.SerializationServices.Serializing
{
    /// <summary>
    ///     Gives extended information about a Type
    /// </summary>
    internal sealed class TypeInfo
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
        public static TypeInfo GetTypeInfo(object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            var type = obj.GetType();
            return GetTypeInfo(type);
        }


        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static TypeInfo GetTypeInfo(Type type)
        {
            // check if Info is in cache
            var typeInfo = Cache.TryGetTypeInfo(type);
            if (typeInfo == null)
            {
                // no info in cache yet
                typeInfo = new TypeInfo
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
                                    examinedType = examinedType.BaseType;
                                    // until key and element definition was found, or the base typ is an object
                                } while (!elementTypeDefinitionFound && (examinedType != null) &&
                                         (examinedType != typeof(object)));
                            }
                        }
                    }
                }
#if PORTABLE
                Cache.AddIfNotExists(typeInfo);
#else
                Cache.Add(typeInfo);
#endif
            }

            return typeInfo;
        }

        /// <summary>
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <param name="type"></param>
        /// <returns>true if the key and value definition was found</returns>
        private static bool FillKeyAndElementType(TypeInfo typeInfo, Type type)
        {
            if (type.IsGenericType)
            {
                var arguments = type.GetGenericArguments();

                if (typeInfo.IsDictionary)
                {
                    // in Dictionary there are keys and values
                    typeInfo.KeyType = arguments[0];
                    typeInfo.ElementType = arguments[1];
                }
                else
                {
                    // In Collection there are only items
                    typeInfo.ElementType = arguments[0];
                }
                return arguments.Length > 0;
            }
            return false;
        }
    }
}