
using System;

namespace zero_alloc.benchmark
{
    public static class Replacers
    {
        public static NaiveReplacer CreateNaiveReplacer()
        {
            return new NaiveReplacer(token =>
                token.StartsWith("a") ? token.Length + token : token);
        }

        public static ZeroAllocReplacer CreateZeroAllocReplacer()
        {
            return new ZeroAllocReplacer((Span<char> dst, ReadOnlySpan<char> src, out int replacementLength) =>
            {
                var len = src.Length;
                var writtenLength = 0;
                if (src.StartsWith("a"))
                {
                    if (!len.TryFormat(dst, out var charsWritten))
                        throw new ArgumentException("Destination is shorter than needed");
                    dst = dst.Slice(charsWritten);
                    writtenLength += charsWritten;
                }

                src.CopyTo(dst);
                replacementLength = writtenLength + src.Length;
            });
        }
    }
}