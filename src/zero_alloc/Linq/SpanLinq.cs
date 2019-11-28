using System;
using System.Runtime.CompilerServices;
using ZeroAlloc.Linq.Boost;

namespace ZeroAlloc.Linq
{
    public static class SpanLinq
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SpanPipelineBuilder<T, TState>
            StartWith<T, TState>()
        {
            return new SpanPipelineBuilder<T, TState>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SpanEnumerable<TStart, TSource0, TSource1, TPipeline, TState, TSelectorState>
            Apply<TStart, TSource0, TSource1, TPipeline, TState, TSelectorState>(this Span<TStart> source,
                SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPipeline, TState, TSelectorState> pipeline,
                TSelectorState state)
            where TPipeline : ISpanPipeline<TStart, TSource0, TSelectorState>
            where TState : IDelegatePipeline<TStart, TSource0, TSource1, TPipeline, TSelectorState>
        {
            return new SpanEnumerable<TStart, TSource0, TSource1, TPipeline, TState, TSelectorState>(pipeline, source, state);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SpanPipelineFixedPoint<T, T, TResult, SpanPipelineBuilder<T, TSelectorState>,
                DelegateSelectEngine<T, T, TResult, SpanPipelineBuilder<T, TSelectorState>, TSelectorState>, TSelectorState>
            Select<T, TResult, TSelectorState>(
                this SpanPipelineBuilder<T, TSelectorState> sourcePipeline, Func<TSelectorState, T, TResult> selector)
        {
            return new SpanPipelineFixedPoint<T, T, TResult, SpanPipelineBuilder<T, TSelectorState>,
                DelegateSelectEngine<T, T, TResult, SpanPipelineBuilder<T, TSelectorState>, TSelectorState>, TSelectorState>
            (sourcePipeline,
                new DelegateSelectEngine<T, T, TResult, SpanPipelineBuilder<T, TSelectorState>, TSelectorState>(selector)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SpanPipelineFixedPoint<TStart, TSource1, TResult,
                SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev,
                    TPrevState, TSelectorState>,
                DelegateSelectEngine<TStart, TSource1, TResult, SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev
                    ,TPrevState, TSelectorState>, TSelectorState>, TSelectorState>
            Select<TStart, TSource0, TSource1, TPrev, TResult, TPrevState, TSelectorState>(
                this SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev,
                    TPrevState, TSelectorState> sourcePipeline,
                Func<TSelectorState, TSource1, TResult> selector)
            where TPrev : ISpanPipeline<TStart, TSource0, TSelectorState>
            where TPrevState : IDelegatePipeline<TStart, TSource0, TSource1, TPrev, TSelectorState>
        {
            return new SpanPipelineFixedPoint<TStart, TSource1, TResult,
                SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev, TPrevState, TSelectorState>, DelegateSelectEngine<TStart,
                    TSource1, TResult, SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev, TPrevState, TSelectorState>, TSelectorState>, TSelectorState>(
                sourcePipeline,
                new DelegateSelectEngine<TStart, TSource1, TResult,
                    SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev, TPrevState, TSelectorState>, TSelectorState>(
                    selector));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SpanPipelineFixedPoint<TStart, TSource1, TSource1,
                SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev,
                    TPrevState, TSelectorState>,
                DelegateWhereEngine<TStart, TSource1, SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev
                    ,
                    TPrevState, TSelectorState>, TSelectorState>, TSelectorState>
            Where<TStart, TSource0, TSource1, TPrev, TPrevState, TSelectorState>(
                this SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev,
                    TPrevState, TSelectorState> sourcePipeline,
                Func<TSelectorState, TSource1, bool> predicate)
            where TPrev : ISpanPipeline<TStart, TSource0, TSelectorState>
            where TPrevState : IDelegatePipeline<TStart, TSource0, TSource1, TPrev, TSelectorState>
        {
            return new SpanPipelineFixedPoint<TStart, TSource1, TSource1,
                SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev, TPrevState, TSelectorState>, DelegateWhereEngine<TStart,
                    TSource1, SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev, TPrevState, TSelectorState>, TSelectorState>, TSelectorState>(
                sourcePipeline,
                new DelegateWhereEngine<TStart, TSource1,
                    SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev, TPrevState, TSelectorState>, TSelectorState>(
                    predicate));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SpanPipelineFixedPoint<T, T, TResult, SpanPipelineBuilder<T, TSelectorState>,
                BoostSelectEngine<T, T, TResult, SpanPipelineBuilder<T, TSelectorState>, TOp, TSelectorState>, TSelectorState>
            Select<T, TResult, TOp, TSelectorState>(this SpanPipelineBuilder<T, TSelectorState> sourcePipeline, Op<TOp, T, TResult, TSelectorState> selector)
            where TOp : ILinqUnaryOp<T, TResult, TSelectorState>
        {
            return new SpanPipelineFixedPoint<T, T, TResult, SpanPipelineBuilder<T, TSelectorState>,
                BoostSelectEngine<T, T, TResult, SpanPipelineBuilder<T, TSelectorState>, TOp, TSelectorState>, TSelectorState>
            (sourcePipeline,
                new BoostSelectEngine<T, T, TResult, SpanPipelineBuilder<T, TSelectorState>, TOp, TSelectorState>(selector.InnerOp)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SpanPipelineFixedPoint<TStart, TSource1, TResult,
                SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev,
                    TPrevState, TSelectorState>,
                BoostSelectEngine<TStart, TSource1, TResult, SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev,
                    TPrevState, TSelectorState>, TOp, TSelectorState>, TSelectorState>
            Select<TStart, TSource0, TSource1, TPrev, TResult, TPrevState, TOp, TSelectorState>(
                this SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev,
                    TPrevState, TSelectorState> sourcePipeline,
                Op<TOp, TSource1, TResult, TSelectorState> selector)
            where TPrev : ISpanPipeline<TStart, TSource0, TSelectorState>
            where TPrevState : IDelegatePipeline<TStart, TSource0, TSource1, TPrev, TSelectorState>
            where TOp : ILinqUnaryOp<TSource1, TResult, TSelectorState>
        {
            return new SpanPipelineFixedPoint<TStart, TSource1, TResult,
                SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev,
                    TPrevState, TSelectorState>,
                BoostSelectEngine<TStart, TSource1, TResult, SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev, TPrevState, TSelectorState>, TOp, TSelectorState>, TSelectorState>(
                sourcePipeline,
                new BoostSelectEngine<TStart, TSource1, TResult, SpanPipelineFixedPoint<TStart, TSource0, TSource1,
                    TPrev
                    ,
                    TPrevState, TSelectorState>, TOp, TSelectorState>(
                    selector.InnerOp));
        }
    }
}