using System;
using System.Collections.Generic;

namespace SafeOrbit.Core.Serialization.SerializationServices.Core
{
    /// <summary>
    ///     Represents type which is ICollection
    /// </summary>
    internal sealed class CollectionProperty : ComplexProperty
    {
        private IList<Property> _items;

        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public CollectionProperty(string name, Type type)
            : base(name, type)
        {
        }

        ///<summary>
        ///</summary>
        public IList<Property> Items
        {
            get
            {
                if (_items == null) _items = new List<Property>();
                return _items;
            }
            set => _items = value;
        }

        /// <summary>
        ///     Of what type are items. It's important for polymorphic collection
        /// </summary>
        public Type ElementType { get; set; }

        /// <summary>
        ///     Makes flat copy (only references) of vital properties
        /// </summary>
        /// <param name="source"></param>
        public override void MakeFlatCopyFrom(ReferenceTargetProperty source)
        {
            var collectionSource = source as CollectionProperty;
            if (collectionSource == null)
                throw new InvalidCastException(
                    $"Invalid property type to make a flat copy. Expected {typeof(CollectionProperty)}, current {source.GetType()}");

            base.MakeFlatCopyFrom(source);

            ElementType = collectionSource.ElementType;
            Items = collectionSource.Items;
        }

        /// <summary>
        ///     Gets the property art.
        /// </summary>
        /// <returns></returns>
        protected override PropertyArt GetPropertyArt()
        {
            return PropertyArt.Collection;
        }
    }
}