namespace Whiteparse.Grammar.Tokens.Ranges
{
    /// <summary>
    /// Specifies that a token will be repeated exactly <see cref="Repetitions"/> times.
    /// </summary>
    public class NumericRange : IRangeSpecifier
    {
        /// <summary>
        /// The number of times to repeat the token
        /// </summary>
        public int Repetitions { get; }

        public NumericRange(int repetitions)
        {
            Repetitions = repetitions;
        }

        public override string ToString()
        {
            return $"NumericRange<{Repetitions}>";
        }
    }
}