using System.Collections.Generic;
using Whiteparse.Grammar;
using Whiteparse.Grammar.Tokens;
using Xunit;
using Xunit.Abstractions;

namespace Whiteparse.Test
{
    public class GrammarMix : AbstractTestSuite
    {
        public GrammarMix(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void SingleNamedToken()
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

            CompareGrammar("$?[int]numLocations\n" +
                           "$location $x $y\n\n" +
                           "\t$$location = $name $pos\n\n" +
                           "hub=$hub\n\n" +
                           "\t$$journey = $from $to", expected);
        }

        [Fact]
        public void InvalidTrailingDotInStructuredToken()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken(new[] {"a", "b", "c"}),
                    new LiteralToken(".")
                },
                new List<Variable>());

            CompareGrammar("$a.b.c.", expected);
        }

        [Fact]
        public void MultipleDotsInIdentifier()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("a"),
                    new LiteralToken("..b.c.")
                },
                new List<Variable>());

            CompareGrammar("$a..b.c.", expected);
        }

        [Fact]
        public void ListTokenInVariable()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("size"),
                    new NamedToken("var")
                },
                new List<Variable>
                {
                    new Variable("var", new List<Token>
                    {
                        new ListToken(new NamedToken("element"), "size"),
                    })
                });

            CompareGrammar("$size $var \n $$var = $element{$size}", expected);
        }

        [Fact]
        public void ListTokenForVariable()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("size"),
                    new ListToken(
                        new NamedToken("var"), "size")
                },
                new List<Variable>
                {
                    new Variable("var", new List<Token>
                    {
                        new LiteralToken("abc")
                    })
                });

            CompareGrammar("$size $var{$size} \n $$var = abc", expected);
        }

        [Fact]
        public void WhiteSpacedListToken()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("size"),
                    new NamedToken("token"),
                    new LiteralToken("{"),
                    new NamedToken("size2"),
                    new LiteralToken("}")
                },
                new List<Variable>());

            CompareGrammar(@"$size $token \{$size2\}", expected);
        }

        [Fact]
        public void InlineListWithListTokenExtension()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new ListToken(
                        new InlineListToken(new List<Token>
                        {
                            new NamedToken("a"),
                            new NamedToken("b")
                        }), 10)
                },
                new List<Variable>());

            CompareGrammar("[$a $b]{10}", expected);
        }

        [Fact]
        public void TokenAfterVariable()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("b")
                },
                new List<Variable>
                {
                    new Variable("var", new List<Token>
                    {
                        new LiteralToken("123"),
                        new NamedToken("a")
                    })
                });

            CompareGrammar("$$var = 123 $a \n $b", expected);
        }

        [Fact]
        public void LiteralAfterVariable()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new LiteralToken("a")
                },
                new List<Variable>
                {
                    new Variable("var", new List<Token>
                    {
                        new LiteralToken("123"),
                        new NamedToken("a")
                    })
                });

            CompareGrammar("$$var = 123 $a \n a", expected);
        }
    }
}