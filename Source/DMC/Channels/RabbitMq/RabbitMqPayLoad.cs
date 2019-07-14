using System.Collections.Generic;
using RabbitMQ.Client;

namespace DMC.Channels.RabbitMq
{
    public class RabbitMqPayLoad
    { 

        public RabbitMqPayLoad(byte[] data, IBasicProperties basicProperties)
        {
            this.Data = data;
            BasicProperties = basicProperties;
            BasicProperties.Headers = new Dictionary<string, object> { };
        }

        public byte[] Data { get; internal set; }
        public IBasicProperties BasicProperties { get; internal set; }
    }
}