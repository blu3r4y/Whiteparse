using System;

namespace Whiteparse.Grammar.Tokens
{
    public class LiteralToken : Token
    {
        public string Content { get; }

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