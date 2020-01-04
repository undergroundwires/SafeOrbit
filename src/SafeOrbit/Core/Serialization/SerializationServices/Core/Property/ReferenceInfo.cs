namespace SafeOrbit.Core.Serialization.SerializationServices.Core
{
    /// <summary>
    ///     Provides information about property references
    /// </summary>
    internal sealed class ReferenceInfo
    {
        ///<summary>
        ///</summary>
        public ReferenceInfo()
        {
            Count = 1;
        }

        /// <summary>
        ///     How many references to the same object
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        ///     Every Object must have a unique Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     During serialization is true if the target object was already serialized.
        ///     Then the target must not be serialized again. Only its reference must be created.
        ///     During deserialization it means, the target object was parsed and read
        ///     from the stream. It can be further used to resolve its references.
        /// </summary>
        public bool IsProcessed { get; set; }

        ///<summary>
        ///</summary>
        ///<returns></returns>
        public override string ToString()
        {
            return $"{GetType().Name}, Count={Count}, Id={Id}, IsProcessed={IsProcessed}";
        }
    }
}