using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LoaderExceptionCatcherApp
{
    class Program
    {
        static void Main(string[] args)
        {
            bool errorsFound = false;
            try
            {
                var types = Assembly.GetExecutingAssembly().GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                errorsFound = true;
                foreach (var item in ex.LoaderExceptions)
                {
                    Console.WriteLine(item.Message);
                }
            }
            if (!errorsFound)
            {
                Console.WriteLine("No type load errors are caught.");
            }
            Console.ReadKey();
        }
    }
}
