using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Net.Security;
using System.Threading;
using System.Threading.Tasks;
using DMC.Logging;
using Newtonsoft.Json;
using DMC.BackPlane;
using DMC.Channels.EventMessageHandler;
using DMC.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DMC.Channels.RabbitMq
{
    public class RmqChannel : ICommunicationChannel
    {
        private readonly ICacheConfig _cacheConfig;
        private readonly ICacheLogger _cacheLogger;
        private readonly ConnectionFactory _connectionFactory;
        private readonly IConnection _connection;
        private readonly EventingBasicConsumer _eventingBasicConsumer;
        private readonly CancellationToken _cancellationToken;
        private readonly IModel _subscriptionClient;
        private readonly IModel _topicClient;

        public RmqChannel(ICacheConfig cacheConfig, ICacheLogger logger)
        {
            this._cacheConfig = cacheConfig;
            this._cacheLogger = logger;
            this._connectionFactory = new ConnectionFactory()
            {
                HostName = cacheConfig.RabbitConnection.HostName,
                UserName = cacheConfig.RabbitConnection.UserName,
                Password = cacheConfig.RabbitConnection.Password,
                Port = cacheConfig.RabbitConnection.Port,
                VirtualHost = cacheConfig.RabbitConnection.VirtualHost

            };
            if (this._cacheConfig.RabbitConnection.IsSSlEnabled)
            {
                SslOption sslOption = new SslOption
                {
                    Enabled = true,
                    ServerName = cacheConfig.RabbitConnection.HostName,
                    AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateNameMismatch |
                                                   SslPolicyErrors.RemoteCertificateChainErrors,
                };

                this._connectionFactory.Ssl = sslOption;
            }
            this._connection = this._connectionFactory.CreateConnection();

            this._subscriptionClient = this._connection.CreateModel();
            this._subscriptionClient.QueueDeclare(queue: this._cacheConfig.ListenerInstanceId,
                                           durable: false,
                                           exclusive: false,
                                           autoDelete: false);
            this._eventingBasicConsumer = new EventingBasicConsumer(this._subscriptionClient);
            this._topicClient = this._connection.CreateModel();
            this._topicClient.ExchangeDeclare(exchange: this._cacheConfig.CacheBackPlaneChannel,
                                        type: "fanout",
                                         durable: false,
                                         autoDelete: false);
        }

        public Action<EventMessage, IDictionary<string, string>> OnMessage { get; set; }

        private CancellationTokenSource CancellationTokenSourceHandler { get; set; }
        public string ChannelUid => nameof(RmqChannel);

        public Task<bool> PublishEventAsync(EventMessage eventMessage)
        {
            _cacheLogger.LogAsync($" PublishMessage for RMQ channel - {JsonConvert.SerializeObject(eventMessage)}", EventLevel.Verbose);

            RabbitMqPayLoad temp = MessageEncodingHelper.GetRabbitMqMessage(eventMessage, this._topicClient.CreateBasicProperties());
            this._topicClient.BasicPublish(this._cacheConfig.CacheBackPlaneChannel, "fanout", temp.BasicProperties, temp.Data);
            this._cacheLogger.LogAsync($"Published Message on RMQ for Event Type  CorrelationId {eventMessage.CorrelationId}", EventLevel.Verbose);
            return Task.FromResult(true);
        }

        public bool Subscribe()
        {
            this._eventingBasicConsumer.Received += (o, msg) =>
            {

                try
                {
                    EventMessage eventMessage = MessageEncodingHelper.GetMessage(msg);
                    this._cacheLogger.LogAsync($"Incoming message in RMQ BackPlane Subscription Channel with CorrelationId = {eventMessage.CorrelationId} ", EventLevel.Verbose); 

                    OnMessage(eventMessage, new Dictionary<string, string>() { { nameof(BackPlaneChannelType), BackPlaneChannelType.RabbitMq.ToString() } });
                }
                catch (Exception ex)
                {
                    this._cacheLogger.LogException(ex);
                }
                this._subscriptionClient.BasicAck(msg.DeliveryTag, false);
            };


            this._subscriptionClient.QueueBind(queue: this._cacheConfig.ListenerInstanceId, exchange: this._cacheConfig.CacheBackPlaneChannel, routingKey: "fanout");
            this._subscriptionClient.BasicConsume(this._cacheConfig.ListenerInstanceId, false, this._eventingBasicConsumer);

            return true;

        }

        public bool UnSubscribe()
        {
            this.CancellationTokenSourceHandler.Cancel();
            this._subscriptionClient.Close();
            return true;
        }
    }
}
