using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Methods;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

[RPlotExporter, RankColumn, MemoryDiagnoser, ShortRunJob]
    public class ReplacerBenchmark
    {
        const int NumWords = 64;
        const int PercentReplaced = 50;
        const int JobSize = 500;
        const int NumJobs = 500;
        private int _numJobsLeft;

        double correlation_factor = 1.97;

        [Params(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 13, 14, 15, 16)]
        public int NumThreads { get; set; }
        private IReplace NaiveReplacer;
        private IReplace NaiveReplacer2;
        private ZeroAllocReplacer ZeroAllocReplacer;
        private string source;
        private ReadOnlyMemory<char> _sourceMemory;
        [GlobalSetup]
        public void Setup()
        {
            var words = GenerateManyUnique().Take(NumWords).ToArray();
            var replacerBuilder = new NaiveReplacerBuilder();
            var replacements = new Dictionary<string, string>();
            source = string.Join(' ', words);
            _sourceMemory = source.AsMemory();
            var r = new Random(NumWords + PercentReplaced);
            FisherYatesShuffle(words, r);
            var replaced = words.Take(NumWords * PercentReplaced / 100).ToArray();
            foreach (var x in replaced)
            {
                replacements.Add(x, "replaced");
                replacerBuilder.Add(x, "replaced");
            }
            NaiveReplacer = replacerBuilder.BuildReplacer();
            NaiveReplacer2 = replacerBuilder.BuildReplacer2();
            ZeroAllocReplacer = new ZeroAllocReplacer(replacements);
            _numJobsLeft = NumJobs;
        }

        [Benchmark(Baseline = true)]
        public string NaiveReplace()
        {
            return MultithreadedBench(buffer=> NaiveReplacer.Replace(source), 1.0);
        }

        [Benchmark]
        public string ZA_Replace()
        {
            return MultithreadedBench(buffer=>
            {
                ZeroAllocReplacer.Replace(_sourceMemory, buffer);
                return "";
            }, correlation_factor);
        }

        private IEnumerable<string> GenerateManyUnique() {
            return
                from a in new[] {"a", "b", "c", "d", "e", "f"}
                from b in new[] {"a", "b", "c", "d", "e", "f"}
                from c in new[] {"a", "b", "c", "d", "e", "f"}
                from d in new[] {"a", "b", "c", "d", "e", "f"}
                select a + b + c + d;
        }

        void FisherYatesShuffle<T>(T[] a, Random r) {
            var n = a.Length;
            for (var i = 0; i < n - 1; i++) {
                var j = r.Next(0, n - i);
                Swap(ref a[i], ref a[j]);
            }
        }

        void Swap<T>(ref T a, ref T b) {
            T t = a;
            a = b;
            b = t;
        }

        T MultithreadedBench<T>(Func<Memory<char>, T> func, double correlation_factor) where T: class {
            _numJobsLeft = NumJobs;
            var tasks = Enumerable
            .Range(0, NumThreads)
            .Select(ti => Task.Factory.StartNew(() =>
            {
                T last = default(T);

                var buffer = new Memory<char>(new char[1024 * 100]);

                var realJobSize = (int)((double)JobSize * correlation_factor);
                while (Interlocked.Decrement(ref _numJobsLeft) > 0)
                {
                    for (var i = 0; i < realJobSize; i++) {
                        last = func(buffer);
                        GC.KeepAlive(last);
                    }
                }
                return last;
            }, TaskCreationOptions.LongRunning)).ToArray();
            Task.WhenAll(tasks).Wait();
            return tasks.First().Result;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var replacerBuilder = new NaiveReplacerBuilder();
            replacerBuilder.Add("Hello", "Good bye");
            replacerBuilder.Add("World", "hell");
            var replacer = replacerBuilder.BuildReplacer();
            Console.WriteLine(replacer.Replace("Hello World!"));

            var summary = BenchmarkRunner.Run<ReplacerBenchmark>();
        }
    }
