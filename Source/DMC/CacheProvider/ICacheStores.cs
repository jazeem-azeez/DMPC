using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DMC.Models;

namespace DMC.Implementations
{
    public interface ICacheStores<T> 
    {        
        bool Compact() ;
        T Get(string key) ;
        TimeSpan GetDefaultExpiry() ;
        T GetOrSet(string key, Func<T> getItemCallBack) ;
        T GetOrSet(string key, Func<T> getItemCallBack, TimeSpan? expiry) ;
        Task<T> GetOrSetAsync(string key, Func<Task<T>> getItemCallBack, TimeSpan? expiry) ;
        Task<T> GetOrSetAsync(string key, Func<Task<T>> getItemCallBack) ;
        bool Invalidate(string key) ;
        bool Invalidate(List<string> keys) ;
        bool Invalidate(List<string> keys, List<string> filters) ;
        bool Set(string key, T value, TimeSpan? expiry) ;
        bool Update(string key, T data, TimeSpan? expiry = null, T oldData = default(T)) ;
        bool Update(List<string> keys, T data, TimeSpan? expiry = null, T oldData = default(T)) ;
    }
}