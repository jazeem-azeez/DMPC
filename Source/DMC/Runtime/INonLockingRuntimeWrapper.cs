using System;
using System.Threading;
using System.Threading.Tasks;

public interface INonLockingRuntimeWrapper
{
    string Name { get; }

    Task<Tout> ThreadSafefWrapperAsync<Tout>(string operationKey, Func<AutoResetEvent, Task<Tout>> callback);
}