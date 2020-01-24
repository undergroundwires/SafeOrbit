using System;
using System.Linq;
using System.Threading.Tasks;
using SafeOrbit.Cryptography.Encryption;
using SafeOrbit.Cryptography.Random;
using SafeOrbit.Exceptions;
using SafeOrbit.Library;
using SafeOrbit.Memory.SafeBytesServices.DataProtection;
using SafeOrbit.Memory.SafeBytesServices.Factory;
using SafeOrbit.Memory.SafeBytesServices.Id;
using SafeOrbit.Parallel;

namespace SafeOrbit.Memory.SafeBytesServices
{
    /// <summary>
    ///     <see cref="SafeByte" /> represents a single cryptographically secured byte in the memory.
    ///     It's the lowest level memory protection algorithm.
    /// </summary>
    /// <seealso cref="ISafeByte" />
    /// <seealso cref="ISafeBytes" />
    /// <seealso cref="ISafeByteFactory" />
    /// <seealso cref="MemoryCachedSafeByteFactory" />
    internal sealed class SafeByte : ISafeByte
    {
        private readonly IByteIdGenerator _byteIdGenerator;
        private readonly IMemoryProtectedBytes _encryptedByte;
        private readonly IMemoryProtectedBytes _encryptionKey;
        private readonly IFastEncryptor _encryptor;
        private readonly IFastRandom _fastRandom;

        private int _encryptedByteLength;
        private int _id;
        private volatile bool _isDisposed;
        private int _realBytePosition;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SafeByte" /> class.
        /// </summary>
        /// <exception cref="MemoryInjectionException">If the object has been modified outside of the application scope.</exception>
        public SafeByte() : this(SafeOrbitCore.Current.Factory.Get<IFastEncryptor>(),
            SafeOrbitCore.Current.Factory.Get<IFastRandom>(),
            SafeOrbitCore.Current.Factory.Get<IByteIdGenerator>(),
            SafeOrbitCore.Current.Factory.Get<IMemoryProtectedBytes>(),
            SafeOrbitCore.Current.Factory.Get<IMemoryProtectedBytes>()
        )
        {
        }

        internal SafeByte(
            IFastEncryptor encryptor,
            IFastRandom fastRandom,
            IByteIdGenerator byteIdGenerator,
            IMemoryProtectedBytes encryptedByte,
            IMemoryProtectedBytes encryptionKey)
        {
            _encryptor = encryptor ?? throw new ArgumentNullException(nameof(encryptor));
            _fastRandom = fastRandom ?? throw new ArgumentNullException(nameof(fastRandom));
            _byteIdGenerator = byteIdGenerator ?? throw new ArgumentNullException(nameof(fastRandom));
            _encryptedByte = encryptedByte ?? throw new ArgumentNullException(nameof(encryptedByte));
            _encryptionKey = encryptionKey ?? throw new ArgumentNullException(nameof(encryptionKey));
        }

        /// <summary>
        ///     Private constructor for creating identical instance of the <see cref="SafeByte" />.
        /// </summary>
        private SafeByte(
            int id, int realBytePosition,
            int encryptedByteLength,
            IFastEncryptor encryptor,
            IFastRandom fastRandom,
            IByteIdGenerator byteIdGenerator,
            IMemoryProtectedBytes encryptedByte,
            IMemoryProtectedBytes encryptionKey)
        {
            _encryptor = encryptor;
            _fastRandom = fastRandom;
            _byteIdGenerator = byteIdGenerator;
            //Deep copy
            _id = id;
            _realBytePosition = realBytePosition;
            _encryptedByte = encryptedByte.DeepClone();
            _encryptionKey = encryptionKey.DeepClone();
            _encryptedByteLength = encryptedByteLength;
            IsByteSet = true;
        }

