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
    public class OmniLinq
    {
        private const long Iterations = 1_000;
        
        [Params(1, 2, 8, 16, 32)] public long N;

        private List<long> _list;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _list = Enumerable.Repeat(1, (int)N).Select(i => (long)i).ToList();
        }

        [Benchmark(Baseline = true)]
        public long List()
        {
            long s = 0;
            long addition = 3;
            for (long i = 0; i < Iterations; i++)
            {
                s = _list
                    .Select(x => x + N)
                    .Select(x => addition + x)
                    .Sum();
            }

            return s;
        }

//        [Benchmark]
//        public long ListOmni()
//        {
//            long s = 0;
//            for (long i = 0; i < Iterations; i++)
//            {
//                s = _list
//                    .Select(N, (n, i) => n + i)
//                    .Select(3, (addition, i) => addition + i)
//                    .GetEnumerator()
//                    .Sum();
//            }
//
//            return s;
//        }

//        [Benchmark]
//        public long ListProviderEnumerator()
//        {
//            var s = 0L;
//            for (var i = 0L; i < Iterations; i++)
//            {
//                s = _list.SelectWithProvider(N, (n, i) => n + i)
//                    //.Select1(3, (int addition, int x) => addition + x)
//                    .Sum();
//            }
//
//            return s;
//        }
        
        [Benchmark]
        public long SpanEnumerator()
        {
            var s = 0L;
            Span<int> source = stackalloc int[(int)N];
            var pipeline = SpanLinq.StartWith<int>()
                .Select(i => i + 3)
                .Select(i => 2 + i);
            
            for (var i = 0L; i < Iterations; i++)
            {
                var ie = source.GetEnumerator();
                while (pipeline.MoveNext(ref ie))
                {
                    var current = pipeline.GetCurrent(ref ie);
                    s += current;
                }
            }

            return s;
        }
    }
}