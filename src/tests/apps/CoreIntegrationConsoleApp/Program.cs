using System;
using System.Collections.Generic;
using CoreIntegrationConsoleApp.Tests;
using SafeOrbit.Cryptography.Encryption;
using SafeOrbit.Library;
using SafeOrbit.Memory;

namespace CoreIntegrationConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SafeOrbitCore.Current.StartEarly();
            var tests = GetAllTests();
            foreach (var test in tests)
            {
                test.RunTest();
                Console.WriteLine("----------------------");
            }
            Console.ReadKey();
        }

        private static IEnumerable<ITest> GetAllTests()
        {
            yield return new BlowfishEncryptorTest();
            yield return new SafeStringTest();
            yield return new SafeStringToStringMarshalerTest();
        }
    }
}
