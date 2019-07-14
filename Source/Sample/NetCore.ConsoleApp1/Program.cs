using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using DMC.Logging;
using Microsoft.Extensions.DependencyInjection;
using Model;
using DMC;
using DMC.Injection;
using DMC.Logging.LoggingBinder;

namespace NetCore.ConsoleApp1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            Console.Title = "NetCore.ConsoleApp1";
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<ICacheLogger>((ctx) => new DynamicLoggingWrapper<ConsoleLogger>(new ConsoleLogger(),(io,message, ex, level) =>
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
            cache.Invalidate("key");
            Console.WriteLine("Data Priming Completed & invaldationg fired Press Enter Key To Try a fetch");
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
