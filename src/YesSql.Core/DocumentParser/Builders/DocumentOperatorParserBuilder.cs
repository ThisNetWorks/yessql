using Parlot.Fluent;

namespace YesSql.Core.QueryParser.Builders
{
    public abstract class OperatorParserBuilder<T> where T : class
    {
        public abstract (Parser<OperatorNode> Parser, QueryTermOption2<T> TermOption) Build();
    }
}