using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SafeOrbit.Cryptography.Encryption;
using SafeOrbit.Cryptography.Random;
using SafeOrbit.Extensions;
using SafeOrbit.Helpers;
using SafeOrbit.Library;
using SafeOrbit.Memory.SafeBytesServices.DataProtection;
using SafeOrbit.Memory.SafeBytesServices.Factory;

namespace SafeOrbit.Memory.SafeBytesServices.Collection
{
    /// <summary>
    ///     An encrypted collection of <see cref="ISafeByte" /> instances.
    /// </summary>
    /// <seealso cref="ISafeByte" />
    internal class EncryptedSafeByteCollection : ISafeByteCollection
    {
        /// <summary>
        ///     The encryption key to encrypt/decrypt the inner list.
        /// </summary>
        private readonly IMemoryProtectedBytes _encryptionKey;
        private readonly byte[] _salt;

        private readonly IFastEncryptor _encryptor;
        private readonly ISafeByteFactory _safeByteFactory;
        private readonly IByteIdListSerializer<int> _serializer;

        /// <summary>
        ///     Encrypted inner collection.
        /// </summary>
        private byte[] _encryptedCollection;

        private bool _isDisposed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="EncryptedSafeByteCollection" /> class.
        /// </summary>
        public EncryptedSafeByteCollection() : this(
            SafeOrbitCore.Current.Factory.Get<IFastEncryptor>(),
            SafeOrbitCore.Current.Factory.Get<IMemoryProtectedBytes>(),
            SafeOrbitCore.Current.Factory.Get<IFastRandom>(),
            SafeOrbitCore.Current.Factory.Get<ISafeByteFactory>(),
            SafeOrbitCore.Current.Factory.Get<IByteIdListSerializer<int>>())
        {
        }

