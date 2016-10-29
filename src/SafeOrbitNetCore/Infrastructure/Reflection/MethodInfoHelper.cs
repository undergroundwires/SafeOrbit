using System.IO;
using System.Reflection;

#if NETCORE
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
#endif

namespace SafeOrbit.Common.Reflection
{
    public static class MethodInfoExtensions
    {
        /// <remarks>
        /// Based on https://gist.github.com/nguerrera/72444715c7ea0b40addb
        /// </remarks>
        public static byte[] GetIlBytes(this MethodInfo methodInfo)
        {
#if !NETCORE
            return methodInfo.GetMethodBody().GetILAsByteArray();
#else
            var metadataToken = methodInfo.GetMetadataToken();

            using (var stream = File.OpenRead(methodInfo.DeclaringType.GetTypeInfo().Assembly.Location))
            using (var peReader = new PEReader(stream))
            {
                var metadataReader = peReader.GetMetadataReader();
                var methodHandle = MetadataTokens.MethodDefinitionHandle(metadataToken);
                var methodDef = metadataReader.GetMethodDefinition(methodHandle);
                var methodBody = peReader.GetMethodBody(methodDef.RelativeVirtualAddress);
                return methodBody.GetILBytes();
            }
#endif
        }
    }
}