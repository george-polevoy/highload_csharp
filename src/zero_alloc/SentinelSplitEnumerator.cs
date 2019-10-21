using System;

namespace TextExtensions
{
    public ref struct SentinelSplitEnumerator<T> where T: IEquatable<T>
    {
        private readonly ReadOnlySpan<T> _source;
        private readonly T _separator;
        private int _begin;
        private int _end;
        public SentinelSplitEnumerator(ReadOnlySpan<T> source, T separator)
        {
            _source = source;
            _separator = separator;
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

            var found = _source.Slice(_end).IndexOf(_separator);
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