using Parlot;
using Parlot.Fluent;
using System;
using YesSql.Core.QueryParser;
using static Parlot.Fluent.Parsers;

namespace YesSql.Core.DocumentParser.Builders
{
    public abstract class DocumentTermParserBuilder<T> where T : class
    {
        public DocumentTermParserBuilder(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public bool Single { get; }

        protected DocumentOperatorParserBuilder<T> _operatorParser;

        public DocumentTermParserBuilder<T> SetOperator(DocumentOperatorParserBuilder<T> operatorParser)
        {
            _operatorParser = operatorParser;

            return this;
        }

        public abstract (Parser<TermNode> Parser, DocumentTermOption<T> TermOption) Build();
    }
}
