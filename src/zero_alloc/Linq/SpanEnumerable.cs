using System;
using System.Runtime.CompilerServices;

namespace ZeroAlloc.Linq
{
    public ref struct SpanEnumerable<TStart1, TStart2, T, TCombinationInterceptor, TState, TSelectorState>
        where TCombinationInterceptor : ISpanPipeline<TStart1, TStart2, TSelectorState>
        where TState : IDelegatePipeline<TStart1, TStart2, T, TCombinationInterceptor, TSelectorState>
    {
        private readonly SpanPipelineFixedPoint<TStart1, TStart2, T, TCombinationInterceptor, TState, TSelectorState> _pipeline;
        private readonly Span<TStart1> _source;
        private readonly TSelectorState _state;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SpanEnumerable(
            SpanPipelineFixedPoint<TStart1, TStart2, T, TCombinationInterceptor, TState, TSelectorState> pipeline,
            Span<TStart1> source,
            TSelectorState state)
        {
            _pipeline = pipeline;
            _source = source;
            _state = state;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator
            GetEnumerator()
        {
            return new Enumerator(_pipeline, _source.GetEnumerator(), _state);
        }

        public ref struct Enumerator
        {
            private SpanPipelineFixedPoint<TStart1, TStart2, T, TCombinationInterceptor, TState, TSelectorState> _pipeline;
            private Span<TStart1>.Enumerator _source;
            private TSelectorState _state;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Enumerator(SpanPipelineFixedPoint<TStart1, TStart2, T, TCombinationInterceptor, TState, TSelectorState> pipeline,
                Span<TStart1>.Enumerator source,
                TSelectorState state)
            {
                _pipeline = pipeline;
                _source = source;
                _state = state;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext() => _pipeline.MoveNext(ref _source, ref _state);

            public T Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _pipeline.GetCurrent(ref _source, ref _state);
            }
        }
    }
}