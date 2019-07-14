using Microsoft.Extensions.DependencyInjection;
using DMC.BackPlane;
using DMC.CacheContext;
using DMC.Channels.ChannelFactory;
using DMC.Implementations;

namespace DMC.Injection
{
    public static class CacheProviderInjectionExtension
    {
        public static IServiceCollection AddCacheProviderDependencies(this IServiceCollection services)
        {
            services.AddSingleton(typeof(ICacheProvider<>), typeof(BasicCacheProvider<>));
            services.AddSingleton<IBackPlane, BaseBackPlane>();
            services.AddSingleton<ICommunicationChannelFactory, CommunicationChannelFactory>();
            services.AddSingleton<IBaseCacheContextManager, BaseCacheContextManager>();

            return services;
        }
    }
}