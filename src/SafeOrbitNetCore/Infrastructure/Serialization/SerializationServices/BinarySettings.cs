
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

using System.Text;
using SafeOrbit.Infrastructure.Serialization.SerializationServices.Core;
using SafeOrbit.Memory.SafeObject.SharpSerializer;

namespace SafeOrbit.Infrastructure.Serialization.SerializationServices
{
    /// <summary>
    ///     All the most important settings for binary serialization
    /// </summary>
    internal sealed class BinarySettings : SerializerSettings<AdvancedSerializerBinarySettings>
    {
        /// <summary>
        ///     Default constructor. Serialization in <see cref="BinarySerializationMode.SizeOptimized" /> mode.
        ///     For other modes choose an overloaded constructor
        /// </summary>
        /// <seealso cref="BinarySerializationMode" />
        public BinarySettings()
        {
            Encoding = Encoding.UTF8;
        }

        /// <summary>
        ///     Overloaded constructor. Chooses mode in which the data is serialized.
        /// </summary>
        /// <param name="mode">
        ///     <p>
        ///         <see cref="BinarySerializationMode.SizeOptimized" /> - all types are stored in a header, objects only reference
        ///         these types (better for collections).
        ///     </p>
        ///     <p>
        ///         <see cref="BinarySerializationMode.Burst" /> - all types are serialized with their objects (better for
        ///         serializing of single objects).
        ///     </p>
        /// </param>
        /// <seealso cref="BinarySerializationMode" />
        public BinarySettings(BinarySerializationMode mode)
        {
            Encoding = Encoding.UTF8;
            Mode = mode;
        }

        /// <summary>
        ///     How are strings serialized. Default is UTF-8.
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        ///     Default is  <see cref="BinarySerializationMode.SizeOptimized" /> - Types and property names are stored in a header.
        ///     The opposite is <see cref="BinarySerializationMode.Burst" /> mode when all
        ///     types are serialized with their objects.
        /// </summary>
        public BinarySerializationMode Mode { get; set; }
    }
}