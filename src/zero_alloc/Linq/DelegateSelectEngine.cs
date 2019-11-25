using System;
using System.Runtime.CompilerServices;

namespace ZeroAlloc.Linq
{
    public struct DelegateSelectEngine<TStart, T0, T1, TPrev> : IDelegatePipeline<TStart, T0, T1, TPrev>
        where TPrev : ISpanPipeline<TStart, T0>
    {
        private readonly Func<T0, T1> _selector;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DelegateSelectEngine(Func<T0, T1> selector)
        {
            _selector = selector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext(ref Span<TStart>.Enumerator enumerator, ref TPrev prev)
        {
            return prev.MoveNext(ref enumerator);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T1 GetCurrent(ref Span<TStart>.Enumerator enumerator, ref TPrev prev)
        {
            return _selector(prev.GetCurrent(ref enumerator));
        }
    }
}