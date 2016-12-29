using System;
using SafeOrbit.Memory;

namespace CoreIntegrationConsoleApp.Tests
{
    public class SafeStringTest : ITest
    {
        public void RunTest()
        {
            Console.WriteLine($"Testing {nameof(SafeString)}");
            var @string = "aÄaa =)";
            var safeString = new SafeString();
            Console.WriteLine("Instance created.");
            safeString.Append(@string);
            Console.WriteLine($"{@string} is appended");
            Console.Write("Retrieving chars (GetAsChar) :");
            for (var i = 0; i < @string.Length; i++)
            {
                Console.Write($"{safeString.GetAsChar(i)}");
            }
            Console.WriteLine();
        }
    }
}