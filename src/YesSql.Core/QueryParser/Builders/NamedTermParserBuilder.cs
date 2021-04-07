using Parlot;
using Parlot.Fluent;
using System;
using static Parlot.Fluent.Parsers;

namespace YesSql.Core.QueryParser.Builders
{
    public class NamedTermParserBuilder2<T> : TermParserBuilder<T> where T : class
    {
        public NamedTermParserBuilder2(string name) : base(name)
        {
        }

        public override (Parser<TermNode> Parser, TermOption<T> TermOption) Build()
        {
            var op = _operatorParser.Build();

            var parser = Terms.Text(Name, caseInsensitive: true)
                .AndSkip(Literals.Char(':'))
                .And(op.Parser)
                    .Then<TermNode>(static x => new NamedTermNode(x.Item1, x.Item2));


            return (parser, op.TermOption);

        }                    
    }
}