using System.Collections.Generic;
using Whiteparse.Grammar;
using Whiteparse.Grammar.Tokens;
using Xunit;
using Xunit.Abstractions;

namespace Whiteparse.Test
{
    public class GrammarSemantics : AbstractTestSuite
    {
        public GrammarSemantics(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void SeparateNestedScopes()
        {
            var expected = new Template(
                new List<Token>
                {
                    new InlineListToken(new List<Token>
                    {
                        new NamedToken("a"),
                        new NamedToken("b")
                    }),
                    new InlineListToken(new List<Token>
                    {
                        new NamedToken("a"),
                        new NamedToken("b")
                    })
                });

            CompareGrammar("[$a $b] [$a $b]", expected);
        }

        [Fact]
        public void TokenShadowing()
        {
            var expected = new Template(
                new List<Token>
                {
                    new NamedToken("point")
                },
                new List<Variable>
                {
                    new Variable("point", new List<Token>
                    {
                        new NamedToken("x"),
                        new NamedToken("y"),
                        new NamedToken("inner")
                    }),
                    new Variable("inner", new List<Token>
                    {
                        new NamedToken("x")
                    })
                });

            CompareGrammar("$point \n$$point = $x $y $inner\n$$inner = $x", expected);
        }

        [Fact]
        public void UnusedVariable()
        {
            var expected = new Template(
                new List<Token>
                {
                    new NamedToken("b")
                },
                new List<Variable>
                {
                    new Variable("a", new List<Token>
                    {
                        new NamedToken("c")
                    })
                });

            CompareGrammar("$b $$a = $c", expected);
        }

        [Fact]
        public void MultiUseVariable()
        {
            var expected = new Template(
                new List<Token>
                {
                    new NamedToken("a1"),
                    new NamedToken("a2")
                },
                new List<Variable>
                {
                    new Variable("a1", new List<Token>
                    {
                        new NamedToken("a")
                    }),
                    new Variable("a2", new List<Token>
                    {
                        new NamedToken("a")
                    }),
                    new Variable("a", new List<Token>
                    {
                        new NamedToken("end")
                    })
                });

            CompareGrammar("$a1 $a2 \n $$a1 = $a \n $$a2 = $a \n $$a = $end", expected);
        }
    }
}