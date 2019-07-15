using Microsoft.Extensions.DependencyInjection;
using DMC.BackPlane;
using DMC.CacheContext;
using DMC.Channels.ChannelFactory;
using DMC.Implementations;
using DMC.CacheProvider.CacheStores;

namespace DMC.Injection
{
    public static class CacheProviderInjectionExtension
    {
        public static IServiceCollection AddCacheProviderDependencies(this IServiceCollection services)
        {
            services.AddSingleton(typeof(ICacheProvider<>), typeof(CacheProviderV1<>));
            services.AddSingleton(typeof(ICacheStores<>), typeof(InMemStore<>));
            services.AddSingleton(typeof(ICacheProvider<>), typeof(CacheProviderV1<>));
            services.AddSingleton<IBackPlane, BaseBackPlane>();
            services.AddSingleton<ICommunicationChannelFactory, CommunicationChannelFactory>();
            services.AddSingleton<IBaseCacheContextManager, BaseCacheContextManager>();
            services.AddSingleton<IRedisFactory, IRedisFactory>();


            return services;
        }
    }
}