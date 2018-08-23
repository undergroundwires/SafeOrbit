
/*
MIT License

Copyright (c) 2016-2018 Erkin Ekici - undergroundwires@safeorb.it

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
using System.Collections.Generic;
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
        /// <summary>
        ///     Returns if the factory instances are cached in the memory.
        /// </summary>
        /// <seealso cref="_safeBytesDictionary" />
        private bool _isCached;

        /// <summary>
        ///     Protected dictionary that holds <see cref="int" /> keys and <see cref="ISafeByte" /> instances.
        /// </summary>
        /// <seealso cref="SafeObject{Dictionary}" />
        private ISafeObject<Dictionary<int, ISafeByte>> _safeBytesDictionary;

        /// <summary>
        ///     The byte identifier generator.
        /// </summary>
        /// <seealso cref="HashedByteIdGenerator" />
        private readonly IByteIdGenerator _byteIdGenerator;

        /// <summary>
        ///     The safe byte factory.
        /// </summary>
        /// <seealso cref="MemoryCachedSafeByteFactory" />
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
            if (_isCached) return;
            var dictionary = new Dictionary<int, ISafeByte>();
            //Allocate place
            var safeBytes = GetAllSafeBytes();
            foreach (var safeByte in safeBytes) //faster than ToDictionary
                dictionary.Add(safeByte.Id, safeByte);
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