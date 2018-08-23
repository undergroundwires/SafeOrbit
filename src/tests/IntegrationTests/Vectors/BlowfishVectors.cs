
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

using System.Collections;
using NUnit.Framework;
using SafeOrbit.Tests;

namespace SafeOrbit.Vectors
{
    public class BlowfishVectors
    {

        /// <summary>
        /// Gets or sets the blowfish vectors.
        /// First argument => key bytes
        /// Second argument => clear bytes
        /// Returns => cipher bytes
        /// </summary>
        /// <remarks>https://www.schneier.com/code/vectors.txt</remarks>
        public static IEnumerable Vectors
        {
            get
            {
                yield return GetBlowfishVector("0000000000000000", "0000000000000000", "4EF997456198DD78");
                yield return GetBlowfishVector("FFFFFFFFFFFFFFFF", "FFFFFFFFFFFFFFFF", "51866FD5B85ECB8A");
                yield return GetBlowfishVector("3000000000000000", "1000000000000001", "7D856F9A613063F2");
                yield return GetBlowfishVector("1111111111111111", "1111111111111111", "2466DD878B963C9D");
                yield return GetBlowfishVector("0123456789ABCDEF", "1111111111111111", "61F9C3802281B096");
                yield return GetBlowfishVector("1111111111111111", "0123456789ABCDEF", "7D0CC630AFDA1EC7");
                yield return GetBlowfishVector("0000000000000000", "0000000000000000", "4EF997456198DD78");
                yield return GetBlowfishVector("FEDCBA9876543210", "0123456789ABCDEF", "0ACEAB0FC6A0A28D");
                yield return GetBlowfishVector("7CA110454A1A6E57", "01A1D6D039776742", "59C68245EB05282B");
                yield return GetBlowfishVector("0131D9619DC1376E", "5CD54CA83DEF57DA", "B1B8CC0B250F09A0");
                yield return GetBlowfishVector("07A1133E4A0B2686", "0248D43806F67172", "1730E5778BEA1DA4");
                yield return GetBlowfishVector("3849674C2602319E", "51454B582DDF440A", "A25E7856CF2651EB");
                yield return GetBlowfishVector("04B915BA43FEB5B6", "42FD443059577FA2", "353882B109CE8F1A");
                yield return GetBlowfishVector("0113B970FD34F2CE", "059B5E0851CF143A", "48F4D0884C379918");
                yield return GetBlowfishVector("0170F175468FB5E6", "0756D8E0774761D2", "432193B78951FC98");
                yield return GetBlowfishVector("43297FAD38E373FE", "762514B829BF486A", "13F04154D69D1AE5");
                yield return GetBlowfishVector("07A7137045DA2A16", "3BDD119049372802", "2EEDDA93FFD39C79");
                yield return GetBlowfishVector("04689104C2FD3B2F", "26955F6835AF609A", "D887E0393C2DA6E3");
                yield return GetBlowfishVector("37D06BB516CB7546", "164D5E404F275232", "5F99D04F5B163969");
                yield return GetBlowfishVector("1F08260D1AC2465E", "6B056E18759F5CCA", "4A057A3B24D3977B");
                yield return GetBlowfishVector("584023641ABA6176", "004BD6EF09176062", "452031C1E4FADA8E");
                yield return GetBlowfishVector("025816164629B007", "480D39006EE762F2", "7555AE39F59B87BD");
                yield return GetBlowfishVector("49793EBC79B3258F", "437540C8698F3CFA", "53C55F9CB49FC019");
                yield return GetBlowfishVector("4FB05E1515AB73A7", "072D43A077075292", "7A8E7BFA937E89A3");
                yield return GetBlowfishVector("49E95D6D4CA229BF", "02FE55778117F12A", "CF9C5D7A4986ADB5");
                yield return GetBlowfishVector("018310DC409B26D6", "1D9D5C5018F728C2", "D1ABB290658BC778");
                yield return GetBlowfishVector("1C587F1C13924FEF", "305532286D6F295A", "55CB3774D13EF201");
                yield return GetBlowfishVector("0101010101010101", "0123456789ABCDEF", "FA34EC4847B268B2");
                yield return GetBlowfishVector("1F1F1F1F0E0E0E0E", "0123456789ABCDEF", "A790795108EA3CAE");
                yield return GetBlowfishVector("E0FEE0FEF1FEF1FE", "0123456789ABCDEF", "C39E072D9FAC631D");
                yield return GetBlowfishVector("0000000000000000", "FFFFFFFFFFFFFFFF", "014933E0CDAFF6E4");
                yield return GetBlowfishVector("FFFFFFFFFFFFFFFF", "0000000000000000", "F21E9A77B71C49BC");
                yield return GetBlowfishVector("0123456789ABCDEF", "0000000000000000", "245946885754369A");
                yield return GetBlowfishVector("FEDCBA9876543210", "FFFFFFFFFFFFFFFF", "6B5C5A9C5D9E0A5A");
            }
        }

        /// <summary>
        /// Gets the blowfish vector.
        /// </summary>
        /// <param name="keyBytesHexString">The key bytes as hexadecimal string.</param>
        /// <param name="clearBytesHexString">The clear bytes as hexadecimal string.</param>
        /// <param name="cipherBytesHexString">The cipher bytes as hexadecimal string.</param>
        /// <returns>A <see cref="TestCaseData"/> for with the argument order as <see cref="GetBlowfishVector"/> method.</returns>
        private static TestCaseData GetBlowfishVector(string keyBytesHexString, string clearBytesHexString,
            string cipherBytesHexString)
        {
            return new TestCaseData
            (
                Hex.DecodeHexToBytes(keyBytesHexString),
                Hex.DecodeHexToBytes(clearBytesHexString),
                Hex.DecodeHexToBytes(cipherBytesHexString)
            );
        }
    }
}