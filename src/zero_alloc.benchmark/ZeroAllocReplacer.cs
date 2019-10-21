using System;
using TextExtensions;

namespace zero_alloc.benchmark
{
    public class ZeroAllocReplacer
    {
        /// <summary>
        /// TransformDelegate writes transformed token and returns slice of destination starting from the end of written chunk.
        /// </summary>
        /// <param name="destination">Destination to write to.</param>
        /// <param name="token">Token to transform.</param>
        public delegate Span<char> TransformDelegate(Span<char> destination, ReadOnlySpan<char> token);

        private readonly TransformDelegate _transform;
        
        public ZeroAllocReplacer(TransformDelegate transform)
        {
            _transform = transform;
        }

        public void Replace(Memory<char> destination, ReadOnlySpan<char> source)
        {
            var rest = destination.Span;
            var splitter = source.Split(' ');
            while (splitter.MoveNext())
            {
                var range = splitter.Current;
                var token = source[range.Begin..range.End];
                rest = _transform(rest, token);
            }
        }
    }
}