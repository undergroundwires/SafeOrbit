
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

using System;
using System.IO;
using System.Reflection;
#if NETCORE
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

#endif

namespace SafeOrbit.Infrastructure.Reflection
{
    public static class MethodInfoHelper
    {
        /// <summary>
        ///     Gets the IL-code as bytes for the specified <see cref="MethodInfo" />.
        /// </summary>
        /// <param name="methodInfo">The method information of the IL-code.</param>
        /// <returns>IL-code as bytes</returns>
        /// <exception cref="ArgumentNullException"><paramref name="methodInfo" /> is <see langword="null" /></exception>
        public static byte[] GetIlBytes(this MethodInfo methodInfo)
        {
            if (methodInfo == null) throw new ArgumentNullException(nameof(methodInfo));
#if !NETCORE
            var methodBody = methodInfo.GetMethodBody();
            var result =  methodBody?.GetILAsByteArray();
            return result;
#else
            //.NET Core implementation is based on https://gist.github.com/nguerrera/72444715c7ea0b40addb
            var metadataToken = methodInfo.GetMetadataToken();
            var assemblyLocation = methodInfo.DeclaringType.GetTypeInfo().Assembly.Location;
            using (var stream = File.OpenRead(assemblyLocation))
            using (var peReader = new PEReader(stream))
            {
                var metadataReader = peReader.GetMetadataReader();
                var methodHandle = MetadataTokens.MethodDefinitionHandle(metadataToken);
                var methodDef = metadataReader.GetMethodDefinition(methodHandle);
                var virtualAddress = methodDef.RelativeVirtualAddress;
                if (virtualAddress == 0) return null; //method not found
                var methodBody = peReader.GetMethodBody(methodDef.RelativeVirtualAddress);
                return methodBody.GetILBytes();
            }
#endif
        }
    }
}