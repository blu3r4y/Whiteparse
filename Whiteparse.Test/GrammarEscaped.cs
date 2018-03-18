using System.Collections.Generic;
using Whiteparse.Grammar;
using Whiteparse.Grammar.Tokens;
using Whiteparse.Grammar.Tokens.Ranges;
using Xunit;
using Xunit.Abstractions;

namespace Whiteparse.Test
{
    public class GrammarEscaped : AbstractTestSuite
    {
        public GrammarEscaped(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void EscapedTokenPrefix()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new LiteralToken("$")
                },
                new List<Variable>());

            CompareGrammar(@"\$", expected);
        }

        [Fact]
        public void EscapedTokenPrefixDouble()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new LiteralToken("$$")
                },
                new List<Variable>());

            CompareGrammar(@"\$\$", expected);
        }

        [Fact]
        public void EscapedTokenPrefixNewLine()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new LiteralToken("$.")
                },
                new List<Variable>());

            CompareGrammar(@"\$.", expected);
        }

        [Fact]
        public void EscapedComment()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new LiteralToken("#")
                });

            CompareGrammar(@"\#", expected);
        }

        [Fact]
        public void EscapedCommentText()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("token"),
                    new LiteralToken("#"),
                    new LiteralToken("some"),
                    new LiteralToken("text")
                });

            CompareGrammar(@"$token \# some text", expected);
        }

        [Fact]
        public void EscapedToken()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new LiteralToken("$name")
                },
                new List<Variable>());

            CompareGrammar(@"\$name", expected);
        }

        [Fact]
        public void EscapedParenthesisInsideRegEx()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new RegExToken(@"\d+\(1\)")
                },
                new List<Variable>());

            CompareGrammar(@"$(\d+\(1\))", expected);
        }

        [Fact]
        public void EscapedWrongParenthesisInsideRegEx()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new RegExToken(@"\d+\(1")
                },
                new List<Variable>());

            CompareGrammar(@"$(\d+\(1)", expected);
        }

        [Fact]
        public void UnescapedEscapeCharacterInRegEx()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new RegExToken(@"\d+\(1\x")
                },
                new List<Variable>());

            CompareGrammar(@"$(\d+\(1\x)", expected);
        }

        [Fact]
        public void EscapedBackslashInRegEx()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new RegExToken(@"\\a")
                },
                new List<Variable>());

            CompareGrammar(@"$(\\a)", expected);
        }

        [Fact]
        public void EscapedQuoteInLiteralToken()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new LiteralToken(@" lit ""eral"" ")
                },
                new List<Variable>());

            CompareGrammar(@"$"" lit \""eral\"" """, expected);
        }

        [Fact]
        public void EscapedVariableDefinition()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("token")
                },
                new List<Variable>()
                {
                    new Variable("variable", new List<Token>
                    {
                        new LiteralToken("$$token")
                    })
                });

            CompareGrammar(@"$token $$variable = \$\$token", expected);
        }

        [Fact]
        public void EscapedEscapeCharacter()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new LiteralToken(@"\")
                },
                new List<Variable>());

            CompareGrammar(@"\\", expected);
        }

        [Fact]
        public void ListTokenEscapedDelimiter()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new ListToken(new NamedToken("value"), AutomaticRangeType.Many, new[] {":"})
                },
                new List<Variable>());

            CompareGrammar(@"$value{*:\:}", expected);
        }

        [Fact]
        public void ListTokenOpeningBracket()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new ListToken(new NamedToken("value"), AutomaticRangeType.Many, new[] {"{"})
                },
                new List<Variable>());

            CompareGrammar(@"$value{*:\{}", expected);
        }

        [Fact]
        public void ListTokenEscapedClosingBracket()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new ListToken(new NamedToken("value"), AutomaticRangeType.Many, new[] {"}"})
                },
                new List<Variable>());

            CompareGrammar(@"$value{*:\}}", expected);
        }

        [Fact]
        public void ListTokenEscapedBrackets()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("value"),
                    new LiteralToken(@"{*:}}")
                },
                new List<Variable>());

            CompareGrammar(@"$value\{*:\}\}", expected);
        }

        [Fact]
        public void ListTokenEscapedColon()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new ListToken(new NamedToken("value"), 2, new[] {":"})
                },
                new List<Variable>());

            CompareGrammar(@"$value{2:\:}", expected);
        }

        [Fact]
        public void ListTokenPrefixInDelimiter()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new ListToken(new NamedToken("value"), AutomaticRangeType.Many, new[] {"$"})
                },
                new List<Variable>());

            CompareGrammar(@"$value{*:$}", expected);
        }
    }
}