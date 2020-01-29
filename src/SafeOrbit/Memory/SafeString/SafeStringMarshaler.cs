using System;
using System.Runtime.InteropServices;
using SafeOrbit.Common;
using SafeOrbit.Helpers;
using SafeOrbit.Threading;

namespace SafeOrbit.Memory
{
    /// <summary>
    ///     <p>Lets you use the SecureString inside SafeString until it's disposed</p>
    ///     <p>Creates an allocated string in the memory after setting its <see cref="SafeString" /> property.</p>
    ///     <p>The place and the string is removed from the memory after disposing it.</p>
    /// </summary>
    /// <remarks>
    ///     <p>
    ///         Based on a marshaler for SecureString :
    ///         https://github.com/hilalsaim/sambapos/blob/master/Samba.Core/SecureStringToStringMarshaller.cs
    ///     </p>
    /// </remarks>
    /// <example>
    ///     <code>
    ///           using(var sm = new SafeStringToStringMarshaler(safeString))
    ///           {
    ///             // Use sm.String here.  While in the 'using' block, the string is accessible
    ///             // but pinned in memory.  When the 'using' block terminates, the string is zeroed
    ///             // out for security, and garbage collected as usual.
    ///           }
    ///     </code>
    /// </example>
    public class SafeStringToStringMarshaler : DisposableBase, ISafeStringToStringMarshaler
    {
        private GCHandle _gch;
        private ISafeString _safeString;

        /// <summary>
        ///     <p>Initializes a new instance of the <see cref="SafeStringToStringMarshaler" /> class.</p>
        ///     <p>
        ///         You need to set <see cref="SafeString" /> explicitly to get <see cref="String" /> property for the given
        ///         <see cref="ISafeString" />
        ///     </p>
        /// </summary>
        public SafeStringToStringMarshaler()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SafeStringToStringMarshaler" /> class using a
        ///     <see cref="ISafeString" />. Plain data of the <paramref name="safeString" /> will be available in
        ///     <see cref="String" /> property until this instance is disposed.
        /// </summary>
        /// <param name="safeString">The safe string to decrypt.</param>
        public SafeStringToStringMarshaler(ISafeString safeString)
        {
            SafeString = safeString;
        }

        /// <inheritdoc />
        /// <exception cref="T:System.ArgumentNullException" accessor="set"><paramref name="value" /> is <see langword="null" />.</exception>
        /// <exception cref="ObjectDisposedException" accessor="set"><paramref name="value" /> is disposed or <see cref="SafeStringToStringMarshaler"/> instance is disposed</exception>
        public ISafeString SafeString
        {
            get => _safeString;
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(SafeString));
                if (value.IsDisposed) throw new ObjectDisposedException(nameof(SafeString));
                this.ThrowIfDisposed();
                _safeString = value;
                UpdateStringValue();
            }
        }

        /// <inheritdoc />
        public string String { get; protected set; }

        protected override void DisposeUnmanagedResources()
        {
            this.Deallocate();
        }

        /// <summary>
        ///     Updates the string value from previously set SafeString.
        /// </summary>
        private void UpdateStringValue()
        {
            if (Memory.SafeString.IsNullOrEmpty(SafeString) || SafeString.IsDisposed)
            {
                String = "";
                return;
            }

            Deallocate();
            unsafe
            {
                String = new string('\0', SafeString.Length);
                _gch = new GCHandle();

                RuntimeHelper.PrepareConstrainedRegions();
                try { /* Not constrained region */ }
                finally
                {
                    // Pin our string, disallowing the garbage collector from moving it around.
                    _gch = GCHandle.Alloc(String, GCHandleType.Pinned);
                }


                RuntimeHelper.PrepareConstrainedRegions();
                try { /* Not constrained region */ }
                finally
                {
                    var pInsecureString = (char*) _gch.AddrOfPinnedObject();
                    for (var i = 0; i < SafeString.Length; i++)
                    {
                        var currentIndex = i;
                        pInsecureString[i] = TaskContext.RunSync(() => SafeString.RevealDecryptedCharAsync(currentIndex));
                    }
                }
            }
        }


        internal void Deallocate()
        {
            if (String == null)
                return;
            if (_gch.IsAllocated)
                unsafe
                {
                    // Determine the length of the string
                    var length = String.Length;
                    // Zero each character of the string.
                    var pInsecureString = (char*) _gch.AddrOfPinnedObject();
                    for (var index = 0; index < length; index++)
                        pInsecureString[index] = '\0';
                    // Free the handle so the garbage collector
                    // can dispose of it properly.
                    _gch.Free();
                    String = null;
                }
        }
    }
}