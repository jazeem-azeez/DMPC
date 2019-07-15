using StackExchange.Redis;

namespace DMC.CacheProvider.CacheStores
{
    public interface IRedisFactory
    {
        ConnectionMultiplexer Connection { get; }

        void Dispose();
    }
}