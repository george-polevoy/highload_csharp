using System;
using System.Runtime.CompilerServices;

namespace ZeroAlloc.Linq.Boost
{
    public struct BoostSelectEngine<TStart, T0, T1, TPrev, TOp> : IDelegatePipeline<TStart, T0, T1, TPrev>
        where TPrev : ISpanPipeline<TStart, T0>
        where TOp : ILinqUnaryOp<T0, T1>
    {
        private readonly TOp _selector;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BoostSelectEngine(TOp selector)
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
            return _selector.Invoke(prev.GetCurrent(ref enumerator));
        }
    }
}