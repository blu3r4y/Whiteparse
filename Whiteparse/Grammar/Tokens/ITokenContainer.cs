using System.Collections.Generic;

namespace Whiteparse.Grammar.Tokens
{
    /// <summary>
    /// A token container wraps one or more inner tokens
    /// </summary>
    public interface ITokenContainer
    {
        /// <summary>
        /// Inner tokens
        /// </summary>
        IEnumerable<Token> InnerTokens { get; }
    }
}