        internal EncryptedSafeByteCollection(
            IFastEncryptor encryptor,
            IMemoryProtectedBytes encryptionKey,
            ICryptoRandom fastRandom,
            ISafeByteFactory safeByteFactory,
            IByteIdListSerializer<int> serializer)
        {
            _encryptionKey = encryptionKey ?? throw new ArgumentNullException(nameof(encryptionKey));
            _encryptionKey.Initialize(fastRandom.GetBytes(encryptionKey.BlockSizeInBytes));
            _encryptor = encryptor ?? throw new ArgumentNullException(nameof(encryptor));
            _safeByteFactory = safeByteFactory ?? throw new ArgumentNullException(nameof(safeByteFactory));
            _serializer = serializer;
            _salt = fastRandom.GetBytes(encryptor.BlockSizeInBits / 8);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="safeByte" /> is <see langword="null" />.</exception>
        /// <exception cref="ObjectDisposedException">Throws if the <see cref="EncryptedSafeByteCollection" /> instance is disposed</exception>
        public void Append(ISafeByte safeByte)
        {
            if (safeByte == null) throw new ArgumentNullException(nameof(safeByte));
            AppendMany(new[] {safeByte});
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="safeBytes" /> is <see langword="null" />.</exception>
        /// <exception cref="ObjectDisposedException">Throws if the <see cref="EncryptedSafeByteCollection" /> instance is disposed</exception>
        public void AppendMany(IEnumerable<ISafeByte> safeBytes)
        {
            EnsureNotDisposed();
            if (safeBytes == null) throw new ArgumentNullException(nameof(safeBytes));
            var bytes = safeBytes as ISafeByte[] ?? safeBytes.ToArray();
            if (!bytes.Any())  return;
            if(bytes.Any(b=> b == null))  throw new ArgumentNullException("List has null object", nameof(safeBytes));

            using (var key = _encryptionKey.RevealDecryptedBytes())
            {
                var list = TaskContext.RunSync(() => DecryptAndDeserializeAsync(_encryptedCollection, key.PlainBytes));
                foreach (var @byte in bytes)
                {
                    list.Add(@byte.Id);
                    Length++;
                }
                _encryptedCollection =
                    TaskContext.RunSync(() => SerializeAndEncryptAsync(list.ToArray(), key.PlainBytes));
                list.Clear();
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="index" /> is lower than zero or higher/equals to the <see cref="Length" />. </exception>
        /// <exception cref="InvalidOperationException"> <see cref="EncryptedSafeByteCollection" /> instance is empty. </exception>
        /// <exception cref="ObjectDisposedException"> <see cref="EncryptedSafeByteCollection" /> instance is disposed </exception>
        /// <seealso cref="ISafeByte" />
        public async Task<ISafeByte> GetAsync(int index)
        {
            if (index < 0 && index >= Length)
                throw new ArgumentOutOfRangeException(nameof(index), index,
                    $"Must be non-negative than zero and lower than length {Length}");
            EnsureNotDisposed();
            EnsureNotEmpty();
            ICollection<int> list;
            using (var key = _encryptionKey.RevealDecryptedBytes())
            {
                list = await DecryptAndDeserializeAsync(_encryptedCollection, key.PlainBytes)
                    .ConfigureAwait(false);
            }

            if (index >= list.Count)
                throw new ArgumentOutOfRangeException(nameof(index), index,
                    "Inner encrypted collection is corrupt." +
                    $"Must be lower than length {Length} but was {index}.");
            var id = list.ElementAt(index);
            var safeByte = _safeByteFactory.GetById(id);
            list.Clear();
            return safeByte;
        }

        /// <inheritdoc />
        /// <exception cref="InvalidOperationException"><see cref="EncryptedSafeByteCollection" /> instance is empty.</exception>
        /// <exception cref="ObjectDisposedException"><see cref="EncryptedSafeByteCollection" /> instance is disposed</exception>
        public byte[] ToDecryptedBytes()
        {
            EnsureNotEmpty();
            EnsureNotDisposed();
            ICollection<int> safeBytesIds;
            using (var key = _encryptionKey.RevealDecryptedBytes())
            {
                safeBytesIds =
                    TaskContext.RunSync(() => DecryptAndDeserializeAsync(_encryptedCollection, key.PlainBytes));
            }

            if (safeBytesIds.IsNullOrEmpty()) return new byte[0];
            var safeBytes = safeBytesIds.Select(id => _safeByteFactory.GetById(id));
            var decryptedBytes = GetAllAndMerge(safeBytes);
            return decryptedBytes;
        }

        /// <inheritdoc />
        public int Length { get; private set; }

        /// <inheritdoc />
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_encryptedCollection != null) Array.Clear(_encryptedCollection, 0, _encryptedCollection.Length);
            _encryptionKey.Dispose();
            _isDisposed = true;
            Length = 0;
        }

        private byte[] GetAllAndMerge(IEnumerable<ISafeByte> safeBytes)
        {
            var buffer = new byte[Length];
            Fast.For(0, Length, i =>
            {
                var @byte = safeBytes.ElementAt(i).Get();
                buffer[i] = @byte;
            });
            return buffer;
        }

        private async Task<ICollection<int>> DecryptAndDeserializeAsync(byte[] encryptedCollection,
            byte[] encryptionKey)
        {
            if (encryptedCollection == null)
                return new List<int>();
            var decryptedBytes = await DecryptAsync(encryptedCollection, _salt.Length, encryptionKey)
                .ConfigureAwait(false);
            try
            {
                var deserializedBytes = await _serializer.DeserializeAsync(decryptedBytes)
                    .ConfigureAwait(false);
                ;
                return deserializedBytes.ToList();
            }
            finally
            {
                Array.Clear(decryptedBytes, 0, decryptedBytes.Length);
            }
        }

        private async Task<byte[]> SerializeAndEncryptAsync(IReadOnlyCollection<int> safeByteIdList,
            byte[] encryptionKey)
        {
            var serialized = await _serializer.SerializeAsync(safeByteIdList).ConfigureAwait(false);
            try
            {
                var encrypted = await EncryptAsync(serialized, _salt, encryptionKey)
                    .ConfigureAwait(false);
                return encrypted;
            }
            finally
            {
                if(serialized != null)
                    Array.Clear(serialized, 0, serialized.Length);
            }
        }
        private async Task<byte[]> DecryptAsync(byte[] encryptedBytes, int saltLength, byte[] encryptionKey)
        {
            var saltedBytes = await _encryptor.DecryptAsync(encryptedBytes, encryptionKey)
                .ConfigureAwait(false);
            try
            {
                return saltedBytes
                    .Take(encryptedBytes.Length - saltLength)
                    .ToArray();
            }
            finally
            {
                if (saltedBytes != null)
                    Array.Clear(saltedBytes, 0, saltedBytes.Length);
            }
        }
        private async Task<byte[]> EncryptAsync(byte[] plainBytes, byte[] saltBytes, byte[] encryptionKey)
        {
            var salted = plainBytes.Combine(saltBytes);
            try
            {
                return await _encryptor.EncryptAsync(salted, encryptionKey)
                    .ConfigureAwait(false);
            }
            finally
            {
                if(salted != null)
                    Array.Clear(salted, 0, salted.Length);
            }
        }

        /// <exception cref="ObjectDisposedException">Throws if the <see cref="EncryptedSafeByteCollection" /> instance is disposed</exception>
        private void EnsureNotDisposed()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(EncryptedSafeByteCollection));
        }

        /// <exception cref="InvalidOperationException">Throws if the <see cref="EncryptedSafeByteCollection" /> instance is empty.</exception>
        private void EnsureNotEmpty()
        {
            if (Length == 0) throw new InvalidOperationException($"{nameof(SafeBytes)} is empty");
        }
    }
}