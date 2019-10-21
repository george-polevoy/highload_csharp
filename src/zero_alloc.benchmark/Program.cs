using BenchmarkDotNet.Running;

namespace zero_alloc.benchmark
{
    class Program
    {
        static void Main(string[] args) 
            => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}