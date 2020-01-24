using System;

namespace SafeOrbit.Memory
{
    public interface ISafeBytes : IDisposable, IDeepCloneable<ISafeBytes>,
        IEquatable<byte[]>, IEquatable<ISafeBytes>
    {
        /// <summary>
        ///     Returns to real length of the bytes inside
        /// </summary>
        int Length { get; }
        bool IsDisposed { get; }
        /// <summary>
        ///     Adds the byte t the end of the list.
        /// </summary>
        void Append(byte b);
        /// <summary>
        ///     Adds the given bytes at the end of the list.
        /// </summary>
        void AppendMany(SafeMemoryStream bytes);
        /// <summary>
        ///     Adds the given encrypted bytes t the end of the list.
        /// </summary>
        void Append(ISafeBytes safeBytes);
        /// <summary>
        ///     Gets single byte in the safe list
        /// </summary>
        /// <param name="position">Index of the byte</param>
        /// <returns>Byte from the array</returns>
        byte GetByte(int position);
        byte[] ToByteArray();
        int GetHashCode();
    }
}