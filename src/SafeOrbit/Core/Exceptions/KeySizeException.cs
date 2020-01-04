using SafeOrbit.Exceptions.SerializableException;

#if !NETSTANDARD1_6
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
#endif

namespace SafeOrbit.Exceptions
{
    /// <summary>
    /// This type of exception is thrown if the key size was given wrong to complete the cryptologic operation.
    /// </summary>
#if !NETSTANDARD1_6
    [Serializable]
#endif
    public class KeySizeException : SafeOrbitException
    {
        public int MinSize { get; set; }
        public int MaxSize { get; set; }

        public KeySizeException(int actual, int minSize, int maxSize) : base(
            $"The length of the key parameter for the cryptographic function must be between {minSize} bits ({minSize / 8} bytes) and {maxSize} bits (({maxSize / 8} bytes) but it was {actual * 8} bits ({actual} bytes)")
        {
            MinSize = minSize;
            MaxSize = maxSize;
        }

#if !NETSTANDARD1_6
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public KeySizeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        protected override void ConfigureSerialize(ISerializationContext serializationContext)
        {
            serializationContext.Add(() => MinSize);
            serializationContext.Add(() => MaxSize);
            base.ConfigureSerialize(serializationContext);
        }
#endif
    }
}