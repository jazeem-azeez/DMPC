using System;
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
    public class CacheProviderV1<T> : ICacheProvider<T>
    {
        //private readonly bool autoPropogateOrCachingEnabled;
        private readonly IBackPlane _backPlane;

        private readonly ICacheConfig _cacheConfig;
        private readonly IBaseCacheContextManager _cacheContextManager;
        private readonly ICacheLogger _cacheLogger;
        private readonly SortedDictionary<int, ICacheStores<T>> _cacheStoreCollection;
        private readonly NonLockingRuntimeWrapper<T> _nonLockingRuntimeWrapperForCallBacks;
        private readonly int maxStoreIndex;
        private readonly int minStoreIndex;

        public CacheProviderV1(IStoreCollectionProvider<T> storeCollectionProvider,
                                ICacheConfig cacheConfig,
                                IBackPlane backPlane,
                                ICacheLogger logger,
                                IBaseCacheContextManager cacheContextManager)
        {
            this._cacheLogger = logger;
            this._cacheContextManager = cacheContextManager;
            this._cacheStoreCollection = storeCollectionProvider.GetCacheStoreCollection();
            this.minStoreIndex = 0;
            this.maxStoreIndex = this._cacheStoreCollection.Count;
            this._cacheConfig = cacheConfig;
            this.FilterName = typeof(T).FullName;
            this._backPlane = backPlane;
            this._nonLockingRuntimeWrapperForCallBacks = new NonLockingRuntimeWrapper<T>(this._cacheLogger);
            if (this._cacheConfig.BackPlaneEnabled)
            {
                this._backPlane.SubscribeToBackPlanEvents<T>(this.FilterName, this.OnBackPlaneEvent);
            }
            this._cacheLogger = logger;
        }

        public string FilterName { get; }

        public bool IsCacheEnabled
        {
            get
            {
                bool? val = this._cacheContextManager?.CacheContext?.IsCacheEnabled;
                return val.HasValue ? val.Value : true;
            }
        }

        public bool AutoPropogateOrCachingEnabled
        {
            get
            {
                bool? val = this._cacheContextManager?.CacheContext?.AutoPropogateOrCachingEnabled;
                return val.HasValue ? val.Value : true;
            }
        }

        public bool Compact() => throw new NotSupportedException();

        public void ConnectToBackPlane() => this._backPlane.SubscribeToBackPlanEvents<T>(this.FilterName, this.OnBackPlaneEvent);

        public void DisconnectFromBackPlane() => this._backPlane.UnSubscribeToBackPlanEvents<T>(this.FilterName);

        public Task DispatchEvent(EventMessage eventMessage) => this._backPlane.DispatchEventMessageAsync(eventMessage);

        public T Get(string key, bool autoPropogateOrCachingEnabled = true)
        {
            StoreWrapper<T> temp = this.Get(key, this.minStoreIndex, autoPropogateOrCachingEnabled);
            return temp != default(StoreWrapper<T>) ? temp.Data : default(T);
        }

        public T Get(string key) => this.Get(key, this.AutoPropogateOrCachingEnabled);

        public StoreWrapper<T> Get(string key, int level, bool autoPropogateOrCachingEnabled)
        {
            StoreWrapper<T> storeWrapper = default(StoreWrapper<T>);
            if (level < this.maxStoreIndex)
            {
                storeWrapper = this._cacheStoreCollection[level].GetEntry(key);
                if (storeWrapper == default(StoreWrapper<T>))
                {
                    storeWrapper = this.Get(key, level + 1, autoPropogateOrCachingEnabled);
                    if (autoPropogateOrCachingEnabled && storeWrapper != default(StoreWrapper<T>))
                    {
                        this._cacheStoreCollection[level].SetEntry(key, storeWrapper);
                    }
                }
            }

            return storeWrapper;
        }

        public T GetOrSet(string key, Func<T> getItemCallBack, bool autoPropogateOrCachingEnabled) => this.GetOrSet(key, getItemCallBack, null, autoPropogateOrCachingEnabled);

        public T GetOrSet(string key, Func<T> getItemCallBack, TimeSpan? expiry, bool autoPropogateOrCachingEnabled)
            => this.GetOrSetAsync(key, () => { return Task.FromResult(getItemCallBack()); }, expiry, autoPropogateOrCachingEnabled)
                    .GetAwaiter().GetResult();

        public Task<T> GetOrSetAsync(string key, Func<Task<T>> getItemCallBack) => this.GetOrSetAsync(key, getItemCallBack, null, this.AutoPropogateOrCachingEnabled);
        public Task<T> GetOrSetAsync(string key, Func<Task<T>> getItemCallBack, bool autoPropogateOrCachingEnabled) => this.GetOrSetAsync(key, getItemCallBack, null, autoPropogateOrCachingEnabled);

        public async Task<T> GetOrSetAsync(string key, Func<Task<T>> getItemCallBack, TimeSpan? expiry, bool autoPropogateOrCachingEnabled)
        {
            this._cacheLogger.LogAsync($"GetOrSetAsync for  {key}", EventLevel.Verbose);
            T item = default(T);
            try
            {
                item = this.Get(key, autoPropogateOrCachingEnabled);

                bool isCacheEnabled = this.IsCacheEnabled;

                this._cacheLogger.LogAsync($"GetOrSetAsync - shouldByPassItemCheck {isCacheEnabled}", EventLevel.Verbose);

                if (item == null || isCacheEnabled == true)
                {
                    item = await this._nonLockingRuntimeWrapperForCallBacks.ThreadSafefWrapperAsync(key, async (handle) =>
                    {
                        // double checking to avoid race conditions
                        if (isCacheEnabled == true)
                        {
                            item = this.Get(key, autoPropogateOrCachingEnabled);
                            if (item != null)
                            {
                                return item;
                            }
                        }
                        this._cacheLogger.LogAsync($"GetOrSetAsync - Get returned null for key {key}", EventLevel.Verbose);

                        item = await getItemCallBack();
                        return item;
                    });

                    if (item != null)
                    {
                        this.Set(key, item, expiry, this.minStoreIndex, autoPropogateOrCachingEnabled);
                    }
                }
            }
            catch (Exception ex)
            {
                this._cacheLogger.LogException(ex);
                throw;
            }
            return item;
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
                this.RemoveEntry(key, this.minStoreIndex, true);
            }

            return true;
        }

        public bool Invalidate(List<string> keys, int level) => throw new NotImplementedException();

        public bool Invalidate(List<string> keys, List<string> filters, int level) => throw new NotImplementedException();

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
                                this.Set(item, newValue, expiry, this.minStoreIndex, false);
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

        public bool Set(string key, T value, TimeSpan? expiry) => this.Set(key, value, expiry, this.minStoreIndex, this.AutoPropogateOrCachingEnabled);

        public bool Set(string key, T value, TimeSpan? expiry, int level, bool autoPropogateOrCachingEnabled)
        {
            this._cacheLogger.LogAsync($"Setting {key}, {value} with expiry {expiry} - Set Method", EventLevel.Verbose);

            try
            {
                if (level < this.maxStoreIndex)
                {
                    this._cacheStoreCollection[level].SetEntry(key, StoreWrapper<T>.GetStoreWrapperEntry(value, expiry));
                    if (autoPropogateOrCachingEnabled)
                    {
                        this.Set(key, value, expiry, level + 1, autoPropogateOrCachingEnabled);
                    }
                }
            }
            catch (Exception ex)
            {
                this._cacheLogger.LogException(ex);
            }

            return true;
        }

        public bool Update(string key, T data, TimeSpan? expiry = null, T oldData = default(T), bool autoPropogateOrCachingEnabled = true)
                    => this.Update(new List<string> { key }, new List<string> { this.FilterName }, data, expiry, oldData, autoPropogateOrCachingEnabled);

        public bool Update(List<string> keys, T data, TimeSpan? expiry = null, T oldData = default(T), bool autoPropogateOrCachingEnabled = true)
                    => this.Update(keys, new List<string> { this.FilterName }, data, expiry, oldData, autoPropogateOrCachingEnabled);

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

        private async Task<bool> CheckAndWaitIfOperationInProgressAsync(string key)
        {
            bool hasWaited = false;
            // while (_operationsInProgress.TryAdd(key, true) == false)
            {
                hasWaited = true;
                await Task.Delay(5);
            }
            return hasWaited;
        }

        private void RemoveEntry(string key, int level = 0, bool autoPropogateOrCachingEnabled = false)
        {
            this._cacheLogger.LogAsync($"Removing entry with Key {key}", EventLevel.Verbose);
            bool temp = false;
            if (level < this.maxStoreIndex)
            {
                this._cacheStoreCollection[level].RemoveEntry(key);
                if (autoPropogateOrCachingEnabled)
                {
                    this.RemoveEntry(key, level + 1);
                }
            }
        }

        private bool Update(List<string> keys, List<string> filters, T data, TimeSpan? expiry = null, T oldData = default(T), bool autoPropogateOrCachingEnabled = true)
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
                this.Set(key, data, expiry, this.minStoreIndex, true);
            }

            return true;
        }
    }
}