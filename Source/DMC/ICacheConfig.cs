using Microsoft.Azure.ServiceBus;
 
using DMC.Models;

namespace DMC
{
    public interface ICacheConfig
    {
        bool BackPlaneEnabled { get; }
        string ServiceBusConnectionString { get; }
        string CacheBackPlaneChannel { get; } 
        RetryPolicy RetryPolicy { get; } 
        string ListenerInstanceId { get; }

        RabbitConnectionConfiguration RabbitConnection { get; }
        bool IsRabbitCommnunicationChannelEnabled { get; }
        bool IsServiceBusCommnunicationChannelEnabled { get; }
        string RedisConnectionString { get; }
    }
}