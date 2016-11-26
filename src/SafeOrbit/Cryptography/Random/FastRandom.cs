using System.Security.Cryptography;
using SafeOrbit.Cryptography.Random.RandomGenerators;
using SafeOrbit.Random;

namespace SafeOrbit.Cryptography.Random
{
    /// <summary>
    ///     <p>
    ///         <see cref="FastRandom" /> returns cryptographically strong random data. It's about 1000x times faster than
    ///         <see cref="SafeRandom" />
    ///     </p>
    ///     <p>
    ///         For general purposes, <see cref="FastRandom" /> is recommended because of its performance
    ///         characteristics, but for extremely strong keys and other things that don't require a large number of bytes
    ///         quickly,  <see cref="SafeRandom" /> is recommended instead.
    ///     </p>
    /// </summary>
    /// <remarks>
    ///     <p>Wrapper for <see cref="FastRandomGenerator" /></p>
    /// </remarks>
    /// <seealso cref="SafeRandom" />
    /// <seealso cref="RandomNumberGenerator" />
    /// <seealso cref="FastRandomGenerator" />
    public class FastRandom : RandomBase, IFastRandom
    {
        /// <summary>
        ///     Gets the static instance of <see cref="FastRandom"/>.
        /// </summary>
        public static IFastRandom StaticInstance = new FastRandom();
        /// <summary>
        ///     Initializes a new instance of the <see cref="FastRandom" /> class.
        /// </summary>
        public FastRandom() : base(FastRandomGenerator.StaticInstance)
        {
        }
    }
}