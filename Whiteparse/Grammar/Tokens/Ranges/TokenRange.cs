namespace Whiteparse.Grammar.Tokens.Ranges
{
    /// <summary>
    /// Specifies that a token will be repeated as often as the value of the referenced <see cref="NamedToken"/> indicates.
    /// </summary>
    public class TokenRange : IRangeSpecifier
    {
        /// <summary>
        /// The parsed value of the token with this name indicates how often something will be repeated
        /// </summary>
        public string ReferencedName { get; }

        public TokenRange(string referencedName)
        {
            ReferencedName = referencedName;
        }

        public override string ToString()
        {
            return $"TokenRange<{ReferencedName}>";
        }
    }
}