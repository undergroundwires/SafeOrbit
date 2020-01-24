using System;
using System.Collections.Generic;
using System.Linq;
using SafeOrbit.Library;
using SafeOrbit.Memory.InjectionServices;
using SafeOrbit.Memory.SafeBytesServices.Id;

namespace SafeOrbit.Memory.SafeBytesServices.Factory
{
    /// <summary>
    ///     Memory cached provider for <see cref="ISafeByte" />.
    ///     It saves the <see cref="ISafeByte.Id" /> of in the memory as keys for fast and hashed access.
    /// </summary>
    /// <seealso cref="ISafeByteFactory" />
    /// <seealso cref="ISafeByte" />
    /// <seealso cref="IByteIdGenerator" />
    internal class MemoryCachedSafeByteFactory : ISafeByteFactory
    {
        public const SafeObjectProtectionMode DefaultInnerDictionaryProtection = SafeObjectProtectionMode.JustState;
        private static readonly object SyncRoot = new object();
        private readonly IByteIdGenerator _byteIdGenerator;
        private readonly IFactory<ISafeByte> _safeByteFactory;
        private readonly ISafeObjectFactory _safeObjectFactory;
        private readonly SafeObjectProtectionMode _innerDictionaryProtectionMode;

        /// <summary>
        ///     Returns if the factory instances are cached in the memory.
        /// </summary>
        /// <seealso cref="_safeBytesDictionary" />
        private bool _isCached;

        private ISafeObject<Dictionary<int, ISafeByte>> _safeBytesDictionary;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MemoryCachedSafeByteFactory" /> class.
        /// </summary>
        public MemoryCachedSafeByteFactory() : this(DefaultInnerDictionaryProtection)
        {
        }

        public MemoryCachedSafeByteFactory(SafeObjectProtectionMode innerDictionaryProtectionMode) :
            this(
                SafeOrbitCore.Current.Factory.Get<IByteIdGenerator>(),
                SafeOrbitCore.Current.Factory.Get<IFactory<ISafeByte>>(),
                SafeOrbitCore.Current.Factory.Get<ISafeObjectFactory>(),
                innerDictionaryProtectionMode)
        {
        }

        internal MemoryCachedSafeByteFactory(IByteIdGenerator byteIdGenerator, IFactory<ISafeByte> safeByteFactory,
            ISafeObjectFactory safeObjectFactory, SafeObjectProtectionMode protectionMode)
        {
            _byteIdGenerator = byteIdGenerator ?? throw new ArgumentNullException(nameof(byteIdGenerator));
            _safeByteFactory = safeByteFactory ?? throw new ArgumentNullException(nameof(safeByteFactory));
            _safeObjectFactory = safeObjectFactory;
            _innerDictionaryProtectionMode = protectionMode;
        }


        /// <inheritdoc />
        /// <remarks>
        ///     Initializes the service by caching all instances in the memory.
        ///     Virtual for testing testing purposes.
        /// </remarks>
        public virtual void Initialize()
        {
            lock (SyncRoot)
            {
                if (_isCached) return;
                //Allocate place
                var safeBytes = GetAllSafeBytes();
                var dictionary = safeBytes.ToDictionary(safeByte => safeByte.Id);
                var settings = new InitialSafeObjectSettings
                (
                    protectionMode: SafeObjectProtectionMode
                        .JustState, /* TODO: Ignore _innerDictionaryProtectionMode because code protection is broken for dictionary as of v0.1 */
                    initialValue: dictionary, /* set our dictionary */
                    isReadOnly: true, /* the dictionary will not be modifiable after initialization */
                    alertChannel: InjectionAlertChannel
                        .ThrowException //**TODO : FIx here by implementing Ialerts for the class**/
                );
                _safeBytesDictionary = _safeObjectFactory.Get<Dictionary<int, ISafeByte>>(settings);
                _isCached = true;
            }
        }

        /// <inheritdoc />
        public ISafeByte GetByByte(byte @byte)
        {
            EnsureInitialized();
            var id = _byteIdGenerator
                .Generate(@byte); //Do not skip or cache this step to minimize the amount of time for ids to be seen.
            return _safeBytesDictionary.Object[id];
        }

        /// <inheritdoc />
        public ISafeByte GetById(int safeByteId)
        {
            EnsureInitialized();
            return _safeBytesDictionary.Object[safeByteId];
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/></exception>
        public IEnumerable<ISafeByte> GetByBytes(SafeMemoryStream stream)
        {
            EnsureInitialized();
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            var ids = _byteIdGenerator.GenerateMany(stream);
            var dictObj = _safeBytesDictionary.Object; // We call getter once so integrity is only checked once for better performance
            foreach (var id in ids)
                yield return dictObj[id];
        }

        private IEnumerable<ISafeByte> GetAllSafeBytes()
        {
            const int totalBytes = 256;
            var safeBytes = new ISafeByte[totalBytes];
            for (var i = 0; i < totalBytes; i++)
            {
                var @byte = (byte) i;
                var safeByte = _safeByteFactory.Create();
                safeByte.Set(@byte);
                safeBytes[i] = safeByte;
            }
            //Fast.For(0, 256, i =>
            //{
            //    var @byte = (byte)i;
            //    safeBytes[i] = _safeByteFactory.Create();
            //    safeBytes[i].Set(@byte);
            //});
            return safeBytes;
        }

        private void EnsureInitialized()
        {
            if (!_isCached) Initialize();
        }
    }
}