
/*
MIT License

Copyright (c) 2018 Erkin Ekici - undergroundwires@safeorb.it

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
using SafeOrbit.Infrastructure.Serialization.SerializationServices.Core;

namespace SafeOrbit.Infrastructure.Serialization.SerializationServices.Serializing
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