
/*
MIT License

Copyright (c) 2016 Erkin Ekici - undergroundwires@safeorb.it

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

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