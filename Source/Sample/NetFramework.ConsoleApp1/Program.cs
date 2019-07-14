using System;
using Model;
using Ninject;
using DMC;
using DMC.Injection;
using DMC.Logging;
using DMC.Logging.LoggingBinder;

namespace NetFramework.ConsoleApp1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            StandardKernel kernel = new StandardKernel();
            kernel.Load(new CacheProviderNinjectModule());
           
            kernel.Bind<ICacheLogger>().ToMethod((ctx) => new DynamicLoggingWrapper<ConsoleLogger>(new ConsoleLogger(), (io, message, ex, level) =>
            {
                io.WriteLine(message, ex, level);
                return true;
            }
            )).InSingletonScope();
            kernel.Bind<ICacheConfig>().To<CacheConfig>().InSingletonScope();

            ICacheProvider<MyDummyType> cache = kernel.Get<ICacheProvider<MyDummyType>>();
            cache.GetOrSet("key", () => { return new MyDummyType { Value = "val1" }; });
            cache.Invalidate("key");
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
