using System;
using System.Collections.Generic;

namespace ZeroAlloc.EnumeratorLinq
{
    public ref struct RefSelectEnumerator<T, TResult, TState, TSourceEnumerator>
        where TSourceEnumerator : IEnumerator<T>
    {
        private readonly TState _state;

        private TSourceEnumerator _source;

        private readonly Func<TState, T, TResult> _selector;

        public RefSelectEnumerator(TState state, TSourceEnumerator source, Func<TState, T, TResult> selector)
        {
            _state = state;
            _source = source;
            _selector = selector;
        }

        public bool MoveNext() => _source.MoveNext();

        public TResult Current => _selector(_state, _source.Current);

        public void Dispose()
        {
            _source.Dispose();
        }

        public void Reset()
        {
            _source.Reset();
        }

        //object? IEnumerator.Current => Current;
    }
}