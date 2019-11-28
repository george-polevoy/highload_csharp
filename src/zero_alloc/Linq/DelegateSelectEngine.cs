using System;
using System.Runtime.CompilerServices;

namespace ZeroAlloc.Linq
{
    public struct DelegateSelectEngine<TStart, T0, T1, TPrev, TSelectorState>
        : IDelegatePipeline<TStart, T0, T1, TPrev, TSelectorState>
        where TPrev : ISpanPipeline<TStart, T0, TSelectorState>
    {
        private readonly Func<TSelectorState, T0, T1> _selector;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DelegateSelectEngine(Func<TSelectorState, T0, T1> selector)
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
            return _selector(state, prev.GetCurrent(ref enumerator, ref state));
        }
    }
}