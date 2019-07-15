using System;

using StackExchange.Redis;

namespace DMC.CacheProvider.CacheStores
{
    /// <summary> Factory For Redis Connection </summary>
    /// <seealso cref="System.IDisposable" />
    public sealed class RedisFactory : IDisposable, IRedisFactory
    {
        private readonly ICacheConfig _cacheConfig;

        public RedisFactory(ICacheConfig cacheConfig) => this._cacheConfig = cacheConfig;

        /// <summary> Gets the connection. </summary>
        /// <value> The connection. </value>
        public ConnectionMultiplexer Connection
        {
            get
            {
                string connectionString = this._cacheConfig.RedisConnectionString; 
                ConfigurationOptions options = ConfigurationOptions.Parse(connectionString);
                options.AbortOnConnectFail = false;
                return ConnectionMultiplexer.Connect(options);
            }
        }

        /// <summary> 
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting
        ///   unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.Connection != null)
            {
                this.Connection.Dispose();
                GC.SuppressFinalize(this);
            }
        }
    }
}