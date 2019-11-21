using System;

namespace ZeroAlloc.Text
{
    public static class SpanSplitExtensions
    {
        public static SentinelSplitEnumerable<T> Split<T>(this ReadOnlySpan<T> source, T separator)
            where T: IEquatable<T>
        {
            return new SentinelSplitEnumerable<T>(source, separator);
        }

        public static SubSequenceSplitEnumerator<T> Split<T>(this ReadOnlySpan<T> source, ReadOnlySpan<T> separatorText)
            where T : IEquatable<T>
        {
            return new SubSequenceSplitEnumerator<T>(source, separatorText);
        }
    }
}