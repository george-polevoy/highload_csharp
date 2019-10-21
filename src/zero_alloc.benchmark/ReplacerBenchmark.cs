using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using TextExtensions;

namespace zero_alloc.benchmark
{
    [RPlotExporter]
    [RankColumn]
    [MemoryDiagnoser]
    [ShortRunJob]
    public class ReplacerBenchmark
    {
        private const int NumWords = 64;
        private const int PercentReplaced = 50;
        private const int JobSize = 500;
        private const int NumJobs = 500;
        private NaiveReplacer _naiveReplacer;
        private int _numJobsLeft;
        private string _source;
        private ReadOnlyMemory<char> _sourceMemory;
        private ZeroAllocReplacer _zeroAllocReplacer;

        [Params(1, 2, 3, 4, 5, 6, 7, 8)] public int NumThreads { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            var words = GenerateManyUnique().Take(NumWords).ToArray();
            _source = string.Join(' ', words);
            _sourceMemory = _source.AsMemory();
            var r = new Random(NumWords + PercentReplaced);
            FisherYatesShuffle(words, r);

            _naiveReplacer = new NaiveReplacer(token =>
                token.StartsWith("a") ? token.Length + token : token);

            _zeroAllocReplacer = new ZeroAllocReplacer((dst, src) =>
            {
                var len = src.Length;
                if (src.StartsWith("a"))
                {
                    if (!len.TryFormat(dst, out var charsWritten))
                        throw new ArgumentException("Destination is shorter than needed");
                    dst = dst.Slice(charsWritten);
                }

                src.CopyTo(dst);
                dst = dst.Slice(src.Length);
                return dst;
            });
            _numJobsLeft = NumJobs;
        }

        [Benchmark(Baseline = true)]
        public string NaiveReplace()
        {
            return MultithreadedBench(buffer => _naiveReplacer.Replace(_source));
        }

        [Benchmark]
        public string ZA_Replace()
        {
            return MultithreadedBench(buffer =>
            {
                _zeroAllocReplacer.Replace(buffer, _sourceMemory.Span);
                return "";
            });
        }

        private IEnumerable<string> GenerateManyUnique()
        {
            return
                from a in new[] {"a", "b"}
                from b in new[] {"a", "b", "c", "d", "e", "f"}
                from c in new[] {"a", "b", "c", "d", "e", "f"}
                from d in new[] {"a", "b", "c", "d", "e", "f"}
                select string.Join("", Enumerable.Repeat(a + b + c + d, 8));
        }

        private void FisherYatesShuffle<T>(T[] a, Random r)
        {
            var n = a.Length;
            for (var i = 0; i < n - 1; i++)
            {
                var j = r.Next(0, n - i);
                Swap(ref a[i], ref a[j]);
            }
        }

        private void Swap<T>(ref T a, ref T b)
        {
            var t = a;
            a = b;
            b = t;
        }

        private T MultithreadedBench<T>(Func<Memory<char>, T> func) where T : class
        {
            _numJobsLeft = NumJobs;
            var tasks = Enumerable
                .Range(0, NumThreads)
                .Select(ti => Task.Factory.StartNew(() =>
                {
                    T last = default;
                    var buffer = new Memory<char>(new char[1024 * 100]);
                    var realJobSize = JobSize;
                    while (Interlocked.Decrement(ref _numJobsLeft) > 0)
                        for (var i = 0; i < realJobSize; i++)
                        {
                            last = func(buffer);
                            GC.KeepAlive(last);
                        }

                    return last;
                }, TaskCreationOptions.LongRunning)).ToArray();
            Task.WhenAll(tasks).Wait();
            return tasks.First().Result;
        }
    }
}