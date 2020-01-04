using System;

namespace RandomComparisonApp
{
    public class RandomResult
    {
        public string AlgorithmName { get; }
        public double CompressionRatio { get; }
        public TimeSpan TimeSpan { get; }

        public RandomResult(string algorithmName, double compressionRatio, TimeSpan timeSpan)
        {
            if (string.IsNullOrEmpty(algorithmName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(algorithmName));
            if (compressionRatio <= 0) throw new ArgumentOutOfRangeException(nameof(compressionRatio));
            AlgorithmName = algorithmName;
            CompressionRatio = compressionRatio;
            TimeSpan = timeSpan;
        }
    }
}