using System;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using BenchmarkDotNet.Attributes;

namespace zero_alloc.benchmark
{
    [MemoryDiagnoser]
    [RPlotExporter]
    public class BenchStructLength
    {
        [Params( 2, 4, 6, 8, 10, 12, 14, 16)]
        public int Length;

        private string _stringSource;

        private Ws16 _structSource;
        
        [GlobalSetup]
        public void GlobalSetup()
        {
            _stringSource = string.Create(16, Length, (span, len) =>
            {
                for (var i = 0; i < len; i++)
                {
                    span[i] = 'a';
                }
            });
            _stringSource.AsSpan().CopyTo(_structSource.Span());
        }
        
        [Benchmark(Baseline = true)]
        public int String()
        {
            return _stringSource.NullTerminatedLength();
        }
        
        /*[Benchmark]
        public int Struct()
        {
            return _structSource.Length();
        }
        */

        [Benchmark]
        public int X86WithBranch()
        {
            return _structSource.CustomLength();
        }
        
        [Benchmark]
        public int Sse128()
        {
            return _structSource.AvxLength();
        }
        
        [Benchmark]
        public int Sse128WithBranch()
        {
            return _structSource.AvxLength2();
        }
        
        [Benchmark]
        public int Avx256()
        {
            return _structSource.AvxLength3();
        }
    }
}