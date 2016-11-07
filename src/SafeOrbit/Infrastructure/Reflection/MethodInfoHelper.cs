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

            using (var stream = File.OpenRead(methodInfo.DeclaringType.GetTypeInfo().Assembly.Location))
            {
                using (var peReader = new PEReader(stream))
                {
                    var metadataReader = peReader.GetMetadataReader();
                    var methodHandle = MetadataTokens.MethodDefinitionHandle(metadataToken);
                    var methodDef = metadataReader.GetMethodDefinition(methodHandle);
                    var methodBody = peReader.GetMethodBody(methodDef.RelativeVirtualAddress);
                    return methodBody.GetILBytes();
                }
            }
#endif
        }
    }
}