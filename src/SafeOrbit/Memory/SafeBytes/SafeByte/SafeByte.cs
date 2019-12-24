using System;
using System.Linq;
using SafeOrbit.Cryptography.Random;
using SafeOrbit.Cryptography.Encryption;
using SafeOrbit.Exceptions;
using SafeOrbit.Library;
using SafeOrbit.Memory.SafeBytesServices.DataProtection;
using SafeOrbit.Memory.SafeBytesServices.Factory;
using SafeOrbit.Memory.SafeBytesServices.Id;
using SafeOrbit.Helpers;

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
        private volatile bool _isDisposed;

        private int _encryptedByteLength;
        private int _id;
        private int _realBytePosition;

        private readonly IByteIdGenerator _byteIdGenerator;
        private readonly IFastEncryptor _encryptor;
        private readonly IFastRandom _fastRandom;
        private readonly IMemoryProtectedBytes _encryptedByte;
        private readonly IMemoryProtectedBytes _encryptionKey;

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
        public void Set(byte b)
        {
            EnsureNotDisposed();
            EnsureByteIsNotSet();
            //Generate ID
            _id = _byteIdGenerator.Generate(b);
            //Encrypt
            Initialize(b);
            IsByteSet = true;
        }

        /// <summary>
        ///     Decrypts and returns the byte that this <see cref="SafeByte" /> instance represents.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Thrown when byte is not set</exception>
        /// <exception cref="ObjectDisposedException">If object is disposed</exception>
        public byte Get()
        {
            EnsureNotDisposed();
            EnsureByteIsSet();
            byte[] byteBuffer = null;
            try
            {
                var encryptedBuffer = new byte[_encryptedByteLength];
                try
                {
                    using(var @byte = _encryptedByte.RevealDecryptedBytes())
                    {
                        Buffer.BlockCopy(@byte.PlainBytes, 0, encryptedBuffer, 0, _encryptedByteLength);
                    }

                    using(var encryptionKey = _encryptionKey.RevealDecryptedBytes())
                    {
                        byteBuffer = _encryptor.Decrypt(encryptedBuffer, encryptionKey.PlainBytes);
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

        public bool Equals(ISafeByte other)
        {
            EnsureNotDisposed();
            if (other == null)
                return false;
            if (!IsByteSet && !other.IsByteSet)
                return true;
            if (IsByteSet && other.IsByteSet)
                return AreIdsSame(this.Id, other.GetHashCode());
            return false;
        }

        public bool Equals(byte other)
        {
            EnsureNotDisposed();
            EnsureByteIsSet();
            var otherId = _byteIdGenerator.Generate(other);
            return AreIdsSame(this.Id, otherId);
        }

        public override bool Equals(object obj)
        {
            return obj switch
            {
                ISafeByte sb => Equals(sb),
                byte @byte => Equals(@byte),
                null => false,
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

        public override string ToString()
        {
#if DEBUG
            return $"InnerByte = {Get()}";
#else
            return "";
#endif
        }
        private void Initialize(byte b)
        {
            //Mix the with arbitrary bytes
            var saltSize = _encryptedByte.BlockSizeInBytes;
            _realBytePosition = _fastRandom.GetInt(0, saltSize);
            var arbitraryBytes = _fastRandom.GetBytes(saltSize);
            RuntimeHelper.ExecuteCodeWithGuaranteedCleanup(
                //Action
                () =>
                {
                    arbitraryBytes[_realBytePosition] = b;
                    //Get key
                    var keySize = _encryptionKey.BlockSizeInBytes;
                    var encryptionKey = _fastRandom.GetBytes(keySize);
                    //Encrypt
                    var encryptedBuffer = default(byte[]);
                    RuntimeHelper.ExecuteCodeWithGuaranteedCleanup(
                        //Action
                        () =>
                        {
                            encryptedBuffer = _encryptor.Encrypt(arbitraryBytes, encryptionKey);
                            //Add arbitrary bytes
                            _encryptedByteLength = encryptedBuffer.Length;
                            _encryptedByte.Initialize(GetMemoryProtectableSizedBytes(encryptedBuffer));
                        },
                        //Cleanup
                        () =>
                        {
                            if (encryptedBuffer != null)
                                Array.Clear(encryptedBuffer, 0, encryptedBuffer.Length);
                        });
                    _encryptionKey.Initialize(encryptionKey);
                },
                //Cleanup
                () =>
                {
                    Array.Clear(arbitraryBytes, 0, arbitraryBytes.Length);
                });
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
            if (!IsByteSet) throw new InvalidOperationException($"Byte must be set using {nameof(Set)} method.");
        }

        /// <exception cref="InvalidOperationException">Thrown when byte is already set</exception>
        private void EnsureByteIsNotSet()
        {
            if (IsByteSet) throw new InvalidOperationException("Byte is already set");
        }

        private static bool AreIdsSame(int id, int other)
        {
            uint result = 0;
            result |= (uint)id ^ (uint)other; // Protects against timing attacks, see: https://security.stackexchange.com/questions/83660/simple-string-comparisons-not-secure-against-timing-attacks 
            return result == 0;
        }

        private void EnsureNotDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(GetType().Name);
        }
    }
}