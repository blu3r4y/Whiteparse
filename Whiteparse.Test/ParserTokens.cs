using Xunit;
using Xunit.Abstractions;

namespace Whiteparse.Test
{
    public class ParserTokens : AbstractTestSuite
    {
        public ParserTokens(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Empty()
        {
            var expected = new { };

            CompareResult("", "", expected);
        }

        [Fact]
        public void Text()
        {
            var expected = new { };

            CompareResult("text", "text", expected);
        }

        [Fact]
        public void TextUnicode()
        {
            var expected = new { };

            CompareResult("\u50dd\u1365\u4e5c\u2441\u6b18\u4921\u6fc3\u052c",
                "\u50dd\u1365\u4e5c\u2441\u6b18\u4921\u6fc3\u052c", expected);
        }

        [Fact]
        public void NamedTokenSingle()
        {
            var expected = new
            {
                key = "value"
            };

            CompareResult("$key", "value", expected);
        }

        [Fact]
        public void ThreeIntegersAuto()
        {
            var expected = new
            {
                a = 1,
                b = 2,
                c = 3
            };

            CompareResult("$a $b $c", "1 2 3", expected);
        }

        [Fact]
        public void ThreeFloatsAuto()
        {
            var expected = new
            {
                a = 12.3,
                b = 133.78,
                c = -0.89
            };

            CompareResult("$a $b $c", "12.3 133.78 -0.89", expected);
        }

        [Fact]
        public void ThreeStringsAuto()
        {
            var expected = new
            {
                a = "aa",
                b = "bb",
                c = "cc"
            };

            CompareResult("$a $b $c", "aa bb cc", expected);
        }

        [Fact]
        public void ThreeBoolsAuto()
        {
            var expected = new
            {
                a = true,
                b = false,
                c = false
            };

            CompareResult("$a $b $c", "true false false", expected);
        }

        [Fact]
        public void ThreeTypedTokens()
        {
            var expected = new
            {
                a = 42,
                b = 13.37,
                c = "Hello",
                d = true
            };

            CompareResult("$[int]a $[float]b $[string]c $[bool]d",
                "42 13.37 Hello true", expected);
        }

        [Fact]
        public void NamedTokenStructured()
        {
            var expected = new
            {
                a = new
                {
                    b = new
                    {
                        c = "Token"
                    }
                }
            };

            CompareResult("$a.b.c", "Token", expected);
        }

        [Fact]
        public void NamedTokenStructuredMultiple()
        {
            var expected = new
            {
                a = new
                {
                    x = "ax",
                    y = "ay"
                },
                b = new
                {
                    c = "bc",
                    d = new
                    {
                        a = "bda",
                        b = "bdb"
                    }
                }
            };

            CompareResult("$a.x $a.y $b.c $b.d.a $b.d.b", "ax ay bc bda bdb", expected);
        }

        [Fact]
        public void HiddenNamedToken()
        {
            var expected = new
            {
                typed = 1337
            };

            CompareResult("$?named $?[int]typed", "ignored 1337", expected);
        }

        [Fact]
        public void HiddenRegexToken()
        {
            var expected = new { };

            CompareResult("$?(\\d+)", "1337", expected);
        }

        [Fact]
        public void LiteralToken()
        {
            var expected = new { };

            CompareResult("$'  something literal  '", "  something literal  ", expected);
        }

        [Fact]
        public void LiteralTokenEmpty()
        {
            var expected = new { };

            CompareResult("$''", "", expected);
        }

        [Fact]
        public void LiteralTokenUnicode()
        {
            var expected = new { };

            CompareResult("$\"\u50dd\u1365\u4e5c\u2441\u6b18\u4921\u6fc3\u052c\"",
                "\u50dd\u1365\u4e5c\u2441\u6b18\u4921\u6fc3\u052c", expected);
        }

        [Fact]
        public void RegExTokenEmpty()
        {
            var expected = new { };

            CompareResult("$()", "", expected);
        }

        [Fact]
        public void RegExTokenUnicode()
        {
            var expected = new { };

            CompareResult(@"$(\d+\(1\x43)", "55(1C", expected);
        }

        [Fact]
        public void NewLineTokenSingle()
        {
            var expected = new { };

            CompareResult("$.", "\n", expected);
        }


        [Fact]
        public void ListTokenNumeric()
        {
            var expected = new
            {
                values = new[]
                {
                    1, 2
                }
            };

            CompareResult("$value{2}", "1 2", expected);
        }

        [Fact]
        public void ListTokenReference()
        {
            var expected = new
            {
                size = 5,
                values = new[]
                {
                    1, 2, 3, 4, 5
                }
            };

            CompareResult("$size $value{$size}", "5 \n 1 2 3 4 5", expected);
        }

        [Fact]
        public void ListTokenAtLeastOnce()
        {
            var expected = new
            {
                values = new[]
                {
                    1, 2, 3, 4, 5
                }
            };

            CompareResult("$value{+}", "1 2 3 4 5", expected);
        }

        [Fact]
        public void ListTokenMany()
        {
            var expected = new
            {
                values = new[]
                {
                    1, 2, 3, 4, 5
                }
            };

            CompareResult("$value{*}", "1 2 3 4 5", expected);
        }

        [Fact]
        public void ListTokenManyEmpty()
        {
            var expected = new
            {
                values = new object[0]
            };

            CompareResult("$value{*}", "", expected);
        }

        [Fact]
        public void ListTokenNested()
        {
            var expected = new
            {
                values = new[]
                {
                    new[] {"a1", "a2"},
                    new[] {"b1", "b2"},
                    new[] {"c1", "c2"},
                    new[] {"d1", "d2"}
                }
            };

            CompareResult("$value{2}{4}", "a1 a2 b1 b2 c1 c2 d1 d2", expected);
        }

        [Fact]
        public void ListTokenDelimitedSingle()
        {
            var expected = new
            {
                values = new[]
                {
                    "a", "b", "c", "d", "e"
                }
            };

            CompareResult("$value{*:,}", "a,b,c,d,e", expected);
        }

        [Fact]
        public void ListTokenDelimitedMultiple()
        {
            var expected = new
            {
                values = new[]
                {
                    "a", "b", "c", "d", "e", "f", "g", "h"
                }
            };

            CompareResult("$value{*:,:;:|: }", "a,b:c;d,e|f g,h", expected);
        }

        [Fact]
        public void ListTokenDelimitedStrings()
        {
            var expected = new
            {
                values = new[]
                {
                    "a", "b", "c"
                }
            };

            CompareResult("$value{*:ab:cd}", "aabbcdc", expected);
        }

        [Fact]
        public void InlineListTokenSimple()
        {
            var expected = new
            {
                xy = new[] {1, 2}
            };

            CompareResult("[$x $y]", "1 2", expected);
        }

        [Fact]
        public void InlineListTokenMultiple()
        {
            var expected = new
            {
                xy = new[] {1, 2},
                ab = new[] {3, 4}
            };

            CompareResult("[$x $y] [$a $b]", "1 2 3 4", expected);
        }
    }
}