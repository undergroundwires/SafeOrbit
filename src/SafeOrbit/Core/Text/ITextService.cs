//using SafeOrbit.Memory;

using SafeOrbit.Memory;

namespace SafeOrbit.Text
{
    /// <summary>
    ///     Defines conversation methods for texts.
    /// </summary>
    /// <seealso cref="Encoding" />
    public interface ITextService
    {
        /// <summary>
        ///     Converts an entire byte array from one encoding to another.
        /// </summary>
        /// <param name="sourceEncoding">The encoding format of <paramref name="bytes" /></param>
        /// <param name="destinationEncoding">The target encoding format.</param>
        /// <param name="bytes">The bytes to convert.</param>
        /// <returns>
        ///     An array of type <see cref="byte" /> containing the results of converting <paramref name="bytes" /> from
        ///     <paramref name="sourceEncoding" /> to <paramref name="destinationEncoding" />.
        /// </returns>
        byte[] Convert(Encoding sourceEncoding, Encoding destinationEncoding, byte[] bytes);

        /// <summary>
        ///     Decodes all the bytes in the specified byte array into a <see cref="ISafeString" />.
        /// </summary>
        /// <param name="bytes">The byte array containing the sequence of bytes to decode.</param>
        /// <param name="encoding">The encoding of the characters in the given byte array.</param>
        /// <returns><see cref="ISafeString" /> instance containing the decoded representation of a given byte array.</returns>
        ISafeString GetSafeString(byte[] bytes, Encoding encoding = Encoding.Utf16LittleEndian);

        /// <summary>
        ///     Decodes all the bytes in the specified byte array into a set of characters.
        /// </summary>
        /// <param name="bytes">The byte array containing the sequence of bytes to decode.</param>
        /// <param name="encoding">The encoding of the characters in the given byte array.</param>
        /// <returns>A character array containing the decoded representation of a given byte array..</returns>
        char[] GetChars(byte[] bytes, Encoding encoding);

        /// <summary>
        ///     Decodes a sequence of bytes into a string.
        /// </summary>
        /// <param name="bytes">The byte array containing the sequence of bytes to decode.</param>
        /// <param name="encoding">The encoding of the characters in the given byte array.</param>
        /// <returns>A string that contains the results of decoding the specified sequence of bytes.</returns>
        string GetString(byte[] bytes, Encoding encoding = Encoding.Utf16LittleEndian);

        /// <summary>
        ///     Encodes all the characters in the specified string into a sequence of bytes.
        /// </summary>
        /// <param name="text">The string containing the characters to encode.</param>
        /// <param name="encoding">The encoding of the given text.</param>
        /// <returns>A byte array containing the results of encoding the specified set of characters.</returns>
        byte[] GetBytes(string text, Encoding encoding = Encoding.Utf16LittleEndian);

        /// <summary>
        ///     Encodes the specified character into a sequence of bytes.
        /// </summary>
        /// <param name="char">The character to encode.</param>
        /// <param name="encoding">The encoding of the given character.</param>
        /// <returns>A byte array containing the results of encoding the given character.</returns>
        byte[] GetBytes(char @char, Encoding encoding);

        /// <summary>
        ///     Encodes the characters in the specified character array into a sequence of bytes.
        /// </summary>
        /// <param name="chars">The character array containing the characters to encode.</param>
        /// <param name="encoding">The encoding of the given characters.</param>
        /// <returns>A byte array containing the encoded representation of the given character array.</returns>
        byte[] GetBytes(char[] chars, Encoding encoding);
    }
}