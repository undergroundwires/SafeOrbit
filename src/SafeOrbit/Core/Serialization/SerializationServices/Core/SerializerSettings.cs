namespace SafeOrbit.Core.Serialization.SerializationServices.Core
{
    /// <summary>
    ///     Base class for the settings of the SharpSerializer. Is passed to its constructor.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal abstract class SerializerSettings<T> where T : AdvancedSerializerSettings, new()
    {
        private T _advancedSettings;

        /// <summary>
        ///     IncludeAssemblyVersionInTypeName, IncludeCultureInTypeName and IncludePublicKeyTokenInTypeName are true
        /// </summary>
        protected SerializerSettings()
        {
            IncludeAssemblyVersionInTypeName = true;
            IncludeCultureInTypeName = true;
            IncludePublicKeyTokenInTypeName = true;
        }

        /// <summary>
        ///     Contains mostly classes from the namespace Polenter.Serialization.Advanced
        /// </summary>
        public T AdvancedSettings
        {
            get
            {
                if (_advancedSettings == default(T)) _advancedSettings = new T();
                return _advancedSettings;
            }
            set => _advancedSettings = value;
        }

        /// <summary>
        ///     Version=x.x.x.x will be inserted to the type name
        /// </summary>
        public bool IncludeAssemblyVersionInTypeName { get; set; }

        /// <summary>
        ///     Culture=.... will be inserted to the type name
        /// </summary>
        public bool IncludeCultureInTypeName { get; set; }

        /// <summary>
        ///     PublicKeyToken=.... will be inserted to the type name
        /// </summary>
        public bool IncludePublicKeyTokenInTypeName { get; set; }
    }
}