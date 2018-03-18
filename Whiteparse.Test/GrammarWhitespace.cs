using System.Collections.Generic;
using Whiteparse.Grammar;
using Whiteparse.Grammar.Tokens;
using Xunit;
using Xunit.Abstractions;

namespace Whiteparse.Test
{
    public class GrammarWhitespace : AbstractTestSuite
    {
        public GrammarWhitespace(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void OnlyWhiteSpace()
        {
            var expected = new Specification(
                new List<Token>(),
                new List<Variable>());

            CompareGrammar("      \r \r\n \t \f \r ", expected);
        }

        [Fact]
        public void SingleWhitespacedToken()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("whitespaced")
                },
                new List<Variable>());

            CompareGrammar("      $whitespaced       ", expected);
        }

        [Fact]
        public void WhitespaceTokenAndLiterals()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("whitesp"),
                    new LiteralToken("aced")
                },
                new List<Variable>());

            CompareGrammar("      $whitesp aced       ", expected);
        }

        [Fact]
        public void ThreeWhitespacedTokens()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("white1"),
                    new NamedToken("white2"),
                    new NamedToken("white3")
                },
                new List<Variable>());

            CompareGrammar("      $white1    $white2  $white3 ", expected);
        }

        [Fact]
        public void LinebreakedWhitespaceTokens()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("white1"),
                    new NamedToken("white2"),
                    new NamedToken("white3"),
                    new NamedToken("white4")
                },
                new List<Variable>());

            CompareGrammar(" \n\n\r\n     $white1 \n\n \r\n  $white2\r\n  $white3 $white4\r\n\n \n", expected);
        }

        [Fact]
        public void LinebreakedIdentifier()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("whi"),
                    new LiteralToken("te1"),
                    new NamedToken("whit"),
                    new LiteralToken("e2")
                },
                new List<Variable>());

            CompareGrammar("      $whi\nte1 \n\n \r\n  $whit\re2 \n", expected);
        }

        [Fact]
        public void NoWhitespace()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("a"),
                    new LiteralToken("lit"),
                    new NamedToken("b"),
                    new RegExToken("\\d+"),
                    new NamedToken("c"),
                    new NamedToken("d"),
                    new NamedToken("efgh", TokenType.Auto, true)
                },
                new List<Variable>());

            CompareGrammar("$a$\"lit\"$b$(\\d+)$c$d$?efgh", expected);
        }

        [Fact]
        public void MinimumWhiteSpace()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("numLocations", TokenType.Int, true),
                    new NamedToken("location"),
                    new NamedToken("x"),
                    new NamedToken("y"),
                    new LiteralToken("hub="),
                    new NamedToken("hub")
                },
                new List<Variable>()
                {
                    new Variable("location", new List<Token>
                    {
                        new NamedToken("name"),
                        new NamedToken("pos")
                    }),
                    new Variable("journey", new List<Token>
                    {
                        new NamedToken("from"),
                        new NamedToken("to")
                    })
                });

            CompareGrammar("$?[int]numLocations$location$x$y$$location=$name$pos\n" +
                           "hub=$hub$$journey=$from$to", expected);
        }

        [Fact]
        public void NoWhiteSpaceNewLineToken()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NewLineToken(),
                    new NewLineToken()
                },
                new List<Variable>());

            CompareGrammar("$.$.", expected);
        }

        [Fact]
        public void RegExWithEscapedLineBreak()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new RegExToken("a\\r\\nb")
                },
                new List<Variable>());

            CompareGrammar("$(a\\r\\nb)", expected);
        }

        [Fact]
        public void InlineListAndTrailingToken()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new InlineListToken(new List<Token>
                    {
                        new NamedToken("x"),
                        new NamedToken("y")
                    }),
                    new NamedToken("a")
                },
                new List<Variable>());

            CompareGrammar("[$x $y]$a", expected);
        }

        [Fact]
        public void InlineListAndLeadingToken()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("a"),
                    new InlineListToken(new List<Token>
                    {
                        new NamedToken("x"),
                        new NamedToken("y")
                    })
                },
                new List<Variable>());

            CompareGrammar("$a[$x $y]", expected);
        }

        [Fact]
        public void DenseInlineList()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new InlineListToken(new List<Token>
                    {
                        new NamedToken("x"),
                        new NamedToken("y"),
                        new NamedToken("z")
                    })
                },
                new List<Variable>());

            CompareGrammar("[$x$y$z]", expected);
        }

        [Fact]
        public void VariableWithLeadingLineContinuation()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("tok")
                },
                new List<Variable>
                {
                    new Variable("var", new List<Token>
                    {
                        new NamedToken("a"),
                        new NamedToken("b")
                    })
                });

            CompareGrammar("$$var =\\\n $a $b \n $tok", expected);
        }

        [Fact]
        public void VariableWithTrailingLineContinuation()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("tok")
                },
                new List<Variable>
                {
                    new Variable("var", new List<Token>
                    {
                        new NamedToken("a"),
                        new NamedToken("b")
                    })
                });

            CompareGrammar("$$var = $a \\\n $b \\\n \n $tok", expected);
        }

        [Fact]
        public void VariableWithEmptyLineContinuations()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("tok")
                },
                new List<Variable>
                {
                    new Variable("var", new List<Token>
                    {
                        new NamedToken("a"),
                        new NamedToken("b")
                    })
                });

            CompareGrammar("$$var =\\\n \\\n\\\n $a\\\n\\\n $b\\\n \\\n\n $tok", expected);
        }

        [Fact]
        public void MultiLineWhiteSpaceWithComments()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("a"),
                    new LiteralToken("lit"),
                    new NamedToken("b"),
                    new RegExToken("\\d+"),
                    new NamedToken("c"),
                    new NamedToken("d"),
                    new NamedToken("efgh", TokenType.Auto, true)
                },
                new List<Variable>());

            CompareGrammar("\n\n\n\n#comment\n#comment\n\n" +
                           "$a$\"lit\"#comment\n\n\n#comment\n\n" +
                           "$b\n\n\n\t\t\t$(\\d+)\t\t" +
                           "$c#######\n\n\t\t\t" +
                           "$d#??$?\\##\n\n\n" +
                           "$?efgh", expected);
        }
    }
}