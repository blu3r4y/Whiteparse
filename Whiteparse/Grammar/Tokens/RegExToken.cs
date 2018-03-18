using System;

namespace Whiteparse.Grammar.Tokens
{
    /// <summary>
    /// Parses a regex pattern
    /// </summary>
    public class RegExToken : Token
    {
        /// <summary>
        /// The regex pattern to be parsed
        /// </summary>
        public string Pattern { get; }

        /// <summary>
        /// Creates a <see cref="RegExToken"/>
        /// </summary>
        /// <param name="pattern">The regex pattern</param>
        /// <param name="hidden">Hidden tokens are hidden in the final object</param>
        /// <exception cref="ArgumentNullException">If the pattern is null</exception>
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