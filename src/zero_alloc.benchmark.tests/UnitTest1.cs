using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using TextExtensions;
using Xunit;
using ZeroAlloc.Linq;

namespace zero_alloc.benchmark.tests
{
    public class UnitTest1
    {
        [Fact]
        public void NaiveReplacerAddsLengthPrefixToWordsStartingWithA()
        {
            var replacer = Replacers.CreateNaiveReplacer();
            var actual = replacer.Replace("hello any");
            var expected = "hello 3any";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ZeroAllocReplacerAddsLengthPrefixToWordsStartingWithA()
        {
            var replacer = Replacers.CreateZeroAllocReplacer();
            Span<char> dst = stackalloc char[100];
            replacer.Replace(dst, "hello any".AsSpan(), out var charsWritten);
            var expected = "hello 3any";
            Assert.Equal(expected, new string(dst[..charsWritten]));
        }

        [Fact]
        public void Ws16_Length()
        {
            for (var i = 0; i <= 16; i++)
            {
                var x = new Ws16();
                for (var j = 0; j < i; j++)
                {
                    x.Span()[j] = 'a';
                }

                Assert.Equal(i, x.CustomLength());
                Assert.Equal(i, x.Length());
            }
        }

        [Fact]
        public unsafe void AvxStrlen16()
        {
            Span<char> s = stackalloc char[16];
            "hello".AsSpan().CopyTo(s); // 5 chars
            fixed (void* p = s)
            {
                var a = Avx2.LoadVector256((short*) p);
                var cmp = Avx2.CompareGreaterThan(a, Vector256<short>.Zero);
                var cmpBytes = cmp.AsByte();
                var mask = Avx2.MoveMask(cmpBytes);
                var popCount = (int) Popcnt.PopCount((uint) mask);
                Assert.Equal("hello".Length, popCount / 2);
            }
        }

        [Fact]
        public void WriteBuffer()
        {
            var source = "hello";
            var charSpan = source.AsSpan();
            var byteSpan = MemoryMarshal.Cast<char, byte>(charSpan);

            var hex = new string[10];
            for (var i = 0; i < byteSpan.Length; i++)
            {
                hex[i] = byteSpan[i].ToString("x2");
            }
        }

        [Fact]
        public void CanEnumerateOmniList()
        {
            var expected = new[] {1, 2, 3};

            var actual = new List<int>();
            var source = expected.ToList();

            var enumerator = source.Omni(0).GetEnumerator();
            while (enumerator.MoveNext())
            {
                actual.Add(enumerator.Current);
            }

            Assert.Equal(expected, actual);
        }


        [Theory]
        [InlineData(new[] {1}, new double[] {1})]
        [InlineData(new[] {2, 3}, new double[] {4, 9})]
        public void CanSelectSquares(int[] source, double[] expected)
        {
            var actual = new List<double>();
            var sourceList = source.ToList();
            using var e = sourceList.GetEnumerator();
            var sel = e.Select1(2, (int power, int x) => Math.Pow(x, power));
            while (sel.MoveNext())
            {
                actual.Add(sel.Current);
            }

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(new[] {1}, new double[] {1})]
        [InlineData(new[] {2, 3}, new double[] {4, 9})]
        public void CanSelectWithProvider(int[] source, double[] expected)
        {
            var actual = new List<double>();
            var sourceList = source.ToList();
            var sel = sourceList.SelectWithProvider(2, (int power, int x) => Math.Pow(x, power));
            foreach (var i in sel)
            {
                actual.Add(i);
            }

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(new[] {1}, new double[] {1})]
        [InlineData(new[] {2, 3}, new double[] {4, 9})]
        public void CanSelectWithSpan(int[] source, double[] expected)
        {
            var actual = new List<double>();
            var sourceSpan = source.AsSpan();

            // I could use a `var`, just for illustration here.
            SpanPipelineFixedPoint<int, string, long,
                SpanPipelineFixedPoint<int, double, string,
                    SpanPipelineFixedPoint<int, int, double,
                        SpanPipelineBuilder<int>>>> pipelineValue;

            pipelineValue = SpanLinq
                .StartWith<int>()
                .Select(x => Math.Pow(x, 2))
                .Select(x => x.ToString(CultureInfo.CurrentCulture))
                .Select(s => long.Parse(s));
            
            // Boxing. This step is not necessary, but can be useful for passing around large struct.
            ISpanPipeline<int, long> pipelineInterface = pipelineValue;
            
            var spanEnumerator = sourceSpan.GetEnumerator();
            while (pipelineValue.MoveNext(ref spanEnumerator))
            {
                actual.Add(pipelineValue.GetCurrent(ref spanEnumerator));
            }

            Assert.Equal(expected, actual);
        }
    }
}