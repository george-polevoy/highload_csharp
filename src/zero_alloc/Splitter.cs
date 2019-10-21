using System;
using System.Collections.Generic;

namespace zero_alloc
{
    public static class SplitterExtensions
    {
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
