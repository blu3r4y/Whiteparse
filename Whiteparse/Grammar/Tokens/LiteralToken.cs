using System;

namespace Whiteparse.Grammar.Tokens
{
    /// <summary>
    /// A token representing a literal string
    /// </summary>
    public class LiteralToken : Token
    {
        /// <summary>
        /// A literal string to be parsed
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Creates a <see cref="LiteralToken"/>
        /// </summary>
        /// <param name="content">The literal string to be parsed</param>
        /// <exception cref="ArgumentNullException">If the content is null</exception>
        public LiteralToken(string content)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public override string ToString()
        {
            return $"LiteralToken<\"{Content}\">";
        }
    }
}