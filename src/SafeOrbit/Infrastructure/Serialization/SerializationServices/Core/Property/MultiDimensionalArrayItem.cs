namespace SafeOrbit.Infrastructure.Serialization.SerializationServices.Core
{
    /// <summary>
    ///     Information about one item in a multidimensional array
    /// </summary>
    internal sealed class MultiDimensionalArrayItem
    {
        /// <summary>
        /// </summary>
        /// <param name="indexes"></param>
        /// <param name="value"></param>
        public MultiDimensionalArrayItem(int[] indexes, Property value)
        {
            Indexes = indexes;
            Value = value;
        }

        /// <summary>
        ///     Represents item coordinates in the array (i.e. [1,5,3] - item has index 1 in the dimension 0, index 5 in the
        ///     dimension 1 and index 3 in the dimension 2).
        /// </summary>
        public int[] Indexes { get; set; }

        /// <summary>
        ///     Item value. It can contain any type.
        /// </summary>
        public Property Value { get; set; }
    }
}