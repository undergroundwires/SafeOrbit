using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace SafeOrbit.Memory.SafeBytesServices.Collection
{
    public class ByteIdListSerializer : IByteIdListSerializer<int>
    {
        /// <summary>
        ///     Serializes list of id's in list of 32 bit unsigned integers. The first value is the length of the array.
        /// </summary>
        public async Task<byte[]> SerializeAsync(IReadOnlyCollection<int> list)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (!list.Any()) return new byte[0];
            var resultSize = sizeof(int) * list.Count; /* size + list bytes */
            ;
            using var memoryStream = new MemoryStream(capacity: resultSize);
            var buffer = new byte[sizeof(int)];
            await WriteInt32Async(list.Count, buffer, memoryStream).ConfigureAwait(false); /* Write the size */
            foreach (var i in list)
                await WriteInt32Async(i, buffer, memoryStream).ConfigureAwait(false);
            Array.Clear(buffer, 0, sizeof(int));
            return memoryStream.ToArray();
        }

        public async Task<IEnumerable<int>> DeserializeAsync(byte[] list)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (list.Length == 0) return Enumerable.Empty<int>();
            using var stream = new MemoryStream(list);
            /* First byte tells the length of the list */
            const int lengthBytesSize = sizeof(int);
            var lengthBytes = await ReadNextBytesAsync(stream, lengthBytesSize).ConfigureAwait(false);
            var length = BitConverter.ToInt32(lengthBytes, 0);
            if (length < 0) ThrowCorrupted();
            /* Retrieve the list bytes */
            var listBytesSize = length * sizeof(int);
            var listBytes = await ReadNextBytesAsync(stream, listBytesSize).ConfigureAwait(false);
            var bytesAsList = BytesToIntList(listBytes);
            return bytesAsList;
        }

        private static async Task<byte[]> ReadNextBytesAsync(Stream stream, int count)
        {
            var buffer = new byte[count];
            var readBytes = await stream.ReadAsync(buffer, 0, count).ConfigureAwait(false);
            if (readBytes != count)
                ThrowCorrupted();
            return buffer;
        }

        private static void ThrowCorrupted() =>
            throw new SerializationException("Serialized bytes are corrupted in memory");

        private static IEnumerable<int> BytesToIntList(byte[] buffer)
        {
            var size = buffer.Length / sizeof(int);
            var integerList = new int[size];
            for (var index = 0; index < size; index++)
                integerList[index] = BitConverter.ToInt32(buffer, index * sizeof(int));
            return integerList;
        }

        private static Task WriteInt32Async(int value, byte[] buffer, Stream outStream)
        {
            unsafe
            {
                fixed (byte* numPtr = buffer)
                {
                    *(int*) numPtr = value;
                }
            }

            return outStream.WriteAsync(buffer, 0, sizeof(int));
        }
    }
}