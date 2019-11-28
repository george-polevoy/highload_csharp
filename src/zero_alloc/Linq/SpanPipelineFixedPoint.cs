using System;
using System.Runtime.CompilerServices;

namespace ZeroAlloc.Linq
{
    public struct SpanPipelineFixedPoint<TStart1, TStart2, T, TCombinationInterceptor, TState, TSelectorState>
        : ISpanPipeline<TStart1, T, TSelectorState>
        where TCombinationInterceptor : ISpanPipeline<TStart1, TStart2, TSelectorState>
        where TState : IDelegatePipeline<TStart1, TStart2, T, TCombinationInterceptor, TSelectorState>
    {
        private readonly TCombinationInterceptor _interceptor;
        private TState _state;
        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal SpanPipelineFixedPoint(
            TCombinationInterceptor interceptor,
            TState state)
        {
            _interceptor = interceptor;
            _state = state;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext(ref Span<TStart1>.Enumerator enumerator, ref TSelectorState state)
        {
            return _state.MoveNext(ref enumerator, _interceptor, ref state);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetCurrent(ref Span<TStart1>.Enumerator enumerator, ref TSelectorState state)
        {
            return _state.GetCurrent(ref enumerator, _interceptor, ref state);
        }
    }
}