using Parlot;
using Parlot.Fluent;
using System;
using static Parlot.Fluent.Parsers;

namespace YesSql.Core.QueryParser.Builders
{
    public class DefaultTermParserBuilder<T> : TermParserBuilder<T> where T : class
    {
        public DefaultTermParserBuilder(string name) : base(name)
        {
        }

        public override (Parser<TermNode> Parser, TermOption<T> TermOption) Build()
        {
            var op = _operatorParser.Build();

            var termParser = Terms.Text(Name, caseInsensitive: true)
                .AndSkip(Literals.Char(':'))
                .And(op.Parser)
                    .Then<TermNode>(static x => new NamedTermNode(x.Item1, x.Item2));

            var defaultParser = op.Parser.Then<TermNode>(x => new DefaultTermNode(Name, x));

            var parser = termParser.Or(defaultParser);

            return (parser, op.TermOption);

        }
    }   
}
