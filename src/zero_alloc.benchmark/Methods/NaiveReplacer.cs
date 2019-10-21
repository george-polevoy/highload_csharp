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

        public NaiveReplacer2 BuildReplacer2()
        {
            return new NaiveReplacer2(_replacements);
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
                    .Select(token => _replacements.TryGetValue(new string(token.ToCharArray()), out var replaced) ? replaced : token)
                );
        }
    }

    public class NaiveReplacer2 : IReplace
    {
        private Dictionary<string, string> _replacements;

        public NaiveReplacer2(Dictionary<string, string> replacements)
        {
            _replacements = replacements;
        }
        public string Replace(string source)
        {
            foreach (var kvp in _replacements)
            {
                var key = new string(kvp.Key.ToCharArray());
                source = source.Replace(key, kvp.Value);
            }
            return source;
        }
    }
}
