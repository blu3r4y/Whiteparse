namespace Whiteparse.Grammar.Tokens
{
    /// <summary>
    /// A token is used to match elements of the input
    /// </summary>
    public abstract class Token
    {
        /// <summary>
        /// Hidden tokens are parsed, but not shown in the final object
        /// </summary>
        public bool Hidden { get; }

        protected Token(bool hidden = false)
        {
            Hidden = hidden;
        }
    }
}