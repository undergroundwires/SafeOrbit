
/*
MIT License

Copyright (c) 2016-2018 Erkin Ekici - undergroundwires@safeorb.it

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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SafeOrbit.Infrastructure.Serialization.SerializationServices.Advanced.Binary;
using SafeOrbit.Infrastructure.Serialization.SerializationServices.Advanced.Deserializing;
using SafeOrbit.Infrastructure.Serialization.SerializationServices.Core;
using SafeOrbit.Infrastructure.Serialization.SerializationServices.Core.Binary;

namespace SafeOrbit.Infrastructure.Serialization.SerializationServices.Advanced
{
    /// <summary>
    ///   Contains logic to deserialize data from a binary format. Format can vary according to the used IBinaryWriter. 
    ///   For data serialized with BurstBinaryWriter you use BurstBinaryReader and for SizeOptimizedBinaryWriter you use SizeOptimizedBinaryReader
    /// </summary>
    internal sealed class BinaryPropertyDeserializer : IPropertyDeserializer
    {
        private readonly IBinaryReader _reader;

        /// <summary>
        /// Properties already processed. Used for reference resolution.
        /// </summary>
        private readonly Dictionary<int, ReferenceTargetProperty> _propertyCache =
            new Dictionary<int, ReferenceTargetProperty>();


        public BinaryPropertyDeserializer(IBinaryReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            _reader = reader;
        }

        #region IPropertyDeserializer Members

        /// <summary>
        ///   Open the stream to read
        /// </summary>
        /// <param name = "stream"></param>
        public void Open(Stream stream)
        {
            _reader.Open(stream);
        }

        /// <summary>
        ///   Reading the property
        /// </summary>
        /// <returns></returns>
        public Property Deserialize()
        {
            var elementId = _reader.ReadElementId();
            return Deserialize(elementId, null);
        }

        /// <summary>
        ///   Cleans all
        /// </summary>
        public void Close()
        {
            _reader.Close();
        }

        #endregion

        private Property Deserialize(byte elementId, Type expectedType)
        {
            // Estimate property name
            var propertyName = _reader.ReadName();
            return Deserialize(elementId, propertyName, expectedType);
        }

        private Property Deserialize(byte elementId, string propertyName, Type expectedType)
        {
            // Estimate property type
            var propertyType = _reader.ReadType();

            // id propertyType is not defined, we'll take the expectedType
            if (propertyType == null)
            {
                propertyType = expectedType;
            }

            var referenceId = 0;
            if (elementId == Elements.Reference || Elements.IsElementWithId(elementId))
            {
                referenceId = _reader.ReadNumber();

                if (elementId == Elements.Reference)
                {
                    // This is reference
                    // Get property from the cache
                    return CreateProperty(referenceId, propertyName, propertyType);
                }
            }

            // create the property
            var property = CreateProperty(elementId, propertyName, propertyType);
            if (property == null) return null;

            // Null property?
            var nullProperty = property as NullProperty;
            if (nullProperty != null)
            {
                return nullProperty;
            }

            // is it simple property?
            var simpleProperty = property as SimpleProperty;
            if (simpleProperty != null)
            {
                ParseSimpleProperty(simpleProperty);
                return simpleProperty;
            }

            var referenceProperty = property as ReferenceTargetProperty;
            if (referenceProperty != null)
            {
                if (referenceId > 0)
                {
                    // object is used multiple times
                    referenceProperty.Reference = new ReferenceInfo
                    {
                        Id = referenceId,
                        IsProcessed = true
                    };
                    _propertyCache.Add(referenceId, referenceProperty);
                }
            }

            var multiDimensionalArrayProperty = property as MultiDimensionalArrayProperty;
            if (multiDimensionalArrayProperty != null)
            {
                ParseMultiDimensionalArrayProperty(multiDimensionalArrayProperty);
                return multiDimensionalArrayProperty;
            }

            var singleDimensionalArrayProperty = property as SingleDimensionalArrayProperty;
            if (singleDimensionalArrayProperty != null)
            {
                ParseSingleDimensionalArrayProperty(singleDimensionalArrayProperty);
                return singleDimensionalArrayProperty;
            }

            var dictionaryProperty = property as DictionaryProperty;
            if (dictionaryProperty != null)
            {
                ParseDictionaryProperty(dictionaryProperty);
                return dictionaryProperty;
            }

            var collectionProperty = property as CollectionProperty;
            if (collectionProperty != null)
            {
                ParseCollectionProperty(collectionProperty);
                return collectionProperty;
            }

            var complexProperty = property as ComplexProperty;
            if (complexProperty != null)
            {
                ParseComplexProperty(complexProperty);
                return complexProperty;
            }

            return property;
        }

        private void ParseComplexProperty(ComplexProperty property)
        {
            // There are properties
            ReadProperties(property.Properties, property.Type);
        }

        private void ReadProperties(PropertyCollection properties, Type ownerType)
        {
            var count = _reader.ReadNumber();
            for (var i = 0; i < count; i++)
            {
                var elementId = _reader.ReadElementId();

                var propertyName = _reader.ReadName();

                // estimating the propertyInfo
                var subPropertyInfo = ownerType.GetProperty(propertyName);
                var propertyType = subPropertyInfo != null ? subPropertyInfo.PropertyType : null;
                var subProperty = Deserialize(elementId, propertyName, propertyType);
                properties.Add(subProperty);
            }
        }

        private void ParseCollectionProperty(CollectionProperty property)
        {
            // Element type
            property.ElementType = _reader.ReadType();

            // Properties
            ReadProperties(property.Properties, property.Type);

            // Items
            ReadItems(property.Items, property.ElementType);
        }

        private void ParseDictionaryProperty(DictionaryProperty property)
        {
            // expected key type
            property.KeyType = _reader.ReadType();

            // expected value type
            property.ValueType = _reader.ReadType();

            // Properties
            ReadProperties(property.Properties, property.Type);

            // Items
            ReadDictionaryItems(property.Items, property.KeyType, property.ValueType);
        }

        private void ReadDictionaryItems(IList<KeyValueItem> items, Type expectedKeyType, Type expectedValueType)
        {
            // count
            var count = _reader.ReadNumber();

            // items
            for (var i = 0; i < count; i++)
            {
                ReadDictionaryItem(items, expectedKeyType, expectedValueType);
            }
        }

        private void ReadDictionaryItem(ICollection<KeyValueItem> items, Type expectedKeyType, Type expectedValueType)
        {
            // key
            var elementId = _reader.ReadElementId();
            var keyProperty = Deserialize(elementId, expectedKeyType);

            // value
            elementId = _reader.ReadElementId();
            var valueProperty = Deserialize(elementId, expectedValueType);

            // add the item
            var item = new KeyValueItem(keyProperty, valueProperty);
            items.Add(item);
        }

        private void ParseSingleDimensionalArrayProperty(SingleDimensionalArrayProperty property)
        {
            // Element type
            property.ElementType = _reader.ReadType();

            // Lower bound
            property.LowerBound = _reader.ReadNumber();

            ReadItems(property.Items, property.ElementType);
        }

        private void ReadItems(ICollection<Property> items, Type expectedElementType)
        {
            var count = _reader.ReadNumber();
            for (var i = 0; i < count; i++)
            {
                var elementId = _reader.ReadElementId();
                var subProperty = Deserialize(elementId, expectedElementType);
                items.Add(subProperty);
            }
        }

        private void ParseMultiDimensionalArrayProperty(MultiDimensionalArrayProperty property)
        {
            // Element Type
            property.ElementType = _reader.ReadType();

            // Dimension Infos
            ReadDimensionInfos(property.DimensionInfos);

            // Items
            ReadMultiDimensionalArrayItems(property.Items, property.ElementType);
        }

        private void ReadMultiDimensionalArrayItems(ICollection<MultiDimensionalArrayItem> items, Type expectedElementType)
        {
            // count
            var count = _reader.ReadNumber();

            // items
            for (var i = 0; i < count; i++)
            {
                ReadMultiDimensionalArrayItem(items, expectedElementType);
            }
        }

        private void ReadMultiDimensionalArrayItem(ICollection<MultiDimensionalArrayItem> items, Type expectedElementType)
        {
            // Coordinates
            var indexes = _reader.ReadNumbers();

            // item itself
            var elementId = _reader.ReadElementId();
            var value = Deserialize(elementId, expectedElementType);
            var item = new MultiDimensionalArrayItem(indexes, value);
            items.Add(item);
        }

        private void ReadDimensionInfos(ICollection<DimensionInfo> dimensionInfos)
        {
            // count
            var count = _reader.ReadNumber();

            // Dimensions
            for (var i = 0; i < count; i++)
            {
                ReadDimensionInfo(dimensionInfos);
            }
        }

        private void ReadDimensionInfo(ICollection<DimensionInfo> dimensionInfos)
        {
            var info = new DimensionInfo
            {
                Length = _reader.ReadNumber(),
                LowerBound = _reader.ReadNumber()
            };
            dimensionInfos.Add(info);
        }

        private void ParseSimpleProperty(SimpleProperty property)
        {
            // There is value
            property.Value = _reader.ReadValue(property.Type);
        }

        private static Property CreateProperty(byte elementId, string propertyName, Type propertyType)
        {
            switch (elementId)
            {
                case Elements.SimpleObject:
                    return new SimpleProperty(propertyName, propertyType);
                case Elements.ComplexObject:
                case Elements.ComplexObjectWithId:
                    return new ComplexProperty(propertyName, propertyType);
                case Elements.Collection:
                case Elements.CollectionWithId:
                    return new CollectionProperty(propertyName, propertyType);
                case Elements.Dictionary:
                case Elements.DictionaryWithId:
                    return new DictionaryProperty(propertyName, propertyType);
                case Elements.SingleArray:
                case Elements.SingleArrayWithId:
                    return new SingleDimensionalArrayProperty(propertyName, propertyType);
                case Elements.MultiArray:
                case Elements.MultiArrayWithId:
                    return new MultiDimensionalArrayProperty(propertyName, propertyType);
                case Elements.Null:
                    return new NullProperty(propertyName);
                default:
                    return null;
            }
        }

        private Property CreateProperty(int referenceId, string propertyName, Type propertyType)
        {
            var cachedProperty = _propertyCache[referenceId];
            var property = (ReferenceTargetProperty)Property.CreateInstance(cachedProperty.Art, propertyName, propertyType);
            cachedProperty.Reference.Count++;
            property.MakeFlatCopyFrom(cachedProperty);
            // Reference must be recreated, cause IsProcessed differs for reference and the full property
            property.Reference = new ReferenceInfo() { Id = referenceId };
            return property;
        }
    }
}