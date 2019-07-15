using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using DMC.Implementations;
using DMC.Logging;
using DMC.Models;

namespace DMC.CacheProvider.CacheStores
{
    public class InMemStore<T> : ICacheStores<T>
    {
        private readonly ICacheConfig _cacheConfig;
        private readonly ICacheLogger _cacheLogger; 
        private readonly ConcurrentDictionary<string, StoreWrapper<T>> _storeEntires = new ConcurrentDictionary<string, StoreWrapper<T>>();


        public InMemStore(ICacheConfig cacheConfig, ICacheLogger cacheLogger)
        {
            this._cacheConfig = cacheConfig;
            this._cacheLogger = cacheLogger;
        }

        public bool Compact()
        {
            string[] keys = this._storeEntires.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                this._storeEntires.TryGetValue(keys[i], out StoreWrapper<T> temp);
                
            }
            return true;
        }

        public TimeSpan GetDefaultExpiry() => throw new NotImplementedException();

        public StoreWrapper<T> GetEntry(string key)
        {
            StoreWrapper<T> result = default(StoreWrapper<T>);
            if (this._storeEntires.ContainsKey(key))
            {
                result = this._storeEntires[key];
            }
            return result;
        }

        public bool RemoveEntries(List<string> keys) => keys.Select(val => this.RemoveEntry(val)).Aggregate((cur, prev) => cur && prev);

        public bool RemoveEntry(string key) => this._storeEntires.TryRemove(key, out StoreWrapper<T> result);

        public bool SetEntry(string key, StoreWrapper<T> value) => this._storeEntires.AddOrUpdate(key, value, (curkey, val) => value) != null;
    }
}