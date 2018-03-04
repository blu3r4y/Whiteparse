using System.Collections.Generic;
using Whiteparse.Grammar;
using Whiteparse.Grammar.Tokens;
using Xunit;
using Xunit.Abstractions;

namespace Whiteparse.Test
{
    public class GrammarVariables : AbstractTestSuite
    {
        public GrammarVariables(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void OneVariable()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("token")
                },
                new List<Variable>
                {
                    new Variable("var", new List<Token>
                    {
                        new NamedToken("token")
                    })
                });

            CompareGrammar("$token\n$$var = $token", expected);
        }

        [Fact]
        public void TwoVariables()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("token")
                },
                new List<Variable>
                {
                    new Variable("var1", new List<Token>
                    {
                        new NamedToken("token1")
                    }),
                    new Variable("var2", new List<Token>
                    {
                        new NamedToken("token2")
                    })
                });

            CompareGrammar("$token\n$$var1 = $token1 \n$$var2 = $token2", expected);
        }

        [Fact]
        public void SinglelineVariables()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("token")
                },
                new List<Variable>
                {
                    new Variable("var1", new List<Token>
                    {
                        new NamedToken("token1")
                    }),
                    new Variable("var2", new List<Token>
                    {
                        new NamedToken("token2")
                    })
                });

            CompareGrammar("$token $$var1 = $token1 $$var2 = $token2", expected);
        }

        [Fact]
        public void InbetweenVariables()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("token1"),
                    new NamedToken("token2"),
                    new NamedToken("token3")
                },
                new List<Variable>
                {
                    new Variable("token1", new List<Token>
                    {
                        new LiteralToken("1")
                    }),
                    new Variable("token2", new List<Token>
                    {
                        new LiteralToken("2")
                    }),
                    new Variable("token3", new List<Token>
                    {
                        new LiteralToken("abc")
                    })
                });

            CompareGrammar("$token1 \t\t $$token1 = 1 \n\t $token2 \n\t$$token2 = 2 \n $token3 \r\n\t$$token3 = abc", expected);
        }

        [Fact]
        public void VariablesAndText()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("token")
                },
                new List<Variable>
                {
                    new Variable("var1", new List<Token>
                    {
                        new LiteralToken("pre"),
                        new NamedToken("token1")
                    }),
                    new Variable("var2", new List<Token>
                    {
                        new NamedToken("token2"),
                        new LiteralToken("post")
                    })
                });

            CompareGrammar("$token $$var1 = pre $token1 $$var2 = $token2 post", expected);
        }

        [Fact]
        public void MultipleEqualSigns()
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
                        new LiteralToken("="),
                        new LiteralToken("equal=")
                    })
                });

            CompareGrammar("$token \n $$variable = = equal=", expected);
        }

        [Fact]
        public void MultiLineVariableStatement()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("token")
                },
                new List<Variable>
                {
                    new Variable("variable", new List<Token>
                    {
                        new NamedToken("a"),
                        new NamedToken("b"),
                        new NamedToken("c"),
                        new NamedToken("d"),
                        new NamedToken("e"),
                        new NamedToken("f"),
                        new NamedToken("g"),
                        new NamedToken("h"),
                        new NamedToken("i")
                    })
                });

            CompareGrammar("$token $$variable = $a $b $c \\\n $d $e $f \\\n $g $h $i", expected);
        }

        [Fact]
        public void MultiLineVariableStatementWithTrailingWhiteSpace()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("token")
                },
                new List<Variable>
                {
                    new Variable("variable", new List<Token>
                    {
                        new NamedToken("a"),
                        new NamedToken("b"),
                        new NamedToken("c")
                    })
                });

            CompareGrammar("$token $$variable = $a \\ \t  \t \n $b $c ", expected);
        }

        [Fact]
        public void ObjectTypedVariables()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("token")
                },
                new List<Variable>
                {
                    new Variable("point", new List<Token>
                    {
                        new NamedToken("x"),
                        new NamedToken("y")
                    }, "Point")
                });

            CompareGrammar("$token\n$$[Point]point = $x $y", expected);
        }

        [Fact]
        public void ObjectTypedVariablesWithUnderscore()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("token")
                },
                new List<Variable>
                {
                    new Variable("point", new List<Token>
                    {
                        new NamedToken("x"),
                        new NamedToken("y")
                    }, "Point_Type")
                });

            CompareGrammar("$token\n$$[Point_Type]point = $x $y", expected);
        }

        [Fact]
        public void ObjectTypedVariablesWithLeadingUnderscore()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("token")
                },
                new List<Variable>
                {
                    new Variable("point", new List<Token>
                    {
                        new NamedToken("x"),
                        new NamedToken("y")
                    }, "_Point")
                });

            CompareGrammar("$token\n$$[_Point]point = $x $y", expected);
        }

        [Fact]
        public void ObjectTypedVariablesWithTrailingUnderscore()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("token")
                },
                new List<Variable>
                {
                    new Variable("point", new List<Token>
                    {
                        new NamedToken("x"),
                        new NamedToken("y")
                    }, "Point_")
                });

            CompareGrammar("$token\n$$[Point_]point = $x $y", expected);
        }


        [Fact]
        public void ObjectTypedVariablesWithDots()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("token")
                },
                new List<Variable>
                {
                    new Variable("inner", new List<Token>
                    {
                        new NamedToken("x"),
                        new NamedToken("y")
                    }, "SomeClass._SomeFeature._Inner")
                });

            CompareGrammar("$token\n$$[SomeClass._SomeFeature._Inner]inner = $x $y", expected);
        }


        [Fact]
        public void ObjectTypedVariablesWithUnicode()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("token")
                },
                new List<Variable>
                {
                    new Variable("inner", new List<Token>
                    {
                        new NamedToken("x"),
                        new NamedToken("y")
                    }, "SomeClass._SomeFäüöüeature._Inner")
                });

            CompareGrammar("$token\n$$[SomeClass._SomeFäüöüeature._Inner]inner = $x $y", expected);
        }
    }
}