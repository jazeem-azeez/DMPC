#if NET462
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Threading;
using System.Threading.Tasks;
using DMC.Logging;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using DMC.BackPlane;
using DMC.Channels.EventMessageHandler;
using DMC.Models;

namespace DMC.Channels.ServiceBus
{
    public class ServiceBusChannel : ICommunicationChannel
    {
        private readonly ICacheConfig _cacheConfig;
        private readonly ICacheLogger _cacheLogger;
        private readonly NamespaceManager _namespaceManager;
        private readonly TopicClient _topicClient;
        private CancellationToken _cancellationToken;
        private SubscriptionClient _subscriptionClient;
        private readonly MessagingFactory _factory;
        private readonly ConcurrentDictionary<string, Task> _taskPool;

        public ServiceBusChannel(ICacheConfig cacheConfig, ICacheLogger logger)
        {
            this._cacheConfig = cacheConfig;
            this._cacheLogger = logger;
            this._namespaceManager = NamespaceManager.CreateFromConnectionString(this._cacheConfig.ServiceBusConnectionString);
            if (this._namespaceManager.TopicExistsAsync(this._cacheConfig.CacheBackPlaneChannel).GetAwaiter().GetResult() == false)
            {
                this._namespaceManager.CreateTopicAsync(this._cacheConfig.CacheBackPlaneChannel);
            }
            if (this._namespaceManager.SubscriptionExistsAsync(this._cacheConfig.CacheBackPlaneChannel, this._cacheConfig.ListenerInstanceId).GetAwaiter().GetResult() == false)
            {
                this._namespaceManager.CreateSubscriptionAsync(this._cacheConfig.CacheBackPlaneChannel, this._cacheConfig.ListenerInstanceId);
            }

            this._factory = MessagingFactory.CreateFromConnectionString(this._cacheConfig.ServiceBusConnectionString);
            this._factory.RetryPolicy = this._cacheConfig.RetryPolicy;
            this._topicClient = this._factory.CreateTopicClient(this._cacheConfig.CacheBackPlaneChannel);
            this._taskPool = new ConcurrentDictionary<string, Task>();
        }

        public string ChannelUid => nameof(ServiceBusChannel);

        public Action<EventMessage, IDictionary<string, string>> OnMessage { get; set; }


        private CancellationTokenSource CancellationTokenSourceHandler { get; set; }

        public Task<bool> PublishEventAsync(EventMessage eventMessage)
        {
            this._cacheLogger.LogAsync($" PublishMessage for SB channel - {JsonConvert.SerializeObject(eventMessage)}", EventLevel.Verbose);

            string task_uid = Guid.NewGuid().ToString() + DateTime.Now.Ticks.ToString();
            Task task = Task.Run(async () =>
            {
                await this._topicClient.SendAsync(MessageEncodingHelper.GetServiceBusMessage(eventMessage));
                this._cacheLogger.LogAsync($"Published Message on SB for Event Type  CorrelationId {eventMessage.CorrelationId}", EventLevel.Verbose);


                if (this._taskPool.TryRemove(task_uid, out Task taskOut))
                {
                    this._cacheLogger.LogAsync($"PublishEventAsync Task has been removed", EventLevel.Verbose);
                }
                else
                {
                    this._cacheLogger.LogAsync($"PublishEventAsync Task removal failed ", EventLevel.Error);
                }

            });
            return Task.FromResult(this._taskPool.TryAdd(task_uid, task));
        }

        public bool Subscribe()
        {
            this._subscriptionClient = this._factory.CreateSubscriptionClient(this._cacheConfig.CacheBackPlaneChannel,
                                        this._cacheConfig.ListenerInstanceId,
                                        ReceiveMode.PeekLock);
            this.CancellationTokenSourceHandler = new CancellationTokenSource();
            this._cancellationToken = this.CancellationTokenSourceHandler.Token;
            OnMessageOptions onMessageOptions = new OnMessageOptions()
            {
                AutoComplete = false
            };
            this._subscriptionClient.OnMessageAsync((message) =>
                {
                    try
                    {
                        EventMessage eventMessage = MessageEncodingHelper.GetMessage(message);
                        this._cacheLogger.LogAsync($"Incoming message in SB BackPlane Subscription Channel with CorrelationId = {eventMessage.CorrelationId} ", EventLevel.Verbose);

                        this.OnMessage(eventMessage, new Dictionary<string, string>() { { nameof(BackPlaneChannelType), BackPlaneChannelType.ServiceBus.ToString() } });

                    }
                    catch (Exception ex)
                    {
                        this._cacheLogger.LogException(ex);
                    }
                    this._subscriptionClient.CompleteAsync(message.LockToken);
                    return Task.CompletedTask;
                }, onMessageOptions);
            return true;
        }

        public bool UnSubscribe()
        {
            this.CancellationTokenSourceHandler.Cancel();
            this._subscriptionClient.CloseAsync().GetAwaiter().GetResult();
            this._namespaceManager.DeleteSubscriptionAsync(this._cacheConfig.CacheBackPlaneChannel, this._cacheConfig.ListenerInstanceId).GetAwaiter().GetResult();
            this._cacheLogger.LogAsync("UnSubscribed from BackPlane Subscription Channel", EventLevel.Verbose);
            return true;
        }


    }
}
#endif