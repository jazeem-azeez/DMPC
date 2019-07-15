using System;
using System.Diagnostics.Tracing;

namespace NetCore.ConsoleApp1
{
    public class ConsoleLogger
    {
        public void WriteLine(string message, Exception ex, EventLevel level)
        {
            try
            {

                Console.WriteLine($"message:{message}, ex:{ex}, level:{level}");
            }
            catch (Exception ex1)
            {
                Console.WriteLine(message);
                Console.WriteLine($"{ex1}");
            }
        }
    }
}