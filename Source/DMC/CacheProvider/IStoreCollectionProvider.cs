using System.Collections.Generic;

namespace DMC.Implementations
{
    public interface IStoreCollectionProvider<T>
    {
        SortedDictionary<int, ICacheStores<T>> GetCacheStoreCollection();
    }
}