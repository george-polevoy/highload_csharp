using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BenchmarkDotNet.Attributes;
using ZeroAlloc;
using ZeroAlloc.Text;

namespace zero_alloc.benchmark
{
    
    [RPlotExporter]
    [RankColumn]
    [MemoryDiagnoser]
    [GcServer(true)]
    //[GcConcurrent(true)]
    public class ReplacerBenchmark
    {
        private const int NumWords = 64;
        private const int PercentReplaced = 50;
        private const int JobSize = 500;
        private const int NumJobs = 1000;
        private NaiveReplacer _naiveReplacer;
        private int _numJobsLeft;
        private string _source;
        private ReadOnlyMemory<char> _sourceMemory;
        private ZeroAllocReplacer _zeroAllocReplacer;
        private CountdownEvent _threadReady;
        private ManualResetEventSlim _operationScheduled;
        private ManualResetEventSlim _operationAcknowledged;
        private CancellationToken _cancellationToken;
        private CancellationTokenSource _cancellationTokenSource;

        private Func<Memory<char>, string> _currentJob;

        private string _result;
        private CountdownEvent _threadCompletedOperation;
        private Int64[] _large = new Int64[10_000_000_000 / 8];
        
        [Params(1, 2, 4, 8, 16, 32, 64)] public int NumThreads { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            Log("setting up...");
            var words = GenerateManyUnique().Take(NumWords).ToArray();
            _source = string.Join(' ', words);
            _sourceMemory = _source.AsMemory();
            var r = new Random(NumWords + PercentReplaced);
            FisherYatesShuffle(words.AsSpan(), r);
            _naiveReplacer = Replacers.CreateNaiveReplacer();
            _zeroAllocReplacer = Replacers.CreateZeroAllocReplacer();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _operationScheduled = new ManualResetEventSlim();
            _operationAcknowledged = new ManualResetEventSlim();
            _threadReady = new CountdownEvent(NumThreads);
            _threadCompletedOperation = new CountdownEvent(NumThreads);
            for (var iT = 0; iT < NumThreads; iT++)
            {
                var thread = new Thread(ThreadProc) {IsBackground = true};
                thread.Start();
            }
            _threadReady.Wait();
        }

        [IterationSetup]
        public void IterationSetup()
        {
            _threadReady.Wait();
            _threadReady.Reset();
            _operationAcknowledged.Reset();
            
            for (var i = 0; i < _large.Length; i++)
            {
                _large[i] = i;
            }
        }

        [IterationCleanup]
        public void IterationCleanup()
        {
            _threadCompletedOperation.Reset();
            _operationScheduled.Reset();
            _threadReady.Reset();
            Log("Acknowledging operation.");
            _operationAcknowledged.Set();
        }

        private void Log(string what)
        {
            //Console.WriteLine(what);
        }
        
        
        private void ThreadProc(object state)
        {
            Log("entering thread proc");
            
            while (!_cancellationToken.IsCancellationRequested)
            {
                _threadReady.Signal();
                Log("waiting for sheduling...");
                _operationScheduled.Wait();
                Log("acquired scheduling.");
                HandleIteration();
                Log("signalling completion");
                _threadCompletedOperation.Signal();
                Log("waiting for ack...");
                _operationAcknowledged.Wait();
            }

            Log("finished thread proc.");
        }
        
        private string MultithreadedBench(Func<Memory<char>, string> func)
        {
            _numJobsLeft = NumJobs;
            _currentJob = func;
            Log("starting operation.");
            _operationScheduled.Set();
            
            Log("waiting for completion.");
            _threadCompletedOperation.Wait();
            return _result;
        }

        
        private void HandleIteration()
        {
            string last = default;
            var buffer = new Memory<char>(new char[1024 * 100]);
            while (Interlocked.Decrement(ref _numJobsLeft) > 0)
            {
                for (var i = 0; i < JobSize; i++)
                {
                    last = _currentJob(buffer);
                    GC.KeepAlive(last);
                }
            }
            _result = last;
        }


        [GlobalCleanup]
        public void Cleanup()
        {
            for (var i = 0; i < _large.Length; i++)
            {
                _large[i] = i;
            }
            Log("canceling thread cycles.");
            _cancellationTokenSource.Cancel();
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
                _zeroAllocReplacer.Replace(buffer.Span, _sourceMemory.Span, out _);
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

        private void FisherYatesShuffle<T>(Span<T> a, Random r)
        {
            var n = a.Length;
            for (var i = 0; i < n - 1; i++)
            {
                var j = r.Next(0, n - i);
                (a[i], a[j]) = (a[j], a[i]);
            }
        }
    }
}