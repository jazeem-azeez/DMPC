using DMC.BackPlane;
using DMC.CacheContext;
using DMC.CacheProvider.CacheStores;
using DMC.Channels.ChannelFactory;
using DMC.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace DMC.Injection
{
    public static class CacheProviderInjectionExtension
    {
        public static IServiceCollection AddCacheProviderDependencies(this IServiceCollection services)
        {
            services.AddSingleton(typeof(ICacheProvider<>), typeof(CacheProviderV1<>));
            services.AddSingleton(typeof(InMemStore<>));
            services.AddSingleton(typeof(RedisStore<>));
            services.AddSingleton(typeof(ICacheProvider<>), typeof(CacheProviderV1<>));
            services.AddSingleton<IBackPlane, BaseBackPlane>();
            services.AddSingleton<ICommunicationChannelFactory, CommunicationChannelFactory>();
            services.AddSingleton<IBaseCacheContextManager, BaseCacheContextManager>();
            services.AddSingleton<IRedisFactory, RedisFactory>();
            services.AddSingleton(typeof(IStoreCollectionProvider<>), typeof(StoreCollectionProvider<>));


            return services;
        }
    }
}