using System.Collections.Generic;
using DMC.Logging;
using DMC.Channels.RabbitMq;
using DMC.Channels.ServiceBus;

namespace DMC.Channels.ChannelFactory
{
    public class CommunicationChannelFactory : ICommunicationChannelFactory
    {
        private readonly ICacheConfig _cacheConfig;
        private readonly ICacheLogger _cacheLogger;
        private readonly List<ICommunicationChannel> channelCollection;

        public CommunicationChannelFactory(ICacheConfig cacheConfig, ICacheLogger logger)
        {
            this._cacheConfig = cacheConfig;
            this._cacheLogger = logger;
            this.channelCollection = new List<ICommunicationChannel>();
            if (this._cacheConfig.IsRabbitCommnunicationChannelEnabled)
            {
                RmqChannel item = new RmqChannel(this._cacheConfig, this._cacheLogger);
                item.Subscribe();
                this.channelCollection.Add(item);
            }
            if (this._cacheConfig.IsServiceBusCommnunicationChannelEnabled)
            {
                ServiceBusChannel item1 = new ServiceBusChannel(this._cacheConfig, this._cacheLogger);
                item1.Subscribe();
                this.channelCollection.Add(item1);
            }
        }

        public List<ICommunicationChannel> GetActiveChannels() => this.channelCollection;
    }
}