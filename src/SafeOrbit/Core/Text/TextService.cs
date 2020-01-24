using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SafeOrbit.Library;
using SafeOrbit.Memory;

namespace SafeOrbit.Text
{
    /// <summary>
    ///     <see cref="TextService" /> is a wrapper around <see cref="System.Text.Encoding" />.
    /// </summary>
    /// <remarks>
    ///     <p><see cref="TextService" /> is a stateless service.</p>
    /// </remarks>
    /// <seealso cref="ITextService" />
    /// <seealso cref="System.Text.Encoding" />
    public class TextService : ITextService
    {
        /// <summary>
        ///     The default encoding.
        /// </summary>
        private const Encoding DefaultEncoding = Encoding.Utf16LittleEndian;

        /// <summary>
        ///     Safe string factory to create <see cref="ISafeString" /> instances when using <see cref="GetSafeStringAsync" /> method.
        /// </summary>
        /// <seealso cref="GetSafeStringAsync" />
        /// <seealso cref="ITextService" />
        private readonly IFactory<ISafeString> _safeStringFactory;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TextService" /> class.
        /// </summary>
        public TextService() : this(SafeOrbitCore.Current.Factory.Get<IFactory<ISafeString>>())
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TextService" /> class.
        /// </summary>
        /// <param name="safeStringFactory">The safe string factory.</param>
        /// <exception cref="ArgumentNullException"><paramref name="safeStringFactory" /> is <see langword="null" /></exception>
        internal TextService(IFactory<ISafeString> safeStringFactory)
        {
            _safeStringFactory = safeStringFactory ?? throw new ArgumentNullException(nameof(safeStringFactory));
        }
        
