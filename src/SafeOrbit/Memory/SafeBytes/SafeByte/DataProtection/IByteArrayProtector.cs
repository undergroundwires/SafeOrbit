
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

namespace SafeOrbit.Memory.SafeBytesServices.DataProtection
{
    /// <summary>
    ///     Encrypts and decrypts a <see cref="byte" /> array.
    /// </summary>
    public interface IByteArrayProtector
    {
        /// <summary>
        /// Gets the size of the blocks for encryption to function.
        /// </summary>
        /// <value>The size of the blocks.</value>
        int BlockSizeInBytes { get; }
        /// <summary>
        /// Encrypts the specified user data.
        /// </summary>
        /// <param name="userData">The data to encrypt.</param>
        void Protect(byte[] userData);
        /// <summary>
        /// Decrypts the specified encrypted data.
        /// </summary>
        /// <param name="encryptedData">The encrypted data to decrypt.</param>
        void Unprotect(byte[] encryptedData);
    }
}