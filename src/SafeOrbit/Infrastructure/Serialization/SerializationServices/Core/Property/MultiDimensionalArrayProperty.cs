using System;
using System.Collections.Generic;

namespace SafeOrbit.Infrastructure.Serialization.SerializationServices.Core
{
    /// <summary>
    ///     Represents multidimensional array. Array properties are in DimensionInfos
    /// </summary>
    internal sealed class MultiDimensionalArrayProperty : ReferenceTargetProperty
    {
        private IList<DimensionInfo> _dimensionInfos;
        private IList<MultiDimensionalArrayItem> _items;

        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public MultiDimensionalArrayProperty(string name, Type type)
            : base(name, type)
        {
        }

        ///<summary>
        ///</summary>
        public IList<MultiDimensionalArrayItem> Items
        {
            get => _items ?? (_items = new List<MultiDimensionalArrayItem>());
            set => _items = value;
        }

        /// <summary>
        ///     Information about the array
        /// </summary>
        public IList<DimensionInfo> DimensionInfos
        {
            get => _dimensionInfos ?? (_dimensionInfos = new List<DimensionInfo>());
            set => _dimensionInfos = value;
        }

        /// <summary>
        ///     Of what type are elements. All elements in all all dimensions must be inheritors of this type.
        /// </summary>
        public Type ElementType { get; set; }

        /// <summary>
        ///     Makes flat copy (only references) of vital properties
        /// </summary>
        /// <param name="source"></param>
        public override void MakeFlatCopyFrom(ReferenceTargetProperty source)
        {
            var arrayProp = source as MultiDimensionalArrayProperty;
            if (arrayProp == null)
                throw new InvalidCastException(
                    $"Invalid property type to make a flat copy. Expected {typeof(SingleDimensionalArrayProperty)}, current {source.GetType()}");

            base.MakeFlatCopyFrom(source);

            ElementType = arrayProp.ElementType;
            DimensionInfos = arrayProp.DimensionInfos;
            Items = arrayProp.Items;
        }

        /// <summary>
        ///     Gets the property art.
        /// </summary>
        /// <returns></returns>
        protected override PropertyArt GetPropertyArt()
        {
            return PropertyArt.MultiDimensionalArray;
        }
    }
}