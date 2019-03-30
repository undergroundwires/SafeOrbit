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