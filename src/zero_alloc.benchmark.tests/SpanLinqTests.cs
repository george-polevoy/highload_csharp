using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using Xunit;
using ZeroAlloc.EnumeratorLinq;
using ZeroAlloc.Linq;
using ZeroAlloc.Linq.Boost;

namespace zero_alloc.benchmark.tests
{
    public class SpanLinqTests
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

        [Theory]
        [InlineData(new[] {1}, new double[] {1})]
        [InlineData(new[] {2, 3}, new double[] {4, 9})]
        public void CanSelectSquares(int[] source, double[] expected)
        {
            var actual = new List<double>();
            var sourceList = source.ToList();
            using var e = sourceList.GetEnumerator();
            var sel = e.RefSelect(2, (int power, int x) => Math.Pow(x, power));
            while (sel.MoveNext())
            {
                actual.Add(sel.Current);
            }

            Assert.Equal(expected, actual);
        }
    
        [Theory]
        [InlineData(new long[] {0}, new double[] {1})]
        [InlineData(new long[] {1, 2}, new double[] {9})]
        public void CanSelectWithSpan(long[] source, double[] expected)
        {
            var actual = new List<double>();
            var sourceSpan = source.AsSpan();

            var builder = SpanLinq
                .StartWith<long, (double power, CultureInfo cultureInfo, string comparand)>();

            SpanPipelineFixedPoint<long, string, long,
                SpanPipelineFixedPoint<long, string, string,
                    SpanPipelineFixedPoint<long, double, string,
                        SpanPipelineFixedPoint<long, long, double,
                            SpanPipelineFixedPoint<long, long, long,
                                SpanPipelineBuilder<long, (double power, CultureInfo cultureInfo, string comparand)>,
                                BoostSelectEngine<long, long, long,
                                    SpanPipelineBuilder<long, (double power, CultureInfo cultureInfo, string comparand)>
                                    ,
                                    LongPlusLong<long,
                                        Param<long, (double power, CultureInfo cultureInfo, string comparand)>,
                                        Const<long, long, (double power, CultureInfo cultureInfo, string comparand)>, (
                                        double power, CultureInfo cultureInfo, string comparand)>, (double power,
                                    CultureInfo cultureInfo, string comparand)>, (double power, CultureInfo cultureInfo,
                                string comparand)>,
                            DelegateSelectEngine<long, long, double, SpanPipelineFixedPoint<long, long, long,
                                SpanPipelineBuilder<long, (double power, CultureInfo cultureInfo, string comparand)>,
                                BoostSelectEngine<long, long, long,
                                    SpanPipelineBuilder<long, (double power, CultureInfo cultureInfo, string comparand)>
                                    ,
                                    LongPlusLong<long,
                                        Param<long, (double power, CultureInfo cultureInfo, string comparand)>,
                                        Const<long, long, (double power, CultureInfo cultureInfo, string comparand)>, (
                                        double power, CultureInfo cultureInfo, string comparand)>, (double power,
                                    CultureInfo cultureInfo, string comparand)>, (double power, CultureInfo cultureInfo,
                                string comparand)>, (double power, CultureInfo cultureInfo, string comparand)>, (double
                            power, CultureInfo cultureInfo, string comparand)>,
                        DelegateSelectEngine<long, double, string, SpanPipelineFixedPoint<long, long, double,
                                SpanPipelineFixedPoint<long, long, long,
                                    SpanPipelineBuilder<long, (double power, CultureInfo cultureInfo, string comparand)>
                                    ,
                                    BoostSelectEngine<long, long, long,
                                        SpanPipelineBuilder<long, (double power, CultureInfo cultureInfo, string
                                            comparand)>
                                        ,
                                        LongPlusLong<long,
                                            Param<long, (double power, CultureInfo cultureInfo, string comparand)>,
                                            Const<long, long, (double power, CultureInfo cultureInfo, string comparand)>
                                            , (
                                            double power, CultureInfo cultureInfo, string comparand)>, (double power,
                                        CultureInfo cultureInfo, string comparand)>, (double power, CultureInfo
                                    cultureInfo,
                                    string comparand)>,
                                DelegateSelectEngine<long, long, double, SpanPipelineFixedPoint<long, long, long,
                                    SpanPipelineBuilder<long, (double power, CultureInfo cultureInfo, string comparand)>
                                    ,
                                    BoostSelectEngine<long, long, long,
                                        SpanPipelineBuilder<long, (double power, CultureInfo cultureInfo, string
                                            comparand)>
                                        ,
                                        LongPlusLong<long,
                                            Param<long, (double power, CultureInfo cultureInfo, string comparand)>,
                                            Const<long, long, (double power, CultureInfo cultureInfo, string comparand)>
                                            , (
                                            double power, CultureInfo cultureInfo, string comparand)>, (double power,
                                        CultureInfo cultureInfo, string comparand)>, (double power, CultureInfo
                                    cultureInfo,
                                    string comparand)>, (double power, CultureInfo cultureInfo, string comparand)>, (
                                double
                                power, CultureInfo cultureInfo, string comparand)>, (double power, CultureInfo
                            cultureInfo,
                            string comparand)>, (double power, CultureInfo cultureInfo, string comparand)>,
                    DelegateWhereEngine<long, string, SpanPipelineFixedPoint<long, double, string,
                            SpanPipelineFixedPoint<long, long, double,
                                SpanPipelineFixedPoint<long, long, long,
                                    SpanPipelineBuilder<long, (double power, CultureInfo cultureInfo, string comparand)>
                                    ,
                                    BoostSelectEngine<long, long, long,
                                        SpanPipelineBuilder<long, (double power, CultureInfo cultureInfo, string
                                            comparand)>
                                        ,
                                        LongPlusLong<long,
                                            Param<long, (double power, CultureInfo cultureInfo, string comparand)>,
                                            Const<long, long, (double power, CultureInfo cultureInfo, string comparand)>
                                            , (
                                            double power, CultureInfo cultureInfo, string comparand)>, (double power,
                                        CultureInfo cultureInfo, string comparand)>, (double power, CultureInfo
                                    cultureInfo,
                                    string comparand)>,
                                DelegateSelectEngine<long, long, double, SpanPipelineFixedPoint<long, long, long,
                                    SpanPipelineBuilder<long, (double power, CultureInfo cultureInfo, string comparand)>
                                    ,
                                    BoostSelectEngine<long, long, long,
                                        SpanPipelineBuilder<long, (double power, CultureInfo cultureInfo, string
                                            comparand)>
                                        ,
                                        LongPlusLong<long,
                                            Param<long, (double power, CultureInfo cultureInfo, string comparand)>,
                                            Const<long, long, (double power, CultureInfo cultureInfo, string comparand)>
                                            , (
                                            double power, CultureInfo cultureInfo, string comparand)>, (double power,
                                        CultureInfo cultureInfo, string comparand)>, (double power, CultureInfo
                                    cultureInfo,
                                    string comparand)>, (double power, CultureInfo cultureInfo, string comparand)>, (
                                double
                                power, CultureInfo cultureInfo, string comparand)>,
                            DelegateSelectEngine<long, double, string, SpanPipelineFixedPoint<long, long, double,
                                    SpanPipelineFixedPoint<long, long, long,
                                        SpanPipelineBuilder<long, (double power, CultureInfo cultureInfo, string
                                            comparand)>,
                                        BoostSelectEngine<long, long, long,
                                            SpanPipelineBuilder<long, (double power, CultureInfo cultureInfo, string
                                                comparand)>
                                            ,
                                            LongPlusLong<long,
                                                Param<long, (double power, CultureInfo cultureInfo, string comparand)>,
                                                Const<long, long, (double power, CultureInfo cultureInfo, string
                                                    comparand)>, (
                                                double power, CultureInfo cultureInfo, string comparand)>, (double power
                                            ,
                                            CultureInfo cultureInfo, string comparand)>, (double power, CultureInfo
                                        cultureInfo,
                                        string comparand)>,
                                    DelegateSelectEngine<long, long, double, SpanPipelineFixedPoint<long, long, long,
                                        SpanPipelineBuilder<long, (double power, CultureInfo cultureInfo, string
                                            comparand)>,
                                        BoostSelectEngine<long, long, long,
                                            SpanPipelineBuilder<long, (double power, CultureInfo cultureInfo, string
                                                comparand)>
                                            ,
                                            LongPlusLong<long,
                                                Param<long, (double power, CultureInfo cultureInfo, string comparand)>,
                                                Const<long, long, (double power, CultureInfo cultureInfo, string
                                                    comparand)>, (
                                                double power, CultureInfo cultureInfo, string comparand)>, (double power
                                            ,
                                            CultureInfo cultureInfo, string comparand)>, (double power, CultureInfo
                                        cultureInfo,
                                        string comparand)>, (double power, CultureInfo cultureInfo, string comparand)>,
                                    (double
                                    power, CultureInfo cultureInfo, string comparand)>, (double power, CultureInfo
                                cultureInfo,
                                string comparand)>, (double power, CultureInfo cultureInfo, string comparand)>, (double
                        power,
                        CultureInfo cultureInfo, string comparand)>, (double power, CultureInfo cultureInfo, string
                    comparand)>, DelegateSelectEngine<long, string, long, SpanPipelineFixedPoint<long, string, string,
                    SpanPipelineFixedPoint<long, double, string,
                        SpanPipelineFixedPoint<long, long, double,
                            SpanPipelineFixedPoint<long, long, long,
                                SpanPipelineBuilder<long, (double power, CultureInfo cultureInfo, string comparand)>,
                                BoostSelectEngine<long, long, long,
                                    SpanPipelineBuilder<long, (double power, CultureInfo cultureInfo, string comparand)>
                                    ,
                                    LongPlusLong<long,
                                        Param<long, (double power, CultureInfo cultureInfo, string comparand)>,
                                        Const<long, long, (double power, CultureInfo cultureInfo, string comparand)>, (
                                        double power, CultureInfo cultureInfo, string comparand)>, (double power,
                                    CultureInfo cultureInfo, string comparand)>, (double power, CultureInfo cultureInfo,
                                string comparand)>,
                            DelegateSelectEngine<long, long, double, SpanPipelineFixedPoint<long, long, long,
                                SpanPipelineBuilder<long, (double power, CultureInfo cultureInfo, string comparand)>,
                                BoostSelectEngine<long, long, long,
                                    SpanPipelineBuilder<long, (double power, CultureInfo cultureInfo, string comparand)>
                                    ,
                                    LongPlusLong<long,
                                        Param<long, (double power, CultureInfo cultureInfo, string comparand)>,
                                        Const<long, long, (double power, CultureInfo cultureInfo, string comparand)>, (
                                        double power, CultureInfo cultureInfo, string comparand)>, (double power,
                                    CultureInfo cultureInfo, string comparand)>, (double power, CultureInfo cultureInfo,
                                string comparand)>, (double power, CultureInfo cultureInfo, string comparand)>, (double
                            power, CultureInfo cultureInfo, string comparand)>,
                        DelegateSelectEngine<long, double, string, SpanPipelineFixedPoint<long, long, double,
                                SpanPipelineFixedPoint<long, long, long,
                                    SpanPipelineBuilder<long, (double power, CultureInfo cultureInfo, string comparand)>
                                    ,
                                    BoostSelectEngine<long, long, long,
                                        SpanPipelineBuilder<long, (double power, CultureInfo cultureInfo, string
                                            comparand)>
                                        ,
                                        LongPlusLong<long,
                                            Param<long, (double power, CultureInfo cultureInfo, string comparand)>,
                                            Const<long, long, (double power, CultureInfo cultureInfo, string comparand)>
                                            , (
                                            double power, CultureInfo cultureInfo, string comparand)>, (double power,
                                        CultureInfo cultureInfo, string comparand)>, (double power, CultureInfo
                                    cultureInfo,
                                    string comparand)>,
                                DelegateSelectEngine<long, long, double, SpanPipelineFixedPoint<long, long, long,
                                    SpanPipelineBuilder<long, (double power, CultureInfo cultureInfo, string comparand)>
                                    ,
                                    BoostSelectEngine<long, long, long,
                                        SpanPipelineBuilder<long, (double power, CultureInfo cultureInfo, string
                                            comparand)>
                                        ,
                                        LongPlusLong<long,
                                            Param<long, (double power, CultureInfo cultureInfo, string comparand)>,
                                            Const<long, long, (double power, CultureInfo cultureInfo, string comparand)>
                                            , (
                                            double power, CultureInfo cultureInfo, string comparand)>, (double power,
                                        CultureInfo cultureInfo, string comparand)>, (double power, CultureInfo
                                    cultureInfo,
                                    string comparand)>, (double power, CultureInfo cultureInfo, string comparand)>, (
                                double
                                power, CultureInfo cultureInfo, string comparand)>, (double power, CultureInfo
                            cultureInfo,
                            string comparand)>, (double power, CultureInfo cultureInfo, string comparand)>,
                    DelegateWhereEngine<long, string, SpanPipelineFixedPoint<long, double, string,
                            SpanPipelineFixedPoint<long, long, double,
                                SpanPipelineFixedPoint<long, long, long,
                                    SpanPipelineBuilder<long, (double power, CultureInfo cultureInfo, string comparand)>
                                    ,
                                    BoostSelectEngine<long, long, long,
                                        SpanPipelineBuilder<long, (double power, CultureInfo cultureInfo, string
                                            comparand)>
                                        ,
                                        LongPlusLong<long,
                                            Param<long, (double power, CultureInfo cultureInfo, string comparand)>,
                                            Const<long, long, (double power, CultureInfo cultureInfo, string comparand)>
                                            , (
                                            double power, CultureInfo cultureInfo, string comparand)>, (double power,
                                        CultureInfo cultureInfo, string comparand)>, (double power, CultureInfo
                                    cultureInfo,
                                    string comparand)>,
                                DelegateSelectEngine<long, long, double, SpanPipelineFixedPoint<long, long, long,
                                    SpanPipelineBuilder<long, (double power, CultureInfo cultureInfo, string comparand)>
                                    ,
                                    BoostSelectEngine<long, long, long,
                                        SpanPipelineBuilder<long, (double power, CultureInfo cultureInfo, string
                                            comparand)>
                                        ,
                                        LongPlusLong<long,
                                            Param<long, (double power, CultureInfo cultureInfo, string comparand)>,
                                            Const<long, long, (double power, CultureInfo cultureInfo, string comparand)>
                                            , (
                                            double power, CultureInfo cultureInfo, string comparand)>, (double power,
                                        CultureInfo cultureInfo, string comparand)>, (double power, CultureInfo
                                    cultureInfo,
                                    string comparand)>, (double power, CultureInfo cultureInfo, string comparand)>, (
                                double
                                power, CultureInfo cultureInfo, string comparand)>,
                            DelegateSelectEngine<long, double, string, SpanPipelineFixedPoint<long, long, double,
                                    SpanPipelineFixedPoint<long, long, long,
                                        SpanPipelineBuilder<long, (double power, CultureInfo cultureInfo, string
                                            comparand)>,
                                        BoostSelectEngine<long, long, long,
                                            SpanPipelineBuilder<long, (double power, CultureInfo cultureInfo, string
                                                comparand)>
                                            , LongPlusLong<long,
                                                Param<long, (double power, CultureInfo cultureInfo, string comparand)>,
                                                Const<long, long, (double power, CultureInfo cultureInfo, string
                                                    comparand)>, (
                                                double power, CultureInfo cultureInfo, string comparand)>, (double power
                                            ,
                                            CultureInfo cultureInfo, string comparand)>, (double power, CultureInfo
                                        cultureInfo,
                                        string comparand)>,
                                    DelegateSelectEngine<long, long, double, SpanPipelineFixedPoint<long, long, long,
                                        SpanPipelineBuilder<long, (double power, CultureInfo cultureInfo, string
                                            comparand)>,
                                        BoostSelectEngine<long, long, long,
                                            SpanPipelineBuilder<long, (double power, CultureInfo cultureInfo, string
                                                comparand)>
                                            , LongPlusLong<long,
                                                Param<long, (double power, CultureInfo cultureInfo, string comparand)>,
                                                Const<long, long, (double power, CultureInfo cultureInfo, string
                                                    comparand)>, (
                                                double power, CultureInfo cultureInfo, string comparand)>, (double power
                                            ,
                                            CultureInfo cultureInfo, string comparand)>, (double power, CultureInfo
                                        cultureInfo,
                                        string comparand)>, (double power, CultureInfo cultureInfo, string comparand)>,
                                    (double
                                    power, CultureInfo cultureInfo, string comparand)>, (double power, CultureInfo
                                cultureInfo,
                                string comparand)>, (double power, CultureInfo cultureInfo, string comparand)>, (double
                        power,
                        CultureInfo cultureInfo, string comparand)>, (double power, CultureInfo cultureInfo, string
                    comparand)>, (double power, CultureInfo cultureInfo, string comparand)>, (double power, CultureInfo
                cultureInfo, string comparand)> pipeline;
            
            pipeline = builder
                .Select(builder.Operations().Plus(
                    builder.Operations().Param<long>(),
                    builder.Operations().Const<long, long>(1)))
                .Select((state, x) => Math.Pow(x, state.power))
                .Select((state, x) => x.ToString(state.cultureInfo))
                .Where((state, x) => x != state.comparand)
                .Select((state, s) => long.Parse(s));

            foreach (var x in sourceSpan.Apply(pipeline, (2, CultureInfo.InvariantCulture, "4")))
            {
                actual.Add(x);
            }

            Assert.Equal(expected, actual);
        }
    }
}