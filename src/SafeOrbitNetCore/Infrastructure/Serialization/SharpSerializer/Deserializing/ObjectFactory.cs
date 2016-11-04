
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
using System.Reflection;
using SafeOrbit.Infrastructure.Serialization.SerializationServices.Core;

namespace SafeOrbit.Infrastructure.Serialization.SerializationServices.Deserializing
{
    /// <summary>
    ///     Takes Property and converts it to an object
    /// </summary>
    internal sealed class ObjectFactory
    {
        private readonly object[] _emptyObjectArray = new object[0];

        /// <summary>
        ///     Contains already created objects. Is used for reference resolving.
        /// </summary>
        private readonly Dictionary<int, object> _objectCache = new Dictionary<int, object>();

        /// <summary>
        ///     Builds object from property
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public object CreateObject(Property property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));

            // Is it NullProperty?
            var nullProperty = property as NullProperty;
            if (nullProperty != null)
                return null;

            if (property.Type == null)
                throw new InvalidOperationException($"Property type is not defined. Property: \"{property.Name}\"");

            // Is it SimpleProperty?
            var simpleProperty = property as SimpleProperty;
            if (simpleProperty != null)
                return CreateObjectFromSimpleProperty(simpleProperty);

            var referenceTarget = property as ReferenceTargetProperty;
            if (referenceTarget == null)
                return null;

            if (referenceTarget.Reference != null)
                if (!referenceTarget.Reference.IsProcessed)
                    return _objectCache[referenceTarget.Reference.Id];

            var value = CreateObjectCore(referenceTarget);
            if (value != null)
                return value;

