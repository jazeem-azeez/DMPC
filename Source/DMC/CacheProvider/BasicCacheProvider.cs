using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;
using DMC.BackPlane;
using DMC.CacheContext;
using DMC.CacheProviders;
using DMC.Logging;
using DMC.Models;
using Newtonsoft.Json;

namespace DMC.Implementations
{
    public class BasicCacheProvider<T> : ICacheProvider<T>
    {
      
        private volatile ConcurrentDictionary<string, bool> _operationsInProgress = new ConcurrentDictionary<string, bool>();
        private volatile NonLockingRuntimeWrapper<T> _nonLockingRuntimeWrapperForCallBacks; 
        private readonly IBackPlane _backPlane; 
        private readonly SortedDictionary<int, ICacheStores<T>> _cacheStoreCollection;
        private readonly ICacheConfig _cacheConfig;

        private readonly List<ICommunicationChannel> _channels;
        private readonly ICacheLogger _cacheLogger;
        private readonly IBaseCacheContextManager _cacheContextManager;

        public BasicCacheProvider(SortedDictionary<int,ICacheStores<T>> cacheStoreCollection, ICacheConfig cacheConfig, IBackPlane backPlane, ICacheLogger logger, IBaseCacheContextManager cacheContextManager)
        {
            this._cacheLogger = logger;
            this._cacheContextManager = cacheContextManager;
            this._cacheStoreCollection = cacheStoreCollection;
            this._cacheConfig = cacheConfig;
            this.FilterName = typeof(T).FullName;
            this._backPlane = backPlane;
            if (this._cacheConfig.BackPlaneEnabled)
            {
                this._backPlane.SubscribeToBackPlanEvents<T>(this.FilterName, this.OnBackPlaneEvent);
            }
            this._cacheLogger = logger;
        }

        public string FilterName { get; }

        public bool Compact() => throw new NotSupportedException();

        public void ConnectToBackPlane() => this._backPlane.SubscribeToBackPlanEvents<T>(this.FilterName, this.OnBackPlaneEvent);

        public void DisconnectFromBackPlane() => this._backPlane.UnSubscribeToBackPlanEvents<T>(this.FilterName);

        public Task DispatchEvent(EventMessage eventMessage) => this._backPlane.DispatchEventMessageAsync(eventMessage); 

        public TimeSpan GetDefaultExpiry() => new TimeSpan(0, 58, 0);

        public T GetOrSet(string key, Func<T> getItemCallBack) => this.GetOrSet(key, getItemCallBack, null);

        public T GetOrSet(string key, Func<T> getItemCallBack, TimeSpan? expiry)
            => this.GetOrSetAsync(key, () => { return Task.FromResult(getItemCallBack()); }, expiry)
                    .GetAwaiter().GetResult();
        public Task<T> GetOrSetAsync(string key, Func<Task<T>> getItemCallBack) => this.GetOrSetAsync(key, getItemCallBack, null);

        public async Task<T> GetOrSetAsync(string key, Func<Task<T>> getItemCallBack, TimeSpan? expiry)
        {

            this._cacheLogger.LogAsync($"GetOrSetAsync for  {key}", EventLevel.Verbose);
            T item = default(T);
            try
            {
                item = this.Get(key);

                bool? shouldByPassItemCheck = !this._cacheContextManager?.CacheContext?.IsCacheEnabled;
                shouldByPassItemCheck = shouldByPassItemCheck.HasValue == true ? shouldByPassItemCheck.Value : false;

                this._cacheLogger.LogAsync($"GetOrSetAsync - shouldByPassItemCheck {shouldByPassItemCheck}", EventLevel.Verbose);


                if (item == null || shouldByPassItemCheck.Value == true)
                {
                    if (await this.CheckAndWaitIfOperationInProgressAsync(key) && shouldByPassItemCheck.Value == false)
                    {
                        item = this.Get(key);
                        if (item != null)
                        {
                            return item;
                        }
                    }
                    this._cacheLogger.LogAsync($"GetOrSetAsync - Get returned null for key {key}", EventLevel.Verbose);

                    item = getItemCallBack().GetAwaiter().GetResult();

                    if (item != null)
                    {
                        this.Set(key, item, expiry);
                    }
                }

            }
            catch (Exception ex)
            {
                this._cacheLogger.LogException(ex);
                throw;
            }
            finally
            {
                _operationsInProgress.TryRemove(key, out bool val);
            }
            return item;
        }



        private async Task<bool> CheckAndWaitIfOperationInProgressAsync(string key)
        {
            bool hasWaited = false;
            while (_operationsInProgress.TryAdd(key, true) == false)
            {
                hasWaited = true;
                await Task.Delay(5);
            }
            return hasWaited;
        }


        public bool Invalidate(string key)
                => this.Invalidate(new List<string> { key }, new List<string> { this.FilterName });
        public bool Invalidate(List<string> keys)
                => this.Invalidate(keys, new List<string> { this.FilterName });

