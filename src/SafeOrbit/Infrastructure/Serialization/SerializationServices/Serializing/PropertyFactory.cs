
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SafeOrbit.Infrastructure.Serialization.SerializationServices.Advanced;
using SafeOrbit.Infrastructure.Serialization.SerializationServices.Core;

namespace SafeOrbit.Infrastructure.Serialization.SerializationServices.Serializing
{
    /// <summary>
    ///     Decomposes object to a Property and its Subproperties
    /// </summary>
    internal sealed class PropertyFactory
    {
        private readonly object[] _emptyObjectArray = new object[0];

        /// <summary>
        ///     Contains reference targets.
        /// </summary>
        private readonly Dictionary<object, ReferenceTargetProperty> _propertyCache =
            new Dictionary<object, ReferenceTargetProperty>();

        private readonly PropertyProvider _propertyProvider;

        /// <summary>
        ///     It will be incremented as neccessary
        /// </summary>
        private int _currentReferenceId = 1;

        /// <summary>
        /// </summary>
        /// <param name="propertyProvider">provides all important properties of the decomposed object</param>
        public PropertyFactory(PropertyProvider propertyProvider)
        {
            _propertyProvider = propertyProvider;
        }

        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns>NullProperty if the value is null</returns>
        public Property CreateProperty(string name, object value)
        {
            if (value == null) return new NullProperty(name);

            // If value type is recognized, it will be taken from typeinfo cache
            var typeInfo = InternalTypeInfo.GetTypeInfo(value);

            // Is it simple type
            var property = CreateSimpleProperty(name, typeInfo, value);
            if (property != null)
                return property;

            // From now it can only be an instance of ReferenceTargetProperty
            var referenceTarget = CreateReferenceTargetInstance(name, typeInfo);

            // Search in Cache
            ReferenceTargetProperty cachedTarget;
            if (_propertyCache.TryGetValue(value, out cachedTarget))
            {
                // Value was already referenced
                // Its reference will be used
                cachedTarget.Reference.Count++;
                referenceTarget.MakeFlatCopyFrom(cachedTarget);
                return referenceTarget;
            }

            // Target was not found in cache
            // it must be created

            // Adding property to cache
            referenceTarget.Reference = new ReferenceInfo
            {
                Id = _currentReferenceId++
            };
            _propertyCache.Add(value, referenceTarget);

            // Parsing the property
            var handled = FillSingleDimensionalArrayProperty(referenceTarget as SingleDimensionalArrayProperty, typeInfo,
                value);
            handled = handled ||
                      FillMultiDimensionalArrayProperty(referenceTarget as MultiDimensionalArrayProperty, typeInfo,
                          value);
            handled = handled || FillDictionaryProperty(referenceTarget as DictionaryProperty, typeInfo, value);
            handled = handled || FillCollectionProperty(referenceTarget as CollectionProperty, typeInfo, value);
            handled = handled || FillComplexProperty(referenceTarget as ComplexProperty, typeInfo, value);

            if (!handled)
                throw new InvalidOperationException($"Property cannot be filled. Property: {referenceTarget}");

            return referenceTarget;
        }

        private static ReferenceTargetProperty CreateReferenceTargetInstance(string name, InternalTypeInfo typeInfo)
        {
            // Is it array?
            if (typeInfo.IsArray)
            {
                if (typeInfo.ArrayDimensionCount < 2)
                    return new SingleDimensionalArrayProperty(name, typeInfo.Type);
                // MultiD-Array
                return new MultiDimensionalArrayProperty(name, typeInfo.Type);
            }

            if (typeInfo.IsDictionary)
                return new DictionaryProperty(name, typeInfo.Type);
            if (typeInfo.IsCollection)
                return new CollectionProperty(name, typeInfo.Type);
            if (typeInfo.IsEnumerable)
                return new CollectionProperty(name, typeInfo.Type);

            // If nothing was recognized, a complex type will be created
            return new ComplexProperty(name, typeInfo.Type);
        }

        private bool FillComplexProperty(ComplexProperty property, InternalTypeInfo typeInfo, object value)
        {
            if (property == null)
                return false;

            // Parsing properties
            ParseProperties(property, typeInfo, value);

            return true;
        }

        private void ParseProperties(ComplexProperty property, InternalTypeInfo typeInfo, object value)
        {
            var propertyInfos = _propertyProvider.GetProperties(typeInfo);
            foreach (var propertyInfo in propertyInfos)
            {
                var subValue = propertyInfo.GetValue(value, _emptyObjectArray);

                var subProperty = CreateProperty(propertyInfo.Name, subValue);

                property.Properties.Add(subProperty);
            }
        }


        private bool FillCollectionProperty(CollectionProperty property, InternalTypeInfo info, object value)
        {
            if (property == null)
                return false;

            // Parsing properties
            ParseProperties(property, info, value);

            // Parse Items
            ParseCollectionItems(property, info, value);

            return true;
        }

        private void ParseCollectionItems(CollectionProperty property, InternalTypeInfo info, object value)
        {
            property.ElementType = info.ElementType;

            var collection = (IEnumerable) value;
            foreach (var item in collection)
            {
                var itemProperty = CreateProperty(null, item);

                property.Items.Add(itemProperty);
            }
        }

        private bool FillDictionaryProperty(DictionaryProperty property, InternalTypeInfo info, object value)
        {
            if (property == null)
                return false;

            // Properties
            ParseProperties(property, info, value);

            // Items
            ParseDictionaryItems(property, info, value);

            return true;
        }

        private void ParseDictionaryItems(DictionaryProperty property, InternalTypeInfo info, object value)
        {
            property.KeyType = info.KeyType;
            property.ValueType = info.ElementType;

            var dictionary = (IDictionary) value;
            foreach (DictionaryEntry entry in dictionary)
            {
                var keyProperty = CreateProperty(null, entry.Key);

                var valueProperty = CreateProperty(null, entry.Value);

                property.Items.Add(new KeyValueItem(keyProperty, valueProperty));
            }
        }

        private bool FillMultiDimensionalArrayProperty(MultiDimensionalArrayProperty property, InternalTypeInfo info,
            object value)
        {
            if (property == null)
                return false;
            property.ElementType = info.ElementType;

            var analyzer = new ArrayAnalyzer(value);

            // DimensionInfos
            property.DimensionInfos = analyzer.ArrayInfo.DimensionInfos;

            // Items
            foreach (var indexSet in analyzer.GetIndexes())
            {
                var subValue = ((Array) value).GetValue(indexSet);
                var itemProperty = CreateProperty(null, subValue);

                property.Items.Add(new MultiDimensionalArrayItem(indexSet, itemProperty));
            }
            return true;
        }

        private bool FillSingleDimensionalArrayProperty(SingleDimensionalArrayProperty property, InternalTypeInfo info,
            object value)
        {
            if (property == null)
                return false;

            property.ElementType = info.ElementType;

            var analyzer = new ArrayAnalyzer(value);

            // Dimensionen
            var dimensionInfo = analyzer.ArrayInfo.DimensionInfos[0];
            property.LowerBound = dimensionInfo.LowerBound;

            // Items
            foreach (object item in analyzer.GetValues())
            {
                var itemProperty = CreateProperty(null, item);

                property.Items.Add(itemProperty);
            }

            return true;
        }

        private static Property CreateSimpleProperty(string name, InternalTypeInfo typeInfo, object value)
        {
            if (!typeInfo.IsSimple) return null;
            var result = new SimpleProperty(name, typeInfo.Type)
            {
                Value = value
            };
            return result;
        }
    }
}