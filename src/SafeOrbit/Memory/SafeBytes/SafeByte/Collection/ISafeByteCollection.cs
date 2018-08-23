
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

using System;
using SafeOrbit.Memory;

namespace SafeOrbit.Memory.SafeBytesServices.Collection
{
    /// <summary>
    ///     Abstraction of a collection for <see cref="ISafeByte"/> instances.
    /// </summary>
    /// <seealso cref="ISafeByte"/>
    internal interface ISafeByteCollection : IDisposable
    {
        int Length { get; }
        void Append(ISafeByte safeByte);
        ISafeByte Get(int index);
        /// <summary>
        /// Returns all of the real byte values that <see cref="ISafeByteCollection"/> holds.
        /// Reveals all protected data in memory.
        /// Usage with <see cref="SafeMemoryStream"/> is recommended.
        /// </summary>
        /// <returns></returns>
        byte[] ToDecryptedBytes();
    }
}