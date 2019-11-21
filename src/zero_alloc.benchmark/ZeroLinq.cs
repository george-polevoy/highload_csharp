using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using ZeroAlloc.Linq;

namespace zero_alloc.benchmark
{
    [MemoryDiagnoser]
    [GcConcurrent(false)]
    [GcServer(true)]
    public class ZeroLinq
    {
        private const long Iterations = 1_000;
        [Params(1, 2, 8, 16, 32)] public long N;
        private List<long> _list;

        [GlobalSetup]
        public void GlobalSetup() => _list = Enumerable.Repeat(1, (int) N).Select(i => (long) i).ToList();

        [Benchmark(Baseline = true)]
        public long None()
        {
            long s = 0;
            Span<long> source = stackalloc long[(int) N];
            for (long i = 0; i < Iterations; i++)
                foreach (var x in source)
                    s += 1 + x + 2 + x;
            return s;
        }

        /// <summary>
        /// SpanEnumeratorCheat factors pipeline initialization out of the loop.
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public long SpanEnumeratorCheat()
        {
            var s = 0L;
            Span<int> source = stackalloc int[(int) N];
            var pipeline = SpanLinq.StartWith<int>()
                .Select(i => i + 1)
                .Select(i => 2 + i);

            for (var i = 0L; i < Iterations; i++)
                foreach (var x in source.Apply(pipeline))
                    s += x;
            return s;
        }

        [Benchmark]
        public long SpanEnumeratorFair()
        {
            var s = 0L;
            Span<int> source = stackalloc int[(int) N];

            for (var i = 0L; i < Iterations; i++)
                foreach (var x in source.Apply(
                    SpanLinq.StartWith<int>()
                        .Select(i1 => i1 + 1)
                        .Select(i2 => 2 + i2)))
                    s += x;

            return s;
        }

        [Benchmark]
        public long List()
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