using System.Threading;
using BenchmarkDotNet.Attributes;

namespace zero_alloc.benchmark
{
    [MemoryDiagnoser]
    public class AsyncLocalBenchmark
    {
        private AsyncLocal<string> _asyncLocal = new AsyncLocal<string>();

        [GlobalSetup]
        public void GlobalSetup()
        {
            _asyncLocal.Value = "hello";
        }
        
        [Benchmark]
        public string Read()
        {
            return _asyncLocal.Value;
        }

        [Benchmark]
        public string Write()
        {
            _asyncLocal.Value = "hello";
            return "hello";
        }

        [Benchmark]
        public string ReadAfterWrite()
        {
            _asyncLocal.Value = "hello";
            return _asyncLocal.Value;
        }
    }
}