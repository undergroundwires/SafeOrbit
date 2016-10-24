
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

namespace SafeOrbit.Memory.Serialization.SerializationServices.Core
{
    /// <summary>
    ///     Base class for all properties. Every object can be defined with inheritors of the Property class.
    /// </summary>
    internal abstract class Property
    {
        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        protected Property(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        /// <summary>
        ///     Not all properties have name (i.e. items of a collection)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Of what type is the property or its value
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        ///     If the properties are nested, i.e. collection items are nested in the collection
        /// </summary>
        public Property Parent { get; set; }

        /// <summary>
        ///     Of what art is the property.
        /// </summary>
        public PropertyArt Art => GetPropertyArt();

        /// <summary>
        ///     Gets the property art.
        /// </summary>
        /// <returns></returns>
        protected abstract PropertyArt GetPropertyArt();

        /// <summary>
        ///     Creates property from PropertyArt
        /// </summary>
        /// <param name="art"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyType"></param>
        /// <returns>null if PropertyArt.Reference is requested</returns>
        /// <exception cref="InvalidOperationException">If unknown PropertyArt requested</exception>
        public static Property CreateInstance(PropertyArt art, string propertyName, Type propertyType)
        {
            switch (art)
            {
                case PropertyArt.Collection:
                    return new CollectionProperty(propertyName, propertyType);
                case PropertyArt.Complex:
                    return new ComplexProperty(propertyName, propertyType);
                case PropertyArt.Dictionary:
                    return new DictionaryProperty(propertyName, propertyType);
                case PropertyArt.MultiDimensionalArray:
                    return new MultiDimensionalArrayProperty(propertyName, propertyType);
                case PropertyArt.Null:
                    return new NullProperty(propertyName);
                case PropertyArt.Reference:
                    return null;
                case PropertyArt.Simple:
                    return new SimpleProperty(propertyName, propertyType);
                case PropertyArt.SingleDimensionalArray:
                    return new SingleDimensionalArrayProperty(propertyName, propertyType);
                default:
                    throw new InvalidOperationException($"Unknown PropertyArt {art}");
            }
        }


        ///<summary>
        ///</summary>
        ///<returns></returns>
        ///<exception cref="NotImplementedException"></exception>
        public override string ToString()
        {
            var name = Name ?? "null";
            var type = Type == null ? "null" : Type.Name;
            var parent = Parent?.GetType().Name ?? "null";
            return $"{GetType().Name}, Name={name}, Type={type}, Parent={parent}";
        }
    }
}