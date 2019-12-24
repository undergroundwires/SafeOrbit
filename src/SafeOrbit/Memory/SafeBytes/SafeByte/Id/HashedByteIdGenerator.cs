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
    ///     It's a singleton so byte ID's are consistent throughout the execution of the process, on restart a new salt & new ID's are generated.
    /// </summary>
    /// <seealso cref="IByteIdGenerator" />
    internal class HashedByteIdGenerator : IByteIdGenerator
    {
        private const int SaltLength = 16;
        private readonly IMemoryProtectedBytes _sessionSalt;
        private readonly IFastHasher _fastHasher;
        private readonly ISafeRandom _safeRandom;

        /// <exception cref="MemoryInjectionException">The object has been injected.</exception>
        public HashedByteIdGenerator() : this(
            SafeOrbitCore.Current.Factory.Get<IFastHasher>(),
            SafeOrbitCore.Current.Factory.Get<ISafeRandom>(),
            SafeOrbitCore.Current.Factory.Get<IMemoryProtectedBytes>())
        {
        }

        internal HashedByteIdGenerator(IFastHasher fastHasher, ISafeRandom safeRandom,
            IMemoryProtectedBytes sessionSalt)
        {
            _fastHasher = fastHasher ?? throw new ArgumentNullException(nameof(fastHasher));
            _safeRandom = safeRandom ?? throw new ArgumentNullException(nameof(safeRandom));
            _sessionSalt = sessionSalt ?? throw new ArgumentNullException(nameof(sessionSalt));
            InitializeSalt();
        }
        public int Generate(byte b)
        {
            using (var salt = _sessionSalt.RevealDecryptedBytes())
            {
                return Generate(b, salt.PlainBytes);
            }
        }

        public int Generate(byte b, byte[] salt)
        {
            byte[] byteBuffer = null;
            try
            {
                byteBuffer = new byte[salt.Length];
                //Append salt + byte
                Array.Copy(salt, byteBuffer, salt.Length);
                byteBuffer[salt.Length - 1] = b;
                var result = _fastHasher.ComputeFast(byteBuffer);
                return result;
            }
            finally
            {
                if (byteBuffer != null)
                    Array.Clear(byteBuffer, 0, byteBuffer.Length);
            }
        }

        private void InitializeSalt()
        {
            while (true)
            {
                var salt = _safeRandom.GetBytes(SaltLength);
                if (!IsSaltValid(salt))
                    continue;
                _sessionSalt.Initialize(salt);
                break;
            }
        }

        /// <summary>
        ///     Determines if all byte id's will have different result with the given salt.
        /// </summary>
        private bool IsSaltValid(byte[] salt)
        {
            const int totalBytes = 256;
            var byteHashes = GetHashesForAllBytes(salt);
            var totalUniqueHashes = byteHashes.Distinct().Count();
            return totalUniqueHashes == totalBytes;
        }

        private IEnumerable<int> GetHashesForAllBytes(byte[] salt)
        {
            const int totalBytes = 256;
            var byteHashes = new int[totalBytes];
            //Fast.For(0, totalBytes, (i) => byteHashes[i] = this.GenerateInternal((byte)i);
            for (var i = 0; i < 256; i++)
                byteHashes[i] = Generate((byte) i, salt);
            return byteHashes;
        }
    }
}