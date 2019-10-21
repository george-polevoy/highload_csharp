using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace TextExtensions
{
    public class SpanSplitExtensionsTests
    {
        [Theory]
        [InlineData("", "()")]
        [InlineData(" ", "(), ()")]
        [InlineData("  ", "(), (), ()")]
        [InlineData("a", "(a)")]
        [InlineData("ab", "(ab)")]
        [InlineData("a b", "(a), (b)")]
        [InlineData(" a", "(), (a)")]
        [InlineData("a ", "(a), ()")]
        [InlineData(" a ", "(), (a), ()")]
        public void SplitsToRanges(string source, string expected)
        {
            var actual = Split(source);

            var actualRepresentation = MakeTestRepresentation(actual);

            Assert.Equal(expected, actualRepresentation);
        }

        private static string MakeTestRepresentation(IReadOnlyList<string> source)
        {
            return string.Join(", ", source.Select(i => $"({i})"));
        }

        private static IReadOnlyList<string> Split(string source)
        {
            var l = new List<string>(100);
            var split = source.AsSpan().Split(' ');
            while (split.MoveNext())
            {
                var (begin, end) = split.Current;
                l.Add(source.Substring(begin, end - begin));
            }
            return l;
        }
    }
}
