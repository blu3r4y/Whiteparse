using System;
using System.Collections.Generic;
using System.Linq;
using Whiteparse.Grammar.Tokens.Ranges;

namespace Whiteparse.Grammar.Tokens
{
    /// <summary>
    /// Parses a list of tokens by repeating a token according to a <see cref="IRangeSpecifier"/>
    /// </summary>
    public class ListToken : Token
    {
        /// <summary>
        /// The <see cref="Token"/> to be repeated
        /// </summary>
        public Token InnerToken { get; }

        /// <summary>
        /// How to repeat the <see cref="InnerToken"/>
        /// </summary>
        public IRangeSpecifier Range { get; }

        /// <summary>
        /// Alternative delimiters (or null if not specified)
        /// </summary>
        public string[] Delimiters { get; }

        /// <summary>
        /// Create a <see cref="ListToken"/> by repeating an inner token according to a <see cref="IRangeSpecifier"/>
        /// </summary>
        /// <param name="innerToken">The token to be repeated</param>
        /// <param name="range">How to repeat the token</param>
        /// <param name="delimiters">Alternative delimiters (default is whitespace)</param>
        /// <exception cref="ArgumentNullException">If the innerToken or range is null</exception>
        public ListToken(Token innerToken, IRangeSpecifier range, IEnumerable<string> delimiters = null)
        {
            InnerToken = innerToken ?? throw new ArgumentNullException(nameof(innerToken));
            Range = range ?? throw new ArgumentNullException(nameof(range));
            Delimiters = delimiters?.ToArray();
        }

        /// <summary>
        /// Create a <see cref="ListToken"/> by repeating an inner token <see cref="repetitions"/> times
        /// </summary>
        /// <param name="innerToken">The token to be repeated</param>
        /// <param name="repetitions">Repeat the token exactly <see cref="repetitions"/> times</param>
        /// <param name="delimiters">Alternative delimiters (default is whitespace)</param>
        public ListToken(Token innerToken, int repetitions, IEnumerable<string> delimiters = null)
            : this(innerToken, new NumericRange(repetitions), delimiters)
        {
        }

        /// <summary>
        /// Create a <see cref="ListToken"/> by repeating an inner token automatically, using a <see cref="AutomaticRangeType"/> 
        /// </summary>
        /// <param name="innerToken">The token to be repeated</param>
        /// <param name="rangeType">The automatic repetition type</param>
        /// <param name="delimiters">Alternative delimiters (default is whitespace)</param>
        public ListToken(Token innerToken, AutomaticRangeType rangeType, IEnumerable<string> delimiters = null)
            : this(innerToken, new AutomaticRange(rangeType), delimiters)
        {
        }

        /// <summary>
        /// Create a <see cref="ListToken"/> by repeating an inner token <see cref="repetitions"/> times
        /// </summary>
        /// <param name="innerToken">The token to be repeated</param>
        /// <param name="referencedName">Repeat the inner token as often as the value of the parsed token named <see cref="referencedName"/> indicates</param>
        /// <param name="delimiters">Alternative delimiters (default is whitespace)</param>
        public ListToken(Token innerToken, string referencedName, IEnumerable<string> delimiters = null)
            : this(innerToken, new TokenRange(referencedName), delimiters)
        {
        }

        public override string ToString()
        {
            string delimiters = Delimiters != null ? $", Delimiters = ['{string.Join("', '", Delimiters)}']" : "";
            return $"ListToken<{InnerToken}, {Range}{delimiters}>";
        }
    }
}