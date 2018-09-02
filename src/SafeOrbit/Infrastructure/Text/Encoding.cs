namespace SafeOrbit.Text
{
    /// <summary>
    ///     Supported character encodings
    /// </summary>
    public enum Encoding
    {
        /// <summary>
        ///     ASCII stands for American Standard Code for Information Interchange. Computers can only understand numbers, so an
        ///     ASCII code is the numerical representation of a character such as 'a' or '@' or an action of some sort.
        /// </summary>
        Ascii,

        /// <summary>
        ///     16-bit Unicode Transformation Format with big endian byte order.
        /// </summary>
        Utf16BigEndian,

        /// <summary>
        ///     16-bit Unicode Transformation Format with little endian byte order.
        /// </summary>
        Utf16LittleEndian
    }
}