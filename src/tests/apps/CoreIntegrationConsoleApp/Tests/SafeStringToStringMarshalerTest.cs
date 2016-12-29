using System;
using SafeOrbit.Memory;

namespace CoreIntegrationConsoleApp.Tests
{
    public class SafeStringToStringMarshalerTest : ITest
    {
        public void RunTest()
        {
            Console.WriteLine($"Testing {nameof(SafeStringToStringMarshaler)}");
            var @string = "aÄaa =)";
            var safeString = new SafeString();
            Console.WriteLine($"{nameof(SafeString)} instance is created.");
            safeString.Append(@string);
            Console.WriteLine($"{@string} is appended");
            var sut = new SafeStringToStringMarshaler();
            Console.WriteLine($"{nameof(SafeStringToStringMarshaler)} instance is created.");
            sut.SafeString = safeString;
            Console.WriteLine($"{nameof(SafeString)} is set.");
            Console.WriteLine($"Retrieved string : {sut.String}");
            sut.Dispose();
            Console.WriteLine($"Instance is disposed. Retrieved string (should be empty) : {sut.String}");
        }
    }
}