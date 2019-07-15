using System.Collections.Generic;
using DMC.CacheProvider.CacheStores;
using DMC.Logging;

namespace DMC.Implementations
{
    public class StoreCollectionProvider<T> : IStoreCollectionProvider<T>
    {
        private readonly IRedisFactory _redisFactory;
        private readonly ICacheConfig _cacheConfig;
        private readonly ICacheLogger _cacheLogger;

        public StoreCollectionProvider(IRedisFactory redisFactory, ICacheConfig cacheConfig,ICacheLogger cacheLogger)
        {
            this._redisFactory = redisFactory;
            this._cacheConfig = cacheConfig;
            this._cacheLogger = cacheLogger;
        }
      
        public SortedDictionary<int, ICacheStores<T>> GetCacheStoreCollection()
        {
            return new SortedDictionary<int, ICacheStores<T>>()
            {
                {0, new InMemStore<T>(_cacheConfig,_cacheLogger) },
                {1, new RedisStore<T>(_redisFactory, _cacheConfig,_cacheLogger) }

            };
        }
    }
}