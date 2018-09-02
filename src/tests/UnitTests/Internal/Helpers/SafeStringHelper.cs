using System;
using SafeOrbit.Memory;

namespace SafeOrbit.UnitTests
{
    internal static class SafeStringHelper
    {
        /// <summary>
        ///     For easier syntax to append and return the appended instance.
        /// </summary>
        /// <example>
        ///     <code>new SafeBytes().AppendAndReturnDeepClone(5).AppendAndReturnDeepClone(10);</code>
        /// </example>
        /// <exception cref="ObjectDisposedException">Throws if the SafeBytes instance is disposed</exception>
        public static ISafeString AppendAndReturnDeepClone(this ISafeString safeString, char c)
        {
            safeString.Append(c);
            return safeString.DeepClone();
        }
    }
}