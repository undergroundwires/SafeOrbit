namespace SafeOrbit.Core.Serialization.SerializationServices.Core
{
    /// <summary>
    ///     Every array is composed of dimensions. Single dimensional arrays have only one info,
    ///     multidimensional have more dimension infos.
    /// </summary>
    internal sealed class DimensionInfo
    {
        /// <summary>
        ///     Start index for the array
        /// </summary>
        public int LowerBound { get; set; }

        /// <summary>
        ///     How many items are in this dimension
        /// </summary>
        public int Length { get; set; }
    }
}