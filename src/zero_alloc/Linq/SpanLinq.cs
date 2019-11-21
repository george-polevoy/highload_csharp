using System;
using System.Runtime.CompilerServices;

namespace ZeroAlloc.Linq
{
    public static class SpanLinq
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SpanPipelineBuilder<T>
            StartWith<T>()
        {
            return new SpanPipelineBuilder<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SpanEnumerable<TStart, TSource0, TSource1, TPipeline, TState>
            Apply<TStart, TSource0, TSource1, TPipeline, TState>(this Span<TStart> source,
                SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPipeline, TState> pipeline)
            where TPipeline : ISpanPipeline<TStart, TSource0>
        {
            return new SpanEnumerable<TStart, TSource0, TSource1, TPipeline, TState>(pipeline, source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SpanPipelineFixedPoint<T, T, TResult, SpanPipelineBuilder<T>, Func<T, TResult>>
            Select<T, TResult>(this SpanPipelineBuilder<T> sourcePipeline, Func<T, TResult> selector)
        {
            return new SpanPipelineFixedPoint<T, T, TResult, SpanPipelineBuilder<T>, Func<T, TResult>>
            (sourcePipeline,
                (ref Span<T>.Enumerator enumerator,
                        ref SpanPipelineFixedPoint<T, T, TResult, SpanPipelineBuilder<T>,
                            Func<T, TResult>> state) =>
                    state.InterceptorMoveNext(ref enumerator), (ref Span<T>.Enumerator enumerator,
                        ref SpanPipelineFixedPoint<T, T, TResult, SpanPipelineBuilder<T>,
                            Func<T, TResult>> state) =>
                    state.State(state.InterceptorGetCurrent(ref enumerator)),
                selector
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SpanPipelineFixedPoint<TStart, TSource1, TResult,
                SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev,
                    TPrevState>,
                Func<TSource1, TResult>>
            Select<TStart, TSource0, TSource1, TPrev, TResult, TPrevState>(
                this SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev,
                    TPrevState> sourcePipeline,
                Func<TSource1, TResult> selector)
            where TPrev : ISpanPipeline<TStart, TSource0>
        {
            return new SpanPipelineFixedPoint<TStart, TSource1, TResult,
                SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev, TPrevState>,
                Func<TSource1, TResult>>(
                sourcePipeline, (
                    ref Span<TStart>.Enumerator enumerator,
                    ref SpanPipelineFixedPoint<TStart, TSource1, TResult,
                        SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev,
                            TPrevState>,
                        Func<TSource1, TResult>> state) => state.InterceptorMoveNext(ref enumerator),
                (
                    ref Span<TStart>.Enumerator enumerator,
                    ref SpanPipelineFixedPoint<TStart, TSource1, TResult,
                        SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev,
                            TPrevState>,
                        Func<TSource1, TResult>> state) => state.State(
                    state.InterceptorGetCurrent(ref enumerator)),
                selector
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SpanPipelineFixedPoint<TStart, TSource1, TSource1,
                SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev,
                    Func<TSource0, TSource1>>,
                Func<TSource1, bool>>
            Where<TStart, TSource0, TSource1, TPrev>(
                this SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev,
                    Func<TSource0, TSource1>> sourcePipeline,
                Func<TSource1, bool> predicate)
            where TPrev : ISpanPipeline<TStart, TSource0>
        {
            return new SpanPipelineFixedPoint<TStart, TSource1, TSource1,
                SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev, Func<TSource0, TSource1>>,
                Func<TSource1, bool>>(
                sourcePipeline, (
                    ref Span<TStart>.Enumerator enumerator,
                    ref SpanPipelineFixedPoint<TStart, TSource1, TSource1,
                        SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev,
                            Func<TSource0, TSource1>>,
                        Func<TSource1, bool>> state) =>
                {
                    while (true)
                    {
                        var innerMoved = state.InterceptorMoveNext(ref enumerator);
                        if (!innerMoved)
                            return false;
                        var nonClosurePredicate = state.State;
                        if (nonClosurePredicate(state.InterceptorGetCurrent(ref enumerator)))
                            return true;
                    }
                },
                (
                    ref Span<TStart>.Enumerator enumerator,
                    ref SpanPipelineFixedPoint<TStart, TSource1, TSource1,
                        SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev,
                            Func<TSource0, TSource1>>,
                        Func<TSource1, bool>> state) => 
                    state.InterceptorGetCurrent(ref enumerator),
                predicate
            );
        }
    }

    public struct DelegateSelectEngine<T, TResult> : ISpanPipeline<T, TResult>
    {
        private Func<T, TResult> _selector;

        public DelegateSelectEngine(Func<T, TResult> selector)
        {
            _selector = selector;
        }

        public bool MoveNext(ref Span<T>.Enumerator enumerator)
        {
            throw new NotImplementedException();
        }

        public TResult GetCurrent(ref Span<T>.Enumerator enumerator)
        {
            throw new NotImplementedException();
        }
    }
}


//                var innerMoved = _interceptor.MoveNext(ref enumerator);
//                if (!innerMoved)
//                    return false;
//                if (_context(_interceptor.GetCurrent(ref enumerator)))
//                    return true;