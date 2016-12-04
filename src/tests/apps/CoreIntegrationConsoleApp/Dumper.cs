using System;

namespace CoreIntegrationConsoleApp
{
    public static class Dumper
    {
        public static string Dump(this byte[] ba)
        {
            var hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }
    }
}