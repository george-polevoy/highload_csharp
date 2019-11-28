using System;

namespace ZeroAlloc.Linq
{
    public interface IDelegatePipeline<TStart, out T0, out T1, in TPrev, TSelectorState>
        where TPrev : ISpanPipeline<TStart, T0, TSelectorState>
    {
        bool MoveNext(ref Span<TStart>.Enumerator enumerator, TPrev prev, ref TSelectorState state);
        T1 GetCurrent(ref Span<TStart>.Enumerator enumerator, TPrev prev, ref TSelectorState state);
    }
}