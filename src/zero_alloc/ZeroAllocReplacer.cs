using System;
using zero_alloc;

namespace Methods
{
    public class ZeroAllocReplacer
    {
        /// <summary>
        /// TransformDelegate writes transformed token and returns slice of destination starting from the end of written chunk.
        /// </summary>
        /// <param name="destination">Destination to write to.</param>
        /// <param name="token">Token to transform.</param>
        public delegate Span<char> TransformDelegate(Span<char> destination, ReadOnlySpan<char> token);

        private TransformDelegate _transform;
        
        public ZeroAllocReplacer(TransformDelegate transform)
        {
            _transform = transform;
        }

        public void Replace(ReadOnlySpan<char> source, Memory<char> buf)
        {
            var rest = buf.Span;
            var splitter = new SplitterExtensions.SplitEnumerator(source);
            while (splitter.MoveNext())
            {
                var range = splitter.Current;
                var token = source[range.Begin..range.End];
                rest = _transform(rest, token);
            }
        }
    }
}