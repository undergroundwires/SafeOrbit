using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;
using Org.BouncyCastle.Crypto.Prng;
using SafeOrbit.Cryptography.Random;
using SafeOrbit.Cryptography.Random.RandomGenerators;

namespace RandomComparisonApp
{
    public class Program
    {
        private static readonly List<RandomResult> Results =  new List<RandomResult>();
        private const int RandBytesLength = 8 * 1024;

        public static void Main(string[] args)
        {
            /*
             * It's always good to call StartEarly.StartFillingEntropyPools() as early as possible when the application is launched.
             * But during benchmarking performance below, then it's not really fair to let them start early.
             */
            Benchmark("SystemRng", (bytesBuffer) =>
            {
                var mySystemRngCryptoServiceProvider = new SystemRng();
                mySystemRngCryptoServiceProvider.GetBytes(bytesBuffer);
                return bytesBuffer;
            });
            Benchmark("RNGCryptoServiceProvider", (bytesBuffer) =>
            {
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(bytesBuffer);
                }
                return bytesBuffer;
            });
            /*
             * Test each of the ThreadedSeedGeneratorRng and ThreadSchedulerRng prior to doing SafeRandom
             * or FastRandom, because otherwise, SafeRandom will create static instances of them, which race, etc
             * thus throwing off my results.
             */
            Benchmark("ThreadedSeedGeneratorRng", (bytesBuffer) =>
            {
                var myThreadedSeedGeneratorRng = new ThreadedSeedGeneratorRng();
                myThreadedSeedGeneratorRng.GetBytes(bytesBuffer);
                return bytesBuffer;
            });
            SleepForPools(3000);
            Benchmark("ThreadedSeedGenerator(fast)", (bytesBuffer) =>
            {
                var myThreadedSeedGenerator = new ThreadedSeedGenerator();
                var seed = myThreadedSeedGenerator.GenerateSeed(RandBytesLength, true);
                return seed;
            });
            Benchmark("ThreadedSeedGenerator(slow)", (bytesBuffer) =>
            {
                var myThreadedSeedGenerator = new ThreadedSeedGenerator();
                var seed = myThreadedSeedGenerator.GenerateSeed(RandBytesLength, false);
                return seed;
            });
            Benchmark("ThreadSchedulerRNG(slow)", (bytesBuffer) =>
            {
                var threadSchedulerRng = new ThreadSchedulerRng();
                threadSchedulerRng.GetBytes(bytesBuffer);
                return bytesBuffer;
            });

            SleepForPools(15000);

            Benchmark(
                "SafeRandom", 
                (bytesBuffer) => SafeRandom.StaticInstance.GetBytes(bytesBuffer.Length));

            SleepForPools(15000);
  
            Benchmark(
                "FastRandom",
                (bytesBuffer) => FastRandom.StaticInstance.GetBytes(bytesBuffer.Length));

            SleepForPools(15000);

            Console.WriteLine("");

            PresentResults();

            Console.WriteLine("");
            Console.Error.WriteLine("Finished");
            Console.Out.Flush();
            Console.ReadKey();
        }
        private static void SleepForPools(int millisecondsTimeout)
        {
            Console.Write("Sleeping to allow pool to fill...");
            Thread.Sleep(millisecondsTimeout); // Should be enough time for its pool to fill up, so it won't slow down next
            Console.WriteLine("  Done.");

        }
        private static void Benchmark(string algorithmName, Func<byte[], byte[]> algorithm)
        {
            var bytesBuffer = new byte[RandBytesLength];
            Console.Write(algorithmName + " ");
            var stopwatch = Stopwatch.StartNew();
            stopwatch.Stop();
            algorithm(bytesBuffer);
            Results.Add(new RandomResult(
                algorithmName,
                CompressionUtility.GetCompressionRatio(bytesBuffer).Result,
                stopwatch.Elapsed
            ));
            Console.WriteLine(stopwatch.Elapsed.ToString());
            Array.Clear(bytesBuffer, 0, bytesBuffer.Length);
        }

        private static void PresentResults()
        {

            var maxCompressionRatio = double.MinValue;
            var minCompressionRatio = double.MaxValue;
            var longestName = 0;
            foreach (var theResult in Results)
            {
                if (theResult.AlgorithmName.Length > longestName) longestName = theResult.AlgorithmName.Length;
                if (theResult.CompressionRatio < minCompressionRatio) minCompressionRatio = theResult.CompressionRatio;
                if (theResult.CompressionRatio > maxCompressionRatio) maxCompressionRatio = theResult.CompressionRatio;
            }
            Console.WriteLine("AlgorithmName".PadLeft(longestName) + " : bits per bit : elapsed sec : effective rate");
            foreach (var theResult in Results)
            {
                var bitsPerBit = (theResult.CompressionRatio - minCompressionRatio) /
                                 (maxCompressionRatio - minCompressionRatio);
                string byteRateString;
                if (theResult.TimeSpan.TotalSeconds == 0)
                {
                    if (theResult.CompressionRatio == minCompressionRatio)
                        byteRateString = "0";
                    else
                        byteRateString = "infinity";
                }
                else
                {
                    var byteRate = bitsPerBit * RandBytesLength / theResult.TimeSpan.TotalSeconds;
                    if (byteRate > 1000000)
                        byteRateString = (byteRate / 1000000).ToString("F2") + " MiB/sec";
                    else if (byteRate > 1000)
                        byteRateString = (byteRate / 1000).ToString("F2") + " KiB/sec";
                    else
                        byteRateString = byteRate.ToString("F2") + " B/sec";
                }
                Console.WriteLine(theResult.AlgorithmName.PadLeft(longestName) + " : " +
                                  bitsPerBit.ToString("0.000").PadLeft(12) + " : " +
                                  theResult.TimeSpan.TotalSeconds.ToString("0.000").PadLeft(11) + " : " +
                                  byteRateString.PadLeft(14));
            }
        }
    }
}