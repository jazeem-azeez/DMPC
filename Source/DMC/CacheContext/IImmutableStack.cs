using System;
using System.Collections.Generic;
using System.Text;

namespace DMC.CacheContext
{
    public interface IImmutableStack<T> : IEnumerable<T>
    {
        IImmutableStack<T> Push(T value);
        IImmutableStack<T> Pop();
        T Peek();
        bool IsEmpty { get; }
    }
}
