using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DMC.Models;

namespace DMC.Implementations
{
    public interface ICacheStores<T> 
    {        
        bool Compact() ;
        StoreWrapper<T> GetEntry(string key) ;
        bool RemoveEntry(string key) ;
        bool RemoveEntries(List<string> keys) ; 
        bool SetEntry(string key, StoreWrapper<T> value);
    }
}