            // No idea what it is
            throw new InvalidOperationException($"Unknown Property type: {property.GetType().Name}");
        }

        private object CreateObjectCore(ReferenceTargetProperty property)
        {
            // Is it multidimensional array?
            var multiDimensionalArrayProperty = property as MultiDimensionalArrayProperty;
            if (multiDimensionalArrayProperty != null)
                return CreateObjectFromMultidimensionalArrayProperty(multiDimensionalArrayProperty);

            // Is it single dimensional array?
            var singleDimensionalArrayProperty = property as SingleDimensionalArrayProperty;
            if (singleDimensionalArrayProperty != null)
                return CreateObjectFromSingleDimensionalArrayProperty(singleDimensionalArrayProperty);

            // Is it dictionary?
            var dictionaryProperty = property as DictionaryProperty;
            if (dictionaryProperty != null)
                return CreateObjectFromDictionaryProperty(dictionaryProperty);

            // Is it collection?
            var collectionProperty = property as CollectionProperty;
            if (collectionProperty != null)
                return CreateObjectFromCollectionProperty(collectionProperty);

            // Is it complex type? Class? Structure?
            var complexProperty = property as ComplexProperty;
            if (complexProperty != null)
                return CreateObjectFromComplexProperty(complexProperty);

            return null;
        }

        private static object CreateObjectFromSimpleProperty(SimpleProperty property)
        {
            return property.Value;
        }

        private object CreateObjectFromComplexProperty(ComplexProperty property)
        {
            var obj = Tools.CreateInstance(property.Type);

            if (property.Reference != null)
                _objectCache.Add(property.Reference.Id, obj);

            FillProperties(obj, property.Properties);

            return obj;
        }


        private object CreateObjectFromCollectionProperty(CollectionProperty property)
        {
            var type = property.Type;
            var collection = Tools.CreateInstance(type);

            if (property.Reference != null)
                _objectCache.Add(property.Reference.Id, collection);

            // fill the properties
            FillProperties(collection, property.Properties);

            // Fill the items but only if the "Add" method was found, which has only 1 parameter
            var methodInfo = collection.GetType().GetTypeInfo().GetMethod("Add");
            var parameters = methodInfo?.GetParameters();
            if (parameters?.Length == 1)
                foreach (var item in property.Items)
                {
                    var value = CreateObject(item);
                    methodInfo.Invoke(collection, new[] {value});
                }
            return collection;
        }

        /// <summary>
        ///     Items will be added only if the "Add" method was found, which exactly 2 parameters (key, value) has
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private object CreateObjectFromDictionaryProperty(DictionaryProperty property)
        {
            var dictionary = Tools.CreateInstance(property.Type);

            if (property.Reference != null)
                _objectCache.Add(property.Reference.Id, dictionary);

            // fill the properties
            FillProperties(dictionary, property.Properties);

            // fill items, but only if Add(key, value) was found
            var methodInfo = dictionary.GetType().GetMethod("Add");
            var parameters = methodInfo?.GetParameters();
            if (parameters?.Length == 2)
                foreach (var item in property.Items)
                {
                    var keyValue = CreateObject(item.Key);
                    var valueValue = CreateObject(item.Value);

                    methodInfo.Invoke(dictionary, new[] {keyValue, valueValue});
                }


            return dictionary;
        }

        /// <summary>
        ///     Fill properties of the class or structure
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="properties"></param>
        private void FillProperties(object obj, IEnumerable<Property> properties)
        {
            foreach (var property in properties)
            {
                var propertyInfo = obj.GetType().GetTypeInfo().GetProperty(property.Name);
                if (propertyInfo == null) continue;

                var value = CreateObject(property);
                if (value == null) continue;

                propertyInfo.SetValue(obj, value, _emptyObjectArray);
            }
        }

        private object CreateObjectFromSingleDimensionalArrayProperty(SingleDimensionalArrayProperty property)
        {
            int itemsCount = property.Items.Count;

            var array = CreateArrayInstance(property.ElementType, new[] {itemsCount}, new[] {property.LowerBound});

            if (property.Reference != null)
                _objectCache.Add(property.Reference.Id, array);

            // Items
            for (var index = property.LowerBound; index < property.LowerBound + itemsCount; index++)
            {
                Property item = property.Items[index];
                var value = CreateObject(item);
                if (value != null)
                    array.SetValue(value, index);
            }

            return array;
        }

        private object CreateObjectFromMultidimensionalArrayProperty(MultiDimensionalArrayProperty property)
        {
            // determine array type
            var creatingInfo =
                GetMultiDimensionalArrayCreatingInfo(property.DimensionInfos);

            // Instantiating the array
            var array = CreateArrayInstance(property.ElementType, creatingInfo.Lengths, creatingInfo.LowerBounds);

            if (property.Reference != null)
                _objectCache.Add(property.Reference.Id, array);

            // fill the values
            foreach (var item in property.Items)
            {
                var value = CreateObject(item.Value);
                if (value != null)
                    array.SetValue(value, item.Indexes);
            }

            return array;
        }

        private static Array CreateArrayInstance(Type elementType, int[] lengths, int[] lowerBounds)
        {
#if PORTABLE
            return Array.CreateInstance(elementType, lengths);
#elif SILVERLIGHT
            return Array.CreateInstance(elementType, lengths);
#else
            return Array.CreateInstance(elementType, lengths, lowerBounds);
#endif
        }

        /// <summary>
        ///     This internal class helps to instantiate the multidimensional array
        /// </summary>
        /// <param name="infos"></param>
        /// <returns></returns>
        private static MultiDimensionalArrayCreatingInfo GetMultiDimensionalArrayCreatingInfo(
            IEnumerable<DimensionInfo> infos)
        {
            var lengths = new List<int>();
            var lowerBounds = new List<int>();
            foreach (var info in infos)
            {
                lengths.Add(info.Length);
                lowerBounds.Add(info.LowerBound);
            }

            var result = new MultiDimensionalArrayCreatingInfo();
            result.Lengths = lengths.ToArray();
            result.LowerBounds = lowerBounds.ToArray();
            return result;
        }

        #region Nested type: MultiDimensionalArrayCreatingInfo

        private class MultiDimensionalArrayCreatingInfo
        {
            public int[] Lengths { get; set; }
            public int[] LowerBounds { get; set; }
        }

        #endregion
    }
}