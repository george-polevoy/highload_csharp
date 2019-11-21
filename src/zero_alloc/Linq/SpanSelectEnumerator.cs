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
        public static SpanPipelineFixedPoint<T, T, TResult, SpanPipelineBuilder<T>>
            Select<T, TResult>(this SpanPipelineBuilder<T> sourcePipeline, Func<T, TResult> selector)
        {
            return new SpanPipelineFixedPoint<T, T, TResult, SpanPipelineBuilder<T>>
            (sourcePipeline, selector);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SpanPipelineFixedPoint<TStart, TSource1, TResult,
                SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev>>
            Select<TStart, TSource0, TSource1, TPrev, TResult>(
                this SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev> sourcePipeline,
                Func<TSource1, TResult> selector)
            where TPrev : ISpanPipeline<TStart, TSource0>
        {
            return new SpanPipelineFixedPoint<TStart, TSource1, TResult,
                SpanPipelineFixedPoint<TStart, TSource0, TSource1, TPrev>>
            (sourcePipeline, selector);
        }
    }

    /// <summary>
    /// ISpanEnumerator{TStart, T} marker interface for off-tear enumeration.
    /// </summary>
    /// <typeparam name="TStart"></typeparam>
    /// <typeparam name="T"></typeparam>
    public interface ISpanPipeline<TStart, out T>
    {
        bool MoveNext(ref Span<TStart>.Enumerator enumerator);
        T GetCurrent(ref Span<TStart>.Enumerator enumerator);
    }

    /// <summary>
    /// SpanEnumerationBuilder starting point of enumeration. 
    /// </summary>
    /// <typeparam name="T">The element type of a <see cref="Span{T}"/>
    /// which will be the source of enumeration.
    /// </typeparam>
    public readonly struct SpanPipelineBuilder<T> : ISpanPipeline<T, T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext(ref Span<T>.Enumerator enumerator)
        {
            return enumerator.MoveNext();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetCurrent(ref Span<T>.Enumerator enumerator)
        {
            return enumerator.Current;
        }
    }

    public struct SpanPipelineFixedPoint<TStart1, TStart2, T, TCombinationInterceptor>
        : ISpanPipeline<TStart1, T>
        where TCombinationInterceptor : ISpanPipeline<TStart1, TStart2>
    {
        private readonly Func<TStart2, T> _context;
        private TCombinationInterceptor _interceptor;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal SpanPipelineFixedPoint(TCombinationInterceptor interceptor, Func<TStart2, T> context)
        {
            _interceptor = interceptor;
            _context = context;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext(ref Span<TStart1>.Enumerator enumerator)
        {
            return _interceptor.MoveNext(ref enumerator);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetCurrent(ref Span<TStart1>.Enumerator enumerator)
        {
            return _context(_interceptor.GetCurrent(ref enumerator));
        }
    }
}