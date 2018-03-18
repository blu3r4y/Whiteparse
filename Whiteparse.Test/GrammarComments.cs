using System.Collections.Generic;
using Whiteparse.Grammar;
using Whiteparse.Grammar.Tokens;
using Xunit;
using Xunit.Abstractions;

namespace Whiteparse.Test
{
    public class GrammarComments : AbstractTestSuite
    {
        public GrammarComments(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void SingleComment()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("token")
                });

            CompareGrammar("$token # a comment", expected);
        }

        [Fact]
        public void NestedComment()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("token")
                });

            CompareGrammar("$token ### #  ### a comment", expected);
        }

        [Fact]
        public void CommentInBetween()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("token1"),
                    new NamedToken("token2")
                });

            CompareGrammar("$token1 # a comment in between\n$token2", expected);
        }

        [Fact]
        public void CommentsEverywhere()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("token1"),
                    new NamedToken("tok")
                }, new List<Variable>
                {
                    new Variable("var", new List<Token>
                    {
                        new LiteralToken("1")
                    })
                });

            CompareGrammar("$token1 #comment1 #comment2 \n $tok#comment3 #\n#com\n\n#com\n$$var = 1#comment", expected);
        }

        [Fact]
        public void MultipleComments()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("token1"),
                    new NamedToken("token2")
                });

            CompareGrammar("$token1 # a comment \n $token2 # another comment", expected);
        }

        [Fact]
        public void CommentWithTokens()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("token1"),
                    new NamedToken("token2")
                });

            CompareGrammar("$token1 # a comment with $commented tokens\n$token2", expected);
        }

        [Fact]
        public void CommentInLiteralToken()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("token"),
                    new LiteralToken("lite#ral")
                });

            // comments are not allowed within literal tokens
            CompareGrammar("$token $\"lite#ral\"", expected);
        }

        [Fact]
        public void CommentInRegExToken()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("token"),
                    new RegExToken(".*\\d+#\\d+")
                });

            // comments are not allowed within regex tokens
            CompareGrammar("$token $(.*\\d+#\\d+)", expected);
        }

        [Fact]
        public void CommentInIdentifier()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("tok")
                });

            CompareGrammar("$tok#en", expected);
        }

        [Fact]
        public void CommentInText()
        {
            var expected = new Specification(
                new List<Token>
                {
                    new NamedToken("token"),
                    new LiteralToken("te")
                });

            CompareGrammar("$token te#xt", expected);
        }

        [Fact]
        public void CommentInVariableDefinition()
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
                        new NamedToken("var2")
                    })
                });

            CompareGrammar("$token \n $$var1 = $var2 # a comment", expected);
        }

        [Fact]
        public void CommentInVariableDefinitionMulti()
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
                        new NamedToken("var2")
                    }),
                    new Variable("var3", new List<Token>
                    {
                        new NamedToken("var4"),
                        new LiteralToken("text")
                    })
                });

            CompareGrammar("$token \n $$var1 = $var2 # a comment \n $$var3 = $var4 text # another comment", expected);
        }
    }
}