namespace Whiteparse.Grammar.Tokens.Ranges
{
    public enum AutomaticRangeType
    {
        /// <summary>One or more repetitions, as many as possible</summary>
        AtLeastOnce,

        /// <summary>Zero or more repetitions, as many as possible</summary>
        Many,

        /// <summary>Zero or more repetitions, as few as possible, expanding as needed</summary>
        ManyLazy,
    }

    /// <summary>
    /// Automatic range specifiers do not specify the exact number of repetitions,
    /// but rather expand as needed (zero or more, one or more, etc.)
    /// </summary>
    public class AutomaticRange : IRangeSpecifier
    {
        /// <summary>
        /// Type of automatic range resolution
        /// </summary>
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