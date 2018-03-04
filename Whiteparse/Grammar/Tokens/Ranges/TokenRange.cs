namespace Whiteparse.Grammar.Tokens.Ranges
{
    public class TokenRange : IRangeSpecifier
    {
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