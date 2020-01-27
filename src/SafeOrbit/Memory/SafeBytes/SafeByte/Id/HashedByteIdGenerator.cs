using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SafeOrbit.Cryptography.Hashers;
using SafeOrbit.Cryptography.Random;
using SafeOrbit.Exceptions;
using SafeOrbit.Extensions;
using SafeOrbit.Library;
using SafeOrbit.Memory.SafeBytesServices.DataProtection;
using SafeOrbit.Threading;

namespace SafeOrbit.Memory.SafeBytesServices.Id
{
    /// <summary>
    ///     Creates a unique <see cref="int" /> values for each <see cref="byte" />.
    ///     It's a singleton so byte ID's are consistent throughout the execution of the process, on restart a new salt &amp;
    ///     new ID's are generated.
    /// </summary>
    /// <seealso cref="IByteIdGenerator" />
    internal class HashedByteIdGenerator : IByteIdGenerator
    {
        private const int SaltLength = 16;
        private readonly IFastHasher _fastHasher;
        private readonly ISafeRandom _safeRandom;
        private readonly AsyncLazy<IMemoryProtectedBytes> _sessionSalt;

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
            if (sessionSalt == null) throw new ArgumentNullException(nameof(sessionSalt));
            _fastHasher = fastHasher ?? throw new ArgumentNullException(nameof(fastHasher));
            _safeRandom = safeRandom ?? throw new ArgumentNullException(nameof(safeRandom));
            _sessionSalt = new AsyncLazy<IMemoryProtectedBytes>(() => InitializeSaltAsync(sessionSalt));
        }

        /// <inheritdoc />
        public async Task<int> GenerateAsync(byte @byte)
        {
            using (var salt = await _sessionSalt.RevealDecryptedBytesAsync().ConfigureAwait(false))
            {
                return Generate(@byte, salt.PlainBytes);
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/></exception>
        public async Task<IEnumerable<int>> GenerateManyAsync(SafeMemoryStream stream) //TODO: IAsyncEnumerable in C# 8.0
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            var list = new Collection<int>();
            using (var salt = await _sessionSalt.RevealDecryptedBytesAsync().ConfigureAwait(false))
            {
                int byteRead;
                while ((byteRead = stream.ReadByte()) != -1)
                {
                    var id = Generate((byte) byteRead, salt.PlainBytes);
                    list.Add(id);
                }
            }
            return list;
        }

        private int Generate(byte b, byte[] salt)
        {
            byte[] byteBuffer = null;
            try
            {
                byteBuffer = salt.CopyToNewArray();
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

        private async Task<IMemoryProtectedBytes> InitializeSaltAsync(IMemoryProtectedBytes sessionSalt)
        {
            const int maxRetries = 1000;
            var retryCount = 0;
            while (retryCount < maxRetries)
            {
                var salt = _safeRandom.GetBytes(SaltLength);
                if (!IsSaltValid(salt))
                {
                    retryCount++;
                    continue;
                }
                await sessionSalt.InitializeAsync(salt)
                    .ConfigureAwait(false);
                return sessionSalt;
            }
            throw new ArgumentException("Could not initialize the session salt. Max retries exceeded.");
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
            for (var i = 0; i < 256; i++)
                byteHashes[i] = Generate((byte) i, salt);
            return byteHashes;
        }
    }
}