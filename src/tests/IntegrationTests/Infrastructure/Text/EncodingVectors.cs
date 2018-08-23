
/*
MIT License

Copyright (c) 2016-2018 Erkin Ekici - undergroundwires@safeorb.it

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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

// ReSharper disable InconsistentNaming

namespace SafeOrbit.Text
{
    /// <summary>
    ///     Vectors are generated using https://dotnetfiddle.net/, (ex :
    ///     System.Text.Encoding.ASCII.GetBytes("ABC0000").Dump();)
    ///     2016-07-12
    /// </summary>
    public class EncodingVectors
    {
        private static readonly string[] Strings =
        {
            "a", "0", "ğ", "𤭢",
            "Lorem ipsum dolor sit amet",
            "L0R3/\\/\\ 1P5U/\\/\\ d0L0R 517 4/\\/\\37@ä\0", //L0R3/\\/\\ 1P5U/\\/\\ d0L0R 517 4/\\/\\37
            $"string\r\nw1hLinés𐐷"
        };

        public static readonly byte[][] AsciiBytes =
        {
            new byte[] {97}, new byte[] {48}, new byte[] {63}, new byte[] {63, 63},
            new byte[]
            {
                76, 111, 114, 101, 109, 32, 105, 112, 115, 117, 109, 32, 100, 111, 108, 111, 114, 32, 115, 105, 116, 32,
                97, 109, 101, 116
            },
            new byte[]
            {
                76, 48, 82, 51, 47, 92, 47, 92, 32, 49, 80, 53, 85, 47, 92, 47, 92, 32, 100, 48, 76, 48, 82, 32, 53, 49,
                55, 32, 52, 47, 92, 47, 92, 51, 55, 64, 63, 0
            },
            new byte[] {115, 116, 114, 105, 110, 103, 13, 10, 119, 49, 104, 76, 105, 110, 63, 115, 63, 63}
        };

        private static readonly byte[][] Utf16BigEndianBytes =
        {
            new byte[] {0, 97}, new byte[] {0, 48}, new byte[] {1, 31}, new byte[] {216, 82, 223, 98},
            new byte[]
            {
                0, 76, 0, 111, 0, 114, 0, 101, 0, 109, 0, 32, 0, 105, 0, 112, 0, 115, 0, 117, 0, 109, 0, 32, 0, 100, 0,
                111, 0, 108, 0, 111, 0, 114, 0, 32, 0, 115, 0, 105, 0, 116, 0, 32, 0, 97, 0, 109, 0, 101, 0, 116
            },
            new byte[]
            {
                0, 76, 0, 48, 0, 82, 0, 51, 0, 47, 0, 92, 0, 47, 0, 92, 0, 32, 0, 49, 0, 80, 0, 53, 0, 85, 0, 47, 0, 92,
                0, 47, 0, 92, 0, 32, 0, 100, 0, 48, 0, 76, 0, 48, 0, 82, 0, 32, 0, 53, 0, 49, 0, 55, 0, 32, 0, 52, 0, 47,
                0, 92, 0, 47, 0, 92, 0, 51, 0, 55, 0, 64, 0, 228, 0, 0
            },
            new byte[]
            {
                0, 115, 0, 116, 0, 114, 0, 105, 0, 110, 0, 103, 0, 13, 0, 10, 0, 119, 0, 49, 0, 104, 0, 76, 0, 105, 0,
                110, 0, 233, 0, 115, 216, 1, 220, 55
            }
        };

        private static readonly byte[][] Utf16LittleEndianBytes =
        {
            new byte[] {97, 0}, new byte[] {48, 0}, new byte[] {31, 1}, new byte[] {82, 216, 98, 223},
            new byte[]
            {
                76, 0, 111, 0, 114, 0, 101, 0, 109, 0, 32, 0, 105, 0, 112, 0, 115, 0, 117, 0, 109, 0, 32, 0, 100, 0,
                111, 0, 108, 0, 111, 0, 114, 0, 32, 0, 115, 0, 105, 0, 116, 0, 32, 0, 97, 0, 109, 0, 101, 0, 116, 0
            },
            new byte[]
            {
                76, 0, 48, 0, 82, 0, 51, 0, 47, 0, 92, 0, 47, 0, 92, 0, 32, 0, 49, 0, 80, 0, 53, 0, 85, 0, 47, 0, 92, 0,
                47, 0, 92, 0, 32, 0, 100, 0, 48, 0, 76, 0, 48, 0, 82, 0, 32, 0, 53, 0, 49, 0, 55, 0, 32, 0, 52, 0, 47, 0,
                92, 0, 47, 0, 92, 0, 51, 0, 55, 0, 64, 0, 228, 0, 0, 0
            },
            new byte[]
            {
                115, 0, 116, 0, 114, 0, 105, 0, 110, 0, 103, 0, 13, 0, 10, 0, 119, 0, 49, 0, 104, 0, 76, 0, 105, 0, 110,
                0, 233, 0, 115, 0, 1, 216, 55, 220
            }
        };

        public static IEnumerable Utf16BigEndianBytes_To_AsciiBytes_Vector
        {
            get
            {
                for (var i = 0; i < Utf16BigEndianBytes.Count(); i++)
                    yield return new TestCaseData(Utf16BigEndianBytes[i]).Returns(AsciiBytes[i]);
            }
        }

        public static IEnumerable<TestCaseData> AsciiBytes_To_Utf16LittleEndianBytes_Vector
        {
            get
            {
                yield return new TestCaseData(
                        /* ASCII */
                        new byte[] {97})
                    .Returns(
                        /* Utf16LittleEndian */
                        new byte[] {97, 0});
                yield return new TestCaseData(
                        /* ASCII */
                        new byte[] {48})
                    .Returns(
                        /* Utf16LittleEndian */
                        new byte[] {48, 0});
                yield return new TestCaseData(
                        /* ASCII */
                        new byte[] {63})
                    .Returns(
                        /* Utf16LittleEndian */
                        new byte[] {63, 0});
                yield return new TestCaseData(
                        /* ASCII */
                        new byte[] {63, 63})
                    .Returns(
                        /* Utf16LittleEndian */
                        new byte[] {63, 0, 63, 0});
                yield return new TestCaseData(
                    /* ASCII */
                    new byte[]
                    {
                        76, 111, 114, 101, 109, 32, 105, 112, 115, 117, 109, 32, 100, 111, 108, 111,
                        114, 32, 115, 105, 116, 32, 97, 109, 101, 116
                    }).Returns(
                    /* Utf16LittleEndian */
                    new byte[]
                    {
                        76, 0, 111, 0, 114, 0, 101, 0, 109, 0, 32, 0, 105, 0, 112, 0, 115, 0, 117, 0, 109, 0, 32, 0, 100,
                        0, 111, 0, 108, 0, 111, 0, 114, 0, 32, 0, 115, 0, 105, 0, 116, 0, 32, 0, 97, 0, 109, 0, 101, 0,
                        116, 0
                    });
                yield return new TestCaseData(
                    /* ASCII */
                    new byte[]
                    {
                        76, 48, 82, 51, 47, 92, 47, 92, 32, 49, 80, 53, 85, 47, 92, 47, 92, 32, 100, 48, 76, 48, 82, 32,
                        53,
                        49, 55, 32, 52, 47, 92, 47, 92, 51, 55, 64, 63, 0
                    }).Returns(
                    /* Utf16LittleEndian */
                    new byte[]
                    {
                        76, 0, 48, 0, 82, 0, 51, 0, 47, 0, 92, 0, 47, 0, 92, 0, 32, 0, 49, 0, 80, 0, 53, 0, 85, 0, 47, 0,
                        92, 0, 47, 0, 92, 0, 32, 0, 100, 0, 48, 0, 76, 0, 48, 0, 82, 0, 32, 0, 53, 0, 49, 0, 55, 0, 32,
                        0,
                        52, 0, 47, 0, 92, 0, 47, 0, 92, 0, 51, 0, 55, 0, 64, 0, 63, 0, 0, 0
                    });
                yield return
                    new TestCaseData(
                        /* ASCII */
                        new byte[] {115, 116, 114, 105, 110, 103, 13, 10, 119, 49, 104, 76, 105, 110, 63, 115, 63, 63}
                    ).Returns(
                        /* Utf16LittleEndian */
                        new byte[]
                        {
                            115, 0, 116, 0, 114, 0, 105, 0, 110, 0, 103, 0, 13, 0, 10, 0, 119, 0, 49, 0, 104, 0, 76, 0,
                            105, 0, 110, 0, 63, 0, 115, 0, 63, 0, 63, 0
                        });
            }
        }

        public static IEnumerable AsciiBytes_To_Utf16BigEndianBytes_Vector
        {
            get
            {
                yield return new TestCaseData(
                        /* ASCII */
                        new byte[] {97})
                    .Returns(
                        /* Utf16BigEndian */
                        new byte[] {0, 97});
                yield return new TestCaseData(
                        /* ASCII */
                        new byte[] {48})
                    .Returns(
                        /* Utf16BigEndian */
                        new byte[] {0, 48});
                yield return new TestCaseData(
                        /* ASCII */
                        new byte[] {63})
                    .Returns(
                        /* Utf16BigEndian */
                        new byte[] {0, 63});
                yield return new TestCaseData(
                        /* ASCII */
                        new byte[] {63, 63})
                    .Returns(
                        /* Utf16BigEndian */
                        new byte[] {0, 63, 0, 63});
                yield return new TestCaseData(
                    /* ASCII */
                    new byte[]
                    {
                        76, 111, 114, 101, 109, 32, 105, 112, 115, 117, 109, 32, 100, 111, 108, 111,
                        114, 32, 115, 105, 116, 32, 97, 109, 101, 116
                    }).Returns(
                    /* Utf16BigEndian */
                    new byte[]
                    {
                        0, 76, 0, 111, 0, 114, 0, 101, 0, 109, 0, 32, 0, 105, 0, 112, 0, 115, 0, 117, 0, 109, 0, 32, 0,
                        100, 0, 111, 0, 108, 0, 111, 0, 114, 0, 32, 0, 115, 0, 105, 0, 116, 0, 32, 0, 97, 0, 109, 0, 101,
                        0, 116
                    });
                yield return new TestCaseData(
                    /* ASCII */
                    new byte[]
                    {
                        76, 48, 82, 51, 47, 92, 47, 92, 32, 49, 80, 53, 85, 47, 92, 47, 92, 32, 100, 48, 76, 48, 82, 32,
                        53, 49, 55, 32, 52, 47, 92, 47, 92, 51, 55, 64, 63, 0
                    }).Returns(
                    /* Utf16BigEndian */
                    new byte[]
                    {
                        0, 76, 0, 48, 0, 82, 0, 51, 0, 47, 0, 92, 0, 47, 0, 92, 0, 32, 0, 49, 0, 80, 0, 53, 0, 85, 0, 47,
                        0, 92, 0, 47, 0, 92, 0, 32, 0, 100, 0, 48, 0, 76, 0, 48, 0, 82, 0, 32, 0, 53, 0, 49, 0, 55, 0,
                        32, 0, 52, 0, 47, 0, 92, 0, 47, 0, 92, 0, 51, 0, 55, 0, 64, 0, 63, 0, 0
                    });
                yield return
                    new TestCaseData(
                        /* ASCII */
                        new byte[] {115, 116, 114, 105, 110, 103, 13, 10, 119, 49, 104, 76, 105, 110, 63, 115, 63, 63}
                    ).Returns(
                        /* Utf16BigEndian */
                        new byte[]
                        {
                            0, 115, 0, 116, 0, 114, 0, 105, 0, 110, 0, 103, 0, 13, 0, 10, 0, 119, 0, 49, 0, 104, 0, 76,
                            0, 105, 0, 110, 0, 63, 0, 115, 0, 63, 0, 63
                        });
            }
        }

        public static IEnumerable Ascii_To_Utf16BigEndian_Vector
        {
            get
            {
                for (var i = 0; i < Utf16BigEndianBytes.Count(); i++)
                    yield return new TestCaseData(AsciiBytes[i]).Returns(Utf16BigEndianBytes[i]);
            }
        }

        public static IEnumerable Utf16BigEndianBytes_To_Utf16LittleEndianBytes_Vector
        {
            get
            {
                for (var i = 0; i < Utf16BigEndianBytes.Count(); i++)
                    yield return new TestCaseData(Utf16BigEndianBytes[i]).Returns(Utf16LittleEndianBytes[i]);
            }
        }

        public static IEnumerable Utf16LittleEndianBytes_To_Utf16BigEndianBytes_Vector
        {
            get
            {
                for (var i = 0; i < Utf16LittleEndianBytes.Count(); i++)
                    yield return new TestCaseData(Utf16LittleEndianBytes[i]).Returns(Utf16BigEndianBytes[i]);
            }
        }

        public static IEnumerable Utf16LittleEndianBytes_To_AsciiBytes_Vector
        {
            get
            {
                for (var i = 0; i < Utf16LittleEndianBytes.Count(); i++)
                    yield return new TestCaseData(Utf16LittleEndianBytes[i]).Returns(AsciiBytes[i]);
            }
        }

        public static IEnumerable String_To_AsciiBytes_Vector
        {
            get
            {
                for (var i = 0; i < Strings.Count(); i++)
                    yield return new TestCaseData(Strings[i]).Returns(AsciiBytes[i]);
            }
        }

        public static IEnumerable String_To_Utf16LittleEndianBytes_Vector
        {
            get
            {
                for (var i = 0; i < Strings.Count(); i++)
                    yield return new TestCaseData(Strings[i]).Returns(Utf16LittleEndianBytes[i]);
            }
        }

        public static IEnumerable String_To_Utf16BigEndianBytes_Vector
        {
            get
            {
                for (var i = 0; i < Strings.Count(); i++)
                    yield return new TestCaseData(Strings[i]).Returns(Utf16BigEndianBytes[i]);
            }
        }


        public static IEnumerable Char_To_AsciiBytes_Vector
        {
            get
            {
                yield return new TestCaseData('a').Returns(new byte[] {97});
                yield return new TestCaseData('Z').Returns(new byte[] {90});
                yield return new TestCaseData('@').Returns(new byte[] {64});
                yield return new TestCaseData('ğ').Returns(new byte[] {63});
                yield return new TestCaseData('Ö').Returns(new byte[] {63});
                yield return new TestCaseData('з').Returns(new byte[] {63});
                yield return new TestCaseData('5').Returns(new byte[] {53});
                yield return new TestCaseData('~').Returns(new byte[] {126});
                yield return new TestCaseData('\0').Returns(new byte[] {0});
                yield return new TestCaseData('\n').Returns(new byte[] {10});
                yield return new TestCaseData('\r').Returns(new byte[] {13});
            }
        }

        public static IEnumerable Char_To_Utf16LittleEndianBytes_Vector
        {
            get
            {
                yield return new TestCaseData('a').Returns(new byte[] {97, 0});
                yield return new TestCaseData('Z').Returns(new byte[] {90, 0});
                yield return new TestCaseData('@').Returns(new byte[] {64, 0});
                yield return new TestCaseData('ğ').Returns(new byte[] {31, 1});
                yield return new TestCaseData('Ö').Returns(new byte[] {214, 0});
                yield return new TestCaseData('з').Returns(new byte[] {55, 4});
                yield return new TestCaseData('5').Returns(new byte[] {53, 0});
                yield return new TestCaseData('~').Returns(new byte[] {126, 0});
                yield return new TestCaseData('\n').Returns(new byte[] {10, 0});
                yield return new TestCaseData('\r').Returns(new byte[] {13, 0});
                yield return new TestCaseData('\0').Returns(new byte[] {0, 0});
            }
        }

        public static IEnumerable Char_To_Utf16BigEndianBytes_Vector
        {
            get
            {
                yield return new TestCaseData('a').Returns(new byte[] {0, 97});
                yield return new TestCaseData('Z').Returns(new byte[] {0, 90});
                yield return new TestCaseData('@').Returns(new byte[] {0, 64});
                yield return new TestCaseData('ğ').Returns(new byte[] {1, 31});
                yield return new TestCaseData('Ö').Returns(new byte[] {0, 214});
                yield return new TestCaseData('з').Returns(new byte[] {4, 55});
                yield return new TestCaseData('5').Returns(new byte[] {0, 53});
                yield return new TestCaseData('~').Returns(new byte[] {0, 126});
                yield return new TestCaseData('\n').Returns(new byte[] {0, 10});
                yield return new TestCaseData('\r').Returns(new byte[] {0, 13});
                yield return new TestCaseData('\0').Returns(new byte[] {0, 0});
            }
        }

        public static IEnumerable AsciiBytes_To_Chars_Vector
        {
            get
            {
                yield return new TestCaseData(new byte[] {97})
                    .Returns(new[] {'a'});
                yield return new TestCaseData(new byte[] {0})
                    .Returns(new[] {'\0'});
                yield return new TestCaseData(new byte[] {48})
                    .Returns(new[] {'0'});
                yield return new TestCaseData(new byte[] {63})
                    .Returns(new[] {'?'}); //undefined char
                yield return new TestCaseData(new byte[] {63, 63})
                    .Returns(new[] {'?', '?'}); //undefined char
                yield return new TestCaseData(new byte[] {10})
                    .Returns(new[] {'\n'}); //newline
                yield return new TestCaseData(new byte[] {13})
                    .Returns(new[] {'\r'}); //return
                yield return
                    new TestCaseData(new byte[]
                        {
                            76, 111, 114, 101, 109, 32, 105, 112, 115, 117, 109, 32, 100, 111, 108, 111, 114, 32, 115,
                            105, 116, 32, 97, 109, 101, 116
                        })
                        .Returns(new[]
                        {
                            'L', 'o', 'r', 'e', 'm', ' ', 'i', 'p', 's', 'u', 'm', ' ', 'd', 'o', 'l', 'o', 'r', ' ',
                            's', 'i', 't', ' ', 'a', 'm', 'e', 't'
                        });
                yield return
                    new TestCaseData(new byte[]
                        {
                            76, 48, 82, 51, 47, 92, 47, 92, 32, 49, 80, 53, 85, 47, 92, 47, 92, 32, 100, 48, 76, 48, 82,
                            32, 53, 49, 55, 32, 52, 47, 92, 47, 92, 51, 55, 64, 63, 0
                        })
                        .Returns(new[]
                        {
                            'L', '0', 'R', '3', '/', '\\', '/', '\\', ' ', '1', 'P', '5', 'U', '/', '\\', '/', '\\',
                            ' ', 'd', '0', 'L', '0', 'R', ' ', '5', '1', '7', ' ', '4', '/', '\\', '/', '\\', '3', '7',
                            '@', '?', '\0'
                        });
                yield return
                    new TestCaseData(new byte[]
                            {115, 116, 114, 105, 110, 103, 13, 10, 119, 49, 104, 76, 105, 110, 63, 115})
                        .Returns(new[]
                            {'s', 't', 'r', 'i', 'n', 'g', '\r', '\n', 'w', '1', 'h', 'L', 'i', 'n', '?', 's'});
            }
        }

        public static IEnumerable Utf16LittleEndianBytes_To_Chars_Vector
        {
            get
            {
                for (var i = 0; i < Utf16LittleEndianBytes.Count(); i++)
                    yield return new TestCaseData(Utf16LittleEndianBytes[i]).Returns(Strings[i].ToCharArray());
            }
        }

        public static IEnumerable Utf16BigEndianBytes_To_Chars_Vector
        {
            get
            {
                for (var i = 0; i < Utf16BigEndianBytes.Count(); i++)
                    yield return new TestCaseData(Utf16BigEndianBytes[i]).Returns(Strings[i].ToCharArray());
            }
        }


        public static IEnumerable AsciiBytes_To_String_Vector
        {
            get
            {
                yield return new TestCaseData(new byte[] {97})
                    .Returns("a");
                yield return new TestCaseData(new byte[] {0})
                    .Returns("\0");
                yield return new TestCaseData(new byte[] {48})
                    .Returns("0");
                yield return new TestCaseData(new byte[] {63})
                    .Returns("?"); //undefined char
                yield return new TestCaseData(new byte[] {63, 63})
                    .Returns("??"); //undefined char
                yield return new TestCaseData(new byte[] {10})
                    .Returns("\n"); //newline
                yield return new TestCaseData(new byte[] {13})
                    .Returns("\r"); //return
                yield return
                    new TestCaseData(new byte[]
                        {
                            76, 111, 114, 101, 109, 32, 105, 112, 115, 117, 109, 32, 100, 111, 108, 111, 114, 32, 115,
                            105, 116, 32, 97, 109, 101, 116
                        })
                        .Returns("Lorem ipsum dolor sit amet");
                yield return
                    new TestCaseData(new byte[]
                        {
                            76, 48, 82, 51, 47, 92, 47, 92, 32, 49, 80, 53, 85, 47, 92, 47, 92, 32, 100, 48, 76, 48, 82,
                            32, 53, 49, 55, 32, 52, 47, 92, 47, 92, 51, 55, 64, 63, 0
                        })
                        .Returns("L0R3/\\/\\ 1P5U/\\/\\ d0L0R 517 4/\\/\\37@?\0");
                yield return
                    new TestCaseData(new byte[]
                            {115, 116, 114, 105, 110, 103, 13, 10, 119, 49, 104, 76, 105, 110, 63, 115})
                        .Returns("string\r\nw1hLin?s");
            }
        }

        public static IEnumerable Utf16LittleEndianBytes_To_String_Vector
        {
            get
            {
                for (var i = 0; i < Utf16LittleEndianBytes.Count(); i++)
                    yield return new TestCaseData(Utf16LittleEndianBytes[i]).Returns(Strings[i]);
            }
        }

        public static IEnumerable Utf16BigEndianBytes_To_String_Vector
        {
            get
            {
                for (var i = 0; i < Utf16BigEndianBytes.Count(); i++)
                    yield return new TestCaseData(Utf16BigEndianBytes[i]).Returns(Strings[i]);
            }
        }
    }
}