namespace Whiteparse.Grammar.Tokens.Ranges
{
    public enum AutomaticRangeType
    {
        AtLeastOnce,
        Many,
        ManyLazy,
    }

    public class AutomaticRange : IRangeSpecifier
    {
        public AutomaticRangeType RangeType { get; }

        public AutomaticRange(AutomaticRangeType rangeType)
        {
            RangeType = rangeType;
        }

        public override string ToString()
        {
            return $"AutomaticRange<{RangeType}>";
        }

        public static AutomaticRange FromType(AutomaticRangeType type)
        {
            return new AutomaticRange(type);
        }
    }
}