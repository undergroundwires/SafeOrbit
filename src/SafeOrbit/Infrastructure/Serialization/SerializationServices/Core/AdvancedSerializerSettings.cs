
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

//This is a modified version of the beautiful SharpSerializer by Pawel Idzikowski (see: http://www.sharpserializer.com)

using SafeOrbit.Infrastructure.Serialization.SerializationServices.Advanced.Serializing;

namespace SafeOrbit.Infrastructure.Serialization.SerializationServices.Core
{
    internal class AdvancedSerializerSettings
    {
        public AdvancedSerializerSettings()
        {
            RootName = "Root";
        }

        /// <summary>
        ///     What name has the root item of your serialization. Default is "Root".
        /// </summary>
        public string RootName { get; set; }

        /// <summary>
        ///     Converts Type to string and vice versa. Default is an instance of TypeNameConverter which serializes Types as "type
        ///     name, assembly name"
        ///     If you want to serialize your objects as fully qualified assembly name, you should set this setting with an
        ///     instance of TypeNameConverter
        ///     with overloaded constructor.
        /// </summary>
        public ITypeNameConverter TypeNameConverter { get; set; }
    }
}