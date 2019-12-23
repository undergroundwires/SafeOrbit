using System;

namespace Shared.Extensions
{
    public static class ByteArrayExtensions
    {
        public static byte[] CopyToNewArray(this byte[] byteArray)
        {
            var newArray = new byte[byteArray.Length];
            Array.Copy(byteArray, newArray, byteArray.Length);
            return newArray;
        }
    }
}