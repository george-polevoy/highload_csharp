using System;

namespace ZeroAlloc.Text
{
    public class ZeroAllocReplacer
    {
        /// <summary>
        /// TransformDelegate writes transformed token and returns slice of destination starting from the end of written chunk.
        /// </summary>
        /// <param name="destination">Destination to write to.</param>
        /// <param name="token">Token to transform.</param>
        public delegate void TransformDelegate(Span<char> destination, ReadOnlySpan<char> token, out int replacementLength);

        private readonly TransformDelegate _transform;
        
        public ZeroAllocReplacer(TransformDelegate transform)
        {
            _transform = transform;
        }

        public void Replace(Span<char> destination, ReadOnlySpan<char> source, out int replacementLength)
        {
            var length = 0;
            bool subsequent = false;
            foreach(var tokenRange in source.Split(' '))
            {
                if (subsequent)
                    destination[length++] = ' ';
                else
                    subsequent = true;

                _transform(destination[length..], source[tokenRange], out var step);
                length += step;
            }
            replacementLength = length;
        }
    }
}