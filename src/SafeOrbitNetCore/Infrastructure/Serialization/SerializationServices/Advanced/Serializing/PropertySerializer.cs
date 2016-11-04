
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
using System.IO;
using SafeOrbit.Infrastructure.Serialization.SerializationServices.Core;
using SafeOrbit.Infrastructure.Serialization.SerializationServices.Serializing;

namespace SafeOrbit.Infrastructure.Serialization.SerializationServices.Advanced.Serializing
{
    /// <summary>
    ///   Base class for all Serializers (Xml, Binary, ...). XmlPropertySerializer inherits from this class
    /// </summary>
    internal abstract class PropertySerializer : IPropertySerializer
    {
        #region IPropertySerializer Members

        /// <summary>
        ///   Serializes property
        /// </summary>
        /// <param name = "property"></param>
        public void Serialize(Property property)
        {
            SerializeCore(new PropertyTypeInfo<Property>(property, null));
        }

        /// <summary>
        ///   Open the stream for writing
        /// </summary>
        /// <param name = "stream"></param>
        public abstract void Open(Stream stream);

        /// <summary>
        ///   Cleaning, but the stream can be used further
        /// </summary>
        public abstract void Close();

        #endregion

        /// <summary>
        /// </summary>
        /// <param name = "property"></param>
        protected void SerializeCore(PropertyTypeInfo<Property> property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));

            var nullProperty = property.Property as NullProperty;
            if (nullProperty != null)
            {
                SerializeNullProperty(new PropertyTypeInfo<NullProperty>(nullProperty, property.ExpectedPropertyType,
                                                                         property.ValueType));
                return;
            }

            // check if the value type is equal to the property type.
            // if so, there is no need to explicit define the value type
            if (property.ExpectedPropertyType != null && property.ExpectedPropertyType == property.ValueType)
            {
                // Type is not required, because the property has the same value type as the expected property type
                property.ValueType = null;
            }

            var simpleProperty = property.Property as SimpleProperty;
            if (simpleProperty != null)
            {
                SerializeSimpleProperty(new PropertyTypeInfo<SimpleProperty>(simpleProperty,
                                                                             property.ExpectedPropertyType,
                                                                             property.ValueType));
                return;
            }

            var referenceTarget = property.Property as ReferenceTargetProperty;
            if (referenceTarget != null)
            {
                if (serializeReference(referenceTarget))
                    // Reference to object was serialized
                    return;

                // Full Serializing of the object
                if (SerializeReferenceTarget(new PropertyTypeInfo<ReferenceTargetProperty>(referenceTarget,
                                                                                       property.ExpectedPropertyType,
                                                                                       property.ValueType)))
                {
                    return;
                }
            }

            throw new InvalidOperationException($"Unknown Property: {property.Property.GetType()}");
        }

        private bool SerializeReferenceTarget(PropertyTypeInfo<ReferenceTargetProperty> property)
        {
            var multiDimensionalArrayProperty = property.Property as MultiDimensionalArrayProperty;
            if (multiDimensionalArrayProperty != null)
            {
                multiDimensionalArrayProperty.Reference.IsProcessed = true;
                SerializeMultiDimensionalArrayProperty(
                    new PropertyTypeInfo<MultiDimensionalArrayProperty>(multiDimensionalArrayProperty,
                                                                        property.ExpectedPropertyType,
                                                                        property.ValueType));
                return true;
            }

            var singleDimensionalArrayProperty = property.Property as SingleDimensionalArrayProperty;
            if (singleDimensionalArrayProperty != null)
            {
                singleDimensionalArrayProperty.Reference.IsProcessed = true;
                SerializeSingleDimensionalArrayProperty(
                    new PropertyTypeInfo<SingleDimensionalArrayProperty>(singleDimensionalArrayProperty,
                                                                         property.ExpectedPropertyType,
                                                                         property.ValueType));
                return true;
            }

            var dictionaryProperty = property.Property as DictionaryProperty;
            if (dictionaryProperty != null)
            {
                dictionaryProperty.Reference.IsProcessed = true;
                SerializeDictionaryProperty(new PropertyTypeInfo<DictionaryProperty>(dictionaryProperty,
                                                                                     property.ExpectedPropertyType,
                                                                                     property.ValueType));
                return true;
            }

            var collectionProperty = property.Property as CollectionProperty;
            if (collectionProperty != null)
            {
                collectionProperty.Reference.IsProcessed = true;
                SerializeCollectionProperty(new PropertyTypeInfo<CollectionProperty>(collectionProperty,
                                                                                     property.ExpectedPropertyType,
                                                                                     property.ValueType));
                return true;
            }

            var complexProperty = property.Property as ComplexProperty;
            if (complexProperty != null)
            {
                complexProperty.Reference.IsProcessed = true;
                SerializeComplexProperty(new PropertyTypeInfo<ComplexProperty>(complexProperty,
                                                                               property.ExpectedPropertyType,
                                                                               property.ValueType));
                return true;
            }

            return false;
        }

        private bool serializeReference(ReferenceTargetProperty property)
        {
            if (property.Reference.Count > 1)
            {
                // There are more references to this object
                if (property.Reference.IsProcessed)
                {
                    // The object is already serialized
                    // Only its reference should be stored
                    SerializeReference(property);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name = "property"></param>
        protected abstract void SerializeNullProperty(PropertyTypeInfo<NullProperty> property);

        /// <summary>
        /// </summary>
        /// <param name = "property"></param>
        protected abstract void SerializeSimpleProperty(PropertyTypeInfo<SimpleProperty> property);

        /// <summary>
        /// </summary>
        /// <param name = "property"></param>
        protected abstract void SerializeMultiDimensionalArrayProperty(
            PropertyTypeInfo<MultiDimensionalArrayProperty> property);

        /// <summary>
        /// </summary>
        /// <param name = "property"></param>
        protected abstract void SerializeSingleDimensionalArrayProperty(
            PropertyTypeInfo<SingleDimensionalArrayProperty> property);

        /// <summary>
        /// </summary>
        /// <param name = "property"></param>
        protected abstract void SerializeDictionaryProperty(PropertyTypeInfo<DictionaryProperty> property);

        /// <summary>
        /// </summary>
        /// <param name = "property"></param>
        protected abstract void SerializeCollectionProperty(PropertyTypeInfo<CollectionProperty> property);

        /// <summary>
        /// </summary>
        /// <param name = "property"></param>
        protected abstract void SerializeComplexProperty(PropertyTypeInfo<ComplexProperty> property);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="referenceTarget"></param>
        protected abstract void SerializeReference(ReferenceTargetProperty referenceTarget);
    }
}