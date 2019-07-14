using System;
using System.Diagnostics.Tracing;

namespace NetCore.ConsoleApp2
{
    internal class ConsoleLogger
    {
        internal void WriteLine(string message, Exception ex, EventLevel level) => Console.WriteLine(message, ex, level);
    }
}