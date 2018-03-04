using System;

namespace Whiteparse.Grammar.Tokens
{
    public class RegExToken : Token
    {
        public string Pattern { get; }

        public RegExToken(string pattern, bool hidden = false) : base(hidden)
        {
            Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
        }
        
        public override string ToString()
        {
            string hidden = Hidden ? ", Hidden" : "";
            return $"RegExToken<\"{Pattern}\"{hidden}>";
        }
    }
}