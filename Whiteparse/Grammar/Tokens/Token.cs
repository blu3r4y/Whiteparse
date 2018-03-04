namespace Whiteparse.Grammar.Tokens
{
    public abstract class Token
    {
        public bool Hidden { get; }

        protected Token(bool hidden = false)
        {
            Hidden = hidden;
        }
    }
}