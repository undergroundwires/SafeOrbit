using System.Security.Cryptography;
using SafeOrbit.Cryptography.Random.RandomGenerators;

namespace SafeOrbit.Cryptography.Random
{
    /// <summary>
    ///     <p>
    ///         <see cref="SafeRandom" /> returns cryptographically strong random data, never to exceed the number of
    ///         bytes available from the specified entropy sources.  This can cause slow generation, and is recommended only
    ///         for generating extremely strong keys and other things that don't require a large number of bytes quickly.  This is
    ///         CPU intensive. For general purposes, see <see cref="FastRandom" /> instead.
    ///     </p>
    /// </summary>
    /// <remarks>
    ///     <p>Wrapper for <see cref="SafeRandomGenerator" /></p>
    /// </remarks>
    /// <seealso cref="FastRandom" />
    /// <seealso cref="RandomNumberGenerator" />
    /// <seealso cref="SafeRandomGenerator" />
    public class SafeRandom : RandomBase, ISafeRandom
    {
        /// <summary>
        ///     Gets the static instance of <see cref="SafeRandom"/>.
        /// </summary>
        public static ISafeRandom StaticInstance = new SafeRandom();
        /// <summary>
        ///     Initializes a new instance of the <see cref="FastRandom" /> class.
        /// </summary>
        public SafeRandom() : base(SafeRandomGenerator.StaticInstance)
        {
        }
    }
}