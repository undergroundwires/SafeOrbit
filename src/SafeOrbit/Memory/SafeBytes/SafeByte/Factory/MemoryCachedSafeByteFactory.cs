using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        private readonly SemaphoreSlim _initializationSlim = new SemaphoreSlim(1, 1); // Max 1 thread can access the lock
        private readonly IByteIdGenerator _byteIdGenerator;
        private readonly IFactory<ISafeByte> _safeByteFactory;
        private readonly ISafeObjectFactory _safeObjectFactory;
        private readonly SafeObjectProtectionMode _innerDictionaryProtectionMode;

        /// <summary>
        ///     Returns if the factory instances are cached in the memory.
        /// </summary>
        /// <seealso cref="_safeBytesDictionary" />
        private volatile bool _isCached;

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
        public virtual async Task InitializeAsync()
        {
            await _initializationSlim.WaitAsync().ConfigureAwait(false);
            try
            {
                if (_isCached) return;
                // Allocate place
                var safeBytes = await GetAllSafeBytesAsync().ConfigureAwait(false);
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
            finally
            {
                _initializationSlim.Release();
            }
        }

        /// <inheritdoc />
        public async Task<ISafeByte> GetByByteAsync(byte @byte)
        {
            await EnsureInitializedAsync().ConfigureAwait(false);
            var id = await _byteIdGenerator
                .GenerateAsync(@byte) //Do not skip or cache this step to minimize the amount of time for ids to be seen.
                .ConfigureAwait(false);
            return _safeBytesDictionary.Object[id];
        }

        /// <inheritdoc />
        public async Task<ISafeByte> GetByIdAsync(int safeByteId)
        {
            await EnsureInitializedAsync().ConfigureAwait(false);
            return _safeBytesDictionary.Object[safeByteId];
        }

        public async Task<ISafeByte[]> GetByIdsAsync(IEnumerable<int> safeByteIds)
        {
            await EnsureInitializedAsync().ConfigureAwait(false);
            var dictObj = _safeBytesDictionary.Object; // We call getter once so integrity is only checked once for better performance
            return safeByteIds.Select(id => dictObj[id])
                .ToArray();
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/></exception>
        public async Task<IEnumerable<ISafeByte>> GetByBytesAsync(SafeMemoryStream stream) //TODO: To IAsyncEnumerable with C# 8.0
        {
            await EnsureInitializedAsync().ConfigureAwait(false);
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            var ids = await _byteIdGenerator.GenerateManyAsync(stream)
                .ConfigureAwait(false);
            var dictObj = _safeBytesDictionary.Object; // We call getter once so integrity is only checked once for better performance
            return ids.Select(id => dictObj[id]);
        }

        private async Task<ISafeByte[]> GetAllSafeBytesAsync()
        {
            const int totalBytes = 256;
            var safeBytes = new ISafeByte[totalBytes];
            for (var i = 0; i < totalBytes; i++)
            {
                var @byte = (byte)i;
                var safeByte = _safeByteFactory.Create();
                await safeByte.SetAsync(@byte).ConfigureAwait(false);
                safeBytes[i] = safeByte;
            }
            return safeBytes;
        }

        private async Task EnsureInitializedAsync()
        {
            if (!_isCached)
            {
                await InitializeAsync()
                    .ConfigureAwait(false);
            }
        }
    }
}