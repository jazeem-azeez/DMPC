using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;
using DMC.Logging;
using DMC.Channels.ChannelFactory;
using DMC.Models;

namespace DMC.BackPlane
{
    public class BaseBackPlane : IBackPlane
    {
        private readonly ConcurrentDictionary<string, ICommunicationChannel> _channels;
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, Action<EventMessage>>> _onEventHandlers;
        private readonly ICacheLogger _cacheLogger;
        private readonly ICacheConfig _cacheConfig;

        public string ListenerInstanceId { get; set; }

        public BaseBackPlane(ICommunicationChannelFactory channelFactory, ICacheLogger logger, ICacheConfig cacheConfig)
        {
            this._cacheLogger = logger;
            this._cacheConfig = cacheConfig;
            this.ListenerInstanceId = this._cacheConfig.ListenerInstanceId;
            this._channels = new ConcurrentDictionary<string, ICommunicationChannel>();
            this._onEventHandlers = new ConcurrentDictionary<string, ConcurrentDictionary<string, Action<EventMessage>>>();
            if (_cacheConfig.BackPlaneEnabled==true)
            {

                foreach (ICommunicationChannel item in channelFactory.GetActiveChannels())
                {
                    if (this._channels.TryAdd(item.ChannelUid, item))
                    {
                        this._channels[item.ChannelUid].OnMessage = this.OnEvent;
                        this._cacheLogger.LogAsync($"Bindng to Communication Channel {item.ChannelUid} Completed", EventLevel.Verbose);
                    }
                    else
                    {
                        this._cacheLogger.LogAsync($"Bindng to Communication Channel {item.ChannelUid} Failed", EventLevel.Error);
                    }
                } 
            }
            this._cacheLogger.LogAsync("Bindng to Communication Channels Completed", EventLevel.Verbose);
        }

        public void AddCommunicationChannel(ICommunicationChannel channel) => this._channels.TryAdd(channel.ChannelUid, channel);

        public Task DispatchEventMessageAsync(EventMessage eventMessage) => Task.Run(() =>
                {
                    eventMessage.CorrelationId = string.IsNullOrEmpty(eventMessage.CorrelationId) ? Guid.NewGuid().ToString() : eventMessage.CorrelationId;
                    eventMessage.EventHeaders.Add(nameof(this.ListenerInstanceId), this.ListenerInstanceId);
                    Parallel.ForEach(this._channels, (async item => { await item.Value.PublishEventAsync(eventMessage); }));
                });

        /// <summary>
        /// Called when [event].
        /// </summary>
        /// <param name="eventMessage">The event message.</param>
        /// <param name="headers">The headers.</param>
        public void OnEvent(EventMessage eventMessage, IDictionary<string, string> headers)
        {
            if (eventMessage.EventHeaders.ContainsKey(nameof(this.ListenerInstanceId)) && eventMessage.EventHeaders[nameof(this.ListenerInstanceId)] == this.ListenerInstanceId)
            {
                // Ignore this event and move on - loop back effect
                this._cacheLogger.LogAsync($" in Backplane OnEventHandlers {ListenerInstanceId}, ignoring this event as ListenerInstanceId = {ListenerInstanceId}", EventLevel.Verbose);

                return;
            }

            try
            {
                this._cacheLogger.LogAsync(" Backplane OnEventHandlers CallBacks - Listing Keys ", EventLevel.Verbose);
                foreach (string item in this._onEventHandlers.Keys)
                {
                    this._cacheLogger.LogAsync($"Key {item}", EventLevel.Verbose);
                }

                foreach (KeyValuePair<string, string> item in headers)
                {
                    if (eventMessage.EventHeaders.ContainsKey(item.Key) == false)
                    {
                        eventMessage.EventHeaders.Add(item.Key, item.Value);
                    }
                }

                foreach (string eventFilterCollectionItem in eventMessage.EventFilterCollection)
                {
                    if (this._onEventHandlers.ContainsKey(eventFilterCollectionItem))
                    {
                        foreach (KeyValuePair<string, Action<EventMessage>> onEventHandlersItem in this._onEventHandlers[eventFilterCollectionItem])
                        {
                            this._cacheLogger.LogAsync($" Invoking OnEventHandlers CallBacks from collection - for {eventFilterCollectionItem}", EventLevel.Verbose);

                            onEventHandlersItem.Value(eventMessage);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this._cacheLogger.LogException(ex);
            }
        }

        public void RemoveCommunicationChannel(ICommunicationChannel channel) => this._channels.TryRemove(channel.ChannelUid, out ICommunicationChannel value);

        public void SubscribeToBackPlanEvents<T>(string filter, Action<EventMessage> onBackPlaneEvent)
        {
            this._cacheLogger.LogAsync($"Registering On EventHandler for filter {filter}", EventLevel.Verbose);

            if (this._onEventHandlers.ContainsKey(filter) == false)
            {
                this._onEventHandlers.TryAdd(filter, new ConcurrentDictionary<string, Action<EventMessage>>());
            }

            this._onEventHandlers[filter].TryAdd((filter + typeof(T).FullName), onBackPlaneEvent);
        }

        public void UnSubscribeToBackPlanEvents<T>(string filter)
        {
            this._cacheLogger.LogAsync($"Registering On EventHandler for filter {filter}", EventLevel.Verbose);

            if (this._onEventHandlers.ContainsKey(filter) == true)
            {
                this._onEventHandlers[filter].TryRemove((filter + typeof(T).FullName), out Action<EventMessage> value);
            }
        }
    }
}