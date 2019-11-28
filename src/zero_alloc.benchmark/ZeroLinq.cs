using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using ZeroAlloc.Linq;
using ZeroAlloc.Linq.Boost;

namespace zero_alloc.benchmark
{
    [MemoryDiagnoser]
    [GcConcurrent(false)]
    [GcServer(true)]
    public class ZeroLinq
    {
        private const long Iterations = 1_024;
        [Params(1, 2, 128)] public long N;
        private List<long> _list;

        [GlobalSetup]
        public void GlobalSetup() => _list = Enumerable.Repeat(1, (int) N).Select(i => (long) i).ToList();

        [Benchmark(Baseline = true)]
        public long SpanForeach()
        {
            long s = 0;
            Span<long> source = stackalloc long[(int) N];
            for (long i = 0; i < Iterations; i++)
                foreach (var x in source)
                    s += 1 + x + 2;
            return s;
        }

        [Benchmark]
        public long ListForeach()
        {
            long s = 0;
            var source = _list;
            for (long i = 0; i < Iterations; i++)
                foreach (var x in source)
                    s += 1 + x + 2;
            return s;
        }

        struct None
        {
        }

        struct CustomSelector : ILinqUnaryOp<long, long, None>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public long Invoke(long arg, ref None state)
            {
                return arg + 1 + 2;
            }
        }

        [Benchmark]
        public long SpanValueFuncFactored()
        {
            var s = 0L;
            Span<long> source = stackalloc long[(int) N];
            var builder = SpanLinq.StartWith<long, None>();

            var pipeline = builder.Select(
                builder.FromOperation(new CustomSelector(), 0L));

            var spanEnumerable = source.Apply(pipeline, new None());

            for (var i = 0L; i < Iterations; i++)
            {
                foreach (var x in spanEnumerable)
                    s += x;
            }

            return s;
        }

        [Benchmark]
        public long SpanBoost2LinqFactored()
        {
            var s = 0L;
            Span<long> source = stackalloc long[(int) N];
            var builder = SpanLinq.StartWith<long, None>();
            var o = builder.Operations();
            var selector = o.Plus(
                o.Plus(
                    o.Param<long>(),
                    o.Const<long, long>(1)),
                o.Const<long, long>(2)
            );

            var pipeline = builder.Select(selector);
            var spanEnumerable = source.Apply(pipeline, new None());
            for (var i = 0L; i < Iterations; i++)
            {
                foreach (var x in spanEnumerable)
                    s += x;
            }

            return s;
        }

        [Benchmark]
        public long SpanBoostLinqFactored()
        {
            var s = 0L;
            Span<long> source = stackalloc long[(int) N];
            var builder = SpanLinq.StartWith<long, None>();
            var o = builder.Operations();

            var plusA = o.Plus(
                o.Param<long>(),
                o.Const<long, long>(1));
            var plusB = o.Plus(
                o.Param<long>(),
                o.Const<long, long>(2));

            SpanPipelineFixedPoint<long, long, long,
                SpanPipelineFixedPoint<long, long, long, SpanPipelineBuilder<long, None>, BoostSelectEngine<long, long,
                    long, SpanPipelineBuilder<long, None>,
                    LongPlusLong<long, Param<long, None>, Const<long, long, None>, None>, None>, None>,
                BoostSelectEngine<long, long, long, SpanPipelineFixedPoint<long, long, long,
                        SpanPipelineBuilder<long, None>,
                        BoostSelectEngine<long, long, long, SpanPipelineBuilder<long, None>,
                            LongPlusLong<long, Param<long, None>, Const<long, long, None>, None>, None>, None>,
                    LongPlusLong<long, Param<long, None>, Const<long, long, None>, None>, None>, None> pipeline;
            
            
            pipeline = builder
                .Select(plusA)
                .Select(plusB);

            var spanEnumerable = source.Apply(pipeline, new None());

            for (var i = 0L; i < Iterations; i++)
            {
                foreach (var x in spanEnumerable)
                    s += x;
            }

            return s;
        }

        /// <summary>
        /// SpanEnumeratorCheat factors pipeline initialization out of the loop.
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public long SpanLinqFactored()
        {
            var s = 0L;
            Span<long> source = stackalloc long[(int) N];
            var pipeline = SpanLinq.StartWith<long, (long a, long b)>()
                .Select((state, x) => x + state.a)
                .Select((state, x) => x + state.b);

            var spanEnumerable = source.Apply(pipeline, (a: 1, b: 2));

            for (var i = 0L; i < Iterations; i++)
            {
                foreach (var x in spanEnumerable)
                    s += x;
            }
            
            return s;
        }

        IEnumerable<long> GetItems()
        {
            yield return 1;
            yield return 2;
        }

        [Benchmark]
        public long SpanLinqFair()
        {
            var s = 0L;
            Span<long> source = stackalloc long[(int) N];

            for (var i = 0L; i < Iterations; i++)
            {
                var pipeline = SpanLinq.StartWith<long, (long a, long b)>()
                    .Select((state, x) => x + state.a)
                    .Select((state, x) => x + state.b);
                foreach (var x in source.Apply(
                    pipeline, (1, 2)))
                    s += x;
            }

            return s;
        }

        [Benchmark]
        public long ListLinq()
        {
            long s = 0;
            for (long i = 0; i < Iterations; i++)
            {
                var a = 1L;
                var b = 2L;
                s = _list
                    .Select(x => x + a)
                    .Select(x => 2 + b)
                    .Sum();
            }

            return s;
        }
    }
}