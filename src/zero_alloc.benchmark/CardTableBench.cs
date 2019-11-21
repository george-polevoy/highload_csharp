using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
namespace zero_alloc.benchmark
{
    [MemoryDiagnoser]
    [RPlotExporter] 
    [GcServer(true)]
    public class CardTableBench
    {
        private Random _random = new Random();
        
        [Params(1, 2, 4, 8, 16, 32, 64, 128, 256, 1024, 2048, 4096)] public long Size;

        private List<string> _stringList;
        private List<Ws16> _structList;

        [GlobalSetup]
        public void GlobalSetup()
        {
            var size = GetArraySize();
            _stringList = new List<string>(size);
            for (var i = 0; i < size; i++)
            {
                _stringList.Add("");
            }
            _structList = new List<Ws16>(size);
            for (var i = 0; i < size; i++)
            {
                _structList.Add(new Ws16());
            }
        }

        private int GetArraySize()
        {
            return (int) (Size * 1024 * 1024 / 8);
        }

        [Benchmark]
        public long Allocating()
        {
            var size = GetArraySize();
            var l = _stringList;
            var s = 0L;
            var iterationCount = 1024L * 1024 * 8;
            for (var j = 0; j < iterationCount; j++)
            {
                var index = _random.Next(size);
                l[index] = new string('a', 16);
                s += l[index].Length;
                l[index] = null;
            }

            return s;
        }

        [Benchmark(Baseline = true)]
        public long ZeroAlloc()
        {
            var size = (int)(Size * 1024 * 1024 / 8);
            var l = _structList;
            var s = 0L;
            var iterationCount = 1024L * 1024 * 8;
            for (var j = 0; j < iterationCount; j++)
            {
                var index = _random.Next(size);
                l[index].Span().Fill('a');
                s += l[index].CustomLength();
            }

            return s;
        }
    }
}