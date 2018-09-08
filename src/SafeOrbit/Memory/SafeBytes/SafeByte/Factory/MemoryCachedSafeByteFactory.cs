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
        private static object SyncRoot = new object();
        public const SafeObjectProtectionMode DefaultInnerDictionaryProtection = SafeObjectProtectionMode.JustState;
        /// <summary>
        ///     Returns if the factory instances are cached in the memory.
        /// </summary>
        /// <seealso cref="_safeBytesDictionary" />
        private bool _isCached;
        private ISafeObject<Dictionary<int, ISafeByte>> _safeBytesDictionary;
        private readonly IByteIdGenerator _byteIdGenerator;
        private readonly IFactory<ISafeByte> _safeByteFactory;
        private readonly ISafeObjectFactory _safeObjectFactory;

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
            InnerDictionaryProtectionMode = protectionMode;
        }

        /// <summary>
        ///     Gets the <see cref="SafeObjectProtectionMode" /> for dictionary protection mode.
        /// </summary>
        /// <value>The dictionary protection mode.</value>
        public SafeObjectProtectionMode InnerDictionaryProtectionMode { get; }

        /// <summary>
        ///     Initializes the service by caching all instances in the memory.
        /// </summary>
        /// <remarks>
        /// Virtual for testing testing purposes.
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
                    protectionMode: SafeObjectProtectionMode.JustState, /* code protection is broken for dictionary as of v0.1 */
                    initialValue: dictionary, /* set our dictionary */
                    isReadOnly: true, /* the dictionary will not be modifiable after initialization */
                    alertChannel: InjectionAlertChannel.ThrowException //**TODO : FIx here by implementing Ialerts for the class**/
                );
                _safeBytesDictionary = _safeObjectFactory.Get<Dictionary<int, ISafeByte>>(settings);
                _isCached = true;
            }
        }
        /// <summary>
        /// Returns the cached <see cref="ISafeByte" /> for the specified <see cref="byte" />.
        /// </summary>
        /// <param name="byte">The byte.</param>
        public ISafeByte GetByByte(byte @byte)
        {
            EnsureInitialized();
            var id = _byteIdGenerator.Generate(@byte); //Do not skip or cache this step to minimize the amount of time for ids to be seen.
            return _safeBytesDictionary.Object[id];
        }
        /// <summary>
        /// Returns cached the <see cref="ISafeByte" /> for the specified <see cref="ISafeByte.Id" />.
        /// </summary>
        /// <param name="safeByteId">The safe byte identifier.</param>
        /// <seealso cref="IByteIdGenerator" />
        public ISafeByte GetById(int safeByteId)
        {
            EnsureInitialized();
            return _safeBytesDictionary.Object[safeByteId];
        }

        private IEnumerable<ISafeByte> GetAllSafeBytes()
        {
            var safeBytes = new ISafeByte[256];
            for (var i = 0; i < 256; i++)
            {
                var @byte = (byte) i;
                safeBytes[i] = _safeByteFactory.Create();
                safeBytes[i].Set(@byte);
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