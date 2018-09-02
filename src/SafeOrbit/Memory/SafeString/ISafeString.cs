using System;
using SafeOrbit.Text;

namespace SafeOrbit.Memory
{
    public interface ISafeString : IEquatable<string>, IEquatable<ISafeString>,
        IDeepCloneable<ISafeString>, IShallowCloneable<ISafeString>,
        IDisposable
    {
        bool IsEmpty { get; }
        bool IsDisposed { get; }

        /// <summary>
        ///     Gets the number of characters in the current SafeString
        ///     Disposed SafeString returns 0
        /// </summary>
        int Length { get; }

        ISafeBytes ToSafeBytes();
        char GetAsChar(int index);
        ISafeBytes GetAsSafeBytes(int index);
        void Append(char ch);

        /// <summary>
        ///     Append bytes of a single character
        /// </summary>
        /// <param name="character">SafeBytes object that contains bytes for one single char</param>
        /// <param name="encoding">Encoding type that bytes are stored. Default = little endian UTF16</param>
        void Append(ISafeBytes character, Encoding encoding = Encoding.Utf16LittleEndian);

        void Append(ISafeString safeString);

        /// <summary>
        ///     Adds string that's not important for memory leaks. Will be seen in memory.
        /// </summary>
        /// <param name="text">String to append</param>
        void Append(string text);

        /// <summary>
        ///     Appends the default line terminator (Environment.NewLine) to the end of the
        ///     current SafeString object
        /// </summary>
        void AppendLine();

        void Insert(int index, char character);
        void Insert(int position, ISafeBytes character, Encoding encoding = Encoding.Utf16LittleEndian);

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