using BenchmarkDotNet.Attributes;

namespace zero_alloc.benchmark
{
    [MemoryDiagnoser]
    public class ObjectSizeBench
    {
        [Benchmark]
        public string CreateOneCharString()
        {
            return new string('-', 1);
        }
    }
}