using System;
using System.Collections.Generic;
using System.Linq;

namespace Whiteparse.Grammar.Tokens
{
    public class InlineListToken : Token
    {
        public IEnumerable<Token> InnerTokens { get; }

        public InlineListToken(IEnumerable<Token> innerTokens)
        {
            InnerTokens = innerTokens ?? throw new ArgumentNullException(nameof(innerTokens));
        }

        public override string ToString()
        {
            return $"InlineListToken<>" + InnerTokens.Select(t => t.ToString()).ToIndentedBlock();
        }
    }
}