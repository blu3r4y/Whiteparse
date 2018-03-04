using Xunit;
using Xunit.Abstractions;

namespace Whiteparse.Test
{
    public class GrammarNegativeSemantics : AbstractTestSuite
    {
        public GrammarNegativeSemantics(ITestOutputHelper output) : base(output)
        {
        }

        [Fact(Skip = "Semantic checks are not implemented yet")]
        public void DuplicateTokenDefinition()
        {
            FailGrammar("$token $token");
        }

        [Fact(Skip = "Semantic checks are not implemented yet")]
        public void DuplicateVariableDefinition()
        {
            FailGrammar("$token \n $$var = $a \n $$var = $b");
        }
    }
}