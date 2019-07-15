using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DMC.Models;

namespace DMC
{
    public interface ICacheProvider<T> : ICacheProviderBase
    {
        Task DispatchEvent(EventMessage eventMessage);

        T Get(string key, bool autoPropogateOrCachingEnabled=true);      

        T GetOrSet(string key, Func<T> getItemCallBack, bool autoPropogateOrCachingEnabled = true);

        T GetOrSet(string key, Func<T> getItemCallBack, TimeSpan? expiry, bool autoPropogateOrCachingEnabled = true);

        Task<T> GetOrSetAsync(string key, Func<Task<T>> getItemCallBack, TimeSpan? expiry,bool autoPropogateOrCachingEnabled= true);

        Task<T> GetOrSetAsync(string key, Func<Task<T>> getItemCallBack, bool autoPropogateOrCachingEnabled = true);

        bool Set(string key, T value, TimeSpan? expiry,int level=0, bool autoPropogateOrCachingEnabled = true); 

        bool Update(string key, T data, TimeSpan? expiry = null, T oldData = default(T), bool autoPropogateOrCachingEnabled = true);

        bool Update(List<string> keys, T data, TimeSpan? expiry = null, T oldData = default(T), bool autoPropogateOrCachingEnabled = true);
         
    }
}