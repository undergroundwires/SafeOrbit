using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using SafeOrbit.Helpers;

namespace SafeOrbit.Memory.SafeStringServices
{
    /// <summary>
    ///     <p>Lets you use the SecureString inside SafeString until it's disposed</p>
    ///     <p>Creates an allocated string in the memory after setting its <see cref="SafeString" /> property.</p>
    ///     <p>The place and the string is removed from the memory after disposing it.</p>
    /// </summary>
    internal class SafeStringToStringMarshaler : ISafeStringToStringMarshaler
    {
        /// <summary>
        ///     Plain data of the <paramref name="safeString" /> will be available in
        ///     <see cref="String" /> property until this instance is disposed.
        /// </summary>
        /// <param name="safeString">The safe string to decrypt.</param>
        /// <exception cref="ObjectDisposedException"><paramref name="safeString"/> is disposed</exception>
        /// <exception cref="ArgumentNullException"> <paramref name="safeString" /> is <see langword="null" />. </exception>
        /// <exception cref="InvalidOperationException">Already initialized</exception>
        public async Task<IDisposableString> MarshalAsync(IReadOnlySafeString safeString)
        {
            if (safeString == null) throw new ArgumentNullException(nameof(safeString));
            if (safeString.IsDisposed)  throw new ObjectDisposedException(nameof(safeString));
            if (safeString.IsEmpty)
                return new DisposableString(string.Empty, default);
            var str = new string('\0', safeString.Length);
            var handle = PinString(str);
            await DecryptAndCopyAsync(safeString, handle).ConfigureAwait(false);
            return new DisposableString(str, handle);
        }
        /// <summary>
        /// Pins the string, disallowing the garbage collector from moving it around.
        /// </summary>
        private static GCHandle PinString(string str)
        {
            GCHandle handle;
            RuntimeHelper.PrepareConstrainedRegions();
            try { /* Not constrained region */ }
            finally
            {
                handle = GCHandle.Alloc(str, GCHandleType.Pinned);
            }
            return handle;
        }

        private static Task DecryptAndCopyAsync(IReadOnlySafeString safeString, GCHandle handle)
        {
            var tasks = new Task[safeString.Length];
            for (var i = 0; i < tasks.Length; i++)
            {
                var currentIndex = i;
                tasks[currentIndex] = safeString.RevealDecryptedCharAsync(i)
                    .ContinueWith(charTask => CopyChar(handle, currentIndex, charTask.Result));
            }
            return Task.WhenAll(tasks);
        }

        private static void CopyChar(GCHandle handle, int index, char @char)
        {
            unsafe
            {
                RuntimeHelper.PrepareConstrainedRegions();
                try { /* Not constrained region */ }
                finally
                {
                    var pInsecureString = (char*)handle.AddrOfPinnedObject(); 
                    pInsecureString[index] = @char;
                }
            }
        }
    }
}