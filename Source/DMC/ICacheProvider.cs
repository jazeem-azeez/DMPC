using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DMC.Models;

namespace DMC
{
    public interface ICacheProvider<T> : ICacheProviderBase
    {
        Task DispatchEvent(EventMessage eventMessage);

        T Get(string key);

        TimeSpan GetDefaultExpiry();

        T GetOrSet(string key, Func<T> getItemCallBack);

        T GetOrSet(string key, Func<T> getItemCallBack, TimeSpan? expiry);

        Task<T> GetOrSetAsync(string key, Func<Task<T>> getItemCallBack, TimeSpan? expiry);

        Task<T> GetOrSetAsync(string key, Func<Task<T>> getItemCallBack);

        bool Set(string key, T value, TimeSpan? expiry);

        bool Update(string key, T data, TimeSpan? expiry = null, T oldData = default(T));

        bool Update(List<string> keys, T data, TimeSpan? expiry = null, T oldData = default(T));
         
    }
}