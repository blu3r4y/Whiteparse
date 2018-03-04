namespace Whiteparse.Grammar.Tokens.Ranges
{
    public class NumericRange : IRangeSpecifier
    {
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