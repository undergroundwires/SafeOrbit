using System;
using SafeOrbit.Memory;

namespace CoreIntegrationConsoleApp.Tests
{
    public class SafeStringTest : ITest
    {
        public void RunTest()
        {
            Console.WriteLine($"Testing {nameof(SafeString)}");
            var @string = "aAaa =)";
            var safeString = new SafeString();
            safeString.Append(@string);
            using (var sm = new SafeStringToStringMarshaler())
            {
                Console.WriteLine(sm.String);
            }
        }
    }
}