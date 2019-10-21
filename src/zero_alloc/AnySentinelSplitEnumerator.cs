using System;

namespace TextExtensions
{
    public ref struct AnySentinelSplitEnumerator<T> where T: IEquatable<T>
    {
        private readonly ReadOnlySpan<T> _source;
        private readonly ReadOnlySpan<T> _separators;
        private int _begin;
        private int _end;
        public AnySentinelSplitEnumerator(ReadOnlySpan<T> source, ReadOnlySpan<T> separators)
        {
            _source = source;
            _separators = separators;
            _begin = -1;
            _end = -1;
        }
            
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

            var found = _source.Slice(_end).IndexOfAny(_separators);
            if (found != -1)
            {
                _end += found;
            }
            else
            {
                _end = _source.Length;
            }

            return true;
        }

        public (int Begin, int End) Current => (_begin, _end);
    }
}
