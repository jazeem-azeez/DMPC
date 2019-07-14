using System;
using System.Threading;

namespace DMC.CacheContext
{
    public class CacheContextScope : IDisposable
    {
        private readonly AsyncLocal<IImmutableStack<CacheContextStore>> _stack;

        public CacheContextScope(AsyncLocal<IImmutableStack<CacheContextStore>> stack) => this._stack = stack;

        public CacheContextStore CurrentCacheContext => this._stack.Value.Peek();

        public void Dispose()
        {
            if (!this._stack.Value.IsEmpty)
            {
                this._stack.Value = this._stack.Value.Pop();
            }
        }
    }
}