        /// <exception cref="ObjectDisposedException" accessor="get">If object is disposed</exception>
        /// <exception cref="InvalidOperationException" accessor="get">Thrown when byte is already set</exception>
        public int Id
        {
            get
            {
                EnsureByteIsSet();
                EnsureNotDisposed();
                return _id;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether any byte is set on this instance.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the byte is set; otherwise, <c>false</c>.
        /// </value>
        public bool IsByteSet { get; private set; }

        /// <exception cref="ObjectDisposedException">If object is disposed</exception>
        /// <exception cref="InvalidOperationException">Thrown when byte is already set</exception>
        public async Task SetAsync(byte b)
        {
            EnsureNotDisposed();
            EnsureByteIsNotSet();
            // Generate ID
            _id = await _byteIdGenerator.GenerateAsync(b)
                .ConfigureAwait(false);
            // Encrypt
            await InitializeAsync(b).ConfigureAwait(false);
            IsByteSet = true;
        }

        /// <inheritdoc />
        /// <exception cref="InvalidOperationException">Thrown when byte is not set</exception>
        /// <exception cref="ObjectDisposedException">If object is disposed</exception>
        public async Task<byte> GetAsync()
        {
            EnsureNotDisposed();
            EnsureByteIsSet();
            byte[] byteBuffer = null;
            try
            {
                var encryptedBuffer = new byte[_encryptedByteLength];
                try
                {
                    using (var @byte = await _encryptedByte.RevealDecryptedBytesAsync().ConfigureAwait(false))
                    {
                        Buffer.BlockCopy(@byte.PlainBytes, 0, encryptedBuffer, 0, _encryptedByteLength);
                    }

                    using (var encryptionKey = await _encryptionKey.RevealDecryptedBytesAsync().ConfigureAwait(false))
                    {
                        byteBuffer = await _encryptor.DecryptAsync(encryptedBuffer, encryptionKey.PlainBytes)
                            .ConfigureAwait(false);
                    }

                    //Extract the byte from arbitrary bytes
                    return byteBuffer[_realBytePosition];
                }
                finally
                {
                    Array.Clear(encryptedBuffer, 0, _encryptedByteLength);
                }
            }
            finally
            {
                if (byteBuffer != null)
                    Array.Clear(byteBuffer, 0, byteBuffer.Length);
            }
        }

        /// <summary>
        ///     Deeply clone the object.
        /// </summary>
        /// <returns>
        ///     Cloned object.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown when byte is not set</exception>
        /// <exception cref="ObjectDisposedException">If object is disposed</exception>
        public ISafeByte DeepClone()
        {
            EnsureByteIsSet();
            EnsureNotDisposed();
            var clone = new SafeByte(
                id: _id,
                realBytePosition: _realBytePosition,
                encryptedByteLength: _encryptedByteLength,
                encryptor: _encryptor,
                fastRandom: _fastRandom,
                byteIdGenerator: _byteIdGenerator,
                encryptedByte: _encryptedByte,
                encryptionKey: _encryptionKey);
            return clone;
        }

        /// <exception cref="InvalidOperationException">Byte is not set in one of the instances</exception>
        public bool Equals(ISafeByte other)
        {
            EnsureNotDisposed();
            if (other == null)
                return false;
            if (!other.IsByteSet || !IsByteSet)
                throw new InvalidOperationException("Byte is not set in one of the instances");
            if (IsByteSet && other.IsByteSet)
                return AreIdsSame(Id, other.GetHashCode());
            return false;
        }

        /// <inheritdoc cref="EnsureByteIsSet"/>
        /// <inheritdoc cref="EnsureNotDisposed"/>
        public async Task<bool> EqualsAsync(byte other)
        {
            EnsureNotDisposed();
            EnsureByteIsSet();
            var otherId = await _byteIdGenerator.GenerateAsync(other)
                .ConfigureAwait(false);
            return AreIdsSame(Id, otherId);
        }

        public void Dispose()
        {
            EnsureNotDisposed();
            _encryptionKey.Dispose();
            _encryptedByte.Dispose();
            _id = 0;
            _encryptedByteLength = 0;
            _realBytePosition = 0;
            _isDisposed = true;
        }

        public override bool Equals(object obj)
        {
            return obj switch
            {
                ISafeByte sb => Equals(sb),
                null => false,
                byte _ => throw new NotSupportedException($"Use {nameof(EqualsAsync)} instead"),
                _ => throw new ArgumentException("Unknown object type")
            };
        }
        

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     Unique hash code based on the byte it is holding, suitable for use in hashing algorithms and data structures like a
        ///     hash table.
        /// </returns>
        public override int GetHashCode()
        {
            EnsureByteIsSet();
            EnsureNotDisposed();
            return _id;
        }

        public override string ToString()
        {
#if DEBUG
            return $"(DEBUG) InnerByte = {TaskContext.RunSync(GetAsync)}";
#else
            return "";
#endif
        }

        private async Task InitializeAsync(byte b)
        {
            //Mix the with arbitrary bytes
            var saltSize = _encryptedByte.BlockSizeInBytes;
            _realBytePosition = _fastRandom.GetInt(0, saltSize);
            var arbitraryBytes = _fastRandom.GetBytes(saltSize);
            try
            {
                arbitraryBytes[_realBytePosition] = b;
                // Get key
                var keySize = _encryptionKey.BlockSizeInBytes;
                var encryptionKey = _fastRandom.GetBytes(keySize);
                // Encrypt
                var encryptedBuffer = default(byte[]);
                try
                {
                    encryptedBuffer = await _encryptor.EncryptAsync(arbitraryBytes, encryptionKey)
                        .ConfigureAwait(false);
                    //Add arbitrary bytes
                    _encryptedByteLength = encryptedBuffer.Length;
                    await _encryptedByte.InitializeAsync(GetMemoryProtectableSizedBytes(encryptedBuffer))
                        .ConfigureAwait(false);
                }
                finally
                {
                    if (encryptedBuffer != null)
                        Array.Clear(encryptedBuffer, 0, encryptedBuffer.Length);
                }
                await _encryptionKey.InitializeAsync(encryptionKey)
                    .ConfigureAwait(false);
            }
            finally
            {
                if(arbitraryBytes != null)
                    Array.Clear(arbitraryBytes, 0, arbitraryBytes.Length);
            }
        }

        /// <summary>
        ///     User data must be multiple of 16 in order to be used in ProtectedMemory.Protect
        /// </summary>
        private byte[] GetMemoryProtectableSizedBytes(byte[] byteArray)
        {
            var multipleOfRule = _encryptedByte.BlockSizeInBytes;
            var length = byteArray.Length;
            var fixedLength = length - length % multipleOfRule + multipleOfRule;
            var result = new byte[fixedLength];
            Buffer.BlockCopy(byteArray, 0, result, 0, length);
            for (var i = byteArray.Length; i < fixedLength; i++)
                result[i] = _fastRandom.GetBytes(1).First();
            return result;
        }

        /// <exception cref="System.InvalidOperationException">Thrown when byte is not set</exception>
        private void EnsureByteIsSet()
        {
            if (!IsByteSet) throw new InvalidOperationException($"Byte must be set using {nameof(SetAsync)} method.");
        }

        /// <exception cref="InvalidOperationException">Thrown when byte is already set</exception>
        private void EnsureByteIsNotSet()
        {
            if (IsByteSet) throw new InvalidOperationException("Byte is already set");
        }

        private static bool AreIdsSame(int id, int other)
        {
            uint result = 0;
            result |= (uint) id ^
                      (uint) other; // Protects against timing attacks, see: https://security.stackexchange.com/questions/83660/simple-string-comparisons-not-secure-against-timing-attacks 
            return result == 0;
        }

        /// <exception cref="ObjectDisposedException">Object id isposed</exception>
        private void EnsureNotDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(GetType().Name);
        }
    }
}