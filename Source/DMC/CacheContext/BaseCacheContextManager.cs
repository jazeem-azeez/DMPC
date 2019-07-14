using System;
using System.Threading;

namespace DMC.CacheContext
{
    public class BaseCacheContextManager : IBaseCacheContextManager
    {
        public BaseCacheContextManager()
        {
            this.CurrentContextStack = new AsyncLocal<IImmutableStack<CacheContextStore>>
            {
                Value = ImmutableStack<CacheContextStore>.Empty
            };
            this.CurrentContextStack.Value = this.CurrentContextStack.Value.Push(new CacheContextStore());
        }
        public AsyncLocal<IImmutableStack<CacheContextStore>> CurrentContextStack { get; private set; }

        public CacheContextStore CacheContext
        {
            get
            {
                try
                {
                    CacheContextStore temp = this.CurrentContextStack?.Value?.Peek();
                    return temp;
                }
                catch (StackEmptyException ex)
                {

                    return null;

                }
            }
        }

        public CacheContextScope CreateScope(bool CacheEnabled = true, Guid trackGuid = default(Guid))
        {
            if (this.CurrentContextStack == null)
            {
                this.CurrentContextStack = new AsyncLocal<IImmutableStack<CacheContextStore>>
                {
                    Value = ImmutableStack<CacheContextStore>.Empty
                };
            }
            if (this.CurrentContextStack.Value == null)
            {
                this.CurrentContextStack.Value = ImmutableStack<CacheContextStore>.Empty;
                this.CurrentContextStack.Value = this.CurrentContextStack.Value.Push(new CacheContextStore());
            }

            CacheContextStore top = this.CurrentContextStack.Value.IsEmpty ? new CacheContextStore() : this.CurrentContextStack.Value.Peek();
            CacheContextStore clone = new CacheContextStore
            {
                IsCacheEnabled = CacheEnabled,
                CacheGuid = (trackGuid == default(Guid)) ? top.CacheGuid : trackGuid
            };

            this.CurrentContextStack.Value = this.CurrentContextStack.Value.Push(clone);
            return new CacheContextScope(this.CurrentContextStack);
        }
    }
}