//This is a modified version of the beautiful SharpSerializer by Pawel Idzikowski (see: http://www.sharpserializer.com)

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SafeOrbitNetCore.Core.Serialization.SharpSerializer.Common;

namespace SafeOrbit.Core.Serialization.SerializationServices.Core.Binary
{
    /// <summary>
    ///     Some methods which are used by IBinaryReader
    /// </summary>
    internal static class BinaryReaderTools
    {
        /// <summary>
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static string ReadString(BinaryReader reader)
        {
            if (!reader.ReadBoolean()) return null;
            return reader.ReadString();
        }

        /// <summary>
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static int ReadNumber(BinaryReader reader)
        {
            // Size
            var size = reader.ReadByte();

            // Number
            return size switch
            {
                NumberSize.Zero => 0,
                NumberSize.B1 => reader.ReadByte(),
                NumberSize.B2 => reader.ReadInt16(),
                _ => reader.ReadInt32()
            };
        }

        /// <summary>
        /// </summary>
        /// <returns>empty array if there are no indexes</returns>
        public static int[] ReadNumbers(BinaryReader reader)
        {
            // Count
            var count = ReadNumber(reader);

            if (count == 0) return new int[0];

            // Items
            var result = new List<int>();
            for (var i = 0; i < count; i++) result.Add(ReadNumber(reader));
            return result.ToArray();
        }

        /// <summary>
        /// </summary>
        /// <param name="expectedType"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static object ReadValue(Type expectedType, BinaryReader reader)
        {
            if (!reader.ReadBoolean()) return null;
            return ReadValueCore(expectedType, reader);
        }

        private static object ReadValueCore(Type type, BinaryReader reader)
        {
            try
            {
                if (type == typeof(byte[])) return ReadArrayOfByte(reader);
                if (type == typeof(string)) return reader.ReadString();
                if (type == typeof(bool)) return reader.ReadBoolean();
                if (type == typeof(byte)) return reader.ReadByte();
                if (type == typeof(char)) return reader.ReadChar();
                if (type == typeof(DateTime)) return new DateTime(reader.ReadInt64());
                if (type == typeof(Guid)) return new Guid(reader.ReadBytes(16));
#if DEBUG || PORTABLE || SILVERLIGHT
                if (type == typeof(decimal)) return ReadDecimal(reader);
#else
                if (type == typeof (Decimal)) return reader.ReadDecimal();
#endif
                if (type == typeof(double)) return reader.ReadDouble();
                if (type == typeof(short)) return reader.ReadInt16();
                if (type == typeof(int)) return reader.ReadInt32();
                if (type == typeof(long)) return reader.ReadInt64();
                if (type == typeof(sbyte)) return reader.ReadSByte();
                if (type == typeof(float)) return reader.ReadSingle();
                if (type == typeof(ushort)) return reader.ReadUInt16();
                if (type == typeof(uint)) return reader.ReadUInt32();
                if (type == typeof(ulong)) return reader.ReadUInt64();

                if (type == typeof(TimeSpan)) return new TimeSpan(reader.ReadInt64());

                // Enumeration
                if (type.GetTypeInfo().IsEnum) return ReadEnumeration(type, reader);

                // Type
                if (ReflectionHelper.IsType(type))
                {
                    var typeName = reader.ReadString();
                    return Type.GetType(typeName, true);
                }

                throw new InvalidOperationException($"Unknown simple type: {type.FullName}");
            }
            catch (Exception ex)
            {
                throw new SimpleValueParsingException(
                    $"Invalid type: {type}. See details in the inner exception.", ex);
            }
        }

        private static object ReadDecimal(BinaryReader reader)
        {
            var bits = new int[4];
            bits[0] = reader.ReadInt32();
            bits[1] = reader.ReadInt32();
            bits[2] = reader.ReadInt32();
            bits[3] = reader.ReadInt32();
            return new decimal(bits);
        }


        private static object ReadEnumeration(Type expectedType, BinaryReader reader)
        {
            // read the enum as int
            var value = reader.ReadInt32();
            var result = Enum.ToObject(expectedType, value);
            return result;
        }

        private static byte[] ReadArrayOfByte(BinaryReader reader)
        {
            var length = ReadNumber(reader);
            if (length == 0) return null;

            return reader.ReadBytes(length);
        }
    }
}