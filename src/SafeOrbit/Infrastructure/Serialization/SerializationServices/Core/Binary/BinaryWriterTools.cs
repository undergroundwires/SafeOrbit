﻿//This is a modified version of the beautiful SharpSerializer by Pawel Idzikowski (see: http://www.sharpserializer.com)

using System;
using System.IO;
using System.Reflection;
using SafeOrbitNetCore.Infrastructure.Serialization.SharpSerializer.Common;

namespace SafeOrbit.Infrastructure.Serialization.SerializationServices.Core.Binary
{
    /// <summary>
    ///     Some methods which are used by IBinaryWriter
    /// </summary>
    internal static class BinaryWriterTools
    {
        /// <summary>
        /// </summary>
        /// <param name="number"></param>
        /// <param name="writer"></param>
        public static void WriteNumber(int number, BinaryWriter writer)
        {
            // Write size
            var size = NumberSize.GetNumberSize(number);
            writer.Write(size);

            // Write number
            if (size > NumberSize.Zero)
                switch (size)
                {
                    case NumberSize.B1:
                        writer.Write((byte) number);
                        break;
                    case NumberSize.B2:
                        writer.Write((short) number);
                        break;
                    default:
                        writer.Write(number);
                        break;
                }
        }

        /// <summary>
        /// </summary>
        /// <param name="numbers"></param>
        /// <param name="writer"></param>
        public static void WriteNumbers(int[] numbers, BinaryWriter writer)
        {
            // Length
            WriteNumber(numbers.Length, writer);

            // Numbers
            foreach (var number in numbers)
                WriteNumber(number, writer);
        }


        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="writer"></param>
        public static void WriteValue(object value, BinaryWriter writer)
        {
            if (value == null)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                WriteValueCore(value, writer);
            }
        }

        /// <summary>
        ///     BinaryWriter.Write(string...) can not be used as it produces exception if the text is null.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="writer"></param>
        public static void WriteString(string text, BinaryWriter writer)
        {
            if (string.IsNullOrEmpty(text))
            {
                // no exception if the text is null
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                writer.Write(text);
            }
        }

        private static void WriteValueCore(object value, BinaryWriter writer)
        {
            if (value == null) throw new ArgumentNullException(nameof(value), "Written data can not be null.");

            // Write argument data
            var type = value.GetType();

            if (type == typeof(byte[]))
            {
                WriteArrayOfByte((byte[]) value, writer);
                return;
            }
            if (type == typeof(string))
            {
                writer.Write((string) value);
                return;
            }
            if (type == typeof(bool))
            {
                writer.Write((bool) value);
                return;
            }
            if (type == typeof(byte))
            {
                writer.Write((byte) value);
                return;
            }
            if (type == typeof(char))
            {
                writer.Write((char) value);
                return;
            }
            if (type == typeof(DateTime))
            {
                writer.Write(((DateTime) value).Ticks);
                return;
            }
            if (type == typeof(Guid))
            {
                writer.Write(((Guid) value).ToByteArray());
                return;
            }
#if DEBUG || PORTABLE || SILVERLIGHT
            if (type == typeof(decimal))
            {
                WriteDecimal((decimal) value, writer);
                return;
            }
#else
            if (type == typeof (Decimal))
            {
                writer.Write((Decimal) value);
                return;
            }
#endif
            if (type == typeof(double))
            {
                writer.Write((double) value);
                return;
            }
            if (type == typeof(short))
            {
                writer.Write((short) value);
                return;
            }
            if (type == typeof(int))
            {
                writer.Write((int) value);
                return;
            }
            if (type == typeof(long))
            {
                writer.Write((long) value);
                return;
            }
            if (type == typeof(sbyte))
            {
                writer.Write((sbyte) value);
                return;
            }
            if (type == typeof(float))
            {
                writer.Write((float) value);
                return;
            }
            if (type == typeof(ushort))
            {
                writer.Write((ushort) value);
                return;
            }
            if (type == typeof(uint))
            {
                writer.Write((uint) value);
                return;
            }
            if (type == typeof(ulong))
            {
                writer.Write((ulong) value);
                return;
            }

            if (type == typeof(TimeSpan))
            {
                writer.Write(((TimeSpan) value).Ticks);
                return;
            }

            // Enumeration
            if (type.GetTypeInfo().IsEnum)
            {
                writer.Write(Convert.ToInt32(value));
                return;
            }

            // Type
            if (ReflectionHelper.IsType(type))
            {
                writer.Write(((Type) value).AssemblyQualifiedName);
                return;
            }

            throw new InvalidOperationException($"Unknown simple type: {type.FullName}");
        }

        private static void WriteDecimal(decimal value, BinaryWriter writer)
        {
            var bits = decimal.GetBits(value);
            writer.Write(bits[0]);
            writer.Write(bits[1]);
            writer.Write(bits[2]);
            writer.Write(bits[3]);
        }
        private static void WriteArrayOfByte(byte[] data, BinaryWriter writer)
        {
            WriteNumber(data.Length, writer);
            writer.Write(data);
        }
    }
}