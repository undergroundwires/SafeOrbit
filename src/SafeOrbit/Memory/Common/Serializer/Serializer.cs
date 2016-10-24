
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

using System;
using System.IO;
using SafeOrbit.Memory.Serialization.SerializationServices;

namespace SafeOrbit.Memory.Serialization
{
    public class Serializer : ISerializer
    {
        private static readonly Lazy<Serializer> StaticInstanceLazy = new Lazy<Serializer>();

        /// <summary>
        ///     Static instance to re-use for better performance.
        /// </summary>
        private static readonly SharpSerializer BinarySerializer = new SharpSerializer();

        /// <summary>
        ///     Gets the static instance of <see cref="Serialize" />.
        /// </summary>
        /// <value>Value of inner <seealso cref="Lazy{Serializer}" /> instance.</value>
        public static Serializer StaticInstance => StaticInstanceLazy.Value;
        /// <summary>
        /// Serializes the specified object to a byte array.
        /// </summary>
        /// <param name="object">The object to serialize.</param>
        /// <returns>Byte array for the serialization of the object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="@object" /> is <see langword="null" />.</exception>
        public byte[] Serialize(object @object)
        {
            if (@object == null) throw new ArgumentNullException(nameof(@object));
            using (var mStream = new MemoryStream())
            {
                BinarySerializer.Serialize(@object, mStream);
                return mStream.ToArray();
            }
        }
        /// <summary>
        /// Deserializes the specified byte array to an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="byteArray">The byte array to deserialize.</param>
        /// <returns>Deserialized object from bytes.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="byteArray" /> is <see langword="null" />.</exception>
        public T Deserialize<T>(byte[] byteArray)
        {
            if (byteArray == null) throw new ArgumentNullException(nameof(byteArray));
            using (var mStream = new MemoryStream(byteArray))
            {
                return (T)BinarySerializer.Deserialize(mStream);
            }
        }
    }
}