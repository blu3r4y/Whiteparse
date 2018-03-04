using System.Collections.Generic;
using Whiteparse.Grammar;
using Whiteparse.Grammar.Tokens;
using Whiteparse.Grammar.Tokens.Ranges;
using Xunit;
using Xunit.Abstractions;

namespace Whiteparse.Test
{
    public class GrammarTokens : AbstractTestSuite
    {
        public GrammarTokens(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Empty()
        {
            var expected = new Specification(
                new List<Token>(),
                new List<Variable>());

            CompareGrammar("", expected);
        }

        [Fact]
        public void Text()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new LiteralToken("text")
                },
                new List<Variable>());

            CompareGrammar("text", expected);
        }

        [Fact]
        public void TextUnicode()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new LiteralToken("\u50dd\u1365\u4e5c\u2441\u6b18\u4921\u6fc3\u052c"),
                },
                new List<Variable>());

            CompareGrammar("\u50dd\u1365\u4e5c\u2441\u6b18\u4921\u6fc3\u052c", expected);
        }

        [Fact]
        public void NamedTokenSingle()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("token")
                },
                new List<Variable>());

            CompareGrammar("$token", expected);
        }

        [Fact]
        public void NamedTokenTriple()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("a"),
                    new NamedToken("b"),
                    new NamedToken("c")
                },
                new List<Variable>());

            CompareGrammar("$a $b $c", expected);
        }

        [Fact]
        public void NamedTokenStructured()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken(new[] {"a", "b", "c"})
                },
                new List<Variable>());

            CompareGrammar("$a.b.c", expected);
        }

        [Fact]
        public void HiddenNamedToken()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("named", TokenDataType.Auto, true),
                    new NamedToken("typed", TokenDataType.Int, true)
                },
                new List<Variable>());

            CompareGrammar("$?named $?[int]typed", expected);
        }

        [Fact]
        public void HiddenRegexToken()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new RegExToken(".*?\\d+\\s\\w", true),
                },
                new List<Variable>());

            CompareGrammar("$?(.*?\\d+\\s\\w)", expected);
        }

        [Fact]
        public void TypedToken()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("token1", TokenDataType.Int),
                    new NamedToken("token2", TokenDataType.Float),
                    new NamedToken("token3", TokenDataType.String),
                    new NamedToken("token4", TokenDataType.Bool)
                },
                new List<Variable>());

            CompareGrammar("$[int]token1 $[float]token2 $[string]token3 $[bool]token4", expected);
        }

        [Fact]
        public void LiteralToken()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new LiteralToken("  something literal  "),
                    new LiteralToken("NJRpT7u=:*8SN:$/wapCC&dW9 R@*aeZFtPUYqfEk @JUC-&3_&RTYxO+x"),
                },
                new List<Variable>());

            CompareGrammar("$\"  something literal  \" $\"NJRpT7u=:*8SN:$/wapCC&dW9 R@*aeZFtPUYqfEk @JUC-&3_&RTYxO+x\"", expected);
        }

        [Fact]
        public void LiteralTokenEmpty()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new LiteralToken("")
                },
                new List<Variable>());

            CompareGrammar("$\"\"", expected);
        }

        [Fact]
        public void LiteralTokenUnicode()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new LiteralToken("\u50dd\u1365\u4e5c\u2441\u6b18\u4921\u6fc3\u052c"),
                },
                new List<Variable>());

            CompareGrammar("$\"\u50dd\u1365\u4e5c\u2441\u6b18\u4921\u6fc3\u052c\"", expected);
        }

        [Fact]
        public void LiteralTokenEqualSign()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new LiteralToken("="),
                    new LiteralToken("5"),
                    new LiteralToken("+"),
                    new LiteralToken("2")
                },
                new List<Variable>());

            CompareGrammar("= 5 + 2", expected);
        }

        [Fact]
        public void RegExTokenSimple()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new RegExToken(".*?\\d+\\s\\w"),
                },
                new List<Variable>());

            CompareGrammar("$(.*?\\d+\\s\\w)", expected);
        }

        [Fact]
        public void RegExTokenEmpty()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new RegExToken("")
                },
                new List<Variable>());

            CompareGrammar("$()", expected);
        }

        [Fact]
        public void RegExTokenWithParenthesis()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new RegExToken(".*@(.+\\.){3}"),
                },
                new List<Variable>());

            CompareGrammar("$(.*@(.+\\.){3})", expected);
        }

        [Fact]
        public void RegExTokenWithNestedParenthesisSingle()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new RegExToken("\\d(  (7) )asd"),
                },
                new List<Variable>());

            CompareGrammar("$(\\d(  (7) )asd)", expected);
        }

        [Fact]
        public void RegExTokenWithNestedParenthesisMulti()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new RegExToken("a()\\d+(\\w(\\d\\s+ )+\\d+(\\d(\\d)) (7) )"),
                },
                new List<Variable>());

            CompareGrammar("$(a()\\d+(\\w(\\d\\s+ )+\\d+(\\d(\\d)) (7) ))", expected);
        }

        [Fact]
        public void RegExTokenComplex()
        {
            // (c) matthew o'riordan https://regex101.com/library/cX0pJ8

            var expected = new Specification(
                new List<Token>
                {
                    new RegExToken("((([A-Za-z]{3,9}:(?:\\/\\/)?)(?:[-;:&=\\+\\$" +
                                   ",\\w]+@)?[A-Za-z0-9.-]+|(?:www.|[-;:&=\\+\\$" +
                                   ",\\w]+@)[A-Za-z0-9.-]+)((?:\\/[\\+~%\\/.\\w-" +
                                   "_]*)?\\??(?:[-\\+=&;%@.\\w_]*)#?(?:[.\\!\\/\\w]*))?)"),
                },
                new List<Variable>());

            CompareGrammar("$(((([A-Za-z]{3,9}:(?:\\/\\/)?)(?:[-;:&=\\+\\$" +
                           ",\\w]+@)?[A-Za-z0-9.-]+|(?:www.|[-;:&=\\+\\$" +
                           ",\\w]+@)[A-Za-z0-9.-]+)((?:\\/[\\+~%\\/.\\w-" +
                           "_]*)?\\??(?:[-\\+=&;%@.\\w_]*)#?(?:[.\\!\\/\\w]*))?))", expected);
        }

        [Fact]
        public void RegExTokenRidiculous()
        {
            // (c) Chas. Owens https://stackoverflow.com/a/800847/927377

            var expected = new Specification(
                new List<Token>
                {
                    new RegExToken("(?:(?:(?:0?[13578]|1[02])(\\/|-|\\.)31)\\1|(" +
                                   "?:(?:0?[13-9]|1[0-2])(\\/|-|\\.)(?:29|30)\\2" +
                                   "))(?:(?:1[6-9]|[2-9]\\d)?\\d{2})$|^(?:0?2(\\" +
                                   "/|-|\\.)29\\3(?:(?:(?:1[6-9]|[2-9]\\d)?(?:0[" +
                                   "48]|[2468][048]|[13579][26])|(?:(?:16|[2468]" +
                                   "[048]|[3579][26])00))))$|^(?:(?:0?[1-9])|(?:" +
                                   "1[0-2]))(\\/|-|\\.)(?:0?[1-9]|1\\d|2[0-8])\\" +
                                   "4(?:(?:1[6-9]|[2-9]\\d)?\\d{2})"),
                },
                new List<Variable>());

            CompareGrammar("$((?:(?:(?:0?[13578]|1[02])(\\/|-|\\.)31)\\1|(" +
                           "?:(?:0?[13-9]|1[0-2])(\\/|-|\\.)(?:29|30)\\2" +
                           "))(?:(?:1[6-9]|[2-9]\\d)?\\d{2})$|^(?:0?2(\\" +
                           "/|-|\\.)29\\3(?:(?:(?:1[6-9]|[2-9]\\d)?(?:0[" +
                           "48]|[2468][048]|[13579][26])|(?:(?:16|[2468]" +
                           "[048]|[3579][26])00))))$|^(?:(?:0?[1-9])|(?:" +
                           "1[0-2]))(\\/|-|\\.)(?:0?[1-9]|1\\d|2[0-8])\\" +
                           "4(?:(?:1[6-9]|[2-9]\\d)?\\d{2}))", expected);
        }

        [Fact]
        public void NewLineTokenSingle()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NewLineToken()
                },
                new List<Variable>());

            CompareGrammar("$;", expected);
        }

        [Fact]
        public void NewLineTokenDouble()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NewLineToken(),
                    new NewLineToken()
                },
                new List<Variable>());

            CompareGrammar("$; $;", expected);
        }

        [Fact]
        public void ListTokenNumeric()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new ListToken(new NamedToken("value"), 2)
                },
                new List<Variable>());

            CompareGrammar("$value{2}", expected);
        }

        [Fact]
        public void ListTokenReference()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("size"),
                    new ListToken(new NamedToken("value"), new NamedToken("size"))
                },
                new List<Variable>());

            CompareGrammar("$size $value{$size}", expected);
        }

        [Fact]
        public void ListTokenAutomatic()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new ListToken(new NamedToken("a"), AutomaticRangeType.AtLeastOnce),
                    new ListToken(new NamedToken("b"), AutomaticRangeType.Many),
                    new ListToken(new NamedToken("c"), AutomaticRangeType.ManyLazy)
                },
                new List<Variable>());

            CompareGrammar("$a{+} $b{*} $c{*?}", expected);
        }

        [Fact]
        public void ListTokenNested()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new ListToken(
                        new ListToken(new NamedToken("value"), 2), 4)
                },
                new List<Variable>());

            CompareGrammar("$value{2}{4}", expected);
        }

        [Fact]
        public void ListTokenNestedMultiple()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("size"),
                    new ListToken(
                        new ListToken(
                            new ListToken(
                                new ListToken(new NamedToken("value"), 2), 4), new NamedToken("size")), AutomaticRangeType.Many)
                },
                new List<Variable>());

            CompareGrammar("$size $value{2}{4}{$size}{*}", expected);
        }

        [Fact]
        public void ListTokenDelimitedSingle()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new ListToken(new NamedToken("value"), AutomaticRangeType.Many, new[] {","})
                },
                new List<Variable>());

            CompareGrammar("$value{*:,}", expected);
        }

        [Fact]
        public void ListTokenDelimitedMultiple()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new ListToken(new NamedToken("value"), AutomaticRangeType.Many, new[] {",", ";", "|", " "})
                },
                new List<Variable>());

            CompareGrammar("$value{*:,:;:|: }", expected);
        }

        [Fact]
        public void ListTokenDelimitedStrings()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new ListToken(new NamedToken("value"), AutomaticRangeType.Many, new[] {"ab", "cd"})
                },
                new List<Variable>());

            CompareGrammar("$value{*:ab:cd}", expected);
        }

        [Fact]
        public void InlineListTokenSimple()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new InlineListToken(new List<Token>
                    {
                        new NamedToken("x"),
                        new NamedToken("y")
                    })
                },
                new List<Variable>());

            CompareGrammar("[$x $y]", expected);
        }

        [Fact]
        public void InlineListTokenMultiple()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new InlineListToken(new List<Token>
                    {
                        new NamedToken("x"),
                        new NamedToken("y")
                    }),
                    new InlineListToken(new List<Token>
                    {
                        new NamedToken("a"),
                        new NamedToken("b")
                    })
                },
                new List<Variable>());

            CompareGrammar("[$x $y] [$a $b]", expected);
        }

        [Fact]
        public void InlineListMixed()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new InlineListToken(new List<Token>
                    {
                        new NamedToken("x"),
                        new RegExToken("\\d+"),
                        new LiteralToken(" literal ")
                    })
                },
                new List<Variable>());

            CompareGrammar("[$x $(\\d+) $\" literal \"]", expected);
        }
    }
}