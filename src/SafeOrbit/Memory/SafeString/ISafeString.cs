using System;
using System.Threading.Tasks;
using SafeOrbit.Memory.SafeStringServices.Text;

namespace SafeOrbit.Memory
{
    public interface ISafeString : IReadOnlySafeString,
        IAsyncDeepCloneable<ISafeString>, IShallowCloneable<ISafeString>,
        IDisposable
    {
        /// <summary>
        ///     Appends a <see cref="char" /> that's already revealed in the memory.
        /// </summary>
        Task AppendAsync(char ch);

        /// <summary>
        ///     Appends a <see cref="string" /> that's already revealed in the memory.
        /// </summary>
        Task AppendAsync(string text);

        /// <summary>
        ///     Append bytes of a single character
        /// </summary>
        /// <param name="character">SafeBytes object that contains bytes for one single char</param>
        /// <param name="encoding">Encoding type that bytes are stored. Default = little endian UTF16</param>
        Task AppendAsync(ISafeBytes character, Encoding encoding = Encoding.Utf16LittleEndian);

        /// <summary>
        ///     Appends encrypted <paramref name="safeString"/> at the end of the current <see cref="ISafeString"/> instance.
        /// </summary>
        Task AppendAsync(ISafeString safeString);

        /// <summary>
        ///     Appends the default line terminator to the end of the current <see cref="ISafeString"/> instance.
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
    }
}