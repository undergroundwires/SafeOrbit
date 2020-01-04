namespace SafeOrbit.Core.Serialization.SerializationServices.Core
{
    /// <summary>
    ///     Of what art is the property
    /// </summary>
    internal enum PropertyArt
    {
        ///<summary>
        ///</summary>
        Unknown = 0,

        ///<summary>
        ///</summary>
        Simple,

        ///<summary>
        ///</summary>
        Complex,

        ///<summary>
        ///</summary>
        Collection,

        ///<summary>
        ///</summary>
        Dictionary,

        ///<summary>
        ///</summary>
        SingleDimensionalArray,

        ///<summary>
        ///</summary>
        MultiDimensionalArray,

        ///<summary>
        ///</summary>
        Null,

        ///<summary>
        ///</summary>
        Reference
    }
}