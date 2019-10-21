using System.Collections.Generic;
using System.Linq;

namespace Methods
{
    public class NaiveReplacerBuilder
    {
        private Dictionary<string, string> _replacements;

        public NaiveReplacerBuilder()
        {
        }

        public void Add(string token, string replacement)
        {
            (_replacements ?? (_replacements = new Dictionary<string, string>(10)))
            .Add(token, replacement);
        }

        public NaiveReplacer BuildReplacer()
        {
            return new NaiveReplacer(_replacements);
        }
    }

    public class NaiveReplacer : IReplace
    {
        private Dictionary<string, string> _replacements;
        internal NaiveReplacer(Dictionary<string, string> replacements)
        {
            _replacements = replacements;
        }

        public string Replace(string source)
        {
            if (_replacements == null)
            {
                return source;
            }
            return string
                .Join(' ', source
                    .Split(' ')
                    .Select(token => _replacements.TryGetValue(token, out var replaced) ? replaced : token)
                );
        }
    }
}
