using System;

namespace ZeroAlloc.Linq
{
    public interface IDelegatePipeline<TStart, out T0, out T1, TPrev>
        where TPrev : ISpanPipeline<TStart, T0>
    {
        bool MoveNext(ref Span<TStart>.Enumerator enumerator, ref TPrev prev);
        T1 GetCurrent(ref Span<TStart>.Enumerator enumerator, ref TPrev prev);
    }
}