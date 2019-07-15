using System;
using System.Diagnostics;
using Microsoft.Azure.ServiceBus;
using DMC;
using DMC.Models;

namespace NetCore.ConsoleApp1
{
    public class CacheConfig : ICacheConfig
    {
        public bool BackPlaneEnabled => true;
        public string ServiceBusConnectionString => @"Endpoint=sb://sb-use-nfy-dev-ss.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=cwVNrtJ1P+CCaGt3sWDkV/zr61pRTyIVMhtpHWq/sLI=";
        public string CacheBackPlaneChannel => nameof(CacheBackPlaneChannel);
        public RetryPolicy RetryPolicy => new RetryExponential(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(30), 5);
        public string ListenerInstanceId => Environment.MachineName + "NetCore.ConsoleApp1";

        public RabbitConnectionConfiguration RabbitConnection
        {
            get => new RabbitConnectionConfiguration
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/",
                Port = 5672,
            };
        }
        public bool IsRabbitCommnunicationChannelEnabled { get=>true; }
        public bool IsServiceBusCommnunicationChannelEnabled { get=>false; }
        public string RedisConnectionString => "localhost";
    }
}