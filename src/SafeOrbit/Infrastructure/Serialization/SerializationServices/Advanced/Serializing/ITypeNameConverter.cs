
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
using System;

namespace SafeOrbit.Infrastructure.Serialization.SerializationServices.Advanced.Serializing
{
    /// <summary>
    ///   Converts Type to its string representation and vice versa. The default instance used in the Framework is TypeNameConverter
    /// </summary>
    internal interface ITypeNameConverter
    {
        /// <summary>
        ///   Gives back Type as text.
        /// </summary>
        /// <param name = "type"></param>
        /// <returns>string.Empty if the type is null</returns>
        string ConvertToTypeName(Type type);

        /// <summary>
        ///   Gives back Type from the text.
        /// </summary>
        /// <param name = "typeName"></param>
        /// <returns></returns>
        Type ConvertToType(string typeName);
    }
}