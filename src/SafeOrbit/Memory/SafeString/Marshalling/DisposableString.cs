using System;
using System.Runtime.InteropServices;
using SafeOrbit.Common;

namespace SafeOrbit.Memory.SafeStringServices
{
    internal class DisposableString : DisposableBase, IDisposableString
    {
        private readonly string _string;
        private readonly GCHandle _handle;

        /// <inheritdoc />
        /// <inheritdoc cref="DisposableBase.ThrowIfDisposed" />
        public string String
        {
            get
            {
                this.ThrowIfDisposed();
                return _string;
            }
        }

        public DisposableString(string @string, GCHandle handle)
        {
            _string = @string ?? throw new ArgumentNullException(nameof(@string));
            _handle = handle;
        }

        protected override void DisposeUnmanagedResources()
        {
            if (!_handle.IsAllocated)
                return;
            unsafe
            {
                // Determine the length of the string
                var length = _string.Length;
                // Zero each character of the string.
                var pInsecureString = (char*)_handle.AddrOfPinnedObject();
                for (var index = 0; index < length; index++)
                    pInsecureString[index] = '\0';
                // Free the handle so the garbage collector
                // can dispose of it properly.
                _handle.Free();
            }
        }
    }
}
