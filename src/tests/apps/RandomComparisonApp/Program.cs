using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using SafeOrbit;
using SafeOrbit.Cryptography.Random;
using SafeOrbit.Cryptography.Random.RandomGenerators;

namespace RandomComparisonApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // It's always good to StartFillingEntropyPools as early as possible when the application is launched.
            // But if I'm benchmarking performance below, then it's not really fair to let them start early.
            // StartEarly.StartFillingEntropyPools();

            DateTime before;
            DateTime after;
            RandomResult result;

            const int randBytesLength = 8 * 1024;

            var results = new List<RandomResult>();

            //AllZeros
            result = new RandomResult {AlgorithmName = "AllZeros"};
            before = DateTime.Now;
            var randBytes = new byte[randBytesLength];
            after = DateTime.Now;
            result.TimeSpan = after - before;
            result.CompressionRatio = CompressionUtility.GetCompressionRatio(randBytes);
            results.Add(result);

            //SystemRng
            result = new RandomResult {AlgorithmName = "SystemRng"};
            Console.Write(result.AlgorithmName + " ");
            before = DateTime.Now;
            var mySystemRngCryptoServiceProvider = new SystemRng();
            mySystemRngCryptoServiceProvider.GetBytes(randBytes);
            after = DateTime.Now;
            result.TimeSpan = after - before;
            result.CompressionRatio = CompressionUtility.GetCompressionRatio(randBytes);
            results.Add(result);
            System.Console.WriteLine((after - before).ToString());

            //RNGCryptoServiceProvider
            result = new RandomResult();
            result.AlgorithmName = "RNGCryptoServiceProvider";
            System.Console.Write(result.AlgorithmName + " ");
            before = DateTime.Now;
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randBytes);
            }
            after = DateTime.Now;
            result.TimeSpan = after - before;
            result.CompressionRatio = CompressionUtility.GetCompressionRatio(randBytes);
            results.Add(result);
            System.Console.WriteLine((after - before).ToString());

            //ThreadedSeedGeneratorRng
            result = new RandomResult();
            result.AlgorithmName = "ThreadedSeedGeneratorRng";
            System.Console.Write(result.AlgorithmName + " ");
            before = DateTime.Now;
            var myThreadedSeedGeneratorRng = new ThreadedSeedGeneratorRng();
            myThreadedSeedGeneratorRng.GetBytes(randBytes);
            after = DateTime.Now;
            result.TimeSpan = after - before;
            result.CompressionRatio = CompressionUtility.GetCompressionRatio(randBytes);
            results.Add(result);
            System.Console.WriteLine((after - before).ToString());
            System.Console.Write("Sleeping to allow pool to fill...");
            System.Threading.Thread.Sleep(3000);    // Should be enough time for its pool to fill up, so it won't slow down next:
            System.Console.WriteLine("  Done.");

            //ThreadedSeedGenerator(fast)
            result = new RandomResult();
            result.AlgorithmName = "ThreadedSeedGenerator(fast)";
            System.Console.Write(result.AlgorithmName + " ");
            var myThreadedSeedGenerator = new Org.BouncyCastle.Crypto.Prng.ThreadedSeedGenerator();
            Array.Clear(randBytes, 0, randBytesLength);
            before = DateTime.Now;
            randBytes = myThreadedSeedGenerator.GenerateSeed(randBytesLength, fast: true);
            after = DateTime.Now;
            result.TimeSpan = after - before;
            result.CompressionRatio = CompressionUtility.GetCompressionRatio(randBytes);
            results.Add(result);
            Console.WriteLine((after - before).ToString());

            //ThreadedSeedGenerator(slow)
            result = new RandomResult();
            result.AlgorithmName = "ThreadedSeedGenerator(slow)";
            System.Console.Write(result.AlgorithmName + " ");
            Array.Clear(randBytes, 0, randBytesLength);
            before = DateTime.Now;
            randBytes = myThreadedSeedGenerator.GenerateSeed(randBytesLength, fast: false);
            after = DateTime.Now;
            result.TimeSpan = after - before;
            result.CompressionRatio = CompressionUtility.GetCompressionRatio(randBytes);
            results.Add(result);
            System.Console.WriteLine((after - before).ToString());

            //ThreadSchedulerRNG(slow)
            result = new RandomResult();
            result.AlgorithmName = "ThreadSchedulerRng";
            System.Console.Write(result.AlgorithmName + " ");
            before = DateTime.Now;
            var myThreadSchedulerRNG = new ThreadSchedulerRng();
            myThreadSchedulerRNG.GetBytes(randBytes);
            after = DateTime.Now;
            result.TimeSpan = after - before;
            result.CompressionRatio = CompressionUtility.GetCompressionRatio(randBytes);
            results.Add(result);
            System.Console.WriteLine((after - before).ToString());

            const int numResults = 14;
            System.Console.Write("ticks bit positions ");
            before = DateTime.Now;
            var ticksResults = new RandomResult[numResults];
            var ticksResultsBytes = new byte[numResults][];
            for (int i = 0; i < numResults; i++)
            {
                ticksResults[i] = new RandomResult
                {
                    AlgorithmName = "ticks bit #" + i.ToString().PadLeft(2)
                };
                ticksResultsBytes[i] = new byte[randBytesLength];
            }
            for (int i = 0; i < randBytesLength; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    long ticks = DateTime.Now.Ticks;
                    for (int bitPos = 0; bitPos < numResults; bitPos++)
                    {
                        ticksResultsBytes[bitPos][i] <<= 1;
                        ticksResultsBytes[bitPos][i] += (byte)(ticks % 2);
                        ticks >>= 1;
                    }
                    System.Threading.Thread.Sleep(1);
                }
            }
            after = DateTime.Now;
            for (int i = 0; i < numResults; i++)
            {
                ticksResults[i].TimeSpan = after - before;
                ticksResults[i].CompressionRatio = CompressionUtility.GetCompressionRatio(ticksResultsBytes[i]);
                results.Add(ticksResults[i]);
            }
            System.Console.WriteLine((after - before).ToString());
            System.Console.Write("Sleeping to allow pool to fill...");
            System.Threading.Thread.Sleep(15000);    // Should be enough time for its pool to fill up, so it won't slow down next:
            System.Console.WriteLine("  Done.");

            // I want to test each of the ThreadedSeedGeneratorRng and ThreadSchedulerRng prior to doing TinHatRandom
            // or TinHatURandom, because otherwise, TinHatRandom will create static instances of them, which race, etc.
            // thus throwing off my benchmark results.

            result = new RandomResult();
            result.AlgorithmName = "TinHatRandom";
            System.Console.Write(result.AlgorithmName + " ");
            before = DateTime.Now;
            randBytes = SafeRandom.StaticInstance.GetBytes(randBytes.Length);
            after = DateTime.Now;
            result.TimeSpan = after - before;
            result.CompressionRatio = CompressionUtility.GetCompressionRatio(randBytes);
            results.Add(result);
            System.Console.WriteLine((after - before).ToString());
            System.Console.Write("Sleeping to allow pool to fill...");
            System.Threading.Thread.Sleep(15000);    // Should be enough time for its pool to fill up, so it won't slow down next:
            System.Console.WriteLine("  Done.");

            result = new RandomResult();
            result.AlgorithmName = "TinHatURandom";
            System.Console.Write(result.AlgorithmName + " ");
            before = DateTime.Now;
            randBytes = FastRandom.StaticInstance.GetBytes(randBytes.Length);
            after = DateTime.Now;
            result.TimeSpan = after - before;
            result.CompressionRatio = CompressionUtility.GetCompressionRatio(randBytes);
            results.Add(result);
            System.Console.WriteLine((after - before).ToString());
            System.Console.Write("Sleeping to allow pool to fill...");
            System.Threading.Thread.Sleep(15000);    // Should be enough time for its pool to fill up, so it won't slow down next:
            System.Console.WriteLine("  Done.");

            System.Console.WriteLine("");

            double maxCompressionRatio = double.MinValue;
            double minCompressionRatio = double.MaxValue;
            int longestName = 0;
            foreach (var theResult in results)
            {
                if (theResult.AlgorithmName.Length > longestName) longestName = theResult.AlgorithmName.Length;
                if (theResult.CompressionRatio < minCompressionRatio) minCompressionRatio = theResult.CompressionRatio;
                if (theResult.CompressionRatio > maxCompressionRatio) maxCompressionRatio = theResult.CompressionRatio;
            }
            System.Console.WriteLine("AlgorithmName".PadLeft(longestName) + " : bits per bit : elapsed sec : effective rate");
            foreach (var theResult in results)
            {
                double bitsPerBit = (theResult.CompressionRatio - minCompressionRatio) / (maxCompressionRatio - minCompressionRatio);
                double byteRate;
                string byteRateString;
                if (theResult.TimeSpan.TotalSeconds == 0)
                {
                    if (theResult.CompressionRatio == minCompressionRatio)
                    {
                        byteRateString = "0";
                    }
                    else
                    {
                        byteRateString = "infinity";
                    }
                }
                else
                {
                    byteRate = bitsPerBit * randBytesLength / theResult.TimeSpan.TotalSeconds;
                    if (byteRate > 1000000)
                        byteRateString = (byteRate / 1000000).ToString("F2") + " MiB/sec";
                    else if (byteRate > 1000)
                        byteRateString = (byteRate / 1000).ToString("F2") + " KiB/sec";
                    else
                        byteRateString = byteRate.ToString("F2") + " B/sec";
                }
                System.Console.WriteLine(theResult.AlgorithmName.PadLeft(longestName) + " : " + bitsPerBit.ToString("0.000").PadLeft(12) + " : " + theResult.TimeSpan.TotalSeconds.ToString("0.000").PadLeft(11) + " : " + byteRateString.PadLeft(14));
            }

            Console.WriteLine("");
            Console.Error.WriteLine("Finished");
            Console.Out.Flush();
            Console.ReadKey();
        }
    }
}
