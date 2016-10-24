using System.Threading.Tasks;

namespace SafeOrbit.Encryption
{
    /// <summary>
    ///     <p>
    ///         Defines the default <see cref="IFastEncryptor{TResult,TInput,TKey}" /> using <see cref="byte" /> arrays as
    ///         the types.
    ///     </p>
    /// </summary>
    /// <seealso cref="ISafeEncryptor" />
    /// <seealso cref="IFastEncryptor{TResult,TInput,TKey}" />
    public interface IFastEncryptor : IFastEncryptor<byte[], byte[], byte[]>
    {
    }

    /// <summary>
    ///     <p>Defines a fast and cryptographically strong encryptor.</p>
    ///     <p>Use this class when you prefer the performance over security gained by slower actions.</p>
    ///     <p>
    ///         It's faster than <see cref="ISafeEncryptor{TResult,TInput,TKey, TSalt}" /> but also cryptographically strong.
    ///         It's not stronger than <see cref="ISafeEncryptor{TResult,TInput,TKey, TSalt}" /> as it takes less time to
    ///         decrypt the data and does not use salt.
    ///     </p>
    ///     <p>
    ///         For slower and cryptographically stronger operations prefer
    ///         <see cref="ISafeEncryptor{TResult,TInput,TKey, TSalt}" />
    ///     </p>
    /// </summary>
    /// <typeparam name="TResult">The type of the result of the algorithm.</typeparam>
    /// <typeparam name="TInput">The type of user input data.</typeparam>
    /// <typeparam name="TKey">The type of key.</typeparam>
    /// <seealso cref="ISafeEncryptor{TResult,TInput,TKey,TSalt}" />
    /// <seealso cref="IFastEncryptor" />
    public interface IFastEncryptor<TResult, in TInput, in TKey> : IEncryptor
    {
        /// <summary>
        ///     Encrypts the specified input with the given key and the salt.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="key">The key.</param>
        /// <returns>Encrypted <see cref="TResult" /></returns>
        /// <seealso cref="EncryptAsync" />
        TResult Encrypt(TInput input, TKey key);

        /// <summary>
        ///     Decrypts the specified input with the given key and the salt.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="key">The key.</param>
        /// <returns>Encrypted <see cref="TResult" /></returns>
        /// <seealso cref="DecryptAsync" />
        TResult Decrypt(TInput input, TKey key);

        /// <summary>
        ///     Encrypts the specified input with the given key and the salt asynchronously.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="key">The key.</param>
        /// <returns>A <see cref="Task" /> for encrypted <see cref="TResult" /></returns>
        /// <seealso cref="Encrypt" />
        Task<TResult> EncryptAsync(TInput input, TKey key);

        /// <summary>
        ///     Decrypts the specified input with the given key and the salt asynchronously.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="key">The key.</param>
        /// <returns>A <see cref="Task" /> for encrypted <see cref="TResult" /></returns>
        /// <seealso cref="Decrypt" />
        Task<TResult> DecryptAsync(TInput input, TKey key);
    }
}