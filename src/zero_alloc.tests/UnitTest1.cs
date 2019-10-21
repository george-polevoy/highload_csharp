using System;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Engine.ClientProtocol;

namespace zero_alloc.tests
{
    public class UnitTest1
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
        public void Test1(string source, string expected)
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
            return source.AsMemory().GetSplitRanges().Select(i => source.Substring(i.begin, i.end - i.begin)).ToList();
        }
    }
}