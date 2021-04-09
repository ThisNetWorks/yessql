using Parlot.Fluent;
using YesSql.Core.QueryParser;

namespace YesSql.Core.DocumentParser.Builders
{
    public abstract class DocumentOperatorParserBuilder<T> where T : class
    {
        public abstract (Parser<OperatorNode> Parser, DocumentTermOption<T> TermOption) Build();
    }
}