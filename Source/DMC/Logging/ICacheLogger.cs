using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Text;

namespace DMC.Logging
{
    public interface ICacheLogger
    {
        void LogAsync(string message, EventLevel level);
        void LogException(Exception ex);
        void Log(string message, Exception exception, EventLevel level);
    }
}
