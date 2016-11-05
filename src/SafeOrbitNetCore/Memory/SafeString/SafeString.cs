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
using System.Collections.Generic;
using System.Linq;
using SafeOrbit.Library;
using SafeOrbit.Text;

namespace SafeOrbit.Memory
{
    //TODO: write tests for public void Insert(int position, ISafeBytes character, Encoding encoding = Encoding.Utf16LittleEndian)
    public class SafeString : ISafeString
    {
        private const Encoding InnerEncoding = Encoding.Utf16LittleEndian;
        private const byte LineFeed = 0x000A; //http://www.fileformat.info/info/unicode/char/000A/index.htm
        private readonly IList<ISafeBytes> _charBytesList;
        private readonly IFactory<ISafeBytes> _safeBytesFactory;
        private readonly IFactory<ISafeString> _safeStringFactory;
        private readonly ITextService _textService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SafeString" /> class.
        /// </summary>
        public SafeString() : this(LibraryManagement.Current.Factory.Get<ITextService>(),
            SafeContainerWrapper<ISafeString>.Wrap(),
            SafeContainerWrapper<ISafeBytes>.Wrap())
        {
        }

        internal SafeString(
            ITextService textService,
            IFactory<ISafeString> safeStringFactory,
            IFactory<ISafeBytes> safeBytesFactory)
        {
            if (textService == null) throw new ArgumentNullException(nameof(textService));
            if (safeStringFactory == null) throw new ArgumentNullException(nameof(safeStringFactory));
            if (safeBytesFactory == null) throw new ArgumentNullException(nameof(safeBytesFactory));
            _textService = textService;
            _safeStringFactory = safeStringFactory;
            _safeBytesFactory = safeBytesFactory;
            _charBytesList = new List<ISafeBytes>();
        }

        public int Length => (_charBytesList == null) || IsDisposed ? 0 : _charBytesList.Count;
        public bool IsEmpty => Length == 0;
        public bool IsDisposed { get; private set; }

        /// <exception cref="ObjectDisposedException">Throws if the <see cref="SafeString" /> instance is disposed.</exception>
        public void Append(char ch)
        {
            Insert(Length, ch);
        }

        /// <summary>
        ///     Appends a <see cref="string" /> that's already revealed in the memory.
        /// </summary>
        /// <param name="text">Non-safe <see cref="string" /> that's already revealed in the memory</param>
        /// <exception cref="ArgumentNullException"> <paramref name="text" /> is <see langword="null" />.</exception>
        public void Append(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            EnsureNotDisposed();
            if (text.Length == 0)
                return;
            foreach (var ch in text)
                Append(ch);
        }

        /// <exception cref="ArgumentNullException">
        ///     <paramref name="character" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     If position is less than zero or higher than the length.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     Throws if the SafeString instance is disposed.
        /// </exception>
        public void Append(ISafeBytes character, Encoding encoding = Encoding.Utf16LittleEndian)
        {
            Insert(Length, character, encoding);
        }

        /// <exception cref="ArgumentNullException">
        ///     <paramref name="safeString" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     Throws if the SafeString instance is disposed.
        /// </exception>
        public void Append(ISafeString safeString)
        {
            if (safeString == null) throw new ArgumentNullException(nameof(safeString));
            EnsureNotDisposed();
            for (var i = 0; i < safeString.Length; i++)
                Append(safeString.GetAsSafeBytes(i).DeepClone());
        }

        /// <exception cref="ObjectDisposedException">Throws if the SafeString instance is disposed</exception>
        public void AppendLine()
        {
            EnsureNotDisposed();
            var safeBytes = _safeBytesFactory.Create();
            safeBytes.Append(LineFeed);
            Append(safeBytes, InnerEncoding);
        }

        /// <exception cref="ObjectDisposedException">Throws if the <see cref="ISafeBytes" /> instance is disposed</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> is not a valid index.</exception>
        public void Insert(int index, char character)
        {
            if ((index < 0) || (index > Length)) throw new ArgumentOutOfRangeException(nameof(index));
            EnsureNotDisposed();
            var bytes = TransformCharToSafeBytes(character, InnerEncoding);
            _charBytesList.Insert(index, bytes);
        }

