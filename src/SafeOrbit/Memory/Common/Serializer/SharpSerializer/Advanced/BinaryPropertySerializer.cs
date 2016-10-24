
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
using System.Collections.Generic;
using System.IO;
using SafeOrbit.Memory.Serialization.SerializationServices.Advanced.Binary;
using SafeOrbit.Memory.Serialization.SerializationServices.Advanced.Serializing;
using SafeOrbit.Memory.Serialization.SerializationServices.Core;
using SafeOrbit.Memory.Serialization.SerializationServices.Core.Binary;
using SafeOrbit.Memory.Serialization.SerializationServices.Serializing;

namespace SafeOrbit.Memory.Serialization.SerializationServices.Advanced
{
    /// <summary>
    ///     Contains logic to serialize data to a binary format. Format varies according to the used IBinaryWriter.
    ///     Actually there are BurstBinaryWriter and SizeOptimizedBinaryWriter (see the constructor)
    /// </summary>
    internal sealed class BinaryPropertySerializer : PropertySerializer
    {
        private readonly IBinaryWriter _writer;


        /// <summary>
        /// </summary>
        /// <param name="writer"></param>
        public BinaryPropertySerializer(IBinaryWriter writer)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            _writer = writer;
        }

        /// <summary>
        ///     Open the stream for writing
        /// </summary>
        /// <param name="stream" />
        public override void Open(Stream stream)
        {
            _writer.Open(stream);
        }

        /// <summary>
        ///     Closes the stream
        /// </summary>
        public override void Close()
        {
            _writer.Close();
        }


        private void WritePropertyHeader(byte elementId, string name, Type valueType)
        {
            _writer.WriteElementId(elementId);
            _writer.WriteName(name);
            _writer.WriteType(valueType);
        }