        /// <inheritdoc />
        /// <returns>
        ///     An array of type <see cref="byte" /> containing the results of converting <paramref name="bytes" /> from
        ///     <paramref name="sourceEncoding" /> to <paramref name="destinationEncoding" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes" /> is <see langword="null" /> or empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <p><paramref name="sourceEncoding" /> must be defined in <seealso cref="Encoding" /> enum.</p>
        ///     <p><paramref name="destinationEncoding" /> must be defined in <seealso cref="Encoding" /> enum.</p>
        /// </exception>
        /// <exception cref="System.Text.DecoderFallbackException">
        ///     A fallback occurred (see https://msdn.microsoft.com/en-us/library/ms404377(v=vs.110).aspx , Character Encoding in
        ///     the .NET Framework)-and-<see cref="P:System.Text.Encoding.DecoderFallback" /> is set to
        ///     <see cref="T:System.Text.DecoderExceptionFallback" />.
        /// </exception>
        /// <exception cref="System.Text.EncoderFallbackException">
        ///     A fallback occurred (see https://msdn.microsoft.com/en-us/library/ms404377(v=vs.110).aspx , Character Encoding in
        ///     the .NET Framework)-and-<see cref="P:System.Text.Encoding.EncoderFallback" /> is set to
        ///     <see cref="T:System.Text.EncoderExceptionFallback" />.
        /// </exception>
        public byte[] Convert(Encoding sourceEncoding, Encoding destinationEncoding, byte[] bytes)
        {
            if (bytes == null || !bytes.Any()) throw new ArgumentNullException(nameof(bytes));
            return System.Text.Encoding.Convert(
                GetEncodingObject(sourceEncoding),
                GetEncodingObject(destinationEncoding),
                bytes);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="text" /> is <see langword="null" /> or empty.</exception>
        /// <exception cref="System.Text.EncoderFallbackException">
        ///     A fallback occurred (see https://msdn.microsoft.com/en-us/library/ms404377(v=vs.110).aspx , Character Encoding in
        ///     the .NET Framework)-and-<see cref="P:System.Text.Encoding.EncoderFallback" /> is set to
        ///     <see cref="T:System.Text.EncoderExceptionFallback" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="encoding" /> must be defined in
        ///     <seealso cref="Encoding" /> enum.
        /// </exception>
        public byte[] GetBytes(string text, Encoding encoding = DefaultEncoding)
        {
            if (string.IsNullOrEmpty(text)) throw new ArgumentNullException(nameof(text));
            return GetEncodingObject(encoding).GetBytes(text);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Throws if
        ///     <paramref name="char" />
        ///     is less than zero.
        /// </exception>
        /// <exception cref="System.Text.EncoderFallbackException">
        ///     A fallback occurred (see https://msdn.microsoft.com/en-us/library/ms404377(v=vs.110).aspx , Character Encoding in
        ///     the .NET Framework)-and-<see cref="P:System.Text.Encoding.EncoderFallback" /> is set to
        ///     <see cref="T:System.Text.EncoderExceptionFallback" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="char" />
        ///     is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="encoding" /> must be defined in
        ///     <seealso cref="Encoding" /> enum.
        /// </exception>
        public byte[] GetBytes(char @char, Encoding encoding = DefaultEncoding)
        {
            return GetBytes(new[] {@char}, encoding);
        }
        
        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="chars" /> is <see langword="null" />.</exception>
        /// <exception cref="System.Text.EncoderFallbackException">
        ///     A fallback occurred (see https://msdn.microsoft.com/en-us/library/ms404377(v=vs.110).aspx , Character Encoding in
        ///     the .NET Framework)-and-<see cref="P:System.Text.Encoding.EncoderFallback" /> is set to
        ///     <see cref="T:System.Text.EncoderExceptionFallback" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">Throws if encoding type is not valid.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="encoding" /> must be defined in
        ///     <seealso cref="Encoding" /> enum.
        /// </exception>
        public byte[] GetBytes(char[] chars, Encoding encoding = DefaultEncoding)
        {
            if (chars == null || !chars.Any()) throw new ArgumentNullException(nameof(chars));
            return GetEncodingObject(encoding).GetBytes(chars);
        }


        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="bytes" /> is <see langword="null" /> or empty.</exception>
        /// <exception cref="ArgumentException">The byte array contains invalid encoding code points.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="encoding" /> must be defined in
        ///     <seealso cref="Encoding" /> enum.
        /// </exception>
        /// <exception cref="System.Text.DecoderFallbackException">
        ///     A fallback occurred (see https://msdn.microsoft.com/en-us/library/ms404377(v=vs.110).aspx , Character Encoding in
        ///     the .NET Framework)-and-<see cref="P:System.Text.Encoding.DecoderFallback" /> is set to
        ///     <see cref="T:System.Text.DecoderExceptionFallback" />.
        /// </exception>
        public string GetString(byte[] bytes, Encoding encoding = DefaultEncoding)
        {
            if (bytes == null || !bytes.Any()) throw new ArgumentNullException(nameof(bytes));
            return GetEncodingObject(encoding).GetString(bytes);
        }


        /// <inheritdoc />
        /// <returns>A character array containing the decoded representation of a given byte array..</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes" /> is null or empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="encoding" /> must be defined in
        ///     <seealso cref="Encoding" /> enum.
        /// </exception>
        /// <exception cref="System.Text.DecoderFallbackException">
        ///     A fallback occurred (see https://msdn.microsoft.com/en-us/library/ms404377(v=vs.110).aspx , Character Encoding in
        ///     the .NET Framework)-and-<see cref="P:System.Text.Encoding.DecoderFallback" /> is set to
        ///     <see cref="T:System.Text.DecoderExceptionFallback" />.
        /// </exception>
        public char[] GetChars(byte[] bytes, Encoding encoding)
        {
            if (bytes == null || !bytes.Any()) throw new ArgumentNullException(nameof(bytes));
            return GetEncodingObject(encoding).GetChars(bytes);
        }


        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="bytes" /> is null or empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="encoding" /> must be defined in
        ///     <seealso cref="Encoding" /> enum.
        /// </exception>
        /// <exception cref="System.Text.DecoderFallbackException">
        ///     A fallback occurred (see https://msdn.microsoft.com/en-us/library/ms404377(v=vs.110).aspx , Character Encoding in
        ///     the .NET Framework)-and-<see cref="System.Text.Encoding.DecoderFallback" /> is set to
        ///     <see cref="System.Text.DecoderExceptionFallback" />.
        /// </exception>
        /// <seealso cref="_safeStringFactory" />
        public async Task<ISafeString> GetSafeStringAsync(byte[] bytes, Encoding encoding)
        {
            if (bytes == null || !bytes.Any()) throw new ArgumentNullException(nameof(bytes));
            var result = _safeStringFactory.Create();
            using (var stream = new MemoryStream(bytes))
            {
                using (var streamReader = new StreamReader(stream, GetEncodingObject(encoding)))
                {
                    do
                    {
                        var ch = (char)streamReader.Read();
                        await result.AppendAsync(ch).ConfigureAwait(false);
                    } while (!streamReader.EndOfStream);
                }
            }
            return result;
        }

        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="encoding" /> must be defined in
        ///     <seealso cref="Encoding" /> enum.
        /// </exception>
        private System.Text.Encoding GetEncodingObject(Encoding encoding)
        {
            if (!Enum.IsDefined(typeof(Encoding), encoding)) throw new ArgumentOutOfRangeException(nameof(encoding));
            return encoding switch
            {
                Encoding.Ascii => System.Text.Encoding.ASCII,
                Encoding.Utf16BigEndian => System.Text.Encoding.BigEndianUnicode,
                Encoding.Utf16LittleEndian => System.Text.Encoding.Unicode,
                _ => throw new ArgumentOutOfRangeException(nameof(encoding),
                    $"Value must be defined in the {nameof(Encoding)} enum.")
            };
        }
    }
}