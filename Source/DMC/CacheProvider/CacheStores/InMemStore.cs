using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DMC.Implementations;
using DMC.Models;

namespace DMC.CacheProvider.CacheStores
{
    public class InMemStore<T>:ICacheStores<T>
    {
        private volatile ConcurrentDictionary<string, StoreWrapper<T>> _storeCollection = new ConcurrentDictionary<string, StoreWrapper<T>>();
        private NonLockingRuntimeWrapper<T> _nonLockingRuntimeWrapper;
        public InMemStore()
        {

        }

        public bool Compact() => throw new NotImplementedException();
        public T Get(string key) => throw new NotImplementedException();
        public TimeSpan GetDefaultExpiry() => throw new NotImplementedException();
        public T GetOrSet(string key, Func<T> getItemCallBack) => throw new NotImplementedException();
        public T GetOrSet(string key, Func<T> getItemCallBack, TimeSpan? expiry) => throw new NotImplementedException();
        public Task<T> GetOrSetAsync(string key, Func<Task<T>> getItemCallBack, TimeSpan? expiry) => throw new NotImplementedException();
        public Task<T> GetOrSetAsync(string key, Func<Task<T>> getItemCallBack) => throw new NotImplementedException();
        public bool Invalidate(string key) => throw new NotImplementedException();
        public bool Invalidate(List<string> keys) => throw new NotImplementedException();
        public bool Invalidate(List<string> keys, List<string> filters) => throw new NotImplementedException();
        public bool Set(string key, T value, TimeSpan? expiry) => throw new NotImplementedException();
        public bool Update(string key, T data, TimeSpan? expiry = null, T oldData = default(T)) => throw new NotImplementedException();
        public bool Update(List<string> keys, T data, TimeSpan? expiry = null, T oldData = default(T)) => throw new NotImplementedException();
    }
}
