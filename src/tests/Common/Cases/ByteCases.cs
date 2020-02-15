namespace SafeOrbit.Tests.Cases
{
    public static class ByteCases
    {
        public static byte[] AllBytes =
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21,
            22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48,
            49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75,
            76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101,
            102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123,
            124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145,
            146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167,
            168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189,
            190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211,
            212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 234,
            235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255
        };

        public static object[] DifferentBytePairs =
        {
            new byte[] {0, 1}, new byte[] {0, 255}, new byte[] {255, 53},
            new byte[] {53, 255}, new byte[] {60, 41}, new byte[] {67, 19}, new byte[] {230, 140}, new byte[] {17, 88},
            new byte[] {10, 58}, new byte[] {50, 166}, new byte[] {227, 71}, new byte[] {133, 134},
            new byte[] {202, 2}, new byte[] {22, 222}, new byte[] {152, 99}
        };

        public static object[] ByteArray32Length =
        {
            new byte[]
            {
                0x7e, 0xa8, 0x77, 0xdb, 0xd2, 0xf6, 0x73, 0xe5, 0x7b, 0xf9, 0x95, 0x12, 0x7e, 0x8b, 0x34, 0xc7,
                0xf7, 0x31, 0xa2, 0xb6, 0x50, 0x6f, 0x7a, 0x20, 0xeb, 0x94, 0x49, 0xcb, 0x2, 0xfa, 0xbf, 0x32
            }
        };

        public static object[] DifferentByteArrayPairs32Length =
        {
            new object[]
            {
                new byte[]
                {
                    0x7e, 0xa8, 0x77, 0xdb, 0xd2, 0xf6, 0x73, 0xe5, 0x7b, 0xf9, 0x95, 0x12, 0x7e, 0x8b, 0x34, 0xc7,
                    0xf7, 0x31, 0xa2, 0xb6, 0x50, 0x6f, 0x7a, 0x20, 0xeb, 0x94, 0x49, 0xcb, 0x2, 0xfa, 0xbf, 0x32
                },
                new byte[]
                {
                    0xb3, 0x7b, 0xe8, 0xcd, 0x2b, 0x1e, 0x64, 0xd5, 0x4e, 0x61, 0xe1, 0x3f, 0xd1, 0xd6, 0x2c, 0xf5,
                    0xed, 0x6d, 0xb, 0x6d, 0xd1, 0xf8, 0xef, 0x95, 0xb0, 0x5e, 0x3c, 0x89, 0x65, 0xf3, 0xdf, 0x2
                }
            },
            new object[] //only last byte on the second byte[] is different
            {
                new byte[]
                {
                    0xb3, 0x7b, 0xe8, 0xcd, 0x2b, 0x1e, 0x64, 0xd5, 0x4e, 0x61, 0xe1, 0x3f, 0xd1, 0xd6, 0x2c, 0xf5,
                    0xed, 0x6d, 0xb, 0x6d, 0xd1, 0xf8, 0xef, 0x95, 0xb0, 0x5e, 0x3c, 0x89, 0x65, 0xf3, 0xdf, 0x2
                },
                new byte[]
                {
                    0xb3, 0x7b, 0xe8, 0xcd, 0x2b, 0x1e, 0x64, 0xd5, 0x4e, 0x61, 0xe1, 0x3f, 0xd1, 0xd6, 0x2c, 0xf5,
                    0xed, 0x6d, 0xb, 0x6d, 0xd1, 0xf8, 0xef, 0x95, 0xb0, 0x5e, 0x3c, 0x89, 0x65, 0xf3, 0xdf, 0x0
                }
            }
        };

        public static object[] TwoDifferentByteArrays16Length =
        {
            new object[]
            {
                new byte[]
                {
                    0x7e, 0xa8, 0x77, 0xdb, 0xd2, 0xf6, 0x73, 0xe5, 0x7b, 0xf9, 0x95, 0x12, 0x7e, 0x8b, 0x34, 0xc7
                },
                new byte[]
                {
                    0xb3, 0x7b, 0xe8, 0xcd, 0x2b, 0x1e, 0x64, 0xd5, 0x4e, 0x61, 0xe1, 0x3f, 0xd1, 0xd6, 0x2c, 0xf5
                }
            }
        };

        public static object[] ThreeDifferentByteArrays16Length =
        {
            new object[]
            {
                new byte[]
                {
                    0x7e, 0xa8, 0x77, 0xdb, 0xd2, 0xf6, 0x73, 0xe5, 0x7b, 0xf9, 0x95, 0x12, 0x7e, 0x8b, 0x34, 0xc7
                },
                new byte[]
                {
                    0xb3, 0x7b, 0xe8, 0xcd, 0x2b, 0x1e, 0x64, 0xd5, 0x4e, 0x61, 0xe1, 0x3f, 0xd1, 0xd6, 0x2c, 0xf5
                },
                new byte[]
                {
                    0xed, 0x6d, 0xb, 0x6d, 0xd1, 0xf8, 0xef, 0x95, 0xb0, 0x5e, 0x3c, 0x89, 0x65, 0xf3, 0xdf, 0x0
                }
            }
        };
    }
}