namespace SafeOrbit.Cryptography.Encryption.Padding
{
    /// <summary>
    ///     An encryption algorithm with padding support.
    /// </summary>
    public interface IPaddedEncryptor : IEncryptor
    {
        PaddingMode Padding { get; set; }
    }
}
