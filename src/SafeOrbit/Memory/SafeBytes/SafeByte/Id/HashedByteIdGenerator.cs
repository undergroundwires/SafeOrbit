
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
using System.Collections.Generic;
using System.Linq;
using SafeOrbit.Cryptography.Hashers;
using SafeOrbit.Cryptography.Random;
using SafeOrbit.Exceptions;
using SafeOrbit.Library;
using SafeOrbit.Memory.SafeBytesServices.DataProtection;

namespace SafeOrbit.Memory.SafeBytesServices.Id
{
    /// <summary>
    ///     Creates a unique <see cref="int" /> values for each <see cref="byte" />.
    ///     The class is stateless but session-based. <see cref="SessionSalt" /> is different values each time the application
    ///     is loaded.
    /// </summary>
    /// <seealso cref="SessionSalt" />
    /// <seealso cref="IByteIdGenerator" />
    internal class HashedByteIdGenerator : IByteIdGenerator
    {
        private const int SaltLength = 16;
        private static byte[] _sessionSalt;
        private readonly IFastHasher _fastHasher;
        private readonly IByteArrayProtector _memoryProtector;
        private readonly ISafeRandom _safeRandom;
        private bool _isSaltEncrypted;

        /// <exception cref="MemoryInjectionException">The object has been injected.</exception>
        public HashedByteIdGenerator() : this(
            SafeOrbitCore.Current.Factory.Get<IFastHasher>(),
            SafeOrbitCore.Current.Factory.Get<ISafeRandom>(),
            SafeOrbitCore.Current.Factory.Get<IByteArrayProtector>())
        {
        }

        internal HashedByteIdGenerator(IFastHasher fastHasher, ISafeRandom safeRandom,
            IByteArrayProtector memoryProtector)
        {
            if (fastHasher == null) throw new ArgumentNullException(nameof(fastHasher));
            if (safeRandom == null) throw new ArgumentNullException(nameof(safeRandom));
            if (memoryProtector == null) throw new ArgumentNullException(nameof(memoryProtector));
            _fastHasher = fastHasher;
            _safeRandom = safeRandom;
            _memoryProtector = memoryProtector;
        }

        public byte[] SessionSalt => _sessionSalt = _sessionSalt ?? GenerateSessionSalt();
        public int Generate(byte b) => GenerateInternal(b, SessionSalt, ref _isSaltEncrypted);

        /// <summary>
        ///     Clears the session salt. A new session salt will created lazily upon request. Please keep in my that requesting
        ///     a new session salt will break all of the SafeOrbit instances and should never be used at all.
        /// </summary>
        internal static void ClearSessionSalt() => _sessionSalt = null;

        private int GenerateInternal(byte b, byte[] salt, ref bool isSaltEncrypted, bool doNotEncrypt = false)
        {
            var byteBuffer = new byte[salt.Length + 1];
            try
            {
                //Decrypt salt
                if (isSaltEncrypted)
                {
                    _memoryProtector.Unprotect(salt);
                    isSaltEncrypted = false;
                }
                //Append salt + byte
                Array.Copy(salt, byteBuffer, salt.Length);
                byteBuffer[salt.Length] = b;
                //Hash it
                var result = _fastHasher.ComputeFast(byteBuffer);
                return result;
            }
            finally
            {
                Array.Clear(byteBuffer, 0, byteBuffer.Length);
                //Encrypt the salt
                if (!isSaltEncrypted && !doNotEncrypt)
                {
                    _memoryProtector.Protect(salt);
                    isSaltEncrypted = true;
                }
            }
        }

        /// <summary>
        ///     Recursive method that generates and returns a validated session salt.
        /// </summary>
        /// <seealso cref="IsSaltValid" />
        private byte[] GenerateSessionSalt()
        {
            var salt = _safeRandom.GetBytes(SaltLength);
            if (!IsSaltValid(salt)) return GenerateSessionSalt(); //try until it's valid
            _memoryProtector.Protect(salt);
            _isSaltEncrypted = true;
            return salt;
        }

        /// <summary>
        ///     Determines if all byte id's will have different result with the given salt.
        /// </summary>
        private bool IsSaltValid(byte[] salt)
        {
            const int totalBytes = 256;
            //generate all possible bytes
            var byteHashes = GetHashesForAllBytes(salt, false);
            //check if they're all unique.
            var totalUniqueHashes = byteHashes.Distinct().Count();
            return totalUniqueHashes == totalBytes;
        }

        private IEnumerable<int> GetHashesForAllBytes(byte[] salt, bool isEncrypted)
        {
            const int totalBytes = 256;
            var byteHashes = new int[totalBytes];
            //Fast.For(0, totalBytes, (i) => byteHashes[i] = this.GenerateInternal((byte)i, salt, ref isEncrypted));
            for (var i = 0; i < 256; i++) byteHashes[i] = GenerateInternal((byte) i, salt, ref isEncrypted, true);
            return byteHashes;
        }
    }
}