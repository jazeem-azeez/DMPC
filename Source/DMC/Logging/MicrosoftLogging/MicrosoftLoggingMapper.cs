using System;
using System.Diagnostics.Tracing;
using Microsoft.Extensions.Logging;

namespace DMC.Logging.ItronLogging
{
    public class MicrosoftLoggingMapper : ICacheLogger
    {
        private readonly ILogger _cacheLogger;

        public MicrosoftLoggingMapper(ILogger logger) => this._cacheLogger = logger;

        public void LogAsync(string message, EventLevel level) => this.WriteToLogger(message, level, null);

        public void Log(string message, Exception exception, EventLevel level) => this.WriteToLogger(message, level, exception);

        public void LogException(Exception ex) => this.WriteToLogger(ex.Message, EventLevel.Error, ex);

        private void WriteToLogger(string message, EventLevel eventLevel, Exception exception)
        {
            switch (eventLevel)
            {
                case EventLevel.Critical:
                    {
                        Exception ex = exception ?? new Exception("exception Type not defined : possible reason wrong event level in log method ");
                        this._cacheLogger.LogCritical(ex, message);
                        break;
                    }

                case EventLevel.Error:
                    {
                        Exception ex = exception ?? new Exception("exception Type not  defined: possible reason wrong event level in log method");
                        this._cacheLogger.LogError(ex, message);
                        break;
                    }

                case EventLevel.LogAlways:
                case EventLevel.Informational:
                    {
                        this._cacheLogger.LogInformation(message);
                        break;
                    }

                case EventLevel.Verbose:
                    {
                        this._cacheLogger.LogDebug(message);
                        break;
                    }

                case EventLevel.Warning:
                    {
                        this._cacheLogger.LogDebug(message);
                        break;
                    }

                default:
                    this._cacheLogger.LogError($"unknown event Level {eventLevel} {message}");
                    break;
            }
        }
    }
}