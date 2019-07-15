#if NET462
using System;
using System.Collections.Generic;
using System.Text;
using DMC.Logging;
using Ninject;
using Ninject.Modules;
using DMC.BackPlane;
using DMC.CacheContext;
using DMC.Channels.ChannelFactory;
using DMC.Channels.RabbitMq;
using DMC.Channels.ServiceBus;
using DMC.Implementations;

namespace DMC.Injection
{
    public class CacheProviderNinjectModule : NinjectModule
    {
        public override void Load() => this.Bind();
        private void Bind()
        {
            this.Kernel.Bind(typeof(ICacheProvider<>)).To(typeof(CacheProviderV1<>)).InSingletonScope();
            this.Kernel.Bind<IBackPlane>().To<BaseBackPlane>().InSingletonScope();
            this.Kernel.Bind<ICommunicationChannel>().To<RmqChannel>();
            this.Kernel.Bind<ICommunicationChannel>().To<ServiceBusChannel>();
            this.Kernel.Bind<ICommunicationChannelFactory>().To<CommunicationChannelFactory>().InSingletonScope();
            this.Kernel.Bind<IBaseCacheContextManager>().To<BaseCacheContextManager>().InSingletonScope();
        }
    }
}
#endif
