using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using ZeroAlloc.Linq;
using ZeroAlloc.Linq.Boost;

namespace zero_alloc.benchmark
{
    [MemoryDiagnoser]
    [GcConcurrent(false)]
    [GcServer(true)]
    public class ZeroLinq
    {
        private const long Iterations = 1_000;
        [Params(1, 2, 32)] public long N;
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
        
        [Benchmark]
        public long SpanBoostLinqFactored()
        {
            var s = 0L;
            Span<long> source = stackalloc long[(int) N];
            var pipeline = SpanLinq.StartWith<long>()
                .Select(Arg.Long + 1)
                .Select(Arg.Long + 2);

            var spanEnumerable = source.Apply(pipeline);
            
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
            
            var selector = Operations.Plus(
                Operations.Plus(
                    Operations.Param<long>(),
                    Operations.Const<long, long>(1)),
                Operations.Const<long, long>(2)
            );
            
            var pipeline = SpanLinq.StartWith<long>()
                .Select(
                    selector
                );
            
            var spanEnumerable = source.Apply(pipeline);
            
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
            var pipeline = SpanLinq.StartWith<long>()
                .Select(x => x + 1)
                .Select(x => 2 + x);
            
            var spanEnumerable = source.Apply(pipeline);
            
            for (var i = 0L; i < Iterations; i++)
            {
                foreach (var x in spanEnumerable)
                    s += x;
            }

            return s;
        }

        [Benchmark]
        public long SpanLinqFair()
        {
            var s = 0L;
            Span<long> source = stackalloc long[(int) N];

            for (var i = 0L; i < Iterations; i++)
            {
                var pipeline = SpanLinq.StartWith<long>()
                    .Select(x => x + 1)
                    .Select(x => 2 + x);
                foreach (var x in source.Apply(
                    pipeline))
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
                s = _list
                    .Select(x => x + 1)
                    .Select(x => 2 + x)
                    .Sum();
            }

            return s;
        }
    }
}