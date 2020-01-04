//This is a modified version of the beautiful SharpSerializer by Pawel Idzikowski (see: http://www.sharpserializer.com)

using System;
using SafeOrbit.Core.Serialization.SerializationServices.Core;

namespace SafeOrbit.Core.Serialization.SerializationServices.Serializing
{
    /// <summary>
    ///   Contains info about property and its type.
    ///   It is of use to avoid double type definitions.
    /// </summary>
    /// <typeparam name = "TProperty"></typeparam>
    /// <remarks>
    ///   During serialization is each property wrapped in this class.
    ///   there is no need to define type of every array element if there is a global ElementType type defined
    ///   and each array element type is equal to that global ElementType
    ///   In such a case ElementType is stored in ExpectedPropertyType, ValueType contains null.
    /// </remarks>
    internal sealed class PropertyTypeInfo<TProperty> where TProperty : Property
    {
        ///<summary>
        ///</summary>
        ///<param name = "property"></param>
        ///<param name = "valueType"></param>
        public PropertyTypeInfo(TProperty property, Type valueType)
        {
            Property = property;
            ExpectedPropertyType = valueType;
            ValueType = property.Type;
            Name = property.Name;
        }

        ///<summary>
        ///</summary>
        ///<param name = "property"></param>
        ///<param name = "expectedPropertyType"></param>
        ///<param name = "valueType"></param>
        public PropertyTypeInfo(TProperty property, Type expectedPropertyType, Type valueType)
        {
            Property = property;
            ExpectedPropertyType = expectedPropertyType;
            ValueType = valueType;
            Name = property.Name;
        }

        /// <summary>
        ///   Of what type should be this property
        /// </summary>
        public Type ExpectedPropertyType { get; set; }

        /// <summary>
        ///   Of what type is the property value. If it is null - then the value type is equal to expectedPropertyType
        ///   and does not need to be additionally serialized
        /// </summary>
        public Type ValueType { get; set; }

        /// <summary>
        ///   Property name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///   Property itself
        /// </summary>
        public TProperty Property { get; set; }
    }
}