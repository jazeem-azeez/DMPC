using System;
using System.Diagnostics.Tracing;

namespace DMC.Logging.LoggingBinder
{
    public class DynamicLoggingWrapper<T> : ICacheLogger
    {
        private readonly T _cacheLogger;
        private readonly Func<T, string, Exception, EventLevel, bool> _logWritterCallBack;

        public DynamicLoggingWrapper(T logger, Func<T, string, Exception, EventLevel, bool> logWritterCallBack)
        {
            this._cacheLogger = logger;
            this._logWritterCallBack = logWritterCallBack;
        }
         
        public void LogAsync(string message, EventLevel level) => this._logWritterCallBack(_cacheLogger,message, default(Exception), level);
        public void Log(string message, Exception exception, EventLevel level) => this._logWritterCallBack(_cacheLogger, message, exception, level);
        public void LogException(Exception ex) => this._logWritterCallBack(_cacheLogger, ex.Message, ex, EventLevel.Critical);
    }
}
