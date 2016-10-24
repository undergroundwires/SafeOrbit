
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SafeOrbit.Encryption;
using SafeOrbit.Library;
using SafeOrbit.Memory.SafeObject;
using SafeOrbit.Utilities;
using SafeOrbit.Memory;
using SafeOrbit.Memory.SafeBytesServices.DataProtection;
using SafeOrbit.Memory.SafeBytesServices.Factory;
using SafeOrbit.Random;

namespace SafeOrbit.Memory.SafeBytesServices.Collection
{
    /// <summary>
    ///     An encrypted collection of <see cref="ISafeByte"/> instances.
    /// </summary>
    /// <seealso cref="ISafeByte"/>
    internal class EncryptedSafeByteCollection : ISafeByteCollection
    {
        /// <summary>
        /// The encryption key to encrypt/decrypt the inner list.
        /// </summary>
        private readonly byte[] _encryptionKey;
        /// <summary>
        /// Encrypted inner collection.
        /// </summary>
        private byte[] _encryptedCollection;
        private readonly IFastEncryptor _encryptor;
        private readonly IByteArrayProtector _memoryProtector;
        private readonly ISafeByteFactory _safeByteFactory;
        private bool _isDisposed;
        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptedSafeByteCollection"/> class.
        /// </summary>
        public EncryptedSafeByteCollection() : this(
            LibraryManagement.Factory.Get<IFastEncryptor>(),
            LibraryManagement.Factory.Get<IByteArrayProtector>(),
            LibraryManagement.Factory.Get<IFastRandom>(),
            LibraryManagement.Factory.Get<ISafeByteFactory>()
            )
        {
        }
        internal EncryptedSafeByteCollection(IFastEncryptor encryptor, IByteArrayProtector memoryProtector,
            IFastRandom fastRandom, ISafeByteFactory safeByteFactory)
        {
            if (encryptor == null) throw new ArgumentNullException(nameof(encryptor));
            if (memoryProtector == null) throw new ArgumentNullException(nameof(memoryProtector));
            if (safeByteFactory == null) throw new ArgumentNullException(nameof(safeByteFactory));
            _encryptor = encryptor;
            _memoryProtector = memoryProtector;
            _safeByteFactory = safeByteFactory;
            _encryptionKey = fastRandom.GetBytes(_memoryProtector.BlockSize);
            memoryProtector.Protect(_encryptionKey);
        }
        /// <summary>
        /// Appends the specified <see cref="ISafeByte"/> instance to the inner encrypted collection.
        /// </summary>
        /// <param name="safeByte">The safe byte.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="safeByte"/> is <see langword="null" />.</exception>
        /// <exception cref="ObjectDisposedException">Throws if the <see cref="EncryptedSafeByteCollection" /> instance is disposed</exception>
        /// <seealso cref="ISafeByte"/>
        public void Append(ISafeByte safeByte)
        {
            EnsureNotDisposed();
            if (safeByte == null) throw new ArgumentNullException(nameof(safeByte));
            _memoryProtector.Unprotect(_encryptionKey);
            var list = DecryptAndDeserialize(_encryptedCollection, _encryptionKey);
            list.Add(safeByte.Id);
            _encryptedCollection = SerializeAndEncrypt(list, _encryptionKey);
            list.Clear();
            _memoryProtector.Protect(_encryptionKey);
            Length++;
        }

        /// <summary>
        /// Gets the byte as <see cref="ISafeByte"/> for the specified index.
        /// </summary>
        /// <param name="index">The position of the byte.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is lower than zero or higher/equals to the <see cref="Length"/>.</exception>
        /// <exception cref="InvalidOperationException"><see cref="EncryptedSafeByteCollection" /> instance is empty.</exception>
        /// <exception cref="ObjectDisposedException"><see cref="EncryptedSafeByteCollection" /> instance is disposed</exception>
        /// <seealso cref="ISafeByte"/>
        public ISafeByte Get(int index)
        {
            if (index < 0 && index >= this.Length) throw new ArgumentOutOfRangeException(nameof(index));
            EnsureNotDisposed();
            EnsureNotEmpty();
            _memoryProtector.Unprotect(_encryptionKey);
            var list = DecryptAndDeserialize(_encryptedCollection, _encryptionKey);
            _memoryProtector.Protect(_encryptionKey);
            var id = list.ElementAt(index);
            var safeByte = _safeByteFactory.GetById(id);
            list.Clear();
            return safeByte;
        }

        /// <summary>
        /// Returns all of the real byte values that <see cref="ISafeByteCollection" /> holds.
        /// Reveals all protected data in memory.
        /// </summary>
        /// <returns>System.Byte[].</returns>
        /// <exception cref="InvalidOperationException"><see cref="EncryptedSafeByteCollection" /> instance is empty.</exception>
        /// <exception cref="ObjectDisposedException"><see cref="EncryptedSafeByteCollection" /> instance is disposed</exception>
        public byte[] ToDecryptedBytes()
        {
            EnsureNotEmpty();
            EnsureNotDisposed();
            _memoryProtector.Unprotect(_encryptionKey);
            var safeBytesList = DecryptAndDeserialize(_encryptedCollection, _encryptionKey).Select(id => _safeByteFactory.GetById(id));
            var decryptedBytes = GetAllAndMerge(safeBytesList);
            return decryptedBytes;
        }
        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>The length.</value>
        public int Length { get; private set; }
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_encryptedCollection != null) Array.Clear(_encryptedCollection, 0, _encryptedCollection.Length);
            if (_encryptionKey != null) Array.Clear(_encryptionKey, 0, _encryptionKey.Length);
            _isDisposed = true;
            this.Length = 0;
        }
        private byte[] GetAllAndMerge(IEnumerable<ISafeByte> safeBytes)
        {
            var buffer = new byte[this.Length];
            Fast.For(0, this.Length, i =>
            {
                var @byte = safeBytes.ElementAt(i).Get();
                buffer[i] = @byte;
            });
            return buffer;

        }
        private ICollection<int> DecryptAndDeserialize(byte[] encryptedCollection, byte[] encryptionKey)
        {
            if (encryptedCollection == null)
                return new List<int>();
            var decryptedBytes = _encryptor.Decrypt(encryptedCollection, encryptionKey);
            try
            {
                var deserializedBytes = Deserialize(decryptedBytes);
                return deserializedBytes.ToList();
            }
            finally
            {
                Array.Clear(decryptedBytes, 0, decryptedBytes.Length);
            }
        }
        private byte[] SerializeAndEncrypt(IEnumerable<int> safeByteIdList, byte[] encryptionKey)
        {
            var serialization = Serialize(safeByteIdList);
            var encrypted = _encryptor.Encrypt(serialization, encryptionKey);
            return encrypted;
        }
        private static byte[] Serialize(IEnumerable<int> list)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(memoryStream))
                {
                    writer.Write(list.Count());
                    foreach (var i in list)
                    {
                        writer.Write(i);
                    }
                }
                return memoryStream.ToArray();
            }
        }
        private static IEnumerable<int> Deserialize(byte[] list)
        {
            using (var memoryStream = new MemoryStream(list))
            {
                using (var reader = new BinaryReader(memoryStream))
                {
                    var count = reader.ReadInt32();
                    var result = new int[count];
                    for (int i = 0; i < count; i++)
                    {
                        result[i] = reader.ReadInt32();
                    }
                    reader.Close();
                    return result;
                }
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