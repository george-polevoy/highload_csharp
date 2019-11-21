using System;
using System.Runtime.CompilerServices;

namespace ZeroAlloc.Linq
{    
    public delegate bool MoveNextDelegate<TStart, TState>(
        ref Span<TStart>.Enumerator enumerator, ref TState state);

    public delegate T GetCurrentDelegate<out T, TStart, TState>(
        ref Span<TStart>.Enumerator enumerator, ref TState state);
    
    public struct SpanPipelineFixedPoint<TStart1, TStart2, T, TCombinationInterceptor, TState>
        : ISpanPipeline<TStart1, T>
        where TCombinationInterceptor : ISpanPipeline<TStart1, TStart2>
    {
        private TCombinationInterceptor _interceptor;
        private MoveNextDelegate<TStart1, SpanPipelineFixedPoint<TStart1, TStart2, T, TCombinationInterceptor, TState>> _moveNext;
        private GetCurrentDelegate<T, TStart1, SpanPipelineFixedPoint<TStart1, TStart2, T, TCombinationInterceptor, TState>> _getCurrent;
        public TState State { get; }

        public bool InterceptorMoveNext(ref Span<TStart1>.Enumerator enumerator)
        {
            return _interceptor.MoveNext(ref enumerator);
        }

        public TStart2 InterceptorGetCurrent(ref Span<TStart1>.Enumerator enumerator)
        {
            return _interceptor.GetCurrent(ref enumerator);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal SpanPipelineFixedPoint(
            TCombinationInterceptor interceptor,
            MoveNextDelegate<TStart1, SpanPipelineFixedPoint<TStart1, TStart2, T, TCombinationInterceptor, TState>> moveNext,
            GetCurrentDelegate<T, TStart1, SpanPipelineFixedPoint<TStart1, TStart2, T, TCombinationInterceptor, TState>> getCurrent, TState state)
        {
            _interceptor = interceptor;
            _moveNext = moveNext;
            _getCurrent = getCurrent;
            State = state;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext(ref Span<TStart1>.Enumerator enumerator)
        {
            return _moveNext(ref enumerator, ref this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetCurrent(ref Span<TStart1>.Enumerator enumerator)
        {
            return _getCurrent(ref enumerator, ref this);
        }
    }
}