using System;
using BenchmarkDotNet.Running;

namespace zero_alloc.benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            // apply memory pressure to force more often GC.
            // as reported by linux `free` command.
            var freeKib = 31547284;
            var allowedKib = 300_000;
            GC.AddMemoryPressure((freeKib - allowedKib) * 1024);
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }
}