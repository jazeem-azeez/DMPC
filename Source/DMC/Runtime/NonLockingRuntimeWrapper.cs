using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using DMC.Logging;

namespace DMC.RunTime
{
    public class NonLockingRuntimeWrapper<T> : INonLockingRuntimeWrapper
    {
        private readonly ConcurrentDictionary<string, AutoResetEvent> _operationsInProgress = new ConcurrentDictionary<string, AutoResetEvent>();
        private readonly ICacheLogger _cacheLogger;

        public string Name { get; private set; }

        public NonLockingRuntimeWrapper(ICacheLogger cacheLogger)
        {
            this._cacheLogger = cacheLogger;
            this.Name = typeof(T).FullName;
        }
        private AutoResetEvent GetKeyBasedResetEventHandle(string key)
        {

            if (this._operationsInProgress.ContainsKey(key) == false)
            {
                this._operationsInProgress.TryAdd(key, new AutoResetEvent(true));
            }

            return this._operationsInProgress[key];
        }
        public async Task<Tout> ThreadSafefWrapperAsync<Tout>(string operationKey, Func<AutoResetEvent, Task<Tout>> callback)
        {
            AutoResetEvent handle = this.GetKeyBasedResetEventHandle(operationKey);
            handle.WaitOne();
            handle.Reset();
            try
            {
                Tout result = await callback(handle);
                return result;
            }
            catch (Exception ex)
            {
                handle.Set();
                this._cacheLogger.LogException(ex);
                throw;
            }
            finally
            {
                handle.Set();
            }
        }
    }
}