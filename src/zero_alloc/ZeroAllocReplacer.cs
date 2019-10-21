using System;
using System.Collections.Generic;
using System.Linq;
using zero_alloc;

namespace Methods
{
    public class ZeroAllocReplacer
    {
        private Dictionary<ReadOnlyMemory<char>, ReadOnlyMemory<char>> _replacements;

        public ZeroAllocReplacer(IDictionary<string, string> replacements)
        {
            _replacements = new Dictionary<ReadOnlyMemory<char>, ReadOnlyMemory<char>>();
            foreach (var replacement in replacements)
            {
                _replacements.Add(replacement.Key.AsMemory(), replacement.Value.AsMemory());
            }
        }

        public void Replace(ReadOnlyMemory<char> source, Memory<char> buf)
        {
            int begin = 0;
            var splitter = new SplitterExtensions.SplitEnumerator(source.Span);
            while (splitter.MoveNext())
            {
                var range = splitter.Current;
                var token = source[range.Begin..range.End];
                var chunk = _replacements
                    .TryGetValue(token, out var replaced)
                    ? replaced
                    : token;
                    
                chunk.CopyTo(buf[begin..]);
                begin += chunk.Length;
            }
        }
    }
}