#if NETCOREAPP2_0
using Microsoft.Azure.ServiceBus;

#endif
#if NET462
using Microsoft.ServiceBus;

#endif

using DMC.Models;

namespace DMC
{
    public interface ICacheConfig
    {
        bool BackPlaneEnabled { get; }
        string ServiceBusConnectionString { get; }
        string CacheBackPlaneChannel { get; }
#if NETCOREAPP2_0
        RetryPolicy RetryPolicy { get; }
#endif
#if NET462
        RetryPolicy RetryPolicy { get; }

#endif
        string ListenerInstanceId { get; }

        RabbitConnectionConfiguration RabbitConnection { get; }
        bool IsRabbitCommnunicationChannelEnabled { get; }
        bool IsServiceBusCommnunicationChannelEnabled { get; } 
    }
}