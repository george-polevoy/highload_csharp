using System;
using System.Runtime.CompilerServices;

namespace ZeroAlloc.Linq
{
    public ref struct SpanEnumerable<TStart1, TStart2, T, TCombinationInterceptor, TState>
        where TCombinationInterceptor : ISpanPipeline<TStart1, TStart2>
        where TState : IDelegatePipeline<TStart1, TStart2, T, TCombinationInterceptor>
    {
        private SpanPipelineFixedPoint<TStart1, TStart2, T, TCombinationInterceptor, TState> _pipeline;
        private Span<TStart1> _source;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SpanEnumerable(
            SpanPipelineFixedPoint<TStart1, TStart2, T, TCombinationInterceptor, TState> pipeline, Span<TStart1> source)
        {
            _pipeline = pipeline;
            _source = source;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator
            GetEnumerator()
        {
            return new Enumerator(_pipeline, _source.GetEnumerator());
        }

        public ref struct Enumerator
        {
            private SpanPipelineFixedPoint<TStart1, TStart2, T, TCombinationInterceptor, TState> _pipeline;
            private Span<TStart1>.Enumerator _source;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Enumerator(SpanPipelineFixedPoint<TStart1, TStart2, T, TCombinationInterceptor, TState> pipeline,
                Span<TStart1>.Enumerator source)
            {
                _pipeline = pipeline;
                _source = source;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext() => _pipeline.MoveNext(ref _source);

            public T Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _pipeline.GetCurrent(ref _source);
            }
        }
    }
}