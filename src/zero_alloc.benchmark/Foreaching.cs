using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;

namespace zero_alloc.benchmark
{
    [MemoryDiagnoser]
    [GcConcurrent(false)]
    [GcServer(true)]
    public class Foreaching
    {
        private const long Iterations = 1_024;
        [Params(1, 4, 16)] public long N;
        private List<long> _list;
        private long[] _array;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _list = Enumerable.Repeat(1, (int) N).Select(i => (long) i).ToList();
            _array = _list.ToArray();
        }

        [Benchmark]
        public long ListForeach()
        {
            var s = 0L;
            for (var i = 0; i < Iterations; i++)
            {
                s = 0;
                foreach (var x in _list)
                {
                    s += x;
                }
            }

            return s;
        }
        
        [Benchmark]
        public long ListMarshalSpanForeach()
        {
            var s = 0L;
            for (var i = 0; i < Iterations; i++)
            {
                s = 0;
                var span = CollectionsMarshal.AsSpan(_list);
                foreach (var x in span)
                {
                    s += x;
                }
            }
            
            return s;
        }
        
        [Benchmark]
        public long ArrayAsSpanForeach()
        {
            var s = 0L;
            for (var i = 0; i < Iterations; i++)
            {
                s = 0;
                var span = _array.AsSpan();
                foreach (var x in span)
                {
                    s += x;
                }
            }
            
            return s;
        }
        
        [Benchmark(Baseline = true)]
        public long SpanForeach()
        {
            var span = _array.AsSpan();
            var s = 0L;
            for (var i = 0; i < Iterations; i++)
            {
                s = 0;
                foreach (var x in span)
                {
                    s += x;
                }
            }
            
            return s;
        }
        
        [Benchmark]
        public long ArrayForeach()
        {
            var s = 0L;
            for (var i = 0; i < Iterations; i++)
            {
                s = 0;
                foreach (var x in _array)
                {
                    s += x;
                }
            }
            
            return s;
        }
        
        [Benchmark(Baseline = true)]
        public long ArrayFor()
        {
            var s = 0L;
            for (var i = 0; i < Iterations; i++)
            {
                s = 0;
                for (var j = 0; j < _array.Length; j++)
                {
                    s += _array[j];
                }
            }
            
            return s;
        }
    }
}