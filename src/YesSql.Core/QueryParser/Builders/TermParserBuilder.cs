using Parlot;
using Parlot.Fluent;
using System;
using static Parlot.Fluent.Parsers;

namespace YesSql.Core.QueryParser.Builders
{
    public abstract class TermParserBuilder<T> where T : class
    {
        public TermParserBuilder(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public bool Single { get; }

        protected OperatorParserBuilder<T> _operatorParser;

        public TermParserBuilder<T> SetOperator(OperatorParserBuilder<T> operatorParser)
        {
            _operatorParser = operatorParser;

            return this;
        }

        public abstract (Parser<TermNode> Parser, TermOption<T> TermOption) Build();
    }
}