        private bool WritePropertyHeaderWithReferenceId(byte elementId, ReferenceInfo info, string name, Type valueType)
        {
            if (info.Count < 2)
                // no need to write id
                return false;
            WritePropertyHeader(elementId, name, valueType);
            _writer.WriteNumber(info.Id);
            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="property"></param>
        protected override void SerializeNullProperty(PropertyTypeInfo<NullProperty> property)
        {
            WritePropertyHeader(Elements.Null, property.Name, property.ValueType);
        }

        /// <summary>
        /// </summary>
        /// <param name="property"></param>
        protected override void SerializeSimpleProperty(PropertyTypeInfo<SimpleProperty> property)
        {
            WritePropertyHeader(Elements.SimpleObject, property.Name, property.ValueType);
            _writer.WriteValue(property.Property.Value);
        }

        /// <summary>
        /// </summary>
        /// <param name="property"></param>
        protected override void SerializeMultiDimensionalArrayProperty(
            PropertyTypeInfo<MultiDimensionalArrayProperty> property)
        {
            if (
                !WritePropertyHeaderWithReferenceId(Elements.MultiArrayWithId, property.Property.Reference,
                    property.Name, property.ValueType))
                WritePropertyHeader(Elements.MultiArray, property.Name, property.ValueType);

            // ElementType
            _writer.WriteType(property.Property.ElementType);

            // DimensionInfos
            WriteDimensionInfos(property.Property.DimensionInfos);

            // Einträge
            WriteMultiDimensionalArrayItems(property.Property.Items, property.Property.ElementType);
        }

        private void WriteMultiDimensionalArrayItems(ICollection<MultiDimensionalArrayItem> items, Type defaultItemType)
        {
            // Count
            _writer.WriteNumber(items.Count);

            // Items
            foreach (var item in items)
                WriteMultiDimensionalArrayItem(item, defaultItemType);
        }

        private void WriteMultiDimensionalArrayItem(MultiDimensionalArrayItem item, Type defaultItemType)
        {
            // Write coordinates
            _writer.WriteNumbers(item.Indexes);

            // Write Data
            SerializeCore(new PropertyTypeInfo<Property>(item.Value, defaultItemType));
        }

        private void WriteDimensionInfos(ICollection<DimensionInfo> dimensionInfos)
        {
            // count
            _writer.WriteNumber(dimensionInfos.Count);

            // items
            foreach (var info in dimensionInfos)
                WriteDimensionInfo(info);
        }

        private void WriteDimensionInfo(DimensionInfo info)
        {
            // Length
            _writer.WriteNumber(info.Length);

            // LowerBound
            _writer.WriteNumber(info.LowerBound);
        }

        /// <summary>
        /// </summary>
        /// <param name="property"></param>
        protected override void SerializeSingleDimensionalArrayProperty(
            PropertyTypeInfo<SingleDimensionalArrayProperty> property)
        {
            if (
                !WritePropertyHeaderWithReferenceId(Elements.SingleArrayWithId, property.Property.Reference,
                    property.Name, property.ValueType))
                WritePropertyHeader(Elements.SingleArray, property.Name, property.ValueType);

            // ElementType
            _writer.WriteType(property.Property.ElementType);

            // Lower Bound
            _writer.WriteNumber(property.Property.LowerBound);

            // items
            WriteItems(property.Property.Items, property.Property.ElementType);
        }

        private void WriteItems(ICollection<Property> items, Type defaultItemType)
        {
            // Count
            _writer.WriteNumber(items.Count);

            // items
            foreach (var item in items)
                SerializeCore(new PropertyTypeInfo<Property>(item, defaultItemType));
        }

        /// <summary>
        /// </summary>
        /// <param name="property"></param>
        protected override void SerializeDictionaryProperty(PropertyTypeInfo<DictionaryProperty> property)
        {
            if (
                !WritePropertyHeaderWithReferenceId(Elements.DictionaryWithId, property.Property.Reference,
                    property.Name, property.ValueType))
                WritePropertyHeader(Elements.Dictionary, property.Name, property.ValueType);

            // type of keys
            _writer.WriteType(property.Property.KeyType);

            // type of values
            _writer.WriteType(property.Property.ValueType);

            // Properties
            WriteProperties(property.Property.Properties, property.Property.Type);

            // Items
            WriteDictionaryItems(property.Property.Items, property.Property.KeyType, property.Property.ValueType);
        }

        private void WriteDictionaryItems(IList<KeyValueItem> items, Type defaultKeyType, Type defaultValueType)
        {
            // count
            _writer.WriteNumber(items.Count);

            foreach (var item in items)
                WriteDictionaryItem(item, defaultKeyType, defaultValueType);
        }

        private void WriteDictionaryItem(KeyValueItem item, Type defaultKeyType, Type defaultValueType)
        {
            // Key
            SerializeCore(new PropertyTypeInfo<Property>(item.Key, defaultKeyType));

            // Value
            SerializeCore(new PropertyTypeInfo<Property>(item.Value, defaultValueType));
        }

        /// <summary>
        /// </summary>
        /// <param name="property"></param>
        protected override void SerializeCollectionProperty(PropertyTypeInfo<CollectionProperty> property)
        {
            if (
                !WritePropertyHeaderWithReferenceId(Elements.CollectionWithId, property.Property.Reference,
                    property.Name, property.ValueType))
                WritePropertyHeader(Elements.Collection, property.Name, property.ValueType);

            // ElementType
            _writer.WriteType(property.Property.ElementType);

            // Properties
            WriteProperties(property.Property.Properties, property.Property.Type);

            //Items
            WriteItems(property.Property.Items, property.Property.ElementType);
        }

        /// <summary>
        /// </summary>
        /// <param name="property"></param>
        protected override void SerializeComplexProperty(PropertyTypeInfo<ComplexProperty> property)
        {
            if (
                !WritePropertyHeaderWithReferenceId(Elements.ComplexObjectWithId, property.Property.Reference,
                    property.Name, property.ValueType))
                WritePropertyHeader(Elements.ComplexObject, property.Name, property.ValueType);

            // Properties
            WriteProperties(property.Property.Properties, property.Property.Type);
        }

        /// <summary>
        /// </summary>
        /// <param name="referenceTarget"></param>
        protected override void SerializeReference(ReferenceTargetProperty referenceTarget)
        {
            WritePropertyHeader(Elements.Reference, referenceTarget.Name, null);
            _writer.WriteNumber(referenceTarget.Reference.Id);
        }

        private void WriteProperties(PropertyCollection properties, Type ownerType)
        {
            // How many
            _writer.WriteNumber(Convert.ToInt16(properties.Count));

            // Serialize all of them
            foreach (var property in properties)
            {
                var propertyInfo = ownerType.GetProperty(property.Name);
                SerializeCore(new PropertyTypeInfo<Property>(property, propertyInfo.PropertyType));
            }
        }
    }
}