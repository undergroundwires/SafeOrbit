namespace SafeOrbit.Threading
{
    /// <summary>
    ///     Helper class for <see cref="System.Threading.Interlocked" />.
    ///     <see cref="System.Threading.Interlocked" /> can not handle bools. So ints are used as bools
    /// </summary>
    internal static class IntCondition
    {
        public const int True = 1;
        public const int False = 0;
    }
}