
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

namespace SafeOrbit.Infrastructure.Serialization.SerializationServices.Core.Binary
{
    /// <summary>
    ///     These elements are used during the binary serialization. They should be unique from SubElements and Attributes.
    /// </summary>
    internal static class Elements
    {
        ///<summary>
        ///</summary>
        public const byte Collection = 1;

        ///<summary>
        ///</summary>
        public const byte ComplexObject = 2;

        ///<summary>
        ///</summary>
        public const byte Dictionary = 3;

        ///<summary>
        ///</summary>
        public const byte MultiArray = 4;

        ///<summary>
        ///</summary>
        public const byte Null = 5;

        ///<summary>
        ///</summary>
        public const byte SimpleObject = 6;

        ///<summary>
        ///</summary>
        public const byte SingleArray = 7;

        /// <summary>
        ///     For binary compatibility reason extra type-id: same as ComplexObjectWith, but contains
        /// </summary>
        public const byte ComplexObjectWithId = 8;

        /// <summary>
        ///     reference to previously serialized  ComplexObjectWithId
        /// </summary>
        public const byte Reference = 9;

        ///<summary>
        ///</summary>
        public const byte CollectionWithId = 10;

        ///<summary>
        ///</summary>
        public const byte DictionaryWithId = 11;

        ///<summary>
        ///</summary>
        public const byte SingleArrayWithId = 12;

        ///<summary>
        ///</summary>
        public const byte MultiArrayWithId = 13;

        ///<summary>
        ///</summary>
        ///<param name="elementId"></param>
        ///<returns></returns>
        public static bool IsElementWithId(byte elementId)
        {
            if (elementId == ComplexObjectWithId)
                return true;
            if (elementId == CollectionWithId)
                return true;
            if (elementId == DictionaryWithId)
                return true;
            if (elementId == SingleArrayWithId)
                return true;
            if (elementId == MultiArrayWithId)
                return true;
            return false;
        }
    }

    /// <summary>
    ///     These elements are used during the binary serialization. They should be unique from Elements and Attributes.
    /// </summary>
    public static class SubElements
    {
        ///<summary>
        ///</summary>
        public const byte Dimension = 51;

        ///<summary>
        ///</summary>
        public const byte Dimensions = 52;

        ///<summary>
        ///</summary>
        public const byte Item = 53;

        ///<summary>
        ///</summary>
        public const byte Items = 54;

        ///<summary>
        ///</summary>
        public const byte Properties = 55;

        ///<summary>
        ///</summary>
        public const byte Unknown = 254;

        ///<summary>
        ///</summary>
        public const byte Eof = 255;
    }

    /// <summary>
    ///     These attributes are used during the binary serialization. They should be unique from Elements and SubElements.
    /// </summary>
    public class Attributes
    {
        ///<summary>
        ///</summary>
        public const byte DimensionCount = 101;

        ///<summary>
        ///</summary>
        public const byte ElementType = 102;

        ///<summary>
        ///</summary>
        public const byte Indexes = 103;

        ///<summary>
        ///</summary>
        public const byte KeyType = 104;

        ///<summary>
        ///</summary>
        public const byte Length = 105;

        ///<summary>
        ///</summary>
        public const byte LowerBound = 106;

        ///<summary>
        ///</summary>
        public const byte Name = 107;

        ///<summary>
        ///</summary>
        public const byte Type = 108;

        ///<summary>
        ///</summary>
        public const byte Value = 109;

        ///<summary>
        ///</summary>
        public const byte ValueType = 110;
    }

    /// <summary>
    ///     How many bytes occupies a number value
    /// </summary>
    public static class NumberSize
    {
        /// <summary>
        ///     is zero
        /// </summary>
        public const byte Zero = 0;

        /// <summary>
        ///     serializes as 1 byte
        /// </summary>
        public const byte B1 = 1;

        /// <summary>
        ///     serializes as 2 bytes
        /// </summary>
        public const byte B2 = 2;

        /// <summary>
        ///     serializes as 4 bytes
        /// </summary>
        public const byte B4 = 4;

        /// <summary>
        ///     Gives the least required byte amount to store the number
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte GetNumberSize(int value)
        {
            if (value == 0) return Zero;
            if ((value > short.MaxValue) || (value < short.MinValue)) return B4;
            if ((value < byte.MinValue) || (value > byte.MaxValue)) return B2;
            return B1;
        }
    }
}