using System;
using System.Collections.Generic;

namespace ZeroAlloc.EnumeratorLinq
{
    public static class EnumeratorSelectExtensions
    {
        public static RefSelectEnumerator<T, TResult, TState, TEnumerator>
            RefSelect<T, TResult, TEnumerator, TState>(this TEnumerator source, TState state,
                Func<TState, T, TResult> selector) where TEnumerator : IEnumerator<T>
        {
            return new RefSelectEnumerator<T, TResult, TState, TEnumerator>(state, source, selector);
        }

        public static long Sum<TEnumerator>(this TEnumerator source) where TEnumerator : IEnumerator<long>
        {
            long s = 0;
            while (source.MoveNext())
            {
                s += source.Current;
            }

            return s;
        }
        
        public static long Sum<T, TState, TEnumerator>(this RefSelectEnumerator<T, long, TState, TEnumerator> source) where TEnumerator : IEnumerator<T>
        {
            long s = 0;
            while (source.MoveNext())
            {
                s += source.Current;
            }

            return s;
        }
    }
}