namespace SafeOrbit.Core.Serialization.SerializationServices.Core
{
    /// <summary>
    ///     Represents the null value. Null values are serialized too.
    /// </summary>
    internal sealed class NullProperty : Property
    {
        ///<summary>
        ///</summary>
        public NullProperty() : base(null, null)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        public NullProperty(string name)
            : base(name, null)
        {
        }

        /// <summary>
        ///     Gets the property art.
        /// </summary>
        /// <returns></returns>
        protected override PropertyArt GetPropertyArt()
        {
            return PropertyArt.Null;
        }
    }
}