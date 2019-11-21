using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using ZeroAlloc.Text;

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

        private static string MakeTestRepresentation(IEnumerable<string> source)
        {
            return string.Join(", ", source.Select(i => $"({i})"));
        }

        private static IReadOnlyList<string> Split(string source)
        {
            var l = new List<string>(5);
            foreach (var token in source.AsSpan().Split(' '))
            {
                l.Add(source[token]);
            }
            return l;
        }
    }
}
