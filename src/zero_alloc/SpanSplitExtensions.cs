using System;

namespace TextExtensions
{
    public static class SpanSplitExtensions
    {
        public static SentinelSplitEnumerator<T> Split<T>(this ReadOnlySpan<T> source, T separator)
            where T: IEquatable<T>
        {
            return new SentinelSplitEnumerator<T>(source, separator);
        }

        public static SubSequenceSplitEnumerator<T> Split<T>(this ReadOnlySpan<T> source, ReadOnlySpan<T> separatorText)
            where T : IEquatable<T>
        {
            return new SubSequenceSplitEnumerator<T>(source, separatorText);
        }
        
        public static AnySentinelSplitEnumerator<T> SplitOnAny<T>(this ReadOnlySpan<T> source, ReadOnlySpan<T> separators)
            where T : IEquatable<T>
        {
            return new AnySentinelSplitEnumerator<T>(source, separators);
        }
    }
}