        /// <exception cref="ArgumentNullException">
        ///     <paramref name="character" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     Throws if the SafeString instance is disposed.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Throws if <paramref name="character" /> is less than zero or higher than the length.
        /// </exception>
        public void Insert(int position, ISafeBytes character, Encoding encoding = Encoding.Utf16LittleEndian)
        {
            EnsureNotDisposed();
            if ((position < 0) || (position > Length)) throw new ArgumentOutOfRangeException(nameof(position));
            if (SafeBytes.IsNullOrEmpty(character)) throw new ArgumentNullException(nameof(character));
            if (encoding == InnerEncoding)
            {
                _charBytesList.Insert(position, character);
                return;
            }
            //Convert encoding
            var buffer = character.ToByteArray();
            try
            {
                buffer = _textService.Convert(encoding, InnerEncoding, buffer);
                var safeBytes = _safeBytesFactory.Create();
                for (var i = 0; i < buffer.Length; i++)
                {
                    //Append
                    safeBytes.Append(buffer[i]);
                    buffer[i] = 0x0;
                }
                _charBytesList.Insert(position, safeBytes);
            }
            finally
            {
                Array.Clear(buffer, 0, buffer.Length);
            }
        }

        /// <exception cref="ObjectDisposedException">Throws if the <see cref="SafeString" /> instance is disposed.</exception>
        /// <exception cref="InvalidOperationException">Throws if the <see cref="SafeString" /> instance is empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="index" /> is not a valid index.
        /// </exception>
        public ISafeBytes GetAsSafeBytes(int index)
        {
            EnsureNotDisposed();
            EnsureNotEmpty();
            if ((index < 0) || (index >= Length)) throw new ArgumentOutOfRangeException(nameof(index));
            var result = _charBytesList.ElementAt(index);
            return result;
        }

        /// <exception cref="ObjectDisposedException">Throws if the <see cref="SafeString" /> instance is disposed.</exception>
        /// <exception cref="InvalidOperationException">Throws if the <see cref="SafeString" /> instance is empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Throws if <paramref name="index" /> is less than zero, higher than/equals
        ///     to the length.
        /// </exception>
        public char GetAsChar(int index)
        {
            EnsureNotDisposed();
            EnsureNotEmpty();
            if ((index < 0) || (index >= Length))
                throw new ArgumentOutOfRangeException(nameof(index));
            var asSafeBytes = _charBytesList.ElementAt(index);
            var asChar = TransformSafeBytesToChar(asSafeBytes, InnerEncoding);
            return asChar;
        }

        /// <exception cref="ArgumentOutOfRangeException">
        ///     Throws when <paramref name="startIndex" /> is out of range.
        ///     Throws when <paramref name="count" /> is less than one.
        ///     Throws when the total of <paramref name="startIndex" /> and <paramref name="count" /> is higher than lenth.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     Throws if the <see cref="SafeString" /> instance is disposed.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     Throws if the <see cref="SafeString" /> instance is empty.
        /// </exception>
        public void Remove(int startIndex, int count = 1)
        {
            EnsureNotDisposed();
            EnsureNotEmpty();
            if ((startIndex < 0) || (startIndex >= Length))
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (startIndex + count > Length)
                throw new ArgumentOutOfRangeException(
                    $"Total of {nameof(count)}({count}) and {nameof(startIndex)}({startIndex} is higher than the length({Length})");
            for (var i = 0; i < count; i++)
            {
                _charBytesList[startIndex].Dispose();
                _charBytesList[startIndex] = null;
                _charBytesList.RemoveAt(startIndex);
            }
        }

        /// <exception cref="ObjectDisposedException">Throws if the <see cref="SafeString" /> instance is disposed</exception>
        public void Clear()
        {
            EnsureNotDisposed();
            for (var i = 0; i < Length; i++)
                Remove(i);
            _charBytesList.Clear();
        }

