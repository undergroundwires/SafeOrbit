
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

using NUnit.Framework;
using SafeOrbit.Tests;

namespace SafeOrbit.Text
{
    public class TextServiceTests : TestsFor<ITextService>
    {
        protected override ITextService GetSut()
        {
            return new TextService();
        }

        #region [Convert]
        [Test]
        [TestCaseSource(typeof(EncodingVectors), nameof(EncodingVectors.AsciiBytes_To_Utf16BigEndianBytes_Vector))]
        public byte[] Convert_From_Ascii_To_Utf16BigEndian_returnsZeroBytePlusAscii(byte[] asciiBytes)
        {
            //Arrange
            var sut = GetSut();
            //Act
            var result = sut.Convert(Encoding.Ascii, Encoding.Utf16BigEndian, asciiBytes);
            //Assert
            return result;
        }
        [Test]
        [TestCaseSource(typeof(EncodingVectors), nameof(EncodingVectors.AsciiBytes_To_Utf16LittleEndianBytes_Vector))]
        public byte[] Convert_From_Ascii_To_Utf16LittleIndian_returnVector(byte[] asciiBytes)
        {
            //Arrange
            var sut = GetSut();
            //Act
            var result = sut.Convert(Encoding.Ascii, Encoding.Utf16LittleEndian, asciiBytes);
            //Assert
            return result;
        }
        [Test]
        [TestCaseSource(typeof(EncodingVectors), nameof(EncodingVectors.Utf16BigEndianBytes_To_AsciiBytes_Vector))]
        public byte[] Convert_From_Utf16BigEndian_To_Ascii_returnsVector(byte[] utf16BigEndianBytes)
        {
            //Arrange
            var sut = GetSut();
            //Act
            var result = sut.Convert(Encoding.Utf16BigEndian, Encoding.Ascii, utf16BigEndianBytes);
            //Assert
            return result;
        }
        [Test]
        [TestCaseSource(typeof(EncodingVectors), nameof(EncodingVectors.Utf16BigEndianBytes_To_Utf16LittleEndianBytes_Vector))]
        public byte[] Convert_From_Utf16BigEndian_To_Utf16LittleIndian_returnsVector(byte[] utf16BigEndianBytes)
        {
            //Arrange
            var sut = GetSut();
            //Act
            var result = sut.Convert(Encoding.Utf16BigEndian, Encoding.Utf16LittleEndian, utf16BigEndianBytes);
            //Assert
            return result;
        }
        [Test]
        [TestCaseSource(typeof(EncodingVectors), nameof(EncodingVectors.Utf16LittleEndianBytes_To_AsciiBytes_Vector))]
        public byte[] Convert_From_Utf16LittleEndian_To_Ascii_returnsVector(byte[] utf16LittleEndianBytes)
        {
            //Arrange
            var sut = GetSut();
            //Act
            var result = sut.Convert(Encoding.Utf16LittleEndian, Encoding.Ascii, utf16LittleEndianBytes);
            //Assert
            return result;
        }
        [Test]
        [TestCaseSource(typeof(EncodingVectors), nameof(EncodingVectors.Utf16LittleEndianBytes_To_Utf16BigEndianBytes_Vector))]
        public byte[] Convert_From_Utf16LittleEndian_To_Utf16BigEndian_returnsVector(byte[] utf16LittleEndianBytes)
        {
            //Arrange
            var sut = GetSut();
            //Act
            var result = sut.Convert(Encoding.Utf16LittleEndian, Encoding.Utf16BigEndian, utf16LittleEndianBytes);
            //Assert
            return result;
        }
        #endregion 

        #region [GetBytes(string,Encoding)]
        [Test]
        [TestCaseSource(typeof(EncodingVectors), nameof(EncodingVectors.String_To_AsciiBytes_Vector))]
        public byte[] GetBytes_From_String_To_Ascii_returnsBytesInVector(string @string)
        {
            //Arrange
            var sut = GetSut();
            //Act
            var result = sut.GetBytes(@string, Encoding.Ascii);
            //Assert
            return result;
        }
        [Test]
        [TestCaseSource(typeof(EncodingVectors), nameof(EncodingVectors.String_To_Utf16LittleEndianBytes_Vector))]
        public byte[] GetBytes_From_String_To_Utf16LittleEndian_returnsBytesInVector(string @string)
        {
            //Arrange
            var sut = GetSut();
            //Act
            var result = sut.GetBytes(@string, Encoding.Utf16LittleEndian);
            //Assert
            return result;
        }
        [Test]
        [TestCaseSource(typeof(EncodingVectors), nameof(EncodingVectors.String_To_Utf16BigEndianBytes_Vector))]
        public byte[] GetBytes_From_String_To_Utf16BigEndian_returnsBytesInVector(string @string)
        {
            //Arrange
            var sut = GetSut();
            //Act
            var result = sut.GetBytes(@string, Encoding.Utf16BigEndian);
            //Assert
            return result;
        }
        #endregion

