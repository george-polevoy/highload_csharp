using System;
using System.Runtime.CompilerServices;
using ZeroAlloc.Linq.Boost;

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
            where TState : IDelegatePipeline<TStart, TSource0, TSource1, TPipeline>
        {
            return new SpanEnumerable<TStart, TSource0, TSource1, TPipeline, TState>(pipeline, source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SpanPipelineFixedPoint<T, T, TResult, SpanPipelineBuilder<T>,
                DelegateSelectEngine<T, T, TResult, SpanPipelineBuilder<T>>>
            Select<T, TResult>(this SpanPipelineBuilder<T> sourcePipeline, Func<T, TResult> selector)
        {
            return new SpanPipelineFixedPoint<T, T, TResult, SpanPipelineBuilder<T>,
                DelegateSelectEngine<T, T, TResult, SpanPipelineBuilder<T>>>
            (sourcePipeline,
                new DelegateSelectEngine<T, T, TResult, SpanPipelineBuilder<T>>(selector)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SpanPipelineFixedPoint<TStart, TSource1, TResult,
                SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev,
                    TPrevState>,
                DelegateSelectEngine<TStart, TSource1, TResult, SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev
                    ,
                    TPrevState>>>
            Select<TStart, TSource0, TSource1, TPrev, TResult, TPrevState>(
                this SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev,
                    TPrevState> sourcePipeline,
                Func<TSource1, TResult> selector)
            where TPrev : ISpanPipeline<TStart, TSource0>
            where TPrevState : IDelegatePipeline<TStart, TSource0, TSource1, TPrev>
        {
            return new SpanPipelineFixedPoint<TStart, TSource1, TResult,
                SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev, TPrevState>, DelegateSelectEngine<TStart,
                    TSource1, TResult, SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev, TPrevState>>>(
                sourcePipeline,
                new DelegateSelectEngine<TStart, TSource1, TResult,
                    SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev, TPrevState>>(
                    selector));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SpanPipelineFixedPoint<TStart, TSource1, TSource1,
                SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev,
                    TPrevState>,
                DelegateWhereEngine<TStart, TSource1, SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev
                    ,
                    TPrevState>>>
            Where<TStart, TSource0, TSource1, TPrev, TPrevState>(
                this SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev,
                    TPrevState> sourcePipeline,
                Func<TSource1, bool> predicate)
            where TPrev : ISpanPipeline<TStart, TSource0>
            where TPrevState : IDelegatePipeline<TStart, TSource0, TSource1, TPrev>
        {
            return new SpanPipelineFixedPoint<TStart, TSource1, TSource1,
                SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev, TPrevState>, DelegateWhereEngine<TStart,
                    TSource1, SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev, TPrevState>>>(
                sourcePipeline,
                new DelegateWhereEngine<TStart, TSource1,
                    SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev, TPrevState>>(
                    predicate));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SpanPipelineFixedPoint<T, T, TResult, SpanPipelineBuilder<T>,
                BoostSelectEngine<T, T, TResult, SpanPipelineBuilder<T>, TOp>>
            Select<T, TResult, TOp>(this SpanPipelineBuilder<T> sourcePipeline, Op<TOp, T, TResult> selector)
            where TOp : ILinqUnaryOp<T, TResult>
        {
            return new SpanPipelineFixedPoint<T, T, TResult, SpanPipelineBuilder<T>,
                BoostSelectEngine<T, T, TResult, SpanPipelineBuilder<T>, TOp>>
            (sourcePipeline,
                new BoostSelectEngine<T, T, TResult, SpanPipelineBuilder<T>, TOp>(selector.InnerOp)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SpanPipelineFixedPoint<TStart, TSource1, TResult,
                SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev,
                    TPrevState>,
                BoostSelectEngine<TStart, TSource1, TResult, SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev
                    ,
                    TPrevState>, TOp>>
            Select<TStart, TSource0, TSource1, TPrev, TResult, TPrevState, TOp>(
                this SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev,
                    TPrevState> sourcePipeline,
                Op<TOp, TSource1, TResult> selector)
            where TPrev : ISpanPipeline<TStart, TSource0>
            where TPrevState : IDelegatePipeline<TStart, TSource0, TSource1, TPrev>
            where TOp : ILinqUnaryOp<TSource1, TResult>
        {
            return new SpanPipelineFixedPoint<TStart, TSource1, TResult,
                SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev,
                    TPrevState>,
                BoostSelectEngine<TStart, TSource1, TResult, SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev
                    ,
                    TPrevState>, TOp>>(
                sourcePipeline,
                new BoostSelectEngine<TStart, TSource1, TResult, SpanPipelineFixedPoint<TStart, TSource0, TSource1,
                    TPrev
                    ,
                    TPrevState>, TOp>(
                    selector.InnerOp));
        }
    }
}