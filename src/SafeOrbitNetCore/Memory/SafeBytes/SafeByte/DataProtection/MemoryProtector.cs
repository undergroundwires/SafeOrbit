
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
using System.Security.Cryptography;
using SafeOrbit.Memory.SafeBytesServices.DataProtection;

namespace SafeOrbit.Memory.SafeBytesServices.DataProtection
{
    /// <summary>
    ///     A wrapper for <see cref="ProtectedMemory" />.
    /// </summary>
    /// <seealso cref="IByteArrayProtector" />
    /// <seealso cref="ProtectedMemory" />
    public class MemoryProtector : IByteArrayProtector
    {
        public int BlockSize => 16;

        public void Protect(byte[] userData)
        {
            if (userData == null) throw new ArgumentNullException(nameof(userData));
            if (userData.Length%BlockSize != 0)
                throw new ArgumentOutOfRangeException($"Size of {nameof(userData)} must be {BlockSize}");
            ProtectedMemory.Protect(userData, MemoryProtectionScope.SameProcess);
        }

        public void Unprotect(byte[] encryptedData)
        {
            if (encryptedData == null) throw new ArgumentNullException(nameof(encryptedData));
            if (encryptedData.Length%BlockSize != 0)
                throw new ArgumentOutOfRangeException($"Size of {nameof(encryptedData)} must be {BlockSize}");
            ProtectedMemory.Unprotect(encryptedData, MemoryProtectionScope.SameProcess);
        }
    }
}