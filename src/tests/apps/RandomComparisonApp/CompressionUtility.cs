using System.IO;
using SharpCompress.Compressors.LZMA;

namespace RandomComparisonApp
{
    public class CompressionUtility
    {
        public static double GetCompressionRatioLowerBound(int length)
        {
            /* 
             * If we feed strings of all zero's into lzma, for various input lengths, here are the output lengths, to be 
             * used as the lower bound for how much compression could possibly squish things.  This is actually the envelope
             * of the output size, up to 8192 input size.  So in reality, sometimes it could squish more, but by taking the
             * envelope, we reduce our estimate of the entropy in the user string.
             *     2-274     : 6.985 + 0.007326 * Length
             *     275-564   : 7.103 + 0.00689655 * Length
             *     565-1384  : 7.555 + 0.00609756 * Length
             *     1385-2202 : 9.227 + 0.00488998 * Length
             *     2203-3022 : 9.254 + 0.00487805 * Length
             *     3023-4096 : 12.741 + 0.00372439 * Length
             *     4097-5188 : 12.993 + 0.003663 * Length
             *     5189-6025 : 13.401 + 0.00358423 * Length
             *     >=6026    : 17.341 + 0.0029304 * Length
             *     
             * This is the envelope of the output size, for varying lengths of input obtained from urandom
             *     1-8       : 4.875 + 1.125 * Length
             *     9-37      : 5.068965512 + 1.103448276 * Length
             *     38-49     : 5.8333333 + 1.08333333 * Length
             *     50-63     : 6.428571429 + 1.071428571 * Length
             *     64-95     : 7 + 1.0625 * Length
             *     96-142    : 8.914893617 + 1.042553191 * Length
             *     143-168   : 9.5 + 1.038461538 * Length
             *     169-203   : 11.17142857 + 1.028571429 * Length
             *     204-401   : 11.84848485 + 1.025252525 * Length
             *     >=402     : 16.94700013 + 1.012569651 * Length
            */
            if (length < 275)
                return 6.985 + 0.007326*length;
            if (length < 565)
                return 7.103 + 0.00689655*length;
            if (length < 1385)
                return 7.555 + 0.00609756*length;
            if (length < 2203)
                return 9.227 + 0.00488998*length;
            if (length < 3023)
                return 9.254 + 0.00487805*length;
            if (length < 4097)
                return 12.741 + 0.00372439*length;
            if (length < 5189)
                return 12.993 + 0.003663*length;
            if (length < 6026)
                return 13.401 + 0.00358423*length;
            return 17.341 + 0.0029304*length;
        }

        public static double GetCompressionRatioUpperBound(int length)
        {
            if (length < 9)
                return 4.875 + 1.125*length;
            if (length < 38)
                return 5.068965512 + 1.103448276*length;
            if (length < 50)
                return 5.8333333 + 1.08333333*length;
            if (length < 64)
                return 6.428571429 + 1.071428571*length;
            if (length < 96)
                return 7 + 1.0625*length;
            if (length < 143)
                return 8.914893617 + 1.042553191*length;
            if (length < 169)
                return 9.5 + 1.038461538*length;
            if (length < 204)
                return 11.17142857 + 1.028571429*length;
            if (length < 402)
                return 11.84848485 + 1.025252525*length;
            return 16.94700013 + 1.012569651*length;
        }

        public static double GetCompressionRatio(byte[] data)
        {
            var length = data.Length;
            var lowerBound = GetCompressionRatioLowerBound(length);
            var upperBound = GetCompressionRatioUpperBound(length);

            double compressionRatio;
            using (var outStream = new MemoryStream())
            {
                using (var lzmaStream = new LzmaStream(new LzmaEncoderProperties(), false, outStream))
                {
                    using (var inStream = new MemoryStream(data))
                    {
                        inStream.CopyTo(lzmaStream);
                    }
                }
                var outBytes = outStream.ToArray();
                compressionRatio = (outBytes.Length - lowerBound)/(upperBound - lowerBound);
                // Because we used an envelope for both the upper and lower bound, the compression ratio is very unlikely
                // to exceed 1.0, but it's possible.  It will quite often be negative if the input is pure rubbish.
                if (compressionRatio > 1.0)
                    compressionRatio = 1.0;
                if (compressionRatio < 0)
                    compressionRatio = 0;
            }
            return compressionRatio;
        }
    }
}