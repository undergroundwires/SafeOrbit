using SafeOrbit.Memory;

namespace SafeOrbit.UnitTests
{
    internal static class SafeBytesHelper
    {
        /// <summary>
        ///     Class SafeBytesHelper.
        /// </summary>
        /// <summary>
        ///     Appends the and return deep clone.
        /// </summary>
        /// <param name="safeBytes">The safe bytes.</param>
        /// <param name="byte">The byte to add.</param>
        /// <returns>DeepClone of appended <see cref="@byte" />.</returns>
        public static ISafeBytes AppendAndReturnDeepClone(this ISafeBytes safeBytes, byte @byte)
        {
            safeBytes.Append(@byte);
            return safeBytes.DeepClone();
        }
    }
}