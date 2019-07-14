#if NETCOREAPP2_0
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Threading;
using System.Threading.Tasks;
using DMC.Logging;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
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
        private readonly ITopicClient _topicClient;
        private CancellationToken _cancellationToken;
        private ISubscriptionClient _subscriptionClient;
        private readonly ManagementClient _managementClient;
        private ConcurrentDictionary<string, Task> _taskPool;

        public ServiceBusChannel(ICacheConfig cacheConfig, ICacheLogger logger)
        {
            _cacheConfig = cacheConfig;
            _cacheLogger = logger;
            _managementClient = new ManagementClient(_cacheConfig.ServiceBusConnectionString);
            if (_managementClient.TopicExistsAsync(_cacheConfig.CacheBackPlaneChannel).GetAwaiter().GetResult() == false)
            {
                _managementClient.CreateTopicAsync(_cacheConfig.CacheBackPlaneChannel);
            }
            if (_managementClient.SubscriptionExistsAsync(_cacheConfig.CacheBackPlaneChannel, _cacheConfig.ListenerInstanceId).GetAwaiter().GetResult() == false)
            {
                _managementClient.CreateSubscriptionAsync(_cacheConfig.CacheBackPlaneChannel, _cacheConfig.ListenerInstanceId);
            }

            _topicClient = new TopicClient(_cacheConfig.ServiceBusConnectionString, _cacheConfig.CacheBackPlaneChannel, _cacheConfig.RetryPolicy);
            _taskPool = new ConcurrentDictionary<string, Task>();
        }

        public string ChannelUid { get=>nameof(ServiceBusChannel); }

        public Action<EventMessage, IDictionary<string, string>> OnMessage { get; set; }


        private CancellationTokenSource CancellationTokenSourceHandler { get; set; }

        public  Task<bool> PublishEventAsync(EventMessage eventMessage)
        {
            _cacheLogger.LogAsync($" PublishMessage for SB channel - {JsonConvert.SerializeObject(eventMessage)}",EventLevel.Verbose);
            var task_uid = Guid.NewGuid().ToString()+DateTime.Now.Ticks.ToString();
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
            return Task.FromResult( _taskPool.TryAdd(task_uid,task));
        }

        public bool Subscribe()
        {



            _subscriptionClient = new SubscriptionClient(_cacheConfig.ServiceBusConnectionString,
                                    _cacheConfig.CacheBackPlaneChannel,
                                    _cacheConfig.ListenerInstanceId,
                                    ReceiveMode.PeekLock,
                                    _cacheConfig.RetryPolicy);
            CancellationTokenSourceHandler = new CancellationTokenSource();
            _cancellationToken = CancellationTokenSourceHandler.Token;
            MessageHandlerOptions onMessageOptions = new MessageHandlerOptions(ErrorCallback)
            {
                AutoComplete = false
            };
            _subscriptionClient.RegisterMessageHandler((message, token) =>
            {
                try
                {
                    EventMessage eventMessage = MessageEncodingHelper.GetMessage(message);
                    _cacheLogger.LogAsync($"Incoming message in SB BackPlane Subscription Channel with CorrelationId = {eventMessage.CorrelationId} ", EventLevel.Verbose);
                    OnMessage(eventMessage, new Dictionary<string, string>() { { nameof(BackPlaneChannelType), BackPlaneChannelType.ServiceBus.ToString() } });
                }
                catch (Exception ex)
                {
                    _cacheLogger.LogException(ex);
                }
                _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
                return Task.CompletedTask;
            }, onMessageOptions);
            return true;
        }

        public bool UnSubscribe()
        {
            CancellationTokenSourceHandler.Cancel();
            _subscriptionClient.CloseAsync().GetAwaiter().GetResult();
            _managementClient.DeleteSubscriptionAsync(_cacheConfig.CacheBackPlaneChannel, _cacheConfig.ListenerInstanceId).GetAwaiter().GetResult();
            _cacheLogger.LogAsync("UnSubscribed from BackPlane Subscription Channel", EventLevel.Verbose);
            return true;
        }

        private Task ErrorCallback(ExceptionReceivedEventArgs arg)
        {
            _cacheLogger.Log(arg.ExceptionReceivedContext.ToString(), arg.Exception, EventLevel.Error);
            return Task.CompletedTask;
        }
    }
}
#endif