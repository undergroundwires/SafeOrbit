using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using SafeOrbit.Interfaces;
using SafeOrbit.Library;
using SafeOrbit.Memory;
using SafeOrbit.Memory.SafeBytesServices;
using SafeOrbit.Memory.SafeBytesServices.Collection;
using SafeOrbit.Memory.SafeBytesServices.Factory;
using SafeOrbit.Random;

namespace SafeOrbit.Memory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    public class SafeBytes : ISafeBytes
    {
        /// <summary>
        /// Indicates whether the specified SafeBytes object is null or holds zero bytes.
        /// </summary>
        public static bool IsNullOrEmpty(ISafeBytes safebytes) => safebytes == null || safebytes.Length == 0;
        /// <summary>
        /// Returns to real length of the bytes inside
        /// </summary>
        public int Length => _safeByteCollection?.Length ?? 0;
        public bool IsDisposed { get; private set; }
        private readonly IFastRandom _fastRandom;
        private readonly ISafeByteFactory _safeByteFactory;
        private readonly IFactory<ISafeBytes> _safeBytesInstantiator;
        private readonly ISafeByteCollection _safeByteCollection;
        [DebuggerHidden]
        public SafeBytes() : this(
            LibraryManagement.Factory.Get<IFastRandom>(),
            LibraryManagement.Factory.Get<ISafeByteFactory>(),
            LibraryManagement.Factory.Get<IFactory<ISafeBytes>>(),
            LibraryManagement.Factory.Get<IFactory<ISafeByteCollection>>()) { }
        internal SafeBytes(
            IFastRandom fastRandom,
            ISafeByteFactory safeByteFactory,
            IFactory<ISafeBytes> safeBytesFactory,
            IFactory<ISafeByteCollection> safeByteCollectionFactory)
        {
            if (fastRandom == null)  throw new ArgumentNullException(nameof(fastRandom));
            if (safeByteFactory == null) throw new ArgumentNullException(nameof(safeByteFactory));
            if (safeBytesFactory == null) throw new ArgumentNullException(nameof(safeBytesFactory));
            _fastRandom = fastRandom;
            _safeByteFactory = safeByteFactory;
            _safeBytesInstantiator = safeBytesFactory;
            _safeByteCollection = safeByteCollectionFactory.Create();
        }
        /// <summary>
        /// Adds the byte, and one random byte after it to avoid memory leaking
        /// </summary>
        /// <exception cref="ObjectDisposedException">Throws if the SafeBytes instance is disposed</exception>
        public void Append(byte b)
        {
            EnsureNotDisposed();
            var safeByte = _safeByteFactory.GetByByte(b);
            _safeByteCollection.Append(safeByte);
        }
        /// <summary>
        /// Clones and appends the each byte from given SafeBytes object to the end.
        /// </summary>
        /// <exception cref="ArgumentNullException">Throws if the argument is null</exception>
        /// <exception cref="ObjectDisposedException">Throws if the SafeBytes instance is disposed</exception>
        /// <exception cref="ArgumentException">Throws if the argument is empty</exception>
        public void Append(ISafeBytes safeBytes)
        {
            EnsureNotDisposed();
            if (safeBytes == null) throw new ArgumentNullException(nameof(safeBytes));
            if (safeBytes.IsDisposed) throw new ObjectDisposedException(nameof(safeBytes));
            if (safeBytes.Length == 0) throw new ArgumentException($"{nameof(safeBytes)} is empty.");
            //If it's the known SafeBytes then it reveals nothing in the memory
            var asSafeBytes = safeBytes as SafeBytes;
            if (asSafeBytes != null)
            {
                for (var i = 0; i < safeBytes.Length; i++)
                {
                    var safeByte = asSafeBytes.GetAsSafeByte(i);
                    _safeByteCollection.Append(safeByte);
                }
            }
            //If it's not, then reveals each byte in memory.
            else
            {
                for (var i = 0; i < safeBytes.Length; i++)
                {
                    var safeByte = _safeByteFactory.GetByByte(safeBytes.GetByte(i));
                    _safeByteCollection.Append(safeByte);
                }
            }
            //Add the arbitrary byte
        }
        /// <summary>
        /// Gets the byte in the safe list
        /// </summary>
        /// <param name="position">Index of the byte</param>
        /// <returns>Byte from the array</returns>
        /// <exception cref="ArgumentOutOfRangeException">Throws if the position is lower than 0 or greater than/equals to the length</exception>
        /// <exception cref="ObjectDisposedException">Throws if the SafeBytes instance is disposed</exception>
        /// <exception cref="InvalidOperationException">Throws if the SafeBytes instance is empty.</exception>
        public byte GetByte(int position)
        {
            EnsureNotDisposed();
            EnsureNotEmpty();
            if (position < 0 && position >= this.Length) throw new ArgumentOutOfRangeException(nameof(position));
            return _safeByteCollection.Get(position).Get();
        }
        /// <exception cref="ObjectDisposedException">Throws if the SafeBytes instance is disposed</exception>
        /// <exception cref="InvalidOperationException">Throws if the SafeBytes instance is empty.</exception>
        public byte[] ToByteArray()
        {
            EnsureNotDisposed();
            EnsureNotEmpty();
            return _safeByteCollection.ToDecryptedBytes();
        }
        /// <exception cref="ObjectDisposedException">Throws if the SafeBytes instance is disposed</exception>
        public ISafeBytes DeepClone()
        {
            EnsureNotDisposed();
            var clone = _safeBytesInstantiator.Create();
            var asSafeBytes = clone as SafeBytes;
            if (asSafeBytes != null)
            {
                //If it's the known SafeBytes then it reveals nothing in the memory
                for (var i = 0; i < this.Length; i++)
                {
                    asSafeBytes.Append(this.GetAsSafeByte(i));
                }
                return asSafeBytes;
            }
            else
            {
                //If it's not, then reveals each byte in memory.
                for (var i = 0; i < this.Length; i++)
                {
                    clone.Append(this.GetByte(i));
                }
                return clone;
            }
        }

        #region IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed) return; //skip redundant calls
            if (disposing)
            {
                //managed resources
                _safeByteCollection.Dispose();
            }
            this.IsDisposed = true;
        }
        ~SafeBytes()
        {
            Dispose(false);
        }
        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Equality
        public bool Equals(byte[] other)
        {
            if (this.Length != other?.Length)
            {
                return false;
            }
            for (var i = 0; i < Length; i++)
            {
                if (!this.GetAsSafeByte(i).Equals(other[i]))
                {
                    return false;
                }
            }
            return true;
        }
        public bool Equals(ISafeBytes other)
        {
            if (this.Length != other?.Length)
            {
                return false;
            }
            var asSafeBytes = other as SafeBytes;
            if (asSafeBytes != null)
            {
                //If it's the known SafeBytes then it reveals nothing in the memory
                for (var i = 0; i < Length; i++)
                {
                    if (!this.GetAsSafeByte(i).Equals(asSafeBytes.GetAsSafeByte(i)))
                    {
                        return false;
                    }
                }
            }
            else
            {
                //If it's not, then reveals each byte in memory.
                for (var i = 0; i < Length; i++)
                {
                    if (!this.GetAsSafeByte(i).Equals(other.GetByte(i)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public override bool Equals(object obj)
        {
            var sb = obj as SafeBytes;
            if (sb != null)
            {
                return this.Equals(sb);
            }
            else if (obj is byte[])
            {
                return this.Equals((byte[])obj);
            }
            return false;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                const int multiplier = 26;
                var hashCode = 1;
                for (var i = 0; i < this.Length; i++)
                {
                    hashCode *= multiplier * this.GetAsSafeByte(i).Id;
                }
                return hashCode;
            }
        }
        #endregion

        internal ISafeByte GetAsSafeByte(int position)
        {
            var realPosition = position * 2;
            return _safeByteCollection.Get(realPosition);
        }
        /// <exception cref="ArgumentNullException"><paramref name="safeByte"/> is <see langword="null" />.</exception>
        private void Append(ISafeByte safeByte)
        {
            if (safeByte == null) throw new ArgumentNullException(nameof(safeByte));
            _safeByteCollection.Append(safeByte);
            AddArbitraryByte();
        }
        private void AddArbitraryByte()
        {
            var safeByte = _safeByteFactory.GetByByte((byte)_fastRandom.GetInt(0, 256));
            _safeByteCollection.Append(safeByte);
        }
        /// <exception cref="ObjectDisposedException">Throws if the SafeBytes instance is disposed</exception>
        private void EnsureNotDisposed()
        {
            if (IsDisposed) throw new ObjectDisposedException(nameof(SafeBytes));
        }
        /// <exception cref="InvalidOperationException">Throws if the SafeBytes instance is empty.</exception>
        private void EnsureNotEmpty()
        {
            if (this.Length == 0) throw new InvalidOperationException($"{nameof(SafeBytes)} is empty");
        }
    }
}
