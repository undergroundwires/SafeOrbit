
/*
MIT License

Copyright (c) 2016-2018 Erkin Ekici - undergroundwires@safeorb.it

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

namespace SafeOrbit.Infrastructure.Serialization
{
    /// <summary>
    ///     An abstraction for a generic <see cref="object"/> serializer.
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        ///     Serializes the specified object to a byte array.
        /// </summary>
        /// <param name="object">The object to serialize.</param>
        /// <returns>Byte array for the serialization of the object.</returns>
        byte[] Serialize(object @object);

        /// <summary>
        ///     Deserializes the specified byte array to an object.
        /// </summary>
        /// <param name="byteArray">The byte array to deserialize.</param>
        /// <returns>Deserialized object from bytes.</returns>
        T Deserialize<T>(byte[] byteArray);
    }
}