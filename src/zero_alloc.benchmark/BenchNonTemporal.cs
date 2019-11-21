using System;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using BenchmarkDotNet.Attributes;

namespace zero_alloc.benchmark
{
    [RPlotExporter]
    public class BenchNonTemporal
    {
        private const int N = 1024 * 1024 * 32;

        private static IntPtr _storage = Marshal.AllocHGlobal(N * 32 + 128);


        [Benchmark]
        public void NonTemporal()
        {
            var buffer = new StreamingCircularBuffer(_storage);
            buffer.Store(N);
        }

        [Benchmark(Baseline = true)]
        public void Simple()
        {
            var buffer = new StreamingCircularBuffer(_storage);
            buffer.StoreSimple(N);
        }

        ref struct StreamingCircularBuffer
        {
            private IntPtr _source;

            public StreamingCircularBuffer(IntPtr source)
            {
                _source = source;
            }

            public unsafe void Store(int n)
            {
                //Console.WriteLine("hello");
                //fixed (char* rawPtr = _source)
                {
                    Span<long> temp = new long[4];

                    fixed (void* pTemp = temp)
                    {
                        var aligned = new IntPtr(32 * (((long) _source + 31) / 32));
                        for (var i = 0; i < n; i++)
                        {
                            var p = aligned + (i * 32); // + i * 32;
                            //Console.WriteLine(ptr1);

                            /*
                            temp[0] = i * 100;
                            temp[1] = temp[0] / 123;
                            temp[2] = temp[1] * 375;
                            temp[3] = temp[2] / 537;
                            var x = Avx2.LoadVector256((long*)pTemp);
                            */
                            var t = i * 100;
                            var x = Vector256<long>.Zero.WithElement(0, t);
                            t = t / 123;
                            x = x.WithElement(1, t);
                            t = t * 375;
                            x = x.WithElement(2, t);
                            t = t / 537;
                            x = x.WithElement(3, t);
                            
                            /*x = x.WithElement(0, (short) ('a' + n % 26));
                            x = x.WithElement(1, (short) ('a' + n % 26));
                            x = x.WithElement(2, (short) ('a' + n % 26));
                            x = x.WithElement(3, (short) ('a' + n % 26));
                            x = x.WithElement(4, (short) ('a' + n % 26));
                            x = x.WithElement(5, (short) ('a' + n % 26));
                            x = x.WithElement(6, (short) ('a' + n % 26));
                            x = x.WithElement(7, (short) ('a' + n % 26));
                            x = x.WithElement(8, (short) ('a' + n % 26));
                            x = x.WithElement(9, (short) ('a' + n % 26));
                            x = x.WithElement(10, (short) ('a' + n % 26));
                            x = x.WithElement(11, (short) ('a' + n % 26));
                            x = x.WithElement(12, (short) ('a' + n % 26));
                            x = x.WithElement(13, (short) ('a' + n % 26));
                            x = x.WithElement(14, (short) ('a' + n % 26));
                            x = x.WithElement(15, (short) ('a' + n % 26));
                            */

                            Avx2.StoreAlignedNonTemporal((byte*) p, x.AsByte());
                        }
                    }

                    //bufferHandle.Free();
                }
            }

            public unsafe void StoreSimple(int n)
            {
                var span = new Span<char>((void*) _storage, N * 16);
                for (var i = 0; i < n; i++)
                {
                    var temp = MemoryMarshal.Cast<char, long>(span.Slice(i * 16));

                    temp[0] = i * 100;
                    temp[1] = temp[0] / 123;
                    temp[2] = temp[1] * 375;
                    temp[3] = temp[2] / 537;

                    /*
                    start[0] = (char) ('a' + n % 26);
                    start[1] = (char) ('a' + n % 26);
                    start[2] = (char) ('a' + n % 26);
                    start[3] = (char) ('a' + n % 26);
                    start[4] = (char) ('a' + n % 26);
                    start[5] = (char) ('a' + n % 26);
                    start[6] = (char) ('a' + n % 26);
                    start[7] = (char) ('a' + n % 26);
                    start[8] = (char) ('a' + n % 26);
                    start[9] = (char) ('a' + n % 26);
                    start[10] = (char) ('a' + n % 26);
                    start[11] = (char) ('a' + n % 26);
                    start[12] = (char) ('a' + n % 26);
                    start[13] = (char) ('a' + n % 26);
                    start[14] = (char) ('a' + n % 26);
                    start[15] = (char) ('a' + n % 26);
                    */
                }
            }
        }
    }
}