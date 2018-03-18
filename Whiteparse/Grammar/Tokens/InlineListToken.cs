using System;
using System.Collections.Generic;
using System.Linq;

namespace Whiteparse.Grammar.Tokens
{
    /// <summary>
    /// Parses the specified tokens into a list (and not an object).
    /// </summary>
    public class InlineListToken : Token
    {
        /// <summary>
        /// Elements of the parsed list
        /// </summary>
        public IEnumerable<Token> InnerTokens { get; }

        /// <summary>
        /// Create a <see cref="InlineListToken"/>
        /// </summary>
        /// <param name="innerTokens">Elements to be parsed into a list</param>
        /// <exception cref="ArgumentNullException">If innerTokens is null</exception>
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