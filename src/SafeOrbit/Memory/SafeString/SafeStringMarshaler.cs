/*


Here's how you use the class:

*/

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
    ///         https://sambapos.googlecode.com/hg/Samba.Infrastructure/SecureStringToStringMarshaller.cs
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
    public class SafeStringToStringMarshaler : ISafeStringToStringMarshaler
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

        /// <summary>
        ///     Gets or sets the safe string. Setting a new SafeString will automatically update the String property.
        /// </summary>
        /// <value>
        ///     The safe string that should be converted to a string.
        /// </value>
        /// <exception cref="ArgumentNullException" accessor="set"><paramref name="value" /> is <see langword="null" />.</exception>
        /// <exception cref="ObjectDisposedException" accessor="set">Throws when <paramref name="value" /> is disposed.</exception>
        public ISafeString SafeString
        {
            get { return _safeString; }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(SafeString));
                if (value.IsDisposed) throw new ObjectDisposedException(nameof(SafeString));
                _safeString = value;
                UpdateStringValue();
            }
        }

        /// <summary>
        ///     Gets or sets the string representation of the SafeString object.
        ///     This string will be cleared from the memory when the class is disposed.
        /// </summary>
        /// <value>
        ///     The string.
        /// </value>
        public string String { get; protected set; }

        public void Dispose()
        {
            Deallocate();
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
            unsafe
            {
                String = new string('\0', SafeString.Length);
                _gch = new GCHandle();
                // Create a CER (Constrained Execution Region)
                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                }
                finally
                {
                    // Pin our string, disallowing the garbage collector from moving it around.
                    _gch = GCHandle.Alloc(String, GCHandleType.Pinned);
                }
                // Create a CER (Constrained Execution Region)
                RuntimeHelpers.PrepareConstrainedRegions();
                var pInsecureString = (char*) _gch.AddrOfPinnedObject();
                for (var charIndex = 0; charIndex < SafeString.Length; charIndex++)
                    pInsecureString[charIndex] = SafeString.GetAsChar(charIndex);
            }
        }

        internal void Deallocate()
        {
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
                }
        }
    }
}