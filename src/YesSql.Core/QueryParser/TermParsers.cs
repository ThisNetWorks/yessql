using Parlot;
using Parlot.Fluent;
using System;
using static Parlot.Fluent.Parsers;

namespace YesSql.Core.QueryParser
{
    public abstract class TermParserBuilder<T> where T : class
    {
        public TermParserBuilder(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public bool OneOrMany { get; }

        public TermQueryOption<T> TermQueryOption { get; internal set; }


        public Parser<TermNode> Parser { get; internal set; }
        public Parser<char> SeperatorParser { get; internal set; }
    }

    // Options here for One, or Many

    public class NamedTermParserBuilder<T> : TermParserBuilder<T> where T : class
    {
        public NamedTermParserBuilder(string name, OperatorParserBuilder<T> operatorParserBuilder) : base(name)
        {
            var operatorParser = operatorParserBuilder.Parser;

            SeperatorParser = Terms.Text(name, caseInsensitive: true)
                .SkipAnd(Literals.Char(':'));

            var parser = Terms.Text(name, caseInsensitive: true)
                .AndSkip(Literals.Char(':'))
                .And(operatorParser)
                    .Then<TermNode>(static x => new NamedTermNode(x.Item1, x.Item2));

            Parser = parser;

            TermQueryOption = operatorParserBuilder.TermQueryOption;
        }
    }

    public class DefaultTermParserBuilder<T> : TermParserBuilder<T> where T : class
    {
        public DefaultTermParserBuilder(string name, OperatorParserBuilder<T> operatorParserBuilder) : base(name)
        {
            var operatorParser = operatorParserBuilder.Parser;

            SeperatorParser = Terms.Text(name, caseInsensitive: true).SkipAnd(Literals.Char(':'))
                .Or(
                    Literals.Char(' ').AndSkip(Literals.WhiteSpace()) // a default term is also seperated by a space
                );

            var termParser = Terms.Text(name, caseInsensitive: true)
                .AndSkip(Literals.Char(':'))
                .And(operatorParser)
                    .Then<TermNode>(static x => new NamedTermNode(x.Item1, x.Item2));

            var defaultParser = operatorParser.Then<TermNode>(x => new DefaultTermNode(name, x));

            var parser = termParser.Or(defaultParser);

            Parser = parser;

            TermQueryOption = operatorParserBuilder.TermQueryOption;
        }
    }
}