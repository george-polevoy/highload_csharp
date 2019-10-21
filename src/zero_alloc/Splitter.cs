using System;
using System.Collections.Generic;

namespace zero_alloc
{
    public static class SplitterExtensions
    {
        public static IEnumerable<(int begin, int end)> GetSplitRanges_1(this ReadOnlyMemory<char> source)
        {
            if (source.Length == 0)
            {
                yield break;
            }
            int begin = 0, end = 0;
            while (begin <= source.Length)
            {
                while (end < source.Length && source.Span[end] != ' ')
                {
                    end ++;
                }
                yield return (begin, end);
                if (end == source.Length)
                {
                    break;
                }
                begin = end + 1;
                end = begin;
            }
        }

        public static IEnumerable<(int begin, int end)> GetSplitRanges(this ReadOnlyMemory<char> source)
        {
            var ie = new SplitEnumerator(source);
            while (ie.MoveNext())
            {
                yield return ie.Current;
            }
        }

        public struct SplitEnumerator
        {
            private ReadOnlyMemory<char> _source;
            public SplitEnumerator(ReadOnlyMemory<char> source)
            {
                _source = source;
                _begin = -1;
                _end = -1;
            }

            int _begin;
            int _end;

            public bool MoveNext()
            {
                if (_end >= _source.Length)
                {
                    return false;
                }

                _begin = _end + 1;
                if (_begin > _source.Length)
                {
                    return false;
                }
                _end = _begin;

                while (_end < _source.Length && _source.Span[_end] != ' ')
                {
                    _end++;
                }

                return true;
            }

            public (int Begin, int End) Current => (_begin, _end);
        }
    }
}
