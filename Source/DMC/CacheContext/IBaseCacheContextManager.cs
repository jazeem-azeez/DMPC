using System;
using System.Threading;

namespace DMC.CacheContext
{
    public interface IBaseCacheContextManager
    {
        CacheContextStore CacheContext { get; }
        AsyncLocal<IImmutableStack<CacheContextStore>> CurrentContextStack { get; }

        CacheContextScope CreateScope(bool CacheEnabled = true, Guid trackGuid = default(Guid));
    }
}