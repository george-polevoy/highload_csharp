using System;
using System.Runtime.CompilerServices;
using ZeroAlloc.Linq.Boost;

namespace ZeroAlloc.Linq
{
    public readonly struct SpanPipelineBuilder<T, TState> : ISpanPipeline<T, T, TState>
    {
        public Operations<TState> Operations()
        {
            return new Operations<TState>();
        }

        public Op<TOp, T, TResult, TState> FromOperation<TOp, TResult>(TOp op, TResult result)
            where TOp : ILinqUnaryOp<T, TResult, TState>
        {
            return op;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext(ref Span<T>.Enumerator enumerator, ref TState state)
        {
            return enumerator.MoveNext();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetCurrent(ref Span<T>.Enumerator enumerator, ref TState state)
        {
            return enumerator.Current;
        }
    }
}