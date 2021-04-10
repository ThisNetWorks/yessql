using Parlot.Fluent;

namespace YesSql.Core.FilterEngines.Builders
{
    public abstract class OperatorEngineBuilder<T, TTermOption> where T : class where TTermOption : TermOption
    {
        public abstract (Parser<OperatorNode> Parser, TTermOption TermOption) Build();
    }
}
