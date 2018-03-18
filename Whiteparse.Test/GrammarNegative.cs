using Xunit;
using Xunit.Abstractions;

namespace Whiteparse.Test
{
    public class GrammarNegative : AbstractTestSuite
    {
        public GrammarNegative(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void UnnamedToken()
        {
            FailGrammar("$");
        }

        [Fact]
        public void HiddenLiteralToken()
        {
            FailGrammar("$?\"not possible\"");
        }

        [Fact]
        public void InvalidTokenNames()
        {
            FailGrammar("$%percentage $in\"v");
        }

        [Fact]
        public void InvalidType()
        {
            FailGrammar("$[magic]inv");
        }

        [Fact]
        public void NoClosingBracketOnType()
        {
            FailGrammar("$[inttoken1 $[float]token2 $[string]token3");
        }

        [Fact]
        public void NoClosingQuoteOnLiteral()
        {
            FailGrammar("$(\\d+(\\w(\\d\\s+ )+\\d+(\\d(\\d)) (7)  $[int]some $garbage");
        }

        [Fact]
        public void NoClosingParenthesisOnRegEx()
        {
            FailGrammar("$\"  literal   $[int]some $garbage");
        }

        [Fact]
        public void InvalidVariableDeclaration()
        {
            FailGrammar("$$var1 $var2");
        }

        [Fact]
        public void CommentAsName()
        {
            FailGrammar("$#");
        }

        [Fact]
        public void CommentInType()
        {
            FailGrammar("$[int#]token");
        }

        [Fact]
        public void CommentInVariableDefinition()
        {
            FailGrammar("$token \n $$var1 #= $var2");
        }

        [Fact]
        public void EscapedCommentAsFirstCharacter()
        {
            FailGrammar("$\\#");
        }

        [Fact]
        public void InvalidEscapeInTokenType()
        {
            FailGrammar("$?\\[int]typed");
        }

        [Fact]
        public void InvalidEscapeInTokenIdentifier()
        {
            FailGrammar("$?[int]\\typed");
        }

        [Fact]
        public void InvalidEscapeInTokenHiddenFlag()
        {
            FailGrammar("$\\?hidden");
        }

        [Fact]
        public void InvalidEscapeInLiteralToken()
        {
            FailGrammar("$\\\"literal\"");
        }

        [Fact]
        public void InvalidEscapeInRegExToken()
        {
            FailGrammar("$\\(ab)");
        }

        [Fact]
        public void RegExWithLineBreak()
        {
            FailGrammar("$(a\r\nb)");
        }

        [Fact]
        public void MultiLineVariableStatementWithTrailingGarbage()
        {
            FailGrammar("$token $$variable = $a $b $c \\ garbage \n $d $e $f");
        }

        [Fact]
        public void MultiLineVariableStatementWithTrailingVariable()
        {
            FailGrammar("$token $$variable = $a $b $c \\ $trailing \n $d $e $f");
        }

        [Fact]
        public void ObjectTypedVariableWithTrailingDot()
        {
            FailGrammar("$token $$[TrailingDot.]variable = $a");
        }

        [Fact]
        public void ObjectTypedVariableWithInvalidDots()
        {
            FailGrammar("$token $$[Do..Dot]variable = $a");
        }

        [Fact]
        public void InvalidStructuredVariableIdentifier()
        {
            FailGrammar("$token $$token.field = $a 5");
        }

        [Fact]
        public void ListTokenEscapedAutomaticRangeMany()
        {
            FailGrammar("$token{\\*}");
        }

        [Fact]
        public void ListTokenEscapedAutomaticRangeManyLazy()
        {
            FailGrammar("$token{\\*?}");
        }

        [Fact]
        public void ListTokenEscapedAutomaticRangeAtLeastOnce()
        {
            FailGrammar("$token{\\+}");
        }

        [Fact]
        public void ListTokenEscapedInvalidRangeSpecifier()
        {
            FailGrammar("$token{-}");
        }
    }
}