using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SafeOrbit.Cryptography.Random;
using SafeOrbit.Cryptography.Encryption;
using SafeOrbit.Library;
using SafeOrbit.Memory.SafeBytesServices.DataProtection;
using SafeOrbit.Memory.SafeBytesServices.Factory;
using SafeOrbit.Extensions;
using SafeOrbit.Helpers;

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
        private readonly MemoryProtectedBytes _encryptionKey;
        //TODO: Encryption salt

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
            SafeOrbitCore.Current.Factory.Get<IByteArrayProtector>(),
            SafeOrbitCore.Current.Factory.Get<IFastRandom>(),
            SafeOrbitCore.Current.Factory.Get<ISafeByteFactory>(),
            SafeOrbitCore.Current.Factory.Get<IByteIdListSerializer<int>>())
        {
        }

        internal EncryptedSafeByteCollection(IFastEncryptor encryptor, IByteArrayProtector memoryProtector,
            IFastRandom fastRandom, ISafeByteFactory safeByteFactory, IByteIdListSerializer<int> serializer)
        {
            _encryptionKey = GenerateEncryptionKey(memoryProtector, fastRandom);
            _encryptor = encryptor ?? throw new ArgumentNullException(nameof(encryptor));
            _safeByteFactory = safeByteFactory ?? throw new ArgumentNullException(nameof(safeByteFactory));
            _serializer = serializer;
        }

        /// <summary>
        ///     Appends the specified <see cref="ISafeByte" /> instance to the inner encrypted collection.
        /// </summary>
        /// <param name="safeByte">The safe byte.</param>
        /// <exception cref="ArgumentNullException"><paramref name="safeByte" /> is <see langword="null" />.</exception>
        /// <exception cref="ObjectDisposedException">Throws if the <see cref="EncryptedSafeByteCollection" /> instance is disposed</exception>
        /// <seealso cref="ISafeByte" />
        public void Append(ISafeByte safeByte)
        {
            EnsureNotDisposed();
            if (safeByte == null) throw new ArgumentNullException(nameof(safeByte));

            using (var key = _encryptionKey.UseDecryptedData())
            {
                var list = TaskContext.RunSync(() => DecryptAndDeserializeAsync(_encryptedCollection, key.Data));
                list.Add(safeByte.Id);
                _encryptedCollection = TaskContext.RunSync(() => SerializeAndEncryptAsync(list.ToArray(), key.Data));
                list.Clear();
            }

            Length++;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Gets the byte as <see cref="T:SafeOrbit.Memory.SafeBytesServices.ISafeByte" /> for the specified index asynchronously.
        /// </summary>
        /// <param name="index">The position of the byte.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="index" /> is lower than zero or higher/equals to the
        ///     <see cref="P:SafeOrbit.Memory.SafeBytesServices.Collection.EncryptedSafeByteCollection.Length" />.
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException"><see cref="T:SafeOrbit.Memory.SafeBytesServices.Collection.EncryptedSafeByteCollection" /> instance is empty.</exception>
        /// <exception cref="T:System.ObjectDisposedException"><see cref="T:SafeOrbit.Memory.SafeBytesServices.Collection.EncryptedSafeByteCollection" /> instance is disposed</exception>
        /// <seealso cref="M:SafeOrbit.Memory.SafeBytesServices.Collection.EncryptedSafeByteCollection.Get(System.Int32)" />
        /// <seealso cref="T:SafeOrbit.Memory.SafeBytesServices.ISafeByte" />
        public async Task<ISafeByte> GetAsync(int index)
        {
            if ((index < 0) && (index >= Length))
                throw new ArgumentOutOfRangeException(nameof(index), index, $"Must be non-negative than zero and lower than length {Length}");
            EnsureNotDisposed();
            EnsureNotEmpty();
            ICollection<int> list;
            using (var key = _encryptionKey.UseDecryptedData())
            {
                list = await DecryptAndDeserializeAsync(_encryptedCollection, key.Data)
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

        /// <summary>
        ///     Returns all of the real byte values that <see cref="ISafeByteCollection" /> holds.
        ///     Reveals all protected data in memory.
        /// </summary>
        /// <returns>System.Byte[].</returns>
        /// <exception cref="InvalidOperationException"><see cref="EncryptedSafeByteCollection" /> instance is empty.</exception>
        /// <exception cref="ObjectDisposedException"><see cref="EncryptedSafeByteCollection" /> instance is disposed</exception>
        public byte[] ToDecryptedBytes()
        {
            EnsureNotEmpty();
            EnsureNotDisposed();
            ICollection<int> safeBytesIds;
            using (var key = _encryptionKey.UseDecryptedData())
            {
                safeBytesIds = TaskContext.RunSync(() => DecryptAndDeserializeAsync(_encryptedCollection, key.Data));
            }
            if (safeBytesIds.IsNullOrEmpty()) return new byte[0];
            var safeBytes = safeBytesIds.Select(id => _safeByteFactory.GetById(id));
            var decryptedBytes = GetAllAndMerge(safeBytes);
            return decryptedBytes;
        }

        /// <summary>
        ///     Gets the length.
        /// </summary>
        /// <value>The length.</value>
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
        private async Task<ICollection<int>> DecryptAndDeserializeAsync(byte[] encryptedCollection, byte[] encryptionKey)
        {
            if (encryptedCollection == null)
                return new List<int>();
            var decryptedBytes = await _encryptor.DecryptAsync(encryptedCollection, encryptionKey)
                .ConfigureAwait(false); ;
            try
            {
                var deserializedBytes = await _serializer.DeserializeAsync(decryptedBytes)
                    .ConfigureAwait(false); ;
                return deserializedBytes.ToList();
            }
            finally
            {
                Array.Clear(decryptedBytes, 0, decryptedBytes.Length);
            }
        }
        private async Task<byte[]> SerializeAndEncryptAsync(IReadOnlyCollection<int> safeByteIdList, byte[] encryptionKey)
        {
            var serializedBytes = await _serializer.SerializeAsync(safeByteIdList)
                .ConfigureAwait(false);
            try
            {
                var encrypted = await _encryptor.EncryptAsync(serializedBytes, encryptionKey)
                    .ConfigureAwait(false);
                return encrypted;
            }
            finally
            {
                Array.Clear(serializedBytes, 0, serializedBytes.Length);
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
        private static MemoryProtectedBytes GenerateEncryptionKey(IByteArrayProtector memoryProtector, ICryptoRandom random)
        {
            if (memoryProtector == null) throw new ArgumentNullException(nameof(memoryProtector));
            if (random == null) throw new ArgumentNullException(nameof(random));
            var encryptionKeyBytes = random.GetBytes(memoryProtector.BlockSizeInBytes);
            return new MemoryProtectedBytes(memoryProtector, encryptionKeyBytes);
        }
        private class MemoryProtectedBytes : IDisposable
        {
            private readonly IByteArrayProtector _protector;
            private readonly byte[] _encryptedBytes;
            public MemoryProtectedBytes(IByteArrayProtector protector, byte[] unencryptedBytes)
            {
                _protector = protector;
                _encryptedBytes = unencryptedBytes ?? throw new ArgumentNullException(nameof(unencryptedBytes));
                _protector.Protect(unencryptedBytes);
            }
            public UnprotectedBytes UseDecryptedData() => new UnprotectedBytes(_protector, _encryptedBytes);
            public void Dispose() => Array.Clear(_encryptedBytes, 0, _encryptedBytes.Length);
            public class UnprotectedBytes : IDisposable
            {
                private readonly IByteArrayProtector _protector;
                public byte[] Data { get; }
                public UnprotectedBytes(IByteArrayProtector protector, byte[] encryptedBytes)
                {
                    _protector = protector;
                    protector.Protect(encryptedBytes);
                    Data = encryptedBytes;
                }
                public void Dispose() => _protector.Protect(Data);
            }
        }
    }
}