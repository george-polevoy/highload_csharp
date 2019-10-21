using System;
using System.Collections.Generic;

namespace zero_alloc
{
    public static class SplitterExtensions
    {
        public ref struct SplitEnumerator
        {
            private ReadOnlySpan<char> _source;
            public SplitEnumerator(ReadOnlySpan<char> source)
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

                var found = _source.Slice(_end).IndexOf(' ');
                if (found != -1)
                {
                    _end += found;
                }
                else
                {
                    _end = _source.Length;
                }
//                while (_end < _source.Length && _source[_end] != ' ')
//                {
//                    _end++;
//                }

                return true;
            }

            public (int Begin, int End) Current => (_begin, _end);
        }
    }
}
