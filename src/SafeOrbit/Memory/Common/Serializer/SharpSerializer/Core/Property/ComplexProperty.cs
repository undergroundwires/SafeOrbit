using System;

namespace SafeOrbit.Memory.Serialization.SerializationServices.Core
{
    /// <summary>
    ///     Represents complex type which contains properties.
    /// </summary>
    internal class ComplexProperty : ReferenceTargetProperty
    {
        private PropertyCollection _properties;

        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public ComplexProperty(string name, Type type)
            : base(name, type)
        {
        }

        ///<summary>
        ///</summary>
        public PropertyCollection Properties
        {
            get
            {
                if (_properties == null) _properties = new PropertyCollection {Parent = this};
                return _properties;
            }
            set { _properties = value; }
        }


        /// <summary>
        ///     Makes flat copy (only references) of vital properties
        /// </summary>
        /// <param name="source"></param>
        public override void MakeFlatCopyFrom(ReferenceTargetProperty source)
        {
            var complexProperty = source as ComplexProperty;
            if (complexProperty == null)
                throw new InvalidCastException(
                    $"Invalid property type to make a flat copy. Expected {typeof(ComplexProperty)}, current {source.GetType()}");

            base.MakeFlatCopyFrom(source);

            Properties = complexProperty.Properties;
        }

        /// <summary>
        ///     Gets the property art.
        /// </summary>
        /// <returns></returns>
        protected override PropertyArt GetPropertyArt()
        {
            return PropertyArt.Complex;
        }
    }
}