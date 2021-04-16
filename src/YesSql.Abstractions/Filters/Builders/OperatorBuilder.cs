using OrchardCore.Filters.Nodes;
using OrchardCore.Filters.Services;
using Parlot.Fluent;

namespace OrchardCore.Filters.Builders
{
    public abstract class OperatorEngineBuilder<T, TTermOption> where TTermOption : TermOption
    {
        public abstract (Parser<OperatorNode> Parser, TTermOption TermOption) Build();
    }
}
