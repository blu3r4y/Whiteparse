using System;
using System.Text.RegularExpressions;

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
        /// The compiled regex (if compilation was enabled)
        /// </summary>
        internal Regex CompiledRegex { get; }

        /// <summary>
        /// Whether the regex pattern has been compiled or not 
        /// </summary>
        internal bool IsCompiled => CompiledRegex != null;

        /// <summary>
        /// Creates a <see cref="RegExToken"/>
        /// </summary>
        /// <param name="pattern">The regex pattern</param>
        /// <param name="compiled">Whether to compile the regex or not</param>
        /// <param name="hidden">Hidden tokens are hidden in the final object</param>
        /// <exception cref="ArgumentNullException">If the pattern is null</exception>
        public RegExToken(string pattern, bool compiled = true, bool hidden = false) : base(hidden)
        {
            Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
            if (compiled)
            {
                CompiledRegex = new Regex(pattern, RegexOptions.Compiled);
            }
        }

        public override string ToString()
        {
            string hidden = Hidden ? ", Hidden" : "";
            string compiled = IsCompiled ? ", Compiled" : "";
            return $"RegExToken<\"{Pattern}\"{hidden}{compiled}>";
        }
    }
}