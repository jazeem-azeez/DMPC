using System;
using System.Collections.Generic;
using System.Linq;
using DMC.Implementations;
using DMC.Logging;
using DMC.Models;

using StackExchange.Redis;

namespace DMC.CacheProvider.CacheStores
{
    internal class RedisStore<T> : ICacheStores<T>
    {
        /// <summary>  </summary>
        private readonly IDatabase DB;
        private readonly ICacheConfig _cacheConfig;
        private readonly ICacheLogger _cacheLogger;
        private readonly NonLockingRuntimeWrapper<T> _nonLockingRuntimeWrapper;
        public RedisStore(IRedisFactory redisFactory, ICacheConfig cacheConfig, ICacheLogger cacheLogger)
        {
            this._cacheConfig = cacheConfig;
            this._cacheLogger = cacheLogger;
            long index = (typeof(T).FullName.Select(c => Convert.ToInt64(c)).Aggregate((cur, next) => cur + next)) % 16;
            this._cacheLogger.LogAsync($"Connecting to RedisDB:{index}", System.Diagnostics.Tracing.EventLevel.Verbose);
            this.DB = redisFactory.Connection?.GetDatabase((int)index);
        }

        public bool Compact() => throw new NotSupportedException();

        public StoreWrapper<T> GetEntry(string key)
        {
            string strObj = this.DB.StringGetAsync(key).GetAwaiter().GetResult();
            return string.IsNullOrEmpty(strObj) ? default(StoreWrapper<T>) : this.DeserializeFromString(strObj);
        }

        private string SerializeToString(StoreWrapper<T> obj) => Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        private StoreWrapper<T> DeserializeFromString(string strObj) => Newtonsoft.Json.JsonConvert.DeserializeObject<StoreWrapper<T>>(strObj);
        public bool RemoveEntries(List<string> keys) => keys.Select(val => this.RemoveEntry(val)).Aggregate((cur, prev) => cur && prev);

        public bool RemoveEntry(string key) => this.DB.KeyDelete(key);

        public bool SetEntry(string key, StoreWrapper<T> value) => this.DB.StringSet(key, this.SerializeToString(value), value.TimeToLive);
    }
}