        public bool Invalidate(List<string> keys, List<string> filters)
        {
            this._cacheLogger.LogAsync($"Invalidating {keys} - Invalidate Method", EventLevel.Verbose);

            if (this._cacheConfig.BackPlaneEnabled)
            {
                EventMessage eventMessage = new EventMessage
                {
                    EventFilterCollection = filters,
                    EventKeyCollection = keys,
                    EventType = CacheEventTypes.Invalidated,
                    EventTimeStamp = DateTime.Now
                };
                this.DispatchEvent(eventMessage).GetAwaiter().GetResult();
            }

            this._cacheLogger.LogAsync($"Removing entry - Invalidate Method", EventLevel.Verbose);

            foreach (string key in keys)
            {
                this.RemoveEntry(key);
            }

            return true;
        }

        public void OnBackPlaneEvent(EventMessage eventMessage)
        {
            this._cacheLogger.LogAsync($" {eventMessage.EventType.ToString()} Event Arrived for cache Provider {this.FilterName} from Backplane ", EventLevel.Verbose);

            switch (eventMessage.EventType)
            {
                case CacheEventTypes.Invalidated:
                    {
                        foreach (string item in eventMessage.EventKeyCollection)
                        {
                            this._cacheLogger.LogAsync($" Removing Entry {item} ", EventLevel.Verbose);

                            this.RemoveEntry(item);
                        }
                        break;
                    }
                case CacheEventTypes.Updated:
                    {
                        TimeSpan? expiry = ComputeExpiry(eventMessage);
                        foreach (string item in eventMessage.EventKeyCollection)
                        {
                            this._cacheLogger.LogAsync($" Updating Entry {item} ", EventLevel.Verbose); try
                            {
                                T newValue = JsonConvert.DeserializeObject<T>(eventMessage.EventData);
                                this.Set(item, newValue, expiry);
                            }
                            catch (Exception ex)
                            {

                                this._cacheLogger.LogException(ex);
                            }
                        }

                        break;
                    }
                default:
                    this._cacheLogger.LogAsync("Unknown Event type ", EventLevel.Warning);
                    break;
            }

            return;
        }

        public bool Set(string key, T value, TimeSpan? expiry)
        {
            this._cacheLogger.LogAsync($"Setting {key}, {value} with expiry {expiry} - Set Method", EventLevel.Verbose);

            try
            {
             
            }
            catch (Exception ex)
            {
                this._cacheLogger.LogException(ex);
            }


            return true;
        }

        public bool Update(string key, T data, TimeSpan? expiry = null, T oldData = default(T))
                    => this.Update(new List<string> { key }, new List<string> { this.FilterName }, data, expiry, oldData);
        public bool Update(List<string> keys, T data, TimeSpan? expiry = null, T oldData = default(T))
                    => this.Update(keys, new List<string> { this.FilterName }, data, expiry, oldData);

        private bool Update(List<string> keys, List<string> filters, T data, TimeSpan? expiry = null, T oldData = default(T))
        {
            if (this._cacheConfig.BackPlaneEnabled)
            {
                this._cacheLogger.LogAsync($"Tryin to Update via Backplane {keys}  with {data} & expiry {expiry} - Update Method", EventLevel.Verbose);
                double expiryInMins = expiry.HasValue ? expiry.Value.TotalMinutes : 0;
                EventMessage temp = new EventMessage
                {
                    EventFilterCollection = filters,
                    EventKeyCollection = keys,
                    EventData = JsonConvert.SerializeObject(data),
                    OldEventData = JsonConvert.SerializeObject(oldData),
                    EventTimeStamp = DateTime.Now,
                    ExpiryInMins = (int)expiryInMins,
                    EventType = CacheEventTypes.Updated
                };
                this.DispatchEvent(temp).GetAwaiter().GetResult();
            }

            this._cacheLogger.LogAsync($"Tryin to Update directly {keys}  with {data} & expiry {expiry} - Update Method", EventLevel.Verbose);
            foreach (string key in keys)
            {
                this.Set(key, data, expiry);
            }

            return true;
        }

        private static TimeSpan? ComputeExpiry(EventMessage eventMessage)
        {
            int expiryInMins = (int)eventMessage.ExpiryInMins;

            TimeSpan? expiry = expiryInMins > 0 ? new TimeSpan(0, expiryInMins, 0) : default(TimeSpan);
            if (expiry.HasValue && expiry.Value != default(TimeSpan))
            {
                expiry = expiry.Value.Subtract(DateTime.Now.Subtract(eventMessage.EventTimeStamp));
            }

            return expiry;
        }

        private void RemoveEntry(string key)
        {
            this._cacheLogger.LogAsync($"Removing entry with Key {key}", EventLevel.Verbose);
             bool temp = false;

            try
            {
             
            }
            catch (Exception ex)
            {
                this._cacheLogger.LogException(ex);
            }
         }

        public T Get(string key) => throw new NotImplementedException();
    }
}