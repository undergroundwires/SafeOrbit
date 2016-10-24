using System;

namespace SafeOrbit.Memory.Serialization.SerializationServices.Core
{
    /// <summary>
    ///     Represents all primitive types (i.e. int, double...) and additionally
    ///     DateTime, TimeSpan, Decimal und enumerations
    ///     Contains no nested properties
    /// </summary>
    /// <remarks>
    ///     See SimpleValueConverter for a list of supported types.
    /// </remarks>
    internal sealed class SimpleProperty : Property
    {
        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public SimpleProperty(string name, Type type)
            : base(name, type)
        {
        }

        /// <summary>
        ///     It could only one of the simple types, see Tools.IsSimple(...)
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        ///     Gets the property art.
        /// </summary>
        /// <returns></returns>
        protected override PropertyArt GetPropertyArt()
        {
            return PropertyArt.Simple;
        }

        ///<summary>
        ///</summary>
        ///<returns></returns>
        public override string ToString()
        {
            var text = base.ToString();
            return Value != null
                ? $"{text}, ({Value})"
                : $"{text}, (null)";
        }
    }
}