        /// <exception cref="ObjectDisposedException">Throws if the <see cref="SafeString" /> instance is disposed</exception>
        /// <exception cref="InvalidOperationException">Throws if the <see cref="SafeString" /> instance is empty.</exception>
        public ISafeBytes ToSafeBytes()
        {
            EnsureNotDisposed();
            EnsureNotEmpty();
            var safeBytes = _safeBytesFactory.Create();
            foreach (var charBytes in _charBytesList)
                safeBytes.Append(charBytes.DeepClone());
            return safeBytes;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                const int multiplier = 27;
                var hashCode = 1;
                for (var i = 0; i < Length; i++)
                    hashCode *= multiplier*_charBytesList[i].GetHashCode();
                return hashCode;
            }
        }

        public bool Equals(string other)
        {
            if (other?.Length != Length)
                return false;
            using (var safeString = _safeStringFactory.Create())
            {
                safeString.Append(other);
                return Equals(safeString);
            }
        }

        public bool Equals(ISafeString other)
        {
            if (other?.Length != Length)
                return false;
            for (var i = 0; i < Length; i++)
                if (!GetAsSafeBytes(i).Equals(other.GetAsSafeBytes(i)))
                    return false;
            return true;
        }

        /// <exception cref="ObjectDisposedException">Throws if the <see cref="SafeString" /> instance is disposed</exception>
        public ISafeString DeepClone()
        {
            EnsureNotDisposed();
            var result = _safeStringFactory.Create();
            for (var i = 0; i < Length; i++)
                result.Append(GetAsSafeBytes(i).DeepClone(), InnerEncoding);
            return result;
        }

        /// <exception cref="ObjectDisposedException">Throws if the <see cref="SafeString" /> instance is disposed</exception>
        public ISafeString ShallowClone()
        {
            EnsureNotDisposed();
            // The MemberwiseClone method creates a shallow copy by creating a new object,
            // and then copying the nonstatic fields of the current object to the new object.
            // If a field is a value type, a bit - by - bit copy of the field is performed.
            // If a field is a reference type, the reference is copied but the referred object is not;
            // therefore, the original object and its clone refer to the same object.
            // Documentation : https://msdn.microsoft.com/en-us/library/system.object.memberwiseclone.aspx
            return MemberwiseClone() as ISafeString;
        }

        public void Dispose()
        {
            Clear();
            IsDisposed = true;
        }

        public static bool IsNullOrEmpty(ISafeString safeString)
            => (safeString == null) || safeString.IsDisposed || safeString.IsEmpty;

        public override bool Equals(object obj)
        {
            var safeString = obj as ISafeString;
            if (safeString != null)
                return Equals(safeString);
            if (obj is string)
                return Equals((string) obj);
            return false;
        }

        private char TransformSafeBytesToChar(ISafeBytes safeBytes, Encoding encoding)
        {
            var byteBuffer = safeBytes.ToByteArray();
            try
            {
                return _textService.GetChars(byteBuffer, InnerEncoding).First();
            }
            finally
            {
                Array.Clear(byteBuffer, 0, byteBuffer.Length);
            }
        }

        private ISafeBytes TransformCharToSafeBytes(char c, Encoding encoding)
        {
            byte[] byteBuffer = null;
            try
            {
                byteBuffer = _textService.GetBytes(c, encoding);
                var charBytes = _safeBytesFactory.Create();
                for (var i = 0; i < byteBuffer.Length; i++)
                {
                    charBytes.Append(byteBuffer[i]);
                    byteBuffer[i] = 0;
                }
                return charBytes;
            }
            finally
            {
                Array.Clear(byteBuffer, 0, byteBuffer.Length);
            }
        }

        /// <exception cref="ObjectDisposedException">Throws if the <see cref="SafeString" /> instance is disposed.</exception>
        private void EnsureNotDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(SafeString));
        }

        /// <exception cref="InvalidOperationException">Throws if the <see cref="SafeString" /> instance is empty.</exception>
        private void EnsureNotEmpty()
        {
            if (Length == 0)
                throw new InvalidOperationException($"{nameof(SafeString)} is empty");
        }

#if DEBUG
        public override string ToString()
        {
            var asString = "";
            for (var i = 0; i < Length; i++)
                asString += GetAsChar(i);
            return asString;
        }
#endif
    }
}