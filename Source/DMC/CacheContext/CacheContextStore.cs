using System;

namespace DMC.CacheContext
{
    public class CacheContextStore
    {
        public CacheContextStore()
        {
            IsCacheEnabled = true;
            CacheGuid = Guid.NewGuid();
        }
        public CacheContextStore(CacheContextStore cacheContext)
        {
            this.IsCacheEnabled = cacheContext.IsCacheEnabled;
            CacheGuid = new Guid(cacheContext.CacheGuid.ToString());
        }

        public bool IsCacheEnabled { get; set; }
        public Guid CacheGuid { get; set; }
        public bool AutoPropogateOrCachingEnabled { get; set; }
    }
}