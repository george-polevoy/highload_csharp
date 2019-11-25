using System;
using System.Runtime.CompilerServices;

namespace ZeroAlloc.Linq
{
    public struct DelegateWhereEngine<TStart, T0, TPrev> : IDelegatePipeline<TStart, T0, T0, TPrev>
        where TPrev : ISpanPipeline<TStart, T0>
    {
        private readonly Func<T0, bool> _predicate;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DelegateWhereEngine(Func<T0, bool> predicate)
        {
            _predicate = predicate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext(ref Span<TStart>.Enumerator enumerator, ref TPrev prev)
        {
            while (true)
            {
                var innerMoved = prev.MoveNext(ref enumerator);
                if (!innerMoved)
                    return false;
                if (_predicate(prev.GetCurrent(ref enumerator)))
                    return true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T0 GetCurrent(ref Span<TStart>.Enumerator enumerator, ref TPrev prev)
        {
            return prev.GetCurrent(ref enumerator);
        }
    }
}