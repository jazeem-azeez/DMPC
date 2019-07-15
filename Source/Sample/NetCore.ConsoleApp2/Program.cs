using System;

using DMC;
using DMC.Injection;
using DMC.Logging;
using DMC.Logging.LoggingBinder;

using Microsoft.Extensions.DependencyInjection;
using NetCore.ConsoleApp1;
using NetCore.ConsoleApp1.Model;

namespace NetCore.ConsoleApp2
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            Console.Title = "NetCore.ConsoleApp2";

            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<ICacheLogger>((ctx) => new DynamicLoggingWrapper<ConsoleLogger>(new ConsoleLogger(), (io, message, ex, level) =>
            {
                io.WriteLine(message, ex, level);
                return true;
            }
            ));
            services.AddSingleton<ICacheConfig, CacheConfig>();

            services.AddCacheProviderDependencies();
            IServiceProvider injection = services.BuildServiceProvider();
            ICacheProvider<MyDummyType> cache = injection.GetRequiredService<ICacheProvider<MyDummyType>>();
            cache.GetOrSet("key", () => { return new MyDummyType { Value = "val1" }; });
            Console.WriteLine("Data Priming Completed Press Enter Key To Try a fetch");
            Console.ReadLine();
            MyDummyType temp = cache.Get("key");
            if (temp != null)
            {
                Console.WriteLine("Invalidation failed ");
            }
            else
            {
                Console.WriteLine("Invalidation Sucess ");
            }
            Console.ReadLine();
        }
    }
}