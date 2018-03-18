namespace Whiteparse.Grammar.Tokens.Ranges
{
    /// <summary>
    /// Specifies that a token will be repeated as often as the value of the referenced <see cref="NamedToken"/> indicates.
    /// </summary>
    public class TokenRange : IRangeSpecifier
    {
        /// <summary>
        /// The parsed value of this <see cref="NamedToken"/> indicates how often the token will be repeated
        /// </summary>
        public NamedToken Reference { get; }

        public TokenRange(NamedToken reference)
        {
            Reference = reference;
        }

        public override string ToString()
        {
            return $"TokenRange<{Reference}>";
        }
    }
}