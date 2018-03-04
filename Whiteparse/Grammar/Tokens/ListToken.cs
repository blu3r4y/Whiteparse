using System;
using System.Collections.Generic;
using System.Linq;
using Whiteparse.Grammar.Tokens.Ranges;

namespace Whiteparse.Grammar.Tokens
{
    public class ListToken : Token
    {
        public Token InnerToken { get; }
        public IRangeSpecifier Range { get; }
        public string[] Delimiters { get; }

        public ListToken(Token innerToken, IRangeSpecifier range, IEnumerable<string> delimiters = null)
        {
            InnerToken = innerToken ?? throw new ArgumentNullException(nameof(innerToken));
            Range = range ?? throw new ArgumentNullException(nameof(range));
            Delimiters = delimiters?.ToArray();
        }

        public ListToken(Token innerToken, int repetitions, IEnumerable<string> delimiters = null)
            : this(innerToken, new NumericRange(repetitions), delimiters)
        {
        }

        public ListToken(Token innerToken, AutomaticRangeType rangeType, IEnumerable<string> delimiters = null)
            : this(innerToken, new AutomaticRange(rangeType), delimiters)
        {
        }

        public ListToken(Token innerToken, NamedToken reference, IEnumerable<string> delimiters = null)
            : this(innerToken, new TokenRange(reference), delimiters)
        {
        }

        public override string ToString()
        {
            return $"ListToken<{InnerToken}, {Range}>";
        }
    }
}