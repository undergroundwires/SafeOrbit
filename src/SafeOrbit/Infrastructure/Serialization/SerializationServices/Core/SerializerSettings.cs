
/*
MIT License

Copyright (c) 2018 Erkin Ekici - undergroundwires@safeorb.it

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

namespace SafeOrbit.Infrastructure.Serialization.SerializationServices.Core
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