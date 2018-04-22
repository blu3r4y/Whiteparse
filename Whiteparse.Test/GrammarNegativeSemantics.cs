using Xunit;
using Xunit.Abstractions;

namespace Whiteparse.Test
{
    public class GrammarNegativeSemantics : AbstractTestSuite
    {
        public GrammarNegativeSemantics(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void DuplicateReferencedTokenDefinition()
        {
            // TODO: This should not fail in the future (but we need to solve the problem with duplicate json key values)
            FailGrammar("$point $point\n$$point = $x $y");
        }

        [Fact]
        public void DuplicateTokenDefinition()
        {
            FailGrammar("$token $token");
        }

        [Fact]
        public void DuplicateVariableDefinition()
        {
            FailGrammar("$token \n $$var = $a \n $$var = $b");
        }

        [Fact]
        public void DuplicateTokenInCurrentScope()
        {
            FailGrammar("$point \n$$point = $x $y $inner\n$$inner = $x $x");
        }

        [Fact]
        public void EmptyVariable()
        {
            FailGrammar("$point \n$$point = \n");
        }

        [Fact]
        public void RecursiveDefinition()
        {
            FailGrammar("$a $$a = $a");
        }

        [Fact]
        public void RecursiveDefinitionIndirect()
        {
            FailGrammar("$a $$a = $b\n$$b = $a");
        }

        [Fact]
        public void RecursiveDefinitionDiamond()
        {
            FailGrammar("$start $$start = $m1 \n $m2 $$m1 = $end \n $$m2 == $end \n $$end = $start");
        }

        [Fact]
        public void ListTokenReferenceNotDefined()
        {
            FailGrammar("$nope $value{$size}");
        }

        [Fact]
        public void ListTokenReferenceForwardReference()
        {
            FailGrammar("$value{$size} $size");
        }
    }
}