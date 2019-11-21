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
            return new SpanPipelineFixedPoint<TStart, TSource1, TSource1, SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev, TPrevState>, DelegateWhereEngine<TStart, TSource1, SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev, TPrevState>>>(
                sourcePipeline, new DelegateWhereEngine<TStart, TSource1, SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev, TPrevState>>(
                    predicate));
        }
    }

    public interface IDelegatePipeline<TStart, out T0, out T1, TPrev>
        where TPrev : ISpanPipeline<TStart, T0>
    {
        bool MoveNext(ref Span<TStart>.Enumerator enumerator, ref TPrev prev);
        T1 GetCurrent(ref Span<TStart>.Enumerator enumerator, ref TPrev prev);
    }

    public struct DelegateSelectEngine<TStart, T0, T1, TPrev> : IDelegatePipeline<TStart, T0, T1, TPrev>
        where TPrev : ISpanPipeline<TStart, T0>
    {
        private readonly Func<T0, T1> _selector;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DelegateSelectEngine(Func<T0, T1> selector)
        {
            _selector = selector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext(ref Span<TStart>.Enumerator enumerator, ref TPrev prev)
        {
            return prev.MoveNext(ref enumerator);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T1 GetCurrent(ref Span<TStart>.Enumerator enumerator, ref TPrev prev)
        {
            return _selector(prev.GetCurrent(ref enumerator));
        }
    }
    
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
