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

namespace SafeOrbit.Cryptography.Encryption
{
    /// <summary>
    ///     Common interface for all encryption algorithms.
    /// </summary>
    public interface IEncryptor
    {
        /// <summary>
        ///     Minimum size of key in bits for algorithm to function.
        /// </summary>
        /// <value>The minimum size of the key.</value>
        int MinKeySize { get; }

        /// <summary>
        ///     Maximum size of key in bits for algorithm to function.
        /// </summary>
        /// <value>The maximum size of the key.</value>
        int MaxKeySize { get; }

        /// <summary>
        ///     Gets the size of the iv bytes for crypto.
        /// </summary>
        /// <value>The size of the iv bytes that's created for crypto.</value>
        int IvSize { get; }

        /// <summary>
        ///     Gets the size of the block in bits.
        /// </summary>
        /// <remarks>https://en.wikipedia.org/wiki/Block_size_(cryptography)</remarks>
        /// <value>The size of the block in bits.</value>
        int BlockSize { get; }
    }
}