        #region [GetBytes(char,Encoding)]
        [Test]
        [TestCaseSource(typeof(EncodingVectors), nameof(EncodingVectors.Char_To_AsciiBytes_Vector))]
        public byte[] GetBytes_From_Char_To_Ascii_returnsBytesInVector(char @char)
        {
            //Arrange
            var sut = GetSut();
            //Act
            var result = sut.GetBytes(@char, Encoding.Ascii);
            //Assert
            return result;
        }
        [Test]
        [TestCaseSource(typeof(EncodingVectors), nameof(EncodingVectors.Char_To_Utf16LittleEndianBytes_Vector))]
        public byte[] GetBytes_From_Char_To_Utf16LittleEndian_returnsBytesInVector(char @char)
        {
            //Arrange
            var sut = GetSut();
            //Act
            var result = sut.GetBytes(@char, Encoding.Utf16LittleEndian);
            //Assert
            return result;
        }
        [Test]
        [TestCaseSource(typeof(EncodingVectors), nameof(EncodingVectors.Char_To_Utf16BigEndianBytes_Vector))]
        public byte[] GetBytes_From_Char_To_Utf16BigEndian_returnsBytesInVector(char @char)
        {
            //Arrange
            var sut = GetSut();
            //Act
            var result = sut.GetBytes(@char, Encoding.Utf16BigEndian);
            //Assert
            return result;
        }
        #endregion

        #region [GetBytes(charArray,Encoding)]
        [Test]
        [TestCaseSource(typeof(EncodingVectors), nameof(EncodingVectors.String_To_AsciiBytes_Vector))]
        public byte[] GetBytes_From_CharArray_To_Ascii_returnsBytesInVector(string @string)
        {
            //Arrange
            var sut = GetSut();
            var charArray = @string.ToCharArray();
            //Act
            var result = sut.GetBytes(charArray, Encoding.Ascii);
            //Assert
            return result;
        }
        [Test]
        [TestCaseSource(typeof(EncodingVectors), nameof(EncodingVectors.String_To_Utf16LittleEndianBytes_Vector))]
        public byte[] GetBytes_From_CharArray_To_Utf16LittleEndian_returnsBytesInVector(string @string)
        {
            //Arrange
            var sut = GetSut();
            var charArray = @string.ToCharArray();
            //Act
            var result = sut.GetBytes(charArray, Encoding.Utf16LittleEndian);
            //Assert
            return result;
        }
        [Test]
        [TestCaseSource(typeof(EncodingVectors), nameof(EncodingVectors.String_To_Utf16BigEndianBytes_Vector))]
        public byte[] GetBytes_From_CharArray_To_Utf16BigEndian_returnsBytesInVector(string @string)
        {
            //Arrange
            var sut = GetSut();
            var charArray = @string.ToCharArray();
            //Act
            var result = sut.GetBytes(charArray, Encoding.Utf16BigEndian);
            //Assert
            return result;
        }
        #endregion

        #region [GetChars(byteArray,Encoding)]
        [Test]
        [TestCaseSource(typeof(EncodingVectors), nameof(EncodingVectors.AsciiBytes_To_Chars_Vector))]
        public char[] GetChars_From_AsciiBytes_To_Chars_returnsBytesInVector(byte[] asciiBytes)
        {
            //Arrange
            var sut = GetSut();
            //Act
            var result = sut.GetChars(asciiBytes, Encoding.Ascii);
            //Assert
            return result;
        }
        [Test]
        [TestCaseSource(typeof(EncodingVectors), nameof(EncodingVectors.Utf16LittleEndianBytes_To_Chars_Vector))]
        public char[] GetChars_From_Utf16LittleEndianBytes_To_Chars_returnsBytesInVector(byte[] utf16LittleEndianBytes)
        {
            //Arrange
            var sut = GetSut();
            //Act
            var result = sut.GetChars(utf16LittleEndianBytes, Encoding.Utf16LittleEndian);
            //Assert
            return result;
        }
        [Test]
        [TestCaseSource(typeof(EncodingVectors), nameof(EncodingVectors.Utf16BigEndianBytes_To_Chars_Vector))]
        public char[] GetChars_From_Utf16BigEndianByteArray_To_Chars_returnsBytesInVector(byte[] utf16BigEndianBytes)
        {
            //Arrange
            var sut = GetSut();
            //Act
            var result = sut.GetChars(utf16BigEndianBytes, Encoding.Utf16BigEndian);
            //Assert
            return result;
        }
        #endregion

        #region [GetString(byteArray, Encoding)]
        [Test]
        [TestCaseSource(typeof(EncodingVectors), nameof(EncodingVectors.AsciiBytes_To_String_Vector))]
        public string GetString_From_AsciiBytes_To_Chars_returnsBytesInVector(byte[] asciiBytes)
        {
            //Arrange
            var sut = GetSut();
            //Act
            var result = sut.GetString(asciiBytes, Encoding.Ascii);
            //Assert
            return result;
        }
        [Test]
        [TestCaseSource(typeof(EncodingVectors), nameof(EncodingVectors.Utf16LittleEndianBytes_To_String_Vector))]
        public string GetString_From_Utf16LittleEndianBytes_To_Chars_returnsBytesInVector(byte[] utf16LittleEndianBytes)
        {
            //Arrange
            var sut = GetSut();
            //Act
            var result = sut.GetString(utf16LittleEndianBytes, Encoding.Utf16LittleEndian);
            //Assert
            return result;
        }
        [Test]
        [TestCaseSource(typeof(EncodingVectors), nameof(EncodingVectors.Utf16BigEndianBytes_To_String_Vector))]
        public string GetString_From_Utf16BigEndianByteArray_To_Chars_returnsBytesInVector(byte[] utf16BigEndianBytes)
        {
            //Arrange
            var sut = GetSut();
            //Act
            var result = sut.GetString(utf16BigEndianBytes, Encoding.Utf16BigEndian);
            //Assert
            return result;
        }
        #endregion

        #region [GetSafeString(bytes, Encoding)]
        //GetString_ByteArrayEncodedWithAscii_returnsSafeStringInVector
        //GetString_SafeStringEncodedWithUtf16LittleEndian_returnsSafeStringInVector
        //GetString_SafeStringEncodedWithBigEndian_returnsSafeStringInVector
        #endregion
    }
}