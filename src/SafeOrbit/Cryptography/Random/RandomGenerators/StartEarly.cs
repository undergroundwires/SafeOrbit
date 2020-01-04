using System.Runtime.CompilerServices;

namespace SafeOrbit.Cryptography.Random.RandomGenerators
{
    /// <summary>
    ///     Starts all entropy sources filling their respective entropy pools, to reduce wait time when you actually call them.
    ///     It is recommended to use StartEarly.StartFillingEntropyPools(); at the soonest entry point to your application, for
    ///     example the
    ///     first line of Main()
    /// </summary>
    public static class StartEarly
    {
        /// <summary>
        ///     Starts all entropy sources filling their respective entropy pools, to reduce wait time when you actually call them.
        ///     It is recommended to use StartEarly.StartFillingEntropyPools(); at the soonest entry point to your application, for
        ///     example the
        ///     first line of Main()
        /// </summary>
        public static void StartFillingEntropyPools()
        {
            InvokeStaticConstructorFor<FastRandomGenerator>();
            InvokeStaticConstructorFor<ThreadedSeedGeneratorRng>();
            InvokeStaticConstructorFor<ThreadSchedulerRng>();
            // Initializing FastRandomGenerator, initializes also SafeRandomGenerator
            var junk = FastRandomGenerator.StaticInstance.ToString();
        }

        private static void InvokeStaticConstructorFor<T>()
            => RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);
    }
}