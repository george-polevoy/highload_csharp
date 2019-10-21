using System;
using System.Linq;

namespace zero_alloc.benchmark
{
    public class NaiveReplacer
    {
        private readonly Func<string, string> _transform;

        internal NaiveReplacer(Func<string, string> transform)
        {
            _transform = transform ?? throw new ArgumentNullException(nameof(transform));
        }

        public string Replace(string source)
        {
            return string
                .Join(' ', source
                    .Split(' ')
                    .Select(token => _transform(token))
                );
        }
    }
}