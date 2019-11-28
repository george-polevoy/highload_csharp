using System;
using System.Runtime.CompilerServices;

namespace ZeroAlloc.Linq
{
    public struct DelegateWhereEngine<TStart, T0, TPrev, TSelectorState> : IDelegatePipeline<TStart, T0, T0, TPrev, TSelectorState>
        where TPrev : ISpanPipeline<TStart, T0, TSelectorState>
    {
        private readonly Func<TSelectorState, T0, bool> _predicate;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DelegateWhereEngine(Func<TSelectorState, T0, bool> predicate)
        {
            _predicate = predicate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext(ref Span<TStart>.Enumerator enumerator, TPrev prev, ref TSelectorState state)
        {
            while (true)
            {
                var innerMoved = prev.MoveNext(ref enumerator, ref state);
                if (!innerMoved)
                    return false;
                if (_predicate(state, prev.GetCurrent(ref enumerator, ref state)))
                    return true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T0 GetCurrent(ref Span<TStart>.Enumerator enumerator, TPrev prev, ref TSelectorState state)
        {
            return prev.GetCurrent(ref enumerator, ref state);
        }
    }
}