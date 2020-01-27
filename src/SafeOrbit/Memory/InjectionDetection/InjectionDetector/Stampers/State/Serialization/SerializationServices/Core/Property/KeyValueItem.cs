namespace SafeOrbit.Memory.InjectionServices.Stampers.Serialization.SerializationServices.Core.Property
{
    /// <summary>
    ///     Represents one item from the dictionary, a key-value pair.
    /// </summary>
    internal sealed class KeyValueItem
    {
        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public KeyValueItem(Property key, Property value)
        {
            Key = key;
            Value = value;
        }

        /// <summary>
        ///     Represents key. There can be everything
        /// </summary>
        public Property Key { get; set; }

        /// <summary>
        ///     Represents value. There can be everything
        /// </summary>
        public Property Value { get; set; }
    }
}