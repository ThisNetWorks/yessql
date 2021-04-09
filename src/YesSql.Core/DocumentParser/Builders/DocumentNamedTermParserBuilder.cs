using Parlot;
using Parlot.Fluent;
using System;
using YesSql.Core.QueryParser;
using static Parlot.Fluent.Parsers;

namespace YesSql.Core.DocumentParser.Builders
{
    public class DocumentNamedTermParserBuilder<T> : DocumentTermParserBuilder<T> where T : class
    {
        public DocumentNamedTermParserBuilder(string name) : base(name)
        {
        }

        public override (Parser<TermNode> Parser, DocumentTermOption<T> TermOption) Build()
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