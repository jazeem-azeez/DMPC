using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DMC.CacheContext
{
   
    public sealed class ImmutableStack<T> : IImmutableStack<T>
    {
        private sealed class EmptyStack : IImmutableStack<T>
        {
            public bool IsEmpty { get { return true; } }
            public T Peek() { throw new StackEmptyException("Empty stack"); }
            public IImmutableStack<T> Push(T value) { return new ImmutableStack<T>(value, this); }
            public IImmutableStack<T> Pop() { throw new StackEmptyException("Empty stack"); }
            public IEnumerator<T> GetEnumerator() { yield break; }
            IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }
        }
        private static readonly EmptyStack empty = new EmptyStack();
        public static IImmutableStack<T> Empty { get { return empty; } }
        private readonly T head;
        private readonly IImmutableStack<T> tail;
       
        private ImmutableStack(T head, IImmutableStack<T> tail)
        {
            this.head = head;
            this.tail = tail;
        }
        public bool IsEmpty { get { return false; } }
        public T Peek() { return head; }
        public IImmutableStack<T> Pop() { return tail; }
        public IImmutableStack<T> Push(T value) { return new ImmutableStack<T>(value, this); }
        public IEnumerator<T> GetEnumerator()
        {
            for (IImmutableStack<T> stack = this; !stack.IsEmpty; stack = stack.Pop())
                yield return stack.Peek();
        }
        IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }
    }
}
