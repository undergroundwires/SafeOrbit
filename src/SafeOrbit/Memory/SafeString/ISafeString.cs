using System;
using System.Threading.Tasks;
using SafeOrbit.Core;
using SafeOrbit.Text;

namespace SafeOrbit.Memory
{
    public interface ISafeString : IAsyncEquatable<string>, IAsyncEquatable<ISafeString>,
        IAsyncDeepCloneable<ISafeString>, IShallowCloneable<ISafeString>,
        IDisposable
    {
        bool IsEmpty { get; }
        bool IsDisposed { get; }

        /// <summary>
        ///     Gets the number of characters in the current SafeString
        ///     Disposed SafeString returns 0
        /// </summary>
        int Length { get; }

        Task<ISafeBytes> ToSafeBytesAsync();
        Task<char> GetAsCharAsync(int index);
        ISafeBytes GetAsSafeBytes(int index);

        /// <summary>
        ///     Appends a <see cref="String" /> that's already revealed in the memory.
        /// </summary>
        Task AppendAsync(char ch);

        /// <summary>
        ///     Append bytes of a single character
        /// </summary>
        /// <param name="character">SafeBytes object that contains bytes for one single char</param>
        /// <param name="encoding">Encoding type that bytes are stored. Default = little endian UTF16</param>
        Task AppendAsync(ISafeBytes character, Encoding encoding = Encoding.Utf16LittleEndian);

        Task AppendAsync(ISafeString safeString);

        /// <summary>
        ///     Adds string that's not important for memory leaks. Will be seen in memory.
        /// </summary>
        /// <param name="text">String to append</param>
        Task AppendAsync(string text);

        /// <summary>
        ///     Appends the default line terminator (Environment.NewLine) to the end of the
        ///     current SafeString object
        /// </summary>
        Task AppendLineAsync();
        Task InsertAsync(int index, char character);
        Task InsertAsync(int position, ISafeBytes character, Encoding encoding = Encoding.Utf16LittleEndian);

        /// <param name="startIndex">Position of char to remove</param>
        /// <param name="count">[Optional] Default = 1 single character</param>
        void Remove(int startIndex, int count = 1);

        /// <summary>
        ///     Removes all data
        /// </summary>
        void Clear();

        int GetHashCode();
    }
}