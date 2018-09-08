using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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
        /// <summary>
        ///     The encryption key size.
        /// </summary>
        private const int KeySize = 16;

        /// <summary>
        ///     The size of the salt for the encryption.
        /// </summary>
        private const int SaltSize = 16;

        private readonly IByteIdGenerator _byteIdGenerator;
        private readonly IFastEncryptor _encryptor;
        private readonly IFastRandom _fastRandom;
        private readonly IByteArrayProtector _memoryProtector;
        private byte[] _encryptedByte; //Its length in order to be used in memory protection
        private int _encryptedByteLength;
        private byte[] _encryptionKey;
        private int _id;
        private int _realBytePosition;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SafeByte" /> class.
        /// </summary>
        /// <exception cref="MemoryInjectionException">If the object has been modified outside of the application scope.</exception>
        public SafeByte() : this(SafeOrbitCore.Current.Factory.Get<IFastEncryptor>(),
            SafeOrbitCore.Current.Factory.Get<IFastRandom>(),
            SafeOrbitCore.Current.Factory.Get<IByteIdGenerator>(),
            SafeOrbitCore.Current.Factory.Get<IByteArrayProtector>()
        )
        {
        }

        internal SafeByte(
            IFastEncryptor encryptor,
            IFastRandom fastRandom,
            IByteIdGenerator byteIdGenerator,
            IByteArrayProtector memoryProtector)
        {
            _encryptor = encryptor ?? throw new ArgumentNullException(nameof(encryptor));
            _fastRandom = fastRandom ?? throw new ArgumentNullException(nameof(fastRandom));
            _byteIdGenerator = byteIdGenerator ?? throw new ArgumentNullException(nameof(fastRandom));
            _memoryProtector = memoryProtector ?? throw new ArgumentNullException(nameof(memoryProtector));
        }

        /// <summary>
        ///     Private constructor for creating identical instance of the <see cref="SafeByte" />.
        /// </summary>
        private SafeByte(
            int id, int realBytePosition,
            int encryptedByteLength, byte[] encryptionKey, byte[] encryptedByte,
            IFastEncryptor encryptor,
            IFastRandom fastRandom,
            IByteIdGenerator byteIdGenerator,
            IByteArrayProtector memoryProtector)
        {
            _encryptor = encryptor;
            _fastRandom = fastRandom;
            _byteIdGenerator = byteIdGenerator;
            _memoryProtector = memoryProtector;
            //Deep copy
            _id = id;
            _realBytePosition = realBytePosition;
            _encryptedByte = new byte[encryptedByte.Length];
            _encryptionKey = new byte[encryptionKey.Length];
            Buffer.BlockCopy(encryptedByte, 0, _encryptedByte, 0, encryptedByte.Length);
            Buffer.BlockCopy(encryptionKey, 0, _encryptionKey, 0, encryptionKey.Length);
            _memoryProtector.Protect(_encryptionKey);
            _memoryProtector.Protect(_encryptedByte);
            _encryptedByteLength = encryptedByteLength;
            IsByteSet = true;
        }

        /// <exception cref="InvalidOperationException" accessor="get">Thrown when byte is already set</exception>
        public int Id
        {
            get
            {
                EnsureByteIsSet();
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

        /// <exception cref="System.InvalidOperationException">Thrown when byte is already set</exception>
        public void Set(byte b)
        {
            EnsureByteIsNotSet();
            RuntimeHelper.ExecuteCodeWithGuaranteedCleanup(
                () =>
                {
                    //Generate ID
                    _id = _byteIdGenerator.Generate(b);
                    //Encrypt
                    Encrypt(b);
                    IsByteSet = true;
                },
                () =>
                {
                    _memoryProtector.Protect(_encryptionKey);
                    _memoryProtector.Protect(_encryptedByte);
                });
        }

        /// <summary>
        ///     Decrypts and returns the byte that this <see cref="SafeByte" /> instance represents.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Thrown when byte is not set</exception>
        public byte Get()
        {
            EnsureByteIsSet();
            byte[] byteBuffer = null;
            try
            {
                _memoryProtector.Unprotect(_encryptionKey);
                _memoryProtector.Unprotect(_encryptedByte);
                var encryptedBuffer = new byte[_encryptedByteLength];
                try
                {
                    Buffer.BlockCopy(_encryptedByte, 0, encryptedBuffer, 0, _encryptedByteLength);
                    byteBuffer = _encryptor.Decrypt(encryptedBuffer, _encryptionKey);
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
                _memoryProtector.Protect(_encryptionKey);
                _memoryProtector.Protect(_encryptedByte);
            }
        }

        /// <summary>
        ///     Deeply clone the object.
        /// </summary>
        /// <returns>
        ///     Cloned object.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown when byte is not set</exception>
        public ISafeByte DeepClone()
        {
            EnsureByteIsSet();
            _memoryProtector.Unprotect(_encryptionKey);
            _memoryProtector.Unprotect(_encryptedByte);
            var clone = new SafeByte(_id, _realBytePosition, _encryptedByteLength, _encryptionKey, _encryptedByte,
                _encryptor, _fastRandom, _byteIdGenerator, _memoryProtector);
            _memoryProtector.Protect(_encryptionKey);
            _memoryProtector.Protect(_encryptedByte);
            return clone;
        }

        public bool Equals(ISafeByte other)
        {
            if (other == null)
                return false;
            if (!IsByteSet && !other.IsByteSet)
                return true;
            if (IsByteSet && other.IsByteSet)
                return _id == other.GetHashCode();
            return false;
        }

        public bool Equals(byte other)
        {
            if (!IsByteSet)
                return false;
            return _id == _byteIdGenerator.Generate(other);
        }

        /// <summary>
        ///     Frees the encryption resources.
        /// </summary>
        public void Dispose()
        {
            _memoryProtector.Unprotect(_encryptionKey);
            _memoryProtector.Unprotect(_encryptedByte);
            Array.Clear(_encryptedByte, 0, _encryptedByte.Length);
            Array.Clear(_encryptionKey, 0, _encryptionKey.Length);
        }

        private void Encrypt(byte b)
        {
            //Mix the with arbitrary bytes
            _realBytePosition = _fastRandom.GetInt(0, SaltSize);
            var arbitraryBytes = _fastRandom.GetBytes(SaltSize);
            RuntimeHelper.ExecuteCodeWithGuaranteedCleanup(
                //Action
                () =>
                {
                    arbitraryBytes[_realBytePosition] = b;
                    //Get key
                    _encryptionKey = _fastRandom.GetBytes(KeySize);
                    //Encrypt
                    var encryptedBuffer = default(byte[]);
                    RuntimeHelper.ExecuteCodeWithGuaranteedCleanup(
                        //Action
                        () =>
                        {
                            encryptedBuffer = _encryptor.Encrypt(arbitraryBytes, _encryptionKey);
                            //Add arbitrary bytes
                            _encryptedByteLength = encryptedBuffer.Length;
                            _encryptedByte = GetMemoryProtectableSizedBytes(encryptedBuffer);
                        },
                        //Cleanup
                        () =>
                        {
                            if (encryptedBuffer != null)
                                Array.Clear(encryptedBuffer, 0, encryptedBuffer.Length);
                        });
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
            var multipleOfRule = _memoryProtector.BlockSizeInBytes;
            var length = byteArray.Length;
            var fixedLength = length - length%multipleOfRule + multipleOfRule;
            var result = new byte[fixedLength];
            Buffer.BlockCopy(byteArray, 0, result, 0, length);
            for (var i = byteArray.Length; i < fixedLength; i++)
                result[i] = _fastRandom.GetBytes(1).First();
            return result;
        }

        public override bool Equals(object obj)
        {
            var sb = obj as SafeByte;
            if (sb != null)
                return Equals(sb);
            if (obj is byte)
                return Equals((byte) obj);
            return false;
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
            return _id;
        }

        public override string ToString()
        {
#if DEBUG
            return $"InnerByte = {Get()}";
#else
            return "";
#endif
        }

        /// <exception cref="System.InvalidOperationException">Thrown when byte is not set</exception>
        private void EnsureByteIsSet()
        {
            if (!IsByteSet) throw new InvalidOperationException($"Byte must be set using {nameof(Set)} method.");
        }

        /// <exception cref="System.InvalidOperationException">Thrown when byte is already set</exception>
        private void EnsureByteIsNotSet()
        {
            if (IsByteSet) throw new InvalidOperationException("Byte is already set");
        }
    }
}