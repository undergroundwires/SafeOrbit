using System;

namespace SafeOrbit.Memory.InjectionServices.Stampers.Serialization.SerializationServices.Core.Property
{
    /// <summary>
    ///     Represents one dimensional array
    /// </summary>
    internal sealed class SingleDimensionalArrayProperty : ReferenceTargetProperty
    {
        private PropertyCollection _items;

        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public SingleDimensionalArrayProperty(string name, Type type)
            : base(name, type)
        {
        }

        ///<summary>
        ///</summary>
        public PropertyCollection Items
        {
            get => _items ?? (_items = new PropertyCollection {Parent = this});
            set => _items = value;
        }

        /// <summary>
        ///     As default is 0, but there can be higher start index
        /// </summary>
        public int LowerBound { get; set; }

        /// <summary>
        ///     Of what type are elements
        /// </summary>
        public Type ElementType { get; set; }


        /// <summary>
        ///     Makes flat copy (only references) of vital properties
        /// </summary>
        /// <param name="source"></param>
        public override void MakeFlatCopyFrom(ReferenceTargetProperty source)
        {
            var arrayProp = source as SingleDimensionalArrayProperty;
            if (arrayProp == null)
                throw new InvalidCastException(
                    $"Invalid property type to make a flat copy. Expected {typeof(SingleDimensionalArrayProperty)}, current {source.GetType()}");

            base.MakeFlatCopyFrom(source);

            LowerBound = arrayProp.LowerBound;
            ElementType = arrayProp.ElementType;
            Items = arrayProp.Items;
        }

        /// <summary>
        ///     Gets the property art.
        /// </summary>
        /// <returns></returns>
        protected override PropertyArt GetPropertyArt()
        {
            return PropertyArt.SingleDimensionalArray;
        }
    }
}