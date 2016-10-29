
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

using SafeOrbit.Infrastructure.Serialization.SerializationServices.Advanced;

namespace SafeOrbit.Memory.SafeObject.SharpSerializer
{
    /// <summary>
    ///     What format has the serialized binary file. It could be SizeOptimized or Burst.
    /// </summary>
    internal enum BinarySerializationMode
    {
        /// <summary>
        ///     All types are serialized to string lists, which are stored in the file header. Duplicates are removed. Serialized
        ///     objects only reference these types. It reduces size especially if serializing collections. Refer to
        ///     <see cref="SizeOptimizedBinaryWriter" /> for more details.
        /// </summary>
        /// <seealso cref="SizeOptimizedBinaryWriter" />
        SizeOptimized = 0,

        /// <summary>
        ///     There are as many type definitions as many objects stored, not regarding if there are duplicate types defined. It
        ///     reduces the overhead if storing single items, but increases the file size if storing collections. See
        ///     <see cref="BurstBinaryWriter" /> for details.
        /// </summary>
        /// <seealso cref="BurstBinaryWriter" />
        Burst
    }
}