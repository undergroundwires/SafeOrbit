using System.Threading.Tasks;

namespace SafeOrbit.Cryptography.Encryption
{
    /// <summary>
    ///     <p>
    ///         Defines the default <see cref="ISafeEncryptor{TResult,TInput,TKey,TSalt}" /> using <see cref="byte" /> arrays as
    ///         the types.
    ///     </p>
    /// </summary>
    /// <seealso cref="ISafeEncryptor" />
    /// <seealso cref="IFastEncryptor{TResult,TInput,TKey}" />
    public interface ISafeEncryptor : ISafeEncryptor<byte[], byte[], byte[], byte[]>
    {
    }

    /// <summary>
    ///     <p>Defines a slow and cryptographically stronger encryptor.</p>
    ///     <p>Use this class when you prefer the security over performance. </p>
    ///     <p>
    ///         It's stronger than <see cref="IFastEncryptor{TResult,TInput,TKey}" /> because it takes more to decrypt and
    ///         it uses an additional <typeparamref name="TSalt" />.
    ///     </p>
    ///     <p>
    ///         For faster and cryptographically strong operations prefer
    ///         <see cref="IFastEncryptor{TResult,TInput,TKey}" />
    ///     </p>
    /// </summary>
    /// <typeparam name="TResult">The type of the result of the algorithm.</typeparam>
    /// <typeparam name="TInput">The type of user input data.</typeparam>
    /// <typeparam name="TKey">The type of key.</typeparam>
    /// <typeparam name="TSalt">The type of the salt.</typeparam>
    /// <seealso cref="IFastEncryptor{TResult,TInput,TKey}" />
    /// <seealso cref="ISafeEncryptor" />
    public interface ISafeEncryptor<TResult, in TInput, in TKey, in TSalt>  : IEncryptor
    {
        /// <summary>
        ///     Encrypts the specified input with the given key and the salt.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="key">The key.</param>
        /// <param name="salt">The salt.</param>
        /// <returns>Encrypted <typeparamref name="TResult" /></returns>
        /// <seealso cref="EncryptAsync"/>
        TResult Encrypt(TInput input, TKey key, TSalt salt);

        /// <summary>
        ///     Decrypts the specified input with the given key and the salt.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="key">The key.</param>
        /// <param name="salt">The salt.</param>
        /// <returns>Encrypted <typeparamref name="TResult" /></returns>
        /// <seealso cref="DecryptAsync"/>
        TResult Decrypt(TInput input, TKey key, TSalt salt);

        /// <summary>
        ///     Encrypts the specified input with the given key and the salt asynchronously.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="key">The key.</param>
        /// <param name="salt">The salt.</param>
        /// <returns>A <see cref="Task"/> for encrypted <typeparamref name="TResult" /></returns>
        /// <seealso cref="Encrypt"/>
        Task<TResult> EncryptAsync(TInput input, TKey key, TSalt salt);

        /// <summary>
        ///     Decrypts the specified input with the given key and the salt asynchronously.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="key">The key.</param>
        /// <param name="salt">The salt.</param>
        /// <returns>A <see cref="Task"/> for encrypted <typeparamref name="TResult" /></returns>
        /// <seealso cref="Decrypt"/>
        Task<TResult> DecryptAsync(TInput input, TKey key, TSalt salt);
    }
}