using System;
using System.Runtime.CompilerServices;

namespace ZeroAlloc.Linq.Boost
{
    public struct BoostSelectEngine<TStart, T0, T1, TPrev, TOp, TSelectorState> : IDelegatePipeline<TStart, T0, T1, TPrev, TSelectorState>
        where TPrev : ISpanPipeline<TStart, T0, TSelectorState>
        where TOp : ILinqUnaryOp<T0, T1, TSelectorState>
    {
        private readonly TOp _selector;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BoostSelectEngine(TOp selector)
        {
            _selector = selector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext(ref Span<TStart>.Enumerator enumerator, TPrev prev, ref TSelectorState state)
        {
            return prev.MoveNext(ref enumerator, ref state);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T1 GetCurrent(ref Span<TStart>.Enumerator enumerator, TPrev prev, ref TSelectorState state)
        {
            return _selector.Invoke(prev.GetCurrent(ref enumerator, ref state), ref state);
        }
    }
}