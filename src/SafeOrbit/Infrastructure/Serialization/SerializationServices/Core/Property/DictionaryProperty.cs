
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

using System;
using System.Collections.Generic;

namespace SafeOrbit.Infrastructure.Serialization.SerializationServices.Core
{
    /// <summary>
    ///     Represents dictionary. Every item is composed of the key and value
    /// </summary>
    internal sealed class DictionaryProperty : ComplexProperty
    {
        private IList<KeyValueItem> _items;

        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public DictionaryProperty(string name, Type type)
            : base(name, type)
        {
        }

        ///<summary>
        ///</summary>
        public IList<KeyValueItem> Items
        {
            get
            {
                if (_items == null) _items = new List<KeyValueItem>();
                return _items;
            }
            set => _items = value;
        }

        /// <summary>
        ///     Of what type are keys
        /// </summary>
        public Type KeyType { get; set; }

        /// <summary>
        ///     Of what type are values
        /// </summary>
        public Type ValueType { get; set; }

        /// <summary>
        ///     Makes flat copy (only references) of vital properties
        /// </summary>
        /// <param name="source"></param>
        public override void MakeFlatCopyFrom(ReferenceTargetProperty source)
        {
            var dictionarySource = source as DictionaryProperty;
            if (dictionarySource == null)
                throw new InvalidCastException(
                    $"Invalid property type to make a flat copy. Expected {typeof(DictionaryProperty)}, current {source.GetType()}");

            base.MakeFlatCopyFrom(source);

            KeyType = dictionarySource.KeyType;
            ValueType = dictionarySource.ValueType;
            Items = dictionarySource.Items;
        }

        /// <summary>
        ///     Gets the property art.
        /// </summary>
        /// <returns></returns>
        protected override PropertyArt GetPropertyArt()
        {
            return PropertyArt.Dictionary;
        }
    }
}